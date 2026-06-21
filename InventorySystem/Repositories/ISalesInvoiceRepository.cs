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
    Task<bool> AddAsync(SalesInvoice model);
    Task<bool> UpdateAsync(SalesInvoice model);
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
    public async Task<PaginationModel<SalesInvoice>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
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

            // optional calculation fix
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
        catch
        {
            return new PaginationModel<SalesInvoice>
            {
                Items = new List<SalesInvoice>(),
                TotalItems = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
    public async Task<SalesInvoice> GetByIdAsync(long id)
    {
        return await _context.SalesInvoices
            .Include(x => x.Customer)
            .Include(x => x.SalesItem)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
    }
    public async Task<bool> AddAsync(SalesInvoice model)
    {
        using var trx = await _context.Database.BeginTransactionAsync();

        try
        {
            model.CreatedBy = _user.UserId ?? 0;
            model.CreatedDate = DateTimeOffset.UtcNow;

            await _context.SalesInvoices.AddAsync(model);
            await _context.SaveChangesAsync();
            decimal subtotal = 0;

            foreach (var item in model.SalesItem)
            {
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

            decimal grandTotal =
                subtotal + model.Tax + model.Vat - model.Discount;

            decimal due = grandTotal - model.PaidAmount;

            await _context.CustomerLedgers.AddAsync(new CustomerLedger
            {
                CustomerId = model.CustomerId,
                TransactionDate = DateTime.Now,
                ReferenceType = "Invoice",
                ReferenceId = model.Id,
                Description = "Sales Invoice",
                Debit = grandTotal,
                Credit = model.PaidAmount,
                ClosingBalance = due
            });

            model.GrandTotal = grandTotal;
            model.DueAmount = due;

            await _context.SaveChangesAsync();
            await trx.CommitAsync();

            return true;
        }
        catch
        {
            await trx.RollbackAsync();
            return false;
        }
    }
    public async Task<bool> UpdateAsync(SalesInvoice model)
    {
        using var trx = await _context.Database.BeginTransactionAsync();

        try
        {
            var existing = await _context.SalesInvoices
                .Include(x => x.SalesItem)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (existing == null)
                return false;
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
                decimal line = item.Quantity * item.UnitPrice;
                subtotal += line;
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
            decimal grandTotal =
                subtotal + model.Tax + model.Vat - model.Discount;
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
                ledger.ClosingBalance = due;
            }
            await _context.SaveChangesAsync();
            await trx.CommitAsync();
            return true;
        }
        catch
        {
            await trx.RollbackAsync();
            return false;
        }
    }
}