using AutoMapper;
using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Application.Features.Products.Commands.CreateProduct;
using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Domain.Entities;

namespace ContadorTabaco.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        CreateMap<CreateProductCommand, Product>();
    }
}
