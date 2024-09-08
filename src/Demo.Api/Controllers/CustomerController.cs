using Asp.Versioning;
using AutoMapper;
using Demo.Api.Models;
using Demo.Api.Dto;
using Demo.Api.Extensions;
using Demo.Api.Filters;
using Demo.Business.Interfaces;
using Demo.Util.FIQL;
using Demo.Util.Logging;
using Demo.Util.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [Route("api/v{version:apiVersion}/customers")]
    [ApiController]
    [ApiVersion("1")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomerController> _logger;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService, IHttpContextAccessor httpContextAccessor, ILogger<CustomerController> logger, IMapper mapper)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AsmAuthorization(ModuleCode.Customer, AccessType.View)]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _customerService.Get(queryParam);
            return Ok(response);
        }

        [HttpPost]
        [AsmAuthorization(ModuleCode.Customer, AccessType.Create)]
        public async Task<ActionResult<CustomerResponseModel>> Create([FromBody] CustomerRequestModel customerRequestModel)
        {
            _logger.LogInformationExtension($"Create Customer - Name: {customerRequestModel.FirstName} {customerRequestModel.LastName}");

            var customerModel = _mapper.Map<Business.Models.CustomerModel>(customerRequestModel);

            var userId = UserExtensions.GetUserId(_httpContextAccessor);

            customerModel.CreatedBy = userId;
            customerModel.Created = DateTime.UtcNow;
            customerModel.LastUpdatedBy = userId;
            customerModel.LastUpdated = DateTime.UtcNow;

            var newCustomer = await _customerService.Create(customerModel);

            return base.Ok(new Response<CustomerResponseModel>(_mapper.Map<CustomerResponseModel>(newCustomer)));
        }

        [HttpPut]
        [AsmAuthorization(ModuleCode.Customer, AccessType.Update)]
        public async Task<ActionResult<CustomerResponseModel>> Update([FromBody] CustomerRequestModel customerRequestModel)
        {
            _logger.LogInformationExtension($"Update Customer - Id: {customerRequestModel.CustomerId}, Name: {customerRequestModel.FirstName} {customerRequestModel.LastName}");

            var customerModel = await _customerService.GetById(customerRequestModel.CustomerId);
            if (customerModel == null)
            {
                _logger.LogErrorExtension($"Customer with id: {customerRequestModel.CustomerId}, hasn't been found in db.", null);
                return base.NotFound(new Response<CustomerResponseModel>(null, $"Customer with id: {customerRequestModel.CustomerId}, hasn't been found in db."));
            }

            var userId = UserExtensions.GetUserId(_httpContextAccessor);

            _mapper.Map(customerRequestModel, customerModel);

            customerModel.LastUpdatedBy = userId;
            customerModel.LastUpdated = DateTime.UtcNow;

            await _customerService.Update(customerModel);

            return base.Ok(new Response<CustomerResponseModel>(_mapper.Map<CustomerResponseModel>(customerModel)));
        }

        [HttpDelete("{id:int}")]
        [AsmAuthorization(ModuleCode.Customer, AccessType.Delete)]
        public async Task<ActionResult<CustomerResponseModel>> Delete(int id)
        {
            _logger.LogInformationExtension($"Delete Customer - Id: {id}");

            var customerEntity = await _customerService.GetById(id);
            if (customerEntity == null)
            {
                _logger.LogErrorExtension($"Customer with id: {id}, hasn't been found in db.", null);
                return base.NotFound(new Response<CustomerResponseModel>(null, $"Customer with id: {id}, hasn't been found in db."));
            }

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value);

            customerEntity.LastUpdatedBy = userId;
            customerEntity.LastUpdated = DateTime.Now;

            await _customerService.Delete(customerEntity);

            return base.Ok(new Response<CustomerResponseModel>(null, $"Customer Id ({id}) is deleted from db."));
        }
    }
}
