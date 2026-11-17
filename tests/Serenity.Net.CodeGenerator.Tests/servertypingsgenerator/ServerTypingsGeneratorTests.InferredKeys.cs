namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Columns_Uses_Inferred_Key_When_InferKey()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Columns.InferredColumns));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/InferredColumns.ts").Text;

            // the root (project) namespace and the Columns suffix are not part of
            // the key, matching ColumnsScriptAttribute.AutoColumnsKeyFor at runtime
            Assert.Contains("static readonly columnsKey = 'Default.Inferred';", code);
            Assert.DoesNotContain("ServerTypingsTest.Default.Columns.InferredColumns", code);
        }

        [Fact]
        public void Columns_Uses_FullName_When_InferKey_Is_False()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Columns.ExplicitFalseColumns));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/ExplicitFalseColumns.ts").Text;

            Assert.Contains(
                "static readonly columnsKey = 'ServerTypingsTest.Default.Columns.ExplicitFalseColumns';", code);
        }

        [Fact]
        public void Columns_Uses_FullName_When_No_Key_And_No_InferKey()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Columns.PlainColumns));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/PlainColumns.ts").Text;

            Assert.Contains(
                "static readonly columnsKey = 'ServerTypingsTest.Default.Columns.PlainColumns';", code);
        }

        [Fact]
        public void Columns_Explicit_Key_Wins_Over_InferKey()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Columns.ExplicitKeyColumns));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/ExplicitKeyColumns.ts").Text;

            Assert.Contains("static readonly columnsKey = 'Registration.Explicit';", code);
        }

        [Fact]
        public void Columns_Infers_Key_From_Module_Attribute()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Modules.Columns.WithModuleColumns));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Modules/WithModuleColumns.ts").Text;

            // [Module("SomeModule")] wins over the namespace
            Assert.Contains("static readonly columnsKey = 'SomeModule.WithModule';", code);
        }

        [Fact]
        public void Form_Uses_Inferred_Key_When_InferKey()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Forms.InferredForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/InferredForm.ts").Text;

            Assert.Contains("static readonly formKey = 'Default.Inferred';", code);
            Assert.DoesNotContain("ServerTypingsTest.Default.Forms.InferredForm", code);
        }

        [Fact]
        public void Form_Uses_FullName_When_InferKey_Is_False()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Forms.ExplicitFalseForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/ExplicitFalseForm.ts").Text;

            Assert.Contains(
                "static readonly formKey = 'ServerTypingsTest.Default.Forms.ExplicitFalseForm';", code);
        }

        [Fact]
        public void Form_Uses_FullName_When_No_Key_And_No_InferKey()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Forms.PlainForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/PlainForm.ts").Text;

            Assert.Contains(
                "static readonly formKey = 'ServerTypingsTest.Default.Forms.PlainForm';", code);
        }

        [Fact]
        public void Form_Explicit_Key_Wins_Over_InferKey()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Forms.ExplicitKeyForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/ExplicitKeyForm.ts").Text;

            Assert.Contains("static readonly formKey = 'Registration.Explicit';", code);
        }

        [Fact]
        public void Form_Infers_Key_From_Module_Attribute()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Modules.Forms.WithModuleForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Modules/WithModuleForm.ts").Text;

            Assert.Contains("static readonly formKey = 'SomeModule.WithModule';", code);
        }

        [Fact]
        public void Form_Infers_Key_From_BasedOnRow_Module_Attribute()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Modules.Forms.BasedOnModuleRowForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Modules/BasedOnModuleRowForm.ts").Text;

            // the module comes from the row type when the form has none
            Assert.Contains("static readonly formKey = 'SomeModule.BasedOnModuleRow';", code);
        }
        [Fact]
        public void Form_Keeps_Lookups_Namespace()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Lookups.CategoryLookupForm));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/Lookups.CategoryLookupForm.ts").Text;

            // only .Forms and .Columns are stripped, matching
            // FormScriptAttribute.AutoFormKeyFor
            Assert.Contains("static readonly formKey = 'Default.Lookups.CategoryLookup';", code);
        }

        [Fact]
        public void Enum_Uses_Inferred_Key_When_EnumKey_Is_Parameterless()
        {
            var generator = CreateGenerator(typeof(ServerTypingsTest.Default.Enums.InferredEnum));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Default/Enums.InferredEnum.ts").Text;

            // ".Enums" is six characters: stripping eight used to eat two
            // characters of the module name
            Assert.Contains("registerEnum(InferredEnum, 'ServerTypingsTest.Default.Enums.InferredEnum', 'Default.InferredEnum');", code);
        }
    }
}

namespace ServerTypingsTest.Default.Lookups
{
    [FormScript(InferKey = true)]
    public class CategoryLookupForm
    {
        public string? Name { get; set; }
    }
}

namespace ServerTypingsTest.Default.Enums
{
    [ScriptInclude]
    [EnumKey]
    public enum InferredEnum
    {
        Inactive = 0,
        Active = 1
    }
}

namespace ServerTypingsTest.Default.Columns
{
    // the shape sergen generates for an entity in the Default module
    [ColumnsScript(InferKey = true)]
    public class InferredColumns
    {
        public string? Name { get; set; }
    }

    [ColumnsScript(InferKey = false)]
    public class ExplicitFalseColumns
    {
        public string? Name { get; set; }
    }

    [ColumnsScript]
    public class PlainColumns
    {
        public string? Name { get; set; }
    }

    [ColumnsScript("Registration.Explicit", InferKey = true)]
    public class ExplicitKeyColumns
    {
        public string? Name { get; set; }
    }
}

namespace ServerTypingsTest.Default.Forms
{
    [FormScript(InferKey = true)]
    public class InferredForm
    {
        public string? Name { get; set; }
    }

    [FormScript(InferKey = false)]
    public class ExplicitFalseForm
    {
        public string? Name { get; set; }
    }

    [FormScript]
    public class PlainForm
    {
        public string? Name { get; set; }
    }

    [FormScript("Registration.Explicit", InferKey = true)]
    public class ExplicitKeyForm
    {
        public string? Name { get; set; }
    }
}

namespace ServerTypingsTest.Modules.Columns
{
    [Module("SomeModule")]
    [ColumnsScript(InferKey = true)]
    public class WithModuleColumns
    {
        public string? Name { get; set; }
    }
}

namespace ServerTypingsTest.Modules.Forms
{
    [Module("SomeModule")]
    [FormScript(InferKey = true)]
    public class WithModuleForm
    {
        public string? Name { get; set; }
    }

    [FormScript(InferKey = true)]
    [BasedOnRow(typeof(ModuleRow))]
    public class BasedOnModuleRowForm
    {
        public string? Name { get; set; }
    }

    [Module("SomeModule")]
    public class ModuleRow
    {
        public string? Name { get; set; }
    }
}
