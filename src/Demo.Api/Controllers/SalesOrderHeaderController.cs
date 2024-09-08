using Asp.Versioning;
using AutoMapper;
using Demo.Api.Filters;
using Demo.Business.Interfaces;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [Route("api/v{version:apiVersion}/salesorderheaders")]
    [ApiController]
    [ApiVersion("1")]
    public class SalesOrderHeaderController : Controller
    {
        private readonly ISalesOrderHeaderService _salesOrderHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SalesOrderHeaderController> _logger;
        private readonly IMapper _mapper;

        public SalesOrderHeaderController(ISalesOrderHeaderService salesOrderHeaderService, IHttpContextAccessor httpContextAccessor, ILogger<SalesOrderHeaderController> logger, IMapper mapper)
        {
            _salesOrderHeaderService = salesOrderHeaderService ?? throw new ArgumentNullException(nameof(salesOrderHeaderService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AsmAuthorization(ModuleCode.SalesOrderHeader, AccessType.View)]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _salesOrderHeaderService.Get(queryParam);
            return Ok(response);
        }
    }
}
