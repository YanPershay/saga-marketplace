using BuildingBlocks.Observability;
using Microsoft.AspNetCore.Mvc;
using Order.API.Contracts.Requests;
using Order.API.Contracts.Responses;
using Order.Application.Orders.CreateOrder;
using Order.Application.Orders.GetOrderById;

namespace Order.API.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrderHandler;
    private readonly GetOrderByIdHandler _getOrderByIdHandler;

    public OrdersController(
        CreateOrderHandler createOrderHandler,
        GetOrderByIdHandler getOrderByIdHandler)
    {
        _createOrderHandler = createOrderHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.GetCorrelationId();
        
        var parsedCorrelationId =
            Guid.TryParse(correlationId, out var value)
                ? value
                : Guid.NewGuid();
        
        var command = new CreateOrderCommand(
            parsedCorrelationId,
            request.CustomerId,
            request.Items.Select(i => new CreateOrderItem(
                i.ProductId,
                i.Quantity,
                i.Price)).ToList());

        var orderId = await _createOrderHandler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = orderId },
            new CreateOrderResponse(orderId, "Pending"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailsResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getOrderByIdHandler.HandleAsync(
            new GetOrderByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(new OrderDetailsResponse(
            result.Id,
            result.CustomerId,
            result.Status,
            result.TotalPrice,
            result.CreatedAt,
            result.Items.Select(i => new OrderItemResponse(
                i.ProductId,
                i.Quantity,
                i.Price)).ToList()));
    }
}