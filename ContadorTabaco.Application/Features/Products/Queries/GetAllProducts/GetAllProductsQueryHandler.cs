using AutoMapper;
using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using MediatR;

namespace ContadorTabaco.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return _mapper.Map<List<ProductDto>>(products);        
    }
}
