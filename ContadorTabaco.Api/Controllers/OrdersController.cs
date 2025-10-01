using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Application.Features.Orders.Queries.GetAllOrders;
using ContadorTabaco.Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContadorTabaco.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : Controller
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllOrdersQuery();
        var orders = await _mediator.Send(query, cancellationToken);

        return Ok(orders);        
    }

    [HttpGet("{id}", Name = "GetOrderById")]
    public async Task<ActionResult<List<OrderDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result is not null ? Ok(result) : NotFound();
    }


}
