using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;

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
}