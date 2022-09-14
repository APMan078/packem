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
    public class LotController : ControllerBase
    {
        private readonly ILotService _lotService;

        public LotController(ILotService lotService)
        {
            _lotService = lotService;
        }

        // POST /api/lot/create
        [HttpPost("Create")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateLot([FromBody] LotCreateModel model)
        {
            var result = await _lotService.CreateLotAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/lot/lookup/{itemId}?searchText={searchText}
        [HttpGet("Lookup/{itemId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetLotLookup(int itemId, [FromQuery] string searchText)
        {
            var result = await _lotService.GetLotLookupByItemIdAsync(User.ToAppState(), itemId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/lot/{itemId}/{binId}
        [HttpGet("{itemId}/{binId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetLotByItemIdAndBinId(int itemId, int binId)
        {
            var result = await _lotService.GetLotByItemIdAndBinIdAsync(User.ToAppState(), itemId, binId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/lot/device/lookup/{itemId}?searchText={searchText}
        [HttpGet("Device/Lookup/{itemId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetLotLookupDevice(int itemId, [FromQuery] string searchText)
        {
            var result = await _lotService.GetLotLookupByItemIdDeviceAsync(User.ToCustomerDeviceTokenAuth(), itemId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/lot/device/create
        [HttpPost("Device/Create")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> CreateLotDevice([FromBody] LotCreateModel model)
        {
            var result = await _lotService.CreateLotDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}