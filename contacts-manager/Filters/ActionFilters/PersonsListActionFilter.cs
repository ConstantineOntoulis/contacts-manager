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
        context.HttpContext.Items["arguments"] =  context.ActionArguments;
        
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
                    
                    _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("PersonsListActionFilter.OnActionExecuted method");
        
        HomeController homeController = (HomeController)context.Controller;

        IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

        if (parameters != null)
        {
            if (parameters.ContainsKey("searchBy"))
            {
                homeController.ViewData["searchBy"] = Convert.ToString(parameters["searchBy"]);
            }
            
            if (parameters.ContainsKey("searchString"))
            {
                homeController.ViewData["searchString"] = Convert.ToString(parameters["searchString"]);
            }
            
            if (parameters.ContainsKey("sortBy"))
            {
                homeController.ViewData["sortBy"] = Convert.ToString(parameters["sortBy"]);
            }
            
            if (parameters.ContainsKey("sortOrder"))
            {
                homeController.ViewData["sortOrder"] = Convert.ToString(parameters["sortOrder"]);
            }
        }
        
    }
}