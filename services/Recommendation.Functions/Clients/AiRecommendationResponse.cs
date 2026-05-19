using Newtonsoft.Json;

namespace Recommendation.Functions.Clients;

public sealed record AiRecommendationResponse(
    [property: JsonProperty("recommendations")]
    IReadOnlyCollection<AiRecommendedProduct> Recommendations,

    [property: JsonProperty("provider")]
    string Provider,

    [property: JsonProperty("model")]
    string Model);

public sealed record AiRecommendedProduct(
    [property: JsonProperty("productId")]
    Guid ProductId,

    [property: JsonProperty("reason")]
    string Reason);