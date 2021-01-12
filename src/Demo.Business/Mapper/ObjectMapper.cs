using System;
using System.Linq;
using AutoMapper;
using Demo.Business.Models;
using Demo.Core.Entities;
using Demo.Core.Models;

namespace Demo.Business.Mapper
{
    // The best implementation of AutoMapper for class libraries -> https://www.abhith.net/blog/using-automapper-in-a-net-core-class-library/
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> Lazy = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsAssembly);
                cfg.AddProfile<DemoDtoMapper>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class DemoDtoMapper : Profile
    {
        public DemoDtoMapper()
        {
            CreateMap<Product, ProductModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        }
    }

    public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination,
            ResolutionContext context)
        {
            destination ??= new PagedList<TDestination>();
            destination.AddRange(source.Select(item => context.Mapper.Map<TSource, TDestination>(item)));
            destination.CurrentPage = source.CurrentPage;
            destination.PageSize = source.PageSize;
            destination.TotalPages = source.TotalPages;
            destination.TotalCount = source.TotalCount;
            destination.HasPreviousPage = source.HasPreviousPage;
            destination.HasNextPage = source.HasNextPage;
            return destination;
        }
    }
}
