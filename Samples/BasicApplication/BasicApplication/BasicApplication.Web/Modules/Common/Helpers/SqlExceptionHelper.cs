using BasicApplication;
using System;
using Serenity;
using System.Data.SqlClient;
using Serenity.Services;

namespace BasicApplication
{
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
        public static void HandleDeleteForeignKeyException(Exception e)
        {
            ForeignKeyExceptionInfo fk;
            if (SqlExceptionHelper.IsForeignKeyException(e, out fk))
                throw new ValidationError(String.Format(Texts.Validation.DeleteForeignKeyError, fk.TableName));
        }

        public static void HandleSavePrimaryKeyException(Exception e, string fieldName = "ID")
        {
            PrimaryKeyExceptionInfo fk;
            if (SqlExceptionHelper.IsPrimaryKeyException(e, out fk))
                throw new ValidationError(String.Format(Texts.Validation.SavePrimaryKeyError, fk.TableName, fieldName));
        }

        public static bool IsForeignKeyException(Exception e, out ForeignKeyExceptionInfo fk)
        {
            // sample message: The DELETE statement conflicted with the REFERENCE constraint "FK_SomeTable_SomeFieldID". The conflict occurred in database "DBSome", table "dbo.SomeTable", column 'SomeFieldID'.

            var sql = e as SqlException;
            if (sql != null && sql.Errors.Count > 0 && sql.Errors[0].Number == 547)
            {
                var msg = sql.Errors[0].Message;
                fk = new ForeignKeyExceptionInfo();
                fk.TableName = "???";

                var s1 = ", table \"";
                var idx = msg.IndexOf(s1);
                if (idx >= 0)
                {
                    idx += s1.Length;
                    var idx2 = msg.IndexOf("\", column", idx + 1);
                    if (idx2 >= 0)
                    {
                        fk.TableName = msg.Substring(idx, idx2 - idx);
                        if (fk.TableName.StartsWith("dbo."))
                            fk.TableName = fk.TableName.Substring("dbo.".Length);
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

            var sql = e as SqlException;
            if (sql != null && sql.Errors.Count > 0 && sql.Errors[0].Number == 2627)
            {
                var msg = sql.Errors[0].Message;
                pk = new PrimaryKeyExceptionInfo();
                pk.TableName = "???";

                var s1 = "in object '";
                var idx = msg.IndexOf(s1);
                if (idx >= 0)
                {
                    idx += s1.Length;
                    var idx2 = msg.IndexOf("'.", idx + 1);
                    if (idx2 >= 0)
                    {
                        pk.TableName = msg.Substring(idx, idx2 - idx);
                        if (pk.TableName.StartsWith("dbo."))
                            pk.TableName = pk.TableName.Substring("dbo.".Length);
                    }
                }

                return true;
            }

            pk = null;
            return false;
        }
    }
}