using AutoMapper;
using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Application.Interfaces;
using MediatR;

namespace ContadorTabaco.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return _mapper.Map<List<OrderDto>>(orders);
        
    }
}
