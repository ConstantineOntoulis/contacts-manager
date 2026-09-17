using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field so that several methods, and several calls to those methods, need access to the same list
    private readonly List<Country> _countries;
    
    //constructor
    public CountriesService()
    {
        _countries = new List<Country>();
    }
    
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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
        if (_countries.Any(temp => temp.CountryName == countryAddRequest.CountryName))
        {
            throw new ArgumentException("Given country name already exists");
        }
        
        //Convert object from CountryAddRequest to Country Type
        Country country = countryAddRequest.ToCountry();
        
        //Generate CountryID
        country.CountryID = Guid.NewGuid();
        
        //Add country object into _countries
        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryID(Guid? countryID)
    {
        throw new NotImplementedException();
    }
}