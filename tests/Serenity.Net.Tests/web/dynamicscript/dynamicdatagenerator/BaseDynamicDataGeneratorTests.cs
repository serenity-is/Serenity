using Serenity.Extensions.DependencyInjection;
using Serenity.PropertyGrid;

namespace Serenity.Web;

public class BaseDynamicDataGeneratorTests
{
    private class TestGenerator : BaseDynamicDataGenerator
    {
        public string Root { get; set; } = "";
        public bool RegisterScript { get; set; }
        public bool RegisterColumns { get; set; }

        public bool Skip(string name) => ShouldSkipScript(name);
        public string DataFolder => GetDynamicDataFolder();
        public string ProjectRoot => GetProjectRoot();

        protected override ITypeSource GetTypeSource() => new MockTypeSource();
        protected override string GetProjectRoot() => Root;

        protected override void InitializeScripts(IServiceProvider services)
        {
            var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
            if (RegisterScript)
            {
                scriptManager.Register("TestScript", new DataScript("TestScript", () => new { A = 1 }));
            }

            if (RegisterColumns)
            {
                scriptManager.Register("Columns.Test", new DataScript("Columns.Test",
                    () => new PropertyItemsData { Items = [], AdditionalItems = [] }));
            }
        }
    }

    [Fact]
    public void Run_Writes_Dynamic_Data_Files_And_Skips_Unchanged()
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ddgentest-" + Guid.NewGuid().ToString("N"));
        try
        {
            var generator = new TestGenerator { Root = root, RegisterScript = true };

            generator.Run();

            var file = System.IO.Path.Combine(root, "dynamic-data", "TestScript.json");
            Assert.True(System.IO.File.Exists(file));
            var first = System.IO.File.ReadAllText(file);
            Assert.Contains("\"A\"", first);

            generator.Run();
            Assert.Equal(first, System.IO.File.ReadAllText(file));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("ColumnsBundle", true)]
    [InlineData("FormBundle", true)]
    [InlineData("ColumnAndFormBundle", true)]
    [InlineData("RegisteredScripts", true)]
    [InlineData("Other", false)]
    public void ShouldSkipScript_Returns_Expected(string name, bool expected)
    {
        Assert.Equal(expected, new TestGenerator().Skip(name));
    }

    [Fact]
    public void Run_Writes_Columns_Json_Using_Newtonsoft_Path()
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ddgentest-" + Guid.NewGuid().ToString("N"));
        try
        {
            var generator = new TestGenerator { Root = root, RegisterColumns = true };

            generator.Run();

            var file = System.IO.Path.Combine(root, "dynamic-data", "Columns.Test.json");
            Assert.True(System.IO.File.Exists(file));
            Assert.Contains("Items", System.IO.File.ReadAllText(file));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void GetDynamicDataFolder_Combines_Project_Root()
    {
        var generator = new TestGenerator { Root = "C:\\proj" };
        Assert.Equal(System.IO.Path.Combine("C:\\proj", "dynamic-data"), generator.DataFolder);
    }

    private class RawGenerator : BaseDynamicDataGenerator
    {
        public string ProjectRoot => GetProjectRoot();
        public ITypeSource TypeSource => GetTypeSource();
        public IServiceCollection Services => AddServices(new ServiceCollection());
        protected override ITypeSource GetTypeSource() => new MockTypeSource();
        protected override void InitializeScripts(IServiceProvider services)
        {
        }
    }

    [Fact]
    public void AddServices_Registers_Required_Services()
    {
        var services = new RawGenerator().Services;

        Assert.Contains(services, x => x.ServiceType == typeof(IDynamicScriptManager));
        Assert.Contains(services, x => x.ServiceType == typeof(IPropertyItemProvider));
        Assert.Contains(services, x => x.ServiceType == typeof(ITwoLevelCache));
        Assert.Contains(services, x => x.ServiceType == typeof(IRowFieldsProvider));
    }

    [Fact]
    public void GetTypeSource_Returns_TypeSource()
    {
        Assert.NotNull(new RawGenerator().TypeSource);
    }

    [Fact]
    public void GetProjectRoot_Returns_Current_Directory()
    {
        Assert.Equal(Environment.CurrentDirectory, new RawGenerator().ProjectRoot);
    }

    [Fact]
    public void RunAndExitIf_Does_Nothing_For_Other_Arguments()
    {
        new TestGenerator().RunAndExitIf(["other"]);
    }

    [Fact]
    public void NullPermissionService_Grants_All()
    {
        Assert.True(new NullPermissionService().HasPermission("anything"));
    }
}
