namespace AI.Infrastructure.Exceptions;

public class AiProviderRateLimitException : Exception
{
    public AiProviderRateLimitException(string message) : base(message)
    {
    }
}