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
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        // POST /api/zone/createzone
        [HttpPost("CreateZone")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateZone([FromBody] ZoneCreateModel model)
        {
            var result = await _zoneService.CreateZoneAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/zone/{zoneId}
        [HttpPut("{zoneId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditZone([FromBody] ZoneEditModel model)
        {
            var result = await _zoneService.EditZoneAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/zone/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteZone([FromBody] ZoneDeleteModel model)
        {
            var result = await _zoneService.DeleteZoneAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/zone/zones/{customerLocationId}
        [HttpGet("Zones/{customerLocationId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetZones(int customerLocationId)
        {
            var result = await _zoneService.GetZoneByCustomerLocationIdAsync(User.ToAppState(), customerLocationId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/zone/{zoneId}
        [HttpGet("{zoneId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetZone(int zoneId)
        {
            var result = await _zoneService.GetZoneAsync(User.ToAppState(), zoneId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/zone/lookup/{customerFacilityId}?searchText={searchText}
        [HttpGet("Lookup/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetZoneLookup(int customerFacilityId, [FromQuery] string searchText)
        {
            var result = await _zoneService.GetZoneLookupAsync(User.ToAppState(), customerFacilityId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/zone/device/inventory/{customerFacilityId}/{inventoryId}
        [HttpGet("Device/Inventory/{customerFacilityId}/{inventoryId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetZoneInventoryDevice(int customerFacilityId, int inventoryId)
        {
            var result = await _zoneService
                .GetZoneInventoryDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, inventoryId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/zone/device/lookup/{customerFacilityId}?searchText={searchText}
        [HttpGet("Device/Lookup/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetZoneLookupDevice(int customerFacilityId, [FromQuery] string searchText)
        {
            var result = await _zoneService
                .GetZoneLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/zone/device/lookup/itemquantity/{customerFacilityId}/{itemId}?searchText={searchText}
        [HttpGet("Device/Lookup/ItemQuantity/{customerFacilityId}/{itemId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetZoneLookupItemQuantityDevice(int customerFacilityId, int itemId, [FromQuery] string searchText)
        {
            var result = await _zoneService
                .GetZoneLookupItemQuantityDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, itemId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}