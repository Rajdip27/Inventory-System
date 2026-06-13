using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface IProductRepository
{
    Task<PaginationModel<Product>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Product> GetByIdAsync(int id);
    Task<bool> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISignInHelper _user;

    public ProductRepository(ApplicationDbContext context, ISignInHelper user)
    {
        _context = context;
        _user = user;
    }

    // GET ALL (PAGINATION + SEARCH)
    public async Task<PaginationModel<Product>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Where(x => x.Status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(x.ProductName, $"%{search}%") ||
                    EF.Functions.Like(x.Sku, $"%{search}%") ||
                    EF.Functions.Like(x.Barcode, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationModel<Product>
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch
        {
            return new PaginationModel<Product>
            {
                Items = new List<Product>(),
                TotalItems = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }

    // GET BY ID
    public async Task<Product> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id && x.Status);
        }
        catch
        {
            return null;
        }
    }

    // ADD
    public async Task<bool> AddAsync(Product product)
    {
        try
        {
            product.CreatedBy = _user.UserId ?? 0;
            product.CreatedDate = DateTimeOffset.UtcNow;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    // UPDATE
    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            var existing = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == product.Id && x.Status);

            if (existing == null)
                return false;

            existing.ProductName = product.ProductName;
            existing.Sku = product.Sku;
            existing.Barcode = product.Barcode;
            existing.Description = product.Description;
            existing.PurchasePrice = product.PurchasePrice;
            existing.SellingPrice = product.SellingPrice;
            existing.WarrantyMonths = product.WarrantyMonths;

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

    // DELETE (SOFT DELETE STYLE LIKE YOUR CUSTOMER)
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id && x.Status);

            if (entity == null)
                return false;

            entity.Status = false; // soft delete
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