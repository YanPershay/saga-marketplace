namespace Catalog.Application.Abstractions.AI.Exceptions;

public sealed class AiServiceTimeoutException : Exception
{
    public AiServiceTimeoutException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}