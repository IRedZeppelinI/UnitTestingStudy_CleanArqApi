using AutoMapper;
using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Application.Interfaces;
using MediatR;

namespace ContadorTabaco.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepo;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepo = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepo.GetByIdAsync(request.Id, cancellationToken);
        if (product == null) return null;

        return _mapper.Map<ProductDto>(product);
    }
}
