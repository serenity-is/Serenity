using ServerTypingsTest.RecordViews;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Columns_With_RecordViews_References_Their_Types()
        {
            var generator = CreateGenerator(typeof(ColumnsWithRecordViews));
            generator.AddTSType(RecordViewTypeFor("CustomerCard", "Views.CustomerCard"));
            generator.AddTSType(RecordViewTypeFor("EmployeeCard", "Views.EmployeeCard"));

            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename.EndsWith("ColumnsWithRecordViews.ts", StringComparison.Ordinal)).Text;

            Assert.Contains("import { CustomerCard, EmployeeCard } from \"MyModule\";", code);
            Assert.Contains("[CustomerCard, EmployeeCard]; // referenced types", code);
        }

        [Fact]
        public void Columns_With_A_Card_View_On_The_Class_References_Its_Type()
        {
            var generator = CreateGenerator(typeof(ColumnsWithClassRecordView));
            generator.AddTSType(RecordViewTypeFor("ProductCardView", "Views.ProductCardView"));

            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename.EndsWith("ColumnsWithClassRecordView.ts", StringComparison.Ordinal)).Text;

            Assert.Contains("import { ProductCardView } from \"MyModule\";", code);
            Assert.Contains("[ProductCardView]; // referenced types", code);
        }

        private static ExternalType RecordViewTypeFor(string name, string key)
        {
            return new ExternalType
            {
                Name = name,
                Module = "MyModule",
                Fields =
                [
                    new ExternalMember
                    {
                        Name = "[Symbol.typeInfo]",
                        IsStatic = true,
                        Type = "RecordViewTypeInfo",
                        Value = key
                    }
                ]
            };
        }
    }
}

namespace ServerTypingsTest.RecordViews
{
    [ColumnsScript]
    public class ColumnsWithRecordViews
    {
        [CustomerCard]
        public string Customer { get; set; }

        // the same view twice is referenced once
        [CustomerCard, RecordViewType("Views.EmployeeCard")]
        public string Employee { get; set; }

        public string Plain { get; set; }
    }

    [ColumnsScript, ProductCardView(Default = true, Below = "md")]
    public class ColumnsWithClassRecordView
    {
        public string Name { get; set; }
    }

    public class ProductCardViewAttribute : RecordViewTypeAttribute
    {
        public const string Key = "Views.ProductCardView";

        public ProductCardViewAttribute()
            : base(Key)
        {
            Placement = RecordViewPlacement.Card;
        }
    }

    public class CustomerCardAttribute : RecordViewTypeAttribute
    {
        public const string Key = "Views.CustomerCard";

        public CustomerCardAttribute()
            : base(Key)
        {
        }
    }
}
