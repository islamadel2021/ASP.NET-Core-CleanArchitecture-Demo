using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Person
    {
        public Guid Id { get; set; }

        [StringLength(40)]
        public string? Name { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(6)]
        public string? Gender { get; set; }

        public Guid? CountryId { get; set; } //Foreign Key

        //[ForeignKey("CountryId")]
        public Country? Country { get; set; } //Navigation Property

        public bool ReceiveEmails { get; set; } // bit 

        public string? PN { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Email: {Email}, Date of Birth: {DateOfBirth?.ToString("dd/MM/yyyy")}, Gender: {Gender}, Country: {Country?.Name}, Receive Emails: {ReceiveEmails}";
        }
    }
}
