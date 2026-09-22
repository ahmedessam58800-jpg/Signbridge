using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Dictionary;
using SignBridge.Application.Exceptions;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Entities;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class DictionaryService(AppDbContext db) : IDictionaryService
{
    public async Task<IReadOnlyList<SignEntryDto>> SearchAsync(
        string? query,
        string? category,
        string? difficulty,
        CancellationToken cancellationToken)
    {
        var q = db.SignEntries.AsNoTracking().Where(x => x.IsPublished);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            q = q.Where(x => x.Word.Contains(term) || x.ArabicWord.Contains(term) || x.Description.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(category))
            q = q.Where(x => x.Category == category);

        if (!string.IsNullOrWhiteSpace(difficulty))
            q = q.Where(x => x.Difficulty == difficulty);

        return await q.OrderBy(x => x.Category).ThenBy(x => x.Word)
            .Select(x => new SignEntryDto(
                x.Id, x.Word, x.ArabicWord, x.Category, x.Difficulty,
                x.Description, x.VideoUrl, x.ImageUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(CreateSignEntryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Word))
            throw new ValidationException("Word is required.");

        var entity = new SignEntry
        {
            Word = request.Word.Trim(),
            ArabicWord = request.ArabicWord.Trim(),
            Category = request.Category.Trim(),
            Difficulty = request.Difficulty.Trim(),
            Description = request.Description.Trim(),
            VideoUrl = request.VideoUrl?.Trim(),
            ImageUrl = request.ImageUrl?.Trim()
        };

        db.SignEntries.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
