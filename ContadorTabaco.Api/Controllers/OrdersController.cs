using ContadorTabaco.Application.Features.Orders.Dtos;
using ContadorTabaco.Application.Features.Orders.Queries.GetAllOrders;
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

}
