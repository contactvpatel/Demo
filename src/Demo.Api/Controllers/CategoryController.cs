using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Demo.Api.Dto;
using Demo.Business.Interfaces;
using Demo.Business.Models;
using Demo.Core.Models;
using Demo.Util.Logging;
using Demo.Util.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.Api.Controllers
{
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IMapper mapper)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetAllCategories")]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Categories");
            var categories = await _categoryService.Get(paginationQuery);
            if (categories == null)
            {
                var message = "No categories found";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<CategoryResponse>(false, message));
            }

            _logger.LogInformationExtension($"Found {categories.Count} categories");

            var paginationMetadata = new
            {
                categories.TotalCount,
                categories.PageSize,
                categories.CurrentPage,
                categories.TotalPages,
                categories.HasPreviousPage,
                categories.HasNextPage
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(new Response<IEnumerable<CategoryResponse>>(
                _mapper.Map<IEnumerable<CategoryResponse>>(categories)));
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Category By Id: {id}");
            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                var message = $"No category found with id {id}";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<CategoryResponse>(false, message));
            }

            return Ok(new Response<CategoryResponse>(_mapper.Map<CategoryResponse>(category)));
        }

        [HttpPost(Name = "CreateCategory")]
        public async Task<ActionResult<CategoryResponse>> Create([FromBody] CategoryCreateRequest categoryCreateRequest)
        {
            _logger.LogInformationExtension($"Create Category - Name: {categoryCreateRequest.Name}");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);
            categoryCreateRequest.CreatedBy = userId;

            var newCategory = await _categoryService.Create(_mapper.Map<CategoryModel>(categoryCreateRequest));

            return Ok(new Response<CategoryResponse>(_mapper.Map<CategoryResponse>(newCategory)));
        }

        [HttpPut(Name = "UpdateCategory")]
        public async Task<ActionResult<CategoryResponse>> Update([FromBody] CategoryUpdateRequest categoryUpdateRequest)
        {
            _logger.LogInformationExtension(
                $"Update Category - Id: {categoryUpdateRequest.CategoryId}, Name: {categoryUpdateRequest.Name}");

            var categoryEntity = await _categoryService.GetById(categoryUpdateRequest.CategoryId);
            if (categoryEntity == null)
            {
                var message = $"Category with id: {categoryUpdateRequest.CategoryId}, hasn't been found in db.";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<CategoryResponse>(false, message));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            categoryUpdateRequest.LastUpdatedBy = userId;

            var updatedCategory = _categoryService.Update(_mapper.Map(categoryUpdateRequest, categoryEntity));

            return Ok(new Response<CategoryResponse>(_mapper.Map<CategoryResponse>(updatedCategory)));
        }
    }
}
