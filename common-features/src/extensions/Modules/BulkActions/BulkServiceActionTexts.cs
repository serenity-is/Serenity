namespace Serenity.Extensions;

[NestedLocalTexts(Prefix = "Site.BulkServiceAction.")]
public static class BulkServiceActionTexts
{
    public static readonly LocalText AllHadErrorsFormat = "All {0} record(s) that are processed had errors!";
    public static readonly LocalText AllSuccessFormat = "Finished processing on {0} record(s) with success.";
    public static readonly LocalText ConfirmationFormat = "Perform this operation on {0} selected record(s)?";
    public static readonly LocalText ErrorCount = "{0} error(s)";
    public static readonly LocalText NothingToProcess = "Please select some records to process!";
    public static readonly LocalText SomeHadErrorsFormat = "Finished processing on {0} record(s) with success. {1} record(s) had errors!";
    public static readonly LocalText SuccessCount = "{0} done";
}