using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Serenity.CodeGenerator
{
    public class SqlSchemaInfo
    {
        public static ISchemaProvider GetSchemaProvider(string serverType)
        {
            var providerType = Type.GetType("Serenity.CodeGenerator." + serverType + "SchemaProvider");
            if (providerType == null || typeof(ISchemaProvider).GetTypeInfo().IsAssignableFrom(providerType))
                throw new ArgumentOutOfRangeException("serverType", (object)serverType, "Unknown server type");

            return (ISchemaProvider)Activator.CreateInstance(providerType);
        }

        public static List<ForeignKeyInfo> GetSingleFieldForeignKeys(IDbConnection connection, string schema, string tableName,
            ISchemaProvider schemaProvider = null)
        {
            schemaProvider = schemaProvider ?? GetSchemaProvider(connection.GetDialect().ServerType);
            return schemaProvider.GetForeignKeys(connection, schema, tableName)
                .ToLookup(x => x.FKName)
                .Where(x => x.Count() == 1)
                .SelectMany(x => x)
                .ToList();
        }

        public static List<FieldInfo> GetTableFieldInfos(IDbConnection connection, string schema, string tableName)
        {
            var schemaProvider = GetSchemaProvider(connection.GetDialect().ServerType);
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            List<string> primaryFields = schemaProvider.GetPrimaryKeyFields(connection, schema, tableName).ToList();
            List<string> identityFields = schemaProvider.GetIdentityFields(connection, schema, tableName).ToList();
            var singleFieldForeignKeys = GetSingleFieldForeignKeys(connection, schema, tableName, schemaProvider);

            var query = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@schema and TABLE_NAME = @tableName";
            bool sqlite = connection.GetDialect().ServerType.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase);

            if (sqlite)
                query = "PRAGMA table_info(@tableName)";

            var columns = connection.Query(query, new
            {
                schema,
                tableName
            });
            var order = new Dictionary<string, int>();

            if (!columns.Any())
                return new List<FieldInfo>();

            var first = columns.First() as IDictionary<string, object>;

            var ordinal = "ORDINAL_POSITION";
            var columnName = "COLUMN_NAME";
            var isNullable = "IS_NULLABLE";
            var charMax = "CHARACTER_MAXIMUM_LENGTH";
            var numPrec = "NUMERIC_PRECISION";
            var numScale = "NUMERIC_SCALE";
            var dataType = "DATA_TYPE";

            if (!first.ContainsKey(ordinal) &&
                first.ContainsKey(ordinal.ToLowerInvariant()))
            {
                ordinal = ordinal.ToLowerInvariant();
                columnName = columnName.ToLowerInvariant();
                dataType = dataType.ToLowerInvariant();
                isNullable = isNullable.ToLowerInvariant();
                charMax = charMax.ToLowerInvariant();
                numPrec = numPrec.ToLowerInvariant();
                numScale = numScale.ToLowerInvariant();
            }

            if (!first.ContainsKey(dataType) &&
                first.ContainsKey("COLUMN_DATA_TYPE"))
                dataType = "COLUMN_DATA_TYPE";

            if (!first.ContainsKey(charMax) &&
                first.ContainsKey("COLUMN_SIZE"))
                charMax = "COLUMN_SIZE";

            foreach (IDictionary<string, object> row in columns)
            {
                FieldInfo fieldInfo = new FieldInfo();
                fieldInfo.FieldName = (string)row[columnName];
                    
                order[fieldInfo.FieldName] = Convert.ToInt32(row[ordinal]);

                fieldInfo.IsPrimaryKey =
                    primaryFields.IndexOf(fieldInfo.FieldName) >= 0;
                fieldInfo.IsIdentity =
                    identityFields.IndexOf(fieldInfo.FieldName) >= 0;
                fieldInfo.IsNullable = (row[isNullable] as string == "YES") || (row[isNullable] as Boolean? == true);
                fieldInfo.DataType = row[dataType] as string;
                fieldInfo.Size = 0;

                if (fieldInfo.DataType != SqlInt &&
                    fieldInfo.DataType != SqlInteger &&
                    fieldInfo.DataType != SqlReal &&
                    fieldInfo.DataType != SqlFloat &&
                    fieldInfo.DataType != SqlTinyInt &&
                    fieldInfo.DataType != SqlSmallInt &&
                    fieldInfo.DataType != SqlBigInt &&
                    fieldInfo.DataType != SqlInt8 &&
                    fieldInfo.DataType != SqlInt4)
                {
                    var val = row[charMax];
                    var size = (val == null || val == DBNull.Value) ? (Int64?)null : Convert.ToInt64(val);
                    if (size != null && size > 0 && size <= 1000000000)
                        fieldInfo.Size = (int)size.Value;

                    val = row[numPrec];
                    var prec = (val == null || val == DBNull.Value) ? (Int64?)null : Convert.ToInt64(val);

                    string dataType2;
                    if (prec != null && (SqlTypeNameToFieldType(fieldInfo.DataType, fieldInfo.Size, out dataType2) != "String") &&
                        prec >= 0 && prec < 1000000000)
                    {
                        fieldInfo.Size = Convert.ToInt32(prec.Value);
                    }

                    val = row[numScale];
                    var scale = (val == null || val == DBNull.Value) ? (Int64?)null : Convert.ToInt64(val);
                    if (scale != null && scale >= 0 && scale < 1000000000)
                        fieldInfo.Scale = Convert.ToInt32(scale.Value);
                }

                fieldInfos.Add(fieldInfo);
            }

            fieldInfos.Sort((x, y) => order[x.FieldName].CompareTo(order[y.FieldName]));

            // şimdi ForeignKey tespiti yap, bunu önceki döngüde yapamayız reader'lar çakışır

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                ForeignKeyInfo foreignKey = singleFieldForeignKeys.Find(
                    delegate(ForeignKeyInfo d) { return d.FKColumn == fieldInfo.FieldName; });

                if (foreignKey != null)
                {
                    fieldInfo.PKSchema = foreignKey.PKSchema;
                    fieldInfo.PKTable = foreignKey.PKTable;
                    fieldInfo.PKColumn = foreignKey.PKColumn;
                }
            }

            return fieldInfos;
        }

        const string SqlBit = "bit";
        const string SqlDate = "date";
        const string SqlDateTime = "datetime";
        const string SqlDateTime2 = "datetime2";
        const string SqlSmallDateTime = "smalldatetime";
        const string SqlDateTimeOffset = "datetimeoffset";
        const string SqlDecimal = "decimal";
        const string SqlNumeric = "numeric";
        const string SqlBigInt = "bigint";
        const string SqlInt = "int";
        const string SqlInteger = "integer";
        const string SqlDouble = "double";
        const string SqlDoublePrecision = "double precision";
        const string SqlMoney = "money";
        const string SqlNChar = "nchar";
        const string SqlNVarChar = "nvarchar";
        const string SqlNText = "ntext";
        const string SqlText = "text";
        const string SqlBlobSubType1 = "blob sub_type 1";
        const string SqlReal = "real";
        const string SqlFloat = "float";
        const string SqlSmallInt = "smallint";
        const string SqlVarChar = "varchar";
        const string SqlChar = "char";
        const string SqlUniqueIdentifier = "uniqueidentifier";
        const string SqlVarBinary = "varbinary";
        const string SqlTinyInt = "tinyint";
        const string SqlTime = "time";
        const string SqlTimestamp = "timestamp";
        const string SqlRowVersion = "rowversion";
        const string SqlInt8 = "int8";
        const string SqlInt4 = "int4";

        public static string SqlTypeNameToFieldType(string sqlTypeName, int size, out string dataType)
        {
            dataType = null;

            if (sqlTypeName == SqlNVarChar || sqlTypeName == SqlNText || sqlTypeName == SqlText || sqlTypeName == SqlNChar ||
                sqlTypeName == SqlVarChar || sqlTypeName == SqlChar || sqlTypeName == SqlBlobSubType1)
                return "String";
            else if (sqlTypeName == SqlInt || sqlTypeName == SqlInteger || sqlTypeName == SqlInt4)
                return "Int32";
            else if (sqlTypeName == SqlBigInt || sqlTypeName == SqlInt8)
                return "Int64";
            else if (sqlTypeName == SqlMoney || sqlTypeName == SqlDecimal || sqlTypeName == SqlNumeric)
                return "Decimal";
            else if (sqlTypeName == SqlDateTime || sqlTypeName == SqlDateTime2 || sqlTypeName == SqlDate || sqlTypeName == SqlSmallDateTime)
                return "DateTime";
            else if (sqlTypeName == SqlDateTimeOffset)
                return "DateTimeOffset";
            else if (sqlTypeName == SqlTime)
                return "TimeSpan";
            else if (sqlTypeName == SqlBit)
                return "Boolean";
            else if (sqlTypeName == SqlReal)
                return "Single";
            else if (sqlTypeName == SqlFloat || sqlTypeName == SqlDouble || sqlTypeName == SqlDoublePrecision)
                return "Double";
            else if (sqlTypeName == SqlSmallInt || sqlTypeName == SqlTinyInt)
                return "Int16";
            else if (sqlTypeName == SqlUniqueIdentifier)
                return "Guid";
            else if (sqlTypeName == SqlVarBinary)
            {
                if (size == 0 || size > 256)
                    return "Stream";

                dataType = "byte[]";
                return "ByteArray";
            }
            else if (sqlTypeName == SqlTimestamp || sqlTypeName == SqlRowVersion)
            {
                dataType = "byte[]";
                return "ByteArray";
            }
            else
                return "Stream";
        }
    }
}