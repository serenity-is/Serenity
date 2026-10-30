namespace Serenity.Testing;

public static class MemoryDbExtensions
{
    public static void CreateTable<TFields>(this MemoryDbBase db)
        where TFields : RowFieldsBase
    {
        CreateTable(db, RowFieldsProvider.Resolve<TFields>());
    }

    public static void CreateTable(this MemoryDbBase db, RowFieldsBase fields)
    {
        var schema = fields.Schema;
        if (!string.IsNullOrEmpty(schema))
        {
            schema = SqlSyntax.Unquote(schema);
            var instance = db.GetDbInstanceByKey(fields.ConnectionKey);
            instance.EnsureSchema(schema);
        }

        var sb = new StringBuilder();
        sb.Append("CREATE TABLE ");
        sb.Append(fields.TableName);
        sb.Append('(');
        int fieldCount = 0;
        string hasAutoIncrement = null;
        foreach (var field in fields)
        {
            if (field.IsTableField())
            {
                if (fieldCount++ > 0)
                    sb.Append(',');

                sb.Append(field.Name);
                sb.Append(' ');
                sb.Append(ToFieldType(field));

                if ((field.Flags & FieldFlags.AutoIncrement) == FieldFlags.AutoIncrement &&
                    (field.Type == FieldType.Int32 ||
                     field.Type == FieldType.Int64) &&
                     hasAutoIncrement == null)
                {
                    sb.Append(" PRIMARY KEY AUTOINCREMENT");
                    hasAutoIncrement ??= field.Name;
                }
            }
        }
        sb.Append(')');

        using var connection = db.NewByKey(fields.ConnectionKey);
        SqlHelper.ExecuteNonQuery(connection, sb.ToString());
    }

    private static string ToFieldType(Field field)
    {
        if (field is StringField)
            return "TEXT COLLATE NOCASE";

        if (field is BooleanField)
            return "INTEGER";

        if (field is DateTimeField ||
            field is DateTimeOffsetField ||
            field is DateOnlyField)
            return "DATETIME";

        if (field is Int16Field ||
            field is Int32Field ||
            field is Int64Field)
            return "INTEGER";

        if (field is SingleField ||
            field is DoubleField)
            return "DOUBLE";

        if (field is DecimalField)
            return "NUMERIC";

        return "TEXT COLLATE NOCASE";
    }
}