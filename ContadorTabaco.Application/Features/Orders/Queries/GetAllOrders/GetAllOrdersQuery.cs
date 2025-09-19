using ContadorTabaco.Application.Features.Orders.Dtos;
using MediatR;

namespace ContadorTabaco.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<List<OrderDto>>
{

}
