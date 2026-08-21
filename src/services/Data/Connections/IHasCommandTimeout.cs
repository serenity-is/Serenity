namespace Serenity.Data;

/// <summary>
/// Interface for types that have a <see cref="CommandTimeout"/> property that
/// determines the default command timeout for that connection.
/// </summary>
public interface IHasCommandTimeout
{
    /// <summary>
    /// Gets or sets the command timeout.
    /// </summary>
    int? CommandTimeout { get; set; }
}