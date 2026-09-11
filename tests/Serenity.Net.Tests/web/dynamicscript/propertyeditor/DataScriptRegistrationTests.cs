namespace Serenity.Web;

[DataScript("Registration.Test")]
public class RegistrationTestDataScript : DataScript<Dictionary<string, string>>
{
    protected override Dictionary<string, string> GetData() => new();
}

public class DataScriptRegistrationTests
{
    [Fact]
    public void RegisterDataScripts_Throws_For_Null_Arguments()
    {
        var manager = new MockDynamicScriptManager();
        var typeSource = new MockTypeSource();
        var services = new ServiceCollection().BuildServiceProvider();

        Assert.Throws<ArgumentNullException>(() =>
            DataScriptRegistration.RegisterDataScripts(null!, typeSource, services));
        Assert.Throws<ArgumentNullException>(() =>
            DataScriptRegistration.RegisterDataScripts(manager, null!, services));
        Assert.Throws<ArgumentNullException>(() =>
            DataScriptRegistration.RegisterDataScripts(manager, typeSource, null!));
    }

    [Fact]
    public void RegisterDataScripts_Registers_Types_With_Attribute()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        DataScriptRegistration.RegisterDataScripts(manager,
            new MockTypeSource(typeof(RegistrationTestDataScript)), services);

        Assert.True(manager.IsRegistered("RemoteData.Registration.Test"));
    }

    [Fact]
    public void RegisterDataScripts_Skips_Types_Without_Attribute()
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection().BuildServiceProvider();

        DataScriptRegistration.RegisterDataScripts(manager,
            new MockTypeSource(typeof(string)), services);

        Assert.Empty(manager.GetRegisteredScriptNames());
    }
}
