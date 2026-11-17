using Serenity.Localization;

namespace Serenity.Web;

public class ColumnsScriptRegistrationTests
{
    [ColumnsScript("Registration.Columns")]
    private class TestColumnsType
    {
    }

    [Fact]
    public void RegisterColumnsScripts_Throws_For_Null_Arguments()
    {
        var manager = new MockDynamicScriptManager();
        var typeSource = new MockTypeSource();
        var provider = new MockPropertyItemProvider();
        var services = new ServiceCollection().BuildServiceProvider();

        Assert.Throws<ArgumentNullException>(() =>
            ColumnsScriptRegistration.RegisterColumnsScripts(null!, typeSource, provider, services));
        Assert.Throws<ArgumentNullException>(() =>
            ColumnsScriptRegistration.RegisterColumnsScripts(manager, null!, provider, services));
        Assert.Throws<ArgumentNullException>(() =>
            ColumnsScriptRegistration.RegisterColumnsScripts(manager, typeSource, provider, null!));
    }

    [Fact]
    public void RegisterColumnsScripts_Registers_Scripts_And_Bundle()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = ColumnsScriptRegistration.RegisterColumnsScripts(manager,
            new MockTypeSource(typeof(TestColumnsType)), new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Equal("Columns.Registration.Columns", scripts[0].ScriptName);
        Assert.True(manager.IsRegistered("Columns.Registration.Columns"));
        Assert.True(manager.IsRegistered("ColumnsBundle"));
    }

    [Fact]
    public void RegisterColumnsScripts_Uses_FullName_When_No_Key()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = ColumnsScriptRegistration.RegisterColumnsScripts(manager,
            new MockTypeSource(typeof(NoKeyColumnsType)), new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Contains(nameof(NoKeyColumnsType), scripts[0].ScriptName);
    }

    [ColumnsScript]
    private class NoKeyColumnsType
    {
    }

    [Fact]
    public void RegisterColumnsScripts_Uses_Inferred_Key_When_InferKey()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = ColumnsScriptRegistration.RegisterColumnsScripts(manager,
            new MockTypeSource(typeof(Acme.Default.Columns.VersionInfoColumns)),
            new MockPropertyItemProvider(), services).ToList();

        // generated code is InferKey = true, and the client asks for exactly this
        // key: the root (project) namespace and the Columns suffix are not in it
        Assert.Single(scripts);
        Assert.Equal("Columns.Default.VersionInfo", scripts[0].ScriptName);
        Assert.True(manager.IsRegistered("Columns.Default.VersionInfo"));
        Assert.False(manager.IsRegistered("Columns." + typeof(Acme.Default.Columns.VersionInfoColumns).FullName));
    }

    [Fact]
    public void RegisterColumnsScripts_Uses_FullName_When_InferKey_Is_False()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var scripts = ColumnsScriptRegistration.RegisterColumnsScripts(manager,
            new MockTypeSource(typeof(Acme.Default.Columns.ExplicitFalseColumns)),
            new MockPropertyItemProvider(), services).ToList();

        Assert.Single(scripts);
        Assert.Equal("Columns." + typeof(Acme.Default.Columns.ExplicitFalseColumns).FullName,
            scripts[0].ScriptName);
    }

    [Fact]
    public void RegisterColumnsScripts_Texts_Use_The_Inferred_Key()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        var type = typeof(Acme.Default.Columns.VersionInfoColumns);
        var scripts = ColumnsScriptRegistration.RegisterColumnsScripts(manager,
            new MockTypeSource(type), new MockPropertyItemProvider(), services).ToList();

        // field texts and the script share the key, so the two cannot drift apart
        Assert.Equal("Columns.Default.VersionInfo", scripts[0].ScriptName);
        Assert.Equal("Columns.Default.VersionInfo.",
            PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(type));
    }
}
