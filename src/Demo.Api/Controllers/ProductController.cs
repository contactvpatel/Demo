using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Demo.Api.Attributes;
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

        [HttpGet(Name = "GetAllProducts")]
        //[TypeFilter(typeof(TrackActionPerformance))] //Track Performance of Individual Action
        public async Task<ActionResult<IEnumerable<ProductResponse>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Products");
            var products = await _productService.Get(paginationQuery);
            if (products == null)
            {
                var message = "No products found";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<ProductResponse>(false, message));
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

            return Ok(new Response<IEnumerable<ProductResponse>>(_mapper.Map<IEnumerable<ProductResponse>>(products)));
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductResponse>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Product By Id: {id}");
            var product = await _productService.GetById(id);
            if (product == null)
            {
                var message = $"No product found with id {id}";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<ProductResponse>(false, message));
            }

            return Ok(new Response<ProductResponse>(_mapper.Map<ProductResponse>(product)));
        }

        [HttpGet("categories/{categoryId}", Name = "GetProductByCategoryId")]
        public async Task<ActionResult<ProductResponse>> GetByCategoryId(int categoryId)
        {
            _logger.LogInformationExtension($"Get Product By Category. CategoryId: {categoryId}");
            var products = await _productService.GetByCategoryId(categoryId);
            if (!products.Any())
            {
                var message = $"Product By Category Not Found. CategoryId : {categoryId}";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<ProductResponse>(false, message));
            }

            return Ok(new Response<IEnumerable<ProductResponse>>(_mapper.Map<IEnumerable<ProductResponse>>(products)));
        }

        [HttpPost(Name = "CreateProduct")]
        public async Task<ActionResult<ProductResponse>> Create([FromBody] ProductCreateRequest productCreateRequest)
        {
            _logger.LogInformationExtension($"Create Product - Name: {productCreateRequest.Name}");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);
            productCreateRequest.CreatedBy = userId;

            var newProduct = await _productService.Create(_mapper.Map<ProductModel>(productCreateRequest));

            return Ok(new Response<ProductResponse>(_mapper.Map<ProductResponse>(newProduct)));
        }

        [HttpPut(Name = "UpdateProduct")]
        public async Task<ActionResult<ProductResponse>> Put([FromBody] ProductUpdateRequest productUpdateRequest)
        {
            _logger.LogInformationExtension(
                $"Update Product - Id: {productUpdateRequest.ProductId}, Name: {productUpdateRequest.Name}");

            var productEntity = await _productService.GetById(productUpdateRequest.ProductId);
            if (productEntity == null)
            {
                var message = $"Product with id: {productUpdateRequest.ProductId}, hasn't been found in db.";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<ProductResponse>(false, message));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            productUpdateRequest.LastUpdatedBy = userId;

            var updatedProduct = _productService.Update(_mapper.Map(productUpdateRequest, productEntity));

            return Ok(new Response<ProductResponse>(_mapper.Map<ProductResponse>(updatedProduct)));
        }

        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<ActionResult<ProductResponse>> Delete(int id)
        {
            _logger.LogInformationExtension($"Delete Product - Id: {id}");

            var productEntity = await _productService.GetById(id);
            if (productEntity == null)
            {
                var message = $"Product with id: {id}, hasn't been found in db.";
                _logger.LogErrorExtension(message, null);
                return NotFound(new Response<ProductResponse>(false, message));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            productEntity.LastUpdatedBy = userId;
            productEntity.LastUpdated = DateTime.Now;

            await _productService.Delete(productEntity);

            return Ok(new Response<ProductResponse>(null));
        }
    }
}
