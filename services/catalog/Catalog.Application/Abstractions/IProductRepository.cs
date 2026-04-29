using Catalog.Domain;

namespace Catalog.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Product>> GetRecommendationCandidatesAsync(
        Guid productId, string category, int maxCandidates, CancellationToken cancellationToken = default);
}