using ServiceContracts;
using Xunit;
using Entities;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace Tests;

public class PersonsServiceTest
{
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _personsService = new PersonsService();
        _countriesService = new CountriesService();
        _testOutputHelper  = testOutputHelper;
    }
    
    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_NullPerson()
    {
        //Arrange
        PersonAddRequest? personAddRequest = null;

        //Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            //Act
            _personsService.AddPerson(personAddRequest);
        });
    }

    //When we supply null value as PersonName, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        //Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = null
        };

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _personsService.AddPerson(personAddRequest);
        });
    }

    //when we supply proper person details, it should insert the person into the persons' list; and
    //it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        //Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "Josh",
            Email = "person@example.com",
            Address = "Sample Address",
            CountryID = Guid.NewGuid(),
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01"),
            ReceiveNewsLetters = true
        };

        //Act
        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

        List<PersonResponse> persons_from_GetAllPersons = _personsService.GetAllPersons();

        //Assert
        Assert.True(personResponse.PersonID != Guid.Empty);
        Assert.Contains(personResponse, persons_from_GetAllPersons);
    }

    #endregion

    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public void GetPersonByPersonID_NullPersonID()
    {
        //Arrange
        Guid? personID = null;

        //Act
        PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(personID);

        //Assert
        Assert.Null(person_response_from_get);
    }

    //If we supply a valid person id, it should return the valid person details as PersonResponse Object
    [Fact]
    public void GetPersonByPersonID_WithPersonID()
    {
        //Arrange
        CountryAddRequest country_request = new CountryAddRequest()
        {
            CountryName = "Greece"
        };

        CountryResponse country_response = _countriesService.AddCountry(country_request);

        PersonAddRequest person_request = new PersonAddRequest()
        {
            PersonName = "Josh",
            Email = "email@example.com",
            Address = "address",
            CountryID = country_response.CountryID,
            DateOfBirth = DateTime.Parse("2000-01-01"),
            Gender = GenderOptions.Male, ReceiveNewsLetters = false
        };

        PersonResponse person_response_from_add = _personsService.AddPerson(person_request);

        PersonResponse? person_response_from_get =
            _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        //Assert
        Assert.Equal(person_response_from_add, person_response_from_get);
    }

    #endregion

    #region GetAllPersons

    //The GetAllPersons() should return an empty list by default
    [Fact]
    public void GetAllPersons_EmptyList()
    {
        //Act
        List<PersonResponse> persons_from_get = _personsService.GetAllPersons();

        //Assert
        Assert.Empty(persons_from_get);
    }

    //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public void GetPeopleList_AddFewPeople()
    {
        CountryAddRequest countryRequest1 = new CountryAddRequest() { CountryName = "Italy" };
        CountryAddRequest countryRequest2 = new CountryAddRequest() { CountryName = "Japan" };


        CountryResponse countryResponse1 = _countriesService.AddCountry(countryRequest1);
        CountryResponse countryResponse2 = _countriesService.AddCountry(countryRequest2);

        PersonAddRequest personRequest1 = new PersonAddRequest()
        {
            PersonName = "Name1",
            Email = "person1@example.com",
            Address = "Sample Address 1",
            CountryID = countryResponse1.CountryID,
            Gender = ServiceContracts.Enums.GenderOptions.Female,
            DateOfBirth = DateTime.Parse("2000-01-01"),
            ReceiveNewsLetters = true
        };

        PersonAddRequest personRequest2 = new PersonAddRequest()
        {
            PersonName = "Name2",
            Email = "person2@example.com",
            Address = "Sample Address 2",
            CountryID = countryResponse2.CountryID,
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01"),
            ReceiveNewsLetters = true
        };

        PersonAddRequest personRequest3 = new PersonAddRequest()
        {
            PersonName = "Name3",
            Email = "person3@example.com",
            Address = "Sample Address 3",
            CountryID = countryResponse2.CountryID,
            Gender = ServiceContracts.Enums.GenderOptions.Female,
            DateOfBirth = DateTime.Parse("2000-01-01"),
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> peopleRequests = new List<PersonAddRequest>()
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        List<PersonResponse> responseList = new List<PersonResponse>();

        
        foreach (PersonAddRequest personRequest in peopleRequests)
        {
            PersonResponse fullPeopleResponse = _personsService.AddPerson(personRequest);
            responseList.Add(fullPeopleResponse);
        }
        
        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_response in responseList)
        {
            _testOutputHelper.WriteLine(person_response.ToString());
        }
        
        //Act
        List<PersonResponse> updatedList = _personsService.GetAllPersons();

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_update in updatedList)
        {
            _testOutputHelper.WriteLine(person_update.ToString());
        }
        
        //Assert
        foreach (PersonResponse added in responseList)
        {
            Assert.Contains(added, updatedList);
        }
    }

    #endregion
}