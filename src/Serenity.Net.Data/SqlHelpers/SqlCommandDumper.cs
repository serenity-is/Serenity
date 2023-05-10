using Microsoft.Data.SqlClient;

namespace Serenity.Data;

/// <summary>
/// http://stackoverflow.com/questions/265192/get-the-generated-sql-statement-from-a-sqlcommand-object (answer by Mitch)
/// </summary>
public class SqlCommandDumper
{
    /// <summary>
    /// Gets the command text.
    /// </summary>
    /// <param name="sqc">The SQL command.</param>
    /// <returns></returns>
    public static string GetCommandText(SqlCommand sqc)
    {
        var sbCommandText = new StringBuilder();

        // params
        for (int i = 0; i < sqc.Parameters.Count; i++)
            LogParameterToSqlBatch(sqc.Parameters[i], sbCommandText);

        sbCommandText.AppendLine("");

        // command
        if (sqc.CommandType == CommandType.StoredProcedure)
        {
            sbCommandText.Append("EXEC ");

            bool hasReturnValue = false;
            for (int i = 0; i < sqc.Parameters.Count; i++)
            {
                if (sqc.Parameters[i].Direction == ParameterDirection.ReturnValue)
                    hasReturnValue = true;
            }
            if (hasReturnValue)
            {
                sbCommandText.Append("@returnValue = ");
            }

            sbCommandText.Append(sqc.CommandText);

            bool hasPrev = false;
            for (int i = 0; i < sqc.Parameters.Count; i++)
            {
                var cParam = sqc.Parameters[i];
                if (cParam.Direction != ParameterDirection.ReturnValue)
                {
                    if (hasPrev)
                        sbCommandText.Append(", ");

                    sbCommandText.Append(cParam.ParameterName);
                    sbCommandText.Append(" = ");
                    sbCommandText.Append(cParam.ParameterName);

                    if (cParam.Direction.HasFlag(ParameterDirection.Output))
                        sbCommandText.Append(" OUTPUT");

                    hasPrev = true;
                }
            }
        }
        else
        {
            sbCommandText.AppendLine(sqc.CommandText);
        }

        bool anyOut = false;
        foreach (SqlParameter p in sqc.Parameters)
            if (p.Direction == ParameterDirection.ReturnValue ||
                p.Direction == ParameterDirection.Output)
            {
                anyOut = true;
                break;
            }

        if (anyOut)
        {
            sbCommandText.AppendLine("-- RESULTS");
            sbCommandText.Append("SELECT 1 as Executed");
            for (int i = 0; i < sqc.Parameters.Count; i++)
            {
                var cParam = sqc.Parameters[i];

                if (cParam.Direction == ParameterDirection.ReturnValue)
                {
                    sbCommandText.Append(", @returnValue as ReturnValue");
                }
                else if (cParam.Direction.HasFlag(ParameterDirection.Output))
                {
                    sbCommandText.Append(", ");
                    sbCommandText.Append(cParam.ParameterName);
                    sbCommandText.Append(" as [");
                    sbCommandText.Append(cParam.ParameterName);
                    sbCommandText.Append(']');
                }
            }
            sbCommandText.AppendLine(";");
        }

        return sbCommandText.ToString();
    }

    private static void LogParameterToSqlBatch(SqlParameter param, StringBuilder sbCommandText)
    {
        sbCommandText.Append("DECLARE ");
        if (param.Direction == ParameterDirection.ReturnValue)
        {
            sbCommandText.AppendLine("@returnValue INT;");
        }
        else
        {
            sbCommandText.Append(param.ParameterName);

            sbCommandText.Append(' ');
            if (param.SqlDbType != SqlDbType.Structured)
            {
                LogParameterType(param, sbCommandText);
                sbCommandText.Append(" = ");
                LogQuotedParameterValue(param.Value, sbCommandText);

                sbCommandText.AppendLine(";");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    private static void LogQuotedParameterValue(object value, StringBuilder sbCommandText)
    {
        try
        {
            if (value == null || value == DBNull.Value)
            {
                sbCommandText.Append("NULL");
            }
            else
            {
                if (value is string
                    || value is char
                    || value is char[])
                {
                    sbCommandText.Append('\'');
                    sbCommandText.Append(value.ToString().Replace("'", "''"));
                    sbCommandText.Append('\'');
                }
                else if (value is bool)
                {
                    // True -> 1, False -> 0
                    sbCommandText.Append(Convert.ToInt32(value));
                }
                else if (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                {
                    sbCommandText.Append(value.ToString());
                }
                else if (value is DateTime dt)
                {
                    sbCommandText.Append(dt.ToSql(SqlServer2012Dialect.Instance));
                }
                else if (value is DateTimeOffset dto)
                {
                    sbCommandText.Append('\'');
                    sbCommandText.Append(dto.ToString("o"));
                    sbCommandText.Append('\'');
                }
                else if (value is Guid guid)
                {
                    sbCommandText.Append('\'');
                    sbCommandText.Append(guid.ToString());
                    sbCommandText.Append('\'');
                }
                else if (value is byte[] data)
                {
                    if (data.Length == 0)
                    {
                        sbCommandText.Append("NULL");
                    }
                    else
                    {
                        sbCommandText.Append("0x");
                        for (int i = 0; i < data.Length; i++)
                        {
                            sbCommandText.Append(data[i].ToString("h2"));
                        }
                    }
                }
                else
                {
                    sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                    sbCommandText.Append(value.GetType().ToString());
                    sbCommandText.Append(" *" + "/ '");
                    sbCommandText.Append(value.ToString());
                    sbCommandText.Append('\'');
                }
            }
        }

        catch (Exception ex)
        {
            sbCommandText.AppendLine("/* Exception occurred while converting parameter: ");
            sbCommandText.AppendLine(ex.ToString());
            sbCommandText.AppendLine("*/");
        }
    }

    private static void LogParameterType(SqlParameter param, StringBuilder sbCommandText)
    {
        switch (param.SqlDbType)
        {
            // variable length
            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.Binary:
                {
                    sbCommandText.Append(param.SqlDbType.ToString().ToUpperInvariant());
                    sbCommandText.Append('(');
                    sbCommandText.Append(param.Size == 0 ? 1 : param.Size);
                    sbCommandText.Append(')');
                }
                break;
            case SqlDbType.VarChar:
            case SqlDbType.NVarChar:
            case SqlDbType.VarBinary:
                {
                    sbCommandText.Append(param.SqlDbType.ToString().ToUpperInvariant());
                    sbCommandText.Append("(");
                    sbCommandText.Append(param.Size == 0 ? 1 : param.Size);
                    sbCommandText.Append(")");
                }
                break;
            // fixed length
            case SqlDbType.Text:
            case SqlDbType.NText:
            case SqlDbType.Bit:
            case SqlDbType.TinyInt:
            case SqlDbType.SmallInt:
            case SqlDbType.Int:
            case SqlDbType.BigInt:
            case SqlDbType.SmallMoney:
            case SqlDbType.Money:
            case SqlDbType.Decimal:
            case SqlDbType.Real:
            case SqlDbType.Float:
            case SqlDbType.Date:
            case SqlDbType.DateTime:
            case SqlDbType.DateTime2:
            case SqlDbType.DateTimeOffset:
            case SqlDbType.UniqueIdentifier:
            case SqlDbType.Image:
                {
                    sbCommandText.Append(param.SqlDbType.ToString().ToUpperInvariant());
                }
                break;
            // Unknown
            case SqlDbType.Timestamp:
            default:
                {
                    sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                    sbCommandText.Append(param.SqlDbType.ToString().ToUpperInvariant());
                    sbCommandText.Append(" *" + "/ ");
                    sbCommandText.Append(param.SqlDbType.ToString().ToUpperInvariant());
                }
                break;
        }
    }
}