namespace Serenity.Data;

public class TransactionSettingsTests
{
    [Fact]
    public void TransactionSettings_Defaults_And_Assignment()
    {
        var settings = new TransactionSettings();
        Assert.Null(settings.IsolationLevel);
        Assert.Null(settings.DeferStart);
        Assert.Equal("TransactionSettings", TransactionSettings.SectionKey);

        settings.IsolationLevel = IsolationLevel.ReadCommitted;
        settings.DeferStart = true;

        Assert.Equal(IsolationLevel.ReadCommitted, settings.IsolationLevel);
        Assert.True(settings.DeferStart);
    }

    [Fact]
    public void TransactionSettingsAttribute_Defaults()
    {
        var attr = new TransactionSettingsAttribute();

        Assert.Equal(IsolationLevel.Unspecified, attr.IsolationLevel);
        Assert.False(attr.DeferStart);
        Assert.False(attr.HasDeferStart);
    }

    [Fact]
    public void TransactionSettingsAttribute_IsolationLevel_Constructor()
    {
        var attr = new TransactionSettingsAttribute(IsolationLevel.Serializable);
        Assert.Equal(IsolationLevel.Serializable, attr.IsolationLevel);
    }

    [Fact]
    public void TransactionSettingsAttribute_DeferStart_Setter()
    {
        var attr = new TransactionSettingsAttribute { DeferStart = true };

        Assert.True(attr.DeferStart);
        Assert.True(attr.HasDeferStart);
    }
}
