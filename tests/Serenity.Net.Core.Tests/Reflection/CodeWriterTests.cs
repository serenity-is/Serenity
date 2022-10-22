using NuGet.Frameworks;
using Serenity.Reflection;

namespace Serenity.Tests.Reflection;

public class CodeWriterTests
{
    [Fact]
    public void Ctor_Throws_ArgumentNull_If_StringBuilder_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CodeWriter(sb: null, tabSize: 4));
        Assert.Throws<ArgumentNullException>(() => new CodeWriter(sb: null, tab: "\t"));
    }

    [Fact]
    public void Ctor_Throws_ArgumentNull_If_TabString_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CodeWriter(new(), null));
    }

    [Fact]
    public void Ctor_Tab_Size_Is_4_ByDefault()
    {
        var cw = new CodeWriter();
        Assert.Equal("    ", cw.Tab);
    }

    [Fact]
    public void Ctor_Uses_Specified_Tab_String_As_Is()
    {
        var cw1 = new CodeWriter(new(), "\t");
        Assert.Equal("\t", cw1.Tab);
        var cw2 = new CodeWriter(new(), "@@@");
        Assert.Equal("@@@", cw2.Tab);
    }

    [Fact]
    public void Increase_Indent_Adds_One_Tab_String()
    {
        var cw = new CodeWriter(new(), "123");
        Assert.Equal("", cw.ToString());
        cw.IncreaseIndent();
        cw.Indented("X");
        Assert.Equal("123X", cw.ToString());
        cw.IncreaseIndent();
        cw.Indented("Y");
        Assert.Equal("123X123123Y", cw.ToString());
    }

    [Fact]
    public void Decrease_Indent_Removes_One_Tab_String()
    {
        var cw = new CodeWriter(new(), "123");
        cw.IncreaseIndent();
        cw.IncreaseIndent();
        cw.IncreaseIndent();
        cw.Indented("X");
        Assert.Equal("123123123X", cw.ToString());
        cw.DecreaseIndent();
        cw.Indented("Y");
        Assert.Equal("123123123X123123Y", cw.ToString());
        cw.DecreaseIndent();
        cw.Indented("Z");
        Assert.Equal("123123123X123123Y123Z", cw.ToString());
        cw.DecreaseIndent();
        cw.Indented("!");
        Assert.Equal("123123123X123123Y123Z!", cw.ToString());
    }

    [Fact]
    public void Decrease_Indent_Ignores_If_No_Current_Indent()
    {
        var cw = new CodeWriter(new(), "123");
        cw.IncreaseIndent();
        cw.IncreaseIndent();
        cw.DecreaseIndent();
        cw.DecreaseIndent();
        cw.DecreaseIndent();
        cw.Indented("Z");
        Assert.Equal("Z", cw.ToString());
    }

    [Fact]
    public void Block_Throws_ArgumentNull_If_Action_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CodeWriter().Block(insideBlock: null));
    }

    [Fact]
    public void Block_Increases_Indent_Calls_Action_Then_Decreases_Indent()
    {
        var cw = new CodeWriter(new(), "\t");
        cw.IndentedLine("A");
        cw.Block(() =>
        {
            cw.IndentedLine("X");
            cw.IndentedLine("Y");
        });
        cw.IndentedLine("B");

        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n\tX\n\tY\nB", actual);
    }

    [Fact]
    public void Block_CanBe_Nested()
    {
        var cw = new CodeWriter(new(), "\t");
        cw.IndentedLine("A");
        cw.Block(() =>
        {
            cw.IndentedLine("X");
            cw.Block(() =>
            {
                cw.IndentedLine("Y");
            });
            cw.IndentedLine("Z");
        });
        cw.IndentedLine("B");

        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n\tX\n\t\tY\n\tZ\nB", actual);
    }

}