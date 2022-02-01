using Demo.Core.Services;
using ISsoService = Demo.Core.Services.ISsoService;

namespace Demo.Business.Services
{
    public class SsoService : Interfaces.ISsoService
    {
        private readonly ISsoService _ssoService;
        private readonly IRedisCacheService _redisCacheService;

        public SsoService(ISsoService ssoService, IRedisCacheService redisCacheService)
        {
            _ssoService = ssoService ?? throw new ArgumentNullException(nameof(ssoService));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task<bool> ValidateToken(string token)
        {
            return await _ssoService.ValidateToken(token);
        }

        public async Task<bool> Logout(string token)
        {
            return await _ssoService.Logout(token);
        }
    }
}
