using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO.PersonDTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    //Private field
    private readonly ApplicationDbContext _db;
    private readonly ICountriesService _countriesService;

    //constructor
    public PersonsService(ApplicationDbContext applicationDbContext, ICountriesService countriesService)
    {
        _db = applicationDbContext;
        _countriesService = countriesService;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        //check if PersonAddRequest is not null
        if (personAddRequest == null)
        {
            throw new ArgumentNullException(nameof(personAddRequest));
        }

        //Model Validations
        ValidationHelper.ModelValidation(personAddRequest);

        //convert personAddRequest into Person Type
        Person person = personAddRequest.ToPerson();

        //generate PersonID
        person.PersonID = Guid.NewGuid();

        //add person object to persons list
        _db.Persons
            .Add(person);
        await _db.SaveChangesAsync();
        //_db.sp_AddPerson(person);
        
        //convert the Person Object into PersonResponse type
        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var persons = await _db.Persons.Include("Country").ToListAsync();
        
        //SELECT * from Persons
        return persons
            .Select(temp => temp.ToPersonResponse()).ToList();
        
        /*
        return _db.sp_GetAllPersons()
            .Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        */
        //Need to do the same or UPDATE and DELETE Person!!
    }

    public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
    {
        if (personID == null)
            return null;

        Person? person = await _db.Persons
            .Include("Country")
            .FirstOrDefaultAsync(temp => temp.PersonID == personID);

        if (person == null)
            return null;

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPeople = await GetAllPersons();
        List<PersonResponse> matchingPeople = allPeople;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return matchingPeople;

        switch (searchBy)
        {
            case nameof(PersonResponse.PersonName):
                matchingPeople = allPeople.Where(temp =>
                    temp.PersonName?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.Email):
                matchingPeople = allPeople.Where(temp =>
                    temp.Email?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.DateOfBirth):
                matchingPeople = allPeople.Where(temp =>
                    temp.DateOfBirth?.ToString("dd MMM yyyy")
                        .Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.Gender):
                matchingPeople = allPeople.Where(temp =>
                    temp.Gender?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.CountryID):
                matchingPeople = allPeople.Where(temp =>
                    temp.Country?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            case nameof(PersonResponse.Address):
                matchingPeople = allPeople.Where(temp =>
                    temp.Address?.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) == true).ToList();
                break;

            default:
                matchingPeople = allPeople;
                break;
        }

        return matchingPeople;
    }

    public async Task<List<PersonResponse>> GetSortedPeople(List<PersonResponse> allPeople, string sortBy,
        SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return allPeople;

        List<PersonResponse> sortedPeople = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Age).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Age).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                => allPeople.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
                => allPeople.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

            _ => allPeople
        };

        return sortedPeople;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
            throw new ArgumentNullException(nameof(Person));

        //validation
        ValidationHelper.ModelValidation(personUpdateRequest);

        //get matching person object to update
        Person? matchingPerson = await _db.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);

        if (matchingPerson == null)
        {
            throw new ArgumentException("Given person id doesn't exist");
        }

        //update all details
        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryID = personUpdateRequest.CountryID;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        //_db.sp_UpdatePerson(matchingPerson); //UPDATE
        await _db.SaveChangesAsync();
        
        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
        if (personID == null)
        {
            throw new ArgumentNullException(nameof(personID));
        }

        Person? person = await _db.Persons
            .FirstOrDefaultAsync(temp => temp.PersonID == personID.Value);

        if (person == null)
            return false;

        _db.Persons.Remove(person);

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        
        CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);
        
        //Writes all property names as headings. Eg: Id, Name
        //csvWriter.WriteHeader<PersonResponse>(); //PersonID, PersonName...
        
        //PersonName, Email, DateOfBirth, Age, Gender, Country, Address, ReceiveNewsLetters
        csvWriter.WriteField(nameof(PersonResponse.PersonName));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField(nameof(PersonResponse.Age));
        csvWriter.WriteField(nameof(PersonResponse.Gender));
        csvWriter.WriteField(nameof(PersonResponse.Country));
        csvWriter.WriteField(nameof(PersonResponse.Address));
        csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
        
        csvWriter.NextRecord();

        List<PersonResponse> persons = _db.Persons
            .Include("Country")
            .Select(temp => temp.ToPersonResponse()).ToList();

        foreach (PersonResponse person in persons)
        {
            csvWriter.WriteField(person.PersonName);
            csvWriter.WriteField(person.Email);
            if (person.DateOfBirth.HasValue)
                csvWriter.WriteField(person.DateOfBirth.Value.ToString("dd-MM-yyyy"));
            else
                csvWriter.WriteField("");
            
            csvWriter.WriteField(nameof(person.Age));
            csvWriter.WriteField(nameof(person.Gender));
            csvWriter.WriteField(nameof(person.Country));
            csvWriter.WriteField(nameof(person.Address));
            csvWriter.WriteField(nameof(person.ReceiveNewsLetters));
            
            csvWriter.NextRecord();
            csvWriter.Flush();
        }
        
        //Writes all objects in the given collection Eg: Id, Name
        //                                               1, abc
        //                                               2, def
        //await csvWriter.WriteRecordsAsync(persons);
        //1,abc,...
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        MemoryStream memoryStream = new MemoryStream();
        using (ExcelPackage excelPackage = new ExcelPackage())
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
            
            workSheet.Cells["A1"].Value = "Person Name";
            workSheet.Cells["B1"].Value = "Email";
            workSheet.Cells["C1"].Value = "Date of Birth";
            workSheet.Cells["D1"].Value = "Age";
            workSheet.Cells["E1"].Value = "Gender";
            workSheet.Cells["F1"].Value = "Country";
            workSheet.Cells["G1"].Value = "Address";
            workSheet.Cells["H1"].Value = "Receive News Letters";

            using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                headerCells.Style.Font.Bold = true;
            }
            
            int row = 2;
            
            List<PersonResponse> persons = _db.Persons
                .Include("Country")
                .Select(temp => temp.ToPersonResponse())
                .ToList();

            foreach (PersonResponse person in persons)
            {
                workSheet.Cells[row, 1].Value = person.PersonName;
                workSheet.Cells[row, 2].Value = person.Email;
                if (person.DateOfBirth.HasValue)
                    workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("dd-MM-yyyy");
                workSheet.Cells[row, 4].Value = person.Age;
                workSheet.Cells[row, 5].Value = person.Gender;
                workSheet.Cells[row, 6].Value = person.Country;
                workSheet.Cells[row, 7].Value = person.Address;
                workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                row++;
            }
            
            workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

            await excelPackage.SaveAsAsync(memoryStream);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}