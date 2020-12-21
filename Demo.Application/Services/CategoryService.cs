using Demo.Application.Mapper;
using Demo.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Core.Repositories;
using Demo.Application.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Application.Services
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

        public async Task<IEnumerable<CategoryModel>> GetAll()
        {
            // Example of using Base Repository method without using Infrastructure Layer's CategoryRepository
            var category = await _categoryRepository.GetAllAsync();
            return ObjectMapper.Mapper.Map<IEnumerable<CategoryModel>>(category);
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
