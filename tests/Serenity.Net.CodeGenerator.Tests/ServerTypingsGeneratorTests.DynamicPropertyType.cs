using ServerTypingsTest.DynamicPropertyType;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Converts_DynamicTypeProperty_As_Any()
        {
            var generator = CreateGenerator(typeof(TypeWithDynamicMember));
            var files = generator.Run();
            var actual = Assert.Single(files).Text;
            Assert.Equal(NormalizeTS(@"namespace ServerTypingsTest.DynamicPropertyType {
    export interface TypeWithDynamicMember {
        SomeDynamic?: any;
    }
}"), NormalizeTS(actual));
        }
    }
}

namespace ServerTypingsTest.DynamicPropertyType
{
    [ScriptInclude]
    public class TypeWithDynamicMember
    {
        public dynamic SomeDynamic { get; set; }
    }
}