using AutoFixture;
using Moq;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using Xunit;
using contacts;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Enums;
using FluentAssertions;
using ServiceContracts;

namespace Tests;

public class PeopleControllerTest
{
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;

    private readonly Mock<ICountriesService> _mockCountriesService;
    private readonly Mock<IPersonsService> _mockPersonService;

    private readonly Fixture _fixture;

    public PeopleControllerTest()
    {
        _fixture = new Fixture();
        _mockCountriesService = new Mock<ICountriesService>();
        _mockPersonService = new Mock<IPersonsService>();
        _countriesService = _mockCountriesService.Object;
        _personService = _mockPersonService.Object;
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnIndexViewWithPeopleList()
    {
        //Arrange
        List<PersonResponse> responseList = _fixture.Create<List<PersonResponse>>();

        HomeController personDetailsController =
            new HomeController(_personService, _countriesService);

        _mockPersonService
            .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(responseList);

        _mockPersonService
            .Setup(temp => temp.GetSortedPeople(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(),
                It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(responseList);

        //Act
        //create dummy figures as parameters for the action method arguments
        IActionResult result = await personDetailsController.Index(_fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<SortOrderOptions>());

        //Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(responseList);
    }
    
    #endregion

    #region Create
    
    [Fact]
    public async void Create_IfModelErrors_ToReturnCreateView()
    {
        //Arrange
        PersonAddRequest personRequest = _fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        HomeController personDetailsController =
            new HomeController(_personService, _countriesService);

        _mockCountriesService
            .Setup(temp => temp.GetAllCountries())
            .ReturnsAsync(countries);

        _mockPersonService
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        HomeController personController = new HomeController(_personService, _countriesService);

        //Act
        //create fake model errors
        personController.ModelState.AddModelError("PersonName", "Person Name cannot be empty");

        //personRequest needs to be supplied as an argument to this method
        IActionResult result = await personDetailsController.Create(personRequest);
        
        
        //Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personRequest);
    }
    
    [Fact]
    public async void Create_IfNoModelErrors_ToReturnToIndexView()
    {
        //Arrange
        PersonAddRequest personRequest = _fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        HomeController personDetailsController =
            new HomeController(_personService, _countriesService);

        _mockCountriesService
            .Setup(temp => temp.GetAllCountries())
            .ReturnsAsync(countries);

        _mockPersonService
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        HomeController personController = new HomeController(_personService, _countriesService);

        //Act
        //personRequest needs to be supplied as an argument to this method
        IActionResult result = await personDetailsController.Create(personRequest);
        
        //Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        
        redirectResult.ActionName.Should().Be("Index");
    }
    
    #endregion
    
    [Fact]
    public async Task Create_IfModelStateIsInvalid_ShouldNotAddPerson()
    {
        // Arrange
        PersonAddRequest personRequest = _fixture.Create<PersonAddRequest>();

        HomeController controller =
            new HomeController(_personService, _countriesService);

        controller.ModelState.AddModelError(
            "PersonName", "Person Name cannot be empty");

        // Act
        await controller.Create(personRequest);

        // Assert
        _mockPersonService.Verify(
            temp => temp.AddPerson(It.IsAny<PersonAddRequest>()),
            Times.Never);
    }
}