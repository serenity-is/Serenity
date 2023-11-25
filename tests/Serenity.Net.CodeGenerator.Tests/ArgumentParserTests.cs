using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class ArgumentParserTests
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
        Assert.False(ArgumentParser.IsSwitch(argument));
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
        Assert.True(ArgumentParser.IsSwitch(argument));
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
        Assert.Null(ArgumentParser.ParseSwitch(argument, out _));
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
        Assert.Equal(expectedName, ArgumentParser.ParseSwitch(argument, out string actualValue));
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GetSingleValue_ThrowsArgumentNullException_IfNames_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ArgumentParser.GetSingleValue(["/test"], null));

        Assert.Throws<ArgumentNullException>(() =>
            ArgumentParser.GetSingleValue(["/test"], []));
    }

    [InlineData()]
    [InlineData("\\test", "test")]
    [InlineData("/p:test", "/tes", "/t", "/alternative")]
    [InlineData(":test", "--v", "test")]
    [InlineData("@test")]
    [InlineData("test", "--tes-t", "get-test", "/get-test", "test:test")]
    [Theory]
    public void GetSingleValue_ReturnsNull_IfArgument_IsOmitted(params string[] arguments)
    {
        var list = arguments.ToList();
        Assert.Null(ArgumentParser.GetSingleValue(list, ["test", "alt"]));
    }

    [InlineData("/test")]
    [InlineData("/p:test", "/alt")]
    [InlineData("-p:test", "--test")]
    [InlineData(":test", "/p", "--test", "/val")]
    [InlineData("-alt", "--m", "value")]
    [Theory]
    public void GetSingleValue_ThrowsArgumentException_IfArgument_HasNoValueAssigned(params string[] arguments)
    {
        var list = arguments.ToList();
        Assert.Throws<ArgumentException>(() => 
            ArgumentParser.GetSingleValue(list, ["test", "alt"]));
    }

    [InlineData("/test", "val")]
    [InlineData("/p:test", "/alt", "val")]
    [InlineData("-p:test", "--test", "val", "/x")]
    [InlineData(":test", "/p", "--test", "val", "/val")]
    [InlineData("-alt", "val", "--m", "value")]
    [Theory]
    public void GetSingleValue_Returns_Following_NonSwitch_AsValue_IfNoAssignment(params string[] arguments)
    {
        var list = arguments.ToList();
        Assert.Equal("val", ArgumentParser.GetSingleValue(list, ["test", "alt"]));
        Assert.Equal(arguments.Length - 2, list.Count);
    }
}
