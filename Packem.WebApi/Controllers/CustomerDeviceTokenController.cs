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
    public class CustomerDeviceTokenController : ControllerBase
    {
        private readonly ICustomerDeviceTokenService _customerDeviceTokenService;

        public CustomerDeviceTokenController(ICustomerDeviceTokenService customerDeviceTokenService)
        {
            _customerDeviceTokenService = customerDeviceTokenService;
        }

        // POST /api/customerdevicetoken/createcustomerdevicetoken
        [HttpPost("CreateCustomerDeviceToken")]
        public async Task<ActionResult> CreateCustomerDeviceToken([FromBody] CustomerDeviceTokenCreateModel model)
        {
            var result = await _customerDeviceTokenService.CreateCustomerDeviceTokenAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerdevicetoken/customerdevicetokens/{customerDeviceId}
        [HttpGet("CustomerDeviceTokens/{customerDeviceId}")]
        public async Task<ActionResult> GetCustomerDeviceTokens(int customerDeviceId)
        {
            var result = await _customerDeviceTokenService.GetCustomerDeviceTokensByCustomerDeviceIdAsync(User.ToAppState(), customerDeviceId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerdevicetoken/{customerDeviceTokenId}
        [HttpGet("{customerDeviceTokenId}")]
        public async Task<ActionResult> GetCustomerDeviceToken(int customerDeviceTokenId)
        {
            var result = await _customerDeviceTokenService.GetCustomerDeviceTokenAsync(User.ToAppState(), customerDeviceTokenId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
