using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTOs;
using System.Globalization;

namespace Services
{
    public class PersonsGetterServiceChild : PersonsGetterService
    {
        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext) : base(personsRepository, logger, diagnosticContext)
        {
        }
        public async override Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new();
            StreamWriter streamWriter = new(memoryStream);
            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonForReturnDTO.Id));
            csvWriter.WriteField(nameof(PersonForReturnDTO.Name));
            csvWriter.WriteField(nameof(PersonForReturnDTO.Age));
            csvWriter.NextRecord();

            List<PersonForReturnDTO> persons = await GetAllPersons();
            //if(persons.Count == 0)
            //{
            //    throw new InvalidOperationException("No persons Data");
            //}
            persons.ForEach(p =>
            {
                csvWriter.WriteField(p.Id);
                csvWriter.WriteField(p.Name);
                csvWriter.WriteField(p.Age);
                csvWriter.NextRecord();
                csvWriter.Flush();
            });
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
