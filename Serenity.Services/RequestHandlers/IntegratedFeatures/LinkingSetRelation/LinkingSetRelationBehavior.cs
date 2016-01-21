using Serenity;
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Serenity.Services
{
    public class LinkingSetRelationBehavior : BaseSaveDeleteBehavior, 
        IImplicitBehavior, IRetrieveBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private LinkingSetRelationAttribute attr;
        private Func<IList> listFactory;
        private Func<Row> rowFactory;
        private Func<IListRequestProcessor> listHandlerFactory;
        private Func<ISaveRequestProcessor> saveHandlerFactory;
        private Func<IDeleteRequestProcessor> deleteHandlerFactory;
        private Func<ISaveRequest> saveRequestFactory;
        private Field thisKeyField;
        private Field itemKeyField;

        public bool ActivateFor(Row row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<LinkingSetRelationAttribute>();
            if (attr == null)
                return false;

            if (!(row is IIdRow))
            {
                throw new ArgumentException(String.Format("Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                    "but it doesn't implement IIdRow!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }


            var listType = Target.ValueType;
            if (!listType.IsGenericType ||
                listType.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new ArgumentException(String.Format("Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                    "but its property type is not a generic List (e.g. List<int>)!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            var rowType = attr.RowType;
            if (rowType.IsAbstract ||
                !typeof(Row).IsAssignableFrom(rowType))
            {
                throw new ArgumentException(String.Format(
                    "Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                    "but specified row type is not valid row class!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            if (!typeof(IIdRow).IsAssignableFrom(rowType))
            {
                throw new ArgumentException(String.Format(
                    "Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                    "but specified row type doesn't implement IIdRow!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            listFactory = FastReflection.DelegateForConstructor<IList>(listType);
            rowFactory = FastReflection.DelegateForConstructor<Row>(rowType);

            listHandlerFactory = FastReflection.DelegateForConstructor<IListRequestProcessor>(
                typeof(ListRequestHandler<>).MakeGenericType(rowType));

            saveHandlerFactory = FastReflection.DelegateForConstructor<ISaveRequestProcessor>(
                typeof(SaveRequestHandler<>).MakeGenericType(rowType));

            saveRequestFactory = FastReflection.DelegateForConstructor<ISaveRequest>(
                typeof(SaveRequest<>).MakeGenericType(rowType));

            deleteHandlerFactory = FastReflection.DelegateForConstructor<IDeleteRequestProcessor>(
                typeof(DeleteRequestHandler<>).MakeGenericType(rowType));

            var detailRow = rowFactory();

            thisKeyField = detailRow.FindFieldByPropertyName(attr.ThisKey) ??
                detailRow.FindField(attr.ThisKey);

            if (ReferenceEquals(thisKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                    attr.ThisKey, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            itemKeyField = detailRow.FindFieldByPropertyName(attr.ItemKey) ??
                detailRow.FindField(attr.ItemKey);

            if (ReferenceEquals(itemKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                    attr.ItemKey, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            return true;
        }

        public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
        public void OnValidateRequest(IRetrieveRequestHandler handler) { }

        public void OnReturn(IRetrieveRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                !handler.ShouldSelectField(Target))
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);

            var listHandler = listHandlerFactory();

            var listRequest = new ListRequest
            {
                ColumnSelection = ColumnSelection.KeyOnly,
                IncludeColumns = new HashSet<string>
                {
                    itemKeyField.PropertyName ?? itemKeyField.Name
                },
                EqualityFilter = new Dictionary<string, object>
                {
                    { thisKeyField.PropertyName ?? thisKeyField.Name, idField.AsObject(handler.Row) }
                }
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = listFactory();
            foreach (Row item in response.Entities)
                list.Add(itemKeyField.AsObject(item));

            Target.AsObject(handler.Row, list);
        }

        private void SaveDetail(IUnitOfWork uow, object masterId, object detailId, object itemKey)
        {
            var detail = rowFactory();
            ((Field)(((IIdRow)detail).IdField)).AsObject(detail, detailId);
            thisKeyField.AsObject(detail, masterId);
            itemKeyField.AsObject(detail, itemKeyField.ConvertValue(itemKey, CultureInfo.InvariantCulture));

            var saveHandler = saveHandlerFactory();
            var saveRequest = saveRequestFactory();
            saveRequest.Entity = detail;
            saveHandler.Process(uow, saveRequest, detailId == null ? SaveRequestType.Create : SaveRequestType.Update);
        }

        private void DeleteDetail(IUnitOfWork uow, object detailId)
        {
            var deleteHandler = deleteHandlerFactory();
            var deleteRequest = new DeleteRequest { EntityId = detailId };
            deleteHandler.Process(uow, deleteRequest);
        }

        private void DetailListSave(IUnitOfWork uow, object masterId, IList<Row> oldRows, 
            IList<object> newItemKeys)
        {
            if (oldRows.Count == 0)
            {
                foreach (object itemKey in newItemKeys)
                    SaveDetail(uow, masterId, null, itemKey);

                return;
            }

            var row = rowFactory();
            var rowIdField = (Field)((row as IIdRow).IdField);

            newItemKeys = newItemKeys.Where(x => x != null).Distinct().ToList();

            if (newItemKeys.Count == 0)
            {
                foreach (Row entity in oldRows)
                    DeleteDetail(uow, rowIdField.AsObject(entity));

                return;
            }

            var oldByItemKey = new Dictionary<string, Row>(oldRows.Count);
            foreach (Row item in oldRows)
            {
                var itemKey = itemKeyField.AsObject(item);
                if (itemKey != null)
                    oldByItemKey[itemKey.ToString()] = item;
            }

            if (attr.PreserveOrder)
            {
                if (!newItemKeys.Take(oldRows.Count).SequenceEqual(
                        oldRows.Select(x => itemKeyField.AsObject(x))) &&
                    newItemKeys.Any(x => !oldByItemKey.ContainsKey(x.ToString())))
                {
                    foreach (Row item in oldRows)
                    {
                        var id = rowIdField.AsObject(item);
                        DeleteDetail(uow, id);
                    }

                    oldRows = new List<Row>();
                    oldByItemKey = new Dictionary<string, Row>();
                }
            }

            var newByItemKey = new HashSet<string>();
            foreach (object item in newItemKeys)
            {
                if (item != null)
                    newByItemKey.Add(item.ToString());
            }

            foreach (Row item in oldRows)
            {
                var itemKey = itemKeyField.AsObject(item);
                var id = rowIdField.AsObject(item);
                if (itemKey == null || !newByItemKey.Contains(itemKey.ToString()))
                    DeleteDetail(uow, id);
            }

            foreach (object itemKey in newItemKeys)
            {
                if (oldByItemKey.ContainsKey(itemKey.ToString()))
                    continue;

                SaveDetail(uow, masterId, null, itemKey);
            }
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            var newList = Target.AsObject(handler.Row) as IList;
            if (newList == null)
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);
            var masterId = idField.AsObject(handler.Row);

            if (handler.IsCreate)
            {
                foreach (object itemKey in newList)
                    if (itemKey != null)
                        SaveDetail(handler.UnitOfWork, masterId, null, itemKey);

                return;
            }

            var oldRows = new List<Row>();

            var row = rowFactory();
            var rowIdField = (Field)((row as IIdRow).IdField);

            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select(rowIdField)
                    .Select(itemKeyField)
                    .OrderBy(rowIdField)
                    .WhereEqual(thisKeyField, masterId)
                    .ForEach(handler.Connection, () =>
                    {
                        oldRows.Add(row.Clone());
                    });

            DetailListSave(handler.UnitOfWork, masterId, oldRows,
                newList.Cast<object>().ToList());
        }

        public override void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);
            var masterId = idField.AsObject(handler.Row);
            var row = rowFactory();
            var rowIdField = (Field)((row as IIdRow).IdField);

            var deleteHandler = deleteHandlerFactory();
            var deleteList = new List<object>();
            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select(rowIdField)
                    .WhereEqual(thisKeyField, masterId)
                    .ForEach(handler.Connection, () =>
                    {
                        deleteList.Add(rowIdField.AsObject(row));
                    });

            foreach (var id in deleteList)
                DeleteDetail(handler.UnitOfWork, id);
        }
    }
}