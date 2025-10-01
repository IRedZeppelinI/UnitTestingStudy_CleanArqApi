using ContadorTabaco.Application.Features.Orders.Dtos;
using MediatR;

namespace ContadorTabaco.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public int Id { get; set; }

    public GetOrderByIdQuery(int id )
    {
        Id = id;
    }
}
