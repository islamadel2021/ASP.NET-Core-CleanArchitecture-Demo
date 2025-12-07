using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTOs;
using ServiceContracts.Interfaces;
using Services.Helpers;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<PersonForReturnDTO> UpdatePerson(PersonForUpdateDTO? personForUpdateDTO)
        {
            if (personForUpdateDTO == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            Helper.ValidateModel(personForUpdateDTO);

            //get matching person object to update
            Person? personForUpdate = await _personsRepository.GetPersonById(personForUpdateDTO.Id);
            if (personForUpdate == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //update all details
            personForUpdate.Name = personForUpdateDTO.Name;
            personForUpdate.Email = personForUpdateDTO.Email;
            personForUpdate.DateOfBirth = personForUpdateDTO.DateOfBirth;
            personForUpdate.Gender = personForUpdateDTO.Gender.ToString();
            personForUpdate.CountryId = personForUpdateDTO.CountryId;
            personForUpdate.ReceiveEmails = personForUpdateDTO.ReceiveEmails;
            await _personsRepository.UpdatePerson(personForUpdate);
            return personForUpdate.ToPersonForReturn();
        }
    }
}
