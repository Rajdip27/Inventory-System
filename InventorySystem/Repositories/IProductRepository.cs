using InventorySystem.Cmmon;
using InventorySystem.DatabaseContext;
using InventorySystem.Extensions;
using InventorySystem.HealperUnit;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Repositories;

public interface IProductRepository
{
    Task<PaginationModel<Product>> GetAllAsync(string search, int pageNumber, int pageSize);
    Task<Product> GetByIdAsync(int id);
    Task<bool> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<List<SelectListItem>> GetProductDropdownAsync();
    Task<List<SelectListItem>> GetWarehouseDropdownAsync();
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
    public async Task<List<SelectListItem>> GetWarehouseDropdownAsync()
    {
        return await _context.Warehouses
            .Where(x => !x.IsDelete )
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToListAsync();
    }
    public async Task<List<SelectListItem>> GetProductDropdownAsync()
    {
        return await _context.Products
            .Where(x => !x.IsDelete && x.Status)
            .OrderBy(x => x.ProductName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ProductName
            })
            .ToListAsync();
    }
    // GET ALL (PAGINATION + SEARCH)
    public async Task<PaginationModel<Product>> GetAllAsync(string search, int pageNumber, int pageSize)
    {
        try
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Where(x => !x.IsDelete);

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
        catch (Exception )
        {
            // TODO: log exception
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
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }
        catch (Exception )
        {
            // TODO: log exception
            return null;
        }
    }

    // ADD
    public async Task<bool> AddAsync(Product product)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
                return false;

            // Generate UNIQUE barcode if empty
            if (string.IsNullOrWhiteSpace(product.Barcode))
            {
                string barcode;
                do
                {
                    barcode = BarcodeGenerator.Generate();
                }
                while (await _context.Products.AnyAsync(x => x.Barcode == barcode));

                product.Barcode = barcode;
            }

            product.CreatedBy = _user.UserId ?? 0;
            product.CreatedDate = DateTimeOffset.UtcNow;
            product.IsDelete = false;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception )
        {
            // TODO: log exception
            return false;
        }
    }

    // UPDATE
    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            var existing = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == product.Id && !x.IsDelete);

            if (existing == null)
                return false;

            if (!string.IsNullOrWhiteSpace(product.ProductName))
                existing.ProductName = product.ProductName;

            if (!string.IsNullOrWhiteSpace(product.Sku))
                existing.Sku = product.Sku;

            existing.Description = product.Description;
            existing.PurchasePrice = product.PurchasePrice;
            existing.SellingPrice = product.SellingPrice;
            existing.WarrantyMonths = product.WarrantyMonths;
            existing.Status = product.Status;

            existing.ModifiedBy = _user.UserId ?? 0;
            existing.ModifiedDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception )
        {
            // TODO: log exception
            return false;
        }
    }

    // DELETE (SOFT DELETE FIXED)
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (entity == null)
                return false;

            entity.IsDelete = true; // FIXED
            entity.ModifiedBy = _user.UserId ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception )
        {
            // TODO: log exception

            return false;
        }
    }
}