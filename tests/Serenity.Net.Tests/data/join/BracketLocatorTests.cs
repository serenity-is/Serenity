namespace Serenity.Data;

public class BracketLocatorTests
{
    [Fact]
    public void ReplaceBracketContents_Null_Returns_Null()
    {
        Assert.Null(BracketLocator.ReplaceBracketContents(null, '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Empty_Returns_Empty()
    {
        Assert.Equal("", BracketLocator.ReplaceBracketContents("", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Without_Brackets_Is_Unchanged()
    {
        Assert.Equal("a + b = c",
            BracketLocator.ReplaceBracketContents("a + b = c", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Replaces_Simple_Bracket()
    {
        Assert.Equal("[ABC]",
            BracketLocator.ReplaceBracketContents("[abc]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Replaces_All_Brackets()
    {
        Assert.Equal("a.[X] + b.[Y]",
            BracketLocator.ReplaceBracketContents("a.[x] + b.[y]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Empty_Brackets_Are_Unchanged()
    {
        Assert.Equal("[]",
            BracketLocator.ReplaceBracketContents("[]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_With_Invalid_Char_After_Start_Is_Unchanged()
    {
        Assert.Equal("x [a b]",
            BracketLocator.ReplaceBracketContents("x [a b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_With_Quote_Inside_Is_Unchanged()
    {
        Assert.Equal("[a'b]",
            BracketLocator.ReplaceBracketContents("[a'b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Ignores_Brackets_In_Quotes()
    {
        Assert.Equal("x = 'a [b] c'",
            BracketLocator.ReplaceBracketContents("x = 'a [b] c'", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Allows_ValidChar1_In_Brackets()
    {
        Assert.Equal("x [^Y]",
            BracketLocator.ReplaceBracketContents("x [^y]", '^', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Invalid_ValidChar1_In_Brackets_Is_Unchanged()
    {
        Assert.Equal("x [^y]",
            BracketLocator.ReplaceBracketContents("x [^y]", ' ', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Digits_Are_Valid_In_Brackets()
    {
        Assert.Equal("[X]",
            BracketLocator.ReplaceBracketContents("[123]", '_', s => "X"));
    }

    [Fact]
    public void ReplaceBracketContents_Underscore_Is_Valid_In_Brackets()
    {
        Assert.Equal("[A_B]",
            BracketLocator.ReplaceBracketContents("[a_b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Nested_Brackets_Are_Unchanged()
    {
        // A nested '[' means the outer bracket contents can't be a valid
        // identifier, so the whole fragment is passed through untouched.
        Assert.Equal("[a[b]]",
            BracketLocator.ReplaceBracketContents("[a[b]]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Nested_Bracket_DoesNot_Affect_Later_Valid_Bracket()
    {
        Assert.Equal("[a[b] [C]",
            BracketLocator.ReplaceBracketContents("[a[b] [c]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Bracket_At_Start_With_Invalid_Char_Is_Unchanged()
    {
        Assert.Equal("[a b]",
            BracketLocator.ReplaceBracketContents("[a b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Valid_Bracket_After_Invalid_Is_Replaced()
    {
        Assert.Equal("x [a b] [C]",
            BracketLocator.ReplaceBracketContents("x [a b] [c]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Single_Char_Contents_Are_Replaced()
    {
        Assert.Equal("[A]",
            BracketLocator.ReplaceBracketContents("[a]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Closing_Bracket_Without_Opening_Is_Unchanged()
    {
        Assert.Equal("x ] y [B]",
            BracketLocator.ReplaceBracketContents("x ] y [b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Trailing_Bracket_After_Replacement_Is_Unchanged()
    {
        // a ']' right after a replaced bracket does not start a new replacement
        Assert.Equal("[A]b]",
            BracketLocator.ReplaceBracketContents("[a]b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Ignores_Brackets_After_Doubled_Quote()
    {
        // 'it''s [a b]' is a single string literal containing [a b]
        Assert.Equal("x = 'it''s [a b]'",
            BracketLocator.ReplaceBracketContents("x = 'it''s [a b]'", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Replaces_After_Quoted_String()
    {
        Assert.Equal("x = 'q' [B]",
            BracketLocator.ReplaceBracketContents("x = 'q' [b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Unclosed_Quote_Skips_Rest()
    {
        Assert.Equal("x = 'abc [b]",
            BracketLocator.ReplaceBracketContents("x = 'abc [b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Bracket_Opened_Before_Quote_Is_Unchanged()
    {
        // the quote invalidates the bracket; the ']' inside the quoted
        // section does not close it
        Assert.Equal("x [a' b]",
            BracketLocator.ReplaceBracketContents("x [a' b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Ignores_Brackets_In_Double_Quoted_Text()
    {
        // double quotes are identifier quoting in Postgres/Oracle/Firebird
        // and string delimiters in T-SQL, so their contents are skipped
        Assert.Equal("x = \"a [b] c\"",
            BracketLocator.ReplaceBracketContents("x = \"a [b] c\"", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Ignores_Brackets_In_Backtick_Quoted_Text()
    {
        Assert.Equal("x = `a [b] c`",
            BracketLocator.ReplaceBracketContents("x = `a [b] c`", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Apostrophe_In_Double_Quotes_Does_Not_End_Quote()
    {
        // only the same quote character ends a quoted section
        Assert.Equal("x = \"it's [b] ok\"",
            BracketLocator.ReplaceBracketContents("x = \"it's [b] ok\"", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Double_Quote_In_Single_Quotes_Does_Not_End_Quote()
    {
        Assert.Equal("x = 'say \"hi\" [b]'",
            BracketLocator.ReplaceBracketContents("x = 'say \"hi\" [b]'", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Backtick_In_Single_Quotes_Does_Not_End_Quote()
    {
        Assert.Equal("x = 'a `b` [b]'",
            BracketLocator.ReplaceBracketContents("x = 'a `b` [b]'", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Replaces_After_Double_Quoted_String()
    {
        Assert.Equal("x = \"q\" [B]",
            BracketLocator.ReplaceBracketContents("x = \"q\" [b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_Unclosed_Double_Quote_Skips_Rest()
    {
        Assert.Equal("x = \"abc [b]",
            BracketLocator.ReplaceBracketContents("x = \"abc [b]", '_', s => s.ToUpperInvariant()));
    }

    [Fact]
    public void ReplaceBracketContents_With_Same_Replacement_Is_Unchanged()
    {
        Assert.Equal("[abc]",
            BracketLocator.ReplaceBracketContents("[abc]", '_', s => s));
    }

    [Fact]
    public void ReplaceBrackets_Null_Returns_Null()
    {
        Assert.Null(BracketLocator.ReplaceBrackets(null, SqliteDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Empty_Returns_Empty()
    {
        Assert.Equal("", BracketLocator.ReplaceBrackets("", SqliteDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Quotes_Identifier_With_Dialect()
    {
        Assert.Equal("\"Field\"",
            BracketLocator.ReplaceBrackets("[Field]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Already_Quoted_Identifier_For_Sqlite()
    {
        Assert.Equal("[Field]",
            BracketLocator.ReplaceBrackets("[Field]", SqliteDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Quotes_Identifier_With_Space()
    {
        Assert.Equal("\"My Field\"",
            BracketLocator.ReplaceBrackets("[My Field]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Array_Indexer_Like_Expressions()
    {
        Assert.Equal("a[5]",
            BracketLocator.ReplaceBrackets("a[5]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Bracket_Without_Closing()
    {
        Assert.Equal("[abc",
            BracketLocator.ReplaceBrackets("[abc", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Bracket_Followed_By_Identifier_Char()
    {
        Assert.Equal("[abc]d",
            BracketLocator.ReplaceBrackets("[abc]d", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Empty_Brackets()
    {
        Assert.Equal("[]",
            BracketLocator.ReplaceBrackets("[]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Numeric_Brackets()
    {
        Assert.Equal("[5]",
            BracketLocator.ReplaceBrackets("[5]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Brackets_Containing_Quote()
    {
        Assert.Equal("[a'b]",
            BracketLocator.ReplaceBrackets("[a'b]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Keeps_Nested_Brackets()
    {
        Assert.Equal("[a[b]]",
            BracketLocator.ReplaceBrackets("[a[b]]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Ignores_Brackets_In_Quotes()
    {
        Assert.Equal("x = 'a [b] c'",
            BracketLocator.ReplaceBrackets("x = 'a [b] c'", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Quotes_Identifier_After_Dot()
    {
        Assert.Equal("a.\"Field\" = 5",
            BracketLocator.ReplaceBrackets("a.[Field] = 5", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Ignores_Brackets_In_Double_Quotes()
    {
        Assert.Equal("\"a\" = \"x [b] y\"",
            BracketLocator.ReplaceBrackets("[a] = \"x [b] y\"", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Ignores_Brackets_In_Backticks()
    {
        Assert.Equal("\"a\" = `x [b] y`",
            BracketLocator.ReplaceBrackets("[a] = `x [b] y`", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Apostrophe_In_Double_Quotes_Does_Not_End_Quote()
    {
        Assert.Equal("\"a\" = \"it's [b]\"",
            BracketLocator.ReplaceBrackets("[a] = \"it's [b]\"", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Double_Quote_In_Single_Quotes_Does_Not_End_Quote()
    {
        Assert.Equal("\"a\" = 'say \"hi\" [b]'",
            BracketLocator.ReplaceBrackets("[a] = 'say \"hi\" [b]'", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Quotes_After_Double_Quoted_String()
    {
        Assert.Equal("\"a\" = \"q\" + \"b\"",
            BracketLocator.ReplaceBrackets("[a] = \"q\" + [b]", PostgresDialect.Instance));
    }

    [Fact]
    public void ReplaceBrackets_Unclosed_Double_Quote_Keeps_Rest()
    {
        Assert.Equal("\"a\" = \"x [b] y",
            BracketLocator.ReplaceBrackets("[a] = \"x [b] y", PostgresDialect.Instance));
    }
}
