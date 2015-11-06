using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Data
{
    public class CaptureLogConsts
    {
        public static readonly DateTime ValidUntil = new DateTime(9999, 1, 1);
    }

    public class CaptureLogHandler<TRow> : ICaptureLogHandler
        where TRow: Row, IIdRow, new() 
    {
        private static string logConnectionKey;
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
            SchemaChangeSource.Observers += (connectionKey, table) =>
            {
                if (connectionKey == logConnectionKey)
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

            logConnectionKey = logTableAttr.LogConnectionKey ?? RowRegistry.GetConnectionKey(typeof(TRow));
            var logRowInstance = RowRegistry.GetConnectionRow(logConnectionKey, logTableAttr.LogTable);
            if (logRowInstance == null)
                throw new InvalidOperationException(String.Format("Can't locate {0} capture log table in connection {1} for {2} row type!",
                    logTableAttr.LogTable, logConnectionKey, typeof(TRow).Name));

            var captureLogRow = logRowInstance as ICaptureLogRow;
            if (captureLogRow == null)
                throw new InvalidOperationException(String.Format("Capture log table {0} doesn't implement ICaptureLogRow interface!",
                    logTableAttr.LogTable, logConnectionKey, typeof(TRow).Name));

            if (!(captureLogRow is IIsActiveRow))
                throw new InvalidOperationException(String.Format("Capture log table {0} doesn't implement IIsActiveRow interface!",
                    logTableAttr.LogTable, logConnectionKey, typeof(TRow).Name));

            newInfo = new StaticInfo();
            newInfo.logRowInstance = logRowInstance;
            newInfo.captureLogInstance = captureLogRow;
            newInfo.rowInstance = new TRow();
            newInfo.rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(newInfo.rowInstance.EnumerateTableFields(), x => x.Name);
            newInfo.logFieldPrefixLength = PrefixHelper.DeterminePrefixLength(logRowInstance.EnumerateTableFields(), x => x.Name);
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

                if (ReferenceEquals(info.captureLogInstance.ChangingUserIdField, logField) ||
                    ReferenceEquals(info.captureLogInstance.ValidFromField, logField) ||
                    ReferenceEquals(info.captureLogInstance.ValidUntilField, logField))
                    continue;

                if (ReferenceEquals(((IIdRow)info.captureLogInstance).IdField, logField))
                    continue;

                if (ReferenceEquals(((IIsActiveRow)info.logRowInstance).IsActiveField, logField))
                    continue;

                if (ReferenceEquals(logField, info.mappedIdField))
                    yield return new Tuple<Field, Field>(logField, (Field)((IIdRow)info.rowInstance).IdField);
                else
                {
                    var name = logField.Name.Substring(info.logFieldPrefixLength);
                    name = ((Field)info.rowInstance.IdField).Name.Substring(0, info.rowFieldPrefixLength) + name;
                    var match = info.rowInstance.FindField(name);
                    if (ReferenceEquals(null, match))
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

            uow.Connection.Insert(logRow);
        }

        void ICaptureLogHandler.LogSave(IUnitOfWork uow, Row row, long userId)
        {
            Log(uow, (TRow)row, userId, isDelete: false);
        }

        void ICaptureLogHandler.LogDelete(IUnitOfWork uow, Row old, long userId)
        {
            Log(uow, (TRow)old, userId, isDelete: true);
        }
    }
}