using Entities;
using ServiceContracts;
using ServiceContracts.DTO.CountryDTO;

namespace Services;

public class CountriesService : ICountriesService
{
    //private field so that several methods, and several calls to those methods, need access to the same list
    private readonly List<Country> _countries;
    
    //constructor
    public CountriesService(bool initialize = true)
    {
        _countries = new List<Country>();

        if (initialize)
        {
            _countries.AddRange(new List<Country>()
            {
                new Country()
                {
                    CountryID = Guid.Parse("BBE7AB87-3435-4C72-8188-4F52BF7D0987"),
                    CountryName = "Greece"
                },
            
                new Country()
                {
                    CountryID = Guid.Parse("A001039D-A3A3-4BD2-8814-C6F0647048B5"),
                    CountryName = "Italy"
                },
            
                new Country()
                {
                    CountryID = Guid.Parse("5FBA1034-D1CC-41C0-918A-324A6F0FB6CD"),
                    CountryName = "Spain"
                },
            
                new Country()
                {
                    CountryID = Guid.Parse("32D9CA63-EF74-4CBA-B829-80FBE03B65E8"),
                    CountryName = "Serbia"
                },
            
                new Country()
                {
                    CountryID = Guid.Parse("D2937E39-1913-49C8-B39D-C3F7FBFF42C2"),
                    CountryName = "Cyprus"
                }
            });

            
        }
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
        if (countryID == null)
            return null;

        Country? country_response_from_list = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

        if (country_response_from_list == null)
            return null;
        
        return country_response_from_list.ToCountryResponse();
    }
}