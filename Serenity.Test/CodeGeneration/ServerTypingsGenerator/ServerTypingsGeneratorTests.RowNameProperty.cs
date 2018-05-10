using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Reads_NameProperty_FromINameRow()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.WithNameRowInterfaceRow.ts", result.Keys);
            var code = result["SomeModule.WithNameRowInterfaceRow.ts"];
            Assert.Contains("nameProperty = 'SomeName'", code);
        }

        [Fact]
        public void Reads_NameProperty_FromAttribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.WithNamePropertyRow.ts", result.Keys);
            var code = result["SomeModule.WithNamePropertyRow.ts"];
            Assert.Contains("nameProperty = 'WithNameProp'", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{ 
    public class WithNameRowInterfaceRow : Row, INameRow
    {
        public string SomeName
        {
            get { return Fields.SomeName[this]; }
            set { Fields.SomeName[this] = value; }
        }

        StringField INameRow.NameField => Fields.SomeName;

        public class RowFields : RowFieldsBase
        {
            public StringField SomeName;
        }

        public static RowFields Fields = new RowFields().Init();

        public WithNameRowInterfaceRow()
            : base(Fields)
        {
        }
    }

    public class WithNamePropertyRow : Row
    {
        [NameProperty]
        public string WithNameProp
        {
            get { return Fields.WithNameProp[this]; }
            set { Fields.WithNameProp[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public StringField WithNameProp;
        }

        public static RowFields Fields = new RowFields().Init();

        public WithNamePropertyRow()
            : base(Fields)
        {
        }
    }
}