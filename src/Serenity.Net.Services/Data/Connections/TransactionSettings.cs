namespace Serenity.Data;

/// <summary>
/// Determines the transaction isolation level and defer start flag used for
/// service endpoint action use when creating the UnitOfWork class
/// </summary>
[DefaultSectionKey(SectionKey)]
public class TransactionSettings
{
    /// <summary>
    /// Default sectionkey for TransactionSettings
    /// </summary>
    public const string SectionKey = "TransactionSettings";

    /// <summary>
    /// Gets sets the isolation level
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Gets sets the defer start flag
    /// </summary>
    public bool? DeferStart { get; set; }
}