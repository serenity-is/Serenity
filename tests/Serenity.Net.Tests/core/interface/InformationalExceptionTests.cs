namespace Serenity;

public class InformationalExceptionTests
{
    [Fact]
    public void Ctor_SetsMessage()
    {
        var ex = new InformationalException("info");
        Assert.Equal("info", ex.Message);
    }

    [Fact]
    public void EventId_Is77701()
    {
        Assert.Equal(77701, InformationalException.EventId);
    }
}