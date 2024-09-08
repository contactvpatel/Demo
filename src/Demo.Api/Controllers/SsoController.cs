using Asp.Versioning;
using Demo.Api.Extensions;
using Demo.Api.Models;
using Demo.Core.Models;
using Demo.Util.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    /// <summary>
    /// SSO Controller. 
    /// Contain SSO API
    /// </summary>
    [Route("api/v{version:apiVersion}/sso")]
    [ApiController]
    [ApiVersion("1")]
    public class SsoController : ControllerBase
    {
        private readonly Business.Interfaces.ISsoService _ssoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MisController> _logger;

        public SsoController(Business.Interfaces.ISsoService ssoService, IHttpContextAccessor httpContextAccessor, ILogger<MisController> logger)
        {
            _ssoService = ssoService ?? throw new ArgumentNullException(nameof(ssoService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("renew-token", Name = "RenewToken")]
        [AllowAnonymous]
        public async Task<ActionResult<SsoAuthModel>> RenewToken([FromBody] SsoRenewTokenModel ssoRenewTokenModel)
        {
            _logger.LogInformationExtension("Renew Token requested");

            var token = UserExtensions.GetUserToken(_httpContextAccessor);

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(ssoRenewTokenModel.RefreshToken))
            {
                var ssoAuthResponse = await _ssoService.RenewToken(token, ssoRenewTokenModel.RefreshToken);
                return Ok(new Response<SsoAuthModel>(ssoAuthResponse, null));
            }

            return Unauthorized(new Response<bool>(false, "Renew Token failed - Access Token or Refresh Token is missing"));
        }

        [HttpPost("logout", Name = "Logout")]
        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            _logger.LogInformationExtension("Logout requested");
            var token = UserExtensions.GetUserToken(_httpContextAccessor);
            if (!string.IsNullOrEmpty(token))
            {
                if (await _ssoService.Logout(token))
                {
                    return Ok(new Response<bool>(true, "Logout is successfully completed"));
                }
            }

            return Unauthorized(new Response<bool>(false, "Logout failed"));
        }
    }
}