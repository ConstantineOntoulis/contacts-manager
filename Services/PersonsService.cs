using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    //Private field
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;

    //constructor
    public PersonsService(bool initialize = true)
    {
        _persons = new List<Person>();
        _countriesService = new CountriesService();

        if (initialize)
        {
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("5ED75E76-2957-4761-B67D-96BADC88C22B"),
                PersonName = "Garrek",
                Email = "gliebrecht0@mashable.com",
                DateOfBirth = DateTime.Parse("1995-02-26"),
                Gender = "Male",
                Address = "70 Moland Road",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("BBE7AB87-3435-4C72-8188-4F52BF7D0987")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("D0073139-BAFA-43DF-84DC-D5E7DB2D7AEB"),
                PersonName = "Audrey",
                Email = "awetherhead1@cdbaby.com",
                DateOfBirth = DateTime.Parse("1994-08-03"),
                Gender = "Female",
                Address = "Northfield Circle",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("A001039D-A3A3-4BD2-8814-C6F0647048B5")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("618F10E5-A649-468C-BD9D-DE37E1C138A7"),
                PersonName = "Kelli",
                Email = "kgissop2@ed.gov",
                DateOfBirth = DateTime.Parse("1998-05-09"),
                Gender = "Female",
                Address = "199 Almo Plaza",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("5FBA1034-D1CC-41C0-918A-324A6F0FB6CD")
            });

            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("879183C0-C62B-43EE-BB41-65DA517C4DD3"),
                PersonName = "Rudd",
                Email = "rbater3@flickr.com",
                DateOfBirth = DateTime.Parse("1998-02-13"),
                Gender = "Male",
                Address = "65 Meadow Vale Street",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("32D9CA63-EF74-4CBA-B829-80FBE03B65E8")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("7E282436-E4A6-49ED-B58D-321AB3278AE7"),
                PersonName = "Jarrad",
                Email = "jtwitty4@nytimes.com",
                DateOfBirth = DateTime.Parse("1990-10-20"),
                Gender = "Male",
                Address = "85723 Mandrake Center",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("D2937E39-1913-49C8-B39D-C3F7FBFF42C2")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("7DF39B20-190B-4499-8A3C-A3445484C887"),
                PersonName = "Dallas",
                Email = "dbonwell5@biglobe.ne.jp",
                DateOfBirth = DateTime.Parse("2000-02-08"),
                Gender = "Female",
                Address = "60724 La Follette Trail",
                ReceiveNewsLetters = false,
                CountryID = Guid.Parse("D2937E39-1913-49C8-B39D-C3F7FBFF42C2")
            });

            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("BB304953-7F83-4FBF-BABA-FB43B8FBCC5E"),
                PersonName = "Dame",
                Email = "dhoutbie6@linkedin.com",
                DateOfBirth = DateTime.Parse("1990-11-14"),
                Gender = "Male",
                Address = "0665 Mccormick Way",
                ReceiveNewsLetters = false,
                CountryID = Guid.Parse("32D9CA63-EF74-4CBA-B829-80FBE03B65E8")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("70459AD1-CF06-4AE8-BF76-E4D5B21BE7F9"),
                PersonName = "Stephen",
                Email = "sdurdle7@yolasite.com",
                DateOfBirth = DateTime.Parse("1996-02-11"),
                Gender = "Male",
                Address = "11077 Brown Terrace",
                ReceiveNewsLetters = false,
                CountryID = Guid.Parse("32D9CA63-EF74-4CBA-B829-80FBE03B65E8")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("9C301455-9685-4422-B6ED-08C48524608C"),
                PersonName = "Nancie",
                Email = "ngraser8@oracle.com",
                DateOfBirth = DateTime.Parse("1995-05-13"),
                Gender = "Female",
                Address = "5 Eagle Crest Park",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("5FBA1034-D1CC-41C0-918A-324A6F0FB6CD")
            });
            
            _persons.Add(new Person()
            {
                PersonID = Guid.Parse("7003A94B-AA8C-49A1-950C-3E0C45983989"),
                PersonName = "Riccardo",
                Email = "rgradwell9@hostgator.com",
                DateOfBirth = DateTime.Parse("1990-10-31"),
                Gender = "Male",
                Address = "01912 Mifflin Parkway",
                ReceiveNewsLetters = true,
                CountryID = Guid.Parse("32D9CA63-EF74-4CBA-B829-80FBE03B65E8")
            });
        }
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
            case nameof(PersonResponse.PersonName):
                matchingPeople = allPeople.Where(temp =>
                    (!string.IsNullOrEmpty(temp.PersonName)
                        ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true)).ToList();
                break;
            case nameof(PersonResponse.Email):
                matchingPeople = allPeople.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Email)
                        ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true)).ToList();
                break;
            case nameof(PersonResponse.DateOfBirth):
                matchingPeople = allPeople.Where(temp =>
                    (temp.DateOfBirth != null)
                        ? temp.DateOfBirth.Value.ToString("dd MMMM yyyy")
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true).ToList();
                break;
            case nameof(PersonResponse.Gender):
                matchingPeople = allPeople.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Gender)
                        ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true)).ToList();
                break;
            case nameof(PersonResponse.CountryID):
                matchingPeople = allPeople.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Country)
                        ? temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true)).ToList();
                break;
            case nameof(PersonResponse.Address):
                matchingPeople = allPeople.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Address)
                        ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true)).ToList();
                break;

            default: matchingPeople = allPeople; break;
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
        Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);

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

        return matchingPerson.ToPersonResponse();
    }

    public bool DeletePerson(Guid? personID)
    {
        if (personID == null)
        {
            throw new ArgumentNullException(nameof(personID));
        }

        Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);

        if (person == null)
            return false;

        _persons.RemoveAll(temp => temp.PersonID == personID);

        return true;
    }
}