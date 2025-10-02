using AutoMapper;
using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using MediatR;

namespace ContadorTabaco.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _productRepo = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var newProduct = _mapper.Map<Product>(request);
        await _productRepo.AddAsync(newProduct, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);      

        return newProduct.Id;
    }
}
