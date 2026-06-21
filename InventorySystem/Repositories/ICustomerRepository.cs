using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface ICustomerRepository
{
    Task<PaginationModel<Customer>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Customer> GetByIdAsync(int id);
    Task<bool> AddAsync(Customer customer);
    Task<bool> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
    Task<List<SelectListItem>> GetCustomerDropdownAsync();
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
    public async Task<List<SelectListItem>> GetCustomerDropdownAsync()
    {
        return await _context.Customers
            .Where(x => !x.IsDelete)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToListAsync();
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
                    EF.Functions.Like(x.Name, $"%{search}%") ||
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
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            customer.CreatedBy = _user.UserId ?? 0;
            customer.CreatedDate = DateTimeOffset.UtcNow;
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            if (customer.OpeningBalance != 0)
            {
                var ledger = new CustomerLedger
                {
                    CustomerId = customer.Id,
                    TransactionDate = DateTime.Now,
                    ReferenceType = "Opening",
                    ReferenceId = customer.Id,
                    Description = "Customer Opening Balance",

                    OpeningBalance = 0,
                    Debit = customer.OpeningBalance,
                    Credit = 0,
                    ClosingBalance = customer.OpeningBalance,

                    CreatedBy = _user.UserId ?? 0,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await _context.CustomerLedgers.AddAsync(ledger);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existing = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id && !x.IsDelete);

            if (existing == null)
                return false;

            existing.Name = customer.Name;
            existing.Phone = customer.Phone;
            existing.Email = customer.Email;
            existing.Address = customer.Address;
            existing.OpeningBalance = customer.OpeningBalance;

            existing.ModifiedBy = _user.UserId ?? 0;
            existing.ModifiedDate = DateTimeOffset.UtcNow;

            var openingLedger = await _context.CustomerLedgers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.Id &&
                    x.ReferenceType == "Opening");

            if (openingLedger != null)
            {
                openingLedger.Debit = customer.OpeningBalance;
                openingLedger.Credit = 0;
                openingLedger.ClosingBalance = customer.OpeningBalance;

                openingLedger.ModifiedBy = _user.UserId ?? 0;
                openingLedger.ModifiedDate = DateTimeOffset.UtcNow;
            }
            else if (customer.OpeningBalance > 0)
            {
                await _context.CustomerLedgers.AddAsync(new CustomerLedger
                {
                    CustomerId = customer.Id,
                    TransactionDate = DateTime.Now,
                    ReferenceType = "Opening",
                    ReferenceId = customer.Id,
                    Description = "Customer Opening Balance",

                    OpeningBalance = 0,
                    Debit = customer.OpeningBalance,
                    Credit = 0,
                    ClosingBalance = customer.OpeningBalance,

                    CreatedBy = _user.UserId ?? 0,
                    CreatedDate = DateTimeOffset.UtcNow
                });
            }

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

    public async Task<bool> DeleteAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customer == null)
                return false;

            customer.IsDelete = true;
            customer.ModifiedBy = _user.UserId ?? 0;
            customer.ModifiedDate = DateTimeOffset.UtcNow;

            // Customer Ledger
            var ledgers = await _context.CustomerLedgers
                .Where(x => x.CustomerId == id && !x.IsDelete)
                .ToListAsync();

            ledgers.ForEach(x =>
            {
                x.IsDelete = true;
                x.ModifiedBy = _user.UserId ?? 0;
                x.ModifiedDate = DateTimeOffset.UtcNow;
            });

            // Customer Payments
            var payments = await _context.CustomerPayments
                .Where(x => x.CustomerId == id && !x.IsDelete)
                .ToListAsync();

            payments.ForEach(x =>
            {
                x.IsDelete = true;
                x.ModifiedBy = _user.UserId ?? 0;
                x.ModifiedDate = DateTimeOffset.UtcNow;
            });

            // Warranties
            var warranties = await _context.Warranties
                .Where(x => x.CustomerId == id && !x.IsDelete)
                .ToListAsync();

            warranties.ForEach(x =>
            {
                x.IsDelete = true;
                x.ModifiedBy = _user.UserId ?? 0;
                x.ModifiedDate = DateTimeOffset.UtcNow;
            });

            // Warranty Claims
            var claims = await _context.WarrantyClaims
                .Where(x => x.CustomerId == id && !x.IsDelete)
                .ToListAsync();

            claims.ForEach(x =>
            {
                x.IsDelete = true;
                x.ModifiedBy = _user.UserId ?? 0;
                x.ModifiedDate = DateTimeOffset.UtcNow;
            });

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