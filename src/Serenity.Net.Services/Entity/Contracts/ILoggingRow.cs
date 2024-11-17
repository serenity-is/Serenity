namespace Serenity.Data;

/// <summary>
/// A combination of IUpdateLogRow and IInsertLogRow
/// </summary>
/// <seealso cref="IUpdateLogRow" />
/// <seealso cref="IInsertLogRow" />
public interface ILoggingRow : IUpdateLogRow, IInsertLogRow
{
}