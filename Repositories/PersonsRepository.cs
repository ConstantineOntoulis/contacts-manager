using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository : IPersonsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PersonsRepository> _logger;
    
    public PersonsRepository(ApplicationDbContext dbContext, ILogger<PersonsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<Person> AddPerson(Person person)
    {
        _dbContext.Persons.Add(person);
        await _dbContext.SaveChangesAsync();
        
        return person;
    }

    public async Task<bool> DeletePersonByPersonID(Guid? personID)
    {
        _dbContext.Persons.RemoveRange(_dbContext.Persons.Where(p => p.PersonID == personID));
        int rowsDeleted = await _dbContext.SaveChangesAsync();
        
        return rowsDeleted > 0;
    }
    
    public async Task<List<Person>> GetAllPersons()
    {
        return await _dbContext.Persons.Include("Country").ToListAsync();
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        _logger.LogInformation("GetFilteredPersons of PersonsRepository");
        
        return await _dbContext.Persons.Include("Country")
            .Where(predicate)
            .ToListAsync();
    }
    
    public async Task<Person?> GetPersonByPersonID(Guid? personID)
    {
        return await _dbContext.Persons.Include("Country")
            .FirstOrDefaultAsync(temp => temp.PersonID == personID);
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        Person matchingPerson = await _dbContext.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);

        if (matchingPerson == null)
            return person;

        matchingPerson.PersonName = person.PersonName;
        matchingPerson.Email = person.Email;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.Gender = person.Gender;
        matchingPerson.CountryID = person.CountryID;
        matchingPerson.Address = person.Address;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
        
        int countUpdated = await _dbContext.SaveChangesAsync();
        
        return matchingPerson;
    }
}