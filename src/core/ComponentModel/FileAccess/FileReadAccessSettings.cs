using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Settings for file read access control
/// </summary>
[DefaultSectionKey(SectionKey)]
public class FileReadAccessSettings : IOptions<FileReadAccessSettings>
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "FileReadAccess";

    /// <summary>
    /// Default file read access when no FileReadAccess attribute is present.
    /// Use "*" for public, "?" for logged-in users, or a specific permission.
    /// Default is "*" (public) for backward compatibility.
    /// </summary>
    public string? DefaultPermission { get; set; } = "*";

    /// <summary>
    /// Permission that bypasses all file read access checks.
    /// You may set this to a special permission like "Administration:General"
    /// to allow administrators to read all files.
    /// </summary>
    public string? BypassPermission { get; set; }

    /// <summary>
    /// What to do when .meta file is missing or corrupted.
    /// Options: "Allow" (default), "Deny", "DefaultPermission"
    /// </summary>
    public FileReadAccessMissingBehavior? MissingMetaBehavior { get; set; } = FileReadAccessMissingBehavior.DefaultPermission;

    /// <summary>
    /// What to do when related entity cannot be found or accessed.
    /// Options: "Allow", "Deny" (default), "DefaultPermission"
    /// </summary>
    public FileReadAccessMissingBehavior? MissingEntityBehavior { get; set; } = FileReadAccessMissingBehavior.Deny;

    /// <summary>
    /// Regular expression patterns for paths and corresponding permissions that are handled before 
    /// checking file access permissions. Default is "^public/:*;^temporary/:*" for allowing public 
    /// access to files under "public/" and "temporary/".
    /// Separate multiple patterns with semicolon (;). The format is "pattern:permission", where 
    /// pattern is a regular expression to match the file path, and permission is the permission 
    /// to check if the pattern matches. Use "*" for public access, "?" for logged-in users, 
    /// "NEVER" to block access or a specific permission. Patterns are evaluated in order,
    /// and the first match is used.
    /// </summary>
    public string? PathPermissions { get; set; } = "^public/:*;^temporary/:*";

    /// <summary>
    /// Whether to log access control decisions for debugging
    /// </summary>
    public bool? EnableAccessLogging { get; set; }

    /// <summary>
    /// Whether to return "forbidden" (403) result instead of the default "not found" (404).
    /// </summary>
    public bool? ReturnForbidResult { get; set; }

    /// <inheritdoc />
    public FileReadAccessSettings Value => throw new NotImplementedException();
}