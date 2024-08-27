using Asp.Versioning;
using AutoMapper;
using Demo.Api.Dto;
using Demo.Api.Extensions;
using Demo.Api.Filters;
using Demo.Api.Models;
using Demo.Business.Interfaces;
using Demo.Business.Models;
using Demo.Core.Models;
using Demo.Util.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Demo.Api.Controllers
{
    [Route("/categories")]
    [ApiController]
    [ApiVersion("1")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IHttpContextAccessor httpContextAccessor,
            ILogger<CategoryController> logger, IMapper mapper)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AsmAuthorization(ModuleCode.Category, AccessType.View)]
        public async Task<ActionResult<IEnumerable<CategoryResponseModel>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Categories");
            var categories = await _categoryService.Get(paginationQuery);
            if (categories == null)
            {
                _logger.LogInformationExtension("No categories found");
                return NotFound(new Response<CategoryResponseModel>(null, "No categories found"));
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
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(new Response<IEnumerable<CategoryResponseModel>>(
                _mapper.Map<IEnumerable<CategoryResponseModel>>(categories)));
        }

        [HttpGet("{id:int}")]
        [AsmAuthorization(ModuleCode.Category, AccessType.View)]
        public async Task<ActionResult<CategoryResponseModel>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Category By Id: {id}");
            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                _logger.LogInformationExtension($"No category found with id {id}");
                return NotFound(new Response<CategoryResponseModel>(null, $"No category with id {id}"));
            }

            return Ok(new Response<CategoryResponseModel>(_mapper.Map<CategoryResponseModel>(category)));
        }

        [HttpPost]
        [AsmAuthorization(ModuleCode.Category, AccessType.Create)]
        public async Task<ActionResult<Dto.ProductResponseModel>> Create(
            [FromBody] CategoryRequestModel categoryRequestModel)
        {
            _logger.LogInformationExtension($"Create Category - Name: {categoryRequestModel.Name}");

            var categoryModel = _mapper.Map<CategoryModel>(categoryRequestModel);

            var userId = UserExtensions.GetUserId(_httpContextAccessor);
            categoryModel.CreatedBy = userId;
            categoryModel.Created = DateTime.UtcNow;
            categoryModel.LastUpdatedBy = userId;
            categoryModel.LastUpdated = DateTime.UtcNow;

            var newCategory = await _categoryService.Create(categoryModel);

            return Ok(new Response<CategoryResponseModel>(_mapper.Map<CategoryResponseModel>(newCategory)));
        }
    }
}
