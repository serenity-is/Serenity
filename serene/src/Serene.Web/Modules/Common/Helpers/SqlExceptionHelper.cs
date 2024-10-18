using Microsoft.Data.SqlClient;
using System.Globalization;

namespace Serene;

public class ForeignKeyExceptionInfo
{
    public string TableName { get; set; }
}

public class PrimaryKeyExceptionInfo
{
    public string TableName { get; set; }
}

public static class SqlExceptionHelper
{
    public static void HandleDeleteForeignKeyException(Exception e, ITextLocalizer localizer)
    {
        if (IsForeignKeyException(e, out ForeignKeyExceptionInfo fk))
            throw new ValidationError(string.Format(CultureInfo.CurrentCulture, Texts.Validation.DeleteForeignKeyError.ToString(localizer), fk.TableName));
    }

    public static void HandleSavePrimaryKeyException(Exception e, ITextLocalizer localizer, string fieldName = "ID")
    {
        if (IsPrimaryKeyException(e, out PrimaryKeyExceptionInfo fk))
            throw new ValidationError(string.Format(CultureInfo.CurrentCulture, Texts.Validation.SavePrimaryKeyError.ToString(localizer), fk.TableName, fieldName));
    }

    public static bool IsForeignKeyException(Exception e, out ForeignKeyExceptionInfo fk)
    {
        // sample message: The DELETE statement conflicted with the REFERENCE constraint "FK_SomeTable_SomeFieldID". The conflict occurred in database "DBSome", table "dbo.SomeTable", column 'SomeFieldID'.

        if (e as SqlException != null && (e as SqlException).Errors.Count > 0 && (e as SqlException).Errors[0].Number == 547)
        {
            var msg = (e as SqlException).Errors[0].Message;
            fk = new ForeignKeyExceptionInfo
            {
                TableName = "???"
            };

            var s1 = ", table \"";
            var idx = msg.IndexOf(s1, StringComparison.Ordinal);
            if (idx >= 0)
            {
                idx += s1.Length;
                var idx2 = msg.IndexOf("\", column", idx + 1, StringComparison.Ordinal);
                if (idx2 >= 0)
                {
                    fk.TableName = msg[idx..idx2];
                    if (fk.TableName.StartsWith("dbo.", StringComparison.Ordinal))
                        fk.TableName = fk.TableName["dbo.".Length..];
                }
            }

            return true;
        }

        fk = null;
        return false;
    }

    public static bool IsPrimaryKeyException(Exception e, out PrimaryKeyExceptionInfo pk)
    {
        // sample: Violation of PRIMARY KEY constraint 'PK_SomeTable'. Cannot insert duplicate key in object 'dbo.SomeTable'. The duplicate key value is (7005950).

        if (e is SqlException sql && sql.Errors.Count > 0 && sql.Errors[0].Number == 2627)
        {
            var msg = sql.Errors[0].Message;
            pk = new PrimaryKeyExceptionInfo
            {
                TableName = "???"
            };

            var s1 = "in object '";
            var idx = msg.IndexOf(s1, StringComparison.Ordinal);
            if (idx >= 0)
            {
                idx += s1.Length;
                var idx2 = msg.IndexOf("'.", idx + 1, StringComparison.Ordinal);
                if (idx2 >= 0)
                {
                    pk.TableName = msg[idx..idx2];
                    if (pk.TableName.StartsWith("dbo.", StringComparison.Ordinal))
                        pk.TableName = pk.TableName["dbo.".Length..];
                }
            }

            return true;
        }

        pk = null;
        return false;
    }
}