using InventorySystem.DatabaseContext;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface IStockLedgerRepository
{
    Task<List<StockLedger>> GetLedgerAsync(
        long? productId,
        long? warehouseId,
        DateTime? startDate,
        DateTime? endDate);
}

public class StockLedgerRepository : IStockLedgerRepository
{
    private readonly ApplicationDbContext _context;

    public StockLedgerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockLedger>> GetLedgerAsync(
        long? productId,
        long? warehouseId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _context.StockLedgers
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .AsQueryable();

        // Product Filter
        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        // Warehouse Filter
        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        // Opening Balance
        decimal openingBalance = 0;

        if (startDate.HasValue)
        {
            openingBalance = await query
                .Where(x => x.TransactionDate < startDate.Value.Date)
                .Select(x => x.StockIn - x.StockOut)
                .DefaultIfEmpty(0)
                .SumAsync();

            query = query.Where(x => x.TransactionDate >= startDate.Value.Date);
        }

        // End Date
        if (endDate.HasValue)
        {
            query = query.Where(x => x.TransactionDate <= endDate.Value.Date);
        }

        var ledger = await query
            .OrderBy(x => x.TransactionDate)
            .ThenBy(x => x.Id)
            .ToListAsync();

        // Running Balance
        decimal balance = openingBalance;

        foreach (var item in ledger)
        {
            balance += item.StockIn;
            balance -= item.StockOut;

            item.BalanceQty = balance;
        }

        return ledger;
    }
}