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
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        // POST /api/receipt/createreceipt
        [HttpPost("CreateReceipt")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateReceipt([FromBody] ReceiptCreateModel model)
        {
            var result = await _receiptService.CreateReceiptAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/receipt/queue/{customerLocationId}/{customerFacilityId}
        [HttpGet("Queue/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetReceiptQueue(int customerLocationId, int customerFacilityId)
        {
            var result = await _receiptService.GetReceiptQueueAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/receipt/device/createreceipt
        [HttpPost("Device/CreateReceipt")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreateReceiptDevice([FromBody] ReceiptDeviceCreateModel model)
        {
            var result = await _receiptService.CreateReceiptDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}