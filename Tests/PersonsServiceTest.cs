using ServiceContracts;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;

namespace Tests;

public class PersonsServiceTest
{
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        
        var countriesInitialData = new List<Country>() { };
        var personsInitialData = new List<Person>() { };
        
        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            
        ApplicationDbContext dbContext = dbContextMock.Object;

        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
        
        _countriesService = new CountriesService(dbContext);
        _personsService = new PersonsService(dbContext, _countriesService);
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        //Arrange
        PersonAddRequest? personAddRequest = null;

        //Assert
        Func<Task> action = async () =>
        {
            //Act
            await _personsService.AddPerson(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        /*
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            //Act
            await _personsService.AddPerson(personAddRequest);
        });
        */
    }

    //When we supply null value as PersonName, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        //Arrange
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();

        //Assert
        Func<Task> action = async () =>
        {
            //Act
            await _personsService.AddPerson(personAddRequest);
        };
        
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //when we supply proper person details, it should insert the person into the persons' list; and
    //it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        //Arrange
        
        //With AutoFixture
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com")
            .Create();
        
        //Without AutoFixture
        /*
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
        */

        //Act
        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

        List<PersonResponse> persons_from_GetAllPersons = await _personsService.GetAllPersons();

        //Assert
        //Assert.True(personResponse.PersonID != Guid.Empty);
        personResponse.PersonID.Should().NotBe(Guid.Empty);
        
        //Assert.Contains(personResponse, persons_from_GetAllPersons);
        persons_from_GetAllPersons.Should().Contain(personResponse);
    }

    #endregion

    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
        //Arrange
        Guid? personID = null;

        //Act
        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

        //Assert
        //Assert.Null(person_response_from_get);
        person_response_from_get.Should().BeNull();
    }

    //If we supply a valid person id, it should return the valid person details as PersonResponse Object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID()
    {
        //Arrange
        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email@sample.com")
            .Create();

        PersonResponse person_response_from_add = await _personsService.AddPerson(person_request);

        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        //Assert
        //Assert.Equal(person_response_from_add, person_response_from_get);
        
        person_response_from_get.Should().Be(person_response_from_add);
    }

    #endregion

    #region GetAllPersons
    
    //The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_ReturnsExistingPersons()
    {
        // Arrange
        CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();

        CountryResponse countryResponse =
            await _countriesService.AddCountry(countryRequest);

        PersonAddRequest personRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "person@example.com")
            .Create();

        PersonResponse addedPerson =
            await _personsService.AddPerson(personRequest);

        // Act
        List<PersonResponse> persons_from_get =
            await _personsService.GetAllPersons();

        // Assert
        Assert.Single(persons_from_get);

        Assert.Equal(
            addedPerson.PersonID,
            persons_from_get[0].PersonID);

        Assert.Equal(
            addedPerson.PersonName,
            persons_from_get[0].PersonName);
    }

    //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetPeopleList_AddFewPeople()
    {
        CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();


        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

        PersonAddRequest personRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone1@example.com")
            .Create();

        PersonAddRequest personRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone2@example.com")
            .Create();

        PersonAddRequest personRequest3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone3@example.com")
            .Create();
        
        List<PersonAddRequest> peopleRequests = new List<PersonAddRequest>()
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        List<PersonResponse> responseList = new List<PersonResponse>();


        foreach (PersonAddRequest personRequest in peopleRequests)
        {
            PersonResponse fullPeopleResponse = await _personsService.AddPerson(personRequest);
            responseList.Add(fullPeopleResponse);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_response in responseList)
        {
            _testOutputHelper.WriteLine(person_response.ToString());
        }

        //Act
        List<PersonResponse> updatedList = await _personsService.GetAllPersons();

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_update in updatedList)
        {
            _testOutputHelper.WriteLine(person_update.ToString());
        }

        //Assert
        /*foreach (PersonResponse added in responseList)
        {
            Assert.Contains(added, updatedList);
        }
        */
        updatedList.Should().BeEquivalentTo(responseList);
    }

    #endregion

    #region GetFilteredPersons

    //If the search text is empty and search by is "PersonName", it should return all people
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();


        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

        PersonAddRequest personRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone1@example.com")
            .Create();

        PersonAddRequest personRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone2@example.com")
            .Create();

        PersonAddRequest personRequest3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone3@example.com")
            .Create();

        List<PersonAddRequest> peopleRequests = new List<PersonAddRequest>()
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        List<PersonResponse> responseList = new List<PersonResponse>();


        foreach (PersonAddRequest personRequest in peopleRequests)
        {
            PersonResponse fullPeopleResponse = await _personsService.AddPerson(personRequest);
            responseList.Add(fullPeopleResponse);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_response in responseList)
        {
            _testOutputHelper.WriteLine(person_response.ToString());
        }

        //Act
        List<PersonResponse> searchList = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_update in searchList)
        {
            _testOutputHelper.WriteLine(person_update.ToString());
        }

        //Assert
        /*
        foreach (PersonResponse added in responseList)
        {
            Assert.Contains(added, searchList);
        }
        */
        searchList.Should().BeEquivalentTo(responseList);
    }

    //First we will add a few people, and then we will search based on person name with some search string. It should return the matching people
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();


        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

        PersonAddRequest personRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp =>  temp.PersonName, "Nami")
            .With(temp => temp.Email, "someone1@example.com")
            .With(temp => temp.CountryID, countryResponse1.CountryID)
            .Create();

        PersonAddRequest personRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Naruhodo")
            .With(temp => temp.Email, "someone2@example.com")
            .With(temp => temp.CountryID, countryResponse1.CountryID)
            .Create();

        PersonAddRequest personRequest3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Nancy")
            .With(temp => temp.Email, "someone3@example.com")
            .With(temp => temp.CountryID, countryResponse2.CountryID)
            .Create();

        List<PersonAddRequest> peopleRequests = new List<PersonAddRequest>()
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        List<PersonResponse> responseList = new List<PersonResponse>();


        foreach (PersonAddRequest personRequest in peopleRequests)
        {
            PersonResponse fullPeopleResponse = await _personsService.AddPerson(personRequest);
            responseList.Add(fullPeopleResponse);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_response in responseList)
        {
            _testOutputHelper.WriteLine(person_response.ToString());
        }

        //Act
        List<PersonResponse> searchList = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "na");

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_update in searchList)
        {
            _testOutputHelper.WriteLine(person_update.ToString());
        }

        //Assert
        foreach (PersonResponse added in responseList)
        {
            if (added.PersonName != null)
            {
                if (added.PersonName.Contains("na", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(added, searchList);
                }
            }
        }
    }

    #endregion

    #region GetSortedPeople

    //When we sort based on PersonName in DESC, it should return persons list in descending on PersonsName
    [Fact]
    public async Task GetSortedPeople()
    {
        CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();


        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

        PersonAddRequest personRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp =>  temp.PersonName, "Nami")
            .With(temp => temp.Email, "someone1@example.com")
            .With(temp => temp.CountryID, countryResponse1.CountryID)
            .Create();

        PersonAddRequest personRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Naruhodo")
            .With(temp => temp.Email, "someone2@example.com")
            .With(temp => temp.CountryID, countryResponse1.CountryID)
            .Create();;

        PersonAddRequest personRequest3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Nancy")
            .With(temp => temp.Email, "someone3@example.com")
            .With(temp => temp.CountryID, countryResponse2.CountryID)
            .Create();;

        List<PersonAddRequest> peopleRequests = new List<PersonAddRequest>()
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        List<PersonResponse> responseList = new List<PersonResponse>();

        foreach (PersonAddRequest personsRequest in peopleRequests)
        {
            PersonResponse fullPeopleResponse = await _personsService.AddPerson(personsRequest);
            responseList.Add(fullPeopleResponse);
        }

        //print responseList 
        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_ResponseList in responseList)
        {
            _testOutputHelper.WriteLine(person_ResponseList.ToString());
        }

        List<PersonResponse> allPeople = await _personsService.GetAllPersons();
        List<PersonResponse> sortList = await _personsService.GetSortedPeople(allPeople, 
            nameof(Person.PersonName), SortOrderOptions.DESC);

        //print updatedList
        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse sort_List in sortList)
        {
            _testOutputHelper.WriteLine(sort_List.ToString());
        }

        responseList = responseList.OrderByDescending(temp => temp.PersonName).ToList();

        for(int i = 0; i< responseList.Count; i++)
        {
            Assert.Equal(responseList, sortList);
        }
    }

    #endregion

    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        //Arrange
        PersonUpdateRequest? person_update_request = null;

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            //Act
            await _personsService.UpdatePerson(person_update_request);
        });
    }

    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
        //Arrange
        PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
            .Create();

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            //Act
            await _personsService.UpdatePerson(person_update_request);
        });
    }

    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull()
    {
        //Arrange
        CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();

        CountryResponse countryResponse =
            await _countriesService.AddCountry(countryRequest);

        PersonAddRequest personRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Nami")
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.CountryID, countryResponse.CountryID)
            .Create();

        PersonResponse person_response_from_add =
            await _personsService.AddPerson(personRequest);

        PersonUpdateRequest person_update_request =
            person_response_from_add.ToPersonUpdateRequest();

        person_update_request.PersonName = null;

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            //Act
            await _personsService.UpdatePerson(person_update_request);
        });
    }

    //First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetails()
    {
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp=>temp.PersonName, "Nasos")
            .With(temp => temp.Email, "someone@example.com")
            .With(temp=>temp.CountryID, countryResponse.CountryID)
            .Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

        PersonUpdateRequest updateRequest = personResponse.ToPersonUpdateRequest();
        updateRequest.PersonName = "Ada";
        updateRequest.Email = "updatedemailaddress@example.com";

        PersonResponse updatedPerson = await _personsService.UpdatePerson(updateRequest);

        PersonResponse? getByIdResponse = await _personsService.GetPersonByPersonID(updatedPerson.PersonID);

        Assert.Equal(getByIdResponse, updatedPerson);
    }

    #endregion

    #region DeletePerson

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        //Act
        bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());
        
        //Assert
        Assert.False(isDeleted);
    }
    
    //If you supply a valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
        //Arrange
        CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
        
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "example@email.com")
            .Create();
        
        PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
        
        //Act
        bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);
        
        //Assert
        Assert.True(isDeleted);
    }
    
    #endregion
}