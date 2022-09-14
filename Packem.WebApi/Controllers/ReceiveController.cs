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
    public class ReceiveController : ControllerBase
    {
        private readonly IReceiveService _receiveService;

        public ReceiveController(IReceiveService receiveService)
        {
            _receiveService = receiveService;
        }

        // POST /api/receive/createreceive
        [HttpPost("CreateReceive")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateReceive([FromBody] ReceiveCreateModel model)
        {
            var result = await _receiveService.CreateReceiveAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/receive/update/qty
        [HttpPost("Update/Qty")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateReceiveQty([FromBody] ReceiveQtyUpdateModel model)
        {
            var result = await _receiveService.UpdateReceiveQtyAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/receive/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteReceive([FromBody] ReceiveDeleteModel model)
        {
            var result = await _receiveService.DeleteReceiveAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/receive/device/lookup/poreceive/{customerFacilityId}/{purchaseOrderId}?searchText={searchText}&skuSearch={skuSearch}
        [HttpGet("Device/Lookup/POReceive/{customerFacilityId}/{purchaseOrderId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetPurchaseOrderLookupPOReceiveDevice(int customerFacilityId, int purchaseOrderId, [FromQuery] string searchText, [FromQuery] bool skuSearch)
        {
            var result = await _receiveService
                .GetReceiveLookupPOReceiveDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, purchaseOrderId, searchText, skuSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}