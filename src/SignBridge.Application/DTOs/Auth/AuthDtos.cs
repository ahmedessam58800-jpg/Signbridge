using SignBridge.Domain.Enums;

namespace SignBridge.Application.DTOs.Auth;

public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    DateOnly? DateOfBirth);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshRequest(string RefreshToken);

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string? LinkCode);
