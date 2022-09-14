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
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        // POST /api/transfer/createtransfer
        [HttpPost("CreateTransfer")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateTransfer([FromBody] TransferCreateModel model)
        {
            var result = await _transferService.CreateTransferAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/transfer/transferhistory/{customerLocationId}/{customerFacilityId}
        [HttpGet("TransferHistory/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetTransferHistory(int customerLocationId, int customerFacilityId)
        {
            var result = await _transferService.GetTransferHistoryAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/transfer/device/lookup/{customerFacilityId}?searchText={searchText}&skuSearch={skuSearch}
        [HttpGet("Device/Lookup/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetTransferLookupDevice(int customerFacilityId, [FromQuery] string searchText, [FromQuery] bool skuSearch)
        {
            var result = await _transferService.GetTransferLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText, skuSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/transfer/device/manualtransfer
        [HttpPost("Device/ManualTransfer")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreateTransferManualDevice([FromBody] TransferManualCreateModel model)
        {
            var result = await _transferService.CreateTransferManualDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/transfer/device/transferrequest
        [HttpPost("Device/TransferRequest")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreateTransferRequestDevice([FromBody] TransferRequestCreateModel model)
        {
            var result = await _transferService.CreateTransferRequestDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}