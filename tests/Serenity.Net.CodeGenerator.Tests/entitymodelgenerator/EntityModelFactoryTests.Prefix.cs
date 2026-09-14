using static Serenity.CodeGenerator.CustomerEntityInputs;
using Serenity.Data.Schema;
using FieldInfo = Serenity.Data.Schema.FieldInfo;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    private static FieldInfo F(string name, string dataType, bool pk = false,
        bool identity = false, bool nullable = true, int size = 0, int scale = 0,
        string? pkSchema = null, string? pkTable = null, string? pkColumn = null)
    {
        return new()
        {
            FieldName = name,
            DataType = dataType,
            IsPrimaryKey = pk,
            IsIdentity = identity,
            IsNullable = nullable,
            Size = size,
            Scale = scale,
            PKSchema = pkSchema,
            PKTable = pkTable,
            PKColumn = pkColumn
        };
    }

    private static void AddField(CustomerDataSchema schema, string table, FieldInfo field)
    {
        schema.FieldInfos.Add((TestSchema, table, field));
    }

    [Fact]
    public void Field_Prefix_Is_Detected_From_Common_Underscore_Prefix()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Profile" };
        inputs.Table = "Profile";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Profile", F("Profile_Id", "int", pk: true, identity: true, nullable: false));
        AddField(schema, "Profile", F("Profile_Name", "nvarchar", nullable: false, size: 100));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("Profile_", model.FieldPrefix);
        Assert.Equal("Id", model.IdField);
        var id = Assert.Single(model.Fields, x => x.PropertyName == "Id");
        Assert.Equal("Profile_Id", id.Name);
        Assert.Equal("Id", id.Title);
        Assert.Single(model.Fields, x => x.PropertyName == "Name");
        Assert.Equal("Name", Assert.Single(model.Fields, x => x.PropertyName == "Name").PropertyName);

        var attributes = id.AttributeList;
        Assert.Contains(attributes, a =>
            a.TypeName == "Serenity.Data.Mapping.Column" && "Profile_Id".Equals(a.Arguments[0]));
    }

    [Fact]
    public void Field_Prefix_Is_Ignored_If_Not_Shared_By_All_Fields()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Mixed" };
        inputs.Table = "Mixed";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Mixed", F("Mixed_Id", "int", pk: true, nullable: false));
        AddField(schema, "Mixed", F("OtherName", "nvarchar", size: 30));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("", model.FieldPrefix);
        Assert.Equal("MixedId", Assert.Single(model.Fields, x => x.Name == "Mixed_Id").PropertyName);
    }

    [Fact]
    public void PropertyNames_Are_Made_Unique_When_Case_Insensitive_Duplicates()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Dup" };
        inputs.Table = "Dup";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Dup", F("DupId", "int", pk: true, nullable: false));
        AddField(schema, "Dup", F("dup_name", "nvarchar", size: 50));
        AddField(schema, "Dup", F("Dup_Name", "nvarchar", size: 50));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("DupName", Assert.Single(model.Fields, x => x.Name == "dup_name").PropertyName);
        Assert.Equal("DupName1", Assert.Single(model.Fields, x => x.Name == "Dup_Name").PropertyName);
    }
}
