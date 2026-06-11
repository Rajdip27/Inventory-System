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
}
