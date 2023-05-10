using ServerTypingsTest.SomeModule.Entities;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Reads_LocalTextPrefix_SetInCode()
        {
            var generator = CreateGenerator(typeof(RowWithLocalTextPrefixSetInCodeRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule.RowWithLocalTextPrefixSetInCodeRow.ts").Text;
            Assert.Contains("localTextPrefix = 'Set.InCode'", code);
        }

        [Fact]
        public void LocalTextPrefix_SetInCode_Overrides_Attribute()
        {
            var generator = CreateGenerator(typeof(RowWithLocalTextPrefixSetInCodeAndAttributeRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule.RowWithLocalTextPrefixSetInCodeAndAttributeRow.ts").Text;
            Assert.Contains("localTextPrefix = 'This.ShouldOverride'", code);
        }

        [Fact]
        public void Reads_LocalTextPrefix_SetWithAttribute()
        {
            var generator = CreateGenerator(typeof(RowWithLocalTextPrefixAttributeRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule.RowWithLocalTextPrefixAttributeRow.ts").Text;
            Assert.Contains("localTextPrefix = 'Attribute.Prefix'", code);
        }

        [Fact]
        public void Respects_Module_Attribute_For_Auto_LocalTextPrefix()
        {
            var generator = CreateGenerator(typeof(RowWithModuleAndNoLocalTextPrefixRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule.RowWithModuleAndNoLocalTextPrefixRow.ts").Text;
            Assert.Contains("localTextPrefix = 'ADifferentModule.RowWithModuleAndNoLocalTextPrefix'", code);
        }

        [Fact]
        public void Uses_Namespace_And_TypeName_When_No_LocalTextPrefix_Or_Module()
        {
            var generator = CreateGenerator(typeof(NoModuleNoLocalTextPrefixRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule.NoModuleNoLocalTextPrefixRow.ts").Text;
            Assert.Contains("localTextPrefix = 'SomeModule.NoModuleNoLocalTextPrefix'", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{
    public class RowWithLocalTextPrefixSetInCodeRow : Row<RowWithLocalTextPrefixSetInCodeRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "Set.InCode";
            }
        }
    }

    [LocalTextPrefix("This.ShouldBeOverridden")]
    public class RowWithLocalTextPrefixSetInCodeAndAttributeRow : Row<RowWithLocalTextPrefixSetInCodeAndAttributeRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "This.ShouldOverride";
            }
        }
    }

    [LocalTextPrefix("Attribute.Prefix")]
    public class RowWithLocalTextPrefixAttributeRow : Row<RowWithLocalTextPrefixAttributeRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }
    
    [Module("ADifferentModule")]
    public class RowWithModuleAndNoLocalTextPrefixRow : Row<RowWithModuleAndNoLocalTextPrefixRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    public class NoModuleNoLocalTextPrefixRow : Row<NoModuleNoLocalTextPrefixRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }
}