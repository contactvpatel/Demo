using Asp.Versioning;
using AutoMapper;
using Demo.Api.Attributes;
using Demo.Business.Interfaces;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [Route("api/v{version:apiVersion}/products")]
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
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _productService.Get(queryParam);
            return Ok(response);
        }
    }
}
