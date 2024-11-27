using FluentMigrator;

namespace Serenity.Extensions;

public class MigrationKeyAttribute(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null) : MigrationAttributeBase(version, transactionBehavior, description)
{
}
