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
    public class CustomerDeviceController : ControllerBase
    {
        private readonly ICustomerDeviceService _customerDeviceService;

        public CustomerDeviceController(ICustomerDeviceService customerDeviceService)
        {
            _customerDeviceService = customerDeviceService;
        }

        // POST /api/customerdevice/createcustomerdevice
        [HttpPost("CreateCustomerDevice")]
        public async Task<ActionResult> CreateCustomerDevice([FromBody] CustomerDeviceCreateModel model)
        {
            var result = await _customerDeviceService.CreateCustomerDeviceAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/customerdevice/{customerDeviceId}
        [HttpPut("{customerDeviceId}")]
        public async Task<ActionResult> EditCustomerDevice([FromBody] CustomerDeviceEditModel model)
        {
            var result = await _customerDeviceService.EditCustomerDeviceAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/customerdevice/update/isactive
        [HttpPost("Update/IsActive")]
        public async Task<ActionResult> EditCustomerDeviceIsActive([FromBody] CustomerDeviceIsActiveEditModel model)
        {
            var result = await _customerDeviceService.EditCustomerDeviceIsActiveAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerdevice/all/{customerId}
        [HttpGet("CustomerDevices/all/{customerId}")]
        public async Task<ActionResult> GetCustomerDevicesByCustomerId(int customerId)
        {
            var result = await _customerDeviceService.GetCustomerDevicesByCustomerIdAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerdevice/customerdevices/{customerLocationId}
        [HttpGet("CustomerDevices/{customerLocationId}")]
        public async Task<ActionResult> GetCustomerDevices(int customerLocationId)
        {
            var result = await _customerDeviceService.GetCustomerDevicesByCustomerLocationIdAsync(User.ToAppState(), customerLocationId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customerdevice/{customerDeviceId}
        [HttpGet("{customerDeviceId}")]
        public async Task<ActionResult> GetCustomerDevice(int customerDeviceId)
        {
            var result = await _customerDeviceService.GetCustomerDeviceAsync(User.ToAppState(), customerDeviceId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
