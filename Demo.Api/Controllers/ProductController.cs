using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Demo.Api.Models;
using Demo.Application.Interfaces;
using Demo.Application.Models;
using Demo.Common.Attributes;
using Demo.Common.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<ActionResult<IEnumerable<ProductApiModel>>> GetAll()
        {
            _logger.LogInformationExtension("Get Products");
            var products = await _productService.GetAll();
            if (products == null)
            {
                _logger.LogWarningExtension("Products does not exist");
                return NotFound();
            }

            _logger.LogInformationExtension($"Found {products.Count()} products");
            return Ok(_mapper.Map<IEnumerable<ProductApiModel>>(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductApiModel>> GetById(int productId)
        {
            _logger.LogInformationExtension($"Get Product By Id: {productId}");
            var product = await _productService.GetById(productId);
            if (product == null)
            {
                _logger.LogInformationExtension("Unable to locate product in the database");
                return NotFound();
            }

            return Ok(_mapper.Map<ProductApiModel>(product));
        }

        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<ProductApiModel>> GetByName(string name)
        {
            _logger.LogInformationExtension($"Get Product By Name: {name}");
            var product = await _productService.GetByName(name);
            if (product == null)
            {
                _logger.LogInformationExtension("Unable to locate product in the database");
                return NotFound();
            }

            return Ok(_mapper.Map<ProductApiModel>(product));
        }

        [HttpGet("categories/{categoryId}")]
        public async Task<ActionResult<ProductApiModel>> GetByCategoryId(int categoryId)
        {
            _logger.LogInformationExtension($"Get Product By Category. CategoryId: {categoryId}");
            var products = await _productService.GetByCategoryId(categoryId);
            if (products == null)
            {
                _logger.LogInformationExtension(
                    $"Product By Category Not Found. CategoryId : {categoryId}");
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<ProductApiModel>>(products));
        }

        [HttpPost]
        public async Task<ActionResult<ProductApiModel>> Post([FromBody] ProductApiModel productApiModel)
        {
            _logger.LogInformationExtension($"Post Product - Name: {productApiModel.Name}");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);
            productApiModel.CreatedBy = userId;
            productApiModel.Created = DateTime.Now;
            productApiModel.LastUpdatedBy = userId;
            productApiModel.LastUpdated = DateTime.Now;

            var product = _mapper.Map<ProductModel>(productApiModel);

            await _productService.Create(product);

            return CreatedAtRoute("GetById", new {productId = productApiModel.ProductId}, product);
        }

        [HttpPut]
        public async Task<ActionResult<ProductApiModel>> Put([FromBody] ProductApiModel productApiModel)
        {
            _logger.LogInformationExtension(
                $"Put Product - Id: {productApiModel.ProductId}, Name: {productApiModel.Name}");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            productApiModel.LastUpdatedBy = userId;
            productApiModel.LastUpdated = DateTime.Now;

            var product = _mapper.Map<ProductModel>(productApiModel);

            await _productService.Update(product);

            return Ok(productApiModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductApiModel>> Delete(int id)
        {
            _logger.LogInformationExtension($"Delete Product - Id: {id}");
            var productModel = new ProductModel {ProductId = id};
            await _productService.Delete(productModel);
            return Ok();
        }
    }
}
