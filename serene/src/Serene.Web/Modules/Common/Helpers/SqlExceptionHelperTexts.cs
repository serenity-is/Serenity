namespace Serene.Common;

[NestedLocalTexts(Prefix = "Validation.")]
public static partial class SqlExceptionHelperTexts
{
    public static readonly LocalText DeleteForeignKeyError = "Can't delete record. '{0}' table has records that depends on this one!";
    public static readonly LocalText SavePrimaryKeyError = "Can't save record. There is another record with the same {1} value!";
}