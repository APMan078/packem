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
    [Authorize(Roles = "1,2,5")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerLocationController : ControllerBase
    {
        private readonly ICustomerLocationService _customerLocationService;

        public CustomerLocationController(ICustomerLocationService customerLocationService)
        {
            _customerLocationService = customerLocationService;
        }

        // POST /api/customerlocation/createcustomerlocation
        [HttpPost("CreateCustomerLocation")]
        public async Task<ActionResult> CreateCustomerLocation([FromBody] CustomerLocationCreateModel model)
        {
            var result = await _customerLocationService.CreateCustomerLocationAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/customerlocation/{customerLocationId}
        [HttpPut("{customerLocationId}")]
        public async Task<ActionResult> EditCustomerLocation([FromBody] CustomerLocationEditModel model)
        {
            var result = await _customerLocationService.EditCustomerLocationAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/customerlocation/delete
        [HttpPost("Delete")]
        public async Task<ActionResult> DeleteCustomerLocation([FromBody] CustomerLocationDeleteModel model)
        {
            var result = await _customerLocationService.DeleteCustomerLocationAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        [HttpGet("CustomerLocations")]
        public async Task<ActionResult> GetCustomerLocationsSuperAdmin()
        {
            var result = await _customerLocationService.GetCustomerLocationsSuperAdminAsync();

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerlocation/customerlocations/{customerId}
        [HttpGet("CustomerLocations/{customerId}")]
        public async Task<ActionResult> GetCustomerLocations(int customerId)
        {
            var state = User.ToAppState();
            int? cId;

            if (state.Role == Domain.Common.Enums.RoleEnum.SuperAdmin)
            {
                cId = customerId;
            }
            else
            {
                cId = state.CustomerId;
            }

            var result = await _customerLocationService.GetCustomerLocationsByCustomerIdAsync(cId.Value);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerlocation/{customerLocationId}
        [HttpGet("{customerLocationId}")]
        public async Task<ActionResult> GetCustomerLocation(int customerLocationId)
        {
            var result = await _customerLocationService.GetCustomerLocationAsync(User.ToAppState(), customerLocationId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
