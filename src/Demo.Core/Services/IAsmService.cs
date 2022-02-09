using Demo.Core.Models;

namespace Demo.Core.Services
{
    public interface IAsmService
    {
        Task<IEnumerable<ApplicationSecurityResponseModel>> Get(
            ApplicationSecurityRequestModel applicationSecurityRequestModel);
    }
}