using Microsoft.AspNetCore.Mvc;

namespace xUnit.Controllers;

public class HomeController : Controller
{
    [Route("/persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }
}