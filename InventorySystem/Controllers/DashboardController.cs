using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
