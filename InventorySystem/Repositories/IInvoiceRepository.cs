using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using InventorySystem.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface IInvoiceRepository
{
    Task<PaginationModel<Invoice>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Invoice> GetByIdAsync(long id);
    Task<bool> AddAsync(InvoiceCreateDto dto);
    Task<bool> UpdateAsync(InvoiceCreateDto dto);
    Task<IEnumerable<SelectListItem>> ProductDropdwon();
    Task<IEnumerable<SelectListItem>> CustomerDropdwon();
}
public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISignInHelper _user;

    public InvoiceRepository(ApplicationDbContext context, ISignInHelper user)
    {
        _context = context;
        _user = user;
    }
    public async Task<PaginationModel<Invoice>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
        {
            IQueryable<Invoice> query = _context.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x =>
                    EF.Functions.Like(x.InvoiceNo, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(x => x.Customer)
                .Include(x => x.InvoiceItems)
                    .ThenInclude(x => x.Product)
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationModel<Invoice>
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch
        {
            return new PaginationModel<Invoice>();
        }
    }
    public async Task<Invoice> GetByIdAsync(long id)
    {
        try
        {
            return await _context.Invoices
                .Include(x => x.Customer)
                .Include(x => x.InvoiceItems)
                    .ThenInclude(x => x.Product)
                .Include(x => x.Warranty)
                .Include(x => x.WarrantyClaim)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }
        catch
        {
            return null;
        }
    }
    public async Task<bool> AddAsync(InvoiceCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var invoice = new Invoice
            {
                InvoiceNo = dto.InvoiceNo,
                CustomerId = dto.CustomerId,
                UserId = _user.UserId ?? 0,
                InvoiceDate = dto.InvoiceDate,
                PaidAmount = dto.PaidAmount,
                Notes = dto.Notes,
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = _user.UserId ?? 0
            };
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            var items = new List<InvoiceItem>();
            var ledgers = new List<StockLedger>();
            var warranties = new List<Warranty>();
            decimal totalSelling = 0;
            decimal totalPurchase = 0;
            decimal totalTax = 0;
            foreach (var item in dto.Items)
            {
                var product = await _context.StockEntries.FindAsync(item.ProductId);
                if (product == null) continue;
                var invoiceItem = new InvoiceItem
                {
                    InvoiceId = invoice.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SellingPrice = item.SellingPrice,
                    TaxPercent = item.TaxPercent,
                    VatPercent = item.VatPercent,
                    SerialNumber = item.SerialNumber
                };
                invoiceItem.TotalAmount = item.Quantity * item.SellingPrice;
                invoiceItem.TaxAmount = (invoiceItem.TotalAmount * item.TaxPercent) / 100;
                invoiceItem.VatAmount = (invoiceItem.TotalAmount * item.VatPercent) / 100;
                items.Add(invoiceItem);
                totalSelling += invoiceItem.TotalAmount;
                totalTax += invoiceItem.TaxAmount + invoiceItem.VatAmount;
                totalPurchase += item.Quantity * product.PurchasePrice;
                product.Quantity -= item.Quantity;

                ledgers.Add(new StockLedger
                {
                    ProductId = item.ProductId,
                    ReferenceType = "Sale",
                    ReferenceId = invoice.Id,
                    QuantityIn = 0,
                    QuantityOut = item.Quantity,
                    UnitCost = product.PurchasePrice,
                    TotalCost = item.Quantity * product.PurchasePrice,
                    Remarks = "Invoice Sale",
                    EntryDate = DateTimeOffset.UtcNow
                });
                if (!string.IsNullOrEmpty(item.SerialNumber))
                {
                    var productinfo = await _context.Products.FindAsync(item.ProductId);
                    int months = productinfo?.WarrantyMonths ?? 12;

                    warranties.Add(new Warranty
                    {
                        Invoice = invoice,
                        InvoiceItem = invoiceItem,
                        CustomerId = dto.CustomerId,
                        ProductId = item.ProductId,
                        WarrantyStartDate = DateTime.Now,
                        WarrantyEndDate = DateTime.Now.AddMonths(months),
                        WarrantyStatus = "active",
                        Notes = "Auto generated"
                    });
                }
            }
            await _context.InvoiceItems.AddRangeAsync(items);
            await _context.StockLedgers.AddRangeAsync(ledgers);
            await _context.Warranties.AddRangeAsync(warranties);
            await _context.SaveChangesAsync();
            invoice.TotalSellingAmount = totalSelling;
            invoice.TotalTaxAmount = totalTax;
            invoice.TotalPurchaseCost = totalPurchase;
            invoice.GrandTotal = totalSelling + totalTax;
            invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;
            invoice.TotalProfit = totalSelling - totalPurchase;

            invoice.PaymentStatus =
                invoice.DueAmount <= 0 ? "paid" :
                invoice.PaidAmount > 0 ? "partial" : "due";

            await _context.SaveChangesAsync();
            decimal lastBalance = await _context.CustomerLedgers
         .Where(x => x.CustomerId == dto.CustomerId)
         .OrderByDescending(x => x.Id)
         .Select(x => x.Balance)
         .FirstOrDefaultAsync();
            var customerLedger = new CustomerLedger
            {
                CustomerId = dto.CustomerId,
                InvoiceId = invoice.Id,
                ReferenceType = "INVOICE",
                Debit = 0,
                Credit = invoice.GrandTotal,
                Balance = lastBalance + invoice.GrandTotal,
                PaymentMethod = "SALE",
                Remarks = $"Invoice {invoice.InvoiceNo}",
                TransactionDate = DateTime.Now
            };

            await _context.CustomerLedgers.AddAsync(customerLedger);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
    public async Task<bool> UpdateAsync(InvoiceCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDelete);
            if (invoice == null)
                return false;
            var oldItems = await _context.InvoiceItems
                .Where(x => x.InvoiceId == invoice.Id)
                .ToListAsync();

            foreach (var old in oldItems)
            {
                var product = await _context.StockEntries
                    .FirstOrDefaultAsync(x => x.Id == old.ProductId);

                if (product != null)
                {
                    product.Quantity += old.Quantity;
                }
            }
            var oldLedger = _context.StockLedgers
                .Where(x => x.ReferenceType == "Sale" && x.ReferenceId == invoice.Id);

            var oldWarranty = _context.Warranties
                .Where(x => x.InvoiceId == invoice.Id);

            var oldCustomerLedger = _context.CustomerLedgers
                .Where(x => x.InvoiceId == invoice.Id);
            _context.InvoiceItems.RemoveRange(oldItems);
            _context.StockLedgers.RemoveRange(oldLedger);
            _context.Warranties.RemoveRange(oldWarranty);
            _context.CustomerLedgers.RemoveRange(oldCustomerLedger);
            await _context.SaveChangesAsync();
            invoice.CustomerId = dto.CustomerId;
            invoice.InvoiceDate = dto.InvoiceDate;
            invoice.PaidAmount = dto.PaidAmount;
            invoice.Notes = dto.Notes;
            invoice.ModifiedDate = DateTimeOffset.UtcNow;
            invoice.ModifiedBy = _user.UserId ?? 0;


            var newItems = new List<InvoiceItem>();
            var ledgers = new List<StockLedger>();
            var warranties = new List<Warranty>();

            decimal totalSelling = 0;
            decimal totalPurchase = 0;
            decimal totalTax = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.StockEntries
                    .FirstOrDefaultAsync(x => x.Id == item.ProductId);
                if (product == null)
                    continue;
                var invoiceItem = new InvoiceItem
                {
                    InvoiceId = invoice.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SellingPrice = item.SellingPrice,
                    TaxPercent = item.TaxPercent,
                    VatPercent = item.VatPercent,
                    SerialNumber = item.SerialNumber
                };
                invoiceItem.TotalAmount = item.Quantity * item.SellingPrice;
                invoiceItem.TaxAmount = (invoiceItem.TotalAmount * item.TaxPercent) / 100;
                invoiceItem.VatAmount = (invoiceItem.TotalAmount * item.VatPercent) / 100;
                newItems.Add(invoiceItem);
                totalSelling += invoiceItem.TotalAmount;
                totalTax += invoiceItem.TaxAmount + invoiceItem.VatAmount;
                totalPurchase += item.Quantity * product.PurchasePrice;
                product.Quantity -= item.Quantity;
                ledgers.Add(new StockLedger
                {
                    ProductId = item.ProductId,
                    ReferenceType = "Sale",
                    ReferenceId = invoice.Id,
                    QuantityIn = 0,
                    QuantityOut = item.Quantity,
                    UnitCost = product.PurchasePrice,
                    TotalCost = item.Quantity * product.PurchasePrice,
                    Remarks = "Invoice Update",
                    EntryDate = DateTimeOffset.UtcNow
                });
                if (!string.IsNullOrEmpty(item.SerialNumber))
                {
                    warranties.Add(new Warranty
                    {
                        InvoiceId = invoice.Id,
                        CustomerId = dto.CustomerId,
                        ProductId = item.ProductId,
                        WarrantyStartDate = DateTime.Now,
                        WarrantyEndDate = DateTime.Now.AddYears(1),
                        WarrantyStatus = "active",
                        Notes = "Updated Invoice"
                    });
                }
            }
            await _context.InvoiceItems.AddRangeAsync(newItems);
            await _context.StockLedgers.AddRangeAsync(ledgers);
            await _context.Warranties.AddRangeAsync(warranties);
            await _context.SaveChangesAsync();
            invoice.TotalSellingAmount = totalSelling;
            invoice.TotalTaxAmount = totalTax;
            invoice.TotalPurchaseCost = totalPurchase;
            invoice.GrandTotal = totalSelling + totalTax;
            invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;
            invoice.TotalProfit = totalSelling - totalPurchase;
            invoice.PaymentStatus =
                invoice.DueAmount <= 0 ? "paid" :
                invoice.PaidAmount > 0 ? "partial" : "due";
            await _context.SaveChangesAsync();
            decimal lastBalance = await _context.CustomerLedgers
                .Where(x => x.CustomerId == dto.CustomerId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();
            var customerLedger = new CustomerLedger
            {
                CustomerId = dto.CustomerId,
                InvoiceId = invoice.Id,
                ReferenceType = "INVOICE",
                Debit = 0,
                Credit = invoice.GrandTotal,
                Balance = lastBalance + invoice.GrandTotal,
                PaymentMethod = "SALE",
                Remarks = $"Updated Invoice {invoice.InvoiceNo}",
                TransactionDate = DateTime.Now
            };
            await _context.CustomerLedgers.AddAsync(customerLedger);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
    public async Task<IEnumerable<SelectListItem>> ProductDropdwon()
    {
        return await _context.Products
            .Where(x => !x.IsDelete)
            .Select(x => new SelectListItem
            {
                Text = x.ProductName,
                Value = x.Id.ToString()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SelectListItem>> CustomerDropdwon()
    {
        return await _context.Customers
            .Where(x => !x.IsDelete)
            .Select(x => new SelectListItem
            {
                Text = x.CustomerName,
                Value = x.Id.ToString()
            })
            .ToListAsync();
    }
}
