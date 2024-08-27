using Demo.Business.Interfaces;
using Demo.Business.Mapper;
using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Util.FIQL;
using Demo.Util.Logging;
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

        public async Task<IEnumerable<Models.ProductModel>> Get()
        {
            var productList = await _productRepository.GetAllAsync();
            return ObjectMapper.Mapper.Map<IEnumerable<Models.ProductModel>>(productList);
        }

        public async Task<PagedList<Models.ProductModel>> Get(PaginationQuery paginationQuery)
        {
            var productList = await _productRepository.Get(paginationQuery);
            return ObjectMapper.Mapper.Map<PagedList<Models.ProductModel>>(productList);
        }

        public async Task<Models.ProductModel> GetById(int id)
        {
            return ObjectMapper.Mapper.Map<Models.ProductModel>(await _productRepository.GetByIdAsync(id));
        }

        public async Task<IEnumerable<Models.ProductModel>> GetByCategoryId(int categoryId)
        {
            var productList = await _productRepository.GetByCategoryId(categoryId);
            return ObjectMapper.Mapper.Map<IEnumerable<Models.ProductModel>>(productList);
        }

        public async Task<Models.ProductModel> Create(Models.ProductModel productModel)
        {
            var newProduct = await _productRepository.GetByIdAsync(productModel.ProductId);
            if (newProduct != null)
                throw new ApplicationException($"Product already exits.");

            var mappedEntity = ObjectMapper.Mapper.Map<Product>(productModel);
            if (mappedEntity == null)
                throw new ApplicationException($"Product could not be mapped.");

            var newEntity = await _productRepository.AddAsync(mappedEntity);
            _logger.LogInformationExtension($"Product successfully added.");

            return ObjectMapper.Mapper.Map<Models.ProductModel>(newEntity);
        }

        public async Task Update(Models.ProductModel productModel)
        {
            var editProduct = await _productRepository.GetByIdAsync(productModel.ProductId);
            if (editProduct == null)
                throw new ApplicationException($"Product could not be loaded.");

            ObjectMapper.Mapper.Map(productModel, editProduct);

            await _productRepository.UpdateAsync(editProduct);
            _logger.LogInformationExtension($"Product successfully updated.");
        }

        public async Task Delete(Models.ProductModel productModel)
        {
            var deletedProduct = await _productRepository.GetByIdAsync(productModel.ProductId);
            if (deletedProduct == null)
                throw new ApplicationException($"Product could not be loaded.");

            await _productRepository.DeleteAsync(ObjectMapper.Mapper.Map<Product>(deletedProduct));
            _logger.LogInformationExtension($"Product successfully deleted.");
        }

        public async Task<dynamic> Get(QueryParam queryParam)
        {
            return await _productRepository.GetDynamic(queryParam.Fields, queryParam.Filters, queryParam.Include, queryParam.Sort, queryParam.PageNo, queryParam.PageSize);
        }
    }
}
