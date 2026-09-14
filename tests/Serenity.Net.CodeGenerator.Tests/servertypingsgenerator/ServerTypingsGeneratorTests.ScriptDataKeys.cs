using ServerTypingsTest.SomeModule.Scripts;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Generates_RemoteDataKeys_For_DataScript_Types()
        {
            var generator = CreateGenerator(typeof(AutoKeyData), typeof(ExplicitKeyData), typeof(ModuleData));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "RemoteDataKeys.ts").Text;

            Assert.Contains("export namespace RemoteDataKeys", code);
            Assert.Contains("SomeModule.AutoKeyData", code);
            Assert.Contains("MyModule.ModuleData", code);
            Assert.Contains("Some.Explicit.Key", code);
        }

        [Fact]
        public void Skips_TransformIgnored_DataScript_Types()
        {
            var generator = CreateGenerator(typeof(IgnoredData), typeof(AutoKeyData));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "RemoteDataKeys.ts").Text;

            Assert.DoesNotContain("IgnoredData", code);
            Assert.Contains("SomeModule.AutoKeyData", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Scripts
{
    [DataScript]
    public class AutoKeyData
    {
    }

    [DataScript("Some.Explicit.Key")]
    public class ExplicitKeyData
    {
    }

    [DataScript, Module("MyModule")]
    public class ModuleData
    {
    }

    [DataScript, TransformIgnore]
    public class IgnoredData
    {
    }
}
