namespace Serenity.Tests.Data;

using CaseSwitch = CaseSwitchAttribute;

public class CaseSwitchAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Switch_Or_When1_Or_Then1_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch(null, null, "TEST"));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("TEST", null, "TEST"));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("TEST", "TEST", null));
    }

    [Fact]
    public void Throws_ArgumentNull_If_One_Of_Rest_Expressions_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("SWITCH1", "WHEN1", "ELSE1", (string)null));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("SWITCH1", "WHEN1", "ELSE1", null)); // passing rest[] as null!
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("SWITCH1", "WHEN1", "ELSE1", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("SWITCH1", "WHEN1", "ELSE1", "TEST", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("SWITCH1", "WHEN1", "ELSE1", "TEST", "TEST", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new CaseSwitch("SWITCH1", "WHEN1", "ELSE1", "TEST", null, "TEST", null));
    }

    readonly ISqlDialect dialect = SqlServer2012Dialect.Instance;

    [Fact]
    public void Single_CaseWhen()
    {
        Assert.Equal("(CASE Switch1 WHEN When1 THEN Then1 END)",
            new CaseSwitch("Switch1", "When1", "Then1").ToString(dialect));
        
        Assert.Equal("(CASE Switch1 WHEN When1 THEN SYSDATETIME() END)", 
            new CaseSwitch("Switch1", "When1", typeof(SqlNowAttribute)).ToString(dialect));
        
        Assert.Equal("(CASE Switch1 WHEN SYSDATETIME() THEN SYSDATETIME() END)", 
            new CaseSwitch("Switch1", typeof(SqlNowAttribute), typeof(SqlNowAttribute)).ToString(dialect));
    }

    [Fact]
    public void Single_CaseWhen_Else()
    {
        Assert.Equal("(CASE Switch1 WHEN When1 THEN Then1 ELSE Else1 END)",
            new CaseSwitch("Switch1", "When1", "Then1", "Else1").ToString(dialect));

        Assert.Equal("(CASE Switch1 WHEN When1 THEN SYSDATETIME() ELSE Else1 END)",
            new CaseSwitch("Switch1", "When1", typeof(SqlNowAttribute), "Else1").ToString(dialect));

        Assert.Equal("(CASE SYSUTCDATETIME() WHEN SYSDATETIME() THEN SYSDATETIME() ELSE SYSDATETIME() END)",
            new CaseSwitch(typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute), typeof(SqlNowAttribute), typeof(SqlNowAttribute)).ToString(dialect));
    }

    [Fact]
    public void Double_CaseWhen()
    {
        Assert.Equal("(CASE Switch1 WHEN When1 THEN Then1 WHEN When2 THEN Then2 END)",
            new CaseSwitch("Switch1", "When1", "Then1", "When2", "Then2").ToString(dialect));

        Assert.Equal("(CASE Switch1 WHEN When1 THEN SYSDATETIME() WHEN SYSUTCDATETIME() THEN Then2 END)",
            new CaseSwitch("Switch1", "When1", typeof(SqlNowAttribute), typeof(SqlUtcNowAttribute), "Then2").ToString(dialect));

        Assert.Equal("(CASE SYSUTCDATETIME() WHEN SYSUTCDATETIME() THEN SYSDATETIME() WHEN When2 THEN SYSUTCDATETIME() END)",
            new CaseSwitch(typeof(SqlUtcNowAttribute), typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute), "When2", typeof(SqlUtcNowAttribute)).ToString(dialect));
    }

    [Fact]
    public void Double_CaseWhen_Else()
    {
        Assert.Equal("(CASE Switch1 WHEN When1 THEN Then1 WHEN When2 THEN Then2 ELSE Else1 END)",
            new CaseSwitch("Switch1", "When1", "Then1", "When2", "Then2", "Else1").ToString(dialect));

        Assert.Equal("(CASE Switch1 WHEN When1 THEN SYSDATETIME() WHEN SYSUTCDATETIME() THEN Then2 ELSE Else1 END)",
            new CaseSwitch("Switch1", "When1", typeof(SqlNowAttribute), typeof(SqlUtcNowAttribute), "Then2", "Else1").ToString(dialect));

        Assert.Equal("(CASE SYSUTCDATETIME() WHEN SYSUTCDATETIME() THEN SYSDATETIME() WHEN When2 THEN SYSUTCDATETIME() ELSE SYSDATETIME() END)",
            new CaseSwitch(typeof(SqlUtcNowAttribute), typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute), "When2", typeof(SqlUtcNowAttribute), typeof(SqlNowAttribute)).ToString(dialect));
    }
}