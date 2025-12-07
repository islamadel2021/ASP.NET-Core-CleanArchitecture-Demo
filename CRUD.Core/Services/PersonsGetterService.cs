using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts.DTOs;
using ServiceContracts.Interfaces;
using System.Globalization;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<List<PersonForReturnDTO>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(p => p.ToPersonForReturn()).ToList();
        }
        public async Task<List<PersonForReturnDTO>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons method of PersonsService");

            List<PersonForReturnDTO> allPersons = await GetAllPersons();
            List<PersonForReturnDTO> matchingPersons = allPersons;
            List<Person> persons = new();

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            using (Operation.Time("Time to get filtered persons from database"))
            {
                persons = searchBy switch
                {
                    nameof(Person.Name) => await _personsRepository.GetFilteredPersons(p => p.Name!.StartsWith(searchString)),

                    nameof(Person.Email) => await _personsRepository.GetFilteredPersons(p => p.Email!.StartsWith(searchString)),

                    nameof(Person.DateOfBirth) => await _personsRepository.GetFilteredPersons(p => p.DateOfBirth!.Value.ToString().Contains(searchString)),

                    nameof(Person.Gender) => await _personsRepository.GetFilteredPersons(p => p.Gender!.StartsWith(searchString)),

                    nameof(Person.Country) => await _personsRepository.GetFilteredPersons(p => p.Country!.Name!.StartsWith(searchString)),

                    _ => await _personsRepository.GetAllPersons(),
                };
            }
            _diagnosticContext.Set("Persons", persons);
            return persons.Select(p => p.ToPersonForReturn()).ToList();
        }
        public async Task<PersonForReturnDTO?> GetPersonById(Guid? id)
        {
            if (id == null) return null;
            Person? person = await _personsRepository.GetPersonById(id.Value);
            if (person == null) return null;
            return person.ToPersonForReturn();
        }
        public virtual async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new();
            StreamWriter streamWriter = new(memoryStream);
            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonForReturnDTO.Id));
            csvWriter.WriteField(nameof(PersonForReturnDTO.Name));
            csvWriter.WriteField(nameof(PersonForReturnDTO.DateOfBirth));
            csvWriter.NextRecord();

            List<PersonForReturnDTO> persons = await GetAllPersons();
            persons.ForEach(p =>
            {
                csvWriter.WriteField(p.Id);
                csvWriter.WriteField(p.Name);
                csvWriter.WriteField(p.DateOfBirth!.Value.ToString("dd-MM-yyyy"));
                csvWriter.NextRecord();
                csvWriter.Flush();
            });
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

    }
}
