using ServerTypingsTest.ReExports;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Generates_Module_ReExport_File_For_Each_Directory()
        {
            var generator = CreateGenerator(typeof(ReExportTypeA), typeof(ReExportTypeB));
            generator.ModuleReExports = true;
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "ReExports.ts").Text;

            Assert.Contains("export * from \"./ReExports/ReExportTypeA\";", code);
            Assert.Contains("export * from \"./ReExports/ReExportTypeB\";", code);
        }
    }
}

namespace ServerTypingsTest.ReExports
{
    [ScriptInclude]
    public class ReExportTypeA
    {
        public string Name { get; set; }
    }

    [ScriptInclude]
    public class ReExportTypeB
    {
        public string Name { get; set; }
    }
}
