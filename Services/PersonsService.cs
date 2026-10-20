using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    //Private field
    private readonly PersonsDbContext _db;
    private readonly ICountriesService _countriesService;

    //constructor
    public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
    {
        _db = personsDbContext;
        _countriesService = countriesService;
    }

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
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
        _db.Persons
            .Add(person);
        _db.SaveChanges();
        //_db.sp_AddPerson(person);
        
        //convert the Person Object into PersonResponse type
        return person.ToPersonResponse();
    }

    public List<PersonResponse> GetAllPersons()
    {
        var persons = _db.Persons.Include("Country").ToList();
        
        //SELECT * from Persons
        return persons
            .Select(temp => temp.ToPersonResponse()).ToList();
        
        /*
        return _db.sp_GetAllPersons()
            .Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        */
        //Need to do the same or UPDATE and DELETE Person!!
    }

    public PersonResponse? GetPersonByPersonID(Guid? personID)
    {
        if (personID == null)
            return null;

        Person? person = _db.Persons
            .Include("Country")
            .FirstOrDefault(temp => temp.PersonID == personID);

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
            case nameof(PersonResponse.PersonName):
                matchingPeople = allPeople.Where(temp =>
                    temp.PersonName?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.Email):
                matchingPeople = allPeople.Where(temp =>
                    temp.Email?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.DateOfBirth):
                matchingPeople = allPeople.Where(temp =>
                    temp.DateOfBirth?.ToString("dd MMM yyyy")
                        .Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.Gender):
                matchingPeople = allPeople.Where(temp =>
                    temp.Gender?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.CountryID):
                matchingPeople = allPeople.Where(temp =>
                    temp.Country?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.Address):
                matchingPeople = allPeople.Where(temp =>
                    temp.Address?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            default:
                matchingPeople = allPeople;
                break;
        }

        return matchingPeople;
    }

    public List<PersonResponse> GetSortedPeople(List<PersonResponse> allPeople, string sortBy,
        SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return allPeople;

        List<PersonResponse> sortedPeople = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Age).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Age).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

            _ => allPeople
        };

        return sortedPeople;
    }

    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
            throw new ArgumentNullException(nameof(Person));

        //validation
        ValidationHelper.ModelValidation(personUpdateRequest);

        //get matching person object to update
        Person? matchingPerson = _db.Persons
            .AsNoTracking()
            .FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);

        if (matchingPerson == null)
        {
            throw new ArgumentException("Given person id doesn't exist");
        }

        //update all details
        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryID = personUpdateRequest.CountryID;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        _db.sp_UpdatePerson(matchingPerson); //UPDATE
        return matchingPerson.ToPersonResponse();
    }

    public bool DeletePerson(Guid? personID)
    {
        if (personID == null)
        {
            throw new ArgumentNullException(nameof(personID));
        }

        bool personExists = _db.Persons
            .Any(temp => temp.PersonID == personID.Value);

        if (!personExists)
            return false;
        
        _db.sp_DeletePerson(personID.Value);
        
        return true;
    }
}