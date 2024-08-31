using Asp.Versioning;
using AutoMapper;
using Demo.Api.Filters;
using Demo.Business.Interfaces;
using Demo.Business.Models;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [Route("/addresses")]
    [ApiController]
    [ApiVersion("1")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AddressController> _logger;
        private readonly IMapper _mapper;

        public AddressController(IAddressService addressService, IHttpContextAccessor httpContextAccessor,
            ILogger<AddressController> logger, IMapper mapper)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AsmAuthorization(ModuleCode.Address, AccessType.View)]
        public async Task<ActionResult<HttpResponseModel>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _addressService.Get(queryParam);
            return Ok(response);
        }
    }
}
