using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Services.Contracts;
using Refacto.DotNet.Controllers.Services.Impl;

namespace Refacto.DotNet.Controllers.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderProcessingService _orderProcessingService;
        
         public OrdersController(IOrderProcessingService orderProcessingService)
        {
            _orderProcessingService = orderProcessingService;
        }

        [HttpPost("{orderId}/processOrder")]
        [ProducesResponseType(typeof(ProcessOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProcessOrderResponse>> ProcessOrder(long orderId, CancellationToken cancellationToken)
        {
            Entities.Order? order = await _orderProcessingService.ProcessAsync(orderId, cancellationToken);
            if (order is null)
            {
                return NotFound();
            }

            return Ok(new ProcessOrderResponse(order.Id));
        }
    }
}
