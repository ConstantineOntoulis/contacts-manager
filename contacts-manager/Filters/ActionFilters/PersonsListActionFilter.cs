using Microsoft.AspNetCore.Mvc.Filters;

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
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("PersonsListActionFilter.OnActionExecuted method");
    }
}