using InventorySystem.Cmmon;
using InventorySystem.Logging;
using InventorySystem.Repositories;
using InventorySystem.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class InvoiceController : Controller
{
    private readonly IInvoiceRepository _repo;
    private readonly IAppLogger<InvoiceController> _logger;

    public InvoiceController(
        IInvoiceRepository repo,
        IAppLogger<InvoiceController> logger)
    {
        _repo = repo;
        _logger = logger;
    }
    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"Invoice Index called | search: {search}, page: {page}, pageSize: {pageSize}");

        var pagination = await _repo.GetAllAsync(search, page, pageSize);

        _logger.LogInfo($"Invoice loaded | total: {pagination?.TotalItems}");

        return View(pagination);
    }
    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        ViewBag.Product = await _repo.ProductDropdwon();
        ViewBag.Customer = await _repo.CustomerDropdwon();

        _logger.LogInfo($"Invoice CreateAndEdit GET | id: {id}");

        if (id == null || id == 0)
        {
            var model = new InvoiceCreateDto
            {
                InvoiceNo = InvoiceGenerator.GenerateInvoiceNo(),
                InvoiceDate = DateTime.Now,
                Items = new List<InvoiceItemCreateDto>()
            };

            return View(model);
        }

        var invoice = await _repo.GetByIdAsync(id.Value);

        if (invoice == null)
        {
            _logger.LogWarning($"Invoice not found | id: {id}");
            TempData["Error"] = "Invoice not found!";
            return RedirectToAction(nameof(Index));
        }

        var dto = new InvoiceCreateDto
        {
            Id = invoice.Id,
            InvoiceNo = invoice.InvoiceNo,
            CustomerId = invoice.CustomerId,
            InvoiceDate = invoice.InvoiceDate,
            PaidAmount = invoice.PaidAmount,
            Notes = invoice.Notes,

            Items = invoice.InvoiceItems.Select(x => new InvoiceItemCreateDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                SellingPrice = x.SellingPrice,
                TaxPercent = x.TaxPercent,
                VatPercent = x.VatPercent,
                SerialNumber = x.SerialNumber
            }).ToList()
        };

        return View(dto);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(long? id, InvoiceCreateDto model)
    {
        ViewBag.Product = await _repo.ProductDropdwon();
        ViewBag.Customer = await _repo.CustomerDropdwon();

        _logger.LogInfo($"Invoice POST called | id: {id}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invoice ModelState invalid");
            return View(model);
        }

        bool result;

        try
        {
            if (id == null || id == 0)
            {
                _logger.LogInfo("Creating Invoice");

                result = await _repo.AddAsync(model);

                _logger.LogInfo(result
                    ? $"Invoice created | {model.InvoiceNo}"
                    : "Invoice create failed");
            }
            else
            {
                _logger.LogInfo($"Updating Invoice | id: {id}");

                result = await _repo.UpdateAsync(model);

                _logger.LogInfo(result
                    ? $"Invoice updated | id: {id}"
                    : "Invoice update failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Invoice Create/Edit Exception", ex);
            TempData["Error"] = "Unexpected error occurred!";
            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Invoice saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInfo($"Invoice Details | id: {id}");

        var invoice = await _repo.GetByIdAsync(id);

        if (invoice == null)
        {
            _logger.LogWarning($"Invoice not found | id: {id}");
            TempData["Error"] = "Invoice not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(invoice);
    }
}