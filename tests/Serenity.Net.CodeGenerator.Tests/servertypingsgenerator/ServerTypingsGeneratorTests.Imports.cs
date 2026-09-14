namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Uses_Correct_Import_Location_For_Both_Files_In_Root_Folder()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.TypeAtRoot), typeof(ServerTypingsTest.Common.TypeAtRootDotCommon));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "TypeAtRoot.ts").Text;
            Assert.Contains("from \"./Common/TypeAtRootDotCommon\"", code);
        }
    }
}

namespace ServerTypingsTest
{
    [ScriptInclude]
    public class TypeAtRoot
    {
        public Common.TypeAtRootDotCommon Prop { get; set; }
    }
}

namespace ServerTypingsTest.Common
{
    [ScriptInclude]
    public class TypeAtRootDotCommon
    {
    }
}