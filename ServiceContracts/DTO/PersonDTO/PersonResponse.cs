using Entities;

namespace ServiceContracts.DTO.PersonDTO;

public class PersonResponse
{
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    /// <summary>
    /// Compares the current object data with the parameter object
    /// </summary>
    /// <param name="obj">The PersonResponse Object to compare</param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj.GetType() != typeof(PersonResponse)) return false;
        
        PersonResponse person = (PersonResponse)obj;
        return this.PersonID == person.PersonID && 
               this.PersonName == person.PersonName && 
               this.Email == person.Email && 
               this.DateOfBirth == person.DateOfBirth && 
               this.Gender == person.Gender && 
               this.CountryID == person.CountryID && 
               this.Address == person.Address && 
               this.ReceiveNewsLetters == person.ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date of Birth: {DateOfBirth?.ToString("dd MMM yyyy")}, Gender: {Gender}, CountryID: {CountryID}, Address: {Address}, ReceiveNewsLetters: {ReceiveNewsLetters}";
    }
}