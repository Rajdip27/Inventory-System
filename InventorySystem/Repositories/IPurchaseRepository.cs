using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
namespace InventorySystem.Repositories;
public interface IPurchaseRepository
{
    Task<PaginationModel<Purchase>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Purchase> GetByIdAsync(long id);
    Task<bool> AddAsync(Purchase purchase);
    Task<bool> UpdateAsync(Purchase purchase);
}
public class PurchaseRepository(ApplicationDbContext _context, ISignInHelper _user) : IPurchaseRepository
{
    public async Task<PaginationModel<Purchase>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
        {
            IQueryable<Purchase> query = _context.Purchases
                .Include(x => x.Supplier)
                .Include(x => x.Warehouse)
                .Include(x => x.PurchaseItem)
                .AsNoTracking()
                .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(x.InvoiceNo, $"%{search}%") ||
                    EF.Functions.Like(x.Supplier.Name, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var items = await query
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
        catch
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
                .Include(x => x.Supplier)
                .Include(x => x.Warehouse)
                .Include(x => x.PurchaseItem)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }
        catch
        {
            return null;
        }
    }
    public async Task<bool> AddAsync(Purchase purchase)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            purchase.CreatedBy = _user.UserId ?? 0;
            purchase.CreatedDate = DateTimeOffset.UtcNow;

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            decimal totalAmount = 0;

            foreach (var item in purchase.PurchaseItem)
            {
                item.PurchaseId = purchase.Id;
                totalAmount += item.Total;
                var stock = new StockLedger
                {
                    ProductId = item.ProductId,
                    WarehouseId = purchase.WarehouseId,
                    TransactionDate = purchase.PurchaseDate,
                    ReferenceType = "Purchase",
                    ReferenceId = purchase.Id,
                    StockIn = item.Quantity,
                    StockOut = 0,
                    UnitCost = item.UnitPrice,
                    BalanceQty = 0,

                    CreatedBy = _user.UserId ?? 0,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await _context.StockLedgers.AddAsync(stock);
            }
            var supplierLedger = new SupplierLedger
            {
                SupplierId = purchase.SupplierId,
                TransactionDate = purchase.PurchaseDate,
                ReferenceType = "Purchase",
                ReferenceId = purchase.Id,
                Description = $"Purchase Invoice {purchase.InvoiceNo}",
                OpeningBalance = 0,
                Debit = 0,
                Credit = totalAmount,
                ClosingBalance = totalAmount,
                CreatedBy = _user.UserId ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await _context.SupplierLedgers.AddAsync(supplierLedger);
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
    public async Task<bool> UpdateAsync(Purchase purchase)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existing = await _context.Purchases
                .Include(x => x.PurchaseItem)
                .FirstOrDefaultAsync(x => x.Id == purchase.Id && !x.IsDelete);

            if (existing == null)
                return false;
            existing.InvoiceNo = purchase.InvoiceNo;
            existing.SupplierId = purchase.SupplierId;
            existing.WarehouseId = purchase.WarehouseId;
            existing.PurchaseDate = purchase.PurchaseDate;
            existing.Discount = purchase.Discount;
            existing.Tax = purchase.Tax;
            existing.Vat = purchase.Vat;
            existing.TransportCost = purchase.TransportCost;
            existing.GrandTotal = purchase.GrandTotal;

            existing.ModifiedBy = _user.UserId ?? 0;
            existing.ModifiedDate = DateTimeOffset.UtcNow;
            _context.PurchaseItems.RemoveRange(existing.PurchaseItem);
            await _context.SaveChangesAsync();
            decimal totalAmount = 0;
            foreach (var item in purchase.PurchaseItem)
            {
                item.PurchaseId = existing.Id;
                totalAmount += item.Total;

                await _context.PurchaseItems.AddAsync(item);
                var stock = new StockLedger
                {
                    ProductId = item.ProductId,
                    WarehouseId = purchase.WarehouseId,
                    TransactionDate = purchase.PurchaseDate,
                    ReferenceType = "Purchase-Update",
                    ReferenceId = existing.Id,
                    StockIn = item.Quantity,
                    StockOut = 0,
                    UnitCost = item.UnitPrice,
                    BalanceQty = 0,

                    CreatedBy = _user.UserId ?? 0,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await _context.StockLedgers.AddAsync(stock);
            }
            var oldLedger = _context.SupplierLedgers
                .Where(x => x.ReferenceId == existing.Id && x.ReferenceType == "Purchase");
            _context.SupplierLedgers.RemoveRange(oldLedger);
            var supplierLedger = new SupplierLedger
            {
                SupplierId = purchase.SupplierId,
                TransactionDate = purchase.PurchaseDate,
                ReferenceType = "Purchase",
                ReferenceId = existing.Id,
                Description = $"Updated Purchase Invoice {purchase.InvoiceNo}",
                OpeningBalance = 0,
                Debit = 0,
                Credit = totalAmount,
                ClosingBalance = totalAmount,
                CreatedBy = _user.UserId ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await _context.SupplierLedgers.AddAsync(supplierLedger);
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
}