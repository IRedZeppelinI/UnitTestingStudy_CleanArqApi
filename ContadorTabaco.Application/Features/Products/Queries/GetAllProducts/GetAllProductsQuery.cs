using ContadorTabaco.Application.Features.Products.Dtos;
using MediatR;

namespace ContadorTabaco.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<List<ProductDto>>
{

}
