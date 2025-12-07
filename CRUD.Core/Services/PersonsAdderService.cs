using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTOs;
using ServiceContracts.Interfaces;
using Services.Helpers;

namespace Services
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public async Task<PersonForReturnDTO> AddPerson(PersonForCreateDTO? personForCreateDTO)
        {
            //check if PersonForCreateDTO is not null
            if (personForCreateDTO == null)
            {
                throw new ArgumentNullException(nameof(personForCreateDTO));
            }

            //Model Validations
            Helper.ValidateModel(personForCreateDTO);
            //convert personForCreateDTO into Person type
            Person person = personForCreateDTO.ToPerson();

            //generate Person Id
            person.Id = Guid.NewGuid();

            //add person object to persons list
            await _personsRepository.AddPerson(person);
            //_dbContext.Sp_InsertPerson(person);

            //convert the Person object into PersonForReturnDTO type

            return person.ToPersonForReturn();
        }

    }
}
