namespace Serenity.Services;

[NestedLocalTexts]
internal static partial class Texts
{
    public static class Validation
    {
        public static readonly LocalText ArgumentIsNull = "Argument {0} is null!";
        public static readonly LocalText ArgumentOutOfRange = "Argument {0} is out of range!";
        public static readonly LocalText Recaptcha = "Please validate that you are not a robot!";
        public static readonly LocalText EntityForeignKeyViolation = "You must first delete related {0} records before deleting this one!";
        public static readonly LocalText EntityHasDeletedParent = "Before operating on this record, its parent must be undeleted!";
        public static readonly LocalText EntityIsNotActive = "Can't operate on a deleted {0} record!";
        public static readonly LocalText EntityNotFound = "Record not found. It might be deleted or you don't have required permissions!";
        public static readonly LocalText EntityReadAccessViolation = "You don't have required permissions to view {0} record with key: {1}";
        public static readonly LocalText EntityWriteAccessViolation = "You don't have required permissions to update {0} record with key: {1}";
        public static readonly LocalText FieldIsReadOnly = "{0} field is read only!";
        public static readonly LocalText FieldIsRequired = "{0} field is required!";
        public static readonly LocalText FieldInvalidValue = "Invalid value {0} for field {1}!";
        public static readonly LocalText FieldInvalidDateRange = "{0} field value can't be before date {1}!";
        public static readonly LocalText RequestIsNull = "Request is null!";
        public static readonly LocalText UnexpectedError = "An unexpected error has occurred!";
    }
}