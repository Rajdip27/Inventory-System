using InventorySystem.Logging;
using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerRepository _repo;
    private readonly IAppLogger<CustomerController> _logger;

    public CustomerController(
        ICustomerRepository repo,
        IAppLogger<CustomerController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        _logger.LogInfo($"Customer Index called | search: {search}, page: {page}, pageSize: {pageSize}");
        var pagination = await _repo.GetAllAsync(search, page, pageSize);
        _logger.LogInfo($"Customer Index loaded | total items: {pagination?.TotalItems}");
        return View(pagination);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        _logger.LogInfo($"CreateAndEdit GET called | id: {id}");

        if (id == null || id == 0)
        {
            _logger.LogInfo("Opening Create new Customer form");
            return View(new Customer());
        }

        var customer = await _repo.GetByIdAsync((int)id);

        if (customer == null)
        {
            _logger.LogWarning($"Customer not found | id: {id}");
            TempData["Error"] = "Customer not found!";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInfo($"Editing Customer | id: {id}");

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(Customer model)
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
                _logger.LogInfo("Creating new Customer");
                result = await _repo.AddAsync(model);
                if (result)
                    _logger.LogInfo($"Customer created successfully | name: {model.Name}");
                else
                    _logger.LogError("Failed to create customer", null);
            }
            else
            {
                _logger.LogInfo($"Updating Customer | id: {model.Id}");
                result = await _repo.UpdateAsync(model);
                if (result)
                    _logger.LogInfo($"Customer updated successfully | id: {model.Id}");
                else
                    _logger.LogError("Customer update failed", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred in CreateAndEdit POST", ex);
            TempData["Error"] = "Unexpected error occurred!";
            return RedirectToAction(nameof(Index));
        }

        TempData[result ? "Success" : "Error"] =
            result ? "Customer saved successfully!" : "Operation failed!";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInfo($"Delete called | id: {id}");

        try
        {
            var result = await _repo.DeleteAsync(id);

            if (result)
                _logger.LogInfo($"Customer deleted successfully | id: {id}");
            else
                _logger.LogWarning($"Customer delete failed | id: {id}");

            TempData[result ? "Success" : "Error"] =
                result ? "Customer deleted successfully!" : "Customer not found!";
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception in Delete action", ex);
            TempData["Error"] = "Unexpected error occurred!";
        }

        return RedirectToAction(nameof(Index));
    }
}