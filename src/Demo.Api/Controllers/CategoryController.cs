using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Demo.Api.Models;
using Demo.Business.Interfaces;
using Demo.Util.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<ActionResult<IEnumerable<CategoryApiModel>>> GetAll()
        {
            _logger.LogInformationExtension("Get Categories");
            return Ok(_mapper.Map<IEnumerable<CategoryApiModel>>(await _categoryService.GetAll()));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryApiModel>> GetById(int id)
        {
            _logger.LogInformationExtension($"Get Category By Id: {id}");
            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                _logger.LogInformationExtension($"Category Not Found. CategoryId : {id}");
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryApiModel>(category));
        }
    }
}
