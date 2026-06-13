using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;
public class SupplierController : Controller
{
    private readonly ISupplierRepository _repo;
    private readonly IAppLogger<SupplierController> _logger;

    public SupplierController(
        ISupplierRepository repo,
        IAppLogger<SupplierController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"Supplier Index called | search: {search}, page: {page}, pageSize: {pageSize}");

        var pagination = await _repo.GetAllAsync(search, page, pageSize);

        _logger.LogInfo($"Supplier Index loaded | total items: {pagination?.TotalItems}");

        return View(pagination);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        _logger.LogInfo($"CreateAndEdit GET called | id: {id}");

        if (id == null || id == 0)
        {
            _logger.LogInfo("Opening Create new Supplier form");
            return View(new Supplier());
        }

        var supplier = await _repo.GetByIdAsync((int)id);

        if (supplier == null)
        {
            _logger.LogWarning($"Supplier not found | id: {id}");

            TempData["Error"] = "Supplier not found!";

            return RedirectToAction(nameof(Index));
        }

        _logger.LogInfo($"Editing Supplier | id: {id}");

        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(Supplier model)
    {
        _logger.LogInfo($"CreateAndEdit POST called | id: {model?.Id}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState invalid in CreateAndEdit POST");
            return View(model);
        }

        bool result;

        try
        {
            if (model.Id == 0)
            {
                _logger.LogInfo("Creating new Supplier");

                result = await _repo.AddAsync(model);

                if (result)
                    _logger.LogInfo($"Supplier created successfully | Name: {model.Name}");
                else
                    _logger.LogError("Failed to create supplier", null);
            }
            else
            {
                _logger.LogInfo($"Updating Supplier | id: {model.Id}");

                result = await _repo.UpdateAsync(model);

                if (result)
                    _logger.LogInfo($"Supplier updated successfully | id: {model.Id}");
                else
                    _logger.LogError("Supplier update failed", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred in CreateAndEdit POST", ex);

            TempData["Error"] = "Unexpected error occurred!";

            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Supplier saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInfo($"Delete called | id: {id}");

        try
        {
            var result = await _repo.DeleteAsync(id);

            if (result)
                _logger.LogInfo($"Supplier deleted successfully | id: {id}");
            else
                _logger.LogWarning($"Supplier delete failed | id: {id}");

            TempData[result ? "Success" : "Error"] =
                result ? "Supplier deleted successfully!" : "Supplier not found!";
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in Delete action", ex);

            TempData["Error"] = "Unexpected error occurred!";
        }

        return RedirectToAction(nameof(Index));
    }
}
