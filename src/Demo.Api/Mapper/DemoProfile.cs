using AutoMapper;
using Demo.Api.Dto;
using Demo.Business.Models;

namespace Demo.Api.Mapper
{
    public class DemoProfile : Profile
    {
        public DemoProfile()
        {
            CreateMap<CategoryModel, CategoryResponse>().ReverseMap();
            CreateMap<CategoryModel, CreateCategoryResponse>().ReverseMap();
            CreateMap<CategoryModel, UpdateCategoryResponse>().ReverseMap();
            CreateMap<ProductModel, ProductResponse>().ReverseMap();
            CreateMap<ProductModel, CreateProductRequest>().ReverseMap();
            CreateMap<ProductModel, UpdateProductRequest>().ReverseMap();
        }
    }
}
