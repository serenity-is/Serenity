namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Reads_NameProperty_FromAttribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule.WithNamePropertyRow.ts").Text;
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