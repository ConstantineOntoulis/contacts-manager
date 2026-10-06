using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;

namespace xUnit.Controllers;

public class HomeController : Controller
{
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    
    //constructor
    public HomeController(IPersonsService personsService, ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }
    
    [Route("/")]
    [Route("/persons/index")]
    public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        //Search
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryID), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };
        
        List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;
        
        //Sort
        List<PersonResponse> sortedPeople = _personsService.GetSortedPeople(persons, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();
        
        return View(sortedPeople); //Views/Home/Index.cshtml
    }

    //Executes when the user clicks on the "Create Person" hyperlink
    //(while opening the create view)
    [Route("/persons/add")]
    [HttpGet]
    public IActionResult CreatePerson()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        
        ViewBag.Countries = countries;
        
        return View();
    }

    [Route("/persons/add")]
    [HttpPost]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
        
            ViewBag.Countries = countries;
            
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View("CreatePerson", personAddRequest);
        }
        
        //call the service method
        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        
        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index", "Home");
    }
}