using AutoMapper;
using Demo.Api.Dto;
using Demo.Business.Models;

namespace Demo.Api.Mapper
{
    public class DemoProfile : Profile
    {
        public DemoProfile()
        {
            CreateMap<CustomerRequestModel, CustomerModel>();
            CreateMap<CustomerModel, CustomerResponseModel>();
        }
    }
}
