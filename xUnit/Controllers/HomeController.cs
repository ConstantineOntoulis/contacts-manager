using Microsoft.AspNetCore.Mvc;

namespace xUnit.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/person")]
    public IActionResult Person()
    {
        return View();
    }
}