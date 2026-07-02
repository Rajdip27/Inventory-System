using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class StockLedgerController : Controller
{
    private readonly IStockLedgerRepository _ledgerRepo;
    private readonly IProductRepository _productRepo;

    private readonly IAppLogger<StockLedgerController> _logger;

    public StockLedgerController(
        IStockLedgerRepository ledgerRepo,
        IProductRepository productRepo,
        IAppLogger<StockLedgerController> logger)
    {
        _ledgerRepo = ledgerRepo;
        _productRepo = productRepo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        long? productId,
        long? warehouseId,
        DateTime? startDate,
        DateTime? endDate)
    {
        _logger.LogInfo(
            $"Stock Ledger Index | Product:{productId} | Warehouse:{warehouseId} | Start:{startDate} | End:{endDate}");

        try
        {
            ViewBag.Products = await _productRepo.GetProductDropdownAsync();
            ViewBag.Warehouses = await _productRepo.GetWarehouseDropdownAsync();
            ViewBag.ProductId = productId;
            ViewBag.WarehouseId = warehouseId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // First page load
            if (!productId.HasValue &&
                !warehouseId.HasValue &&
                !startDate.HasValue &&
                !endDate.HasValue)
            {
                return View(new List<StockLedger>());
            }

            // Product is required
            if (!productId.HasValue)
            {
                TempData["Error"] = "Please select a product.";
                return View(new List<StockLedger>());
            }

            var ledger = await _ledgerRepo.GetLedgerAsync(
                productId,
                warehouseId,
                startDate,
                endDate);

            _logger.LogInfo($"Stock Ledger Loaded | Total Records: {ledger.Count}");

            return View(ledger);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while loading Stock Ledger.", ex);

            TempData["Error"] = "Something went wrong.";

            return View(new List<StockLedger>());
        }
    }
}