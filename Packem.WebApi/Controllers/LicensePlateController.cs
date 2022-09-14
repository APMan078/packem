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
    public class LicensePlateController : ControllerBase
    {
        private readonly ILicensePlateService _licensePlateService;

        public LicensePlateController(ILicensePlateService licensePlateService)
        {
            _licensePlateService = licensePlateService;
        }

        // GET /api/licenseplate/generate/{customerId}
        [HttpGet("Generate/{customerId}")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetGenerateLicensePlateNo(int customerId)
        {
            var result = await _licensePlateService.GetGenerateLicensePlateNoAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/licenseplate/create/unknown
        [HttpPost("Create/Unknown")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateLicensePlateUnknown([FromBody] LicensePlateUnknownCreateModel model)
        {
            var result = await _licensePlateService.CreateLicensePlateUnknownAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/licenseplate/create/known
        [HttpPost("Create/Known")]
        [Authorize(Roles = "1,2")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateLicensePlateKnown([FromBody] LicensePlateKnownCreateModel model)
        {
            var result = await _licensePlateService.CreateLicensePlateKnownAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/licenseplate/lookup/lpno/{customerId}?searchText={searchText}
        [HttpGet("Lookup/LPNo/{customerId}")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetLicensePlateLookupByLicensePlateNo(int customerId, [FromQuery] string searchText)
        {
            var result = await _licensePlateService.GetLicensePlateLookupByLicensePlateNoAsync(User.ToAppState(), customerId, searchText);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/licenseplate/assignment/{customerId}/{licensePlateId}
        [HttpGet("Assignment/{customerId}/{licensePlateId}")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetLicensePlateKnownAssignment(int customerId, int licensePlateId)
        {
            var result = await _licensePlateService.GetLicensePlateKnownAssignmentAsync(User.ToAppState(), customerId, licensePlateId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/licenseplate/edit/known
        [HttpPost("Edit/Known")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditLicensePlateKnownAssignment([FromBody] LicensePlateKnownAssignmentEditModel model)
        {
            var result = await _licensePlateService.EditLicensePlateKnownAssignmentAsync(User.ToAppState(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/licenseplate/history/{customerId}
        [HttpGet("History/{customerId}")]
        [Authorize(Roles = "1,2,4")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetLicensePlateHistory(int customerId)
        {
            var result = await _licensePlateService.GetLicensePlateHistoryAsync(User.ToAppState(), customerId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/licenseplate/device/lookup?searchText={searchText}&barcodeSearch={barcodeSearch}
        [HttpGet("Device/Lookup")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetLicensePlateLookupDevice([FromQuery] string searchText, [FromQuery] bool barcodeSearch)
        {
            var result = await _licensePlateService.GetLicensePlateLookupDeviceAsync(User.ToCustomerDeviceTokenAuth(), searchText, barcodeSearch);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/licenseplate/device/assignment/{licensePlateId}
        [HttpGet("Device/Assignment/{licensePlateId}")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> GetLicensePlateKnownAssignmentDevice(int licensePlateId)
        {
            var result = await _licensePlateService.GetLicensePlateKnownAssignmentDeviceAsync(User.ToCustomerDeviceTokenAuth(), licensePlateId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/licenseplate/device/edit/unknown
        [HttpPost("Device/Edit/Unknown")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> EditLicensePlateUnknownToPalletizedDevice([FromBody] LicensePlateUnknownToPalletizedEditModel model)
        {
            var result = await _licensePlateService.EditLicensePlateUnknownToPalletizedDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // POST /api/licenseplate/device/edit/known
        [HttpPost("Device/Edit/Known")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> EditLicensePlateKnownToPalletizedDevice([FromBody] LicensePlateKnownToPalletizedEditModel model)
        {
            var result = await _licensePlateService.EditLicensePlateKnownToPalletizedDeviceAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}