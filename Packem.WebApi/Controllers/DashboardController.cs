using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Data.Enums;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;
using Packem.WebApi.Common.ExtensionMethods;
using System.Threading.Tasks;

namespace Packem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET /api/dashboard/inventoryflow/{customerLocationId}/{customerFacilityId}/{dateFilter}
        [HttpGet("InventoryFlow/{customerLocationId}/{customerFacilityId}/{dateFilter}")]
        public async Task<ActionResult> GetDashboardInventoryFlow(int customerLocationId, int customerFacilityId, LastDayFilterEnum dateFilter)
        {
            var result = await _dashboardService.GetDashboardInventoryFlowAsync(User.ToAppState(), customerLocationId, customerFacilityId, dateFilter);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/dashboard/queues/{customerLocationId}/{customerFacilityId}/{days}
        [HttpGet("Queues/{customerLocationId}/{customerFacilityId}/{days}")]
        public async Task<ActionResult> GetDashboardQueues(int customerLocationId, int customerFacilityId, int days)
        {
            var result = await _dashboardService.GetDashboardQueuesAsync(User.ToAppState(), customerLocationId, customerFacilityId, days);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/dashboard/topsalesorders/{customerLocationId}/{customerFacilityId}/{dateFilter}
        [HttpGet("TopSalesOrders/{customerLocationId}/{customerFacilityId}/{dateFilter}")]
        public async Task<ActionResult> GetDashboardTopSalesOrders(int customerLocationId, int customerFacilityId, LastDayFilterEnum dateFilter)
        {
            var result = await _dashboardService.GetDashboardTopSalesOrdersAsync(User.ToAppState(), customerLocationId, customerFacilityId, dateFilter);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/dashboard/lowstock/{customerId}
        [HttpGet("LowStock/{customerId}")]
        public async Task<ActionResult> GetDashboardLowStock(int customerId)
        {
            var result = await _dashboardService.GetDashboardLowStockAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/dashboard/operations/{customerLocationId}/{customerFacilityId}/{days}
        [HttpGet("Operations/{customerLocationId}/{customerFacilityId}/{days}")]
        public async Task<ActionResult> GetDashboardOperations(int customerLocationId, int customerFacilityId, int days)
        {
            var result = await _dashboardService.GetDashboardOperationsAsync(User.ToAppState(), customerLocationId, customerFacilityId, days);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}