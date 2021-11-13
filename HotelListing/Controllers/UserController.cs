using AutoMapper;
using HotelListing.Core.Dtos;
using HotelListing.Core.Servives;
using HotelListing.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;

        public UserController(ILogger<UserController> logger
            , IMapper mapper
            , UserManager<ApiUser> userManager
            , IAuthManager authManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _authManager = authManager;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

                await _userManager.AddToRolesAsync(userToDb, userRequest.Roles);
                return Accepted();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(Register)}");
                return Problem($"Somethong went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginUserDto userRequest)
        {
            _logger.LogInformation($"Login Attempt for {userRequest.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool loginUserSucceeded = await _authManager.ValidateUser(userRequest);
                if (!loginUserSucceeded)
                {
                    return Unauthorized();
                }

                return Accepted(new { token = await _authManager.CreateToken() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Somethong went wrong in the {nameof(Login)}");
                return Problem($"Somethong went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }
    }
}
