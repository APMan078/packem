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
    public class CustomerFacilityController : ControllerBase
    {
        private readonly ICustomerFacilityService _customerFacilityService;

        public CustomerFacilityController(ICustomerFacilityService customerFacilityService)
        {
            _customerFacilityService = customerFacilityService;
        }

        // POST /api/customerfacility/createcustomerfacility
        [HttpPost("CreateCustomerFacility")]
        public async Task<ActionResult> CreateCustomerFacility([FromBody] CustomerFacilityCreateModel model)
        {
            var result = await _customerFacilityService.CreateCustomerFacilityAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/customerfacility/{customerfacilityId}
        [HttpPut("{customerfacilityId}")]
        public async Task<ActionResult> EditCustomerFacility([FromBody] CustomerFacilityEditModel model)
        {
            var result = await _customerFacilityService.EditCustomerFacilityAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/customerfacility/delete
        [HttpPost("Delete")]
        public async Task<ActionResult> DeleteCustomerFacility([FromBody] CustomerFacilityDeleteModel model)
        {
            var result = await _customerFacilityService.DeleteCustomerFacilityAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerfacility/customerfacilities/customer/{customerId}
        [HttpGet("CustomerFacilities/Customer/{customerId}")]
        public async Task<ActionResult> GetCustomerFacilitiesByCustomerId(int customerId)
        {
            var result = await _customerFacilityService.GetCustomerFacilitiesByCustomerIdAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerfacility/customerfacilities/{customerLocationId}
        [HttpGet("CustomerFacilities/{customerLocationId}")]
        public async Task<ActionResult> GetCustomerFacilities(int customerLocationId)
        {
            var result = await _customerFacilityService.GetCustomerFacilitiesByCustomerLocationIdAsync(User.ToAppState(), customerLocationId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerfacility/all
        [HttpGet("all")]
        public async Task<ActionResult> GetCustomerFacilitiesSuperAdmin()
        {
            var result = await _customerFacilityService.GetCustomerLocationsSuperAdminAsync();

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
        // GET /api/customerfacility/{customerFacilityId}
        [HttpGet("{customerFacilityId}")]
        public async Task<ActionResult> GetCustomerFacility(int customerFacilityId)
        {
            var result = await _customerFacilityService.GetCustomerFacilityAsync(User.ToAppState(), customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
