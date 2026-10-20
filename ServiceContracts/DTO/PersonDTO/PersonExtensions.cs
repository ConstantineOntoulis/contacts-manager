using Entities;

namespace ServiceContracts.DTO.PersonDTO;

public static class PersonExtensions
{
    /// <summary>
    /// An extension method to convert an object of Person class into PersonResponse class
    /// </summary>
    /// <param name="person">The Person object to convert</param>
    /// <returns>Returns the converted PersonResponse object</returns>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        //person => PersonResponse
        return new PersonResponse()
        {
            PersonID = person.PersonID,
            PersonName = person.PersonName, Email = person.Email, 
            DateOfBirth = person.DateOfBirth,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Address = person.Address,
            CountryID = person.CountryID,
            Gender = person.Gender,
            Age = (person.DateOfBirth != null)? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
            Country = person.Country?.CountryName
        };
    }
}