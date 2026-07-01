using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class CustomerPaymentController : Controller
{
    private readonly ICustomerPaymentRepository _repo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ISalesInvoiceRepository _salesInvoiceRepo;
    private readonly IAppLogger<CustomerPaymentController> _logger;
    public CustomerPaymentController(
        ICustomerPaymentRepository repo,
        ICustomerRepository customerRepo,
        ISalesInvoiceRepository salesInvoiceRepo,
        IAppLogger<CustomerPaymentController> logger)
    {
        _repo = repo;
        _customerRepo = customerRepo;
        _logger = logger;
        _salesInvoiceRepo = salesInvoiceRepo;
    }
    public async Task<IActionResult> Index(long? customerId = null, long? invoiceId = null)
    {
        _logger.LogInfo($"Payment Index called | customerId: {customerId}, invoiceId: {invoiceId}");

        try
        {
            ViewBag.Customers = await _customerRepo.GetCustomerDropdownAsync();
            ViewBag.Invoices = await _salesInvoiceRepo.GetSalesInvoicesDropdownAsync();

            var data = await _repo.GetAllPaymentsAsync(customerId, invoiceId);

            _logger.LogInfo($"Payment loaded | total: {data?.Count}");

            return View(data);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in Payment Index", ex);
            return View(new List<CustomerPayment>());
        }
    }
    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        ViewBag.Customers = await _customerRepo.GetCustomerDropdownAsync();
        ViewBag.Invoices = await _salesInvoiceRepo.GetSalesInvoicesDropdownAsync();
        _logger.LogInfo($"Payment CreateAndEdit GET | id: {id}");

        if (id == null || id == 0)
        {
            return View(new CustomerPayment
            {
                PaymentDate = DateTime.Now
            });
        }

        var payment = await _repo.GetPaymentByIdAsync(id.Value);

        if (payment == null)
        {
            _logger.LogWarning($"Payment not found | id: {id}");
            TempData["Error"] = "Payment not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(payment);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(CustomerPayment model)
    {
        _logger.LogInfo($"Payment POST called | id: {model?.Id}, amount: {model.Amount}");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state in Payment POST");
            return View(model);
        }

        try
        {
            var result = await _repo.AddOrEditAsync(model);

            if (result)
                _logger.LogInfo("Payment saved successfully");
            else
                _logger.LogWarning("Payment save failed");

            TempData[result ? "Success" : "Error"] =
                result ? "Payment saved successfully!" : "Operation failed!";
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in Payment CreateAndEdit POST", ex);
            TempData["Error"] = "Unexpected error occurred!";
        }

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Ledger(long? customerId = null, long? invoiceId = null)
    {
        _logger.LogInfo($"Ledger called | customerId: {customerId}, invoiceId: {invoiceId}");
        var data = await _repo.GetCustomerLedgerAsync(customerId, invoiceId);
        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoicesByCustomer(long customerId)
    {
        var invoices = await _customerRepo.GetInvoicesByCustomer(customerId);
        return Json(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoiceDetails(long invoiceId)
    {
        var result = await _repo.GetInvoiceDetails(invoiceId);
        if (result == null)
            return NotFound();
        return Json(result);
    }
}
