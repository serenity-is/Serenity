using Serenity.Data.Schema;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.CodeGenerator
{
    public class SchemaHelper
    {
        public static ISchemaProvider GetSchemaProvider(string serverType)
        {
            var providerType = Type.GetType("Serenity.Data.Schema." + serverType + "SchemaProvider, Serenity.Data");
            if (providerType == null || !typeof(ISchemaProvider).GetTypeInfo().IsAssignableFrom(providerType))
                throw new ArgumentOutOfRangeException("serverType", (object)serverType, "Unknown server type");

            return (ISchemaProvider)Activator.CreateInstance(providerType);
        }

        private static Dictionary<string, string> SqlTypeToFieldTypeMap = 
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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
                { "real", "Single" },
                { "rowversion", "ByteArray" },
                { "smalldatetime", "DateTime" },
                { "smallint", "Int16" },
                { "text", "String" },
                { "time", "TimeSpan" },
                { "timestamp", "DateTime" },
                { "timestamp without time zone", "DateTime" },
                { "timestamp with time zone", "DateTimeOffset" },
                { "tinyint", "Int16" },
                { "uniqueidentifier", "Guid" },
                { "varbinary", "Stream" },
                { "varchar", "String" },
                { "varchar2", "String" }, // Oracle
                { "nvarchar2", "String" } // Oracle
            };

        public static string SqlTypeNameToFieldType(string sqlTypeName, int size, out string dataType)
        {
            dataType = null;
            string fieldType;
            sqlTypeName = sqlTypeName.ToLowerInvariant();

            if (sqlTypeName == "varbinary")
            {
                if (size == 0 || size > 256)
                    return "Stream";

                dataType = "byte[]";
                return "ByteArray";
            }
            else if (sqlTypeName == "timestamp" || sqlTypeName == "rowversion")
            {
                dataType = "byte[]";
                return "ByteArray";
            }
            // Oracle generic NUMBER type
            // Generic type to map C# to Oracle NUMBER is (Decimal) Type
            // => To avoid this slow/heavy-cost type we map C# system types according NUMBER() length
            else if (sqlTypeName == "number")
            {
              if (size < 5) // NUMBER(5) = maxvalue up to 32 767
                return "Int16";
              if (size >= 11) // NUMBER(11) maxvalue from 2 147 483 649 and upper
                return "Int64";
              return "Int32"; // NUMBER(10) = maxvalue up to 2 147 483 648
            }
            else if (SqlTypeToFieldTypeMap.TryGetValue(sqlTypeName, out fieldType))
                return fieldType;
            else
                return "Stream";
        }
    }
}