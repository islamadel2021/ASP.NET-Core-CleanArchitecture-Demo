using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class CRUDDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public CRUDDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            List<Country> countries = CountriesToSeed();
            List<Person> persons = PersonsToSeed();

            //Fluent API
            countries.ForEach(c => modelBuilder.Entity<Country>().HasData(c));
            persons.ForEach(p => modelBuilder.Entity<Person>().HasData(p));
            modelBuilder.Entity<Person>().Property(p => p.PN)
                //.IsRequired()
                .HasColumnName("PassportNumber")
                .HasColumnType("varchar(9)")
                .HasDefaultValue("None");

            //modelBuilder.Entity<Person>().HasIndex(p => p.PN).IsUnique();

            //Table Relations
            //modelBuilder.Entity<Person>(p =>
            //p.HasOne<Country>(p => p.Country)
            //.WithMany(c => c.Persons)
            //.HasForeignKey(p => p.CountryId)
            //);
        }

        private static List<Country> CountriesToSeed()
        {
            return new List<Country>() {
        new() {  Id = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), Name = "Egypt" },

        new() { Id = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), Name = "Palestine" },

        new() { Id = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), Name = "Iraq" },

        new() { Id = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), Name = "Syria" },

        new() { Id = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), Name = "Libya" }
        };
        }
        private static List<Person> PersonsToSeed()
        {
            List<Person> _persons = new()
            {
                new() { Id = Guid.Parse("8082ED0C-396D-4162-AD1D-29A13F929824"), Name = "Muhammad Awadallah", Email = "mo@email.com", DateOfBirth = DateTime.Parse("1981-01-02"), Gender = "Male", ReceiveEmails = true, CountryId = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B") },

                new() { Id = Guid.Parse("06D15BAD-52F4-498E-B478-ACAD847ABFAA"), Name = "Amany Muhammad", Email = "amany@email.com", DateOfBirth = DateTime.Parse("1991-06-24"), Gender = "Female", ReceiveEmails = true, CountryId = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") },

                new() { Id = Guid.Parse("D3EA677A-0F5B-41EA-8FEF-EA2FC41900FD"), Name = "Khaled Jaber", Email = "kha@email.com", DateOfBirth = DateTime.Parse("1993-08-13"), Gender = "Male", ReceiveEmails = false, CountryId = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") },

                new() { Id = Guid.Parse("89452EDB-BF8C-4283-9BA4-8259FD4A7A76"), Name = "Galal Ali", Email = "galal@email.com", DateOfBirth = DateTime.Parse("1985-06-17"), Gender = "Male", ReceiveEmails = true, CountryId = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") },

                new() { Id = Guid.Parse("F5BD5979-1DC1-432C-B1F1-DB5BCCB0E56D"), Name = "Fatima Ahmad", Email = "fatima@email.com", DateOfBirth = DateTime.Parse("1996-09-02"), Gender = "Female", CountryId = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") },

                new() { Id = Guid.Parse("A795E22D-FAED-42F0-B134-F3B89B8683E5"), Name = "Batool Ali", Email = "batool@email.com", DateOfBirth = DateTime.Parse("1993-10-23"), Gender = "Female", CountryId = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D") },

                new() { Id = Guid.Parse("3C12D8E8-3C1C-4F57-B6A4-C8CAAC893D7A"), Name = "Amir Muhammad", Email = "amir@email.com", DateOfBirth = DateTime.Parse("1996-02-14"), Gender = "Male", ReceiveEmails = true, CountryId = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") },

                new() { Id = Guid.Parse("7B75097B-BFF2-459F-8EA8-63742BBD7AFB"), Name = "Basel Mahmoud", Email = "basel@email.com", DateOfBirth = DateTime.Parse("1982-05-31"), Gender = "Male", ReceiveEmails = false, CountryId = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B") },

                new() { Id = Guid.Parse("6717C42D-16EC-4F15-80D8-4C7413E250CB"), Name = "Saad Muhammad", Email = "saad@email.com", DateOfBirth = DateTime.Parse("1999-02-02"), Gender = "Male", ReceiveEmails = false, CountryId = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") },

                new() { Id = Guid.Parse("6E789C86-C8A6-4F18-821C-2ABDB2E95982"), Name = "Fareed Said", Email = "fareed@email.com", DateOfBirth = DateTime.Parse("1996-04-27"), Gender = "Male", ReceiveEmails = false, CountryId = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B") }
            };
            return _persons;
        }
        public List<Person> Sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("Execute GetAllPersons").ToList();
        }
        public int Sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
        new SqlParameter("@Id", person.Id),
        new SqlParameter("@Name", person.Name),
        new SqlParameter("@Email", person.Email),
        new SqlParameter("@DateOfBirth", person.DateOfBirth),
        new SqlParameter("@Gender", person.Gender),
        new SqlParameter("@CountryId", person.CountryId),
        new SqlParameter("@ReceiveEmails", person.ReceiveEmails)
      };
            return Database.ExecuteSqlRaw("EXECUTE InsertPerson @Id, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @ReceiveEmails", parameters);
        }

    }
}
