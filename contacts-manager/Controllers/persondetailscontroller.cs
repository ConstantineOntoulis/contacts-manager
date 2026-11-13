using System.Diagnostics;
using AutoFixture;
using Moq;
using ServiceContracts.DTO.CountryDTO;
using ServiceContracts.DTO.PersonDTO;
using Xunit;
using ContactsManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ServiceContracts.Enums;
using FluentAssertions;
using ServiceContracts;

namespace ContactsManager;

public class PeopleControllerTest
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;

    private readonly Mock<ICountriesService> _mockCountriesService;
    private readonly Mock<IPersonService> _mockPersonService;

    private readonly Fixture _fixture;

    public PeopleControllerTest()
    {
        _fixture = new Fixture();
        _mockCountriesService = new Mock<ICountriesService>();
        _mockPersonService = new Mock<IPersonService>();
        _countriesService = _mockCountriesService.Object;
        _personService = _mockPersonService.Object;
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnIndexViewWithPeopleList()
    {
        //Arrange
        List<PersonResponse> responseList = _fixture.Create<List<PersonResponse>>();

        PersonDetailsController personDetailsController =
            new PersonDetailsController(_personService, _countriesService);

        _mockPersonService
            .Setup(temp => temp.GetFilteredPeople(It.IsAny<string>(), It.IsAny<string>()))
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
}