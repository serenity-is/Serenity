using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class ArgumentReaderTests
{
    [Fact]
    public void ThrowsArgumentNull_IfArgumentsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ArgumentReader(null));
    }

    [Fact]
    public void GetString_ThrowsArgumentNullException_IfNames_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ArgumentReader(["/test"]).GetString(null));

        Assert.Throws<ArgumentNullException>(() =>
            new ArgumentReader(["/test"]).GetString([]));
    }

    [InlineData()]
    [InlineData("\\test", "test")]
    [InlineData("/p:test", "/tes", "/t", "/alternative")]
    [InlineData(":test", "--v", "test")]
    [InlineData("@test")]
    [InlineData("test", "--tes-t", "get-test", "/get-test", "test:test")]
    [Theory]
    public void GetString_ReturnsNull_IfArgument_IsOmitted(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Null(reader.GetString(["test", "alt"]));
    }

    [InlineData("/test")]
    [InlineData("/p:test", "/alt")]
    [InlineData("-p:test", "--test")]
    [InlineData(":test", "/p", "--test", "/val")]
    [InlineData("-alt", "--m", "value")]
    [Theory]
    public void GetString_ThrowsArgumentException_IfArgument_HasNoValueAssigned(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() =>
            reader.GetString(["test", "alt"]));
    }

    [InlineData("/test:")]
    [InlineData("/alt", "")]
    [InlineData("/test:1", "/alt:", "test:")]
    [Theory()]
    public void GetString_ThrowsArgumentException_IfAnyValue_IsEmpty(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() =>
            reader.GetString(["test", "alt"]));
    }

    [InlineData("/test:")]
    [InlineData("/alt", "")]
    [InlineData("/alt:", "/test", "")]
    [Theory()]
    public void GetString_AllowsEmptyStrings_IfRequired_IsFalse(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Empty(reader.GetString(["test", "alt"], required: false));
    }

    [InlineData("/test", "val")]
    [InlineData("/p:test", "/alt", "val")]
    [InlineData("-p:test", "--test", "val", "/x")]
    [InlineData(":test", "/p", "--test", "val", "/val")]
    [InlineData("-alt", "val", "--m", "value")]
    [Theory]
    public void GetString_Returns_Following_NonSwitch_AsValue_IfNoAssignment(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Equal("val", reader.GetString(["test", "alt"]));
        Assert.Equal(arguments.Length - 2, reader.Remaining);
    }

    [InlineData("/test:val")]
    [InlineData("/test=val")]
    [InlineData("-p:test", "--test:val")]
    [InlineData(":test", "/p", "--test=val")]
    [InlineData("-alt=val", "--m", "value")]
    [Theory]
    public void GetString_CanUse_EqualOrColon_AsValueSeparator(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Equal("val", reader.GetString(["test", "alt"]));
        Assert.Equal(arguments.Length - 1, reader.Remaining);
    }

    [InlineData("/test:val", "--test:val2")]
    [InlineData("/test=val", "--test:val2", "/test:val3")]
    [InlineData("-test:val1", "-p:test", "--test:val2")]
    [InlineData(":test", "/test", "val1", "--test=val2")]
    [InlineData("-alt=val", "--m", "--test:val2")]
    [Theory]
    public void GetString_ThrowsArgumentException_IfMultipleValues(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        var exception = Assert.Throws<ArgumentException>(() => reader.GetString(["test", "alt"]));
        Assert.Contains("once", exception.Message);
    }

    [InlineData("/test:val", "--test:val")]
    [InlineData("/test=val", "--test:val", "/test:val")]
    [InlineData("-test:val", "-p:test", "--test:val")]
    [InlineData(":test", "/test", "val", "--test=val")]
    [InlineData("-alt=val", "--m", "--test:val")]
    [Theory]
    public void GetString_DoesNotThrow_IfMultipleValues_AreAllSame(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Equal("val", reader.GetString(["test", "alt"]));
    }

    [Fact]
    public void GetStrings_ThrowsArgumentNull_IfNames_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ArgumentReader(["/test"]).GetStrings(null));

        Assert.Throws<ArgumentNullException>(() =>
            new ArgumentReader(["/test"]).GetStrings([]));
    }

    [InlineData("/test:")]
    [InlineData("/alt", "")]
    [InlineData("/test:1", "/alt:", "test:")]
    [Theory()]
    public void GetStrings_ThrowsArgumentException_IfAnyValue_IsEmpty(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() =>
            reader.GetStrings(["test", "alt"]));
    }

    [InlineData()]
    [InlineData("\\test", "test")]
    [InlineData("/p:test", "/tes", "/t", "/alternative")]
    [InlineData(":test", "--v", "test")]
    [InlineData("@test")]
    [InlineData("test", "--tes-t", "get-test", "/get-test", "test:test")]
    [Theory]
    public void GetStrings_ReturnsEmptyArray_IfArgument_IsOmitted(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Empty(reader.GetStrings(["test", "alt"]));
    }

    [InlineData("/test")]
    [InlineData("/test:b", "/p:test", "/alt")]
    [InlineData("-p:test", "--test")]
    [InlineData(":test", "/p", "val=4", "--test", "/val")]
    [InlineData("-alt", "--m", "value")]
    [Theory]
    public void GetStrings_ThrowsArgumentException_IfArgument_HasNoValueAssigned(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() =>
            reader.GetStrings(["test", "alt"]));
    }

#pragma warning disable CA1861 // Avoid constant arrays as arguments
    [InlineData(new string[] { "/test", "val" }, new string[] { "val" }, 2)]
    [InlineData(new string[] { "/p:test", "/alt", "val1" }, new string[] { "val1" }, 2)]
    [InlineData(new string[] { "-test:val1", "--test", "val2", "/x" }, new string[] { "val1", "val2" }, 3)]
    [InlineData(new string[] { ":test", "/p", "--test", "val2", "/test", "val1" }, 
        new string[] { "val2", "val1" }, 4)]
    [InlineData(new string[] { "-alt", "val", "--m", "value" }, new string[] { "val" }, 2)]
    [Theory]
    public void GetStrings_Returns_Following_NonSwitch_AsValue_IfNoAssignment(
        string[] arguments, string[] expected, int minus)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Equal(expected, reader.GetStrings(["test", "alt"]));
        Assert.Equal(arguments.Length - minus, reader.Remaining);
    }
#pragma warning restore CA1861 // Avoid constant arrays as arguments

    [InlineData(null)]
    [InlineData("")]
    [InlineData("\\test")]
    [InlineData("test")]
    [InlineData(":test")]
    [InlineData("@test")]
    [InlineData("/")]
    [InlineData("/a/b")]
    [Theory]
    public void IsSwitch_Returns_False_ForNonSwitches(string argument)
    {
        Assert.False(ArgumentReader.IsSwitch(argument));
    }

    [InlineData("/test")]
    [InlineData("-test")]
    [InlineData("--test")]
    [InlineData("--test-p")]
    [InlineData("-test-param")]
    [InlineData("/test-param")]
    [InlineData("--test-param")]
    [InlineData("--test-param:value")]
    [InlineData("--test-param=value")]
    [InlineData("/test-param:value")]
    [InlineData("/test-param=value")]
    [InlineData("-test-param:value")]
    [InlineData("-test-param=value")]
    [Theory]
    public void IsSwitch_Returns_True_ForSwitches(string argument)
    {
        Assert.True(ArgumentReader.IsSwitch(argument));
    }

    [InlineData("")]
    [InlineData("\\test")]
    [InlineData("test")]
    [InlineData(":test")]
    [InlineData("@test")]
    [InlineData("/")]
    [InlineData("/a/b")]
    [Theory]
    public void ParseSwitch_ReturnsNull_ForNonSwitches(string argument)
    {
        Assert.Null(ArgumentReader.ParseSwitch(argument, out _));
    }

    [InlineData("/test", "test", null)]
    [InlineData("-test", "test", null)]
    [InlineData("--test", "test", null)]
    [InlineData("--test-p", "test-p", null)]
    [InlineData("-test-param", "test-param", null)]
    [InlineData("/test-param", "test-param", null)]
    [InlineData("--test-param", "test-param", null)]
    [InlineData("--test-param:value", "test-param", "value")]
    [InlineData("--test-param:", "test-param", "")]
    [InlineData("--test-param=", "test-param", "")]
    [InlineData("/test-param:value", "test-param", "value")]
    [InlineData("/test-param=value", "test-param", "value")]
    [InlineData("-test-param:value", "test-param", "value")]
    [InlineData("-test-param=value", "test-param", "value")]
    [Theory]
    public void ParseSwitch_Returns_ExpectedName_And_Value_ForSwitches(string argument,
        string expectedName, string expectedValue)
    {
        Assert.Equal(expectedName, ArgumentReader.ParseSwitch(argument, out string actualValue));
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void ThrowIfRemaining_ThrowsArgumentException_IfAny_RemainingArguments()
    {
        Assert.Throws<ArgumentException>(() =>
            new ArgumentReader(["/test"]).ThrowIfRemaining());

        var reader = new ArgumentReader(["/test", "value"]);
        Assert.Throws<ArgumentException>(() => reader.ThrowIfRemaining());
        Assert.Equal("value", reader.GetString(["test"]));
        reader.ThrowIfRemaining();
    }

    [Fact]
    public void GetDictionary_ThrowsArgumentNull_IfNames_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ArgumentReader(["/test:a=b"]).GetDictionary(null));

        Assert.Throws<ArgumentNullException>(() =>
            new ArgumentReader(["/test:a=b"]).GetStrings([]));
    }

    [InlineData("/test:")]
    [InlineData("/alt", "")]
    [InlineData("/test:1", "/alt:", "test:")]
    [Theory()]
    public void GetDictionary_ThrowsArgumentException_IfAnyValue_IsEmpty(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() =>
            reader.GetStrings(["test", "alt"]));
    }

    [InlineData("/test")]
    [InlineData("/test:b=c", "/p:test", "/alt")]
    [InlineData("-p:test", "--test")]
    [InlineData(":test", "/p", "val=4", "--test", "/val")]
    [InlineData("-alt", "--m", "value")]
    [Theory]
    public void GetDictionary_ThrowsArgumentException_IfArgument_HasNoValueAssigned(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() =>
            reader.GetDictionary(["test", "alt"]));
    }

    [InlineData(2, "/test", "a=b")]
    [InlineData(2, "/p:test", "/alt", "a=b")]
    [InlineData(3, "-test:a=b", "--test", "c=d", "/x")]
    [InlineData(4, ":test", "/p", "--test", "c=d", "/test", "a=b")]
    [InlineData(2, "-alt", "a=b", "--m", "value")]
    [Theory]
    public void GetDictionary_Returns_Following_NonSwitch_AsValue_IfNoAssignment(
        int minus, params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        var actual = reader.GetDictionary(["test", "alt"]);
        Assert.Equal(arguments.Length - minus, reader.Remaining);
        Assert.Equal(minus > 2 ? 2 : 1, actual.Count);
        Assert.Equal("b", actual["a"]);
        if (minus > 2)
            Assert.Equal("d", actual["c"]);
    }

    [InlineData()]
    [InlineData("\\test", "test")]
    [InlineData("/p:test", "/tes", "/t", "/alternative")]
    [InlineData(":test", "--v", "test")]
    [InlineData("@test")]
    [InlineData("test", "--tes-t", "get-test", "/get-test", "test:test")]
    [Theory]
    public void GetDictionary_ReturnsEmptyDictionary_IfArgument_IsOmitted(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Empty(reader.GetDictionary(["test", "alt"]));
    }

    [Fact]
    public void GetDictionary_DoesNotSplitArguments_IfSeparatorsIsNull()
    {
        var reader = new ArgumentReader(["/test:a=b;c=d", "--alt", "e=f;g=h"]);
        var actual = reader.GetDictionary(["test", "alt"], separators: null);
        Assert.Equal(new Dictionary<string, string>
        {
            ["a"] = "b;c=d",
            ["e"] = "f;g=h",
        }, actual);
    }

    [Fact]
    public void GetDictionary_DoesNotSplitArguments_IfSeparatorsIsEmpty()
    {
        var reader = new ArgumentReader(["/test:a=b;c=d", "--alt", "e=f;g=h"]);
        var actual = reader.GetDictionary(["test", "alt"], separators: []);
        Assert.Equal(new Dictionary<string, string>
        {
            ["a"] = "b;c=d",
            ["e"] = "f;g=h",
        }, actual);
    }

    [Fact]
    public void GetDictionary_SplitArguments_IfSeparatorsIsSpecified()
    {
        var reader = new ArgumentReader(["/test:a=b;c=d", "--alt", "e=f;g=h"]);
        var actual = reader.GetDictionary(["test", "alt"], separators: [';']);
        Assert.Equal(new Dictionary<string, string>
        {
            ["a"] = "b",
            ["c"] = "d",
            ["e"] = "f",
            ["g"] = "h",
        }, actual);
    }

    [InlineData("/test:a")]
    [InlineData("/test:a;b")]
    [InlineData("/test:a=b;c")]
    [InlineData("/test:a=b", "--alt", "c")]
    [Theory]
    public void GetDictionary_ThrowsArgumentException_IfEqualSignNotFound(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() => reader.GetDictionary(["test", "alt"], separators: [';']));
    }

    [InlineData("/test:=a")]
    [InlineData("/test:a=b;=c")]
    [InlineData("/test:a=b", "--alt", "=c")]
    [Theory]
    public void GetDictionary_ThrowsArgumentException_IfOneOfKeysIsEmpty(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() => reader.GetDictionary(["test", "alt"], separators: [';']));
    }

    [InlineData("/test:a=b", "/test", "a=c")]
    [InlineData("--alt", "a=b", "--test", "a=c")]
    [Theory]
    public void GetDictionary_ThrowsArgumentException_IfValueForKeySpecifiedMultipleTimes(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Throws<ArgumentException>(() => reader.GetDictionary(["test", "alt"], separators: [';']));
    }

    [InlineData("/test:a=b", "/test", "c=d,a=b")]
    [InlineData("--alt", "a=b,c=d", "--test", "a=b")]
    [Theory]
    public void GetDictionary_DoesNotThrow_IfValueForKeySpecifiedMultipleTimes_HasSameValue(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        var actual = reader.GetDictionary(["test", "alt"], separators: [',']);
        Assert.Equal(new Dictionary<string, string>
        {
            ["a"] = "b",
            ["c"] = "d"
        }, actual);
    }

    [InlineData("/?")]
    [InlineData("/h")]
    [InlineData("/help")]
    [InlineData("--help")]
    [InlineData("command", "--help")]
    [InlineData("/l", "--help")]
    [Theory]
    public void HasHelpSwitch_ReturnsTrue_IfAnyHelpSwitchExists(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.True(reader.HasHelpSwitch());
    }

    [InlineData("?")]
    [InlineData("h")]
    [InlineData("help")]
    [InlineData("--he", "hel")]
    [InlineData("command", "--hel")]
    [InlineData("/d", "help", "/l")]
    [Theory]
    public void HasHelpSwitch_ReturnsFalse_IfNoHelpSwitchExists(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.False(reader.HasHelpSwitch());
    }

    [InlineData("/s", "cmd")]
    [InlineData("cmd", "/s")]
    [InlineData("--s", "-k", "cmd")]
    [InlineData("--he", "cmd")]
    [InlineData("cmd")]
    [InlineData("/d", "-v", "/l", "cmd")]
    [Theory]
    public void GetCommand_Returns_FirstArgument_ThatIsNotASwitch(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Equal("cmd", reader.GetCommand());
    }

    [InlineData("/cmd")]
    [InlineData("/s", "/cmd")]
    [InlineData("--cmd", "/s")]
    [InlineData("--s", "-k", "--cmd")]
    [InlineData("--he", "", "--cmd")]
    [InlineData("/d", "-v", "/l", "", "-cmd")]
    [Theory]
    public void GetCommand_Returns_Null_If_AllAreSwitches(params string[] arguments)
    {
        var reader = new ArgumentReader(arguments);
        Assert.Null(reader.GetCommand());
    }
}
