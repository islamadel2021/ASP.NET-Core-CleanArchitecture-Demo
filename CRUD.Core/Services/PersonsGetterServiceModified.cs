using CsvHelper;
using CsvHelper.Configuration;
using ServiceContracts.DTOs;
using ServiceContracts.Interfaces;
using System.Globalization;

namespace Services
{
    public class PersonsGetterServiceModified : IPersonsGetterService
    {
        private readonly PersonsGetterService _personsGetterService;

        public PersonsGetterServiceModified(PersonsGetterService personsGetterService)
        {
            _personsGetterService = personsGetterService;
        }

        public async Task<List<PersonForReturnDTO>> GetAllPersons()
        {
            return await _personsGetterService.GetAllPersons();
        }

        public async Task<List<PersonForReturnDTO>> GetFilteredPersons(string searchBy, string? searchString)
        {
            return await _personsGetterService.GetFilteredPersons(searchBy, searchString);
        }

        public async Task<PersonForReturnDTO?> GetPersonById(Guid? id)
        {
            return await _personsGetterService.GetPersonById(id);
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new();
            StreamWriter streamWriter = new(memoryStream);
            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonForReturnDTO.Id));
            csvWriter.WriteField(nameof(PersonForReturnDTO.Name));
            csvWriter.NextRecord();

            List<PersonForReturnDTO> persons = await GetAllPersons();
            persons.ForEach(p =>
            {
                csvWriter.WriteField(p.Id);
                csvWriter.WriteField(p.Name);
                csvWriter.NextRecord();
                csvWriter.Flush();
            });
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
