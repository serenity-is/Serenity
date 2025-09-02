using Serenity.Data.Schema;

namespace Serenity.CodeGenerator;

public class SchemaHelper
{
    public static ISchemaProvider GetSchemaProvider(string serverType)
    {
        var providerType = Type.GetType("Serenity.Data.Schema." + serverType + "SchemaProvider, Serenity.Net.Services") ??
            Type.GetType("Serenity.Data.Schema." + serverType + "SchemaProvider, SerenityData");
        if (providerType == null || !typeof(ISchemaProvider).GetTypeInfo().IsAssignableFrom(providerType))
            throw new ArgumentOutOfRangeException(nameof(serverType), serverType, "Unknown server type");

        return (ISchemaProvider)Activator.CreateInstance(providerType);
    }

    private static readonly Dictionary<string, string> SqlTypeToFieldTypeMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "bfile", "Stream" },
            { "bigint", "Int64" },
            { "binary_double", "Double" },
            { "binary_float", "Single" },
            { "bit", "Boolean" },
            { "blob sub_type 1", "String" },
            { "blob", "Stream" },
            { "char", "String" },
            { "character varying", "String" },
            { "character", "String" },
            { "clob", "String" },
            { "date", "DateOnly" },
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
            { "interval day to second", "TimeSpan" },
            { "interval year to month", "TimeSpan" },
            { "long raw", "ByteArray" },
            { "long", "String" },
            { "longtext", "String" },
            { "mediumtext", "String" },
            { "money", "Decimal" },
            { "nchar", "String" },
            { "nclob", "String" },
            { "ntext", "String" },
            { "number", "Decimal" },
            { "numeric", "Decimal" },
            { "nvarchar", "String" },
            { "nvarchar2", "String" },
            { "raw", "ByteArray" },
            { "real", "Single" },
            { "rowid", "String" },
            { "rowversion", "ByteArray" },
            { "smalldatetime", "DateTime" },
            { "smallint", "Int16" },
            { "text", "String" },
            { "time", "TimeSpan" },
            { "timestamp with local time zone", "DateTime" },
            { "timestamp with time zone", "DateTimeOffset" },
            { "timestamp without time zone", "DateTime" },
            { "timestamp", "DateTime" },
            { "tinyint", "Int16" },
            { "tinytext", "String" },
            { "uniqueidentifier", "Guid" },
            { "urowid", "String" },
            { "varbinary", "Stream" },
            { "varchar", "String" },
            { "varchar2", "String" },
            { "xmltype", "String" },
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