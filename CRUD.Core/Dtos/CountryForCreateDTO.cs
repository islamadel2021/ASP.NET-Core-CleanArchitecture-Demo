using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    public class CountryForCreateDTO
    {
        public string? Name { get; set; }
        public Country ToCountry() => new() { Name = Name };
    }
}
