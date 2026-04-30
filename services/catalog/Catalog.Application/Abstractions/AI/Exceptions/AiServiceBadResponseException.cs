namespace Catalog.Application.Abstractions.AI.Exceptions;

public sealed class AiServiceBadResponseException : Exception
{
    public AiServiceBadResponseException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}