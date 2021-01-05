using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Business.Interfaces;
using Demo.Business.Mapper;
using Demo.Business.Models;
using Demo.Core.Communication;
using Demo.Core.Entities;
using Demo.Core.Repositories;
using Demo.Util.Logging;
using Microsoft.Extensions.Logging;

namespace Demo.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IOrderServiceProxy _orderCommunication;

        public ProductService(IProductRepository productRepository, IOrderServiceProxy orderCommunication, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _orderCommunication = orderCommunication ?? throw new ArgumentNullException(nameof(orderCommunication));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ProductModel>> GetAll()
        {
            var productList = await _productRepository.GetAll();
            return ObjectMapper.Mapper.Map<IEnumerable<ProductModel>>(productList);
        }

        public async Task<ProductModel> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return ObjectMapper.Mapper.Map<ProductModel>(product);
        }

        public async Task<IEnumerable<ProductModel>> GetByName(string productName)
        {
            var productList = await _productRepository.GetByName(productName);

            // Note: Get the data from Order Microservice. This is an example of how to get data from another Microservice.
            var orderList = await _orderCommunication.GetOrders(productName);

            return ObjectMapper.Mapper.Map<IEnumerable<ProductModel>>(productList);
        }

        public async Task<IEnumerable<ProductModel>> GetByCategoryId(int categoryId)
        {
            var productList = await _productRepository.GetByCategoryId(categoryId);
            return ObjectMapper.Mapper.Map<IEnumerable<ProductModel>>(productList);
        }

        public async Task<ProductModel> Create(ProductModel productModel)
        {
            var newProduct = await _productRepository.GetByName(productModel.Name);
            if (newProduct != null)
                throw new ApplicationException($"Product already exits.");
            
            var mappedEntity = ObjectMapper.Mapper.Map<Product>(productModel);
            if (mappedEntity == null)
                throw new ApplicationException($"Product could not be mapped."); 

            var newEntity = await _productRepository.AddAsync(mappedEntity);
            _logger.LogInformationExtension($"Product successfully added.");

            return ObjectMapper.Mapper.Map<ProductModel>(newEntity);
        }

        public async Task Update(ProductModel productModel)
        {
            var editProduct = await _productRepository.GetByIdAsync(productModel.ProductId);
            if (editProduct == null)
                throw new ApplicationException($"Product could not be loaded.");

            ObjectMapper.Mapper.Map(productModel, editProduct);

            await _productRepository.UpdateAsync(editProduct);
            _logger.LogInformationExtension($"Product successfully updated.");
        }

        public async Task Delete(ProductModel productModel)
        {
            var deletedProduct = await _productRepository.GetByIdAsync(productModel.ProductId);
            if (deletedProduct == null)
                throw new ApplicationException($"Product could not be loaded.");

            await _productRepository.DeleteAsync(ObjectMapper.Mapper.Map<Product>(deletedProduct));
            _logger.LogInformationExtension($"Product successfully deleted.");
        }
              
    }
}
