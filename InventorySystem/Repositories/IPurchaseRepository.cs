using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using InventorySystem.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface IPurchaseRepository
{
    Task<PaginationModel<Purchase>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Purchase> GetByIdAsync(long id);
    Task<bool> AddAsync(PurchaseCreateDto dto);
    Task<bool> UpdateAsync( PurchaseCreateDto dto);
    Task<IEnumerable<SelectListItem>> ProductDropdwon();
    Task<IEnumerable<SelectListItem>> SupplierDropdwon();
}

public class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISignInHelper _user;
    public PurchaseRepository(ApplicationDbContext context, ISignInHelper user)
    {
        _context = context;
        _user = user;
    }
    public async Task<PaginationModel<Purchase>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
        {
            IQueryable<Purchase> query = _context.Purchases
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
                .Include(x => x.StockEntries)
                .Include(x=>x.Supplier)
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationModel<Purchase>
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception)
        {
            return new PaginationModel<Purchase>
            {
                Items = new List<Purchase>(),
                TotalItems = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
    public async Task<Purchase> GetByIdAsync(long id)
    {
        try
        {
            return await _context.Purchases
                .Include(x => x.StockEntries)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<bool> AddAsync(PurchaseCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var purchase = new Purchase
            {
                InvoiceNo = dto.InvoiceNo,
                SupplierId = dto.SupplierId,
                PurchaseDate = dto.PurchaseDate,
                Discount = dto.Discount,
                TransportCost = dto.TransportCost,
                Note = dto.Note,
                UserId = _user.UserId ?? 0,
                CreatedDate = DateTimeOffset.UtcNow,
                IsDelete = false
            };

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            var stockEntries = new List<StockEntry>();
            var ledgerEntries = new List<StockLedger>();

            foreach (var item in dto.Items)
            {
                var entry = new StockEntry
                {
                    SupplierId = dto.SupplierId,
                    ProductId = item.ProductId,
                    UserId = _user.UserId ?? 0,

                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    SalePrice = item.SalePrice,
                    Discount = item.Discount,
                    TaxAmount = item.TaxAmount,
                    TransportCost = item.TransportCost,

                    InvoiceNo = dto.InvoiceNo,
                    BatchNo = item.BatchNo,
                    ExpiryDate = item.ExpiryDate,
                    Note = dto.Note,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                stockEntries.Add(entry);
            }

            await _context.StockEntries.AddRangeAsync(stockEntries);
            await _context.SaveChangesAsync();

            foreach (var entry in stockEntries)
            {
                ledgerEntries.Add(new StockLedger
                {
                    ProductId = entry.ProductId,
                    ReferenceType = "Purchase",
                    ReferenceId = purchase.Id,
                    StockEntryId = entry.Id,
                    SupplierId = dto.SupplierId,

                    QuantityIn = entry.Quantity,
                    QuantityOut = 0,
                    BalanceQuantity = entry.Quantity,

                    UnitCost = entry.PurchasePrice,
                    TotalCost = entry.TotalCost,
                    Remarks = "Purchase Entry",
                    EntryDate = DateTimeOffset.UtcNow
                });
            }

            await _context.StockLedgers.AddRangeAsync(ledgerEntries);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
    public async Task<bool> UpdateAsync( PurchaseCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDelete);

            if (purchase == null)
                return false;

            purchase.InvoiceNo = dto.InvoiceNo;
            purchase.SupplierId = dto.SupplierId;
            purchase.PurchaseDate = dto.PurchaseDate;
            purchase.Discount = dto.Discount;
            purchase.TransportCost = dto.TransportCost;
            purchase.Note = dto.Note;
            purchase.ModifiedBy = _user.UserId ?? 0;
            purchase.ModifiedDate = DateTimeOffset.UtcNow;

            // remove old stock + ledger
            var oldStock = _context.StockEntries
                .Where(x => x.InvoiceNo == purchase.InvoiceNo);

            var oldLedger = _context.StockLedgers
                .Where(x => x.ReferenceType == "Purchase"
                         && x.ReferenceId == purchase.Id);

            _context.StockEntries.RemoveRange(oldStock);
            _context.StockLedgers.RemoveRange(oldLedger);

            await _context.SaveChangesAsync();

            var stockEntries = new List<StockEntry>();
            var ledgerEntries = new List<StockLedger>();

            foreach (var item in dto.Items)
            {
                var entry = new StockEntry
                {
                    SupplierId = dto.SupplierId,
                    ProductId = item.ProductId,
                    UserId = _user.UserId ?? 0,

                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    SalePrice = item.SalePrice,
                    Discount = item.Discount,
                    TaxAmount = item.TaxAmount,
                    TransportCost = item.TransportCost,

                    InvoiceNo = dto.InvoiceNo,
                    BatchNo = item.BatchNo,
                    ExpiryDate = item.ExpiryDate,
                    Note = dto.Note,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                stockEntries.Add(entry);
            }

            await _context.StockEntries.AddRangeAsync(stockEntries);
            await _context.SaveChangesAsync();

            foreach (var entry in stockEntries)
            {
                ledgerEntries.Add(new StockLedger
                {
                    ProductId = entry.ProductId,
                    ReferenceType = "Purchase",
                    ReferenceId = purchase.Id,
                    StockEntryId = entry.Id,
                    SupplierId = dto.SupplierId,

                    QuantityIn = entry.Quantity,
                    QuantityOut = 0,
                    BalanceQuantity = entry.Quantity,

                    UnitCost = entry.PurchasePrice,
                    TotalCost = entry.TotalCost,
                    Remarks = "Purchase Updated",
                    EntryDate = DateTimeOffset.UtcNow
                });
            }
            await _context.StockLedgers.AddRangeAsync(ledgerEntries);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<IEnumerable<SelectListItem>> ProductDropdwon()
    {
        var query = _context.Products
                        .Where(x => !x.IsDelete);
        var list = await query
            .Select(x => new SelectListItem
            {
                Text = x.ProductName,
                Value = x.Id.ToString()
            })
            .ToListAsync();
        return list;
    }

    public async Task<IEnumerable<SelectListItem>> SupplierDropdwon()
    {
        var query = _context.Suppliers
                       .Where(x => !x.IsDelete);
        var list = await query
            .Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
            .ToListAsync();
        return list;
    }
}
