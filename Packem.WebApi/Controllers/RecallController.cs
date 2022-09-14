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
    public class RecallController : ControllerBase
    {
        private readonly IRecallService _recallService;

        public RecallController(IRecallService recallService)
        {
            _recallService = recallService;
        }

        // POST /api/recall/createrecall
        [HttpPost("CreateRecall")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateRecall([FromBody] RecallCreateModel model)
        {
            var result = await _recallService.CreateRecallAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/recall/queue/{customerLocationId}/{customerFacilityId}
        [HttpGet("Queue/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetRecallQueue(int customerLocationId, int customerFacilityId)
        {
            var result = await _recallService.GetRecallQueueAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/recall/device/update/status
        [HttpPost("Device/Update/Status")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> UpdateRecallStatusDevice([FromBody] RecallStatusUpdateModel model)
        {
            var result = await _recallService.UpdateRecallStatusDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/recall/device/lookup/{customerFacilityId}?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetRecallQueueLookupDevice(int customerFacilityId, [FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _recallService.GetRecallQueueLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/recall/device/detail/{recallId}
        [HttpGet("Device/detail/{recallId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetRecallDetailDevice(int recallId)
        {
            var result = await _recallService.GetRecallDetailDeviceAsync(User.ToCustomerDeviceTokenAuth(), recallId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/recall/device/detail/{recallId}/{binId}
        [HttpGet("Device/detail/{recallId}/{binId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetRecallDetailDevice(int recallId, int binId)
        {
            var result = await _recallService.GetRecallDetailDeviceAsync(User.ToCustomerDeviceTokenAuth(), recallId, binId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/recall/device/bin/createrecallbin
        [HttpPost("Device/Bin/CreateRecallBin")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreateRecallBinDevice([FromBody] RecallBinCreateModel model)
        {
            var result = await _recallService.CreateRecallBinDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}