using Serenity.Localization;

namespace Serenity.Web;

public class FormScriptRegistrationTests
{
    [FormScript("Registration.Form")]
    private class TestFormType
    {
    }

    [Fact]
    public void RegisterFormScripts_Throws_For_Null_Arguments()
    {
        var manager = new MockDynamicScriptManager();
        var typeSource = new MockTypeSource();
        var provider = new MockPropertyItemProvider();
        var services = new ServiceCollection().BuildServiceProvider();

        Assert.Throws<ArgumentNullException>(() =>
            FormScriptRegistration.RegisterFormScripts(null!, typeSource, provider, services));
        Assert.Throws<ArgumentNullException>(() =>
            FormScriptRegistration.RegisterFormScripts(manager, null!, provider, services));
        Assert.Throws<ArgumentNullException>(() =>
            FormScriptRegistration.RegisterFormScripts(manager, typeSource, provider, null!));
    }

    [Fact]
    public void RegisterFormScripts_Registers_Scripts_And_Bundle()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(TestFormType)), new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Equal("Form.Registration.Form", scripts[0].ScriptName);
        Assert.True(manager.IsRegistered("Form.Registration.Form"));
        Assert.True(manager.IsRegistered("FormBundle"));
    }

    [Fact]
    public void RegisterFormScripts_Uses_FullName_When_No_Key()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(NoKeyFormType)), new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Contains(nameof(NoKeyFormType), scripts[0].ScriptName);
    }

    [FormScript]
    private class NoKeyFormType
    {
    }

    [Fact]
    public void RegisterFormScripts_Uses_Inferred_Key_When_InferKey()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(Acme.Default.Forms.VersionInfoForm)),
            new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Equal("Form.Default.VersionInfo", scripts[0].ScriptName);
        Assert.True(manager.IsRegistered("Form.Default.VersionInfo"));
        Assert.False(manager.IsRegistered("Form." + typeof(Acme.Default.Forms.VersionInfoForm).FullName));
    }

    [Fact]
    public void RegisterFormScripts_Uses_FullName_When_InferKey_Is_False()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(Acme.Default.Forms.ExplicitFalseForm)),
            new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Equal("Form." + typeof(Acme.Default.Forms.ExplicitFalseForm).FullName,
            scripts[0].ScriptName);
    }

    [Fact]
    public void RegisterFormScripts_Texts_Use_The_Inferred_Key()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var type = typeof(Acme.Default.Forms.VersionInfoForm);
        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(type), new MockPropertyItemProvider(), services).ToList();

        // field texts use the Forms. prefix (unlike the Form. script key), but the
        // same inferred key: neither may fall back to the full type name
        Assert.Equal("Form.Default.VersionInfo", scripts[0].ScriptName);
        Assert.Equal("Forms.Default.VersionInfo.",
            PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(type));
    }
}
