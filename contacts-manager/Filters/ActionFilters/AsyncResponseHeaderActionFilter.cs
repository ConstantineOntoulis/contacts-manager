using Microsoft.AspNetCore.Mvc.Filters;

namespace contacts.Filters.ActionFilters;

public class AsyncResponseHeaderActionFilter : IAsyncActionFilter
{
    private readonly ILogger<AsyncResponseHeaderActionFilter> _logger;
    private readonly string _key;
    private readonly string _value;
    
    public AsyncResponseHeaderActionFilter(ILogger<AsyncResponseHeaderActionFilter> logger, string key, string value)
    {
        _logger = logger;
        _key = key;
        _value = value;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //Before Logic
        _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(AsyncResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
        
        await next(); //calls the subsequent filter or action method

        //After Logic
        _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(AsyncResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
        
        context.HttpContext.Response.Headers[_key] = _value;
    }
}