using AutoMapper;
using HotelListing.Data.Repository.IRepository;
using HotelListing.Dto;
using HotelListing.Model;
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

        [Authorize(Roles = "Admin")]
        [Authorize]
        [HttpGet("{id:int}", Name ="GetCountry")]
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

        [Authorize(Roles ="Admin")]
        [HttpPost("CreateCountry")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto countryRequest)
        {
            _logger.LogInformation("Accessed create country");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var countryToDb = _mapper.Map<Country>(countryRequest);
                await _unitOfWork.Countries.Insert(countryToDb);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetCountry", new { id = countryToDb.Id }, countryToDb);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Somethong went wrong in the {nameof(CreateCountry)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDto countryRequest)
        {
            _logger.LogInformation("Accessed update country");

            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var countryInDb = await _unitOfWork.Countries.Get(c => c.Id == id);

                if (countryInDb == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(countryRequest, countryInDb);
                _unitOfWork.Countries.Update(countryInDb);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Somethong went wrong in the {nameof(UpdateCountry)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }

    }
}
