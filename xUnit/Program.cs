using System.Globalization;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

var culture = new CultureInfo("en-GB");

builder.Services.AddControllersWithViews();

//add services into IoC container
builder.Services.AddSingleton<ICountriesService, CountriesService>();
builder.Services.AddSingleton<IPersonsService, PersonsService>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles(); 
app.UseRouting();
app.MapControllers();

app.Run();