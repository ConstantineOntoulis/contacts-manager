using ServiceContracts.DTO.PersonDTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Person Entity
/// </summary>
public interface IPersonsService
{
    /// <summary>
    /// Adds a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest">Person to add</param>
    /// <returns>Returns the same person details,along with newly generated PersonID</returns>
    PersonResponse AddPerson(PersonAddRequest personAddRequest);

    /// <summary>
    /// Returns all people
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse type</returns>
    List<PersonResponse> GetAllPersons();
}