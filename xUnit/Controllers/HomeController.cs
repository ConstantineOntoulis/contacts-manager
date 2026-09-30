using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO.PersonDTO;

namespace xUnit.Controllers;

public class HomeController : Controller
{
    //private fields
    private readonly IPersonsService _personsService;
    
    //constructor
    public HomeController(IPersonsService personsService)
    {
        _personsService = personsService;
    }
    
    [Route("/persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        List<PersonResponse> persons = _personsService.GetAllPersons();
        
        return View(persons); //Views/Home/Index.cshtml
    }
}