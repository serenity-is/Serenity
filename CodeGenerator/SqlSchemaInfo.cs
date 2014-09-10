using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Serenity.CodeGenerator
{
    public class SqlSchemaInfo
    {
        public static SqlDialect Dialect { get; set; } 

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
            List<string> identityFields = new List<string>();

            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
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
            foreach (DataRow row in columns.Rows)
            {
                list.Add((string)row["COLUMN_NAME"]);
            }

            return list;
        }

        public static List<ForeignKeyInfo> GetTableSingleFieldForeignKeys(IDbConnection connection, string tableName)
        {
            var inf = InformationSchema(connection);
            List<ForeignKeyInfo> foreignKeyInfos = new List<ForeignKeyInfo>();

            if (Dialect.HasFlag(SqlDialect.Sqlite))
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

                            foreignKeyInfo.SourceTable = tableName;
                            foreignKeyInfo.SourceColumn = reader.GetString(3);
                            foreignKeyInfo.ForeignTable = reader.GetString(2);
                            foreignKeyInfo.ForeignColumn = reader.GetString(4);

                            foreignKeyInfos.Add(foreignKeyInfo);
                        }
                    }
                }
                catch (Exception)
                {
                }

                return foreignKeyInfos;
            }

            try
            {
                // çift (ya da daha çok) alanlı constraint ler yapımıza uymuyor
                // bu yüzden constraint adına göre sıralı liste alıyoruz
                // tarama sırasında tek bir constraint'te birden fazla
                // alan bulduysak bu constraint hiç yokmuş gibi davranacağız
                using (var reader = SqlHelper.ExecuteReader(connection, (String.Format(
                    "SELECT CCU.CONSTRAINT_NAME SRC_CONSTRAINT, CCU.COLUMN_NAME SRC_COL, " +
                    "KCU.TABLE_NAME FOREIGN_TABLE, KCU.COLUMN_NAME FOREIGN_COL " +
                    "FROM " + inf + "CONSTRAINT_COLUMN_USAGE CCU, " +
                    inf + "REFERENTIAL_CONSTRAINTS RC, " +
                    inf + "KEY_COLUMN_USAGE KCU " +
                    "WHERE CCU.TABLE_NAME = {0} AND " +
                    "CCU.CONSTRAINT_NAME = RC.CONSTRAINT_NAME AND " +
                    "KCU.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME " +
                    "ORDER BY CCU.CONSTRAINT_NAME", tableName.ToSql()))))
                {
                    string priorConstraint = "";
                    bool priorDeleted = false;

                    while (reader.Read())
                    {
                        ForeignKeyInfo foreignKeyInfo = new ForeignKeyInfo();

                        foreignKeyInfo.SourceConstraint = reader.GetString(0);

                        // eğer bir önceki ile aynıysa bunu listeye ekleme ve öncekini de sil
                        if (priorConstraint == foreignKeyInfo.SourceConstraint)
                        {
                            if (!priorDeleted)
                            {
                                foreignKeyInfos.RemoveAt(foreignKeyInfos.Count - 1);
                                priorDeleted = true;
                            }
                            continue;
                        }

                        foreignKeyInfo.SourceTable = tableName;
                        foreignKeyInfo.SourceColumn = reader.GetString(1);
                        foreignKeyInfo.ForeignTable = reader.GetString(2);
                        foreignKeyInfo.ForeignColumn = reader.GetString(3);

                        foreignKeyInfos.Add(foreignKeyInfo);
                        priorDeleted = false;
                        priorConstraint = foreignKeyInfo.SourceConstraint;
                    }
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
            List<ForeignKeyInfo> foreignKeys = GetTableSingleFieldForeignKeys(connection, tableName);
            List<string> primaryFields = GetTablePrimaryFields(connection, schema, tableName);
            List<string> identityFields = GetTableIdentityFields(connection, schema, tableName);

            var columns = ((DbConnection)((WrappedConnection)connection).ActualConnection).GetSchema("Columns", new string[] { null, schema, tableName, null });
            foreach (DataRow row in columns.Rows)
            {

            /*new SqlQuery().Select("COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE")
                .From(inf + "COLUMNS")
                .Where(new Criteria("TABLE_NAME") == tableName)
                .OrderBy("ORDINAL_POSITION")
                .ForEach(connection, delegate(IDataReader reader)*/
                {
                    FieldInfo fieldInfo = new FieldInfo();
                    fieldInfo.FieldName = row["COLUMN_NAME"] as string;
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

            // şimdi ForeignKey tespiti yap, bunu önceki döngüde yapamayız reader'lar çakışır

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                ForeignKeyInfo foreignKey = foreignKeys.Find(
                    delegate(ForeignKeyInfo d) { return d.SourceColumn == fieldInfo.FieldName; });

                if (foreignKey != null)
                {
                    fieldInfo.ForeignTable = foreignKey.ForeignTable;
                    fieldInfo.ForeignField = foreignKey.ForeignColumn;
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
            public string ForeignTable;
            public string ForeignField;
            public string DataType;
        }

        public class ForeignKeyInfo
        {
            public string SourceTable;
            public string SourceConstraint;
            public string SourceColumn;
            public string ForeignTable;
            public string ForeignColumn;
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