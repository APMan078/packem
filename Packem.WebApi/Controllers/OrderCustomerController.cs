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
    public class OrderCustomerController : ControllerBase
    {
        private readonly IOrderCustomerService _orderCustomerService;

        public OrderCustomerController(IOrderCustomerService orderCustomerService)
        {
            _orderCustomerService = orderCustomerService;
        }

        // POST /api/ordercustomer/create
        [HttpPost("Create")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateOrderCustomer([FromBody] OrderCustomerCreateModel model)
        {
            var result = await _orderCustomerService.CreateOrderCustomerAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/ordercustomer/edit
        [HttpPost("Edit")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditOrderCustomer([FromBody] OrderCustomerEditModel model)
        {
            var result = await _orderCustomerService.EditOrderCustomerAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/ordercustomer/delete
        [HttpPost("Delete")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteOrderCustomer([FromBody] OrderCustomerDeleteModel model)
        {
            var result = await _orderCustomerService.DeleteOrderCustomerAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/ordercustomer/all/{customerId}
        [HttpGet("All/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetOrderCustomers(int customerId)
        {
            var result = await _orderCustomerService.GetOrderCustomersAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/ordercustomer/{orderCustomerId}
        [HttpGet("{orderCustomerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetOrderCustomer(int orderCustomerId)
        {
            var result = await _orderCustomerService.GetOrderCustomerAsync(User.ToAppState(), orderCustomerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/ordercustomer/management/{customerId}
        [HttpGet("Management/{customerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetOrderCustomerManagement(int customerId)
        {
            var result = await _orderCustomerService.GetOrderCustomerManagementAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/ordercustomer/Detail/{orderCustomerId}
        [HttpGet("Detail/{orderCustomerId}")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetOrderCustomerDetail(int orderCustomerId)
        {
            var result = await _orderCustomerService.GetOrderCustomerDetailAsync(User.ToAppState(), orderCustomerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}