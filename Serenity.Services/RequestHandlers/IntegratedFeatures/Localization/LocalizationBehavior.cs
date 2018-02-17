using Serenity.Data;
using Serenity.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class LocalizationBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IRetrieveBehavior
    {
        private LocalizationRowAttribute attr;
        private int rowPrefixLength;
        private Func<Row> rowFactory;
        private Func<Row> localRowFactory;
        private int localRowPrefixLength;
        private Field foreignKeyField;
        private Field localRowIdField;
        private Field cultureIdField;
        private Row localRowInstance;
        private Func<IListRequestProcessor> listHandlerFactory;
        private Func<ISaveRequestProcessor> saveHandlerFactory;
        private Func<IDeleteRequestProcessor> deleteHandlerFactory;
        private Func<ISaveRequest> saveRequestFactory;
        private BaseCriteria foreignKeyCriteria;
        private Func<IDictionary> dictionaryFactory;

        public bool ActivateFor(Row row)
        {
            attr = row.GetType().GetCustomAttribute<LocalizationRowAttribute>();
            if (attr == null)
                return false;

            var localRowType = attr.LocalizationRow;
            if (!typeof(ILocalizationRow).IsAssignableFrom(localRowType))
            {
                throw new ArgumentException(String.Format(
                    "Row type '{0}' has a LocalizationRowAttribute, " +
                    "but its localization row type ('{1}') doesn't implement ILocalizationRow interface!",
                        row.GetType().FullName, localRowType.FullName));
            }

            if (!typeof(IIdRow).IsAssignableFrom(localRowType))
            {
                throw new ArgumentException(String.Format(
                    "Row type '{0}' has a LocalizationRowAttribute, " +
                    "but its localization row type ('{1}') doesn't implement IIdRow interface!",
                        row.GetType().FullName, localRowType.FullName));
            }

            if (!(row is IIdRow))
            {
                throw new ArgumentException(String.Format(
                    "Row type '{0}' has a LocalizationRowAttribute, " +
                    "but row type itself doesn't implement IIdRow interface!",
                        row.GetType().FullName));
            }

            rowFactory = FastReflection.DelegateForConstructor<Row>(row.GetType());
            localRowFactory = FastReflection.DelegateForConstructor<Row>(localRowType);

            listHandlerFactory = FastReflection.DelegateForConstructor<IListRequestProcessor>(
                typeof(ListRequestHandler<>).MakeGenericType(localRowType));

            saveHandlerFactory = FastReflection.DelegateForConstructor<ISaveRequestProcessor>(
                typeof(SaveRequestHandler<>).MakeGenericType(localRowType));

            saveRequestFactory = FastReflection.DelegateForConstructor<ISaveRequest>(
                typeof(SaveRequest<>).MakeGenericType(localRowType));

            deleteHandlerFactory = FastReflection.DelegateForConstructor<IDeleteRequestProcessor>(
                typeof(DeleteRequestHandler<>).MakeGenericType(localRowType));

            var localRow = localRowFactory();
            localRowInstance = localRow;

            rowPrefixLength = PrefixHelper.DeterminePrefixLength(row.EnumerateTableFields(), x => x.Name);
            localRowPrefixLength = PrefixHelper.DeterminePrefixLength(localRow.EnumerateTableFields(), x => x.Name);
            localRowIdField = (Field)(((IIdRow)localRow).IdField);
            cultureIdField = ((ILocalizationRow)localRow).CultureIdField;

            var foreignKeyFieldName = attr.MappedIdField ?? ((Field)((IIdRow)row).IdField).PropertyName;
            foreignKeyField = localRow.FindFieldByPropertyName(foreignKeyFieldName) ??
                localRow.FindField(foreignKeyFieldName);

            if (ReferenceEquals(null, foreignKeyField))
            {
                throw new ArgumentException(String.Format(
                    "Row type '{0}' has a LocalizationRowAttribute, " +
                    "but its localization row type ('{1}') doesn't have a field with name '{2}'!",
                        row.GetType().FullName, localRowType.FullName, foreignKeyFieldName));
            }

            dictionaryFactory = FastReflection.DelegateForConstructor<IDictionary>(
                typeof(Dictionary<,>).MakeGenericType(typeof(string), row.GetType()));

            this.foreignKeyCriteria = new Criteria(foreignKeyField.PropertyName ?? foreignKeyField.Name);
            return true;
        }

        private bool IsLocalized(Field field)
        {
            return !ReferenceEquals(null, GetLocalizationMatch(field));
        }

        private Field GetLocalizationMatch(Field field)
        {
            if (!field.IsTableField())
                return null;

            var name = field.Name.Substring(rowPrefixLength);
            name = localRowIdField.Name.Substring(0, localRowPrefixLength) + name;
            var match = localRowInstance.FindField(name);
            if (ReferenceEquals(null, match) && field.PropertyName != null)
                match = localRowInstance.FindFieldByPropertyName(field.PropertyName);

            if (ReferenceEquals(null, match))
                return null;

            if (!match.IsTableField())
                return null;

            if (ReferenceEquals(match, localRowIdField) ||
                ReferenceEquals(match, (localRowInstance as ILocalizationRow).CultureIdField))
                return null;

            if (localRowInstance is IIsActiveRow &&
                ReferenceEquals(match, ((IIsActiveRow)localRowInstance).IsActiveField))
                return null;

            if (localRowInstance is IIsDeletedRow &&
                ReferenceEquals(match, ((IIsDeletedRow)localRowInstance).IsDeletedField))
                return null;

            var insertLog = localRowInstance as IInsertLogRow;
            if (insertLog != null && (
                ReferenceEquals(match, insertLog.InsertUserIdField) ||
                ReferenceEquals(match, insertLog.InsertDateField)))
                return null;

            var updateLog = localRowInstance as IUpdateLogRow;
            if (updateLog != null && (
                ReferenceEquals(match, updateLog.UpdateUserIdField) ||
                ReferenceEquals(match, updateLog.UpdateDateField)))
                return null;

            return match;
        }

        private object GetOldLocalizationRowId(IDbConnection connection, object recordId, object cultureId)
        {
            var row = localRowInstance.CreateNew();
            if (new SqlQuery()
                    .From(row)
                    .Select(localRowIdField)
                    .WhereEqual(foreignKeyField, recordId)
                    .WhereEqual(cultureIdField, cultureId)
                    .GetFirst(connection))
                return localRowIdField.AsObject(row);

            return null;
        }

        public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
        public void OnValidateRequest(IRetrieveRequestHandler handler) { }
        public void OnPrepareQuery(IListRequestHandler handler, SqlQuery query) { }
        public void OnValidateRequest(IListRequestHandler handler) { }
        public void OnApplyFilters(IListRequestHandler handler, SqlQuery query) { }
        public void OnBeforeExecuteQuery(IListRequestHandler handler) { }
        public void OnAfterExecuteQuery(IListRequestHandler handler) { }

        public void OnReturn(IRetrieveRequestHandler handler)
        {
            if (handler.Request == null ||
                handler.Request.IncludeColumns == null ||
                !handler.Request.IncludeColumns.Contains("Localizations"))
                return;

            var localIdField = (Field)((handler.Row as IIdRow).IdField);

            var listHandler = listHandlerFactory();

            var listRequest = new ListRequest
            {
                ColumnSelection = ColumnSelection.List,
                Criteria = foreignKeyCriteria == new ValueCriteria(localIdField.AsObject(handler.Row))
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var row = rowFactory();
            var rowIdField = (Field)(((IIdRow)row).IdField);
            var fields = row.GetFields();
            var matches = new Field[fields.Count];
            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (ReferenceEquals(field, rowIdField))
                    continue;

                matches[i] = GetLocalizationMatch(field);
            }

            var dictionary = dictionaryFactory();
            foreach (Row localRow in response.Entities)
            {
                row = rowFactory();
                row.TrackAssignments = true;
                rowIdField.AsObject(row, rowIdField.AsObject(row));

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

                var culture = cultureIdField.AsObject(localRow);
                dictionary[culture == null ? "" : culture.ToString()] = row;
            }

            handler.Response.Localizations = dictionary;
        }

        private void SaveLocalRow(IUnitOfWork uow, Row localRow, object masterId, object localRowId)
        {
            localRow = localRow.Clone();

            foreignKeyField.AsObject(localRow, masterId);
            ((Field)((IIdRow)localRow).IdField).AsObject(localRow, localRowId);

            var saveHandler = saveHandlerFactory();
            var saveRequest = saveRequestFactory();
            saveRequest.Entity = localRow;
            saveHandler.Process(uow, saveRequest, localRowId == null ? SaveRequestType.Create : SaveRequestType.Update);
        }

        private void DeleteLocalRow(IUnitOfWork uow, object detailId)
        {
            var deleteHandler = deleteHandlerFactory();
            var deleteRequest = new DeleteRequest { EntityId = detailId };
            deleteHandler.Process(uow, deleteRequest);
        }

        private string AsString(object obj)
        {
            if (obj == null)
                return null;

            return obj.ToString();
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            var localizations = handler.Request.Localizations;
            if (localizations == null)
                return; 

            var idField = (Field)((handler.Row as IIdRow).IdField);
            var masterId = idField.AsObject(handler.Row);
            
            foreach (DictionaryEntry pair in localizations)
            {
                var cultureId = cultureIdField.ConvertValue(pair.Key, CultureInfo.InvariantCulture);
                var oldId = handler.IsCreate ? null : GetOldLocalizationRowId(handler.UnitOfWork.Connection, masterId, cultureId);
                var localRow = localRowFactory();
                localRow.TrackAssignments = true;
                if (oldId == null)
                    cultureIdField.AsObject(localRow, cultureId);

                var row = pair.Value as Row;

                bool anyNonEmpty = false;

                foreach (var field in row.GetFields())
                {
                    if (ReferenceEquals(field, idField))
                        continue;

                    if (!row.IsAssigned(field))
                        continue;

                    var match = GetLocalizationMatch(field);
                    if (ReferenceEquals(null, match))
                        throw new ValidationError("CantLocalize", field.Name, String.Format("{0} field is not localizable!", field.Title));

                    var value = field.AsObject(row);
                    match.AsObject(localRow, value);

                    if (value != null &&
                        (!(value is string) || !(value as string).IsTrimmedEmpty()))
                    {
                        anyNonEmpty = true;
                    }
                }

                if (anyNonEmpty)
                    SaveLocalRow(handler.UnitOfWork, localRow, masterId, oldId);
                else if (oldId != null)
                    DeleteLocalRow(handler.UnitOfWork, oldId);
            }
        }

        public override void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            if (ServiceQueryHelper.UseSoftDelete(handler.Row))
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);
            var localRow = localRowFactory();

            var deleteHandler = deleteHandlerFactory();
            var deleteList = new List<object>();
            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(localRow)
                    .Select(localRowIdField)
                    .Where(
                        foreignKeyField == new ValueCriteria(idField.AsObject(handler.Row)))
                    .ForEach(handler.Connection, () =>
                    {
                        deleteList.Add(localRowIdField.AsObject(localRow));
                    });

            foreach (var localId in deleteList)
                DeleteLocalRow(handler.UnitOfWork, localId);
        }
    }
}