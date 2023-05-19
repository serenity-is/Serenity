using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Serenity.Tests.Web;

public partial class DefaultUploadValidatorTests
{
    [Fact]
    public void Uses_Blacklist_And_Whitelist()
    {
        var settings = new UploadSettings
        {
            ExtensionBlacklist = ".xlsx",
            ExtensionWhitelist = ".docx"
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(), 
            NullTextLocalizer.Instance, null, Options.Create(settings));

        var ex = Assert.Throws<ValidationError>(() => validator.ValidateFile(
            new UploadOptions(), new MemoryStream(), "test.xlsx", out _));

        Assert.Equal("ExtensionInBlacklist", ex.ErrorCode);

        ex = Assert.Throws<ValidationError>(() => validator.ValidateFile(
            new UploadOptions(), new MemoryStream(), "test.txt", out _));

        Assert.Equal("ExtensionNotInWhitelist", ex.ErrorCode);

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.docx", out _);
    }

    [Fact]
    public void Can_Extend_Default_Blacklist_Via_Include()
    {
        var settings = new UploadSettings
        {
            ExtensionBlacklistInclude = ".xlsx"
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        var ex = Assert.Throws<ValidationError>(() => validator.ValidateFile(
            new UploadOptions(), new MemoryStream(), "test.xlsx", out _));

        Assert.Equal("ExtensionInBlacklist", ex.ErrorCode);

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.txt", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.docx", out _);
    }

    [Fact]
    public void Can_Remove_From_Blacklist_Via_Exclude()
    {
        var settings = new UploadSettings
        {
            ExtensionBlacklistExclude = ".htm;.exe",
            ExtensionWhitelist = ""
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.htm", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.exe", out _);
    }

    [Fact]
    public void Can_Disable_Whitelist_By_Setting_To_Empty()
    {
        var settings = new UploadSettings
        {
            ExtensionWhitelist = ""
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.some", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.another", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.xlsx", out _);
    }


    [Fact]
    public void Can_Disable_Whitelist_By_Setting_To_Null()
    {
        var settings = new UploadSettings
        {
            ExtensionWhitelist = null
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.some", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.another", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.xlsx", out _);
    }

    [Fact]
    public void Can_Disable_Blacklist_By_Setting_To_Empty()
    {
        var settings = new UploadSettings
        {
            ExtensionBlacklist = "",
            ExtensionWhitelist = ".exe;.another;.xlsx"
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.exe", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.another", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.xlsx", out _);
    }

    [Fact]
    public void Can_Disable_Blacklist_By_Setting_To_Null()
    {
        var settings = new UploadSettings
        {
            ExtensionBlacklist = null,
            ExtensionWhitelist = ".exe;.another;.xlsx"
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.exe", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.another", out _);
        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.xlsx", out _);
    }

    [Fact]
    public void Excluding_From_Blacklist_Does_Not_Add_It_To_Whitelist()
    {
        var settings = new UploadSettings
        {
            ExtensionBlacklistExclude = ".exe;.com",
            ExtensionWhitelistInclude = ".com"
        };

        var validator = new DefaultUploadValidator(new MockImageProcessor(),
            NullTextLocalizer.Instance, null, Options.Create(settings));

        var ex = Assert.Throws<ValidationError>(() => validator.ValidateFile(
            new UploadOptions(), new MemoryStream(), "test.exe", out _));

        Assert.Equal("ExtensionNotInWhitelist", ex.ErrorCode);

        validator.ValidateFile(new UploadOptions(), new MemoryStream(), "test.com", out _);
    }
}