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
}
