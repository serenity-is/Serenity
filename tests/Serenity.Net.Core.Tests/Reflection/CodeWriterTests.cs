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

    [Fact]
    public void InBrace_ThrowsArgumentNull_If_InsideBlock_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CodeWriter().InBrace(insideBlock: null, endLine: true));
        Assert.Throws<ArgumentNullException>(() => new CodeWriter().InBrace(insideBlock: null, endLine: false));
    }

    [Fact]
    public void InBrace_Prepends_SpaceBefore_If_BraceOnSameLine_IsTrue()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = true
        };
        cw.Append("A");
        cw.InBrace(() => cw.AppendLine("Test"));
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A {\nTest\n}", actual);
    }

    [Fact]
    public void InBrace_Does_Not_Prepend_Space_If_BraceOnSameLine_IsFalse()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.Append("A");
        cw.InBrace(() => cw.AppendLine("Test"));
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A{\nTest\n}", actual);
    }

    [Fact]
    public void InBrace_Does_Not_EndLine_If_EndLine_IsFalse()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.Append("A");
        cw.InBrace(() => cw.AppendLine("Test"), endLine: false);
        cw.Append("X");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A{\nTest\n}X", actual);
    }

    [Fact]
    public void InBrace_EndsLine_If_EndLine_IsTrue()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.Append("A");
        cw.InBrace(() => cw.AppendLine("Test"), endLine: true);
        cw.Append("X");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A{\nTest\n}\nX", actual);
    }

    [Fact]
    public void InBrace_Increases_Indent_InsideTheBlock()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.AppendLine("A");
        cw.InBrace(() => 
        {
            cw.IndentedLine("Test1");
            cw.IndentedLine("Test2");
        }, endLine: true);
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n{\n\tTest1\n\tTest2\n}", actual);
    }

    [Fact]
    public void InBrace_Restores_Indent_AfterTheBlock()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.AppendLine("A");
        cw.InBrace(() =>
        {
            cw.IndentedLine("Test1");
            cw.InBrace(() => cw.IndentedLine("Test2"));
            cw.IndentedLine("Test3");
        }, endLine: true);
        cw.IndentedLine("Test4");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n{\n\tTest1\n\t{\n\t\tTest2\n\t}\n\tTest3\n}\nTest4", actual);
    }

    [Fact]
    public void StartBrace_Prepends_SpaceBefore_If_BraceOnSameLine_IsTrue()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = true
        };
        cw.Append("A");
        cw.StartBrace();
        cw.Append("X");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A {\nX", actual);
    }

    [Fact]
    public void StartBrace_Does_Not_Prepend_Space_If_BraceOnSameLine_IsFalse()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.Append("A");
        cw.StartBrace();
        cw.Append("X");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A{\nX", actual);
    }

    [Fact]
    public void EndBrace_Does_Not_EndLine_If_EndLine_IsFalse()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.Append("A");
        cw.StartBrace();
        cw.Append("X");
        cw.EndBrace(endLine: false);
        cw.Append("Y");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A{\nX}Y", actual);
    }

    [Fact]
    public void EndBrace_EndsLine_If_EndLine_IsTrue()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.Append("A");
        cw.StartBrace();
        cw.Append("X");
        cw.EndBrace(endLine: true);
        cw.Append("Y");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A{\nX}\nY", actual);
    }

    [Fact]
    public void StartBrace_Increases_Indent()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.AppendLine("A");
        cw.StartBrace();
        cw.IndentedLine("Test1");
        cw.StartBrace();
        cw.IndentedLine("Test2");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n{\n\tTest1\n\t{\n\t\tTest2", actual);
    }

    [Fact]
    public void EndBrace_Decreases_Indent()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            BraceOnSameLine = false
        };
        cw.IncreaseIndent();
        cw.IndentedLine("X");
        cw.EndBrace();
        cw.IndentedLine("Y");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("\tX\n}\nY", actual);
    }

    [Fact]
    public void Indent_Appends_CurrentIndent()
    {
        var cw = new CodeWriter(new(), tab: "\t");
        cw.Append("A");
        cw.Indent();
        cw.Append("B");
        cw.IncreaseIndent();
        cw.Indent();
        cw.Append("C");
        cw.IncreaseIndent();
        cw.Indent();
        cw.Append("D");
        cw.DecreaseIndent();
        cw.DecreaseIndent();
        cw.Indent();
        cw.Append("E");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("AB\tC\t\tDE", actual);
    }

    [Fact]
    public void Indented_MultiLine_Ignores_EmptyString()
    {
        var cw = new CodeWriter(new(), tab: "\t");
        cw.IndentedLine("A");
        cw.IncreaseIndent();
        cw.IncreaseIndent();
        cw.IndentedMultiLine("");
        cw.DecreaseIndent();
        cw.IndentedLine("B");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n\tB", actual);
    }

    [Fact]
    public void Indented_MultiLine_Indents_Each_Line_By_Current_Indent()
    {
        var cw = new CodeWriter(new(), tab: "\t");
        cw.IndentedLine("A");
        cw.IncreaseIndent();
        cw.IncreaseIndent();
        cw.IndentedMultiLine("X\n\tY\n\t\tZ\nU\n");
        cw.DecreaseIndent();
        cw.IndentedLine("B");
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("A\n\t\tX\n\t\t\tY\n\t\t\t\tZ\n\t\tU\n\n\tB", actual);
    }

    [Fact]
    public void InNamespace_DoesNotOpen_An_Actual_Namespace_If_Passed_Null()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            CurrentNamespace = "X"
        };
        cw.InNamespace(null, () =>
        {
            cw.IndentedLine("Test");
            Assert.Null(cw.CurrentNamespace);
        });
        Assert.Equal("X", cw.CurrentNamespace);
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("Test", actual);
    }

    [Fact]
    public void InNamespace_DoesNotOpen_An_Actual_Namespace_If_Passed_EmptyString()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            CurrentNamespace = "X"
        };
        cw.InNamespace("", () =>
        {
            cw.IndentedLine("Test");
            Assert.Equal("", cw.CurrentNamespace);
        });
        Assert.Equal("X", cw.CurrentNamespace);
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("Test", actual);
    }

    [Fact]
    public void InNamespace_Opens_Namespace_With_Braced_Block()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            CurrentNamespace = "X",
            BraceOnSameLine = false
        };
        cw.InNamespace("Z", () =>
        {
            cw.IndentedLine("Test");
            Assert.Equal("Z", cw.CurrentNamespace);
        });
        cw.IndentedLine("Y");
        Assert.Equal("X", cw.CurrentNamespace);
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("namespace Z\n{\n\tTest\n}\nY", actual);
    }

    [Fact]
    public void InNamespace_Uses_Current_Indentation()
    {
        var cw = new CodeWriter(new(), tab: "\t")
        {
            CurrentNamespace = "X",
            BraceOnSameLine = false
        };
        cw.IncreaseIndent();
        cw.IncreaseIndent();
        cw.InNamespace("Z", () =>
        {
            cw.IndentedLine("Test");
            Assert.Equal("Z", cw.CurrentNamespace);
        });
        cw.DecreaseIndent();
        cw.IndentedLine("Y");
        Assert.Equal("X", cw.CurrentNamespace);
        var actual = cw.ToString().Replace("\r", "").TrimEnd();
        Assert.Equal("\t\tnamespace Z\n\t\t{\n\t\t\tTest\n\t\t}\n\tY", actual);
    }
}