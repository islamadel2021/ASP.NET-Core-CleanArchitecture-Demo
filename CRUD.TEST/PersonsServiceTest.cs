using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.Interfaces;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixtuter;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsAdderService> _loggerAdder;
        private readonly Mock<ILogger<PersonsAdderService>> _loggerAdderMock;
        private readonly ILogger<PersonsGetterService> _loggerGetter;
        private readonly Mock<ILogger<PersonsGetterService>> _loggerGetterMock;
        private readonly ILogger<PersonsSorterService> _loggerSorter;
        private readonly Mock<ILogger<PersonsSorterService>> _loggerSorterMock;
        private readonly ILogger<PersonsUpdaterService> _loggerUpdater;
        private readonly Mock<ILogger<PersonsUpdaterService>> _loggerUpdaterMock;
        private readonly ILogger<PersonsDeleterService> _loggerDeleter;
        private readonly Mock<ILogger<PersonsDeleterService>> _loggerDeleterMock;
        private readonly Mock<IDiagnosticContext> _diagnosticContextMock;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixtuter = new Fixture();
            _loggerAdderMock = new Mock<ILogger<PersonsAdderService>>();
            _loggerAdder = _loggerAdderMock.Object;
            _loggerGetterMock = new Mock<ILogger<PersonsGetterService>>();
            _loggerGetter = _loggerGetterMock.Object;
            _loggerSorterMock = new Mock<ILogger<PersonsSorterService>>();
            _loggerSorter = _loggerSorterMock.Object;
            _loggerUpdaterMock = new Mock<ILogger<PersonsUpdaterService>>();
            _loggerUpdater = _loggerUpdaterMock.Object;
            _loggerDeleterMock = new Mock<ILogger<PersonsDeleterService>>();
            _loggerDeleter = _loggerDeleterMock.Object;

            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            _diagnosticContextMock = new Mock<IDiagnosticContext>();
            _diagnosticContext = _diagnosticContextMock.Object;
            _personsAdderService = new PersonsAdderService(_personsRepository, _loggerAdder, _diagnosticContext);
            _personsGetterService = new PersonsGetterService(_personsRepository, _loggerGetter, _diagnosticContext);
            _personsSorterService = new PersonsSorterService(_personsRepository, _loggerSorter, _diagnosticContext);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, _loggerUpdater, _diagnosticContext);
            _personsDeleterService = new PersonsDeleterService(_personsRepository, _loggerDeleter, _diagnosticContext);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonForCreateDTO, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonForCreateDTO? personForCreateDTO = null;

            //Act
            var actual = async () =>
            {
                await _personsAdderService.AddPerson(personForCreateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>();
        }


        //When we supply null value as Person Name, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonForCreateDTO? personForCreateDTO = _fixtuter.Build<PersonForCreateDTO>()
                .With(p => p.Name, null as string)
                .Create();
            Person person = personForCreateDTO.ToPerson();
            _personsRepositoryMock
                .Setup(pr => pr.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            var actual = async () =>
            {
                await _personsAdderService.AddPerson(personForCreateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply proper person details, it should insert the person into the persons list; 	and it should return an object of PersonForReturnDTO, which includes with the newly 	generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonForCreateDTO? personForCreateDTO = _fixtuter.Build<PersonForCreateDTO>()
                .With(p => p.Email, "someone@email.com")
                .Create();
            Person person = personForCreateDTO.ToPerson();
            PersonForReturnDTO expected_person = person.ToPersonForReturn();
            _personsRepositoryMock
                .Setup(pr => pr.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act
            PersonForReturnDTO personForReturnDTO_from_add = await _personsAdderService.AddPerson(personForCreateDTO);
            expected_person.Id = personForReturnDTO_from_add.Id;

            //Assert
            personForReturnDTO_from_add.Id.Should().NotBe(Guid.Empty);

            personForReturnDTO_from_add.Should().Be(expected_person);

        }

        #endregion

        #region GetPersonById

        //If we supply null as Person Id, it should return null as PersonForReturnDTO
        [Fact]
        public async Task GetPersonById_NullPersonId()
        {
            //Arrange
            Guid? personId = null;

            //Act
            PersonForReturnDTO? personForReturnDTO_from_get = await _personsGetterService.GetPersonById(personId);

            //Assert
            personForReturnDTO_from_get.Should().BeNull();
            //Assert.Null(personForReturnDTO_from_get);
        }


        //If we supply a valid person id, it should return the valid person details as PersonForReturnDTO object
        [Fact]
        public async Task GetPersonById_WithPersonId()
        {
            //Arange
            Person? person = _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone@email.com")
                 .With(p => p.Country, null as Country)
                 .Create();
            PersonForReturnDTO expected_person = person.ToPersonForReturn();
            _personsRepositoryMock
                .Setup(pr => pr.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            PersonForReturnDTO? personForReturnDTO_from_get = await _personsGetterService.GetPersonById(person.Id);

            //Assert
            personForReturnDTO_from_get.Should().Be(expected_person);

        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personsRepositoryMock
                .Setup(pr => pr.GetAllPersons())
                .ReturnsAsync(persons);
            //Act
            List<PersonForReturnDTO> persons_from_get = await _personsGetterService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            List<Person> persons = new()
            {
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone1@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone2@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone3@email.com")
                 .With(p => p.Country, null as Country)
                 .Create()
        };
            List<PersonForReturnDTO> expected_persons_list = persons.Select(p => p.ToPersonForReturn()).ToList();
            _personsRepositoryMock
                .Setup(pr => pr.GetAllPersons())
                .ReturnsAsync(persons);
            //Display expected data
            _testOutputHelper.WriteLine("Expected data:");
            expected_persons_list.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Act
            List<PersonForReturnDTO> persons_list_from_get = await _personsGetterService.GetAllPersons();
            //Display actual data
            _testOutputHelper.WriteLine("Actual data:");
            persons_list_from_get.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Assert
            persons_list_from_get.Should().BeEquivalentTo(expected_persons_list);
        }
        #endregion

        #region GetFilteredPersons
        //If the search text is empty and search by is "Name", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            List<Person> persons = new()
            {
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone1@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone2@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone3@email.com")
                 .With(p => p.Country, null as Country)
                 .Create()
        };
            List<PersonForReturnDTO> expected_persons_list = persons.Select(p => p.ToPersonForReturn()).ToList();
            _personsRepositoryMock
                .Setup(pr => pr.GetAllPersons())
                .ReturnsAsync(persons);

            _personsRepositoryMock
               .Setup(pr => pr.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
               .ReturnsAsync(persons);

            //Display Expected Data
            _testOutputHelper.WriteLine("Expected:");
            expected_persons_list.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Act
            List<PersonForReturnDTO> persons_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.Name), "");

            //Display Actual Data
            _testOutputHelper.WriteLine("Actual:");
            persons_list_from_search.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(expected_persons_list);

        }

        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            List<Person> persons = new()
            {
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone1@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone2@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone3@email.com")
                 .With(p => p.Country, null as Country)
                 .Create()
        };
            List<PersonForReturnDTO> expected_persons_list = persons.Select(p => p.ToPersonForReturn()).ToList();
            _personsRepositoryMock
                .Setup(pr => pr.GetAllPersons())
                .ReturnsAsync(persons);

            _personsRepositoryMock
               .Setup(pr => pr.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
               .ReturnsAsync(persons);
            //Display Expected Data
            _testOutputHelper.WriteLine("Expected:");
            expected_persons_list.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Act
            List<PersonForReturnDTO> persons_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.Name), "am");

            //Display Actual Data
            _testOutputHelper.WriteLine("Actual:");
            persons_list_from_search.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(expected_persons_list);

        }
        #endregion

        #region GetSortedPersons

        //When we sort based on Person Name in ASC, it should return persons list in ascending on Person Name
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            List<Person> persons = new()
            {
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone1@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone2@email.com")
                 .With(p => p.Country, null as Country)
                 .Create(),
            _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone3@email.com")
                 .With(p => p.Country, null as Country)
                 .Create()
        };
            List<PersonForReturnDTO> expected_persons_list = persons.Select(p => p.ToPersonForReturn()).ToList();
            _personsRepositoryMock
                .Setup(pr => pr.GetAllPersons())
                .ReturnsAsync(persons);

            _personsRepositoryMock
               .Setup(pr => pr.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
               .ReturnsAsync(persons);

            //Display Expected Data
            _testOutputHelper.WriteLine("Expected:");
            expected_persons_list.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            List<PersonForReturnDTO> allPersons = await _personsGetterService.GetAllPersons();
            //Act
            List<PersonForReturnDTO> persons_list_from_sort = _personsSorterService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.ASC);

            //Display Actual Data
            _testOutputHelper.WriteLine("Actual:");
            persons_list_from_sort.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Assert
            persons_list_from_sort.Should().BeInAscendingOrder(p => p.Name);
            persons_list_from_sort.Should().BeEquivalentTo(expected_persons_list);
        }
        #endregion

        #region UpdatePerson

        //When we supply null as PersonForUpdateDTO, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonForUpdateDTO? personForUpdateDTO = null;

            //Actual
            var actual = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personForUpdateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>();
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonForUpdateDTO? personForUpdateDTO = new() { Id = Guid.NewGuid() };

            //Actual
            var actual = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personForUpdateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>();
        }

        //Validations
        //When Person Name is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            Person person = _fixtuter.Build<Person>()
                 .With(p => p.Name, null as string)
                  .With(p => p.Email, "someone1@email.com")
                  .With(p => p.Gender, "Male")
                  .With(p => p.Country, null as Country)
                  .Create();
            PersonForReturnDTO expected_person = person.ToPersonForReturn();
            PersonForUpdateDTO personForUpdateDTO = expected_person.ToPersonForUpdateDTO();

            //Actual
            var actual = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personForUpdateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
        }


        //First, add a new person and try to update the person name,email,gender
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate()
        {
            //Arrange
            Person person = _fixtuter.Build<Person>()
                 .With(p => p.Email, "someone1@email.com")
                 .With(p => p.Gender, "Male")
                 .With(p => p.Country, null as Country)
                 .Create();
            PersonForReturnDTO expected_person = person.ToPersonForReturn();
            PersonForUpdateDTO personForUpdateDTO = expected_person.ToPersonForUpdateDTO();

            _personsRepositoryMock
            .Setup(p => p.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

            _personsRepositoryMock
             .Setup(p => p.GetPersonById(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Display Person before update
            _testOutputHelper.WriteLine("Person before update:");
            _testOutputHelper.WriteLine(expected_person.ToString());


            //Act
            PersonForReturnDTO personForReturnDTO_from_update = await _personsUpdaterService.UpdatePerson(personForUpdateDTO);

            PersonForReturnDTO? personForReturnDTO_from_get = await _personsGetterService.GetPersonById(personForReturnDTO_from_update.Id);

            //Display Person after update
            _testOutputHelper.WriteLine("Person after update:");
            _testOutputHelper.WriteLine(personForReturnDTO_from_get!.ToString());
            //Assert
            personForReturnDTO_from_update.Should().Be(personForReturnDTO_from_get);

        }

        #endregion

        #region DeletePerson

        //If you supply an invalid Person Id, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();
            //Assert.False(isDeleted);
        }

        //If you supply an valid Person ID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            Person person = _fixtuter.Build<Person>()
                .With(p => p.Name, null as string)
                 .With(p => p.Email, "someone1@email.com")
                 .With(p => p.Gender, "Male")
                 .With(p => p.Country, null as Country)
                 .Create();

            _personsRepositoryMock
            .Setup(p => p.DeletePersonById(It.IsAny<Guid>()))
            .ReturnsAsync(true);

            _personsRepositoryMock
             .Setup(p => p.GetPersonById(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person.Id);

            //Assert
            isDeleted.Should().BeTrue();
            Assert.True(isDeleted);
        }

        #endregion
    }
}
