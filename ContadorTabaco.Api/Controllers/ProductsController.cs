using ContadorTabaco.Application.Features.Products.Dtos;
using ContadorTabaco.Application.Features.Products.Queries.GetAllProducts;
using ContadorTabaco.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ContadorTabaco.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : Controller
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery();
        var products = await _mediator.Send(query, cancellationToken);

        return Ok(products);
    }

    [HttpGet(template: "{id}", Name ="GetProductById")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query,cancellationToken);

        return result is not null ? Ok(result) : NotFound();
    }

}
