using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Models;
using Packem.WebApi.Common.ExtensionMethods;
using System.Threading.Tasks;

namespace Packem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderCustomerAddressController : ControllerBase
    {
        private readonly IOrderCustomerAddressService _orderCustomerAddressService;

        public OrderCustomerAddressController(IOrderCustomerAddressService orderCustomerAddressService)
        {
            _orderCustomerAddressService = orderCustomerAddressService;
        }

        // POST /api/ordercustomeraddress/create
        [HttpPost("Create")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateOrderCustomerAddress([FromBody] OrderCustomerAddressCreateModel model)
        {
            var result = await _orderCustomerAddressService.CreateOrderCustomerAddressAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/ordercustomeraddress/edit
        [HttpPost("Edit")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditOrderCustomerAddress([FromBody] OrderCustomerAddressEditModel model)
        {
            var result = await _orderCustomerAddressService.EditOrderCustomerAddressAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/ordercustomeraddress/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteOrderCustomerAddress([FromBody] OrderCustomerAddressDeleteModel model)
        {
            var result = await _orderCustomerAddressService.DeleteOrderCustomerAddressAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/ordercustomeraddress/all/{orderCustomerId}?addressType={addressType}
        [HttpGet("All/{orderCustomerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetOrderCustomerAddresses(int orderCustomerId, [FromQuery]OrderCustomerAddressType? addressType)
        {
            var result = await _orderCustomerAddressService.GetOrderCustomerAddressesAsync(User.ToAppState(), orderCustomerId, addressType);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/ordercustomeraddress/{orderCustomerAddressId}
        [HttpGet("{orderCustomerAddressId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetOrderCustomerAddress(int orderCustomerAddressId)
        {
            var result = await _orderCustomerAddressService.GetOrderCustomerAddressAsync(User.ToAppState(), orderCustomerAddressId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}