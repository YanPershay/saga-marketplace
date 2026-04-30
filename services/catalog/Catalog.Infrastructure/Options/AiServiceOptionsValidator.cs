using Microsoft.Extensions.Options;

namespace Catalog.Infrastructure.Options;

public sealed class AiServiceOptionsValidator : IValidateOptions<AiServiceOptions>
{
    public ValidateOptionsResult Validate(string? name, AiServiceOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            return ValidateOptionsResult.Fail("AiService BaseUrl is required.");

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            return ValidateOptionsResult.Fail("AiService BaseUrl must be a valid absolute URI.");

        if (options.TimeoutSeconds <= 0)
            return ValidateOptionsResult.Fail("TimeoutSeconds must be greater than 0.");

        return ValidateOptionsResult.Success;
    }
}