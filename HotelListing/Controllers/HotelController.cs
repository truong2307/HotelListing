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
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork
            ,ILogger<HotelController> logger
            ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            _logger.LogInformation("Accessed get all hotel");
            try
            {
                var hotelsInDb = await _unitOfWork.Hotels.GetAll();
                var hotelsDto = _mapper.Map<IList<HotelDto>>(hotelsInDb);

                return Ok(hotelsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(GetHotels)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            _logger.LogInformation("Accessed get hotel");
            try
            {
                var hotelInDb = await _unitOfWork.Hotels
                    .Get(c => c.Id == id, new List<string> { "Country" });
                var hotelDto = _mapper.Map<HotelDto>(hotelInDb);

                return Ok(hotelDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(GetHotels)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto hotelRequest)
        {
            _logger.LogInformation("Accessed create hotel");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var hotelToDb = _mapper.Map<Hotel>(hotelRequest);
                await _unitOfWork.Hotels.Insert(hotelToDb);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetHotel", new { id = hotelToDb.Id }, hotelToDb);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(CreateHotel)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto hotelRequest)
        {
            _logger.LogInformation("Accessed update hotel");
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }
            try
            {
                var hotelInDb = await _unitOfWork.Hotels.Get(c => c.Id == id);

                if (hotelInDb == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(hotelRequest, hotelInDb);
                _unitOfWork.Hotels.Update(hotelInDb);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(CreateHotel)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation("Accessed delete hotel");
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotelInDb = _unitOfWork.Hotels.Get(C => C.Id == id);

                if (hotelInDb == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");

                }

                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Somethong went wrong in the {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal sever error. Please try again later.");
            }
        }
    }
}
