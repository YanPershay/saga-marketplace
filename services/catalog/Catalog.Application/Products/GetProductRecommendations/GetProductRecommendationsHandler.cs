using Catalog.Application.Abstractions;
using Catalog.Application.Abstractions.AI;
using Catalog.Application.Abstractions.AI.Exceptions;
using Catalog.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Catalog.Application.Products.GetProductRecommendations;

public sealed class GetProductRecommendationsHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IAiRecommendationClient _aiRecommendationClient;
    private readonly ILogger<GetProductRecommendationsHandler> _logger;
    private readonly ProductRecommendationsOptions _options;

    public GetProductRecommendationsHandler(
        IProductRepository productRepository,
        IAiRecommendationClient aiRecommendationClient,
        ILogger<GetProductRecommendationsHandler> logger,
        IOptions<ProductRecommendationsOptions> options)
    {
        _productRepository = productRepository;
        _aiRecommendationClient = aiRecommendationClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<IReadOnlyCollection<ProductRecommendationResult>> HandleAsync(
        GetProductRecommendationsQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting product recommendations for product {ProductId}.",
            query.ProductId);
        
        var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product with id {query.ProductId} not found.");

        var candidates = await _productRepository.GetRecommendationCandidatesAsync(
            product.Id,
            product.Category,
            _options.MaxCandidates,
            cancellationToken);

        if (candidates.Count == 0)
        {
            _logger.LogInformation(
                "No recommendation candidates found for product {ProductId}.",
                query.ProductId);

            return Array.Empty<ProductRecommendationResult>();
        }

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
            
            if (aiRecommendations.Count == 0)
            {
                _logger.LogWarning(
                    "AI returned empty recommendations for product {ProductId}. Using fallback.",
                    query.ProductId);

                return BuildFallback(candidates);
            }
            
            _logger.LogInformation(
                "AI returned {RecommendationCount} recommendations for product {ProductId}.",
                aiRecommendations.Count,
                query.ProductId);

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
        catch (AiServiceTimeoutException ex)
        {
            _logger.LogWarning(ex,
                "AI timeout for product {ProductId}. Using fallback.",
                query.ProductId);

            return BuildFallback(candidates);
        }
        catch (AiServiceBadResponseException ex)
        {
            _logger.LogWarning(ex,
                "AI Bad Response for product {ProductId}. Using fallback.",
                query.ProductId);
            
            return BuildFallback(candidates);
        }
        catch (AiServiceUnavailableException ex)
        {
            _logger.LogWarning(ex,
                "AI Service Unavailable Response for product {ProductId}. Using fallback.",
                query.ProductId);
            
            return BuildFallback(candidates);
        }
    }
    
    private IReadOnlyCollection<ProductRecommendationResult> BuildFallback(
        IReadOnlyCollection<Product> candidates)
    {
        return candidates
            .Take(_options.FallbackCount)
            .Select(candidate => MapToRecommendationResult(candidate, reason: null))
            .ToList();
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