namespace Serenity.Extensions;

public class DialogUtilsTextsTests
{
    [Fact]
    public void AllTextKeys_Are_Accessible()
    {
        Assert.NotNull(DialogUtilsTexts.PendingChangesConfirmation);
        Assert.NotNull(DialogUtilsTexts.PendingChangesUnloadWarning);
    }
}
