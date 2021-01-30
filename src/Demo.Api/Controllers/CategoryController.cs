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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Categories");
            var categories = await _categoryService.Get(paginationQuery);
            if (categories == null)
            {
                _logger.LogErrorExtension("No categories found", null);
                return NotFound(new Response<ProductResponse>(null, false, "No categories found"));
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Category By Id: {id}");
            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                _logger.LogErrorExtension($"No category found with id {id}", null);
                return NotFound(new Response<ProductResponse>(null, false, $"No category with id {id}"));
            }

            return Ok(new Response<CategoryResponse>(_mapper.Map<CategoryResponse>(category)));
        }

        [HttpPost]
        public async Task<ActionResult<ProductApiModel>> Post([FromBody] CategoryApiModel categoryApiModel)
        {
            _logger.LogInformationExtension($"Post Category - Name: {categoryApiModel.Name}");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);
            categoryApiModel.CreatedBy = userId;
            categoryApiModel.Created = DateTime.Now;
            categoryApiModel.LastUpdatedBy = userId;
            categoryApiModel.LastUpdated = DateTime.Now;

            var newCategory = await _categoryService.Create(_mapper.Map<CategoryModel>(categoryApiModel));

            return Ok(new Response<CategoryApiModel>(_mapper.Map<CategoryApiModel>(newCategory)));
        }
    }
}
