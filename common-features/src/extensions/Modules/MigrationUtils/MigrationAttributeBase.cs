using FluentMigrator;

namespace Serenity.Extensions;

public abstract class MigrationAttributeBase : MigrationAttribute
{
    public MigrationAttributeBase(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null)
        : base((version >= 20010101_0000 && version <= 99990101_0000) ? version * 100 : version, transactionBehavior, description)
    {
        if (Version < 20010101_000000 || Version > 99990101_000000)
            throw new Exception("Migration versions must be in yyyyMMdd_HHmm or " +
                "yyyyMMdd_HHmm_ss format! Version " + version + " is incorrect.");
    }
}
