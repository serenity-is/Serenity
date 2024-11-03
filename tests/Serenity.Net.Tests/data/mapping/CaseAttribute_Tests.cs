namespace Serenity.Tests.Data;

using Case = CaseAttribute;

public class CaseAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_When1_Or_Then1_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Case(null, null));
        Assert.Throws<ArgumentNullException>(() => new Case(null, "TEST"));
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", null));
    }

    [Fact]
    public void Throws_ArgumentNull_If_One_Of_Rest_Expressions_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", "TEST", (string)null));
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", "TEST", null)); // passing rest[] as null!
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", "TEST", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", "TEST", "TEST", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", "TEST", "TEST", "TEST", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new Case("TEST", "TEST", "TEST", null, "TEST", null));
    }

    readonly ISqlDialect dialect = SqlServer2012Dialect.Instance;

    [Fact]
    public void Single_CaseWhen()
    {
        Assert.Equal("(CASE WHEN When1 THEN Then1 END)",
            new Case("When1", "Then1").ToString(dialect));
        
        Assert.Equal("(CASE WHEN When1 THEN SYSDATETIME() END)", 
            new Case("When1", typeof(SqlNowAttribute)).ToString(dialect));
        
        Assert.Equal("(CASE WHEN SYSDATETIME() THEN SYSDATETIME() END)", 
            new Case(typeof(SqlNowAttribute), typeof(SqlNowAttribute)).ToString(dialect));
    }

    [Fact]
    public void Single_CaseWhen_Else()
    {
        Assert.Equal("(CASE WHEN When1 THEN Then1 ELSE Else1 END)",
            new Case("When1", "Then1", "Else1").ToString(dialect));

        Assert.Equal("(CASE WHEN When1 THEN SYSDATETIME() ELSE Else1 END)",
            new Case("When1", typeof(SqlNowAttribute), "Else1").ToString(dialect));

        Assert.Equal("(CASE WHEN SYSDATETIME() THEN SYSDATETIME() ELSE SYSDATETIME() END)",
            new Case(typeof(SqlNowAttribute), typeof(SqlNowAttribute), typeof(SqlNowAttribute)).ToString(dialect));
    }

    [Fact]
    public void Double_CaseWhen()
    {
        Assert.Equal("(CASE WHEN When1 THEN Then1 WHEN When2 THEN Then2 END)",
            new Case("When1", "Then1", "When2", "Then2").ToString(dialect));

        Assert.Equal("(CASE WHEN When1 THEN SYSDATETIME() WHEN SYSUTCDATETIME() THEN Then2 END)",
            new Case("When1", typeof(SqlNowAttribute), typeof(SqlUtcNowAttribute), "Then2").ToString(dialect));

        Assert.Equal("(CASE WHEN SYSUTCDATETIME() THEN SYSDATETIME() WHEN When2 THEN SYSUTCDATETIME() END)",
            new Case(typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute), "When2", typeof(SqlUtcNowAttribute)).ToString(dialect));
    }

    [Fact]
    public void Double_CaseWhen_Else()
    {
        Assert.Equal("(CASE WHEN When1 THEN Then1 WHEN When2 THEN Then2 ELSE Else1 END)",
            new Case("When1", "Then1", "When2", "Then2", "Else1").ToString(dialect));

        Assert.Equal("(CASE WHEN When1 THEN SYSDATETIME() WHEN SYSUTCDATETIME() THEN Then2 ELSE Else1 END)",
            new Case("When1", typeof(SqlNowAttribute), typeof(SqlUtcNowAttribute), "Then2", "Else1").ToString(dialect));

        Assert.Equal("(CASE WHEN SYSUTCDATETIME() THEN SYSDATETIME() WHEN When2 THEN SYSUTCDATETIME() ELSE SYSDATETIME() END)",
            new Case(typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute), "When2", typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute)).ToString(dialect));
    }
}