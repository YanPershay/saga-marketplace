using Catalog.Application.Abstractions;
using Catalog.Application.Abstractions.AI;
using Catalog.Domain;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.GetProductRecommendations;

public sealed class GetProductRecommendationsHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IAiRecommendationClient _aiRecommendationClient;
    private readonly ILogger<GetProductRecommendationsHandler> _logger;

    private const int MaxCandidates = 20;
    private const int FallbackCount = 3;

    public GetProductRecommendationsHandler(
        IProductRepository productRepository,
        IAiRecommendationClient aiRecommendationClient,
        ILogger<GetProductRecommendationsHandler> logger)
    {
        _productRepository = productRepository;
        _aiRecommendationClient = aiRecommendationClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<ProductRecommendationResult>> HandleAsync(
        GetProductRecommendationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product with id {query.ProductId} not found.");

        var candidates = await _productRepository.GetRecommendationCandidatesAsync(
            product.Id,
            product.Category,
            MaxCandidates,
            cancellationToken);

        if (candidates.Count == 0)
            return Array.Empty<ProductRecommendationResult>();

        var currentAiProduct = MapToAiProduct(product);
        var candidateAiProducts = candidates
            .Select(MapToAiProduct)
            .ToList();

        try
        {
            var aiRecommendations = await _aiRecommendationClient.GetRecommendationsAsync(
                currentAiProduct,
                candidateAiProducts,
                cancellationToken);

            var candidatesById = candidates.ToDictionary(candidate => candidate.Id);

            return aiRecommendations
                .Where(recommendation => candidatesById.ContainsKey(recommendation.ProductId))
                .Select(recommendation =>
                {
                    var recommendedProduct = candidatesById[recommendation.ProductId];

                    return MapToRecommendationResult(
                        recommendedProduct,
                        recommendation.Reason);
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "AI recommendations failed for product {ProductId}. Returning fallback.",
                query.ProductId);

            return candidates
                .Take(FallbackCount)
                .Select(candidate => MapToRecommendationResult(candidate, reason: null))
                .ToList();
        }
    }

    private static AiProduct MapToAiProduct(Product product)
    {
        return new AiProduct(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Category);
    }

    private static ProductRecommendationResult MapToRecommendationResult(Product product, string? reason)
    {
        return new ProductRecommendationResult(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.QuantityAvailable,
            product.CreatedAt,
            reason);
    }
}