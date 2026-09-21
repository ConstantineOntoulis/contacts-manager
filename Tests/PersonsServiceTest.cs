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
        _testOutputHelper = testOutputHelper;
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

    #region GetFilteredPersons

    //If the search text is empty and search by is "PersonName", it should return all people
    [Fact]
    public void GetFilteredPersons_EmptySearchText()
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
        List<PersonResponse> searchList = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected: ");
        foreach (PersonResponse person_update in searchList)
        {
            _testOutputHelper.WriteLine(person_update.ToString());
        }

        //Assert
        foreach (PersonResponse added in responseList)
        {
            Assert.Contains(added, searchList);
        }
    }

    //First we will add a few people, and then we will search based on person name with some search string. It should return the matching people
    [Fact]
    public void GetFilteredPersons_SearchByPersonName()
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
        List<PersonResponse> searchList = _personsService.GetFilteredPersons(nameof(Person.PersonName), "na");

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
    public void GetSortedPeople()
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

        List<PersonResponse> allPeople = _personsService.GetAllPersons();

        //Act
        List<PersonResponse> sortList =
            _personsService.GetSortedPeople(allPeople, nameof(Person.PersonName), SortOrderOptions.DESC);

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Actual: ");
        foreach (PersonResponse person_update in sortList)
        {
            _testOutputHelper.WriteLine(person_update.ToString());
        }

        responseList = responseList.OrderByDescending(temp => temp.PersonName).ToList();

        //Assert
        for (int i = 0; i < responseList.Count; i++)
        {
            Assert.Equal(responseList[i], sortList[i]);
        }
    }

    #endregion

    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public void UpdatePerson_NullPerson()
    {
        //Arrange
        PersonUpdateRequest? person_update_request = null;

        //Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            //Act
            _personsService.UpdatePerson(person_update_request);
        });
    }

    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidPersonID()
    {
        //Arrange
        PersonUpdateRequest? person_update_request = new PersonUpdateRequest()
        {
            PersonID = Guid.NewGuid()
        };

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _personsService.UpdatePerson(person_update_request);
        });
    }

    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_PersonNameIsNull()
    {
        //Arrange
        CountryAddRequest country_add_request = new CountryAddRequest()
        {
            CountryName = "Greece"
        };

        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest()
        {
            PersonName = "John",
            CountryID = country_response_from_add.CountryID
        };

        PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

        person_update_request.PersonName = null;

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _personsService.UpdatePerson(person_update_request);
        });
    }

    //First, add a new person and try to update the person name and email
    [Fact]
    public void UpdatePerson_PersonFullDetailsUpdation()
    {
        //Arrange
        CountryAddRequest country_add_request = new CountryAddRequest()
        {
            CountryName = "Greece"
        };

        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest()
        {
            PersonName = "John",
            CountryID = country_response_from_add.CountryID,
            Email = "email@example.com",
            Address = "Sample Address 3",
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01"),
            ReceiveNewsLetters = true
        };

        PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

        person_update_request.PersonName = "William";
        person_update_request.Email = "email@gmail.com";
        
        //Act
        PersonResponse person_response_from_update = _personsService.UpdatePerson(person_update_request);
        
        PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_update.PersonID);
        
        //Assert
        Assert.Equal(person_response_from_get, person_response_from_update);
    }

    #endregion
}