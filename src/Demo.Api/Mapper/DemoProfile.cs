using AutoMapper;
using Demo.Api.Models;
using Demo.Business.Models;

namespace Demo.Api.Mapper
{
    public class DemoProfile : Profile
    {
        public DemoProfile()
        {
            CreateMap<ProductModel, ProductApiModel>().ReverseMap();
            CreateMap<CategoryModel, CategoryApiModel>().ReverseMap();
        }
    }
}
