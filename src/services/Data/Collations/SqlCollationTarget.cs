namespace Serenity.Data;

/// <summary>
/// What a collation is mapped for, as the available collations can differ between them.
/// </summary>
public enum SqlCollationTarget
{
    /// <summary>The default collation of a database, e.g. in a CREATE DATABASE statement.</summary>
    Database,
    /// <summary>The collation of a string column, e.g. in a migration.</summary>
    Column
}
