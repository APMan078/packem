using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;
using Packem.Domain.Models;
using Packem.WebApi.Common.ExtensionMethods;
using System.Threading.Tasks;

namespace Packem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdjustBinQuantityController : ControllerBase
    {
        private readonly IAdjustBinQuantityService _adjustBinQuantityService;

        public AdjustBinQuantityController(IAdjustBinQuantityService adjustBinQuantityService)
        {
            _adjustBinQuantityService = adjustBinQuantityService;
        }

        // GET /api/adjustbinquantity/createadjustbinquantity/{itemId}/{customerLocationId}/{binId}
        [HttpGet("CreateAdjustBinQuantity/{itemId}/{customerLocationId}/{binId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCreateAdjustBinQuantity(int itemId, int customerLocationId, int binId)
        {
            var result = await _adjustBinQuantityService.GetCreateAdjustBinQuantityAsync(User.ToAppState(), itemId, customerLocationId, binId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/adjustbinquantity/createadjustbinquantity
        [HttpPost("CreateAdjustBinQuantity")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateAdjustBinQuantity([FromBody] AdjustBinQuantityCreateModel model)
        {
            var result = await _adjustBinQuantityService.CreateAdjustBinQuantityAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
