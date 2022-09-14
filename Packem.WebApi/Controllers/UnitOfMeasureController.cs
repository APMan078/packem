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
    public class UnitOfMeasureController : ControllerBase
    {
        private readonly IUnitOfMeasureService _unitOfMeasureService;

        public UnitOfMeasureController(IUnitOfMeasureService unitOfMeasureService)
        {
            _unitOfMeasureService = unitOfMeasureService;
        }

        // POST /api/unitofmeasure/creatunitofmeasure
        [HttpPost("CreatUnitOfMeasure")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateUnitOfMeasureForCustomer([FromBody] UnitOfMeasureForCustomerCreateModel model)
        {
            var result = await _unitOfMeasureService.CreateUnitOfMeasureForCustomerAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/unitofmeasure/creatunitofmeasure/custom
        [HttpPost("CreatUnitOfMeasure/Custom")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateCustomUnitOfMeasureForCustomer([FromBody] CustomUnitOfMeasureForCustomerCreateModel model)
        {
            var result = await _unitOfMeasureService.CreateCustomUnitOfMeasureForCustomerAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/unitofmeasure/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteCustomerUnitOfMeasure([FromBody] CustomerUnitOfMeasureDeleteModel model)
        {
            var result = await _unitOfMeasureService.DeleteCustomerUnitOfMeasureAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/unitofmeasure/lookup?searchText={searchText}
        [HttpGet("Lookup")]
        [Authorize(Roles = "1,2,5")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetDefaultUnitOfMeasures([FromQuery] string searchText)
        {
            var result = await _unitOfMeasureService.GetDefaultUnitOfMeasuresAsync(User.ToAppState(), searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/unitofmeasure/lookup/{customerId}?searchText={searchText}
        [HttpGet("Lookup/{customerId}")]
        [Authorize(Roles = "1,2,5")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCustomerUnitOfMeasures(int customerId, [FromQuery] string searchText)
        {
            var result = await _unitOfMeasureService.GetCustomerUnitOfMeasuresAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}