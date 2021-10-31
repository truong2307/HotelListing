using AutoMapper;
using HotelListing.Data.Repository.IRepository;
using HotelListing.Dto;
using HotelListing.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;

        public UserController(IUnitOfWork unitOfWork
            , ILogger<UserController> logger
            , IMapper mapper
            , UserManager<ApiUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDto userRequest)
        {
            _logger.LogInformation($"Registration Attempt for {userRequest.Email}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userToDb = _mapper.Map<ApiUser>(userRequest);
                userToDb.UserName = userRequest.Email;
                var userResponse = await _userManager.CreateAsync(userToDb, userRequest.Password);

                if (!userResponse.Succeeded)
                {
                    foreach (var error in userResponse.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Accepted();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(Register)}");
                return Problem($"Somethong went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }
    }
}
