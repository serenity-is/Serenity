using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Serenity.Extensions.DependencyInjection;

public class ServiceCollectionConfigureExtensionsTests
{
    private class WithoutSectionKey
    {
        public string? Value { get; set; }
    }

    private static IConfiguration CreateConfiguration(params (string Key, string Value)[] values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(x => new KeyValuePair<string, string?>(x.Key, x.Value)))
            .Build();
    }

    [Fact]
    public void ConfigureSection_Throws_When_Config_Is_Null()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() =>
            services.ConfigureSection<UploadSettings>(null!));
    }

    [Fact]
    public void ConfigureSection_Throws_When_Type_Has_No_DefaultSectionKey()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            services.ConfigureSection<WithoutSectionKey>(config));
    }

    [Fact]
    public void ConfigureSection_Binds_Options_Using_Default_Section_Key()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration(("UploadSettings:Path", "/custom/path"));

        services.ConfigureSection<UploadSettings>(config);
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<UploadSettings>>();

        Assert.Equal("/custom/path", options.Value.Path);
    }

    [Fact]
    public void ConfigureSections_Throws_When_Config_Is_Null()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() =>
            services.ConfigureSections(null!, new MockTypeSource()));
    }

    [Fact]
    public void ConfigureSections_Throws_When_No_TypeSource_Provided_Or_Registered()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration();

        Assert.Throws<ArgumentNullException>(() =>
            services.ConfigureSections(config));
    }

    [Fact]
    public void ConfigureSections_Uses_Registered_TypeSource()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITypeSource>(new MockTypeSource(typeof(UploadSettings)));
        var config = CreateConfiguration(("UploadSettings:Path", "/registered"));

        services.ConfigureSections(config);
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<UploadSettings>>();

        Assert.Equal("/registered", options.Value.Path);
    }

    [Fact]
    public void ConfigureSections_Configures_All_Types_With_DefaultSectionKey()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration(
            ("UploadSettings:Path", "/upload"),
            ("Recaptcha:SiteKey", "site-key"));

        services.ConfigureSections(config, new MockTypeSource(typeof(UploadSettings), typeof(RecaptchaSettings)));
        var provider = services.BuildServiceProvider();

        Assert.Equal("/upload", provider.GetRequiredService<IOptions<UploadSettings>>().Value.Path);
        Assert.Equal("site-key", provider.GetRequiredService<IOptions<RecaptchaSettings>>().Value.SiteKey);
    }

    [Fact]
    public void ConfigureSections_Skips_Types_When_Predicate_Returns_False()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration(
            ("UploadSettings:Path", "/upload"),
            ("Recaptcha:SiteKey", "site-key"));

        services.ConfigureSections(config,
            new MockTypeSource(typeof(UploadSettings), typeof(RecaptchaSettings)),
            type => type == typeof(UploadSettings));
        var provider = services.BuildServiceProvider();

        Assert.Equal("/upload", provider.GetRequiredService<IOptions<UploadSettings>>().Value.Path);
        Assert.Null(provider.GetRequiredService<IOptions<RecaptchaSettings>>().Value.SiteKey);
    }

    [Fact]
    public void ConfigureSections_Ignores_Types_Without_DefaultSectionKey()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration(("UploadSettings:Path", "/upload"));

        services.ConfigureSections(config, new MockTypeSource(typeof(UploadSettings), typeof(WithoutSectionKey)));
        var provider = services.BuildServiceProvider();

        Assert.Equal("/upload", provider.GetRequiredService<IOptions<UploadSettings>>().Value.Path);
    }
}
