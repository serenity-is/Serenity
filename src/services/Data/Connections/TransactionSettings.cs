namespace Serenity.Data;

/// <summary>
/// Determines the transaction isolation level and defer start flag used for
/// service endpoint actions when creating the <see cref="UnitOfWork"/> class.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class TransactionSettings
{
    /// <summary>
    /// The default section key for <see cref="TransactionSettings"/>.
    /// </summary>
    public const string SectionKey = "TransactionSettings";

    /// <summary>
    /// Gets or sets the isolation level.
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Gets or sets the defer start flag.
    /// </summary>
    public bool? DeferStart { get; set; }
}