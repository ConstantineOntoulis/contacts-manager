using Entities;
using Microsoft.AspNetCore.Http;
using ServiceContracts;
using ServiceContracts.DTO.CountryDTO;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field so that several methods, and several calls to those methods, need access to the same list
    private readonly ICountriesRepository _countriesRepository;
    
    //constructor
    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
    }
    
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        //Validation: countryAddRequest parameter can't be null
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }
        
        //Validation: CountryName can't be null
        if (countryAddRequest.CountryName == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        }
        
        //Validation: CountryName can't be duplicate
        if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
        {
            throw new ArgumentException("Given country name already exists");
        }
        
        //Convert object from CountryAddRequest to Country Type
        Country country = countryAddRequest.ToCountry();
        
        //Generate CountryID
        country.CountryID = Guid.NewGuid();
        
        //Add country object into _countries
        await _countriesRepository.AddCountry(country);

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        List<Country> countries = await _countriesRepository.GetAllCountries();

        return countries
            .Select(country => country.ToCountryResponse())
            .ToList();
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null)
            return null;

        Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

        if (country_response_from_list == null)
            return null;
        
        return country_response_from_list.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        using var stream = new MemoryStream();
        await formFile.CopyToAsync(stream);
        stream.Position = 0;

        using var package = new ExcelPackage(stream);

        var worksheet = package.Workbook.Worksheets["Countries"]
                        ?? throw new InvalidDataException("Countries worksheet not found.");

        List<Country> countries = await _countriesRepository.GetAllCountries();
        
        var existingNames = countries
            .Select(country => country.CountryName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newCountries = new List<Country>();
        var lastRow = worksheet.Dimension?.End.Row ?? 1;

        for (var row = 2; row <= lastRow; row++)
        {
            var name = worksheet.Cells[row, 1].Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || !existingNames.Add(name))
                continue;

            newCountries.Add(new Country { CountryName = name });
        }

        if (newCountries.Count == 0)
            return 0;

        await Task.WhenAll(
            newCountries.Select(country => _countriesRepository.AddCountry(country))
        );

        return newCountries.Count;
    }
}