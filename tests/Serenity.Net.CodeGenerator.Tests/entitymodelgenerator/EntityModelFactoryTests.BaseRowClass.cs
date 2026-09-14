using static Serenity.CodeGenerator.CustomerEntityInputs;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    [Fact]
    public void Largest_Matching_Base_Row_Class_Is_Used_And_Fields_Are_Moved_To_Base()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Customer";
        inputs.Config.BaseRowClasses =
        [
            new() { ClassName = "NoFieldsBase", Fields = null },
            new() { ClassName = "BadFieldsBase", Fields = ["CustomerName", "Nope"] },
            new() { ClassName = "SmallBase", Fields = ["CustomerId"] },
            new() { ClassName = "BigBase", Fields = ["CustomerId", "CustomerName"] }
        ];

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("BigBase<CustomerRow.RowFields>", model.RowBaseClass);
        Assert.Equal("BigBaseFields", model.FieldsBaseClass);
        Assert.Equal(2, model.RowBaseFields.Count);
        Assert.All(model.RowBaseFields, f => Assert.Empty(f.FlagList));
        Assert.Equal("CustomerId", model.RowBaseFields[0].PropertyName);
        Assert.Equal("CustomerName", model.RowBaseFields[1].PropertyName);
        Assert.DoesNotContain(model.Fields, f => f.PropertyName == CustomerId);
        Assert.DoesNotContain(model.Fields, f => f.PropertyName == CustomerName);
    }

    [Fact]
    public void Base_Row_Class_Is_Skipped_When_Field_Names_Do_Not_Match_Exact_Case()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Customer";
        inputs.Config.BaseRowClasses =
        [
            new() { ClassName = "CasedBase", Fields = ["customerid"] }
        ];

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("Serenity.Data.Row<CustomerRow.RowFields>", model.RowBaseClass);
        Assert.Equal("Serenity.Data.RowFieldsBase", model.FieldsBaseClass);
        Assert.Empty(model.RowBaseFields);
        Assert.Single(model.Fields, x => x.PropertyName == CustomerId);
        Assert.Single(model.Fields, x => x.PropertyName == CustomerName);
    }

    [Fact]
    public void Serializable_Encoding_For_Base_Row_Classes_Trimmed_Whitespace_Is_Checked()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Customer";
        inputs.Config.BaseRowClasses =
        [
            new() { ClassName = "SpacedFieldsBase", Fields = [" ", " "] }
        ];

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("Serenity.Data.RowFieldsBase", model.FieldsBaseClass);
        Assert.Empty(model.RowBaseFields);
    }
}
