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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // POST /api/vendor/createvendor
        [HttpPost("CreateVendor")]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult> CreateVendor([FromBody] VendorCreateModel model)
        {
            var result = await _vendorService.CreateVendorAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/vendor/{vendorId}
        [HttpPut("{vendorId}")]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult> EditVendor([FromBody] VendorEditModel model)
        {
            var result = await _vendorService.EditVendorAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/vendor/vendoritems/{customerId}
        [HttpGet("VendorItems/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetVendorItems(int customerId)
        {
            var result = await _vendorService.GetVendorItemsAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/vendor/lookup/{customerId}?searchText={searchText}
        [HttpGet("Lookup/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetVendorLookupAsync(int customerId, [FromQuery] string searchText)
        {
            var result = await _vendorService.GetVendorLookupAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/vendor/lookup/name/{customerId}?searchText={searchText}
        [HttpGet("Lookup/Name/{customerId}")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetVendorLookupByName(int customerId, [FromQuery] string searchText)
        {
            var result = await _vendorService.GetVendorLookupByNameAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/vendor/device/lookup/name/{itemId}?searchText={searchText}
        [HttpGet("Device/Lookup/Name/{itemId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetVendorLookupByNameDevice(int itemId, [FromQuery] string searchText)
        {
            var result = await _vendorService.GetVendorLookupByNameDeviceAsync(User.ToCustomerDeviceTokenAuth(), itemId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}