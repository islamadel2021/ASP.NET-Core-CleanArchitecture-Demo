using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Interfaces;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryForReturnDTO> AddCountry(CountryForCreateDTO? countryForCreateDTO)
        {
            //Validation: countryForCreateDTO parameter can't be null
            if (countryForCreateDTO == null)
            {
                throw new ArgumentNullException(nameof(countryForCreateDTO));
            }

            //Validation: Name can't be null
            if (countryForCreateDTO.Name == null)
            {
                throw new ArgumentException(nameof(countryForCreateDTO.Name));
            }

            //Validation: Name can't be duplicate
            if (await _countriesRepository.GetCountryByName(countryForCreateDTO.Name) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryForCreateDTO to Country type
            Country country = countryForCreateDTO.ToCountry();

            //generate Id
            country.Id = Guid.NewGuid();

            //Validation: Every thing is ok , Add country object into _countries
            await _countriesRepository.AddCountry(country);
            return country.ToCountryForReturn();
        }

        public async Task<List<CountryForReturnDTO>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(c => c.ToCountryForReturn()).ToList();
        }

        public async Task<CountryForReturnDTO?> GetCountryById(Guid? id)
        {
            if (id == null) return null;
            Country? country = await _countriesRepository.GetCountryById(id.Value);
            if (country == null) return null;
            return country.ToCountryForReturn();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if (_countriesRepository.GetCountryByName(countryName) == null)
                        {
                            Country country = new() { Name = countryName };
                            await _countriesRepository.AddCountry(country);
                            countriesInserted++;
                        }
                    }
                }
            }
            return countriesInserted;
        }
    }
}
