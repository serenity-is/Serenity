using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Reads_NameProperty_FromAttribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.WithNamePropertyRow.ts", result.Keys);
            var code = result["SomeModule.WithNamePropertyRow.ts"].Text;
            Assert.Contains("nameProperty = 'WithNameProp'", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{
    public class WithNamePropertyRow : Row<WithNamePropertyRow.RowFields>, INameRow
    {
        [NameProperty]
        public string WithNameProp
        {
            get { return fields.WithNameProp[this]; }
            set { fields.WithNameProp[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public StringField WithNameProp;
        }
    }
}