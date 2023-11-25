using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class ArgumentReaderTests
{
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

}
