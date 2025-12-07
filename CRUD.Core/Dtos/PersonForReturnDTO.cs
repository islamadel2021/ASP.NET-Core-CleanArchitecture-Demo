using Entities;
using Exceptions;
using ServiceContracts.Enums;

namespace ServiceContracts.DTOs
{
    public class PersonForReturnDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }
        public bool ReceiveEmails { get; set; }
        public double? Age { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonForReturnDTO)) return false;

            PersonForReturnDTO person = (PersonForReturnDTO)obj;
            return Id == person.Id && Name == person.Name && Email == person.Email && DateOfBirth == person.DateOfBirth && Gender == person.Gender && CountryId == person.CountryId && ReceiveEmails == person.ReceiveEmails;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Email: {Email}, Date of Birth: {DateOfBirth?.ToString("dd MM yyyy")}, Gender: {Gender}, Country ID: {CountryId},Country: {Country}, Receive Emails: {ReceiveEmails}";
        }

        public PersonForUpdateDTO ToPersonForUpdateDTO()
        {
            return new PersonForUpdateDTO() { Id = Id, Name = Name, Email = Email, DateOfBirth = DateOfBirth, Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!, true), CountryId = CountryId, ReceiveEmails = ReceiveEmails };
        }
    }

    public static class PersonExtensions
    {
        public static PersonForReturnDTO ToPersonForReturn(this Person person)
        {
            if (person == null)
            {
                throw new InvalidPersonException("Given person doesn't exist!");
            }
            return new()
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                CountryId = person.CountryId,
                Country = person.Country?.Name,
                Gender = person.Gender,
                ReceiveEmails = person.ReceiveEmails,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
            };
        }
    }
}
