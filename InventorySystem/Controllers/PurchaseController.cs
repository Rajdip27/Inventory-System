using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class PurchaseController : Controller
{
    private readonly IPurchaseRepository _repo;
    private readonly IAppLogger<PurchaseController> _logger;
    public PurchaseController(
        IPurchaseRepository repo,
        IAppLogger<PurchaseController> logger)
    {
        _repo = repo;
        _logger = logger;
    }
    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"Purchase Index called | search: {search}, page: {page}, pageSize: {pageSize}");

        var pagination = await _repo.GetAllAsync(search, page, pageSize);

        _logger.LogInfo($"Purchase Index loaded | total items: {pagination?.TotalItems}");

        return View(pagination);
    }
    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        _logger.LogInfo($"CreateAndEdit GET called | id: {id}");

        if (id == null || id == 0)
        {
            _logger.LogInfo("Opening Create Purchase form");
            return View(new Purchase
            {
                PurchaseDate = DateTime.Now,
                PurchaseItem = new List<PurchaseItem>()
            });
        }

        var purchase = await _repo.GetByIdAsync(id.Value);

        if (purchase == null)
        {
            _logger.LogWarning($"Purchase not found | id: {id}");
            TempData["Error"] = "Purchase not found!";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInfo($"Editing Purchase | id: {id}");

        return View(purchase);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(Purchase model)
    {
        _logger.LogInfo($"CreateAndEdit POST called | id: {model?.Id}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState invalid in Purchase POST");
            return View(model);
        }

        bool result = false;

        try
        {
            if (model.Id == 0)
            {
                _logger.LogInfo("Creating new Purchase");

                result = await _repo.AddAsync(model);

                if (result)
                    _logger.LogInfo($"Purchase created | Invoice: {model.InvoiceNo}");
                else
                    _logger.LogError("Purchase create failed", null);
            }
            else
            {
                _logger.LogInfo($"Updating Purchase | id: {model.Id}");

                result = await _repo.UpdateAsync(model);

                if (result)
                    _logger.LogInfo($"Purchase updated | id: {model.Id}");
                else
                    _logger.LogError("Purchase update failed", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in Purchase CreateAndEdit", ex);
            TempData["Error"] = "Unexpected error occurred!";
            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Purchase saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }
}