using Demo.Core.Models;
using ISsoService = Demo.Core.Services.ISsoService;

namespace Demo.Business.Services
{
    public class SsoService : Interfaces.ISsoService
    {
        private readonly ISsoService _ssoService;

        public SsoService(ISsoService ssoService)
        {
            _ssoService = ssoService ?? throw new ArgumentNullException(nameof(ssoService));
        }

        public async Task<bool> ValidateToken(string token)
        {
            return await _ssoService.ValidateToken(token);
        }

        public async Task<SsoAuthModel> RenewToken(string token, string refreshToken)
        {
            return await _ssoService.RenewToken(token, refreshToken);
        }

        public async Task<bool> Logout(string token)
        {
            return await _ssoService.Logout(token);
        }
    }
}
