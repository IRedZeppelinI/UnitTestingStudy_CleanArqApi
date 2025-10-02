using MediatR;

namespace ContadorTabaco.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public Decimal Price { get; set; }
}
