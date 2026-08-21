using FluentMigrator;

namespace Serenity.Extensions;

/// <summary>
/// Base attribute for migration attributes that validates the migration version format.
/// </summary>
public abstract class MigrationAttributeBase : MigrationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttributeBase"/> class.
    /// </summary>
    /// <param name="version">The migration version in yyyyMMdd_HHmm or yyyyMMdd_HHmm_ss format.</param>
    /// <param name="transactionBehavior">The transaction behavior.</param>
    /// <param name="description">The migration description.</param>
    /// <exception cref="Exception">The version is not in a valid migration version format.</exception>
    public MigrationAttributeBase(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null)
        : base((version >= 20010101_0000 && version <= 99990101_0000) ? version * 100 : version, transactionBehavior, description)
    {
        if (Version < 20010101_000000 || Version > 99990101_000000)
            throw new Exception("Migration versions must be in yyyyMMdd_HHmm or " +
                "yyyyMMdd_HHmm_ss format! Version " + version + " is incorrect.");
    }
}
