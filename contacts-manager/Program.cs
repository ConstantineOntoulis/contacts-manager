using System.Globalization;
using Entities;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialPersonal("Tino");

var culture = new CultureInfo("en-GB");

builder.Services.AddControllersWithViews();

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

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();