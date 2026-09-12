namespace Serenity.Extensions;

public class BaseTranslationOptionsTests
{
    [Fact]
    public void SectionKey_Is_Expected()
    {
        Assert.Equal("Translation", BaseTranslationOptions.SectionKey);
    }

    [Fact]
    public void Defaults_Are_Correct()
    {
        var options = new BaseTranslationOptions();
        Assert.False(options.Enabled);
        Assert.Equal(1, options.ParallelRequests);
        Assert.Equal(1, options.BatchSize);
    }

    [Fact]
    public void Properties_Can_Be_Set_And_Read()
    {
        var options = new BaseTranslationOptions
        {
            Enabled = true,
            ParallelRequests = 4,
            BatchSize = 10
        };

        Assert.True(options.Enabled);
        Assert.Equal(4, options.ParallelRequests);
        Assert.Equal(10, options.BatchSize);
    }
}
