using Demo.Business.Interfaces;
using Demo.Business.Mapper;
using Demo.Business.Models;
using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Util.Logging;
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

        public async Task<IEnumerable<CategoryModel>> Get()
        {
            var category = await _categoryRepository.GetAllAsync();
            return ObjectMapper.Mapper.Map<IEnumerable<CategoryModel>>(category);
        }

        public async Task<PagedList<CategoryModel>> Get(PaginationQuery paginationQuery)
        {
            var category = await _categoryRepository.Get(paginationQuery);
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

        public async Task<CategoryModel> Create(CategoryModel categoryModel)
        {
            var newProduct = await _categoryRepository.GetByIdAsync(categoryModel.CategoryId);
            if (newProduct != null)
                throw new ApplicationException("Category already exits.");

            var mappedEntity = ObjectMapper.Mapper.Map<Category>(categoryModel);
            if (mappedEntity == null)
                throw new ApplicationException("Category could not be mapped.");

            var newEntity = await _categoryRepository.AddAsync(mappedEntity);
            _logger.LogInformationExtension("Category successfully added.");

            return ObjectMapper.Mapper.Map<CategoryModel>(newEntity);
        }
    }
}
