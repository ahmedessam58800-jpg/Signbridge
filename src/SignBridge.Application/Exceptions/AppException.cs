namespace SignBridge.Application.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, 403) { }
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public sealed class ValidationException : AppException
{
    public ValidationException(string message) : base(message, 400) { }
}

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, 401) { }
}
