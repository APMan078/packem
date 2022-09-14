using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;
using Packem.Domain.Models;
using Packem.WebApi.Common.ExtensionMethods;
using System.Threading.Tasks;

namespace Packem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST /api/user/createuser
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserCreateModel model)
        {
            var result = await _userService.CreateUserAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/user/{userId}
        [HttpPut("{userId}")]
        public async Task<ActionResult> EditUser([FromBody] UserEditModel model)
        {
            var result = await _userService.EditUserAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/user/update/isactive
        [HttpPost("Update/IsActive")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditUserIsActive([FromBody] UserIsActiveEditModel model)
        {
            var result = await _userService.EditUserIsActiveAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/user/update/password
        [HttpPost("Update/Password")]
        public async Task<ActionResult> EditUserPassword([FromBody] UserPasswordEditModel model)
        {
            var result = await _userService.EditUserPasswordAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/user/users
        [HttpGet("Users")]
        public async Task<ActionResult> GetUsers()
        {
            var result = await _userService.GetUsersAsync(User.ToAppState());

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // Get /api/user/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult> GetUsersByCustomerId(int customerId)
        {
            var result = await _userService.GetUsersByCustomerIdAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/user/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUser(int userId)
        {
            var result = await _userService.GetUserAsync(User.ToAppState(), userId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/user/vendor/{userId}
        [HttpGet("Vendor/{userId}")]
        public async Task<ActionResult> GetUserVendor(int userId)
        {
            var result = await _userService.GetUserVendorAsync(User.ToAppState(), userId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}