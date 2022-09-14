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
    public class BinController : ControllerBase
    {
        private readonly IBinService _binService;

        public BinController(IBinService binService)
        {
            _binService = binService;
        }

        // POST /api/bin/createbin
        [HttpPost("CreateBin")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateBin([FromBody] BinCreateModel model)
        {
            var result = await _binService.CreateBinAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/bin/createbin
        [HttpPost("AddImportedZonesBins/{customerLocationId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddImportedZonesAndBins([FromRoute] int customerLocationId, [FromBody] ZoneAndBinImportModel[] model)
        {
            var result = await _binService.AddImportedZonesAndBins(User.ToAppState(), customerLocationId, model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // PUT /api/bin/{binId}
        [HttpPut("{binId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditBin([FromBody] BinEditModel model)
        {
            var result = await _binService.EditBinAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/bins/{customerLocationId}
        [HttpGet("Bins/{customerLocationId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBins(int customerLocationId)
        {
            var result = await _binService.GetBinByCustomerLocationIdAsync(User.ToAppState(), customerLocationId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/{binId}
        [HttpGet("{binId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBin(int binId)
        {
            var result = await _binService.GetBinAsync(User.ToAppState(), binId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/lookup/{zoneId}?searchText={searchText}
        [HttpGet("Lookup/{zoneId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBinLookup(int zoneId, [FromQuery] string searchText)
        {
            var result = await _binService.GetBinLookupAsync(User.ToAppState(), zoneId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/lookup/itemquantity/{itemId}/{zoneId}?searchText={searchText}
        [HttpGet("Lookup/ItemQuantity/{itemId}/{zoneId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBinLookupItemQuantity(int itemId, int zoneId, [FromQuery] string searchText)
        {
            var result = await _binService.GetBinLookupItemQuantityAsync(User.ToAppState(), itemId, zoneId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/storagemanagement/{customerLocationId}/{customerFacilityId}
        [HttpGet("StorageManagement/{customerLocationId}/{customerFacilityId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBinStorageManagement(int customerLocationId, int customerFacilityId)
        {
            var result = await _binService.GetBinStorageManagementAsync(User.ToAppState(), customerLocationId, customerFacilityId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/storagemanagement/detail/{binId}
        [HttpGet("StorageManagement/Detail/{binId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBinStorageManagementDetail(int binId)
        {
            var result = await _binService.GetBinStorageManagementDetailAsync(User.ToAppState(), binId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/device/inventory/{customerFacilityId}/{itemId}
        [HttpGet("Device/Inventory/{customerFacilityId}/{itemId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetBinZoneDevice(int customerFacilityId, int itemId)
        {
            var result = await _binService
                .GetBinZoneDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, itemId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/device/lookup/{zoneId}?searchText={searchText}
        [HttpGet("Device/Lookup/{zoneId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetBinLookupDevice(int zoneId, [FromQuery] string searchText)
        {
            var result = await _binService
                .GetBinLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), zoneId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/device/lookup/itemquantitylot/{zoneId}/{itemId}?searchText={searchText}
        [HttpGet("Device/Lookup/ItemQuantityLot/{zoneId}/{itemId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetBinItemQuantityLotLookupDevice(int zoneId, int itemId, [FromQuery] string searchText)
        {
            var result = await _binService
                .GetBinItemQuantityLotLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), zoneId, itemId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/device/lookup/itemquantity/{customerFacilityId}/{itemId}/{zoneId}?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup/ItemQuantity/{customerFacilityId}/{itemId}/{zoneId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetBinLookupItemQuantityDevice(int customerFacilityId, int itemId, int zoneId, [FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _binService
                .GetBinLookupItemQuantityDeviceAsync(User.ToCustomerDeviceTokenAuth(), customerFacilityId, itemId, zoneId, searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/device/itemquantity/{itemId}/{binId}
        [HttpGet("Device/ItemQuantity/{itemId}/{binId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetBinItemQuantityDevice(int itemId, int binId)
        {
            var result = await _binService
                .GetBinItemQuantityDeviceAsync(User.ToCustomerDeviceTokenAuth(), itemId, binId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/bin/device/lookup/pallet/optional/{zoneId}?searchText={searchText}
        [HttpGet("Device/Lookup/Pallet/Optional/{zoneId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetBinLookupOptionalPalletDevice(int zoneId, [FromQuery] string searchText)
        {
            var result = await _binService
                .GetBinLookupOptionalPalletDeviceAsync(User.ToCustomerDeviceTokenAuth(), zoneId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}