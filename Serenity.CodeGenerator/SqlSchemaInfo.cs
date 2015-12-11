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
                
                if (tableType != null && tableType.ToLowerInvariant() == "view")
                    continue;

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

            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
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

            if (primaryFields.Count == 0)
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

            if (columns.Columns.Contains("AUTOINCREMENT"))
                foreach (DataRow row in columns.Rows)
                {
                    var isIdentity = row["AUTOINCREMENT"] as Boolean?;
                    if (isIdentity == true)
                        identityFields.Add((string)row["COLUMN_NAME"]);
                }
            else
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
                .ForEach(connection, delegate(IDataReader reader)
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

            try
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
            catch (Exception)
            {

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

            foreach (DataRow row in columns.Rows)
            {

            /*new SqlQuery().Select("COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE")
                .From(inf + "COLUMNS")
                .Where(new Criteria("TABLE_NAME") == tableName)
                .OrderBy("ORDINAL_POSITION")
                .ForEach(connection, delegate(IDataReader reader)*/
                {
                    FieldInfo fieldInfo = new FieldInfo();
                    fieldInfo.FieldName = (string)row["COLUMN_NAME"];
                    order[fieldInfo.FieldName] = (int)row["ORDINAL_POSITION"];

                    fieldInfo.IsPrimaryKey =
                        primaryFields.IndexOf(fieldInfo.FieldName) >= 0;
                    fieldInfo.IsIdentity =
                        identityFields.IndexOf(fieldInfo.FieldName) >= 0;
                    fieldInfo.IsNullable = (row["IS_NULLABLE"] as string == "YES") || (row["IS_NULLABLE"] as Boolean? == true);
                    fieldInfo.DataType = row["DATA_TYPE"] as string;
                    fieldInfo.Size = 0;

                    if (fieldInfo.DataType != SqlInt &&
                        fieldInfo.DataType != SqlInteger &&
                        fieldInfo.DataType != SqlReal &&
                        fieldInfo.DataType != SqlFloat &&
                        fieldInfo.DataType != SqlTinyInt &&
                        fieldInfo.DataType != SqlSmallInt &&
                        fieldInfo.DataType != SqlBigInt)
                    {
                        var size = row["CHARACTER_MAXIMUM_LENGTH"] as Int32?;
                        if (size != null)
                        {
                            fieldInfo.Size = size.Value;
                            if (fieldInfo.Size < 0 || fieldInfo.Size >= 1000000000)
                                fieldInfo.Size = 0;
                        }

                        var prec = row["NUMERIC_PRECISION"] as Int32?;

                        if (prec != null)
                        {
                            fieldInfo.Size = Convert.ToInt32(prec.Value);
                            if (fieldInfo.Size < 0 || fieldInfo.Size >= 1000000000)
                                fieldInfo.Size = 0;
                        }

                        var scale = row["NUMERIC_SCALE"] as Int32?;
                        if (scale != null)
                        {
                            fieldInfo.Scale = Convert.ToInt32(scale.Value);
                            if (fieldInfo.Scale < 0 || fieldInfo.Scale >= 1000000000)
                                fieldInfo.Scale = 0;
                        }
                    }

                    fieldInfos.Add(fieldInfo);
                }
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
        const string SqlDateTime = "datetime";
        const string SqlSmallDateTime = "smalldatetime";
        const string SqlDecimal = "decimal";
        const string SqlNumeric = "numeric";
        const string SqlBigInt = "bigint";
        const string SqlInt = "int";
        const string SqlInteger = "integer";
        const string SqlDouble = "double";
        const string SqlMoney = "money";
        const string SqlNChar = "nchar";
        const string SqlNVarChar = "nvarchar";
        const string SqlNText = "ntext";
        const string SqlReal = "real";
        const string SqlFloat = "float";
        const string SqlSmallInt = "smallint";
        const string SqlVarChar = "varchar";
        const string SqlChar = "char";
        const string SqlUniqueIdentifier = "uniqueidentifier";
        const string SqlVarBinary = "varbinary";
        const string SqlTinyInt = "tinyint";
        const string SqlTimestamp = "timestamp";

        public static string SqlTypeNameToFieldType(string sqlTypeName)
        {
            if (sqlTypeName == SqlNVarChar || sqlTypeName == SqlNText || sqlTypeName == SqlNChar || sqlTypeName == SqlVarChar || sqlTypeName == SqlChar)
                return "String";
            else if (sqlTypeName == SqlInt || sqlTypeName == SqlInteger)
                return "Int32";
            else if (sqlTypeName == SqlBigInt || sqlTypeName == SqlTimestamp)
                return "Int64";
            else if (sqlTypeName == SqlMoney || sqlTypeName == SqlDecimal || sqlTypeName == SqlNumeric)
                return "Decimal";
            else if (sqlTypeName == SqlDateTime)
                return "DateTime";
            else if (sqlTypeName == SqlSmallDateTime)
                return "DateTime";
            else if (sqlTypeName == SqlBit)
                return "Boolean";
            else if (sqlTypeName == SqlReal)
                return "Single";
            else if (sqlTypeName == SqlFloat || sqlTypeName == SqlDouble)
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