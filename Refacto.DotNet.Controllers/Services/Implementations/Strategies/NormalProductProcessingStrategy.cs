using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Services.Contracts;

namespace Refacto.DotNet.Controllers.Services.Implementations.Strategies
{
    public class NormalProductProcessingStrategy : IProductProcessingStrategy
    {
        private readonly INotificationService _notificationService;

        public string ProductType => "NORMAL";

        public NormalProductProcessingStrategy(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Process(Product product)
        {
            if (product.Available > 0)
            {
                product.Available -= 1;
                return;
            }

            if (product.LeadTime > 0)
            {
                _notificationService.SendDelayNotification(product.LeadTime, product.Name);
            }
        }
    }
}