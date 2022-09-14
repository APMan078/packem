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
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // POST /api/item/createitem
        [HttpPost("CreateItem")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateItem([FromBody] ItemCreateModel model)
        {
            var result = await _itemService.CreateItemAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/item/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteItem([FromBody] ItemDeleteModel model)
        {
            var result = await _itemService.DeleteItemAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/item/update/expirationdate
        [HttpPost("Update/ExpirationDate")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateItemExpirationDate([FromBody] ItemExpirationDateUpdateModel model)
        {
            var result = await _itemService.UpdateItemExpirationDateAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/item/update/threshold
        [HttpPost("Update/Threshold")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateItemThreshold([FromBody] ItemThresholdUpdateModel model)
        {
            var result = await _itemService.UpdateItemThresholdAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/lookup/{customerId}?searchText={searchText}
        [HttpGet("Lookup/{customerId}")]
        [Authorize(Roles = "1,2,5")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemLookup(int customerId, [FromQuery] string searchText)
        {
            var result = await _itemService.GetItemLookupAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/lookup/detail/basic/{customerId}?searchText={searchText}
        [HttpGet("Lookup/Detail/Basic/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemLookupDetailBasic(int customerId, [FromQuery] string searchText)
        {
            var result = await _itemService.GetItemLookupDetailBasicAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/detail/{customerId}/{itemId}
        [HttpGet("Detail/{customerId}/{itemId}")]
        [Authorize(Roles = "1,2,5")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemDetail(int customerId, int itemId)
        {
            var result = await _itemService.GetItemDetailAsync(User.ToAppState(), customerId, itemId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/vendor/{vendorId}
        [HttpGet("Vendor/{vendorId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemByVendorId(int vendorId)
        {
            var result = await _itemService.GetItemByVendorIdAsync(User.ToAppState(), vendorId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/lookup/bysku/{customerId}?searchText={searchText}
        [HttpGet("Lookup/BySKU/{customerId}")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemLookupBySku(int customerId, [FromQuery] string searchText)
        {
            var result = await _itemService.GetItemLookupBySkuAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/lookup/sku/{customerId}?sku={sku}
        [HttpGet("Lookup/SKU/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemSkuLookup(int customerId, [FromQuery] string sku)
        {
            var result = await _itemService.GetItemSkuLookupAsync(User.ToAppState(), customerId, sku);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/lookup/purchaseorder/{purchaseOrderId}?searchText={searchText}
        [HttpGet("Lookup/PurchaseOrder/{purchaseOrderId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemPurchaseOrderLookup(int purchaseOrderId, [FromQuery] string searchText)
        {
            var result = await _itemService.GetItemPurchaseOrderLookupAsync(User.ToAppState(), purchaseOrderId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/lookup/manualreceipt/{customerId}?searchText={searchText}
        [HttpGet("Lookup/ManualReceipt/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetItemManualReceiptLookup(int customerId, [FromQuery] string searchText)
        {
            var result = await _itemService.GetItemManualReceiptLookupAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/device/lookup/{customerFacilityId}?searchText={searchText}
        [HttpGet("Device/Lookup/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetItemLookupDevice(int customerFacilityId, [FromQuery] string searchText)
        {
            var result = await _itemService.GetItemLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/item/device/lookup/sku/{customerFacilityId}?sku={sku}
        [HttpGet("Device/Lookup/SKU/{customerFacilityId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetItemLookupSkuDeviceAsync(int customerFacilityId, [FromQuery] string sku)
        {
            var result = await _itemService.GetItemLookupSkuDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, sku);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        //// GET /api/item/device/image/{vendorId}/{itemId}
        //[HttpGet("Device/Image/{vendorId}/{itemId}")]
        //[Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        //public ActionResult GetItemImageDevice(int vendorId, int itemId)
        //{
        //    var state = User.ToCustomerDeviceTokenAuth();
        //    var path = Path.Combine(_env.WebRootPath,
        //        "images", "items", state.CustomerId.ToString(), state.CustomerLocationId.ToString(), vendorId.ToString());

        //    Directory.CreateDirectory(path);

        //    // images/items/{CustomerId}/{CustomerLocationId}/{VendorId}/{ItemId}.jpeg
        //    var filePath = Path.Combine(path, $"{itemId}.jpg");

        //    if (System.IO.File.Exists(filePath))
        //    {
        //        return PhysicalFile(filePath, "image/jpeg");
        //    }
        //    else
        //    {
        //        var notFoundPath = Path.Combine(_env.WebRootPath, "images", "items", "no_image.jpg");
        //        return PhysicalFile(notFoundPath, "image/jpeg");
        //    }
        //}

        //// GET /api/item/device/image/byte/{vendorId}/{itemId}
        //[HttpGet("Device/Image/byte/{vendorId}/{itemId}")]
        //[Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        //public ActionResult GetItemImageByteDevice(int vendorId, int itemId)
        //{
        //    var state = User.ToCustomerDeviceTokenAuth();
        //    var path = Path.Combine(_env.WebRootPath,
        //        "images", "items", state.CustomerId.ToString(), state.CustomerLocationId.ToString(), vendorId.ToString());

        //    Directory.CreateDirectory(path);

        //    // images/items/{CustomerId}/{CustomerLocationId}/{VendorId}/{ItemId}.jpeg
        //    var filePath = Path.Combine(path, $"{itemId}.jpg");

        //    if (System.IO.File.Exists(filePath))
        //    {
        //        return Ok(System.IO.File.ReadAllBytes(filePath));
        //    }
        //    else
        //    {
        //        var notFoundPath = Path.Combine(_env.WebRootPath, "images", "items", "no_image.jpg");
        //        return Ok(System.IO.File.ReadAllBytes(notFoundPath));
        //    }
        //}
    }
}
