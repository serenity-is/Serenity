using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.CodeGenerator
{
    public class SqlSchemaInfo
    {
        public static List<string> GetTableNames(IDbConnection connection)
        {
            var tables = new List<string>();

            using (var reader = SqlHelper.ExecuteReader(connection,
                "SELECT NAME FROM SYSOBJECTS WHERE XTYPE='U' ORDER BY NAME"))
            {
                while (reader.Read())
                    tables.Add(reader.GetString(0));
            }
            return tables;
        }

        public static List<string> GetTablePrimaryFields(IDbConnection connection, string tableName)
        {
            List<string> primaryFields = new List<string>();

            new SqlQuery().Select(
                "KCU.COLUMN_NAME")
            .From(
                "INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC INNER JOIN " +
                "INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU " +
                "ON KCU.CONSTRAINT_SCHEMA = TC.CONSTRAINT_SCHEMA AND " +
                "KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME AND " +
                "KCU.TABLE_SCHEMA = TC.TABLE_SCHEMA AND " +
                "KCU.TABLE_NAME = TC.TABLE_NAME")
            .Where(
                new Criteria("TC.CONSTRAINT_TYPE") == "PRIMARY KEY" &
                new Criteria("KCU.TABLE_NAME") == tableName)
            .OrderBy(
                "KCU.ORDINAL_POSITION")
            .ForEach(connection, delegate(IDataReader reader)
            {
                primaryFields.Add(reader.GetString(0));
            });

            return primaryFields;
        }


        public static List<string> GetTableIdentityFields(IDbConnection connection, string tableName)
        {
            List<string> identityFields = new List<string>();

            new SqlQuery().Select(
                "C.NAME")
            .From(
                "SYSCOLUMNS C " +
                "LEFT OUTER JOIN SYSOBJECTS T " +
                "ON (C.id = T.id)")
            .Where(
                new Criteria("C.STATUS & 128") == 128 &
                new Criteria("T.NAME") == tableName &
                new Criteria("T.XTYPE") == "U")
            .ForEach(connection, delegate(IDataReader reader)
            {
                identityFields.Add(reader.GetString(0));
            });

            return identityFields;
        }


        public static List<string> GetTableFieldNames(IDbConnection connection, string tableName)
        {
            var list = new List<string>();

            new SqlQuery().Select("COLUMN_NAME").From("INFORMATION_SCHEMA.COLUMNS")
                .Where(new Criteria("TABLE_NAME") == tableName)
                .OrderBy("ORDINAL_POSITION")
                .ForEach(connection, delegate(IDataReader reader)
                {
                    list.Add(reader.GetString(0));
                });

            return list;
        }

        public static List<ForeignKeyInfo> GetTableSingleFieldForeignKeys(IDbConnection connection, string tableName)
        {
            List<ForeignKeyInfo> foreignKeyInfos = new List<ForeignKeyInfo>();

            // çift (ya da daha çok) alanlı constraint ler yapımıza uymuyor
            // bu yüzden constraint adına göre sıralı liste alıyoruz
            // tarama sırasında tek bir constraint'te birden fazla
            // alan bulduysak bu constraint hiç yokmuş gibi davranacağız
            using (var reader = SqlHelper.ExecuteReader(connection, (String.Format(
                "SELECT CCU.CONSTRAINT_NAME SRC_CONSTRAINT, CCU.COLUMN_NAME SRC_COL, " +
                "KCU.TABLE_NAME FOREIGN_TABLE, KCU.COLUMN_NAME FOREIGN_COL " +
                "FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU, " +
                "INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC, " +
                "INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU " +
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

            return foreignKeyInfos;
        }


        public static List<FieldInfo> GetTableFieldInfos(IDbConnection connection, string tableName)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            List<ForeignKeyInfo> foreignKeys = GetTableSingleFieldForeignKeys(connection, tableName);
            List<string> primaryFields = GetTablePrimaryFields(connection, tableName);
            List<string> identityFields = GetTableIdentityFields(connection, tableName);

            new SqlQuery().Select("COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE")
                .From("INFORMATION_SCHEMA.COLUMNS")
                .Where(new Criteria("TABLE_NAME") == tableName)
                .OrderBy("ORDINAL_POSITION")
                .ForEach(connection, delegate(IDataReader reader)
                {
                    FieldInfo fieldInfo = new FieldInfo();
                    fieldInfo.FieldName = reader.GetString(0);
                    fieldInfo.IsPrimaryKey =
                        primaryFields.IndexOf(fieldInfo.FieldName) >= 0;
                    fieldInfo.IsIdentity =
                        identityFields.IndexOf(fieldInfo.FieldName) >= 0;
                    fieldInfo.IsNullable = reader.GetString(2) == "YES";
                    fieldInfo.DataType = reader.GetString(1);
                    fieldInfo.Size = 0;

                    if (fieldInfo.DataType != SqlInt &&
                        fieldInfo.DataType != SqlReal &&
                        fieldInfo.DataType != SqlFloat &&
                        fieldInfo.DataType != SqlTinyInt &&
                        fieldInfo.DataType != SqlSmallInt &&
                        fieldInfo.DataType != SqlBigInt)
                    {
                        if (!reader.IsDBNull(3))
                        {
                            fieldInfo.Size = reader.GetInt32(3);
                            if (fieldInfo.Size < 0 || fieldInfo.Size >= 1000000000)
                                fieldInfo.Size = 0;
                        }

                        if (!reader.IsDBNull(4))
                        {
                            fieldInfo.Size = Convert.ToInt32(reader.GetValue(4));
                            if (fieldInfo.Size < 0 || fieldInfo.Size >= 1000000000)
                                fieldInfo.Size = 0;
                        }

                        if (!reader.IsDBNull(5))
                        {
                            fieldInfo.Scale = Convert.ToInt32(reader.GetValue(5));
                            if (fieldInfo.Scale < 0 || fieldInfo.Scale >= 1000000000)
                                fieldInfo.Scale = 0;
                        }
                    }

                    fieldInfos.Add(fieldInfo);
                });

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
        const string SqlDecimal = "decimal";
        const string SqlNumeric = "numeric";
        const string SqlBigInt = "bigint";
        const string SqlInt = "int";
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
            else if (sqlTypeName == SqlInt)
                return "Int32";
            else if (sqlTypeName == SqlBigInt || sqlTypeName == SqlTimestamp)
                return "Int64";
            else if (sqlTypeName == SqlMoney || sqlTypeName == SqlDecimal || sqlTypeName == SqlNumeric)
                return "Decimal";
            else if (sqlTypeName == SqlDateTime)
                return "DateTime";
            else if (sqlTypeName == SqlBit)
                return "Boolean";
            else if (sqlTypeName == SqlReal)
                return "Single";
            else if (sqlTypeName == SqlFloat)
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