using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Services.Contracts;

namespace Refacto.DotNet.Controllers.Services.Implementations.Strategies
{
    public class ExpirableProductProcessingStrategy : IProductProcessingStrategy
    {
        private readonly INotificationService _notificationService;

        public string ProductType => "EXPIRABLE";

        public ExpirableProductProcessingStrategy(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Process(Product product)
        {
            DateTime today = DateTime.UtcNow.Date;
            bool isExpired = product.ExpiryDate.HasValue && product.ExpiryDate.Value.Date < today;

            if (isExpired)
            {
                _notificationService.SendExpirationNotification(product.Name, product.ExpiryDate.Value);
                product.Available = 0;
                return;
            }

            if (product.Available > 0)
            {
                product.Available -= 1;
            }
        }
    }
}