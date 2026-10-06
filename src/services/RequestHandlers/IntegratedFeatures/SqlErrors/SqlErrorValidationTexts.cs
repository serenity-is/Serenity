namespace Serenity.Services.SqlErrors;

/// <summary>
/// Validation error texts for database constraint violations.
/// </summary>
[NestedLocalTexts(Prefix = "Validation.")]
public static partial class SqlErrorValidationTexts
{
    /// <summary>
    /// Text for a foreign key violation that occurs while deleting a record, when the
    /// referencing table is known.
    /// </summary>
    public static readonly LocalText DeleteForeignKeyError =
        "Can't delete record. '{0}' table has records that depends on this one!";

    /// <summary>
    /// Text for a foreign key violation that occurs while deleting a record, when the
    /// referencing table is not known.
    /// </summary>
    public static readonly LocalText DeleteForeignKeyErrorGeneric =
        "Can't delete record. There are related records that depend on this one!";

    /// <summary>
    /// Text for a foreign key violation that occurs while saving a record, when the
    /// referenced table is known.
    /// </summary>
    public static readonly LocalText SaveForeignKeyError =
        "Can't save record. Referenced '{0}' record does not exist!";

    /// <summary>
    /// Text for a foreign key violation that occurs while saving a record, when the
    /// referenced table is not known.
    /// </summary>
    public static readonly LocalText SaveForeignKeyErrorGeneric =
        "Can't save record. A referenced record does not exist!";

    /// <summary>
    /// Text for a primary key or unique constraint violation while saving a record, when
    /// the field is known.
    /// </summary>
    public static readonly LocalText SavePrimaryKeyError =
        "Can't save record. There is another record with the same {1} value!";

    /// <summary>
    /// Text for a primary key or unique constraint violation while saving a record, when
    /// the field is not known.
    /// </summary>
    public static readonly LocalText SavePrimaryKeyErrorGeneric =
        "Can't save record. There is another record with the same value(s)!";
}
