using AutoMapper;
using Demo.Api.Attributes;
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
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    [ApiVersion("1.0")]
    [TypeFilter(typeof(TrackActionPerformance))] //Track Performance of entire controller's action
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        //[TypeFilter(typeof(TrackActionPerformance))] //Track Performance of Individual Action
        //[AsmAuthorization(ModuleCode.Product, AccessType.View)]
        public async Task<ActionResult<IEnumerable<ProductApiModel>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Products");
            var products = await _productService.Get(paginationQuery);
            if (products == null)
            {
                _logger.LogErrorExtension("No products found", null);
                return NotFound(new Response<ProductApiModel>(null, false, "No products found"));
            }

            _logger.LogInformationExtension($"Found {products.Count} products");

            var paginationMetadata = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages,
                products.HasPreviousPage,
                products.HasNextPage
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(new Response<IEnumerable<ProductApiModel>>(_mapper.Map<IEnumerable<ProductApiModel>>(products)));
        }

        [HttpGet("{id:int}")]
        //[AsmAuthorization(ModuleCode.Product, AccessType.View)]
        public async Task<ActionResult<ProductApiModel>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Product By Id: {id}");
            var product = await _productService.GetById(id);
            if (product == null)
            {
                _logger.LogInformationExtension($"No product found with id {id}");
                return NotFound(new Response<ProductApiModel>(null, false, $"No product found with id {id}"));
            }

            return Ok(new Response<ProductApiModel>(_mapper.Map<ProductApiModel>(product)));
        }

        [HttpGet("categories/{categoryId:int}")]
        //[AsmAuthorization(ModuleCode.Product, AccessType.View)]
        public async Task<ActionResult<ProductApiModel>> GetByCategoryId(int categoryId)
        {
            _logger.LogInformationExtension($"Get Product By Category. CategoryId: {categoryId}");
            var products = await _productService.GetByCategoryId(categoryId);
            if (!products.Any())
            {
                _logger.LogInformationExtension($"Product By Category Not Found. CategoryId : {categoryId}");
                return NotFound(new Response<ProductApiModel>(null, false,
                    $"Product By Category Not Found. CategoryId : {categoryId}"));
            }

            return Ok(new Response<IEnumerable<ProductApiModel>>(_mapper.Map<IEnumerable<ProductApiModel>>(products)));
        }

        [HttpPost]
        //[AsmAuthorization(ModuleCode.Product, AccessType.Create)]
        public async Task<ActionResult<ProductApiModel>> Post([FromBody] ProductApiModel productApiModel)
        {
            _logger.LogInformationExtension($"Post Product - Name: {productApiModel.Name}");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);
            productApiModel.CreatedBy = userId;
            productApiModel.Created = DateTime.Now;
            productApiModel.LastUpdatedBy = userId;
            productApiModel.LastUpdated = DateTime.Now;

            var newProduct = await _productService.Create(_mapper.Map<ProductModel>(productApiModel));

            return Ok(new Response<ProductApiModel>(_mapper.Map<ProductApiModel>(newProduct)));
        }

        [HttpPut("{id:int}")]
        //[AsmAuthorization(ModuleCode.Product, AccessType.Update)]
        public async Task<ActionResult<ProductApiModel>> Put(int id, [FromBody] ProductApiModel productApiModel)
        {
            _logger.LogInformationExtension(
                $"Put Product - Id: {productApiModel.ProductId}, Name: {productApiModel.Name}");

            if (!ModelState.IsValid)
            {
                _logger.LogErrorExtension("Invalid product object sent from client.", null);
                return BadRequest(new Response<ProductApiModel>(null, false, "Invalid model object"));
            }

            var productEntity = await _productService.GetById(id);
            if (productEntity == null)
            {
                _logger.LogErrorExtension($"Product with id: {id}, hasn't been found in db.", null);
                return NotFound(new Response<ProductApiModel>(null, false,
                    $"Product with id: {id}, hasn't been found in db."));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            productApiModel.LastUpdatedBy = userId;
            productApiModel.LastUpdated = DateTime.Now;

            _mapper.Map(productApiModel, productEntity);

            await _productService.Update(productEntity);

            return Ok(new Response<ProductApiModel>(null));
        }

        [HttpDelete("{id:int}")]
        //[AsmAuthorization(ModuleCode.Product, AccessType.Delete)]
        public async Task<ActionResult<ProductApiModel>> Delete(int id)
        {
            _logger.LogInformationExtension($"Delete Product - Id: {id}");

            var productEntity = await _productService.GetById(id);
            if (productEntity == null)
            {
                _logger.LogErrorExtension($"Product with id: {id}, hasn't been found in db.", null);
                return NotFound(new Response<ProductApiModel>(null, false,
                    $"Product with id: {id}, hasn't been found in db."));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            productEntity.LastUpdatedBy = userId;
            productEntity.LastUpdated = DateTime.Now;

            await _productService.Delete(productEntity);

            return Ok(new Response<ProductApiModel>(null));
        }
    }
}
