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
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        // POST /api/purchaseorder/createpurchaseorder
        [HttpPost("CreatePurchaseOrder")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderCreateModel model)
        {
            var result = await _purchaseOrderService.CreatePurchaseOrderAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/purchaseorder/createpurchaseorder
        [HttpPost("AddImportedPurchaseOrders/{customerLocationId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddImportedPurchaseOrders([FromRoute] int customerLocationId, [FromBody] PurchaseOrderImportModel[] model)
        {
            var result = await _purchaseOrderService.AddImportedPurchaseOrdersAsync(User.ToAppState(), customerLocationId, model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/purchaseorder/lookup/{customerFacilityId}?searchText={searchText}
        [HttpGet("Lookup/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetPurchaseOrderLookup(int customerFacilityId, [FromQuery] string searchText)
        {
            var result = await _purchaseOrderService.GetPurchaseOrderLookupAsync(User.ToAppState(), customerFacilityId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/purchaseorder/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeletePurchaseOrder([FromBody] PurchaseOrderDeleteModel model)
        {
            var result = await _purchaseOrderService
                .DeletePurchaseOrderAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/purchaseorder/detail/{purchaseOrderId}
        [HttpGet("Detail/{purchaseOrderId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetPurchaseOrderDetail(int purchaseOrderId)
        {
            var result = await _purchaseOrderService.GetPurchaseOrderDetailAsync(User.ToAppState(), purchaseOrderId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/purchaseorder/purchaseorders
        [HttpGet("PurchaseOrders/GetPurchaseOrdersByCustomerId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetPurchaseOrdersByCustomerId()
        {
            var result = await _purchaseOrderService
                .GetPurchaseOrderByCustomerIdAsync(User.ToAppState());

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/purchaseorder/device/lookup/poreceive/{customerFacilityId}?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup/POReceive/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetPurchaseOrderLookupPOReceiveDevice(int customerFacilityId, [FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _purchaseOrderService
                .GetPurchaseOrderLookupPOReceiveDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}