using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO.PersonDTO;
using Services.Helpers;

namespace Services;

public class PersonsService :IPersonsService
{
    //Private field
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;
    
    //constructor
    public PersonsService()
    {
        _persons = new List<Person>();
        _countriesService = new CountriesService();
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
        
        return personResponse;
    }
    
    public PersonResponse AddPerson(PersonAddRequest personAddRequest)
    {
        //check if PersonAddRequest is not null
        if (personAddRequest == null)
        {
            throw new ArgumentNullException(nameof(personAddRequest));
        }
        
        //Model Validations
        ValidationHelper.ModelValidation(personAddRequest);
        
        //convert personAddRequest into Person Type
        Person person = personAddRequest.ToPerson();
        
        //generate PersonID
        person.PersonID = Guid.NewGuid();
        
        //add person object to persons list
        _persons.Add(person);
        
        //convert the Person Object into PersonResponse type
        return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(temp => temp.ToPersonResponse()).ToList();
    }

    public PersonResponse? GetPersonByPersonID(Guid? personID)
    {
        if (personID == null)
            return null;

        Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
        
        if (person == null)
            return null;
        
        return person.ToPersonResponse();
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPeople = GetAllPersons();
        List<PersonResponse> matchingPeople = allPeople;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return matchingPeople;

        switch (searchBy)
        {
            case nameof(Person.PersonName):
                matchingPeople = allPeople.Where(temp => (!string.IsNullOrEmpty(temp.PersonName)?temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            case nameof(Person.Email):
                matchingPeople = allPeople.Where(temp => (!string.IsNullOrEmpty(temp.Email)?temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            case nameof(Person.DateOfBirth):
                matchingPeople = allPeople.Where(temp => (temp.DateOfBirth != null) ? temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;
            case nameof(Person.Gender):
                matchingPeople = allPeople.Where(temp => (!string.IsNullOrEmpty(temp.Gender)?temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            case nameof(Person.CountryID):
                matchingPeople = allPeople.Where(temp => (!string.IsNullOrEmpty(temp.Country)?temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            case nameof(Person.Address):
                matchingPeople = allPeople.Where(temp => (!string.IsNullOrEmpty(temp.Address)?temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            
            default: matchingPeople = allPeople; break;
        }
        
        return matchingPeople;
    }
}