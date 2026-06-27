using Refacto.DotNet.Controllers.Database.Repositories;
using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Services.Contracts;

namespace Refacto.DotNet.Controllers.Services.Impl
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReadOnlyDictionary<string, IProductProcessingStrategy> _strategies;

        public OrderProcessingService(IOrderRepository orderRepository,
            IEnumerable<IProductProcessingStrategy> strategies)
        {
            _orderRepository = orderRepository;
            _strategies = strategies.ToDictionary(
              s => s.ProductType,
              s => s,
              StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Order?> ProcessAsync(long orderId, CancellationToken cancellationToken = default)
        {
            Order? order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken);

            if (order is null)
            {
                return null;
            }

            foreach (Product product in order.Items ?? [])
            {
                if (product.Type is not null &&
                    _strategies.TryGetValue(product.Type, out IProductProcessingStrategy? strategy))
                {
                    strategy.Process(product);
                }
            }

            await _orderRepository.SaveChangesAsync(cancellationToken);
            return order;
        }
    }
}