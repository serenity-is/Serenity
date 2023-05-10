namespace Serenity.Data.Schema;

/// <summary>
/// Firebird metadata provider.
/// </summary>
/// <seealso cref="ISchemaProvider" />
public class FirebirdSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string DefaultSchema => null;

    private class FieldInfoSource
    {
        public string FIELD_NAME { get; set; }
        public string FIELD_TYPE { get; set; }
        public string FIELD_SUB_TYPE { get; set; }
        public string NUMERIC_SCALE { get; set; }
        public string NUMERIC_PRECISION { get; set; }
        public string SIZE { get; set; }
        public string CHARMAXLENGTH { get; set; }
        public string COLUMN_NULLABLE { get; set; }
    }

    /// <summary>
    /// Gets the field infos.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table)
    {
        return connection.Query<FieldInfoSource>(@"
                SELECT
                    rfr.rdb$field_name AS FIELD_NAME,
                    fld.rdb$field_type AS FIELD_TYPE,
                    fld.rdb$field_sub_type AS FIELD_SUB_TYPE,
                    CAST(fld.rdb$field_length AS integer) AS SIZE,
                    CAST(fld.rdb$field_precision AS integer) AS NUMERIC_PRECISION,
                    CAST(fld.rdb$field_scale AS integer) AS NUMERIC_SCALE,
                    CAST(fld.rdb$character_length AS integer) AS CHARMAXLENGTH,
                    coalesce(fld.rdb$null_flag, rfr.rdb$null_flag) AS COLUMN_NULLABLE
                FROM rdb$relation_fields rfr
                    LEFT JOIN rdb$fields fld ON rfr.rdb$field_source = fld.rdb$field_name
                WHERE
                    rfr.rdb$relation_name = @tbl
                ORDER BY 
                    rfr.rdb$field_position", new
        {
            tbl = table
        }).Select(src =>
        {
            var fi = new FieldInfo
            {
                FieldName = ((string)src.FIELD_NAME).TrimToNull()
            };
            var fieldType = src.FIELD_TYPE == null ? 0 : Convert.ToInt32(src.FIELD_TYPE, CultureInfo.InvariantCulture);
            var fieldSubType = src.FIELD_SUB_TYPE == null ? 0 : Convert.ToInt32(src.FIELD_SUB_TYPE, CultureInfo.InvariantCulture);
            var numericScale = src.NUMERIC_SCALE == null ? 0 : Convert.ToInt32(src.NUMERIC_SCALE, CultureInfo.InvariantCulture);
            var numericPrecision = src.NUMERIC_PRECISION == null ? 0 : Convert.ToInt32(src.NUMERIC_PRECISION, CultureInfo.InvariantCulture);
            var size = src.SIZE == null ? 0 : Convert.ToInt32(src.SIZE, CultureInfo.InvariantCulture);
            var sqlType = GetSqlTypeFromBlrType(fieldType, fieldSubType, size, numericScale);
            fi.DataType = sqlType;

            if (sqlType == "char" || sqlType == "varchar")
                fi.Size = src.CHARMAXLENGTH == null ? 0 : Convert.ToInt32(src.CHARMAXLENGTH);
            else if (sqlType == "varbinary" || sqlType == "text")
                fi.Size = 0;
            else if (sqlType == "decimal" || sqlType == "numeric")
            {
                fi.Size = numericPrecision;
                fi.Scale = -numericScale;
            }
            fi.IsNullable = src.COLUMN_NULLABLE == null;

            return fi;
        });
    }

    /// <summary>
    /// Gets the foreign keys.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table)
    {
        return connection.Query<ForeignKeyInfo>(@"
                SELECT
                    PK.RDB$RELATION_NAME as PKTable,
                    ISP.RDB$FIELD_NAME as PKColumn,
                    FK.RDB$CONSTRAINT_NAME as FKName
                FROM
                    RDB$RELATION_CONSTRAINTS PK, 
                    RDB$RELATION_CONSTRAINTS FK, 
                    RDB$REF_CONSTRAINTS RC, 
                    RDB$INDEX_SEGMENTS ISP, 
                    RDB$INDEX_SEGMENTS ISF
                    WHERE FK.RDB$RELATION_NAME = @tbl
                    AND FK.RDB$CONSTRAINT_NAME = RC.RDB$CONSTRAINT_NAME
                    AND PK.RDB$CONSTRAINT_NAME = RC.RDB$CONST_NAME_UQ
                    AND ISP.RDB$INDEX_NAME = PK.RDB$INDEX_NAME
                    AND ISF.RDB$INDEX_NAME = FK.RDB$INDEX_NAME
                    AND ISP.RDB$FIELD_POSITION = ISF.RDB$FIELD_POSITION
                    ORDER BY ISP.RDB$FIELD_POSITION", new
        {
            tbl = table
        }).Select(x =>
        {
            x.FKName = x.FKName.TrimToNull();
            x.FKColumn = x.FKColumn.TrimToNull();
            x.PKColumn = x.PKColumn.TrimToNull();
            x.PKTable = x.PKTable.TrimToNull();
            return x;
        });
    }

    /// <summary>
    /// Gets the identity fields.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
    {
        var match = connection.Query<string>(@"
                    SELECT RDB$GENERATOR_NAME
                    FROM RDB$GENERATORS
                    WHERE RDB$SYSTEM_FLAG = 0 AND RDB$GENERATOR_NAME LIKE @genprefix",
            new
            {
                genprefix = "GEN_" + table + "_%"
            })
            .Select(x => x[("GEN_" + table + "_").Length..])
            .FirstOrDefault()
            .TrimToNull();

        if (match == null)
        {
            var primaryKeys = GetPrimaryKeyFields(connection, schema, table);
            if (primaryKeys.Count() == 1)
                return primaryKeys;

            return new List<string>();
        }

        return connection.Query<string>(@"
                    SELECT RDB$FIELD_NAME
                    FROM RDB$RELATION_FIELDS
                    WHERE RDB$RELATION_NAME = @tbl
                    AND RDB$FIELD_NAME LIKE @match
                    ORDER BY RDB$FIELD_POSITION",
            new
            {
                tbl = table,
                match = match + "%"
            })
            .Take(1)
            .Select(StringHelper.TrimToNull);
    }

    /// <summary>
    /// Gets the primary key fields.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
    {
        return connection.Query<string>(@"
                SELECT ISGMT.RDB$FIELD_NAME FROM
                RDB$RELATION_CONSTRAINTS rc
                INNER JOIN RDB$INDEX_SEGMENTS ISGMT ON rc.RDB$INDEX_NAME = ISGMT.RDB$INDEX_NAME
                WHERE CAST(RC.RDB$RELATION_NAME AS VARCHAR(40)) = @tbl 
                    AND RC.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
                ORDER BY ISGMT.RDB$FIELD_POSITION", new { tbl = table })
                .Select(StringHelper.TrimToNull);
    }

    private class TableNameSource
    {
        public string NAME { get; set; }
        public string ISVIEW { get; set; }
    }

    /// <summary>
    /// Gets the table names.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns></returns>
    public IEnumerable<TableName> GetTableNames(IDbConnection connection)
    {
        return connection.Query<TableNameSource>(@"
                    SELECT RDB$RELATION_NAME NAME, RDB$VIEW_BLR ISVIEW
                    FROM RDB$RELATIONS  
                    WHERE (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0)")
            .Select(x => new TableName
            {
                Table = StringHelper.TrimToNull(x.NAME),
                IsView = x.ISVIEW != null
            });
    }

    private class BLRCodes
    {
        public const int blr_text = 14;
        public const int blr_text2 = 15;
        public const int blr_short = 7;
        public const int blr_long = 8;
        public const int blr_quad = 9;
        public const int blr_int64 = 16;
        public const int blr_float = 10;
        public const int blr_double = 27;
        public const int blr_d_float = 11;
        public const int blr_timestamp = 35;
        public const int blr_varying = 37;
        public const int blr_varying2 = 38;
        public const int blr_blob = 261;
        public const int blr_cstring = 40;
        public const int blr_cstring2 = 41;
        public const int blr_blob_id = 45;
        public const int blr_sql_date = 12;
        public const int blr_sql_time = 13;
        public const int blr_bool = 23;
    }

    /// <summary>
    /// Gets the type of the SQL type from BLR.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="subType">Type of the sub.</param>
    /// <param name="size">The size.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    public static string GetSqlTypeFromBlrType(int type, int subType, int size, int scale)
    {
        switch (type)
        {
            case BLRCodes.blr_varying:
            case BLRCodes.blr_varying2:
                return "varchar";

            case BLRCodes.blr_text:
            case BLRCodes.blr_text2:
            case BLRCodes.blr_cstring:
            case BLRCodes.blr_cstring2:
                if (size == 16)
                    return "guid";

                return "char";

            case BLRCodes.blr_short:
                if (subType == 2)
                    return "decimal";

                if (subType == 1)
                    return "numeric";

                if (scale < 0)
                    return "decimal";

                return "smallint";

            case BLRCodes.blr_long:
                if (subType == 2)
                    return "decimal";

                if (subType == 1)
                    return "numeric";

                if (scale < 0)
                    return "decimal";

                return "integer";

            case BLRCodes.blr_quad:
            case BLRCodes.blr_int64:
            case BLRCodes.blr_blob_id:
                if (subType == 2)
                    return "decimal";

                if (subType == 1)
                    return "numeric";

                if (scale < 0)
                    return "decimal";

                return "bigint";

            case BLRCodes.blr_float:
                return "float";

            case BLRCodes.blr_double:
            case BLRCodes.blr_d_float:
                if (subType == 2)
                    return "decimal";

                if (subType == 1)
                    return "numeric";

                if (scale < 0)
                    return "decimal";

                return "double";

            case BLRCodes.blr_blob:
                if (subType == 1)
                    return "text";

                return "varbinary";

            case BLRCodes.blr_timestamp:
                return "datetime";

            case BLRCodes.blr_sql_time:
                return "time";

            case BLRCodes.blr_sql_date:
                return "date";

            case BLRCodes.blr_bool:
                return "boolean";

            default:
                return "unknown";
        }
    }
}