namespace Serenity.Data;

public class DatabaseCaretReferencesTests
{
    [Fact]
    public void Replace_Null_Returns_Null()
    {
        Assert.Null(DatabaseCaretReferences.Replace(null));
    }

    [Fact]
    public void Replace_Without_Caret_Returns_Same_Instance()
    {
        var expression = "select 1 from [Table]";

        Assert.Same(expression, DatabaseCaretReferences.Replace(expression));
    }

    [Fact]
    public void Replace_With_Caret_Outside_Brackets_Is_Unchanged()
    {
        Assert.Equal("a ^ b", DatabaseCaretReferences.Replace("a ^ b"));
    }

    [Fact]
    public void Replace_Drops_ConnectionKey_When_No_Database_Name_Resolver()
    {
        var original = DatabaseCaretReferences.SetLocalGetDatabaseName(null);
        try
        {
            Assert.Equal("[Table]", DatabaseCaretReferences.Replace("[Db^Table]"));
        }
        finally
        {
            DatabaseCaretReferences.SetLocalGetDatabaseName(original);
        }
    }

    [Fact]
    public void Replace_Drops_Reference_Without_ConnectionKey_Even_With_Resolver()
    {
        var original = DatabaseCaretReferences.SetLocalGetDatabaseName(key => "ActualDb");
        try
        {
            Assert.Equal("[Table]", DatabaseCaretReferences.Replace("[^Table]"));
        }
        finally
        {
            DatabaseCaretReferences.SetLocalGetDatabaseName(original);
        }
    }

    [Fact]
    public void Replace_Uses_Database_Name_From_Resolver()
    {
        var original = DatabaseCaretReferences.SetLocalGetDatabaseName(key =>
        {
            Assert.Equal("Db", key);
            return "ActualDb";
        });
        try
        {
            Assert.Equal("[ActualDb]", DatabaseCaretReferences.Replace("[Db^Table]"));
        }
        finally
        {
            DatabaseCaretReferences.SetLocalGetDatabaseName(original);
        }
    }

    [Fact]
    public void Replace_Falls_Back_To_Table_When_Resolver_Returns_Empty()
    {
        var original = DatabaseCaretReferences.SetLocalGetDatabaseName(key => "");
        try
        {
            Assert.Equal("[Table]", DatabaseCaretReferences.Replace("[Db^Table]"));
        }
        finally
        {
            DatabaseCaretReferences.SetLocalGetDatabaseName(original);
        }
    }

    [Fact]
    public void Replace_Keeps_Reference_When_Caret_Is_At_End()
    {
        Assert.Equal("[x^]", DatabaseCaretReferences.Replace("[x^]"));
    }

    [Fact]
    public void Replace_Keeps_Lone_Caret_In_Brackets()
    {
        Assert.Equal("[^]", DatabaseCaretReferences.Replace("[^]"));
    }

    [Fact]
    public void Replace_Handles_Multiple_References()
    {
        Assert.Equal("[A].[B]", DatabaseCaretReferences.Replace("[^A].[^B]"));
    }

    [Fact]
    public void Replace_Skips_Nested_Brackets_But_Resolves_Valid_Ones()
    {
        // "[a[^B]]" contains a nested bracket so it is not a valid bracketed
        // identifier and is left untouched; the leading "[^A]" is resolved
        // (falls back to the table name as there is no database name resolver).
        Assert.Equal("[A].[a[^B]]", DatabaseCaretReferences.Replace("[^A].[a[^B]]"));
    }

    [Fact]
    public void Replace_Ignores_Reference_In_Single_Quoted_String()
    {
        Assert.Equal("x = 'a [^B] c'", DatabaseCaretReferences.Replace("x = 'a [^B] c'"));
    }

    [Fact]
    public void Replace_Ignores_Caret_In_Quotes()
    {
        Assert.Equal("'[^x]'", DatabaseCaretReferences.Replace("'[^x]'"));
    }
}
