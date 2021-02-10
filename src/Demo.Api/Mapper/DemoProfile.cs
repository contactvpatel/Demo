using AutoMapper;
using Demo.Api.Dto;
using Demo.Business.Models;

namespace Demo.Api.Mapper
{
    public class DemoProfile : Profile
    {
        public DemoProfile()
        {
            CreateMap<CategoryModel, CategoryCreateRequest>().ReverseMap();
            CreateMap<CategoryModel, CategoryResponse>().ReverseMap();
            CreateMap<CategoryModel, CategoryUpdateRequest>().ReverseMap();
            CreateMap<ProductModel, ProductCreateRequest>().ReverseMap();
            CreateMap<ProductModel, ProductResponse>().ReverseMap();
            CreateMap<ProductModel, ProductUpdateRequest>().ReverseMap();
        }
    }
}
