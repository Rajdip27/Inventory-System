using InventorySystem.Cmmon;
using InventorySystem.Logging;
using InventorySystem.Repositories;
using InventorySystem.ViewModel;
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
        ViewBag.Product = await _repo.ProductDropdwon();
        ViewBag.Supplier = await _repo.SupplierDropdwon();
        _logger.LogInfo($"CreateAndEdit GET called | id: {id}");

        if (id == null || id == 0)
        {
            _logger.LogInfo("Opening Create Purchase form");
            var model = new PurchaseCreateDto
            {
                InvoiceNo = InvoiceGenerator.GenerateInvoiceNo()
            };
            return View(model);
        }

        var purchase = await _repo.GetByIdAsync(id.Value);

        if (purchase == null)
        {
            _logger.LogWarning($"Purchase not found | id: {id}");
            TempData["Error"] = "Purchase not found!";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInfo($"Editing Purchase | id: {id}");

        var dto = new PurchaseCreateDto
        {
            Id = purchase.Id, // Required

            InvoiceNo = purchase.InvoiceNo,
            SupplierId = purchase.SupplierId,
            PurchaseDate = purchase.PurchaseDate,
            Discount = purchase.Discount,
            TransportCost = purchase.TransportCost,
            Note = purchase.Note,

            Items = purchase.StockEntries.Select(x => new PurchaseItemDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                PurchasePrice = x.PurchasePrice,
                SalePrice = x.SalePrice,
                Discount = x.Discount,
                TaxAmount = x.TaxAmount,
                TransportCost = x.TransportCost,
                BatchNo = x.BatchNo,
                ExpiryDate = x.ExpiryDate
            }).ToList()
        };

        return View(dto);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(long? id, PurchaseCreateDto model)
    {
        _logger.LogInfo($"CreateAndEdit POST called | id: {id}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState invalid in Purchase CreateAndEdit");
            return View(model);
        }

        bool result;

        try
        {
            if (id == null || id == 0)
            {
                _logger.LogInfo("Creating new Purchase");

                result = await _repo.AddAsync(model);

                if (result)
                    _logger.LogInfo($"Purchase created | invoice: {model.InvoiceNo}");
                else
                    _logger.LogError("Purchase create failed", null);
            }
            else
            {
                _logger.LogInfo($"Updating Purchase | id: {id}");

                result = await _repo.UpdateAsync(model);

                if (result)
                    _logger.LogInfo($"Purchase updated | id: {id}");
                else
                    _logger.LogError("Purchase update failed", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in Purchase CreateAndEdit POST", ex);
            TempData["Error"] = "Unexpected error occurred!";
            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Purchase saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }

 
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInfo($"Purchase Details called | id: {id}");

        var purchase = await _repo.GetByIdAsync(id);

        if (purchase == null)
        {
            _logger.LogWarning($"Purchase not found | id: {id}");
            TempData["Error"] = "Purchase not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(purchase);
    }
}