using ServiceContracts;
using Xunit;
using Entities;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;
using Services;

namespace Tests;

public class PersonsServiceTest
{
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    //constructor
    public PersonsServiceTest()
    {
        _personsService = new PersonsService();
        _countriesService = new CountriesService();
    }


    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_NullPerson()
    {
        //Arrange
        PersonAddRequest?  personAddRequest = null;

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
        
        PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);
        
        //Assert
        Assert.Equal(person_response_from_add, person_response_from_get);
    }
    

    #endregion

}