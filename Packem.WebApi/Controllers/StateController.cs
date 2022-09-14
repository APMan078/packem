using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;

namespace Packem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        // GET /api/state/states
        [HttpGet("States")]
        public ActionResult GetStates()
        {
            var result = _stateService.GetStates();

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/state/{stateId}
        [HttpGet("{stateId}")]
        public ActionResult GetState(int stateId)
        {
            var result = _stateService.GetState(stateId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/state/statepostals
        [HttpGet("StatePostals")]
        public ActionResult GetStatePostals()
        {
            var result = _stateService.GetStatePostals();

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/state/statepostal/{stateId}
        [HttpGet("StatePostal/{stateId}")]
        public ActionResult GetStatePostal(int stateId)
        {
            var result = _stateService.GetStatePostal(stateId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
