using ServiceContracts.Enums;
using Entities;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO.PersonDTO;

public class PersonAddRequest
{
    /// <summary>
    /// Acts as a DTO for inserting a new person
    /// </summary>
    [Required(ErrorMessage = "Person Name can't be blank")]
    [StringLength(50, ErrorMessage = "Person Name cannot exceed 50 characters")]
    public string? PersonName { get; set; }
    
    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Email value should be a valid email")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    
    [Required(ErrorMessage = "Please select a gender")]
    public GenderOptions? Gender { get; set; }
    
    [Required(ErrorMessage = "Please select a country")]
    public Guid? CountryID { get; set; }
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object of Person type
    /// </summary>
    /// <returns></returns>
    public Person ToPerson()
    {
        return new Person()
        {
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