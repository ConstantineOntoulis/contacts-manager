using ServiceContracts;
using Xunit;
using Entities;
using ServiceContracts.DTO.PersonDTO;
using Services;

namespace Tests;

public class PersonsServiceTest
{
    //private fields
    private readonly IPersonsService _personsService;
    
    //constructor
    public PersonsServiceTest()
    {
        _personsService = new PersonsService();
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


}