using Microsoft.EntityFrameworkCore;

namespace Entities;

public class PersonsDbContext : DbContext
{
    public PersonsDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Person> Persons { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        //Seed to Countries and Persons
        string countriesJson = System.IO.File.ReadAllText("countries.json");
        string personsJson = System.IO.File.ReadAllText("persons.json");
        
        List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
        List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
        
        foreach (Country country in countries)
        {
            modelBuilder.Entity<Country>().HasData(country);
        }

        foreach (Person person in persons)
        {
            modelBuilder.Entity<Person>().HasData(person);
        }
        
    }
}