namespace Serenity.Extensions;

public class BulkActionTextsTests
{
    [Fact]
    public void BasicProgressDialogTexts_Are_Accessible()
    {
        Assert.NotNull(BasicProgressDialogTexts.CancelTitle);
        Assert.NotNull(BasicProgressDialogTexts.PleaseWait);
    }

    [Fact]
    public void BulkServiceActionTexts_Are_Accessible()
    {
        Assert.NotNull(BulkServiceActionTexts.AllHadErrorsFormat);
        Assert.NotNull(BulkServiceActionTexts.AllSuccessFormat);
        Assert.NotNull(BulkServiceActionTexts.ConfirmationFormat);
        Assert.NotNull(BulkServiceActionTexts.ErrorCount);
        Assert.NotNull(BulkServiceActionTexts.NothingToProcess);
        Assert.NotNull(BulkServiceActionTexts.SomeHadErrorsFormat);
        Assert.NotNull(BulkServiceActionTexts.SuccessCount);
    }
}
