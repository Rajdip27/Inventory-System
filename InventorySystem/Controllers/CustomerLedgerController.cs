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

    public async Task<IActionResult> Index(
        long? customerId,
        DateTime? startDate,
        DateTime? endDate)
    {
        _logger.LogInfo(
            $"Customer Ledger Index called | CustomerId:{customerId} | StartDate:{startDate} | EndDate:{endDate}");

        try
        {
            var customers = await _customerRepo.GetAllAsync("", 1, 10000);

            ViewBag.Customers = customers.Items
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            ViewBag.CustomerId = customerId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

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
