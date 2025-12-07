using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities
{
    public class Country
    {
        public Guid Id { get; set; }

        [StringLength(15)]
        public string? Name { get; set; }

        [JsonIgnore]
        public ICollection<Person>? Persons { get; set; }
    }
}