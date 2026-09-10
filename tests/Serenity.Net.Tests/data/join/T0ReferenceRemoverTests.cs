namespace Serenity.Data;

public class T0ReferenceRemoverTests
{
    [Fact]
    public void RemoveT0Aliases_Null_Throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => T0ReferenceRemover.RemoveT0Aliases(null));
    }

    [Fact]
    public void RemoveT0Aliases_Removes_Simple_T0_Reference()
    {
        Assert.Equal("Field", T0ReferenceRemover.RemoveT0Aliases("T0.Field"));
    }

    [Fact]
    public void RemoveT0Aliases_Is_Case_Insensitive_For_T()
    {
        Assert.Equal("field", T0ReferenceRemover.RemoveT0Aliases("t0.field"));
    }

    [Fact]
    public void RemoveT0Aliases_Keeps_Other_Aliases()
    {
        Assert.Equal("T1.Field", T0ReferenceRemover.RemoveT0Aliases("T1.Field"));
    }

    [Fact]
    public void RemoveT0Aliases_Removes_Only_T0_In_Mixed_Expression()
    {
        Assert.Equal("a = T1.b", T0ReferenceRemover.RemoveT0Aliases("T0.a = T1.b"));
    }

    [Fact]
    public void RemoveT0Aliases_Keeps_Longer_T_Identifiers()
    {
        Assert.Equal("T012.Field", T0ReferenceRemover.RemoveT0Aliases("T012.Field"));
    }

    [Fact]
    public void RemoveT0Aliases_Keeps_T0_As_Part_Of_Longer_Identifier()
    {
        Assert.Equal("xT0.Field", T0ReferenceRemover.RemoveT0Aliases("xT0.Field"));
    }

    [Fact]
    public void RemoveT0Aliases_Keeps_T0_Reference_In_Quotes()
    {
        Assert.Equal("'T0.Field'", T0ReferenceRemover.RemoveT0Aliases("'T0.Field'"));
    }

    [Fact]
    public void RemoveT0Aliases_Removes_Trailing_T0_Dot()
    {
        Assert.Equal("", T0ReferenceRemover.RemoveT0Aliases("T0."));
    }

    [Fact]
    public void RemoveT0Aliases_Removes_T0_After_Other_Alias()
    {
        Assert.Equal("a.b", T0ReferenceRemover.RemoveT0Aliases("a.T0.b"));
    }

    [Fact]
    public void RemoveT0Aliases_Removes_All_T0_References()
    {
        Assert.Equal("a + b", T0ReferenceRemover.RemoveT0Aliases("T0.a + T0.b"));
    }

    [Fact]
    public void RemoveT0Aliases_Keeps_Expression_Without_T0()
    {
        Assert.Equal("Field", T0ReferenceRemover.RemoveT0Aliases("Field"));
    }
}
