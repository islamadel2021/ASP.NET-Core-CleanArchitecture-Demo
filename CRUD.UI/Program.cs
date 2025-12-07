using CRUDUI.Filters.ActionFilters;
using CRUDUI.Middlewares;
using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Rotativa.AspNetCore;
using Serilog;
using ServiceContracts.Interfaces;
using Services;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<PersonsListActionFilter>(4);

    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
    options.Filters.Add(new ResponseHeaderActionFilter(logger) { Key = "Global-Key", Value = "Global-value", Order = 2 });
});
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsAdderService, PersonsAdderService>();
builder.Services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>();
builder.Services.AddScoped<PersonsGetterService, PersonsGetterService>();
builder.Services.AddScoped<IPersonsSorterService, PersonsSorterService>();
builder.Services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
builder.Services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddDbContext<CRUDDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<PersonsListActionFilter>();
builder.Services.AddScoped<ResponseHeaderActionFilter>();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

//Enable Identity 
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    //options.Password.RequireDigit = false;
    //options.Password.RequireLowercase = false;
    //options.Password.RequireUppercase = false;
    //options.Password.RequireNonAlphanumeric = false;
    //options.Password.RequiredLength = 2;
})

 .AddEntityFrameworkStores<CRUDDbContext>()

 .AddDefaultTokenProviders()

 .AddUserStore<UserStore<ApplicationUser, ApplicationRole, CRUDDbContext, Guid>>()

 .AddRoleStore<RoleStore<ApplicationRole, CRUDDbContext, Guid>>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseExceptionHandling();
}

if (builder.Environment.IsEnvironment("Test") == false)
    RotativaConfiguration.Setup(builder.Environment.WebRootPath);

app.UseHttpLogging();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var userManager = builder.Services.BuildServiceProvider().GetRequiredService<UserManager<ApplicationUser>>();

var roleManager = builder.Services.BuildServiceProvider().GetRequiredService<RoleManager<ApplicationRole>>();

await IdentityInitializer.Initialize(userManager, roleManager);

app.Run();

public partial class Program { }
