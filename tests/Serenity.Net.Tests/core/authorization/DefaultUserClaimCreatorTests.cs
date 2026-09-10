namespace Serenity.Services;

public class DefaultUserClaimCreatorTests
{
    private static DefaultUserClaimCreator NewCreator(params IUserDefinition[] users)
    {
        return new DefaultUserClaimCreator(new MockUserRetrieveService(users));
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenUserRetrieverIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultUserClaimCreator(null));
    }

    [Fact]
    public void CreatePrincipal_CreatesPrincipalWithNameIdentifierClaim()
    {
        var user = new MockUserDefinition("42", "admin", "Admin User");
        var creator = NewCreator(user);

        var principal = creator.CreatePrincipal("admin", "Test");

        Assert.NotNull(principal.Identity);
        Assert.True(principal.Identity.IsAuthenticated);
        Assert.Equal("admin", principal.Identity.Name);
        Assert.Equal("Test", principal.Identity.AuthenticationType);
        Assert.Equal("42", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    [Fact]
    public void CreatePrincipal_ThrowsArgumentNullException_WhenUsernameIsNull()
    {
        var creator = NewCreator(new MockUserDefinition("42", "admin"));
        Assert.Throws<ArgumentNullException>(() => creator.CreatePrincipal(null, "Test"));
    }

    [Fact]
    public void CreatePrincipal_ThrowsArgumentNullException_WhenAuthTypeIsNull()
    {
        var creator = NewCreator(new MockUserDefinition("42", "admin"));
        Assert.Throws<ArgumentNullException>(() => creator.CreatePrincipal("admin", null));
    }

    [Fact]
    public void CreatePrincipal_ThrowsArgumentOutOfRangeException_WhenUserNotFound()
    {
        var creator = NewCreator(new MockUserDefinition("42", "admin"));
        Assert.Throws<ArgumentOutOfRangeException>(() => creator.CreatePrincipal("missing", "Test"));
    }
}
