using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class SqlSchemaInfo
    {
        public static string InformationSchema(IDbConnection connection)
        {
            return "INFORMATION_SCHEMA.";
        }


        public static List<Tuple<string, string>> GetTableNames(IDbConnection connection)
        {
            var tables = ((DbConnection)(((WrappedConnection)connection)).ActualConnection).GetSchema("Tables");

            var result = new List<Tuple<string, string>>();

            foreach (DataRow row in tables.Rows)
            {
                var tableType = row["TABLE_TYPE"] as string;
                
                //if (tableType != null && tableType.ToLowerInvariant() == "view")
                //    continue;

                var schema = row["TABLE_SCHEMA"] as string;
                var tableName = row["TABLE_NAME"] as string;

                result.Add(new Tuple<string, string>(schema, tableName));
            }

            result.Sort();

            return result;
        }

        public static List<string> GetTablePrimaryFields(IDbConnection connection, string schema, string tableName)
        {
            var inf = InformationSchema(connection);
            List<string> primaryFields = new List<string>();

            if (connection.GetDialect() is FirebirdDialect)
            {
                var query = @"
select
cast(RC.RDB$RELATION_NAME as varchar(31)) as TABLE_NAME,
cast(ISGMT.RDB$FIELD_NAME as varchar(31)) as COLUMN_NAME,
CAST((ISGMT.RDB$FIELD_POSITION + 1) as SMALLINT) as KEY_SEQ,
cast(RC.RDB$CONSTRAINT_NAME as varchar(31)) as PK_NAME
from
RDB$RELATION_CONSTRAINTS RC
INNER JOIN RDB$INDEX_SEGMENTS ISGMT ON RC.RDB$INDEX_NAME = ISGMT.RDB$INDEX_NAME
where CAST(RC.RDB$RELATION_NAME AS VARCHAR(40)) = '{0}' and
RC.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
order by 3";

                foreach (var k in connection.Query(String.Format(query, tableName)))
                {
                    primaryFields.Add(((string)k.COLUMN_NAME).TrimEnd());
                }

                return primaryFields;
            }

            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
            if (columns.Columns.Contains("PRIMARY_KEY"))
            {
                foreach (DataRow row in columns.Rows)
                {
                    try
                    {
                        var isPrimaryKey = row["PRIMARY_KEY"] as Boolean?;
                        if (isPrimaryKey == true)
                            primaryFields.Add((string)row["COLUMN_NAME"]);
                    }
                    catch (Exception)
                    {

                    }
                }

                return primaryFields;
            }

            if (connection.GetDialect() is MySqlDialect ||
                connection.GetDialect().GetType().Name.StartsWith("SqlServer"))
            {
                var query = new SqlQuery().Select(
                        "KCU.COLUMN_NAME")
                    .From(
                        inf + "TABLE_CONSTRAINTS AS TC INNER JOIN " +
                        inf + "KEY_COLUMN_USAGE AS KCU " +
                        "ON KCU.CONSTRAINT_SCHEMA = TC.CONSTRAINT_SCHEMA AND " +
                        "KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME AND " +
                        "KCU.TABLE_SCHEMA = TC.TABLE_SCHEMA AND " +
                        "KCU.TABLE_NAME = TC.TABLE_NAME")
                    .Where(
                        new Criteria("TC.CONSTRAINT_TYPE") == "PRIMARY KEY" &
                        new Criteria("KCU.TABLE_NAME") == tableName)
                    .OrderBy(
                        "KCU.ORDINAL_POSITION");

                query.ForEach(connection, delegate(IDataReader reader)
                {
                    primaryFields.Add(reader.GetString(0));
                });
            }

            return primaryFields;
        }


        public static List<string> GetTableIdentityFields(IDbConnection connection, string schema, string tableName)
        {
            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
            List<string> identityFields = new List<string>();

            if (connection.GetDialect() is FirebirdDialect)
            {
                return new List<string>();
            }

            if (connection.GetDialect() is PostgresDialect)
            {
                foreach (DataRow row in columns.Rows)
                {
                    var defaultValue = row["column_default"] as string;
                    if (defaultValue != null && defaultValue.IndexOf("nextval(") > 0)
                        identityFields.Add((string)row["COLUMN_NAME"]);
                }

                return identityFields;
            }

            if (connection.GetDialect() is MySqlDialect)
            {
                foreach (DataRow row in columns.Rows)
                {
                    var isIdentity = (row["EXTRA"] as string) == "auto_increment";
                    if (isIdentity == true)
                        identityFields.Add((string)row["COLUMN_NAME"]);
                }

                return identityFields;
            }

            if (columns.Columns.Contains("AUTOINCREMENT"))
            {
                foreach (DataRow row in columns.Rows)
                {
                    var isIdentity = row["AUTOINCREMENT"] as Boolean?;
                    if (isIdentity == true)
                        identityFields.Add((string)row["COLUMN_NAME"]);
                }

                return identityFields;
            }


            if (connection.GetDialect().GetType().Name.StartsWith("SqlServer"))
            {
                new SqlQuery().Select(
                    "C.NAME")
                .From(
                    "syscolumns C " +
                    "LEFT OUTER JOIN sysobjects T " +
                    "ON (C.id = T.id)")
                .Where(
                    new Criteria("C.STATUS & 128") == 128 &
                    new Criteria("T.NAME") == tableName &
                    new Criteria("T.XTYPE") == "U")
                .ForEach(connection, delegate (IDataReader reader)
                {
                    identityFields.Add(reader.GetString(0));
                });
            }

            return identityFields;
        }

        public static List<string> GetTableFieldNames(IDbConnection connection, string schema, string tableName)
        {
            var inf = InformationSchema(connection);
            var list = new List<string>();

            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
            var dict = new Dictionary<string, int>();
            foreach (DataRow row in columns.Rows)
            {
                var col = (string)row["COLUMN_NAME"];
                dict[col] = (int)row["ORDINAL_POSITION"];
                list.Add(col);
            }
            list.Sort((x, y) => dict[x].CompareTo(dict[y]));

            return list;
        }

        public static List<ForeignKeyInfo> GetTableSingleFieldForeignKeys(IDbConnection connection, string schema, string tableName)
        {
            var inf = InformationSchema(connection);
            List<ForeignKeyInfo> foreignKeyInfos = new List<ForeignKeyInfo>();

            if (connection.GetDialect() is SqliteDialect)
            {
                try
                {
                    using (var reader = 
                        SqlHelper.ExecuteReader(connection, 
                        (String.Format("PRAGMA foreign_key_list({0});", tableName))))
                    {
                        while (reader.Read())
                        {
                            ForeignKeyInfo foreignKeyInfo = new ForeignKeyInfo();

                            foreignKeyInfo.FKTable = tableName;
                            foreignKeyInfo.FKColumn = reader.GetString(3);
                            foreignKeyInfo.PKTable = reader.GetString(2);
                            foreignKeyInfo.PKColumn = reader.GetString(4);

                            foreignKeyInfos.Add(foreignKeyInfo);
                        }
                    }
                }
                catch (Exception)
                {
                }

                return foreignKeyInfos;
            }

            if (connection.GetDialect() is FirebirdDialect)
            {
                var query = @"
select 
 PK.RDB$RELATION_NAME as PKTABLE_NAME
,ISP.RDB$FIELD_NAME as PKCOLUMN_NAME
,FK.RDB$RELATION_NAME as FKTABLE_NAME
,ISF.RDB$FIELD_NAME as FKCOLUMN_NAME
,(ISP.RDB$FIELD_POSITION + 1) as KEY_SEQ
,RC.RDB$UPDATE_RULE as UPDATE_RULE
,RC.RDB$DELETE_RULE as DELETE_RULE
,PK.RDB$CONSTRAINT_NAME as PK_NAME
,FK.RDB$CONSTRAINT_NAME as FK_NAME
from
 RDB$RELATION_CONSTRAINTS PK
,RDB$RELATION_CONSTRAINTS FK
,RDB$REF_CONSTRAINTS RC
,RDB$INDEX_SEGMENTS ISP
,RDB$INDEX_SEGMENTS ISF
WHERE FK.RDB$RELATION_NAME = '{0}' and 
 FK.RDB$CONSTRAINT_NAME = RC.RDB$CONSTRAINT_NAME 
and PK.RDB$CONSTRAINT_NAME = RC.RDB$CONST_NAME_UQ 
and ISP.RDB$INDEX_NAME = PK.RDB$INDEX_NAME 
and ISF.RDB$INDEX_NAME = FK.RDB$INDEX_NAME 
and ISP.RDB$FIELD_POSITION = ISF.RDB$FIELD_POSITION 
order by 1, 5";

                try
                {
                    using (var reader =
                        SqlHelper.ExecuteReader(connection,
                        (String.Format(query, tableName))))
                    {
                        while (reader.Read())
                        {
                            ForeignKeyInfo foreignKeyInfo = new ForeignKeyInfo();

                            foreignKeyInfo.FKTable = tableName;
                            foreignKeyInfo.FKColumn = reader.GetString(3).TrimEnd();
                            foreignKeyInfo.PKTable = reader.GetString(0).TrimEnd();
                            foreignKeyInfo.PKColumn = reader.GetString(1).TrimEnd();

                            foreignKeyInfos.Add(foreignKeyInfo);
                        }
                    }
                }
                catch (Exception)
                {
                }

                return foreignKeyInfos;
            }

            if (connection.GetDialect() is PostgresDialect)
            {
                try
                {
                    var list = connection.Query(
                        @"SELECT * FROM (
                            SELECT
                                o.conname AS constraint_name,
                                (SELECT nspname FROM pg_namespace WHERE oid=m.relnamespace) AS source_schema,
                                m.relname AS source_table,
                                (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = m.oid AND a.attnum = o.conkey[1] AND a.attisdropped = false) AS source_column,
                                (SELECT nspname FROM pg_namespace WHERE oid=f.relnamespace) AS target_schema,
                                f.relname AS target_table,
                                (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = f.oid AND a.attnum = o.confkey[1] AND a.attisdropped = false) AS target_column
                            FROM
                                pg_constraint o LEFT JOIN pg_class c ON c.oid = o.conrelid
                                LEFT JOIN pg_class f ON f.oid = o.confrelid LEFT JOIN pg_class m ON m.oid = o.conrelid
                            WHERE
                                o.contype = 'f' AND o.conrelid IN (SELECT oid FROM pg_class c WHERE c.relkind = 'r')) x
                        WHERE source_schema = @sh AND source_table = @tb
                        ORDER BY constraint_name", new { sh = schema, tb = tableName }).ToList();


                    foreach (var fk in list)
                    {
                        string priorName = "";
                        bool priorDeleted = false;

                        // eğer bir önceki ile aynıysa bunu listeye ekleme ve öncekini de sil
                        var fkName = fk.constraint_name as string;
                        if (priorName == fkName)
                        {
                            if (!priorDeleted)
                            {
                                foreignKeyInfos.RemoveAt(foreignKeyInfos.Count - 1);
                                priorDeleted = true;
                            }
                            continue;
                        }

                        var foreignKeyInfo = new ForeignKeyInfo
                        {
                            FKName = fkName,
                            FKSchema = fk.source_schema,
                            FKTable = fk.source_table,
                            FKColumn = fk.source_column,
                            PKSchema = fk.target_schema,
                            PKTable = fk.target_table,
                            PKColumn = fk.target_column
                        };

                        foreignKeyInfos.Add(foreignKeyInfo);
                        priorDeleted = false;
                        priorName = foreignKeyInfo.FKName;
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                }

                return foreignKeyInfos;
            }

            if (connection.GetDialect() is MySqlDialect)
            {
                using (var reader =
                    SqlHelper.ExecuteReader(connection,
                    (String.Format(@"
                        SELECT k.COLUMN_NAME, k.REFERENCED_TABLE_NAME, k.REFERENCED_COLUMN_NAME
                        FROM information_schema.TABLE_CONSTRAINTS i
                        LEFT JOIN information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                        WHERE i.CONSTRAINT_TYPE = 'FOREIGN KEY'
                        AND i.TABLE_SCHEMA = '{0}'
                        AND i.TABLE_NAME = '{1}'"
                    , schema, tableName))))
                {
                    while (reader.Read())
                    {
                        ForeignKeyInfo foreignKeyInfo = new ForeignKeyInfo();

                        foreignKeyInfo.FKTable = tableName;
                        foreignKeyInfo.FKColumn = reader.GetString(0);
                        foreignKeyInfo.PKTable = reader.GetString(1);
                        foreignKeyInfo.PKColumn = reader.GetString(2);
                        foreignKeyInfos.Add(foreignKeyInfo);
                    }
                }

                return foreignKeyInfos;
            }

            if (connection.GetDialect().GetType().Name.StartsWith("SqlServer"))
            {
                var c = new Alias(inf + "REFERENTIAL_CONSTRAINTS", "c");
                var fk = new Alias(inf + "CONSTRAINT_COLUMN_USAGE", "fk");
                var pk = new Alias(inf + "KEY_COLUMN_USAGE", "pk");

                var list = connection.Query<ForeignKeyInfo>(new SqlQuery()
                    .From(c)
                    .From(fk)
                    .From(pk)
                    .Where(
                        fk._("TABLE_SCHEMA") == schema &
                        fk._("TABLE_NAME") == tableName &
                        fk._("CONSTRAINT_SCHEMA") == c._("CONSTRAINT_SCHEMA") &
                        fk._("CONSTRAINT_NAME") == c._("CONSTRAINT_NAME") &
                        pk._("CONSTRAINT_SCHEMA") == c._("UNIQUE_CONSTRAINT_SCHEMA") &
                        pk._("CONSTRAINT_NAME") == c._("UNIQUE_CONSTRAINT_NAME"))
                    .Select(fk["CONSTRAINT_NAME"], "FKName")
                    .Select(fk["TABLE_SCHEMA"], "FKSchema")
                    .Select(fk["TABLE_NAME"], "FKTable")
                    .Select(fk["COLUMN_NAME"], "FKColumn")
                    .Select(pk["TABLE_SCHEMA"], "PKSchema")
                    .Select(pk["TABLE_NAME"], "PKTable")
                    .Select(pk["COLUMN_NAME"], "PKColumn")
                    .OrderBy(fk["CONSTRAINT_NAME"]));
                    
                foreach (var foreignKeyInfo in list)
                {
                    string priorName = "";
                    bool priorDeleted = false;

                    // eğer bir önceki ile aynıysa bunu listeye ekleme ve öncekini de sil
                    if (priorName == foreignKeyInfo.FKName)
                    {
                        if (!priorDeleted)
                        {
                            foreignKeyInfos.RemoveAt(foreignKeyInfos.Count - 1);
                            priorDeleted = true;
                        }
                        continue;
                    }

                    foreignKeyInfos.Add(foreignKeyInfo);
                    priorDeleted = false;
                    priorName = foreignKeyInfo.FKName;
                }
            }

            return foreignKeyInfos;
        }


        public static List<FieldInfo> GetTableFieldInfos(IDbConnection connection, string schema, string tableName)
        {
            var inf = InformationSchema(connection);
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            List<ForeignKeyInfo> foreignKeys = GetTableSingleFieldForeignKeys(connection, schema, tableName);
            List<string> primaryFields = GetTablePrimaryFields(connection, schema, tableName);
            List<string> identityFields = GetTableIdentityFields(connection, schema, tableName);

            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
            var order = new Dictionary<string, int>();

            var ordinal = "ORDINAL_POSITION";
            var columnName = "COLUMN_NAME";
            var isNullable = "IS_NULLABLE";
            var charMax = "CHARACTER_MAXIMUM_LENGTH";
            var numPrec = "NUMERIC_PRECISION";
            var numScale = "NUMERIC_SCALE";
            var dataType = "DATA_TYPE";
            if (!columns.Columns.Contains(ordinal) &&
                columns.Columns.Contains(ordinal.ToLowerInvariant()))
            {
                ordinal = ordinal.ToLowerInvariant();
                columnName = columnName.ToLowerInvariant();
                dataType = dataType.ToLowerInvariant();
                isNullable = isNullable.ToLowerInvariant();
                charMax = charMax.ToLowerInvariant();
                numPrec = numPrec.ToLowerInvariant();
                numScale = numScale.ToLowerInvariant();
            }

            if (!columns.Columns.Contains(dataType) &&
                columns.Columns.Contains("COLUMN_DATA_TYPE"))
                dataType = "COLUMN_DATA_TYPE";

            if (!columns.Columns.Contains(charMax) &&
                columns.Columns.Contains("COLUMN_SIZE"))
                charMax = "COLUMN_SIZE";

            foreach (DataRow row in columns.Rows)
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

                    if (prec != null && (SqlTypeNameToFieldType(fieldInfo.DataType) != "String") &&
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
                ForeignKeyInfo foreignKey = foreignKeys.Find(
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

        public class FieldInfo
        {
            public string FieldName;
            public int Size;
            public int Scale;
            public bool IsPrimaryKey;
            public bool IsIdentity;
            public bool IsNullable;
            public string PKSchema;
            public string PKTable;
            public string PKColumn;
            public string DataType;
        }

        public class ForeignKeyInfo
        {
            public string FKName;
            public string FKSchema;
            public string FKTable;
            public string FKColumn;
            public string PKSchema;
            public string PKTable;
            public string PKColumn;
        }

        const string SqlBit = "bit";
        const string SqlDate = "date";
        const string SqlDateTime = "datetime";
        const string SqlSmallDateTime = "smalldatetime";
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
        const string SqlInt8 = "int8";
        const string SqlInt4 = "int4";

        public static string SqlTypeNameToFieldType(string sqlTypeName)
        {
            if (sqlTypeName == SqlNVarChar || sqlTypeName == SqlNText || sqlTypeName == SqlNChar || 
                sqlTypeName == SqlVarChar || sqlTypeName == SqlChar || sqlTypeName == SqlBlobSubType1)
                return "String";
            else if (sqlTypeName == SqlInt || sqlTypeName == SqlInteger || sqlTypeName == SqlInt4)
                return "Int32";
            else if (sqlTypeName == SqlBigInt || sqlTypeName == SqlTimestamp || sqlTypeName == SqlInt8)
                return "Int64";
            else if (sqlTypeName == SqlMoney || sqlTypeName == SqlDecimal || sqlTypeName == SqlNumeric)
                return "Decimal";
            else if (sqlTypeName == SqlDateTime || sqlTypeName == SqlDate || sqlTypeName == SqlSmallDateTime)
                return "DateTime";
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
                return "Stream";
            else
                return "Stream";
        }
    }
}