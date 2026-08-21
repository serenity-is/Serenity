using FluentMigrator;

namespace Serenity.Extensions;

/// <summary>
/// Marks a migration with a version key.
/// </summary>
public class MigrationKeyAttribute(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null) : MigrationAttributeBase(version, transactionBehavior, description)
{
}
