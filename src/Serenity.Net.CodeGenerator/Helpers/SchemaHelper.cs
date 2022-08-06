using Serenity.Data.Schema;

namespace Serenity.CodeGenerator
{
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
                { "bigint", "System.Int64" },
                { "bit", "System.Boolean" },
                { "blob sub_type 1", "System.String" },
                { "char", "System.String" },
                { "character varying", "System.String" },
                { "character", "System.String" },
                { "date", "System.DateTime" },
                { "datetime", "System.DateTime" },
                { "datetime2", "System.DateTime" },
                { "datetimeoffset", "System.DateTimeOffset" },
                { "decimal", "System.Decimal" },
                { "double", "System.Double" },
                { "doubleprecision", "System.Double" },
                { "float", "System.Double" },
                { "guid", "System.Guid" },
                { "int", "System.Int32" },
                { "int4", "System.Int32" },
                { "int8", "System.Int64" },
                { "integer", "System.Int32" },
                { "money", "System.Decimal" },
                { "nchar", "System.String" },
                { "ntext", "System.String" },
                { "numeric", "System.Decimal" },
                { "nvarchar", "System.String" },
                { "nvarchar2", "System.String" },
                { "real", "System.Single" },
                { "rowversion", "ByteArray" },
                { "smalldatetime", "System.DateTime" },
                { "smallint", "System.Int16" },
                { "text", "System.String" },
                { "time", "TimeSpan" },
                { "timestamp", "System.DateTime" },
                { "timestamp without time zone", "System.DateTime" },
                { "timestamp with time zone", "System.DateTimeOffset" },
                { "tinyint", "System.Int16" },
                { "uniqueidentifier", "System.Guid" },
                { "varbinary", "System.IO.Stream" },
                { "varchar", "System.String" },
                { "varchar2", "System.String" }
            };

        public static string SqlTypeNameToFieldType(string sqlTypeName, int size, out string dataType)
        {
            dataType = null;

            if (string.Equals(sqlTypeName, "varbinary", StringComparison.OrdinalIgnoreCase))
            {
                if (size == 0 || size > 256)
                    return "System.IO.Stream";

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
                return "System.IO.Stream";
        }
    }
}