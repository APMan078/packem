using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET /api/auth/test
        [AllowAnonymous]
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("test");
        }

        // POST /api/auth/requestusertoken
        [AllowAnonymous]
        [HttpPost("RequestUserToken")]
        public async Task<ActionResult> RequestUserToken([FromBody] UserLoginModel model)
        {
            var result = await _authService.AuthenticateUserAsync(model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("RequestPasswordResetTokenEmail")]
        public async Task<IActionResult> RequestPasswordResetEmail([FromBody] ResetPasswordModel model)
        {
            var requestResult = await _authService.ResetPasswordRequestAsync(model);

            if (requestResult.IsFailed)
            {
                return BadRequest(requestResult.Errors.ToErrorString());
            }

            return Ok(requestResult.Value);
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestCreateModel model)
        {
            var handler = new JwtSecurityTokenHandler();
            var json = handler.ReadJwtToken(model.Token);
            var date = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var exp = int.Parse(json.Claims.First(claim => claim.Type == "exp").Value);
            var email = json.Claims.First(claim => claim.Type == "Claim_Email").Value;

            if (exp < date || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid Token");
            }

            var resetPassRequest = await _authService.ResetPasswordAsync(email, model.Password);
            return Ok(resetPassRequest.Value);
        }

        //// POST /api/auth/requestusertoken/device
        //[AllowAnonymous]
        //[HttpPost("RequestUserToken/Device")]
        //public async Task<ActionResult> RequestUserTokenDevice([FromBody] UserLoginDeviceModel model)
        //{
        //    var result = await _authService.AuthenticateUserAsync(model);

        //    if (result.IsFailed)
        //    {
        //        return BadRequest(result.Errors.ToErrorString());
        //    }

        //    return Ok(result.Value);
        //}

        // GET /api/auth/usertokeninfo

        [HttpGet("UserTokenInfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UserTokenInfo()
        {
            return Ok(User.ToAppState());
        }

        // POST /api/auth/validatecustomerdevicetoken
        [HttpPost("ValidateCustomerDeviceToken")]
        [AllowAnonymous]
        public async Task<ActionResult> ValidateCustomerDeviceToken([FromBody] CustomerDeviceTokenValidateTokenModel model)
        {
            var result = await _authService.ValidateCustomerDeviceTokenAsync(model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }

        // GET /api/auth/customerdevicetokeninfo
        [HttpGet("CustomerDeviceTokenInfo")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public ActionResult CustomerDeviceTokenInfo()
        {
            return Ok(User.ToCustomerDeviceTokenAuth());
        }

        // POST /api/auth/deactivatecustomerdevicetoken
        [HttpPost("DeactivateCustomerDeviceToken")]
        [Authorize(AuthenticationSchemes = DeviceTokenAuthOptions.DeviceTokenScemeName)]
        public async Task<ActionResult> DeactivateCustomerDeviceToken([FromBody] CustomerDeviceTokenValidateTokenModel model)
        {
            var result = await _authService.DeactivateCustomerDeviceTokenAsync(User.ToCustomerDeviceTokenAuth(), model);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.ToErrorString());
            }

            return Ok(result.Value);
        }
    }
}
