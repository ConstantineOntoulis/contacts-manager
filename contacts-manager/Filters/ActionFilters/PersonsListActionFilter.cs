using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO.PersonDTO;

namespace contacts.Filters.ActionFilters;

public class PersonsListActionFilter : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> _logger;

    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("PersonsListActionFilter.OnActionExecuting method");

        if (context.ActionArguments.ContainsKey("searchBy"))
        {
            string? searchBy =  Convert.ToString(context.ActionArguments["searchBy"]);

            //Validate the searchBy parameter value
            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchByOptions = new List<string>()
                {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.Address),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryID),
                    nameof(PersonResponse.ReceiveNewsLetters)
                };

                //Reset the searchBy parameter value
                if (searchByOptions.Contains(searchBy) == false)
                {
                    _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                    
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    
                    _logger.LogInformation("searchBy updated value {searchBy}", searchBy);
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("PersonsListActionFilter.OnActionExecuted method");
    }
}