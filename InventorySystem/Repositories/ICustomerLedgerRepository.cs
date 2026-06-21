using InventorySystem.DatabaseContext;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface ICustomerLedgerRepository
{
    Task<List<CustomerLedger>> GetLedgerAsync(long? customerId,DateTime? startDate,DateTime? endDate);
}
public class CustomerLedgerRepository(ApplicationDbContext _context) : ICustomerLedgerRepository
{
    public async Task<List<CustomerLedger>> GetLedgerAsync(
        long? customerId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _context.CustomerLedgers
            .Include(x => x.Customer)
            .AsQueryable();
        if (customerId.HasValue)
            query = query.Where(x =>
               x.CustomerId == customerId.Value);

        if (startDate.HasValue)
            query = query.Where(x =>
                x.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
        {
            var endDateTime = endDate.Value
                .Date
                .AddDays(1)
                .AddTicks(-1);

            query = query.Where(x =>
                x.TransactionDate <= endDateTime);
        }
        return await query
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync();
    }
}