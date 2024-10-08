﻿using Asp.Versioning;
using AutoMapper;
using Demo.Api.Attributes;
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
    [Route("/products")]
    [ApiController]
    [ApiVersion("1")]
    [TypeFilter(typeof(TrackActionPerformance))] //Track Performance of entire controller's action
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IHttpContextAccessor httpContextAccessor,
            ILogger<ProductController> logger, IMapper mapper)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        //[TypeFilter(typeof(TrackActionPerformance))] //Track Performance of Individual Action
        [AsmAuthorization(ModuleCode.Product, AccessType.View)]
        public async Task<ActionResult<IEnumerable<ProductResponseModel>>> Get(
            [FromQuery] PaginationQuery paginationQuery)
        {
            _logger.LogInformationExtension("Get Products");
            var products = await _productService.Get(paginationQuery);
            if (products == null)
            {
                _logger.LogInformationExtension("No products found");
                return NotFound(new Response<ProductResponseModel>(null, "No products found"));
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
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(new Response<IEnumerable<ProductResponseModel>>(
                _mapper.Map<IEnumerable<ProductResponseModel>>(products)));
        }

        [HttpGet("{id:int}")]
        [AsmAuthorization(ModuleCode.Product, AccessType.View)]
        public async Task<ActionResult<ProductResponseModel>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Product By Id: {id}");
            var product = await _productService.GetById(id);
            if (product != null)
                return Ok(new Response<ProductResponseModel>(_mapper.Map<ProductResponseModel>(product)));

            _logger.LogInformationExtension($"No product found with id {id}");
            return NotFound(new Response<ProductResponseModel>(null, $"No product found with id {id}"));
        }

        [HttpGet("categories/{categoryId:int}")]
        [AsmAuthorization(ModuleCode.Product, AccessType.View)]
        public async Task<ActionResult<ProductResponseModel>> GetByCategoryId(int categoryId)
        {
            _logger.LogInformationExtension($"Get Product By Category. CategoryId: {categoryId}");
            var products = await _productService.GetByCategoryId(categoryId);
            if (products?.Count() > 0)
                return Ok(new Response<IEnumerable<ProductResponseModel>>(
                    _mapper.Map<IEnumerable<ProductResponseModel>>(products)));

            _logger.LogInformationExtension($"Product By Category Not Found. CategoryId : {categoryId}");
            return NotFound(new Response<ProductResponseModel>(null,
                $"Product By Category Not Found. CategoryId : {categoryId}"));
        }

        [HttpPost]
        [AsmAuthorization(ModuleCode.Product, AccessType.Create)]
        public async Task<ActionResult<ProductResponseModel>> Create([FromBody] ProductRequestModel productRequestModel)
        {
            _logger.LogInformationExtension($"Create Product - Name: {productRequestModel.Name}");

            var productModel = _mapper.Map<ProductModel>(productRequestModel);

            var userId = UserExtensions.GetUserId(_httpContextAccessor);

            productModel.CreatedBy = userId;
            productModel.Created = DateTime.UtcNow;
            productModel.LastUpdatedBy = userId;
            productModel.LastUpdated = DateTime.UtcNow;

            var newProduct = await _productService.Create(productModel);

            return Ok(new Response<ProductResponseModel>(_mapper.Map<ProductResponseModel>(newProduct)));
        }

        [HttpPut]
        [AsmAuthorization(ModuleCode.Product, AccessType.Update)]
        public async Task<ActionResult<ProductResponseModel>> Update([FromBody] ProductRequestModel productRequestModel)
        {
            _logger.LogInformationExtension(
                $"Update Product - Id: {productRequestModel.ProductId}, Name: {productRequestModel.Name}");

            var productModel = await _productService.GetById(productRequestModel.ProductId);
            if (productModel == null)
            {
                _logger.LogErrorExtension($"Product with id: {productRequestModel.ProductId}, hasn't been found in db.",
                    null);
                return NotFound(new Response<ProductResponseModel>(null,
                    $"Product with id: {productRequestModel.ProductId}, hasn't been found in db."));
            }

            var userId = UserExtensions.GetUserId(_httpContextAccessor);

            _mapper.Map(productRequestModel, productModel);

            productModel.LastUpdatedBy = userId;
            productModel.LastUpdated = DateTime.UtcNow;

            await _productService.Update(productModel);

            return Ok(new Response<ProductResponseModel>(_mapper.Map<ProductResponseModel>(productModel)));
        }

        [HttpDelete("{id:int}")]
        [AsmAuthorization(ModuleCode.Product, AccessType.Delete)]
        public async Task<ActionResult<ProductResponseModel>> Delete(int id)
        {
            _logger.LogInformationExtension($"Delete Product - Id: {id}");

            var productEntity = await _productService.GetById(id);
            if (productEntity == null)
            {
                _logger.LogErrorExtension($"Product with id: {id}, hasn't been found in db.", null);
                return NotFound(new Response<ProductResponseModel>(null,
                    $"Product with id: {id}, hasn't been found in db."));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            productEntity.LastUpdatedBy = userId;
            productEntity.LastUpdated = DateTime.Now;

            await _productService.Delete(productEntity);

            return Ok(new Response<ProductResponseModel>(null, $"Product Id ({id}) is deleted from db."));
        }
    }
}
