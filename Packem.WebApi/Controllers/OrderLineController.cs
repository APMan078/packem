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
    public class OrderLineController : ControllerBase
    {
        private readonly IOrderLineService _orderLineService;

        public OrderLineController(IOrderLineService orderLineService)
        {
            _orderLineService = orderLineService;
        }

        // POST /api/orderline/createorderline
        [HttpPost("CreateOrderLine")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateOrderLine([FromBody] OrderLineCreateModel model)
        {
            var result = await _orderLineService.CreateOrderLineAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/orderline/update
        [HttpPost("Update")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateOrderLine([FromBody] OrderLineUpdateModel model)
        {
            var result = await _orderLineService.UpdateOrderLineAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/orderline/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteOrderLine([FromBody] OrderLineDeleteModel model)
        {
            var result = await _orderLineService.DeleteOrderLineAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/orderline/device/{orderLineId}
        [HttpGet("Device/{orderLineId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetOrderLineDevice(int orderLineId)
        {
            var result = await _orderLineService.GetOrderLineDeviceAsync(User.ToCustomerDeviceTokenAuth(), orderLineId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/orderline/device/lookup/{saleOrderId}?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup/{saleOrderId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetOrderLinePickLookupDevice(int saleOrderId, [FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _orderLineService.GetOrderLinePickLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), saleOrderId, searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/orderline/device/bin/createorderlinebin
        [HttpPost("Device/Bin/CreateOrderLineBin")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreateOrderLineBinDevice([FromBody] OrderLineBinCreateModel model)
        {
            var result = await _orderLineService.CreateOrderLineBinDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}