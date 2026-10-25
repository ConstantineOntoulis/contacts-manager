using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
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
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
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
        
        List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;
        
        //Sort
        List<PersonResponse> sortedPeople = await _personsService.GetSortedPeople(persons, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();
        
        return View(sortedPeople); //Views/Home/Index.cshtml
    }

    //Executes when the user clicks on the "Create Person" hyperlink
    //(while opening the create view)
    [Route("add")]
    [HttpGet]
    public async Task<IActionResult> CreatePerson()
    {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp => new SelectListItem()
        {
            Text = temp.CountryName,
            Value = temp.CountryID.ToString()
        });
        /*{
            Text = "Tino",
            Value = "1"
        };*/
        //this is like writing <option value="1">Tino</option>
        
        return View();
    }

    [Route("add")]
    [HttpPost]
    public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
        
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            }).ToList();
            
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View("CreatePerson", personAddRequest);
        }
        
        //call the service method
        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        
        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("edit/{personID}")] //Eg: /persons/edit/1
    public async Task<IActionResult> EditPerson(Guid personID)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
        
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

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
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
            return RedirectToAction("Index");
        }
        else
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
        
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            }).ToList();
            
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();

            return View("EditPerson", personUpdateRequest);
        }
    }

    [HttpGet]
    [Route("delete/{personID}")]
    public async Task<IActionResult> Delete(Guid? personID)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }
        
        return View("DeletePerson", personResponse);
    }

    [HttpPost]
    [Route("delete/{personID}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        _personsService.DeletePerson(personUpdateRequest.PersonID);
        
        return RedirectToAction("Index");
    }

    [Route("persons/PersonsPDF")]
    public async Task<IActionResult> PersonsPDF()
    {
        //Get list of persons
        List<PersonResponse> persons = await _personsService.GetAllPersons();
        
        //Return view as pdf
        return new ViewAsPdf("PersonsPDF", persons, ViewData)
        {
            PageMargins = new Rotativa.AspNetCore.Options.Margins()
            {
                Top = 20,
                Right = 20,
                Bottom = 20,
                Left = 20
            },
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        };
    }
}