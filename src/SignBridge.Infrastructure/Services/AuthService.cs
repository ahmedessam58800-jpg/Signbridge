using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Auth;
using SignBridge.Application.Exceptions;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Entities;
using SignBridge.Domain.Enums;
using SignBridge.Infrastructure.Persistence;
using SignBridge.Infrastructure.Security;

namespace SignBridge.Infrastructure.Services;

internal sealed class AuthService(
    AppDbContext db,
    JwtTokenService tokens) : IAuthService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ValidationException("Full name is required.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ValidationException("A valid email is required.");

        if (request.Password.Length < 8)
            throw new ValidationException("Password must be at least 8 characters.");

        if (request.Role is not UserRole.Parent and not UserRole.Child)
            throw new ValidationException("Public registration only supports Parent and Child accounts.");

        if (await db.Users.AnyAsync(x => x.Email == email, cancellationToken))
            throw new ConflictException("An account with this email already exists.");

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Role = request.Role,
            DateOfBirth = request.DateOfBirth,
            LinkCode = request.Role == UserRole.Child ? GenerateLinkCode() : null
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await db.Users
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("This account is disabled.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password.");

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new ValidationException("Refresh token is required.");

        var hash = JwtTokenService.HashToken(request.RefreshToken);

        var storedToken = await db.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == hash, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Refresh token is expired or revoked.");

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        var response = await IssueTokensAsync(storedToken.User, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return response;
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, accessExpires) = tokens.CreateAccessToken(user);
        var refreshToken = tokens.CreateRefreshToken();

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = JwtTokenService.HashToken(refreshToken),
            ExpiresAtUtc = tokens.GetRefreshExpiryUtc()
        });

        await db.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            accessExpires,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.LinkCode);
    }

    private static string GenerateLinkCode()
    {
        return $"KID-{Random.Shared.Next(100000, 999999)}";
    }
}
