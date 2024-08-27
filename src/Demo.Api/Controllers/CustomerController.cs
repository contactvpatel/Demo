using Asp.Versioning;
using AutoMapper;
using Demo.Api.Filters;
using Demo.Business.Interfaces;
using Demo.Util.FIQL;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [Route("/customers")]
    [ApiController]
    [ApiVersion("1")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomerController> _logger;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService, IHttpContextAccessor httpContextAccessor,
            ILogger<CustomerController> logger, IMapper mapper)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AsmAuthorization(ModuleCode.Customer, AccessType.View)]
        public async Task<ActionResult<dynamic>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _customerService.Get(queryParam);
            return Ok(response);
        }
    }
}
