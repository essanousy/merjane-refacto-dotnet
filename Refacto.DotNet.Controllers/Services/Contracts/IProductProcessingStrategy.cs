using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Services.Contracts
{
    public interface IProductProcessingStrategy
    {
        string ProductType { get; }
        void Process(Product product);
    }
}
