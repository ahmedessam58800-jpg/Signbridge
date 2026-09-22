using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class ParentChildLink : Entity
{
    public Guid ParentId { get; set; }
    public User Parent { get; set; } = null!;

    public Guid ChildId { get; set; }
    public User Child { get; set; } = null!;
}
