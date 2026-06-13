using InventorySystem.Models;
using InventorySystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace InventorySystem.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerRepository _repo;

    public CustomerController(ICustomerRepository repo)
    {
        _repo = repo;
    }
    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        var pagination = await _repo.GetAllAsync(search, page, pageSize);
        return View(pagination);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAndEdit(long? id)
    {
        if (id == null || id == 0)
            return View(new Customer());
        var customer = await _repo.GetByIdAsync((int)id);
        if (customer == null)
        {
            TempData["Error"] = "Customer not found!";
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndEdit(Customer model)
    {
        if (!ModelState.IsValid)
            return View(model);
        bool result;
        if (model.Id == 0)
        {
            result = await _repo.AddAsync(model);
            TempData[result ? "Success" : "Error"] =
                result ? "Customer created successfully!" : "Failed to create customer!";
        }
        else
        {
            result = await _repo.UpdateAsync(model);
            TempData[result ? "Success" : "Error"] =
                result ? "Customer updated successfully!" : "Customer not found or update failed!";
        }
        return RedirectToAction(nameof(Index));
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _repo.DeleteAsync(id);
        TempData[result ? "Success" : "Error"] =
            result ? "Customer deleted successfully!" : "Customer not found!";
        return RedirectToAction(nameof(Index));
    }
}
