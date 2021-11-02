using AutoMapper;
using HotelListing.Data.Repository.IRepository;
using HotelListing.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryController> _logger;

        public CountryController(IUnitOfWork unitOfWork
            ,IMapper mapper
            ,ILogger<CountryController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            _logger.LogInformation("Accessed get all countries");

            try
            {
                var countriesInDb = await _unitOfWork.Countries.GetAll();
                var countriesDto = _mapper.Map<IList<CountryDto>>(countriesInDb);

                return Ok(countriesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(GetCountries)}");
                return StatusCode(500, "Internal sever error. Please try again later.");   
            }
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            _logger.LogInformation("Accessed get country by id");

            try
            {
                var countryInDb = await _unitOfWork.Countries
                    .Get(c => c.Id == id, new List<string> { "Hotels" });
                var countryDto = _mapper.Map<CountryDto>(countryInDb);

                return Ok(countryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(GetCountry)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }
    }
}
