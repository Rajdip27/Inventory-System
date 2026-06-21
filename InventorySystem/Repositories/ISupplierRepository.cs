using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface ISupplierRepository
{
    Task<PaginationModel<Supplier>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Supplier> GetByIdAsync(int id);
    Task<bool> AddAsync(Supplier supplier);
    Task<bool> UpdateAsync(Supplier supplier);
    Task<bool> DeleteAsync(int id);
    Task<List<SelectListItem>> GetSupplierDropdownAsync();

}
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ISignInHelper _user;

        public SupplierRepository(
            ApplicationDbContext context,
            ISignInHelper user)
        {
            _context = context;
            _user = user;
        }
        public async Task<List<SelectListItem>> GetSupplierDropdownAsync()
        {
            return await _context.Suppliers
                .Where(x => !x.IsDelete)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }

        public async Task<PaginationModel<Supplier>> GetAllAsync(
        string search,
        int pageNumber,
        int pageSize)
        {
            try
            {
                IQueryable<Supplier> query = _context.Suppliers
                    .AsNoTracking()
                    .Where(x => !x.IsDelete);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(x.Name, $"%{search}%") ||
                        EF.Functions.Like(x.CompanyName, $"%{search}%") ||
                        EF.Functions.Like(x.ContactPerson, $"%{search}%") ||
                        EF.Functions.Like(x.Phone, $"%{search}%") ||
                        EF.Functions.Like(x.Email, $"%{search}%"));
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginationModel<Supplier>
                {
                    Items = items,
                    TotalItems = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch
            {
                return new PaginationModel<Supplier>
                {
                    Items = new List<Supplier>(),
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Suppliers
                    .Include(x => x.SupplierLedgers)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddAsync(Supplier supplier)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                supplier.CreatedBy = _user.UserId ?? 0;
                supplier.CreatedDate = DateTimeOffset.UtcNow;

                await _context.Suppliers.AddAsync(supplier);
                await _context.SaveChangesAsync();

                if (supplier.OpeningBalance > 0)
                {
                    await _context.SupplierLedgers.AddAsync(new SupplierLedger
                    {
                        SupplierId = supplier.Id,
                        TransactionDate = DateTime.Now,

                        ReferenceType = "Opening",
                        ReferenceId = supplier.Id,
                        Description = "Supplier Opening Balance",

                        OpeningBalance = 0,
                        Debit = 0,
                        Credit = supplier.OpeningBalance,
                        ClosingBalance = supplier.OpeningBalance,

                        CreatedBy = _user.UserId ?? 0,
                        CreatedDate = DateTimeOffset.UtcNow
                    });

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

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _context.Suppliers
                    .FirstOrDefaultAsync(x => x.Id == supplier.Id && !x.IsDelete);

                if (existing == null)
                    return false;

                existing.Name = supplier.Name;
                existing.Phone = supplier.Phone;
                existing.Email = supplier.Email;
                existing.Address = supplier.Address;
                existing.CompanyName = supplier.CompanyName;
                existing.ContactPerson = supplier.ContactPerson;
                existing.TradeLicense = supplier.TradeLicense;
                existing.TIN = supplier.TIN;
                existing.BIN = supplier.BIN;
                existing.OpeningBalance = supplier.OpeningBalance;

                existing.ModifiedBy = _user.UserId ?? 0;
                existing.ModifiedDate = DateTimeOffset.UtcNow;

                var openingLedger = await _context.SupplierLedgers
                    .FirstOrDefaultAsync(x =>
                        x.SupplierId == supplier.Id &&
                        x.ReferenceType == "Opening" &&
                        !x.IsDelete);

                if (openingLedger != null)
                {
                    openingLedger.Credit = supplier.OpeningBalance;
                    openingLedger.ClosingBalance = supplier.OpeningBalance;

                    openingLedger.ModifiedBy = _user.UserId ?? 0;
                    openingLedger.ModifiedDate = DateTimeOffset.UtcNow;
                }
                else if (supplier.OpeningBalance > 0)
                {
                    await _context.SupplierLedgers.AddAsync(new SupplierLedger
                    {
                        SupplierId = supplier.Id,
                        TransactionDate = DateTime.Now,

                        ReferenceType = "Opening",
                        ReferenceId = supplier.Id,
                        Description = "Supplier Opening Balance",

                        OpeningBalance = 0,
                        Debit = 0,
                        Credit = supplier.OpeningBalance,
                        ClosingBalance = supplier.OpeningBalance,

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
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

                if (supplier == null)
                    return false;

                supplier.IsDelete = true;
                supplier.ModifiedBy = _user.UserId ?? 0;
                supplier.ModifiedDate = DateTimeOffset.UtcNow;

                var ledgers = await _context.SupplierLedgers
                    .Where(x => x.SupplierId == id && !x.IsDelete)
                    .ToListAsync();

                foreach (var ledger in ledgers)
                {
                    ledger.IsDelete = true;
                    ledger.ModifiedBy = _user.UserId ?? 0;
                    ledger.ModifiedDate = DateTimeOffset.UtcNow;
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
    }
