namespace Serenity.Data;

/// <summary>
/// Interface for rows that support optimistic concurrency via a version / rowversion field.
/// </summary>
/// <remarks>
/// <para>The <see cref="ConcurrencyVersionField"/> is expected to be a table field that is
/// returned to the client (e.g. selected at Details level), but not editable, so it should
/// normally be annotated with <c>[ReadOnly(true)]</c>, <c>[Insertable(false)]</c> and
/// <c>[Updatable(false)]</c>. It can be a database generated row version (e.g. SQL Server
/// <c>rowversion</c>) or an application managed version.</para>
/// <para>On update, the save handler always appends the version to the update's <c>WHERE</c>
/// clause and throws a concurrency conflict if no row is affected, implementing optimistic
/// concurrency without a load then update race. The value sent by the client is used when
/// present (detecting a stale read); otherwise the value loaded from the database is used,
/// which still detects a concurrent write between the load and the update (mirroring the
/// server tracked concurrency token behavior of frameworks like EF Core / NHibernate).
/// Databases that manage the version automatically do not need any extra work; for application
/// managed versions, increment the field in a save behavior or a database trigger.</para>
/// <para>Client side, the value must be sent back with the entity even though there is no form
/// field for it, e.g. it is transferred via row metadata to the entity dialog.</para>
/// </remarks>
public interface IConcurrencyVersionRow
{
    /// <summary>
    /// Gets the concurrency version field.
    /// </summary>
    Field ConcurrencyVersionField { get; }
}
