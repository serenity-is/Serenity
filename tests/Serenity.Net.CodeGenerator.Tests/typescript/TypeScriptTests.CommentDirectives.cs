using System.Diagnostics;

namespace Serenity.TypeScript;

public partial class TypeScriptTests
{
    private static CommentDirectiveType[] GetCommentDirectiveTypes(string text)
    {
        var sourceFile = new Parser().ParseSourceFile("test.ts", text);
        return [.. (sourceFile.CommentDirectives ?? []).Select(x => x.Type)];
    }

    [Theory]
    [InlineData("// @ts-ignore\nvar a = 1;", CommentDirectiveType.Ignore)]
    [InlineData("/// @ts-ignore\nvar a = 1;", CommentDirectiveType.Ignore)]
    [InlineData("//@ts-expect-error some reason\nvar a = 1;", CommentDirectiveType.ExpectError)]
    [InlineData("/* @ts-ignore */\nvar a = 1;", CommentDirectiveType.Ignore)]
    [InlineData("/*\n * some text\n * @ts-expect-error */\nvar a = 1;", CommentDirectiveType.ExpectError)]
    public void CommentDirective_Is_Recognized(string text, CommentDirectiveType expected)
    {
        Assert.Equal([expected], GetCommentDirectiveTypes(text));
    }

    [Theory]
    [InlineData("// ts-ignore\nvar a = 1;")]
    [InlineData("// some text @ts-ignore\nvar a = 1;")]
    [InlineData("/* some text @ts-ignore */\nvar a = 1;")]
    [InlineData("/*\n * @ts-ignore\n * some text */\nvar a = 1;")]
    public void Comment_Without_A_Directive_At_Its_Start_Is_Not_A_Directive(string text)
    {
        Assert.Empty(GetCommentDirectiveTypes(text));
    }

    /// <summary>
    /// The multi line directive pattern was escaped twice, so it never matched a
    /// directive, and it looped over "zero or more backslashes", which took
    /// exponential time for a last comment line that starts with ~30 backslashes.
    /// </summary>
    [Fact]
    public void Comment_Whose_Last_Line_Starts_With_Many_Backslashes_Is_Scanned_Quickly()
    {
        var text = "/*\n" + new string('\\', 2000) + "x */\nvar a = 1;";

        var sw = Stopwatch.StartNew();
        var actual = GetCommentDirectiveTypes(text);
        sw.Stop();

        Assert.Empty(actual);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Parsing a comment with 2000 backslashes took {sw.Elapsed}.");
    }
}
