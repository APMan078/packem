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
    public class SaleOrderController : ControllerBase
    {
        private readonly ISaleOrderService _saleOrderService;

        public SaleOrderController(ISaleOrderService saleOrderService)
        {
            _saleOrderService = saleOrderService;
        }

        // POST /api/saleorder/createsalesorder
        [HttpPost("AddImportedSaleOrders/{customerLocationId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddImportedSaleOrders([FromRoute] int customerLocationId, [FromBody] SalesOrderImportModel[] model)
        {
            var result = await _saleOrderService.AddImportedSaleOrdersAsync(User.ToAppState(), customerLocationId, model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/saleorder/createsaleorder
        [HttpPost("CreateSaleOrder")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateSaleOrder([FromBody] SaleOrderCreateModel model)
        {
            var result = await _saleOrderService.CreateSaleOrderAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/saleorder/all/{customerLocationId}/{customerFacilityId}
        [HttpGet("All/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetSaleOrderAll(int customerLocationId, int customerFacilityId)
        {
            var result = await _saleOrderService.GetSaleOrderAllAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/saleorder/detail/{saleOrderId}
        [HttpGet("Detail/{saleOrderId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetSaleOrderDetail(int saleOrderId)
        {
            var result = await _saleOrderService.GetSaleOrderDetailAsync(User.ToAppState(), saleOrderId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/saleorder/update/status/printed/{saleOrderId}
        [HttpPost("Update/Status/Printed/{saleOrderId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateSaleOrderStatusToPrinted(int saleOrderId)
        {
            var result = await _saleOrderService.UpdateSaleOrderStatusToPrintedAsync(User.ToAppState(), saleOrderId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/saleorder/print/{saleOrderId}
        [HttpGet("Print/{saleOrderId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetSaleOrderPrint(int saleOrderId)
        {
            var result = await _saleOrderService.GetSaleOrderPrintAsync(User.ToAppState(), saleOrderId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/saleorder/printmultiple
        [HttpPost("PrintMultiple")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetSaleOrderPrintMultiple([FromBody] int[] saleOrderIds)
        {
            var result = await _saleOrderService.GetSaleOrderPrintMultipleAsync(User.ToAppState(), saleOrderIds);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/saleorder/queue/{customerLocationId}/{customerFacilityId}
        [HttpGet("Queue/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetSaleOrderPickQueue(int customerLocationId, int customerFacilityId)
        {
            var result = await _saleOrderService.GetSaleOrderPickQueueAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/saleorder/device/lookup/{customerFacilityId}?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetSaleOrderPickQueueLookupDevice(int customerFacilityId, [FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _saleOrderService.GetSaleOrderPickQueueLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/saleorder/device/update/pickingstatus
        [HttpPost("Device/Update/PickingStatus")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> UpdateSaleOrderPickingStatusDevice([FromBody] SaleOrderPickingStatusUpdateModel model)
        {
            var result = await _saleOrderService.UpdateSaleOrderPickingStatusDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}