namespace SignBridge.Application.DTOs.Dictionary;

public sealed record SignEntryDto(
    Guid Id,
    string Word,
    string ArabicWord,
    string Category,
    string Difficulty,
    string Description,
    string? VideoUrl,
    string? ImageUrl);

public sealed record CreateSignEntryRequest(
    string Word,
    string ArabicWord,
    string Category,
    string Difficulty,
    string Description,
    string? VideoUrl,
    string? ImageUrl);
