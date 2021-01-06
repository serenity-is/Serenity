using Serenity.ComponentModel;
using Serenity.Data;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Reads_LocalTextPrefix_SetInCode()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithLocalTextPrefixSetInCodeRow.ts", result.Keys);
            var code = result["SomeModule.RowWithLocalTextPrefixSetInCodeRow.ts"];
            Assert.Contains("localTextPrefix = 'Set.InCode'", code);
        }

        [Fact]
        public void LocalTextPrefix_SetInCode_Overrides_Attribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithLocalTextPrefixSetInCodeAndAttributeRow.ts", result.Keys);
            var code = result["SomeModule.RowWithLocalTextPrefixSetInCodeAndAttributeRow.ts"];
            Assert.Contains("localTextPrefix = 'This.ShouldOverride'", code);
        }

        [Fact]
        public void Reads_LocalTextPrefix_SetWithAttribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithLocalTextPrefixAttributeRow.ts", result.Keys);
            var code = result["SomeModule.RowWithLocalTextPrefixAttributeRow.ts"];
            Assert.Contains("localTextPrefix = 'Attribute.Prefix'", code);
        }

        public void Respects_Module_Attribute_For_Auto_LocalTextPrefix()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithModuleAndNoLocalTextPrefixRow.ts", result.Keys);
            var code = result["SomeModule.RowWithModuleAndNoLocalTextPrefixRow.ts"];
            Assert.Contains("localTextPrefix = 'ADifferentModule.RowWithModuleAndNoLocalTextPrefix'", code);
        }

        public void Uses_Namespace_And_TypeName_When_No_LocalTextPrefix_Or_Module()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.NoModuleNoLocalTextPrefixRow.ts", result.Keys);
            var code = result["SomeModule.NoModuleNoLocalTextPrefixRow.ts"];
            Assert.Contains("localTextPrefix = 'SomeModule.NoModuleNoLocalTextPrefixRow'", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{
    public class RowWithLocalTextPrefixSetInCodeRow : Row
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "Set.InCode";
            }
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithLocalTextPrefixSetInCodeRow()
            : base(Fields)
        {
        }
    }

    [LocalTextPrefix("This.ShouldBeOverridden")]
    public class RowWithLocalTextPrefixSetInCodeAndAttributeRow : Row
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "This.ShouldOverride";
            }
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithLocalTextPrefixSetInCodeAndAttributeRow()
            : base(Fields)
        {
        }
    }

    [LocalTextPrefix("Attribute.Prefix")]
    public class RowWithLocalTextPrefixAttributeRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithLocalTextPrefixAttributeRow()
            : base(Fields)
        {
        }
    }

    [Module("ADifferentModule")]
    public class RowWithModuleAndNoLocalTextPrefixRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithModuleAndNoLocalTextPrefixRow()
            : base(Fields)
        {
        }
    }

    public class NoModuleNoLocalTextPrefixRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public NoModuleNoLocalTextPrefixRow()
            : base(Fields)
        {
        }
    }
}