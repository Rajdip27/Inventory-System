using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Customer> GetByIdAsync(int id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}
public class CustomerRepository(ApplicationDbContext _context, ISignInHelper _user) : ICustomerRepository
{
    public async Task<PagedResult<Customer>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        IQueryable<Customer> query = _context.Customers
            .AsNoTracking()
            .Where(x => !x.IsDelete);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                EF.Functions.Like(x.CustomerName, $"%{search}%") ||
                EF.Functions.Like(x.Phone, $"%{search}%") ||
                EF.Functions.Like(x.Email, $"%{search}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Customer>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
    public async Task<Customer> GetByIdAsync(int id)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
    }
    public async Task AddAsync(Customer customer)
    {
        customer.CreatedBy = _user.UserId ?? 0;
        customer.CreatedDate = DateTimeOffset.UtcNow;
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Customer customer)
    {
        var existing = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == customer.Id && !x.IsDelete);
        if (existing != null)
        {
            existing.CustomerName = customer.CustomerName;
            existing.Phone = customer.Phone;
            existing.Email = customer.Email;
            existing.Address = customer.Address;
            existing.ModifiedBy = _user.UserId ?? 0;
            existing.ModifiedDate = DateTimeOffset.UtcNow;
            _context.Customers.Update(existing);
            await _context.SaveChangesAsync();
        }
    }
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

        if (entity != null)
        {
            entity.IsDelete = true;
            entity.ModifiedBy = _user.UserId ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;

            _context.Customers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}