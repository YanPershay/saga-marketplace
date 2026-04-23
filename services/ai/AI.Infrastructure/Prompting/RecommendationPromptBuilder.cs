using System.Text;
using AI.Application.Abstractions;
using AI.Domain.Products;

namespace AI.Infrastructure;

public sealed class RecommendationPromptBuilder : IRecommendationPromptBuilder
{
    public string BuildPrompt(ProductContext currentProduct, IReadOnlyCollection<CandidateProduct> candidateProducts)
    {
        if (currentProduct == null) throw new ArgumentNullException(nameof(currentProduct));
        if (candidateProducts == null
            || candidateProducts.Count == 0) throw new ArgumentNullException(nameof(candidateProducts));

        var prompt = new StringBuilder();
        prompt.AppendLine(
            "You are a product recommendation engine. Based on the current product and a list of candidate products, select the most semantically relevant alternatives or complements." +
            "Return exactly 3 recommendations, ranked in order of relevance.");
        prompt.AppendLine(
            "Consider factors such as product category, price, and description when making your recommendations.");
        prompt.AppendLine();
        prompt.AppendLine("Do not recommend current product.");
        prompt.AppendLine("Use only provided candidate products.");
        prompt.AppendLine("Do not invent products. Do not invent ids.");
        prompt.AppendLine("Return result in clear JSON format.");
        prompt.AppendLine("Return valid JSON only.");
        prompt.AppendLine("Do not wrap JSON in markdown or codeblocks.");
        prompt.AppendLine("Do not include commentary before or after JSON.");
        prompt.AppendLine("Use this exact schema:");
        prompt.AppendLine("""
                          {
                              "recommendations": [
                                  {
                                      "productId": "guid",
                                      "reason": "short explanation of why this product was recommended"
                                  }
                              ]
                          }
                          """);
        prompt.AppendLine("Current Product:");
        prompt.AppendLine($"- Id: {currentProduct.Id}");
        prompt.AppendLine($"- Name: {currentProduct.Name}");
        prompt.AppendLine($"- Description: {currentProduct.Description}");
        prompt.AppendLine($"- Price: {currentProduct.Price}");
        prompt.AppendLine($"- Category: {currentProduct.Category}");
        prompt.AppendLine();
        prompt.AppendLine("Candidate Products:");
        foreach (var candidate in candidateProducts)
        {
            prompt.AppendLine($"- Id: {candidate.Id}");
            prompt.AppendLine($"  Name: {candidate.Name}");
            prompt.AppendLine($"  Description: {candidate.Description}");
            prompt.AppendLine($"  Price: {candidate.Price}");
            prompt.AppendLine($"  Category: {candidate.Category}");
        }

        prompt.AppendLine();

        return prompt.ToString();
    }
}
