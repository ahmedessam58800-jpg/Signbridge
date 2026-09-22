using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Achievement : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏅";
    public int RequiredXp { get; set; }
}
