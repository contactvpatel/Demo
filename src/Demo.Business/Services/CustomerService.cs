using AutoMapper.Internal.Mappers;
using Demo.Business.Interfaces;
using Demo.Business.Mapper;
using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Util.FIQL;
using Demo.Util.Logging;
using Demo.Util.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Models.CustomerModel> GetById(int id)
        {
            return ObjectMapper.Mapper.Map<Models.CustomerModel>(await _customerRepository.GetByIdAsync(id));
        }
        public async Task<Models.CustomerModel> Create(Models.CustomerModel customerModel)
        {
            var existingData = await _customerRepository.GetByIdAsync(customerModel.CustomerId);
            if (existingData != null)
                throw new ApplicationException($"Customer already exits.");

            var mappedEntity = ObjectMapper.Mapper.Map<Customer>(customerModel);
            if (mappedEntity == null)
                throw new ApplicationException($"Customer could not be mapped.");

            var newEntity = await _customerRepository.AddAsync(mappedEntity);
            _logger.LogInformationExtension($"Customer successfully added.");

            return ObjectMapper.Mapper.Map<Models.CustomerModel>(newEntity);
        }

        public async Task Update(Models.CustomerModel customerModel)
        {
            var existingData = await _customerRepository.GetByIdAsync(customerModel.CustomerId);
            if (existingData == null)
                throw new ApplicationException($"Customer could not be loaded.");

            ObjectMapper.Mapper.Map(customerModel, existingData);

            await _customerRepository.UpdateAsync(existingData);
            _logger.LogInformationExtension($"Customer successfully updated.");
        }

        public async Task Delete(Models.CustomerModel customerModel)
        {
            var existingData = await _customerRepository.GetByIdAsync(customerModel.CustomerId);
            if (existingData == null)
                throw new ApplicationException($"Customer could not be loaded.");

            await _customerRepository.DeleteAsync(ObjectMapper.Mapper.Map<Customer>(existingData));
            _logger.LogInformationExtension($"Customer successfully deleted.");
        }

        public async Task<ResponseModel> Get(QueryParam queryParam)
        {
            return await _customerRepository.GetDynamic(queryParam.Fields, queryParam.Filters, queryParam.Include, queryParam.Sort, queryParam.PageNo, queryParam.PageSize);
        }
    }
}
