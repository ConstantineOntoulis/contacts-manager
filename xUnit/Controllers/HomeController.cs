using Microsoft.AspNetCore.Mvc;

namespace xUnit.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}