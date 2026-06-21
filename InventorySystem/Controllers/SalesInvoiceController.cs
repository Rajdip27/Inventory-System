using InventorySystem.Cmmon;
using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class SalesInvoiceController : Controller
{
    private readonly ISalesInvoiceRepository _repo;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IAppLogger<SalesInvoiceController> _logger;
    public SalesInvoiceController(
        ISalesInvoiceRepository repo,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IAppLogger<SalesInvoiceController> logger)
    {
        _repo = repo;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }
    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"SalesInvoice Index called | search: {search}, page: {page}, pageSize: {pageSize}");

        var pagination = await _repo.GetAllAsync(search, page, pageSize);

        _logger.LogInfo($"SalesInvoice loaded | total: {pagination?.TotalItems}");

        return View(pagination);
    }
    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        _logger.LogInfo($"CreateAndEdit GET | id: {id}");
        ViewBag.Products = await _productRepository.GetProductDropdownAsync();
        ViewBag.Customers = await _customerRepository.GetCustomerDropdownAsync();
        if (id == null || id == 0)
        {
            _logger.LogInfo("Opening Create Sales Invoice form");

            return View(new SalesInvoice
            {
                InvoiceDate = DateTime.Now,
                InvoiceNo = InvoiceGenerator.GenerateInvoiceNo(),
                SalesItem = new List<SalesItem>()
            });
        }
        var invoice = await _repo.GetByIdAsync(id.Value);
        if (invoice == null)
        {
            _logger.LogWarning($"Invoice not found | id: {id}");
            TempData["Error"] = "Invoice not found!";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInfo($"Editing Invoice | id: {id}");

        return View(invoice);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(SalesInvoice model)
    {
        _logger.LogInfo($"CreateAndEdit POST | id: {model?.Id}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState invalid in SalesInvoice POST");
            return View(model);
        }

        bool result = false;

        try
        {
            if (model.Id == 0)
            {
                _logger.LogInfo("Creating Sales Invoice");

                result = await _repo.AddAsync(model);

                if (result)
                    _logger.LogInfo($"Invoice created | {model.InvoiceNo}");
                else
                    _logger.LogError("Invoice create failed", null);
            }
            else
            {
                _logger.LogInfo($"Updating Sales Invoice | id: {model.Id}");

                result = await _repo.UpdateAsync(model);

                if (result)
                    _logger.LogInfo($"Invoice updated | id: {model.Id}");
                else
                    _logger.LogError("Invoice update failed", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in SalesInvoice CreateAndEdit", ex);
            TempData["Error"] = "Unexpected error occurred!";
            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Sales invoice saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }
}