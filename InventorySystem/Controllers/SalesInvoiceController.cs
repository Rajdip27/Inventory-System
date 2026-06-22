using InventorySystem.Cmmon;
using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using InventorySystem.Services;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;
using static InventorySystem.Models.Auth.IdentityModel;

namespace InventorySystem.Controllers;

public class SalesInvoiceController : Controller
{
    private readonly ISalesInvoiceRepository _repo;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IAppLogger<SalesInvoiceController> _logger;
    private readonly IPdfService _pdfService;
    private readonly IRazorViewToStringRenderer _renderToStringRenderer;

    public SalesInvoiceController(
        ISalesInvoiceRepository repo,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IAppLogger<SalesInvoiceController> logger,
        IPdfService pdfService,
        IRazorViewToStringRenderer renderToStringRenderer)
    {
        _repo = repo;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _logger = logger;
        _pdfService = pdfService;
        _renderToStringRenderer = renderToStringRenderer;
    }

    #region INDEX
    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"Index called | search:{search}");

        var pagination = await _repo.GetAllAsync(search, page, pageSize);

        return View(pagination);
    }
    #endregion

    #region CREATE & EDIT GET
    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        ViewBag.Products = await _productRepository.GetProductDropdownAsync();
        ViewBag.Customers = await _customerRepository.GetCustomerDropdownAsync();

        if (id == null || id == 0)
        {
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
            TempData["Error"] = "Invoice not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(invoice);
    }
    #endregion

    #region CREATE & EDIT POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(SalesInvoice model)
    {
        _logger.LogInfo($"POST Create/Edit | Id:{model?.Id}");

        // IMPORTANT: reload dropdowns (fix view crash)
        ViewBag.Products = await _productRepository.GetProductDropdownAsync();
        ViewBag.Customers = await _customerRepository.GetCustomerDropdownAsync();

        // SERVER VALIDATION
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fix validation errors";
            return View(model);
        }

        if (model.SalesItem == null || model.SalesItem.Count == 0)
        {
            ModelState.AddModelError("", "Please add at least one product");
            return View(model);
        }

        try
        {
            (bool success, string message) result;

            if (model.Id == 0)
            {
                _logger.LogInfo("Creating invoice");
                result = await _repo.AddAsync(model);
            }
            else
            {
                _logger.LogInfo("Updating invoice");
                result = await _repo.UpdateAsync(model);
            }

            if (!result.success)
            {
                TempData["Error"] = result.message;
                return View(model);
            }

            TempData["Success"] = result.message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected error in CreateAndEdit", ex);
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }

    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInfo($"Details called | Id:{id}");

        var invoice = await _repo.GetByIdAsync(id);

        if (invoice == null)
        {
            TempData["Error"] = "Invoice not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(invoice);
    }

    public async Task<IActionResult> Print(long id)
    {
        _logger.LogInfo($"Print called | Id:{id}");

        var invoice = await _repo.GetByIdAsync(id);

        if (invoice == null)
        {
            TempData["Error"] = "Invoice not found!";
            return RedirectToAction(nameof(Index));
        }
        // Render the view to HTML
        ViewBag.SoldBy = "Admin";
        ViewBag.SoldTo = "Walk-in Customer";
        var htmlContent = await _renderToStringRenderer.RenderViewToStringAsync(
            "PdfTemplates/_InvoicePdf",
            invoice
        );
        var pdfOptions = new PdfOptions
        {
            BaseUrl = $"{Request.Scheme}://{Request.Host}",
            EnableLocalFileAccess = true,
            LoadImages = true,
            PageSize = "A4",
            Landscape = false,  // Landscape for horizontal layout
            MarginTop = 5,     // Zero margins - handled by CSS
            MarginBottom = 5,
            MarginLeft = 5,
            MarginRight = 5,
            ShowPageNumbers = false,
            ColorMode = true,
            HideHeader = true,  // Hide DinkToPdf header
            HideFooter = true,  // Hide DinkToPdf footer
        };
        var pdfBytes = await _pdfService.GeneratePdf(htmlContent, pdfOptions);
        return File(pdfBytes, "application/pdf", $"Invoice_{invoice.InvoiceNo}.pdf");
    }
    #endregion
}