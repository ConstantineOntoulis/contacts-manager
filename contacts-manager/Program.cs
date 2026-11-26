using System.Globalization;
using contacts.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Logging
//Serilog:
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //reading configuration setting from built-in IConfiguration
        .ReadFrom.Services(services); //read out current app's services and make them available to Serilog
});

/* Built-in Logging:
builder.Host.ConfigureLogging(loggingProvider =>
{
    loggingProvider.ClearProviders();
    loggingProvider.AddConsole();
    loggingProvider.AddDebug();
    loggingProvider.AddEventLog();
});
*/

ExcelPackage.License.SetNonCommercialPersonal("Tino");

var culture = new CultureInfo("en-GB");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new TypeFilterAttribute(typeof(ResponseHeaderActionFilter))
    {
        Arguments = new object[]
        {
            "My-Key-From-Global",
            "My-Value-From-Global"
        }
    });
});

//add services into IoC container
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ContactsDb;Integrated Security=True;Encrypt=False
// Linux Connection String:
// Server=127.0.0.1,1433;Database=ContactsManagerDb;User Id=sa;Password=ChooseAReallyStrongPassword123!;Encrypt=True;TrustServerCertificate=True;

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");
    
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();