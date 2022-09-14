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
    public class PutAwayController : ControllerBase
    {
        private readonly IPutAwayService _putAwayService;

        public PutAwayController(IPutAwayService putAwayService)
        {
            _putAwayService = putAwayService;
        }

        // GET /api/putaway/queue/{customerLocationId}/{customerFacilityId}
        [HttpGet("Queue/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetPutAwayQueue(int customerLocationId, int customerFacilityId)
        {
            var result = await _putAwayService.GetPutAwayQueueAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/putaway/bin/createputawaybin
        [HttpPost("Bin/CreatePutAwayBin")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreatePutAwayBin([FromBody] PutAwayBinCreateModel model)
        {
            var result = await _putAwayService.CreatePutAwayBinAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/putaway/device/createputaway
        [HttpPost("Device/CreatePutAway")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreatePutAwayDevice([FromBody] PutAwayCreateModel model)
        {
            var result = await _putAwayService.CreatePutAwayDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/putaway/device/bin/createputawaybin
        [HttpPost("Device/Bin/CreatePutAwayBin")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreatePutAwayBinDevice([FromBody] PutAwayBinDeviceCreateModel model)
        {
            var result = await _putAwayService.CreatePutAwayBinDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/putaway/device/lookup/{customerFacilityId}?searchText={searchText}&skuSearch={skuSearch}
        [HttpGet("Device/Lookup/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetPutAwayLookupDevice(int customerFacilityId, [FromQuery] string searchText, [FromQuery] bool skuSearch)
        {
            var result = await _putAwayService.GetPutAwayLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText, skuSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/putaway/device/{putAwayId}
        [HttpGet("Device/{putAwayId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetPutAwayDevice(int putAwayId)
        {
            var result = await _putAwayService.GetPutAwayDeviceAsync(User.ToCustomerDeviceTokenAuth(), putAwayId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/putaway/device/lookup/licenseplate/{customerFacilityId}?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup/LicensePlate/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetPutAwayLookupLicensePlateDevice(int customerFacilityId, [FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _putAwayService.GetPutAwayLookupLicensePlateDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/putaway/device/bin/createputawaybin/pallet
        [HttpPost("Device/Bin/CreatePutAwayBin/Pallet")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreatePutAwayPalletDevice([FromBody] PutAwayPalletDeviceCreateModel model)
        {
            var result = await _putAwayService.CreatePutAwayPalletDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}