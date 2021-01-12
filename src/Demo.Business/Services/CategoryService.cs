using System;
using System.Threading.Tasks;
using Demo.Business.Interfaces;
using Demo.Business.Mapper;
using Demo.Business.Models;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Demo.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedList<CategoryModel>> Get(QueryStringParameters queryStringParameters)
        {
            var category = await _categoryRepository.Get(queryStringParameters);
            return ObjectMapper.Mapper.Map<PagedList<CategoryModel>>(category);
        }

        public async Task<CategoryModel> GetById(int id)
        {
            // Example of using Base Repository method without using Infrastructure Layer's CategoryRepository
            // var category = await _categoryRepository.GetByIdAsync(categoryId);
            // return ObjectMapper.Mapper.Map<CategoryModel>(category);

            //Example of using Infrastructure Layer's CategoryRepository with Specification Pattern
            var category = await _categoryRepository.GetById(id);
            return ObjectMapper.Mapper.Map<CategoryModel>(category);
        }
    }
}
