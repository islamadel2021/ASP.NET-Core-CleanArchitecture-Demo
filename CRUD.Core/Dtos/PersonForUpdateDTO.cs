using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    public class PersonForUpdateDTO
    {
        [Required(ErrorMessage = "Person Id can't be blank")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be a valid email")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public bool ReceiveEmails { get; set; }


        public Person ToPerson()
        {
            return new() { Id = Id, Name = Name, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryId = CountryId, ReceiveEmails = ReceiveEmails };
        }
    }
}
