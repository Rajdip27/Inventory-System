using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface ICustomerRepository
{
    Task<PaginationModel<Customer>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Customer> GetByIdAsync(int id);
    Task<bool> AddAsync(Customer customer);
    Task<bool> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
}
public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISignInHelper _user;

    public CustomerRepository(ApplicationDbContext context, ISignInHelper user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginationModel<Customer>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
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

            return new PaginationModel<Customer>
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch
        {
            return new PaginationModel<Customer>
            {
                Items = new List<Customer>(),
                TotalItems = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> AddAsync(Customer customer)
    {
        try
        {
            customer.CreatedBy = _user.UserId ?? 0;
            customer.CreatedDate = DateTimeOffset.UtcNow;

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        try
        {
            var existing = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id && !x.IsDelete);

            if (existing == null)
                return false;

            existing.CustomerName = customer.CustomerName;
            existing.Phone = customer.Phone;
            existing.Email = customer.Email;
            existing.Address = customer.Address;
            existing.ModifiedBy = _user.UserId ?? 0;
            existing.ModifiedDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (entity == null)
                return false;

            entity.IsDelete = true;
            entity.ModifiedBy = _user.UserId ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}