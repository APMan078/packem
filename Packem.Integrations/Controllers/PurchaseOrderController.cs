using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Integrations.ExtensionMethods;
using Packem.Integrations.Interfaces;
using Packem.Integrations.Models;

namespace Packem.Integrations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<PurchaseOrderController> _logger;
        //di pumapasok pag nag nag create ng new po
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(ILogger<PurchaseOrderController> logger
            ,
            IPurchaseOrderService purchaseOrderService
            )
        {
            _logger = logger;
            _purchaseOrderService = purchaseOrderService;
        }

        // POST /api/purchaseorder/createpurchaseorder
        [HttpPost("CreatePurchaseOrder")]

        public async Task<ActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderCreateModel model)
        {
            var result = await _purchaseOrderService.CreatePurchaseOrderAsync(model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result);
        }

        // GET /api/purchaseorder/detail/{purchaseOrderId}
        [HttpGet("Detail/{purchaseOrderId}")]
        public async Task<ActionResult> GetPurchaseOrderDetail(int purchaseOrderId)
        {
            var result = await _purchaseOrderService.GetPurchaseOrderDetailAsync(purchaseOrderId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

    }
}