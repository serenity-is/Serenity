namespace Serenity.Data;

/// <summary>
/// A combination of <see cref="IUpdateLogRow"/> and <see cref="IInsertLogRow"/>.
/// </summary>
/// <seealso cref="IUpdateLogRow" />
/// <seealso cref="IInsertLogRow" />
public interface ILoggingRow : IUpdateLogRow, IInsertLogRow
{
}