using Entities;
using Microsoft.AspNetCore.Http;
using ServiceContracts;
using ServiceContracts.DTO.CountryDTO;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field so that several methods, and several calls to those methods, need access to the same list
    private readonly ApplicationDbContext _db;
    
    //constructor
    public CountriesService(ApplicationDbContext applicationDbContext)
    {
        _db = applicationDbContext;
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
        if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
        {
            throw new ArgumentException("Given country name already exists");
        }
        
        //Convert object from CountryAddRequest to Country Type
        Country country = countryAddRequest.ToCountry();
        
        //Generate CountryID
        country.CountryID = Guid.NewGuid();
        
        //Add country object into _countries
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await _db.Countries
            .Select(country => country.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null)
            return null;

        Country? country_response_from_list = await _db.Countries
            .FirstOrDefaultAsync(temp => temp.CountryID == countryID);

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

        var existingNames = (await _db.Countries
                .Select(c => c.CountryName)
                .ToListAsync())
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

        _db.Countries.AddRange(newCountries);
        await _db.SaveChangesAsync();

        return newCountries.Count;
    }
}