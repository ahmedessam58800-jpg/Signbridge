using SignBridge.Application.DTOs.Dictionary;

namespace SignBridge.Application.Interfaces;

public interface IDictionaryService
{
    Task<IReadOnlyList<SignEntryDto>> SearchAsync(
        string? query,
        string? category,
        string? difficulty,
        CancellationToken cancellationToken);

    Task<Guid> CreateAsync(CreateSignEntryRequest request, CancellationToken cancellationToken);
}
