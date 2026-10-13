using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;

namespace xUnit.Controllers;

[Route("persons")]
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
    [Route("index")]
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
    [Route("add")]
    [HttpGet]
    public IActionResult CreatePerson()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp => new SelectListItem()
        {
            Text = temp.CountryName,
            Value = temp.CountryID.ToString()
        });
        /*{
            Text = "Tino",
            Value = "1"
        };*/
        //this is like writting <option value="1">Tino</option>
        
        return View();
    }

    [Route("add")]
    [HttpPost]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
        
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            }).ToList();
            
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View("CreatePerson", personAddRequest);
        }
        
        //call the service method
        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        
        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("edit/{personID}")] //Eg: /persons/edit/1
    public IActionResult EditPerson(Guid personID)
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
        
        List<CountryResponse> countries = _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp => new SelectListItem()
        {
            Text = temp.CountryName,
            Value = temp.CountryID.ToString()
        }).ToList();
            
        ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
        
        return View("EditPerson", personUpdateRequest);
    }

    [HttpPost]
    [Route("edit/{personID}")]
    public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
            return RedirectToAction("Index");
        }
        else
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
        
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            }).ToList();
            
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();

            return View();
        }
    }
}