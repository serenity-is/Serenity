namespace Serenity.Services;

/// <summary>
/// Contains constants related to the capture logging
/// </summary>
public class CaptureLogConsts
{
    /// <summary>
    /// Gets the maximum value for the ValidUntil column.
    /// </summary>
    /// <remarks>
    /// Stored as UTC. When used with a capture log row whose ValidUntil field has a different
    /// <see cref="DateTimeKind"/>, normalize it with <see cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/>
    /// (which keeps the clock) so the stored value and the close-active comparison stay identical.
    /// </remarks>
    public static readonly DateTime UntilMax = new(9999, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}