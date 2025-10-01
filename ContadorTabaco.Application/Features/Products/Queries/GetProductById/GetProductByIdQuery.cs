using ContadorTabaco.Application.Features.Products.Dtos;
using MediatR;

namespace ContadorTabaco.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public int Id { get; set; }
    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}
