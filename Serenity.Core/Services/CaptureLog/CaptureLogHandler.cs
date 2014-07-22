using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Data
{
    public class CaptureLogConsts
    {
        public static readonly DateTime ValidUntil = new DateTime(9999, 1, 1);
    }

    public class CaptureLogHandler<TRow>
        where TRow: Row, IIdRow, new()
    {
        private static string schemaName;
        private static StaticInfo info;

        private class StaticInfo
        {
            public TRow rowInstance;
            public Row logRowInstance;
            public ICaptureLogRow captureLogInstance;
            public int rowFieldPrefixLength;
            public int logFieldPrefixLength;
            public IIdField mappedIdField;
        }

        static CaptureLogHandler()
        {
            SchemaChangeSource.Observers += (schema, table) =>
            {
                if (schema == schemaName)
                    info = null;
            };
        }

        static StaticInfo EnsureInfo()
        {
            var newInfo = info;
            if (newInfo != null)
                return newInfo;

            var logTableAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>(false);
            if (logTableAttr == null || logTableAttr.LogTable.IsTrimmedEmpty())
                throw new InvalidOperationException(String.Format("{0} row type has no capture log table attribute defined!", typeof(TRow).Name));

            schemaName = RowRegistry.GetConnectionKey(typeof(TRow));
            var instance = RowRegistry.GetConnectionRow(schemaName, logTableAttr.LogTable);
            if (instance == null)
                throw new InvalidOperationException(String.Format("Can't locate {0} capture log table in schema {1} for {2} row type!",
                    logTableAttr.LogTable, schemaName, typeof(TRow).Name));

            var captureLogRow = instance as ICaptureLogRow;
            if (captureLogRow == null)
                throw new InvalidOperationException(String.Format("Capture log table {0} doesn't implement ICaptureLogRow interface!",
                    logTableAttr.LogTable, schemaName, typeof(TRow).Name));

            if (!(captureLogRow is IIsActiveRow))
                throw new InvalidOperationException(String.Format("Capture log table {0} doesn't implement IIsActiveRow interface!",
                    logTableAttr.LogTable, schemaName, typeof(TRow).Name));

            newInfo = new StaticInfo();
            newInfo.logRowInstance = instance;
            newInfo.captureLogInstance = captureLogRow;
            newInfo.rowInstance = new TRow();
            newInfo.rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(newInfo.rowInstance.EnumerateTableFields(), x => x.Name);
            newInfo.logFieldPrefixLength = PrefixHelper.DeterminePrefixLength(instance.EnumerateTableFields(), x => x.Name);
            newInfo.mappedIdField = ((Row)captureLogRow).FindField(logTableAttr.MappedIdField) as IIdField;
            if (newInfo.mappedIdField == null)
                throw new InvalidOperationException(String.Format("Can't locate capture log table mapped ID field for {0}!",
                    ((Row)captureLogRow).Table));

            info = newInfo;
            return newInfo;
        }

        public static IEnumerable<Tuple<Field, Field>> EnumerateCapturedFields()
        {
            var info = EnsureInfo();
            foreach (var logField in info.logRowInstance.GetFields())
            {
                if (!logField.IsTableField())
                    continue;

                if (info.captureLogInstance.ChangingUserIdField == logField ||
                    info.captureLogInstance.ValidFromField == logField ||
                    info.captureLogInstance.ValidUntilField == logField)
                    continue;

                if (((IIdRow)info.captureLogInstance).IdField == logField)
                    continue;

                if (((IIsActiveRow)info.logRowInstance).IsActiveField == logField)
                    continue;

                if (logField == info.mappedIdField)
                    yield return new Tuple<Field, Field>(logField, (Field)((IIdRow)info.rowInstance).IdField);
                else
                {
                    var name = logField.Name.Substring(info.logFieldPrefixLength);
                    name = ((Field)info.rowInstance.IdField).Name.Substring(0, info.rowFieldPrefixLength) + name;
                    var match = info.rowInstance.FindField(name);
                    if (match == null)
                        throw new InvalidOperationException(String.Format("Can't find match in the row for log table field {0}!", name));

                    yield return new Tuple<Field, Field>(logField, match);
                }
            }
        }

        private static void CopyCapturedFields(Row source, Row target)
        {
            var info = EnsureInfo();
            foreach (var tuple in EnumerateCapturedFields())
            {
                var value = tuple.Item2.AsObject(source);
                tuple.Item1.AsObject(target, value);
            }
        }

        public void Log(IUnitOfWork uow, TRow row, Int64 userId, bool isDelete)
        {
            var info = EnsureInfo();
            var logRow = info.logRowInstance.CreateNew();
            logRow.TrackAssignments = true;
            var capture = logRow as ICaptureLogRow;
            capture.ChangingUserIdField[logRow] = userId;
            capture.IsActiveField[logRow] = isDelete ? -1 : (row is IIsActiveRow ? ((IIsActiveRow)row).IsActiveField[row] : 1);
            capture.ValidFromField[logRow] = DateTime.Now;
            capture.ValidUntilField[logRow] = CaptureLogConsts.ValidUntil;
            CopyCapturedFields(row, logRow);

            if (new SqlUpdate(logRow.Table)
                    .Set(capture.ValidUntilField, capture.ValidFromField[logRow])
                    .WhereEqual((Field)info.mappedIdField, info.mappedIdField[logRow])
                    .WhereEqual(capture.ValidUntilField, CaptureLogConsts.ValidUntil)
                    .Execute(uow.Connection, ExpectedRows.Ignore) > 1)
                throw new InvalidOperationException(String.Format("Capture log has more than one active instance for ID {0}?!", info.mappedIdField[logRow]));

            new SqlInsert(logRow).Execute(uow.Connection);
        }
    }
}