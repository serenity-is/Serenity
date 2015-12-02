using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Services
{
    public class CaptureLogConsts
    {
        public static readonly DateTime UntilMax = new DateTime(9999, 1, 1);
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

            var captureLogAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>(false);
            if (captureLogAttr == null || captureLogAttr.LogRow == null)
                throw new InvalidOperationException(String.Format("{0} row type has no capture log attribute defined!", typeof(TRow).Name));

            logConnectionKey = RowRegistry.GetConnectionKey(captureLogAttr.LogRow);
            var logRowInstance = (Row)Activator.CreateInstance(captureLogAttr.LogRow);

            var captureLogRow = logRowInstance as ICaptureLogRow;
            if (captureLogRow == null)
                throw new InvalidOperationException(String.Format("Capture log table {0} doesn't implement ICaptureLogRow interface!",
                    captureLogAttr.LogRow.FullName, logConnectionKey, typeof(TRow).Name));

            newInfo = new StaticInfo();
            newInfo.logRowInstance = logRowInstance;
            newInfo.captureLogInstance = captureLogRow;
            newInfo.rowInstance = new TRow();
            newInfo.rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(newInfo.rowInstance.EnumerateTableFields(), x => x.Name);
            newInfo.logFieldPrefixLength = PrefixHelper.DeterminePrefixLength(logRowInstance.EnumerateTableFields(), x => x.Name);
            var mappedIdField = captureLogAttr.MappedIdField ?? ((Field)newInfo.rowInstance.IdField).Name;
            newInfo.mappedIdField = ((Row)captureLogRow).FindField(mappedIdField) as IIdField;
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
                    ReferenceEquals(info.captureLogInstance.ValidUntilField, logField) ||
                    ReferenceEquals(info.captureLogInstance.OperationTypeField, logField))
                    continue;

                if (ReferenceEquals(((IIdRow)info.captureLogInstance).IdField, logField))
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

        public void Log(IUnitOfWork uow, TRow old, TRow row, object userId)
        {
            if (old == null && row == null)
                throw new ArgumentNullException("old");

            var now = DateTime.Now;
            var info = EnsureInfo();
            var logRow = info.logRowInstance.CreateNew();
            logRow.TrackAssignments = true;
            var capture = logRow as ICaptureLogRow;

            capture.ChangingUserIdField.AsObject(logRow, userId == null ? null :
                capture.ChangingUserIdField.ConvertValue(userId, Invariants.NumberFormat));

            var operationType = old == null ? CaptureOperationType.Insert :
                (row == null ? CaptureOperationType.Delete : CaptureOperationType.Before);

            capture.OperationTypeField[logRow] = (Int16)operationType;
            capture.ValidFromField[logRow] = now;

            if (operationType == CaptureOperationType.Insert)
            {
                capture.ValidUntilField[logRow] = CaptureLogConsts.UntilMax;
                CopyCapturedFields(row, logRow);
            }
            else
            {
                capture.ValidUntilField[logRow] = now;
                CopyCapturedFields(old, logRow);
            }

            if (new SqlUpdate(info.logRowInstance.Table)
                    .Set(capture.ValidUntilField, now)
                    .WhereEqual((Field)info.mappedIdField, info.mappedIdField[logRow])
                    .WhereEqual(capture.ValidUntilField, CaptureLogConsts.UntilMax)
                    .Execute(uow.Connection, ExpectedRows.Ignore) > 1)
                throw new InvalidOperationException(String.Format("Capture log has more than one active instance for ID {0}?!", info.mappedIdField[logRow]));

            uow.Connection.Insert(logRow);
            if (operationType == CaptureOperationType.Before)
            {
                logRow = info.logRowInstance.CreateNew();
                logRow.TrackAssignments = true;
                capture = logRow as ICaptureLogRow;

                capture.ChangingUserIdField.AsObject(logRow, userId == null ? null :
                    capture.ChangingUserIdField.ConvertValue(userId, Invariants.NumberFormat));
                capture.OperationTypeField[logRow] = (int)CaptureOperationType.Update;
                capture.ValidFromField[logRow] = now;
                capture.ValidUntilField[logRow] = CaptureLogConsts.UntilMax;
                CopyCapturedFields(row, logRow);
                uow.Connection.Insert(logRow);
            }
        }

        void ICaptureLogHandler.Log(IUnitOfWork uow, Row old, Row row, object userId)
        {
            Log(uow, (TRow)old, (TRow)row, userId);
        }
    }
}