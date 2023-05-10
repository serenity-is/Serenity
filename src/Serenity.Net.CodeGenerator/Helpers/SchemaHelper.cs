using Serenity.Data.Schema;

namespace Serenity.CodeGenerator;

public class SchemaHelper
{
    public static ISchemaProvider GetSchemaProvider(string serverType)
    {
        var providerType = Type.GetType("Serenity.Data.Schema." + serverType + "SchemaProvider, Serenity.Net.Data") ??
            Type.GetType("Serenity.Data.Schema." + serverType + "SchemaProvider, SerenityData");
        if (providerType == null || !typeof(ISchemaProvider).GetTypeInfo().IsAssignableFrom(providerType))
            throw new ArgumentOutOfRangeException(nameof(serverType), serverType, "Unknown server type");

        return (ISchemaProvider)Activator.CreateInstance(providerType);
    }

    private static readonly Dictionary<string, string> SqlTypeToFieldTypeMap = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "bigint", "Int64" },
            { "bit", "Boolean" },
            { "blob sub_type 1", "String" },
            { "char", "String" },
            { "character varying", "String" },
            { "character", "String" },
            { "date", "DateTime" },
            { "datetime", "DateTime" },
            { "datetime2", "DateTime" },
            { "datetimeoffset", "DateTimeOffset" },
            { "decimal", "Decimal" },
            { "double", "Double" },
            { "doubleprecision", "Double" },
            { "float", "Double" },
            { "guid", "Guid" },
            { "int", "Int32" },
            { "int4", "Int32" },
            { "int8", "Int64" },
            { "integer", "Int32" },
            { "money", "Decimal" },
            { "nchar", "String" },
            { "ntext", "String" },
            { "numeric", "Decimal" },
            { "nvarchar", "String" },
            { "nvarchar2", "String" },
            { "real", "Single" },
            { "rowversion", "ByteArray" },
            { "smalldatetime", "DateTime" },
            { "smallint", "Int16" },
            { "tinytext", "String" },
            { "text", "String" },
            { "mediumtext", "String" },
            { "longtext", "String" },
            { "time", "TimeSpan" },
            { "timestamp", "DateTime" },
            { "timestamp without time zone", "DateTime" },
            { "timestamp with time zone", "DateTimeOffset" },
            { "tinyint", "Int16" },
            { "uniqueidentifier", "Guid" },
            { "varbinary", "Stream" },
            { "varchar", "String" },
            { "varchar2", "String" }
        };

    public static string SqlTypeNameToFieldType(string sqlTypeName, int size, out string dataType)
    {
        dataType = null;

        if (string.Equals(sqlTypeName, "varbinary", StringComparison.OrdinalIgnoreCase))
        {
            if (size == 0 || size > 256)
                return "Stream";

            dataType = "byte[]";
            return "ByteArray";
        }
        else if (string.Equals(sqlTypeName, "timestamp", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(sqlTypeName, "rowversion", StringComparison.OrdinalIgnoreCase))
        {
            dataType = "byte[]";
            return "ByteArray";
        }
        else if (SqlTypeToFieldTypeMap.TryGetValue(sqlTypeName, out string fieldType))
            return fieldType;
        else
            return "Stream";
    }
}