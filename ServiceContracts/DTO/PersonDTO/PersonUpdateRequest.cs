using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO.PersonDTO;

public class PersonUpdateRequest
{
    /// <summary>
    /// Acts as a DTO for updating an existing person
    /// </summary>
    [Required]
    public Guid PersonID { get; set; }
    [Required(ErrorMessage = "Person Name can't be blank")]
    public string? PersonName { get; set; }
    
    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Email value should be a valid email")]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object of Person type
    /// </summary>
    /// <returns>Returns person object</returns>
    public Person ToPerson()
    {
        return new Person()
        {
            PersonID = PersonID,
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            Address = Address,
            CountryID = CountryID,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}