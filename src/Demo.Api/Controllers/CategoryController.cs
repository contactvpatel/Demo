using AutoMapper;
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
        //[AsmAuthorization(ModuleCode.Category, AccessType.View)]
        public async Task<ActionResult<IEnumerable<CategoryApiModel>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Categories");
            var categories = await _categoryService.Get(paginationQuery);
            if (categories == null)
            {
                _logger.LogInformationExtension("No categories found");
                return NotFound(new Response<ProductApiModel>(null, false, "No categories found"));
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

            return Ok(new Response<IEnumerable<CategoryApiModel>>(
                _mapper.Map<IEnumerable<CategoryApiModel>>(categories)));
        }

        [HttpGet("{id:int}")]
        //[AsmAuthorization(ModuleCode.Category, AccessType.View)]
        public async Task<ActionResult<CategoryApiModel>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Category By Id: {id}");
            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                _logger.LogInformationExtension($"No category found with id {id}");
                return NotFound(new Response<ProductApiModel>(null, false, $"No category with id {id}"));
            }

            return Ok(new Response<CategoryApiModel>(_mapper.Map<CategoryApiModel>(category)));
        }

        [HttpPost]
        //[AsmAuthorization(ModuleCode.Category, AccessType.Create)]
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
