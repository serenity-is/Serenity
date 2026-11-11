namespace Serenity.Abstractions;

/// <summary>
/// Options for listing users with <see cref="IUserListService"/>.
/// </summary>
public class UserListOptions
{
    /// <summary>
    /// Gets or sets a text that the username or the display name of the users should contain. The username is compared
    /// the way the users are looked up by it, which is by its normalized form when the application keeps one, and the
    /// display name the way the database compares text, so it is case insensitive with the usual collations.
    /// All users match when it is blank.
    /// </summary>
    public string? ContainsText { get; set; }

    /// <summary>
    /// Gets or sets whether to list the users that are disabled too, which is <c>false</c> by default.
    /// The users that are deleted are never listed, unless they are asked for with <see cref="UserIds"/>
    /// or <see cref="Usernames"/>.
    /// </summary>
    public bool IncludeInactive { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of the users to list, e.g. to show the usernames of the users that the records of
    /// another table refer to, which may be in another database, so they can't be joined. Only these users are listed
    /// when it is set, whether they are active or not, as they are asked for by who they are. The ones that are not found
    /// or are not valid identifiers are left out. It is no restriction when it is null, and an empty list lists nobody.
    /// If <see cref="Usernames"/> is set too, the users of either are listed.
    /// </summary>
    public IEnumerable<string>? UserIds { get; set; }

    /// <summary>
    /// Gets or sets the usernames of the users to list, e.g. to show their display names. They are compared the way
    /// the users are looked up by their usernames, so the case is ignored when the application does so. Only these users
    /// are listed when it is set, whether they are active or not, as they are asked for by who they are. It is no
    /// restriction when it is null, and an empty list lists nobody. If <see cref="UserIds"/> is set too, the users of
    /// either are listed.
    /// </summary>
    public IEnumerable<string>? Usernames { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of users to list, ordered by username. All users are listed when it is zero
    /// or less.
    /// </summary>
    public int Take { get; set; }
}
