using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;
using Packem.Domain.Models;
using Packem.WebApi.Common.CustomProviders;
using Packem.WebApi.Common.ExtensionMethods;
using System.Threading.Tasks;

namespace Packem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // POST /api/customer/createcustomer
        [HttpPost("CreateCustomer")]
        [Authorize(Roles = "1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateCustomer([FromBody] CustomerCreateModel model)
        {
            var result = await _customerService.CreateCustomerAsync(model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/customer/{customerId}
        [HttpPut("{customerId}")]
        [Authorize(Roles = "1, 2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditCustomer([FromBody] CustomerEditModel model)
        {
            var result = await _customerService.EditCustomerAsync(model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/customer/update/isactive
        [HttpPost("Update/IsActive")]
        [Authorize(Roles = "1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditCustomerIsActive([FromBody] CustomerIsActiveEditModel model)
        {
            var result = await _customerService.EditCustomerIsActiveAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customer/customers
        [HttpGet("Customers")]
        [Authorize(Roles = "1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCustomers()
        {
            //var role = (RoleEnum)Convert.ToInt32(User.FindFirstValue(ClaimTypes.Role));

            //// get all data
            //if (role == RoleEnum.SuperAdmin)
            //{

            //}
            //// limit to their customerlocationid
            //else
            //{

            //}

            var result = await _customerService.GetCustomersAsync();

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customer/{customerId}
        [HttpGet("{customerId}")]
        [Authorize(Roles = "1, 2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCustomer(int customerId)
        {
            var result = await _customerService.GetCustomerAsync(customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customer/GetDefaultThreshold/{customerId}
        [HttpGet("GetDefaultThreshold/{customerId}")]
        [Authorize(Roles = "1, 2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCustomerDefaultThreshold(int customerId)
        {
            var result = await _customerService.GetCustomerDefaultThresholdAsync(customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/customer/UpdateDefaultThreshold/{customerId}/{threshold}
        [HttpPut("UpdateDefaultThreshold/{customerId}/{threshold}")]
        [Authorize(Roles = "1, 2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateCustomerDefaultThreshold(int customerId, int threshold)
        {
            var result = await _customerService.UpdateCustomerDefaultThresholdAsync(customerId, threshold);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }


        // GET /api/customer/current
        [HttpGet("Current")]
        [Authorize(Roles = "1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCurrentCustomer()
        {
            var result = await _customerService.GetCurrentCustomerAsync(User.ToAppState());

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/customer/current/device
        [HttpGet("Current/Device")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetCurrentCustomerForDevice()
        {
            var result = await _customerService.GetCurrentCustomerForDeviceAsync(User.ToCustomerDeviceTokenAuth());

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
