using Demo.Business.Interfaces;
using Demo.Core.Repositories;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResponseModel> Get(QueryParam queryParam)
        {
            return await _productRepository.GetDynamic(queryParam.Fields, queryParam.Filters, queryParam.Include, queryParam.Sort, queryParam.PageNo, queryParam.PageSize);
        }
    }
}
