using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;
public class ProductController : Controller
{
    private readonly IProductRepository _repo;
    private readonly IAppLogger<ProductController> _logger;
    public ProductController(
        IProductRepository repo,
        IAppLogger<ProductController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // LIST
    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"Product Index called | search: {search}, page: {page}, pageSize: {pageSize}");

        var pagination = await _repo.GetAllAsync(search, page, pageSize);

        _logger.LogInfo($"Product Index loaded | total items: {pagination?.TotalItems}");

        return View(pagination);
    }

    // GET CREATE / EDIT
    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        _logger.LogInfo($"CreateAndEdit GET called | id: {id}");

        if (id == null || id == 0)
        {
            _logger.LogInfo("Opening Create new Product form");
            return View(new Product());
        }

        var product = await _repo.GetByIdAsync((int)id);

        if (product == null)
        {
            _logger.LogWarning($"Product not found | id: {id}");
            TempData["Error"] = "Product not found!";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInfo($"Editing Product | id: {id}");

        return View(product);
    }

    // POST CREATE / EDIT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(Product model)
    {
        _logger.LogInfo($"CreateAndEdit POST called | id: {model?.Id}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState invalid in Product CreateAndEdit POST");
            return View(model);
        }

        bool result;

        try
        {
            if (model.Id == 0)
            {
                _logger.LogInfo("Creating new Product");
                result = await _repo.AddAsync(model);
                if (result)
                    _logger.LogInfo($"Product created successfully | name: {model?.ProductName}");
                else
                    _logger.LogError("Failed to create product", null);
            }
            else
            {
                _logger.LogInfo($"Updating Product | id: {model.Id}");
                result = await _repo.UpdateAsync(model);
                if (result)
                    _logger.LogInfo($"Product updated successfully | id: {model.Id}");
                else
                    _logger.LogError("Product update failed", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in Product CreateAndEdit POST", ex);
            TempData["Error"] = "Unexpected error occurred!";
            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Product saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInfo($"Delete Product called | id: {id}");

        try
        {
            var result = await _repo.DeleteAsync(id);

            if (result)
                _logger.LogInfo($"Product deleted successfully | id: {id}");
            else
                _logger.LogWarning($"Product delete failed | id: {id}");
            TempData[result ? "Success" : "Error"] =
                result ? "Product deleted successfully!" : "Product not found!";
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in Product Delete action", ex);
            TempData["Error"] = "Unexpected error occurred!";
        }
        return RedirectToAction(nameof(Index));
    }
}