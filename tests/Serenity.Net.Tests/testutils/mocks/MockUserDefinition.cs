namespace Serenity.TestUtils;

/// <summary>
/// A simple in-memory implementation of <see cref="IUserDefinition"/> for tests.
/// </summary>
public class MockUserDefinition : IUserDefinition
{
    public MockUserDefinition()
    {
    }

    public MockUserDefinition(string id, string username, string? displayName = null,
        string? email = null, short isActive = 1)
    {
        Id = id;
        Username = username;
        DisplayName = displayName ?? username;
        Email = email;
        IsActive = isActive;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public short IsActive { get; set; }
}
