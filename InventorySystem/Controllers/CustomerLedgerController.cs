using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.Controllers;

public class CustomerLedgerController : Controller
{
    private readonly ICustomerLedgerRepository _ledgerRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IAppLogger<CustomerLedgerController> _logger;
public CustomerLedgerController(
    ICustomerLedgerRepository ledgerRepo,
    ICustomerRepository customerRepo,
    IAppLogger<CustomerLedgerController> logger)
    {
        _ledgerRepo = ledgerRepo;
        _customerRepo = customerRepo;
        _logger = logger;
    }

    public async Task<IActionResult> Index(long? customerId,DateTime? startDate,DateTime? endDate)
    {
        _logger.LogInfo(
            $"Customer Ledger Index called | CustomerId:{customerId} | StartDate:{startDate} | EndDate:{endDate}");

        try
        {
            ViewBag.Customers = await _customerRepo.GetCustomerDropdownAsync();

            ViewBag.CustomerId = customerId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // First page load - don't load data
            if (!customerId.HasValue &&
                !startDate.HasValue &&
                !endDate.HasValue)
            {
                return View(new List<CustomerLedger>());
            }

            // Optional: Require customer selection
            if (!customerId.HasValue)
            {
                TempData["Error"] = "Please select a customer.";
                return View(new List<CustomerLedger>());
            }

            var ledgerList = await _ledgerRepo.GetLedgerAsync(
                customerId,
                startDate,
                endDate);

            _logger.LogInfo(
                $"Customer Ledger Loaded | Total Records: {ledgerList.Count}");

            return View(ledgerList);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Exception occurred in CustomerLedger Index",
                ex);

            TempData["Error"] = "Something went wrong.";

            return View(new List<CustomerLedger>());
        }
    }

}
