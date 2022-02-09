using Demo.Core.Models;

namespace Demo.Business.Interfaces
{
    public interface IAsmService
    {
        Task<IEnumerable<ApplicationSecurityResponseModel>> Get(
            ApplicationSecurityRequestModel applicationSecurityRequestModel);
    }
}
