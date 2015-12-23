using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Data;
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
            if (localAttr == null || localAttr.LocalizationRow == null)
                throw new InvalidOperationException(String.Format("{0} row type has no localization row type defined!", typeof(TRow).Name));

            var localInstance = (Row)Activator.CreateInstance(localAttr.LocalizationRow);

            var localRow = localInstance as ILocalizationRow;
            if (localRow == null)
                throw new InvalidOperationException(String.Format("Localization table {0} doesn't implement ILocalizationRow interface!",
                    localAttr.LocalizationRow.GetType().FullName, schemaName, typeof(TRow).Name));

            schemaName = RowRegistry.GetConnectionKey(localInstance);

            newInfo = new StaticInfo();
            newInfo.localRowInterface = localRow;
            newInfo.localRowInstance = localInstance;
            newInfo.rowInstance = new TRow();
            newInfo.rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(newInfo.rowInstance.EnumerateTableFields(), x => x.Name);
            newInfo.localRowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(localInstance.EnumerateTableFields(), x => x.Name);
            newInfo.mappedIdField = (IIdField)((Row)localInstance).FindField(localAttr.MappedIdField ?? ((Field)new TRow().IdField).Name);
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
                    }, SaveRequestType.Update),
                (id) => new DeleteRequestHandler<TLocalRow>().Process(uow, new DeleteRequest
                    {
                        EntityId = id
                    }));
        }
        
        public void Update(IUnitOfWork uow, TRow row, object cultureId,
            Action<Row> create, Action<Row> update, Action<Int64> delete)
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

            bool anyNonEmpty = false;

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

                if (value != null &&
                    (!(value is string) || !(value as string).IsTrimmedEmpty()))
                {
                    anyNonEmpty = true;
                }
            }

            if (oldId == null)
            {
                if (anyNonEmpty)
                    create(localRow);
            }
            else
            {
                if (anyNonEmpty)
                    update(localRow);
                else
                    delete(oldId.Value);
            }
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

        private List<Row> GetOldLocalizationRows(IDbConnection connection, Int64 recordId)
        {
            var info = EnsureInfo();

            var row = info.localRowInstance.CreateNew();
            return new SqlQuery()
                    .From(row)
                    .SelectTableFields()
                    .WhereEqual((Field)info.mappedIdField, recordId)
                    .List(connection, row);
        }

        public RetrieveLocalizationResponse<TRow> Retrieve(IDbConnection connection, RetrieveLocalizationRequest request)
        {
            request.CheckNotNull();

            if (request.EntityId == null)
                throw new ArgumentNullException("entityId");

            var recordId = request.EntityId.Value;

            var localRows = GetOldLocalizationRows(connection, request.EntityId.Value);

            var response = new RetrieveLocalizationResponse<TRow>();
            response.Entities = new Dictionary<string, TRow>();

            if (localRows.IsEmptyOrNull())
                return response;

            TRow row = new TRow();
            var idField = ((IIdRow)row).IdField;
            var fields = new TRow().GetFields();
            var matches = new Field[fields.Count];
            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (ReferenceEquals(field, idField))
                    continue;

                matches[i] = GetLocalizationMatch(field);
            }

            foreach (var localRow in localRows)
            {
                row = new TRow();
                row.TrackAssignments = true;
                row.IdField[row] = recordId;
                
                for (var i = 0; i < fields.Count; i++)
                {
                    var match = matches[i];
                    if (!ReferenceEquals(null, match))
                    {
                        var field = fields[i];
                        var value = match.AsObject(localRow);
                        field.AsObject(row, value);
                    }
                }

                var culture = ((ILocalizationRow)localRow).CultureIdField.AsObject(localRow);
                response.Entities[culture == null ? "" : culture.ToString()] = row;
            }

            return response;
        }
    }
}