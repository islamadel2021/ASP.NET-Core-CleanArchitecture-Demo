using CRUDUI.Filters.ActionFilters;
using CRUDUI.Filters.AuthorizationFilters;
using CRUDUI.Filters.Custom_Filters;
using CRUDUI.Filters.ResourceFilters;
using CRUDUI.Filters.ResultFilters;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.Interfaces;

namespace CRUDUI.Controllers
{
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "Controller-Key", "Controller-Value", 3 }, Order = 3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    //[Authorize]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(ICountriesService countriesService, IPersonsAdderService personsAdderService, IPersonsGetterService personsGetterService, IPersonsSorterService personsSorterService, IPersonsUpdaterService personsUpdaterService, IPersonsDeleterService personsDeleterService, ILogger<PersonsController> logger)
        {
            _countriesService = countriesService;
            _personsAdderService = personsAdderService;
            _personsGetterService = personsGetterService;
            _personsSorterService = personsSorterService;
            _personsUpdaterService = personsUpdaterService;
            _personsDeleterService = personsDeleterService;
            _logger = logger;
        }

        [AllowAnonymous]
        [Route("/")]
        [Route("persons")]
        [Route("persons/index")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "Index-Key", "Index-Value", 1 }, Order = 1)]
        [ResponseHeaderFilterFactory("Index-Key", "Index-Value", 1)]
        [TypeFilter(typeof(IndexResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonForReturnDTO.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"sortBy:{sortBy},sortOders:{sortOrder}");

            //Search
            List<PersonForReturnDTO> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonForReturnDTO> sortedPersons = _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            return View(sortedPersons);
        }

        [Route("/persons/create")]
        [HttpGet]
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "Create-Key", "Create-Value", 1 }, Order = 1)]
        [TypeFilter(typeof(DisableResourceFilter), Arguments = new object[] { false })]
        [SkipFilter]
        public async Task<IActionResult> Create()
        {
            List<CountryForReturnDTO> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            return View();
        }

        [HttpPost]
        [Route("/persons/create")]
        [TypeFilter(typeof(ShortCircuitingActionFilter))]
        public async Task<IActionResult> Create(PersonForCreateDTO personModel)
        {
            if (!ModelState.IsValid) return View(personModel);
            await _personsAdderService.AddPerson(personModel);
            return RedirectToAction("Index", "Persons");
        }

        [Route("/persons/update/{id}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Update(Guid id)
        {
            PersonForReturnDTO? personForReturnDTO = await _personsGetterService.GetPersonById(id);
            if (personForReturnDTO == null)
            {
                throw new InvalidPersonException("Given person doesn't exist!");
            }
            PersonForUpdateDTO personForUpdateDTO = personForReturnDTO.ToPersonForUpdateDTO();

            List<CountryForReturnDTO> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            return View(personForUpdateDTO);
        }

        [HttpPost]
        [Route("/persons/update/{id}")]
        [TypeFilter(typeof(ShortCircuitingActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Update(PersonForUpdateDTO personModel)
        {
            PersonForReturnDTO? personForReturnDTO = await _personsGetterService.GetPersonById(personModel.Id);
            if (personForReturnDTO == null)
            {
                return RedirectToAction("index");
            }

            await _personsUpdaterService.UpdatePerson(personModel);
            return RedirectToAction("Index", "Persons");
        }


        [Route("/persons/delete/{id}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            PersonForReturnDTO? personForReturnDTO = await _personsGetterService.GetPersonById(id);
            if (personForReturnDTO == null)
            {
                return RedirectToAction("index");
            }

            return View(personForReturnDTO);
        }

        [HttpPost]
        [Route("/persons/delete/{id}")]
        public async Task<IActionResult> Delete(PersonForReturnDTO personForReturnDTO)
        {
            PersonForReturnDTO? personForDelete = await _personsGetterService.GetPersonById(personForReturnDTO.Id);
            if (personForDelete == null)
            {
                return RedirectToAction("index");
            }

            await _personsDeleterService.DeletePerson(personForDelete.Id);
            return RedirectToAction("Index", "Persons");
        }

        [Route("PersonsPdf")]
        public async Task<IActionResult> PersonsPdf()
        {
            //Get list of persons
            List<PersonForReturnDTO> persons = await _personsGetterService.GetAllPersons();

            //Return view as pdf
            return new ViewAsPdf("PersonsPdf", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

    }
}
