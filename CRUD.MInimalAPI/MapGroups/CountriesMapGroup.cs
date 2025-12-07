using CRUDMinimalAPI.EndpointFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRUDMinimalAPI.MapGroups
{
    public static class CountriesMapGroup
    {
        public static RouteGroupBuilder CountriesAPI(this RouteGroupBuilder group)
        {
            // GET: api/Countries
            group.MapGet("/", async (CRUDDbContext dbContext) =>
                await dbContext.Countries.ToListAsync());

            // GET: api/Countries/5
            group.MapGet("/{id}", async (Guid id, CRUDDbContext dbcontext) =>
                await dbcontext.Countries.FindAsync(id)
                    is Country country
                        ? Results.Ok(country)
                        : Results.NotFound());

            // PUT: api/Countries/5
            group.MapPut("/{id}", async (Guid id, Country country, CRUDDbContext dbcontext) =>
            {
                var countryToUpdate = await dbcontext.Countries.FindAsync(id);
                if (countryToUpdate is null) return Results.NotFound();
                countryToUpdate.Name = country.Name;
                await dbcontext.SaveChangesAsync();
                return Results.NoContent();
            });

            // POST: api/Countries
            group.MapPost("/", async (Country country, CRUDDbContext dbcontext) =>
            {
                dbcontext.Countries.Add(country);
                await dbcontext.SaveChangesAsync();

            }).AddEndpointFilter<CustomEndpointFilter>()
                .AddEndpointFilter(async (EndpointFilterInvocationContext context, EndpointFilterDelegate next) =>
            {
                //Before Endpoint Execution
                var country = context.Arguments.OfType<Country>().FirstOrDefault();
                if (string.IsNullOrEmpty(country?.Name)) return Results.BadRequest("Country name can't be blank!");
                var validationContext = new ValidationContext(country);
                List<ValidationResult> errors = new();
                bool isValid = Validator.TryValidateObject(country, validationContext, errors, true);

                if (!isValid)
                {
                    return Results.BadRequest(errors.FirstOrDefault()?.ErrorMessage);
                }
                await next(context); //invokes the subsequent endpoint filter or endpoint's request delegate

                //After Endpoint Execution

                return Results.Created($"/api/countries/{country.Id}", country);

            });

            // DELETE: api/Countries/5
            group.MapDelete("/{id}", async (Guid id, CRUDDbContext dbcontext) =>
            {
                if (await dbcontext.Countries.FindAsync(id) is Country country)
                {
                    dbcontext.Countries.Remove(country);
                    await dbcontext.SaveChangesAsync();
                    return Results.Ok(country);
                }
                return Results.NotFound();
            });
            return group;
        }
    }
}
