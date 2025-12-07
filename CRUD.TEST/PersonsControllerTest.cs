using AutoFixture;
using CRUDUI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.Interfaces;

namespace CRUDTest
{
    public class PersonsControllerTest
    {
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;

        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();

            _countriesService = _countriesServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;


        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonForReturnDTO> persons_list = _fixture.Create<List<PersonForReturnDTO>>();

            PersonsController personsController = new(_countriesService, _personsAdderService, _personsGetterService, _personsSorterService, _personsUpdaterService, _personsDeleterService, _logger);

            _personsGetterServiceMock
             .Setup(ps => ps.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(persons_list);

            _personsSorterServiceMock
                .Setup(ps => ps.GetSortedPersons(It.IsAny<List<PersonForReturnDTO>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .Returns(persons_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model!.Should().BeAssignableTo<List<PersonForReturnDTO>>();
            viewResult.ViewData.Model.Should().Be(persons_list);
        }
        #endregion

        #region Create

        [Fact]
        public async void Create_IfModelErrors_ReturnCreateView()
        {
            //Arrange
            PersonForCreateDTO personForCreate = _fixture.Create<PersonForCreateDTO>();

            PersonForReturnDTO personForReturn = _fixture.Create<PersonForReturnDTO>();

            List<CountryForReturnDTO> countries = _fixture.Create<List<CountryForReturnDTO>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countries);

            _personsAdderServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonForCreateDTO>()))
             .ReturnsAsync(personForReturn);

            PersonsController personsController = new(_countriesService, _personsAdderService, _personsGetterService, _personsSorterService, _personsUpdaterService, _personsDeleterService, _logger);


            //Act
            personsController.ModelState.AddModelError("Name", "Name is required");

            IActionResult result = await personsController.Create(personForCreate);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonForCreateDTO>();

            viewResult.ViewData.Model.Should().Be(personForCreate);
        }


        [Fact]
        public async void Create_IfNoModelErrors_ReturnRedirectToIndex()
        {
            //Arrange
            PersonForCreateDTO personForCreateDTO = _fixture.Create<PersonForCreateDTO>();

            PersonForReturnDTO personForReturn = _fixture.Create<PersonForReturnDTO>();

            List<CountryForReturnDTO> countries = _fixture.Create<List<CountryForReturnDTO>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countries);

            _personsAdderServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonForCreateDTO>()))
             .ReturnsAsync(personForReturn);

            PersonsController personsController = new(_countriesService, _personsAdderService, _personsGetterService, _personsSorterService, _personsUpdaterService, _personsDeleterService, _logger);


            //Act
            IActionResult result = await personsController.Create(personForCreateDTO);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
