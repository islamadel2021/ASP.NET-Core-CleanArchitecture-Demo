using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Interfaces;
using Services;

namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        //When CountryForCreateDTO is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryForCreateDTO? countryForCreateDTO = null;
            Country country = _fixture.Build<Country>()
                    .With(c => c.Persons, null as List<Person>).Create();

            _countriesRepositoryMock
             .Setup(cr => cr.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(country);

            //Act
            var actual = async () =>
            {
                await _countriesService.AddCountry(countryForCreateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentNullException>();
        }

        //When the Name is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_NameIsNull()
        {
            //Arrange
            CountryForCreateDTO? countryForCreateDTO = _fixture.Build<CountryForCreateDTO>()
                 .With(c => c.Name, null as string)
                 .Create();
            Country country = _fixture.Build<Country>()
                 .With(c => c.Persons, null as List<Person>)
                 .Create();
            _countriesRepositoryMock
                     .Setup(cr => cr.AddCountry(It.IsAny<Country>()))
                     .ReturnsAsync(country);

            //Act
            var actual = async () =>
            {
                await _countriesService.AddCountry(countryForCreateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
        }

        //When the Name is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateName()
        {
            //Arrange
            CountryForCreateDTO? countryForCreateDTO = _fixture.Create<CountryForCreateDTO>();

            Country country = countryForCreateDTO.ToCountry();

            _countriesRepositoryMock
              .Setup(cr => cr.AddCountry(It.IsAny<Country>()))
              .ReturnsAsync(country);

            _countriesRepositoryMock
              .Setup(cr => cr.GetCountryByName(It.IsAny<string>()))
              .ReturnsAsync(country);

            //Act
            var actual = async () =>
            {
                await _countriesService.AddCountry(countryForCreateDTO);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryForCreateDTO? countryForCreateDTO = _fixture.Create<CountryForCreateDTO>();
            Country country = countryForCreateDTO.ToCountry();
            CountryForReturnDTO expected_country = country.ToCountryForReturn();

            _countriesRepositoryMock
                         .Setup(cr => cr.AddCountry(It.IsAny<Country>()))
                         .ReturnsAsync(country);

            _countriesRepositoryMock
                      .Setup(cr => cr.GetCountryByName(It.IsAny<string>()))
                      .ReturnsAsync(null as Country);

            //Act
            CountryForReturnDTO country_from_add = await _countriesService.AddCountry(countryForCreateDTO);
            country.Id = country_from_add.Id;
            expected_country.Id = country_from_add.Id;

            //Assert
            expected_country.Id.Should().NotBe(Guid.Empty);
            country_from_add.Should().BeEquivalentTo(expected_country);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange
            List<Country> country_empty_list = new();
            _countriesRepositoryMock
                .Setup(cr => cr.GetAllCountries())
                .ReturnsAsync(country_empty_list);

            //Act
            List<CountryForReturnDTO> actual_country_list = await _countriesService.GetAllCountries();

            //Assert
            actual_country_list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<Country> countries_list = new()
            {
                    _fixture.Build<Country>()
                    .With(c => c.Persons, null as List<Person>).Create(),
                    _fixture.Build<Country>()
                    .With(c => c.Persons, null as List<Person>).Create()
            };

            List<CountryForReturnDTO> expected_countries_list = countries_list.Select(c => c.ToCountryForReturn()).ToList();

            _countriesRepositoryMock
                .Setup(cr => cr.GetAllCountries())
                .ReturnsAsync(countries_list);

            //Act
            List<CountryForReturnDTO> actual_countries_list = await _countriesService.GetAllCountries();

            //Assert
            actual_countries_list.Should().BeEquivalentTo(expected_countries_list);
        }
        #endregion

        #region GetCountryById
        [Fact]
        //If we supply null as Country Id, it should return null as CountryForReturnDTO
        public async Task GetCountryByID_NullId()
        {
            //Arrange
            Guid? Id = null;
            _countriesRepositoryMock
                .Setup(cr => cr.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Country);

            //Act
            CountryForReturnDTO? actual_country_from_get = await _countriesService.GetCountryById(Id);

            //Assert
            actual_country_from_get.Should().BeNull();
        }


        [Fact]
        //If we supply a valid country id, it should return the matching country details as CountryForReturnDTO object
        public async Task GetCountryByID_ValidId()
        {
            //Arrange
            Country? country =
                _fixture.Build<Country>()
                        .With(c => c.Persons, null as List<Person>)
                        .Create();

            CountryForReturnDTO expected_country_from_get = country.ToCountryForReturn();

            _countriesRepositoryMock
                 .Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                 .ReturnsAsync(country);

            //Act
            CountryForReturnDTO? actual_country_from_get = await _countriesService.GetCountryById(country.Id);

            //Assert
            actual_country_from_get.Should().Be(expected_country_from_get);
        }
        #endregion
    }
}
