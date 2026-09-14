using static Serenity.CodeGenerator.CustomerEntityInputs;
using Serenity.Data.Schema;
using FieldInfo = Serenity.Data.Schema.FieldInfo;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    [Fact]
    public void Primary_Key_Flag_And_NotNull_Are_Added_For_Non_Identity_PK_With_Markers()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Flag" };
        inputs.Table = "Flag";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Flag", F("FlagId", "int", pk: true, nullable: false));
        AddField(schema, "Flag", F("FlagStamp", "timestamp", nullable: false));
        AddField(schema, "Flag", F("FlagAmount", "decimal", nullable: true, scale: 2, size: 18));

        var model = new EntityModelFactory().Create(inputs);

        var id = Assert.Single(model.Fields, x => x.PropertyName == "FlagId");
        Assert.Collection(id.FlagList,
            primaryKey => Assert.Equal("Serenity.Data.Mapping.PrimaryKey", primaryKey.TypeName),
            notNull => Assert.Equal("Serenity.Data.Mapping.NotNull", notNull.TypeName));

        var stamp = Assert.Single(model.Fields, x => x.PropertyName == "FlagStamp");
        Assert.Equal("ByteArray", stamp.FieldType);
        Assert.Equal("byte[]", stamp.DataType);
        Assert.False(stamp.IsValueType);
        Assert.Equal("number[]", stamp.TSType);
        Assert.Collection(stamp.FlagList,
            insertable => Assert.Equal("Serenity.ComponentModel.Insertable", insertable.TypeName),
            updatable => Assert.Equal("Serenity.ComponentModel.Updatable", updatable.TypeName),
            notNull => Assert.Equal("Serenity.Data.Mapping.NotNull", notNull.TypeName));

        var amount = Assert.Single(model.Fields, x => x.PropertyName == "FlagAmount");
        Assert.Equal("Decimal", amount.FieldType);
        Assert.Equal("number", amount.TSType);
        Assert.Empty(amount.FlagList);
        Assert.Equal(2, amount.Scale);
        Assert.Single(amount.AttributeList, a =>
            a.TypeName == "Serenity.Data.Mapping.Scale" && (int)a.Arguments[0] == 2);
    }

    [Fact]
    public void Markers_Are_Looked_Up_From_Schema_When_Missing_On_Fields()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Lookup" };
        inputs.Table = "Lookup";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Lookup", F("LookupId", "int", nullable: false));
        schema.OnGetPrimaryKeyFields = (_, _) => ["LookupId"];

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("LookupId", model.IdField);
        var id = Assert.Single(model.Fields, x => x.PropertyName == "LookupId");
        Assert.Contains(id.FlagList, a => a.TypeName == "Serenity.Data.Mapping.PrimaryKey");
        Assert.Contains(id.FlagList, a => a.TypeName == "Serenity.Data.Mapping.NotNull");
        Assert.Equal(2, id.FlagList.Count);
    }

    [Fact]
    public void First_Field_Is_Used_As_Id_Field_When_No_PK_Or_Identity()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Num" };
        inputs.Table = "Num";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Num", F("NumValue", "decimal"));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("NumValue", model.IdField);
    }

    [Fact]
    public void Sql_Types_Are_Mapped_To_Field_Types_And_Kind_Types()
    {
        var inputs = new CustomerEntityInputs { Identifier = "T" };
        inputs.Table = "T";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "T", F("TBit", "bit"));
        AddField(schema, "T", F("TDate", "date"));
        AddField(schema, "T", F("TDecimal", "decimal"));
        AddField(schema, "T", F("TGuid", "guid"));
        AddField(schema, "T", F("TChar", "char"));
        AddField(schema, "T", F("TImage", "image"));
        AddField(schema, "T", F("TUnknown", "surowykus"));
        AddField(schema, "T", F("TVarbinarySmall", "varbinary", size: 100));
        AddField(schema, "T", F("TVarbinaryBig", "varbinary", size: 300));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("boolean", Assert.Single(model.Fields, x => x.Name == "TBit").TSType);
        Assert.Equal("DateTime", Assert.Single(model.Fields, x => x.Name == "TDate").FieldType);
        Assert.Equal("string", Assert.Single(model.Fields, x => x.Name == "TDate").TSType);
        Assert.Equal("Guid", Assert.Single(model.Fields, x => x.Name == "TGuid").FieldType);
        Assert.Equal("string", Assert.Single(model.Fields, x => x.Name == "TGuid").TSType);
        Assert.Equal("String", Assert.Single(model.Fields, x => x.Name == "TChar").FieldType);
        Assert.Equal("Stream", Assert.Single(model.Fields, x => x.Name == "TImage").FieldType);
        Assert.False(Assert.Single(model.Fields, x => x.Name == "TImage").IsValueType);
        Assert.Equal("Stream", Assert.Single(model.Fields, x => x.Name == "TUnknown").FieldType);
        Assert.Equal("number[]", Assert.Single(model.Fields, x => x.Name == "TUnknown").TSType);
        Assert.Equal("ByteArray", Assert.Single(model.Fields, x => x.Name == "TVarbinarySmall").FieldType);
        Assert.Equal("byte[]", Assert.Single(model.Fields, x => x.Name == "TVarbinarySmall").DataType);
        Assert.Equal("Stream", Assert.Single(model.Fields, x => x.Name == "TVarbinaryBig").FieldType);
    }

    [Fact]
    public void Non_String_Only_Table_Has_No_Name_Field()
    {
        var inputs = new CustomerEntityInputs { Identifier = "Num2" };
        inputs.Table = "Num2";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Num2", F("Num2Id", "int", pk: true, nullable: false));
        AddField(schema, "Num2", F("Num2Count", "int"));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Null(model.NameField);
    }
}
