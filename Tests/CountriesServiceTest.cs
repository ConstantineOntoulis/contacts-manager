using ServiceContracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTO.CountryDTO;
using Services;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;
using RepositoryContracts;
using Xunit.Abstractions;

namespace Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;
    
    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly ICountriesRepository _countriesRepository;

    private readonly ITestOutputHelper _testOutputHelper;

    public CountriesServiceTest()
    {
        _fixture = new Fixture();
        
        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        _countriesRepository = _countriesRepositoryMock.Object;
        
        var countriesInitialData = new List<Country>() { };
        
        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            
        ApplicationDbContext dbContext = dbContextMock.Object;

        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        
        _countriesService = new CountriesService(null);
    }

    #region AddCountry
    
    //When CountryAddRequest is null, it should ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        //Arrange
        CountryAddRequest? request = null;

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            //Act
            await _countriesService.AddCountry(request);
        });
    }

    //When the CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        //Arrange
        CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
            .With(temp => temp.CountryName, null as string)
            .Create();

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            //Act
            await _countriesService.AddCountry(request);
        });
    }

    //When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        //Arrange
        CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
            .With(temp => temp.CountryName, "Duplicate Country")
            .Create();
        CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
            .With(temp => temp.CountryName, "Duplicate Country")
            .Create();

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            //Act
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);
        });
    }

    //When you supply proper country name, it should insert (add) the country to the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        //Arrange
        CountryAddRequest? request = _fixture.Create<CountryAddRequest>();

        //Act
        CountryResponse response = await _countriesService.AddCountry(request);

        List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();
        
        //Assert
        Assert.True(response.CountryID != Guid.Empty);
        Assert.Contains(response, countries_from_GetAllCountries);
    }
    
    //objA.Equals(objB)
    
    #endregion

    #region GetAllCountries

    [Fact]
    //The list of countries should be empty by default (before adding any countries)
    public async Task GetAllCountries_EmptyList()
    {
        //Act
        List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();
        
        //Assert
        Assert.Empty(actual_country_response_list);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        //Arrange
        List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
        {
            _fixture.Create<CountryAddRequest>(),
            _fixture.Create<CountryAddRequest>()
        };
        
        //Act
        List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
        
        foreach (CountryAddRequest country_request in country_request_list)
        {
            countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
        }

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();
        
        //read each element from countries_list_from_add_country
        foreach (CountryResponse expected_country in countries_list_from_add_country)
        {
            Assert.Contains(expected_country, actualCountryResponseList);
        }
    }

    #endregion

    #region GetCountryByCountryID

    //If we supply null as CountryID, it should return null as CountryResponse
    [Fact]
    public async Task GetCountryByCountryID_NullCountryID()
    {
        //Arrange
        Guid? countryID = null;
        
        //Act
        CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);
        
        //Assert
        Assert.Null(country_response_from_get_method);
    }

    [Fact]
    //If we supply a valid country id, it should return the matching country details as CountryResponse object
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        //Arrange
        CountryAddRequest? country_add_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        //Act
        CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);
        
        //Assert
        Assert.Equal(country_response_from_add, country_response_from_get);
    }

    #endregion
}