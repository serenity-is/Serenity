using Serenity;
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public class MasterDetailRelationBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IRetrieveBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private MasterDetailRelationAttribute attr;
        private Func<IList> rowListFactory;
        private Func<Row> rowFactory;
        private Func<IListRequestProcessor> listHandlerFactory;
        private Func<ISaveRequestProcessor> saveHandlerFactory;
        private Func<IDeleteRequestProcessor> deleteHandlerFactory;
        private Func<ISaveRequest> saveRequestFactory;
        private Field foreignKeyField;

        public bool ActivateFor(Row row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<MasterDetailRelationAttribute>();
            if (attr == null)
                return false;

            var rowListType = Target.ValueType;
            if (!rowListType.IsGenericType ||
                rowListType.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new ArgumentException(String.Format("Field '{0}' in row type '{1}' has a MasterDetailRelationAttribute " +
                    "but its property type is not a generic List (e.g. List<Row>)!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            var rowType = rowListType.GetGenericArguments()[0];
            if (rowType.IsAbstract ||
                !typeof(Row).IsAssignableFrom(rowType))
            {
                throw new ArgumentException(String.Format(
                    "Field '{0}' in row type '{1}' has a MasterDetailRelationAttribute " +
                    "but its property type is not a generic list of rows (e.g. List<Row>)!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            rowListFactory = FastReflection.DelegateForConstructor<IList>(rowListType);
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
            foreignKeyField = detailRow.FindFieldByPropertyName(attr.ForeignKey) ??
                detailRow.FindField(attr.ForeignKey);

            if (ReferenceEquals(foreignKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a master detail relation in field '{2}' of row type '{3}'.",
                    attr.ForeignKey, detailRow.GetType().FullName,
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
                ColumnSelection = ColumnSelection.List,
                EqualityFilter = new Dictionary<string, object>
                {
                    { foreignKeyField.PropertyName ?? foreignKeyField.Name, idField.AsObject(handler.Row) }
                }
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = rowListFactory();
            foreach (var item in response.Entities)
                list.Add(item);

            Target.AsObject(handler.Row, list);
        }

        private void SaveDetail(IUnitOfWork uow, Row detail, object masterId, object detailId)
        {
            detail = detail.Clone();

            foreignKeyField.AsObject(detail, masterId);
            ((Field)((IIdRow)detail).IdField).AsObject(detail, detailId);

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

        private string AsString(object obj)
        {
            if (obj == null)
                return null;

            return obj.ToString();
        }

        private void DetailListSave(IUnitOfWork uow, Int64 masterId, IList oldList, IList newList)
        {
            var row = oldList.Count > 0 ? oldList[0] : 
                (newList.Count > 0) ? newList[0] : null;

            if (row == null)
                return;

            if (oldList.Count == 0)
            {
                foreach (Row entity in newList)
                    SaveDetail(uow, entity, masterId, null);

                return;
            }

            var rowIdField = (Field)((row as IIdRow).IdField);

            if (newList.Count == 0)
            {
                foreach (Row entity in oldList)
                    DeleteDetail(uow, rowIdField.AsObject(entity));

                return;
            }

            var oldById = new Dictionary<string, Row>(oldList.Count);
            foreach (Row item in oldList)
                oldById[rowIdField.AsObject(item).ToString()] = item;

            var newById = new Dictionary<string, Row>(newList.Count);
            foreach (Row item in newList)
            {
                var id = AsString(rowIdField.AsObject(item));

                if (!string.IsNullOrEmpty(id))
                    newById[id.ToString()] = item;
            }

            foreach (Row item in oldList)
            {
                var id = rowIdField.AsObject(item);
                if (!newById.ContainsKey(id))
                    DeleteDetail(uow, id);
            }

            foreach (Row item in newList)
            {
                var id = rowIdField.AsObject(item);
                var idStr = AsString(id);

                Row old;
                if (string.IsNullOrEmpty(id) || !oldById.TryGetValue(id, out old))
                    continue;

                if (attr.CheckChangesOnUpdate)
                {
                    bool anyChanges = false;
                    foreach (var field in item.GetFields())
                    {
                        if (item.IsAssigned(field) &&
                            (field.Flags & FieldFlags.Updatable) == FieldFlags.Updatable &
                            field.IndexCompare(old, item) != 0)
                        {
                            anyChanges = true;
                            break;
                        }
                    }

                    if (!anyChanges)
                        continue;
                }

                SaveDetail(uow, item, masterId, id);
            }

            foreach (Row item in newList)
            {
                var id = rowIdField[item];
                if (id == null || !oldById.ContainsKey(id.Value))
                    SaveDetail(uow, item, masterId, null);
            }
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            var newList = Target.AsObject(handler.Row) as IList;
            if (newList == null)
                return;

            var idField = (handler.Row as IIdRow).IdField;
            var masterId = idField[handler.Row].Value;

            if (handler.IsCreate)
            {
                foreach (Row entity in newList)
                    SaveDetail(handler.UnitOfWork, entity, masterId, null);

                return;
            }

            var oldList = new List<Row>();

            if (!attr.CheckChangesOnUpdate)
            {
                var row = rowFactory();
                var rowIdField = (row as IIdRow).IdField;

                // if we're not gonna compare old rows with new ones
                // no need to call list request handler

                new SqlQuery()
                        .Dialect(handler.Connection.GetDialect())
                        .From(row)
                        .Select((Field)rowIdField)
                        .Where(foreignKeyField == idField[handler.Row].Value)
                        .ForEach(handler.Connection, () =>
                        {
                            oldList.Add(row.Clone());
                        });
            }
            else
            {
                var listRequest = new ListRequest
                {
                    ColumnSelection = ColumnSelection.List,
                    EqualityFilter = new Dictionary<string, object>
                    {
                        { foreignKeyField.PropertyName ?? foreignKeyField.Name, idField[handler.Row] }
                    }
                };

                var listHandler = listHandlerFactory();
                var entities = listHandler.Process(handler.Connection, listRequest).Entities;
                foreach (Row entity in entities)
                    oldList.Add(entity);
            }

            DetailListSave(handler.UnitOfWork, masterId, oldList, newList);
        }

        public override void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                return;

            var idField = (handler.Row as IIdRow).IdField;
            var row = rowFactory();
            var rowIdField = (row as IIdRow).IdField;

            var deleteHandler = deleteHandlerFactory();
            var deleteList = new List<Int64>();
            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select((Field)rowIdField)
                    .Where(foreignKeyField == idField[handler.Row].Value)
                    .ForEach(handler.Connection, () =>
                    {
                        deleteList.Add(rowIdField[row].Value);
                    });

            foreach (var id in deleteList)
                DeleteDetail(handler.UnitOfWork, id);
        }
    }
}