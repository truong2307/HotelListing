using AutoMapper;
using HotelListing.Data.Repository.IRepository;
using HotelListing.Dto;
using HotelListing.Model;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        //[ResponseCache(CacheProfileName = "120SecondsDuration")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]    // override httpcache at endpoint
        [HttpCacheValidation(MustRevalidate = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            _logger.LogInformation("Accessed get all countries");

            var countriesInDb = await _unitOfWork.Countries.GetPageList(requestParams);
            var countriesDto = _mapper.Map<IList<CountryDto>>(countriesInDb);

            return Ok(countriesDto);
        }

        //[Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpGet("{id:int}", Name ="GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            _logger.LogInformation("Accessed get country by id");

            var countryInDb = await _unitOfWork.Countries
                    .Get(c => c.Id == id, include: q => q.Include(i => i.Hotels));
            var countryDto = _mapper.Map<CountryDto>(countryInDb);

            return Ok(countryDto);

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

            var countryToDb = _mapper.Map<Country>(countryRequest);
            await _unitOfWork.Countries.Insert(countryToDb);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetCountry", new { id = countryToDb.Id }, countryToDb);
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            _logger.LogInformation("Accessed delete country.");
            if (id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(DeleteCountry)}");
                return BadRequest(ModelState);
            }

            var CountryInDb = await _unitOfWork.Countries.Get(c => c.Id == id);

            if (CountryInDb == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();

            return NoContent();
        }

    }
}
