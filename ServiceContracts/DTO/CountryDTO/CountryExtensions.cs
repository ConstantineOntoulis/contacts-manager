using Entities;

namespace ServiceContracts.DTO.CountryDTO;

/// <summary>
/// The extension version is chosen because this: country.ToCountryResponse(); ,reads naturally as 
/// "take this country object and convert it to a CountryResponse", as opposed to this
/// CountryExtensions.ToCountryResponse(country);  and also emphasizes the thing being transformed
/// </summary>
public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryID = country.CountryID,
            CountryName = country.CountryName
        };
    }
}