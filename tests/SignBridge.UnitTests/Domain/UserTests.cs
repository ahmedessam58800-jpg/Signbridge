using SignBridge.Domain.Entities;

namespace SignBridge.UnitTests.Domain;

public sealed class UserTests
{
    [Fact]
    public void New_user_is_active_by_default()
    {
        var user = new User();

        Assert.True(user.IsActive);
        Assert.Equal(0, user.TotalXp);
        Assert.Equal(0, user.CurrentStreak);
    }
}
