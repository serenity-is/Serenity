using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Serenity.Data
{
    public interface ILocalizationRowHandler
    {
        bool IsLocalized(Field field);
    }

    public class LocalizationRowHandler<TRow> : ILocalizationRowHandler
        where TRow: Row, IIdRow, new()
    {
        private static string schemaName;
        private static StaticInfo info;

        private class StaticInfo
        {
            public TRow rowInstance;
            public Row localRowInstance;
            public ILocalizationRow localRowInterface;
            public int rowFieldPrefixLength;
            public int localRowFieldPrefixLength;
            public IIdField mappedIdField;
        }

        static LocalizationRowHandler()
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

            var localAttr = typeof(TRow).GetCustomAttribute<LocalizationRowAttribute>(false);
            if (localAttr == null || localAttr.LocalizationTable.IsTrimmedEmpty())
                throw new InvalidOperationException(String.Format("{0} row type has no localization attribute defined!", typeof(TRow).Name));

            schemaName = RowRegistry.GetConnectionKey(typeof(TRow));
            var localInstance = RowRegistry.GetConnectionRow(schemaName, localAttr.LocalizationTable);
            if (localInstance == null)
                throw new InvalidOperationException(String.Format("Can't locate {0} localization table in schema {1} for {2} row type!",
                    localAttr.LocalizationTable, schemaName, typeof(TRow).Name));

            var localRow = localInstance as ILocalizationRow;
            if (localRow == null)
                throw new InvalidOperationException(String.Format("Localization table {0} doesn't implement ILocalizationRow interface!",
                    localAttr.LocalizationTable, schemaName, typeof(TRow).Name));


            newInfo = new StaticInfo();
            newInfo.localRowInterface = localRow;
            newInfo.localRowInstance = localInstance;
            newInfo.rowInstance = new TRow();
            newInfo.rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(newInfo.rowInstance.EnumerateTableFields(), x => x.Name);
            newInfo.localRowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(localInstance.EnumerateTableFields(), x => x.Name);
            newInfo.mappedIdField = (IIdField)((Row)localInstance).FindField(localAttr.MappedIdField);
            if (newInfo.mappedIdField == null)
                throw new InvalidOperationException(String.Format("Can't locate localization table mapped ID field for {0}!",
                    localInstance.Table));

            info = newInfo;
            return newInfo;
        }

        public static Field GetLocalizationMatch(Field field)
        {
            var info = EnsureInfo();

            if (!field.IsTableField())
                return null;

            var name = field.Name.Substring(info.rowFieldPrefixLength);
            name = ((Field)info.localRowInterface.IdField).Name.Substring(0, info.localRowFieldPrefixLength) + name;
            var match = info.localRowInstance.FindField(name);
            
            if (ReferenceEquals(null, match))
                return null;

            if (!match.IsTableField())
                return null;

            if (ReferenceEquals(match, info.localRowInterface.IdField) ||
                ReferenceEquals(match, info.localRowInterface.CultureIdField))
                return null;

            if (info.localRowInstance is IIsActiveRow && 
                ReferenceEquals(match, ((IIsActiveRow)info.localRowInstance).IsActiveField))
                return null;

            var logging = info.localRowInstance as ILoggingRow;
            if (logging != null && (
                ReferenceEquals(match, logging.InsertUserIdField) ||
                ReferenceEquals(match, logging.UpdateUserIdField) ||
                ReferenceEquals(match, logging.UpdateDateField) ||
                ReferenceEquals(match, logging.InsertDateField)))
                return null;

            return match;
        }

        public bool IsLocalized(Field field)
        {
            return !ReferenceEquals(null, GetLocalizationMatch(field));
        }

        private Int64? GetOldLocalizationRowId(IDbConnection connection, Int64 recordId, object cultureId)
        {
            var info = EnsureInfo();

            var row = info.localRowInstance.CreateNew();
            if (new SqlQuery()
                    .From(row)
                    .Select((Field)info.localRowInterface.IdField)
                    .WhereEqual((Field)info.mappedIdField, recordId)
                    .WhereEqual(info.localRowInterface.CultureIdField, cultureId)
                    .GetFirst(connection))
                return info.localRowInterface.IdField[row];

            return null;
        }

        public void Update<TLocalRow>(IUnitOfWork uow, TRow row, object cultureId)
            where TLocalRow: Row, IIdRow, new()
        {
            Update(uow, row, cultureId,
                (r) => new SaveRequestHandler<TLocalRow>().Process(uow,
                    new SaveRequest<TLocalRow>
                    {
                        Entity = (TLocalRow)r
                    }, SaveRequestType.Create),
                (r) => new SaveRequestHandler<TLocalRow>().Process(uow,
                    new SaveRequest<TLocalRow>
                    {
                        Entity = (TLocalRow)r
                    }, SaveRequestType.Update));
        }
        
        public void Update(IUnitOfWork uow, TRow row, object cultureId,
            Action<Row> create, Action<Row> update)
        {
            var info = EnsureInfo();

            var idField = ((IIdRow)row).IdField;
            var recordId = idField[row].Value;

            var oldId = GetOldLocalizationRowId(uow.Connection, recordId, cultureId);
            var localRow = info.localRowInstance.CreateNew();
            localRow.TrackAssignments = true;

            if (oldId != null)
                ((IIdRow)localRow).IdField[localRow] = oldId;

            if (oldId == null)
            {
                info.mappedIdField[localRow] = recordId;
                info.localRowInterface.CultureIdField.AsObject(localRow, cultureId);
            }

            foreach (var field in row.GetFields())
            {
                if (ReferenceEquals(field, idField))
                    continue;

                if (!row.IsAssigned(field))
                    continue;

                var match = GetLocalizationMatch(field);
                if (ReferenceEquals(null, match))
                    throw new ValidationError("CantLocalize", field.Name, String.Format("{0} alanı yerelleştirilemez!", field.Title));

                var value = field.AsObject(row);
                match.AsObject(localRow, value);
            }

            if (oldId == null)
                create(localRow);
            else
                update(localRow);
        }

        private Row GetOldLocalizationRow(IDbConnection connection, Int64 recordId, object cultureId)
        {
            var info = EnsureInfo();

            var row = info.localRowInstance.CreateNew();
            if (new SqlQuery()
                    .From(row)
                    .SelectTableFields()
                    .WhereEqual((Field)info.mappedIdField, recordId)
                    .WhereEqual(info.localRowInterface.CultureIdField, cultureId)
                    .GetFirst(connection))
                return row;

            return null;
        }

        public RetrieveResponse<TRow> Retrieve(IDbConnection connection, RetrieveLocalizationRequest request)
        {
            request.CheckNotNull();

            if (request.EntityId == null)
                throw new ArgumentNullException("entityId");

            if (request.CultureId == null)
                throw new ArgumentNullException("cultureId");

            var row = new TRow();
            row.TrackAssignments = true;
            row.IdField[row] = request.EntityId.Value;

            var recordId = request.EntityId.Value;
            var idField = ((IIdRow)row).IdField;

            var localRow = GetOldLocalizationRow(connection, recordId, request.CultureId);

            var response = new RetrieveResponse<TRow>();
            response.Entity = row;

            if (localRow == null)
                return response;

            foreach (var field in row.GetFields())
            {
                if (ReferenceEquals(field, idField))
                    continue;

                var match = GetLocalizationMatch(field);
                if (!ReferenceEquals(null, match))
                {
                    var value = match.AsObject(localRow);
                    field.AsObject(row, value);
                }
            }

            return response;
        }
    }
}