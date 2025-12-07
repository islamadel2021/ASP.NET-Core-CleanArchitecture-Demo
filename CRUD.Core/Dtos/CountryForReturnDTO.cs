using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    public class CountryForReturnDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryForReturnDTO))
            {
                return false;
            }

            CountryForReturnDTO country_to_compare = (CountryForReturnDTO)obj;

            return Id == country_to_compare.Id && Name == country_to_compare.Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public static class CountryExtensions
    {
        public static CountryForReturnDTO ToCountryForReturn(this Country country) => new() { Id = country.Id, Name = country.Name };
    }
}
