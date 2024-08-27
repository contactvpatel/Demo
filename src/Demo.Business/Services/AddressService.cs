using Demo.Business.Interfaces;
using Demo.Core.Repositories;
using Demo.Util.FIQL;
using Microsoft.Extensions.Logging;

namespace Demo.Business.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger)
        {
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<dynamic> Get(QueryParam queryParam)
        {
            return await _addressRepository.GetDynamic(queryParam.Fields, queryParam.Filters, queryParam.Include, queryParam.Sort, queryParam.PageNo, queryParam.PageSize);
        }
    }
}
