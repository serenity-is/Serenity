using System.Diagnostics;

namespace Serenity.CodeGeneration;

public class CSharpSyntaxRulesTests
{
    [Theory]
    [InlineData("a")]
    [InlineData("_")]
    [InlineData("_a1")]
    [InlineData("Abc")]
    [InlineData("Abc_Def9")]
    [InlineData("İçerik")]
    [InlineData("áb")] // combining mark
    [InlineData("a‿b")] // connector punctuation
    public void IsValidIdentifier_Returns_True_For_Valid_Identifiers(string identifier)
    {
        Assert.True(CSharpSyntaxRules.IsValidIdentifier(identifier, ignoreKeywords: true));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1a")]
    [InlineData("a b")]
    [InlineData("a-b")]
    [InlineData("a.b")]
    [InlineData("a/b")]
    [InlineData(" a")]
    [InlineData("a ")]
    [InlineData("́a")]
    [InlineData("a\nb")]
    public void IsValidIdentifier_Returns_False_For_Invalid_Identifiers(string identifier)
    {
        Assert.False(CSharpSyntaxRules.IsValidIdentifier(identifier, ignoreKeywords: true));
    }

    [Fact]
    public void IsValidIdentifier_Checks_Keywords_Unless_Ignored()
    {
        Assert.True(CSharpSyntaxRules.IsValidIdentifier("class", ignoreKeywords: true));
        Assert.False(CSharpSyntaxRules.IsValidIdentifier("class", ignoreKeywords: false));
    }

    /// <summary>
    /// The identifier pattern repeated its part characters twice, e.g. "((part)+)*",
    /// which has exponentially many ways to split a run of them: a name of 28 valid
    /// characters followed by an invalid one took ~6 seconds, and 32 of them over
    /// 20 seconds, e.g. a long local text key segment with a space or dash in it.
    /// </summary>
    [Theory]
    [InlineData(" x")]
    [InlineData("-")]
    [InlineData("/Hint")]
    public void IsValidIdentifier_Is_Linear_For_A_Long_Name_With_An_Invalid_Character(string ending)
    {
        var identifier = new string('a', 5000) + ending;

        var sw = Stopwatch.StartNew();
        var actual = CSharpSyntaxRules.IsValidIdentifier(identifier, ignoreKeywords: true);
        sw.Stop();

        Assert.False(actual);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Checking an identifier of 5000 characters took {sw.Elapsed}.");
    }

    [Fact]
    public void LocalTextKeyRules_ToIdentifier_Is_Fast_For_A_Long_Segment_With_Invalid_Characters()
    {
        var segment = "ThisIsAVeryLongLocalTextKeySegmentOfManyWords And Some More-Hint";

        var sw = Stopwatch.StartNew();
        var actual = LocalTextKeyRules.ToIdentifier(segment, out var changed);
        sw.Stop();

        Assert.True(changed);
        Assert.NotNull(actual);
        Assert.True(CSharpSyntaxRules.IsValidIdentifier(actual, ignoreKeywords: true));
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Converting a local text key segment took {sw.Elapsed}.");
    }
}
