using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface ISalesInvoiceRepository
{
    Task<PaginationModel<SalesInvoice>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<SalesInvoice> GetByIdAsync(long id);

    Task<(bool success, string message)> AddAsync(SalesInvoice model);
    Task<(bool success, string message)> UpdateAsync(SalesInvoice model);
}

public class SalesInvoiceRepository : ISalesInvoiceRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISignInHelper _user;

    public SalesInvoiceRepository(ApplicationDbContext context, ISignInHelper user)
    {
        _context = context;
        _user = user;
    }

    #region GET ALL
    public async Task<PaginationModel<SalesInvoice>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        IQueryable<SalesInvoice> query = _context.SalesInvoices
            .Include(x => x.Customer)
            .Include(x => x.SalesItem)
            .Where(x => !x.IsDelete)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.InvoiceNo.Contains(search) ||
                x.Customer.Name.Contains(search) ||
                x.Customer.Phone.Contains(search));
        }

        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        data.ForEach(x =>
        {
            x.GrandTotal =
                x.SalesItem.Sum(i => i.Quantity * i.UnitPrice)
                + x.Tax + x.Vat - x.Discount;

            x.DueAmount = x.GrandTotal - x.PaidAmount;
        });

        return new PaginationModel<SalesInvoice>
        {
            Items = data,
            TotalItems = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
    #endregion

    #region GET BY ID
    public async Task<SalesInvoice> GetByIdAsync(long id)
    {
        return await _context.SalesInvoices
            .Include(x => x.Customer)
            .Include(x => x.SalesItem)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
    }
    #endregion

    #region ADD
    public async Task<(bool success, string message)> AddAsync(SalesInvoice model)
    {
        using var trx = await _context.Database.BeginTransactionAsync();

        try
        {
            if (model == null)
                return (false, "Invoice data is required");

            if (model.SalesItem == null || !model.SalesItem.Any())
                return (false, "At least one product is required");

            model.CreatedBy = _user.UserId ?? 0;
            model.CreatedDate = DateTimeOffset.UtcNow;

            decimal subtotal = 0;

            var items = model.SalesItem.ToList();
            model.SalesItem = new List<SalesItem>();

            await _context.SalesInvoices.AddAsync(model);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                if (item.ProductId <= 0)
                    return (false, "Invalid product selected");

                if (item.Quantity <= 0)
                    return (false, "Quantity must be greater than 0");

                // STOCK CHECK
                var stock = await _context.StockLedgers
                    .Where(x => x.ProductId == item.ProductId)
                    .SumAsync(x => x.StockIn - x.StockOut);

                if (stock < item.Quantity)
                    return (false, $"Insufficient stock for ProductId {item.ProductId}");

                item.Id = 0;
                item.SalesInvoiceId = model.Id;

                decimal line = item.Quantity * item.UnitPrice;
                subtotal += line;

                await _context.SalesItems.AddAsync(item);

                await _context.StockLedgers.AddAsync(new StockLedger
                {
                    ProductId = item.ProductId,
                    WarehouseId = 1,
                    TransactionDate = DateTime.Now,
                    ReferenceType = "Sale",
                    ReferenceId = model.Id,
                    StockIn = 0,
                    StockOut = item.Quantity,
                    UnitCost = item.UnitPrice
                });

                var product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == item.ProductId);

                if (product != null && product.WarrantyMonths > 0)
                {
                    var warranty = new Warranty
                    {
                        WarrantyNo = "WR-" + DateTime.Now.Ticks,
                        CustomerId = model.CustomerId,
                        SalesInvoiceId = model.Id,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(product.WarrantyMonths),
                        Status = "Active"
                    };

                    await _context.Warranties.AddAsync(warranty);
                    await _context.SaveChangesAsync();

                    await _context.WarrantyItems.AddAsync(new WarrantyItem
                    {
                        WarrantyId = warranty.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        WarrantyMonths = product.WarrantyMonths,
                        StartDate = warranty.StartDate,
                        EndDate = warranty.EndDate,
                        Status = "Active"
                    });
                }
            }

            decimal grandTotal = subtotal + model.Tax + model.Vat - model.Discount;
            decimal due = grandTotal - model.PaidAmount;

            model.GrandTotal = grandTotal;
            model.DueAmount = due;

            await _context.SaveChangesAsync();

            var lastBalance = await _context.CustomerLedgers
                .Where(x => x.CustomerId == model.CustomerId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.ClosingBalance)
                .FirstOrDefaultAsync();

            decimal openingblanc = lastBalance;
            decimal closing = openingblanc + grandTotal - model.PaidAmount;

            await _context.CustomerLedgers.AddAsync(new CustomerLedger
            {
                CustomerId = model.CustomerId,
                TransactionDate = DateTime.Now,
                ReferenceType = "Invoice",
                ReferenceId = model.Id,
                Description = "Sales Invoice",
                OpeningBalance = openingblanc,
                Debit = grandTotal,
                Credit = model.PaidAmount,
                ClosingBalance = closing
            });

            await _context.SaveChangesAsync();
            await trx.CommitAsync();

            return (true, "Invoice created successfully");
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync();
            return (false, ex.Message);
        }
    }
    #endregion

    #region UPDATE
    public async Task<(bool success, string message)> UpdateAsync(SalesInvoice model)
    {
        using var trx = await _context.Database.BeginTransactionAsync();

        try
        {
            var existing = await _context.SalesInvoices
                .Include(x => x.SalesItem)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (existing == null)
                return (false, "Invoice not found");

            existing.InvoiceNo = model.InvoiceNo;
            existing.CustomerId = model.CustomerId;
            existing.InvoiceDate = model.InvoiceDate;
            existing.Discount = model.Discount;
            existing.Tax = model.Tax;
            existing.Vat = model.Vat;
            existing.PaidAmount = model.PaidAmount;
            existing.ModifiedBy = _user.UserId ?? 0;
            existing.ModifiedDate = DateTimeOffset.UtcNow;

            _context.SalesItems.RemoveRange(existing.SalesItem);

            decimal subtotal = 0;

            foreach (var item in model.SalesItem)
            {
                if (item.Quantity <= 0)
                    return (false, "Invalid quantity");

                decimal line = item.Quantity * item.UnitPrice;
                subtotal += line;

                item.Id = 0;
                item.SalesInvoiceId = existing.Id;

                await _context.SalesItems.AddAsync(item);

                await _context.StockLedgers.AddAsync(new StockLedger
                {
                    ProductId = item.ProductId,
                    WarehouseId = 1,
                    TransactionDate = DateTime.Now,
                    ReferenceType = "Sale",
                    ReferenceId = existing.Id,
                    StockIn = 0,
                    StockOut = item.Quantity,
                    UnitCost = item.UnitPrice
                });
            }

            decimal grandTotal = subtotal + model.Tax + model.Vat - model.Discount;
            decimal due = grandTotal - model.PaidAmount;

            existing.GrandTotal = grandTotal;
            existing.DueAmount = due;

            var ledger = await _context.CustomerLedgers
                .FirstOrDefaultAsync(x =>
                    x.ReferenceId == existing.Id &&
                    x.ReferenceType == "Invoice");

            if (ledger != null)
            {
                ledger.Debit = grandTotal;
                ledger.Credit = model.PaidAmount;

                var lastBalance = await _context.CustomerLedgers
                    .Where(x => x.CustomerId == existing.CustomerId)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.ClosingBalance)
                    .FirstOrDefaultAsync();

                ledger.ClosingBalance = lastBalance + grandTotal - model.PaidAmount;
            }

            await _context.SaveChangesAsync();
            await trx.CommitAsync();

            return (true, "Invoice updated successfully");
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync();
            return (false, ex.Message);
        }
    }
    #endregion
}