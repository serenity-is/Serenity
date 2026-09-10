namespace Serenity.Data;

using System.Text.Json;

public class ListRowAndRowListFieldTests
{
    private static AllFieldsRow NewRow() => new();

    private static IdNameRow MakeIdName(int id, string name)
    {
        var row = new IdNameRow
        {
            ID = id,
            Name = name
        };
        return row;
    }

    // --- GenericField paths via ListField ---

    [Fact]
    public void Indexer_GetSet_WorksAndMarksAssigned()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AList;
        Assert.Null(field[row]);

        field[row] = ["a", "b"];
        Assert.Equal(["a", "b"], field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void Copy_CopiesValueAndTargetAssigned()
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AList;
        field[row1] = ["x"];
        Assert.Null(field[row2]);

        field.Copy(row1, row2);

        Assert.Equal("x", Assert.Single(field[row2]!));
        Assert.True(row2.IsAssigned(field));
    }

    // --- ListField ---

    [Fact]
    public void ListField_IndexCompare_ListWiseComparison()
    {
        var field = AllFieldsRow.Fields.AList;
        var row1 = NewRow();
        var row2 = NewRow();

        Assert.Equal(0, field.IndexCompare(row1, row2));

        field[row1] = ["a"];
        Assert.Equal(1, field.IndexCompare(row1, row2));
        Assert.Equal(-1, field.IndexCompare(row2, row1));

        field[row2] = ["b"];
        Assert.True(field.IndexCompare(row1, row2) < 0);

        field[row2] = ["a", "x"];
        Assert.True(field.IndexCompare(row1, row2) < 0);
    }

    // --- RowField ---

    [Fact]
    public void RowField_GetSet_AndClone()
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.ARow;
        field[row1] = MakeIdName(7, "n");

        field.Copy(row1, row2);

        var cloned = field[row2];
        Assert.NotNull(cloned);
        Assert.NotSame(field[row1], cloned);
        Assert.Equal(7, cloned!.ID);
        Assert.Equal("n", cloned.Name);
    }

    [Fact]
    public void RowField_IndexCompare_ComparesByRowFields()
    {
        var field = AllFieldsRow.Fields.ARow;
        var row1 = NewRow();
        var row2 = NewRow();

        Assert.Equal(0, field.IndexCompare(row1, row2));

        field[row1] = MakeIdName(1, "a");
        Assert.Equal(1, field.IndexCompare(row1, row2));
        Assert.Equal(-1, field.IndexCompare(row2, row1));

        field[row2] = MakeIdName(2, "a");
        Assert.True(field.IndexCompare(row1, row2) < 0);

        field[row2] = MakeIdName(1, "b");
        Assert.True(field.IndexCompare(row1, row2) < 0);
    }

    [Fact]
    public void RowField_NullValue_HandlesAllBranches()
    {
        var field = AllFieldsRow.Fields.ARow;
        var row1 = NewRow();
        var row2 = NewRow();
        field[row2] = MakeIdName(1, "a");

        Assert.True(field.IndexCompare(row1, row2) < 0);
        Assert.Equal(0, field.IndexCompare(row1, row1));
    }

    // --- RowListField ---

    [Fact]
    public void RowListField_GetSet_AndClone()
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.ARowList;
        field[row1] = [MakeIdName(1, "a"), MakeIdName(2, "b")];

        field.Copy(row1, row2);

        var cloned = field[row2];
        Assert.NotNull(cloned);
        Assert.Same(cloned.GetType(), field[row1].GetType());
        Assert.Equal(2, cloned!.Count);
        Assert.Equal(2, cloned[1].ID);
    }

    [Fact]
    public void RowListField_IndexCompare_RowWiseComparison()
    {
        var field = AllFieldsRow.Fields.ARowList;
        var row1 = NewRow();
        var row2 = NewRow();

        Assert.Equal(0, field.IndexCompare(row1, row2));

        field[row1] = [null, MakeIdName(1, "a")];
        Assert.Equal(1, field.IndexCompare(row1, row2));
        Assert.Equal(-1, field.IndexCompare(row2, row1));

        field[row2] = [MakeIdName(1, "a")];
        Assert.True(field.IndexCompare(row1, row2) < 0);

        field[row1] = [MakeIdName(1, "a")];
        field[row2] = [MakeIdName(2, "a")];
        Assert.True(field.IndexCompare(row1, row2) < 0);

        field[row1] = [MakeIdName(1, "a")];
        field[row2] = [MakeIdName(1, "b")];
        Assert.True(field.IndexCompare(row1, row2) < 0);
    }

    // --- CustomClassField base behavior (via subtypes) ---

    private class StubValueClassField(Func<IRow, AllFieldsRow.SampleJson?> getter, Action<IRow, AllFieldsRow.SampleJson?> setter)
        : CustomClassField<AllFieldsRow.SampleJson>(null!, "STUB", null, 0, FieldFlags.Default, getter, setter)
    {
        protected override AllFieldsRow.SampleJson GetFromReader(IDataReader reader, int index)
            => new() { Name = reader.GetString(index) };
    }

    [Fact]
    public void CustomClassField_GetFromReader_UsesVirtualReaderValue()
    {
        AllFieldsRow.SampleJson? stored = null;
        var stub = new StubValueClassField(_ => stored, (_, v) => stored = v);
        var row = NewRow();
        using var reader = new MockDbDataReader(new { STUB = "abc" });
        reader.Read();

        stub.GetFromReader(reader, 0, row);

        Assert.Equal("abc", stored!.Name);
    }

    [Fact]
    public void CustomClassField_NewtonsoftSerializeDeserialize_RoundTrips()
    {
        AllFieldsRow.SampleJson? stored = null;
        var stub = new StubValueClassField(_ => stored, (_, v) => stored = v);
        var row = NewRow();

        var sw = new System.IO.StringWriter();
        var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        stub.ValueToJson(writer, row, new Newtonsoft.Json.JsonSerializer());
        writer.Flush();
        Assert.Equal("null", sw.ToString());

        stored = new AllFieldsRow.SampleJson { Name = "a", Value = 5 };
        sw = new System.IO.StringWriter();
        writer = new Newtonsoft.Json.JsonTextWriter(sw);
        stub.ValueToJson(writer, row, new Newtonsoft.Json.JsonSerializer());
        writer.Flush();
        Assert.Contains("\"Name\":\"a\"", sw.ToString(), StringComparison.Ordinal);

        var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("null"));
        reader.Read();
        stub.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());
        Assert.Null(stored);

        reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("undefined"));
        reader.Read();
        stub.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());
        Assert.Null(stored);

        reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"Name\":\"b\",\"Value\":4}"));
        reader.Read();
        stub.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());
        Assert.Equal("b", stored!.Name);
        Assert.Equal(4, stored.Value);
    }

    [Fact]
    public void CustomClassField_Utf8SerializeDeserialize_RoundTrips()
    {
        AllFieldsRow.SampleJson? stored = null;
        var stub = new StubValueClassField(_ => stored, (_, v) => stored = v);
        var row = NewRow();

        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        stub.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("null", System.Text.Encoding.UTF8.GetString(ms.ToArray()));

        stored = new AllFieldsRow.SampleJson { Name = "a", Value = 2 };
        ms.SetLength(0);
        using var writer2 = new Utf8JsonWriter(ms);
        stub.ValueToJson(writer2, row, new JsonSerializerOptions());
        writer2.Dispose();
        Assert.Equal("{\"Name\":\"a\",\"Value\":2}", System.Text.Encoding.UTF8.GetString(ms.ToArray()));

        var reader = ReadFirst("null");
        stub.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(stored);

        reader = ReadFirst("{\"Name\":\"b\",\"Value\":4}");
        stub.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal("b", stored!.Name);
        Assert.Equal(4, stored.Value);
    }

    private static Utf8JsonReader ReadFirst(string json)
    {
        var raw = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(raw);
        reader.Read();
        return reader;
    }
}
