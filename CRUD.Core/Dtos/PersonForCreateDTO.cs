using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs
{
    public class PersonForCreateDTO
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be a valid email")]

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Select country")]
        public Guid? CountryId { get; set; }

        [Required]
        public bool ReceiveEmails { get; set; }


        public Person ToPerson()
        {
            return new() { Name = Name, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryId = CountryId, ReceiveEmails = ReceiveEmails };
        }
    }
}
