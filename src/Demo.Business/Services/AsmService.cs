using Demo.Business.Interfaces;
using Demo.Core.Models;

namespace Demo.Business.Services
{
    public class AsmService : IAsmService
    {
        private readonly Core.Services.IAsmService _asmService;

        public AsmService(Core.Services.IAsmService asmService)
        {
            _asmService = asmService ?? throw new ArgumentNullException(nameof(asmService));
        }

        public async Task<IEnumerable<ApplicationSecurityResponseModel>> Get(ApplicationSecurityRequestModel applicationSecurityRequestModel)
        {
            return await _asmService.Get(applicationSecurityRequestModel);
        }
    }
}
