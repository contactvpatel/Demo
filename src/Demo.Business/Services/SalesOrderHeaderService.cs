using Demo.Business.Interfaces;
using Demo.Core.Repositories;
using Demo.Util.FIQL;
using Microsoft.Extensions.Logging;

namespace Demo.Business.Services
{
    public class SalesOrderHeaderService : ISalesOrderHeaderService
    {
        private readonly ISalesOrderHeaderRepository _salesOrderHeaderRepository;
        private readonly ILogger<AddressService> _logger;

        public SalesOrderHeaderService(ISalesOrderHeaderRepository salesOrderHeaderRepository, ILogger<AddressService> logger)
        {
            _salesOrderHeaderRepository = salesOrderHeaderRepository ?? throw new ArgumentNullException(nameof(salesOrderHeaderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<dynamic> Get(QueryParam queryParam)
        {
            return await _salesOrderHeaderRepository.GetDynamic(queryParam.Fields, queryParam.Filters, queryParam.Include, queryParam.Sort, queryParam.PageNo, queryParam.PageSize);
        }
    }
}
