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
    public class MasterDetailRelationBehavior : BaseSaveDeleteBehavior, 
        IImplicitBehavior, IRetrieveBehavior, IListBehavior, IFieldBehavior
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
        private BaseCriteria foreignKeyCriteria;
        private Field filterField;
        private object filterValue;
        public BaseCriteria filterCriteria;
        private HashSet<string> includeColumns;

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

            this.foreignKeyCriteria = new Criteria(foreignKeyField.PropertyName ?? foreignKeyField.Name);

            if (!string.IsNullOrEmpty(attr.FilterField))
            {
                this.filterField = detailRow.FindFieldByPropertyName(attr.FilterField) ?? detailRow.FindField(attr.FilterField);
                if (ReferenceEquals(null, this.filterField))
                    throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                        "This field is specified for a master detail relation as FilterField in field '{2}' of row type '{3}'.",
                        attr.FilterField, detailRow.GetType().FullName,
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

                this.filterCriteria = new Criteria(filterField.PropertyName ?? filterField.Name);
                this.filterValue = filterField.ConvertValue(attr.FilterValue, CultureInfo.InvariantCulture);               
                if (this.filterValue == null)
                    this.filterCriteria = this.filterCriteria.IsNull();
                else
                    this.filterCriteria = this.filterCriteria == new ValueCriteria(this.filterValue);
            }

            this.includeColumns = new HashSet<string>();

            if (!string.IsNullOrEmpty(attr.IncludeColumns))
                foreach (var s in attr.IncludeColumns.Split(','))
                {
                    var col = s.TrimToNull();
                    if (col != null)
                        this.includeColumns.Add(col);
                }

            return true;
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
            if (ReferenceEquals(null, Target) ||
                !handler.ShouldSelectField(Target))
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);

            var listHandler = listHandlerFactory();

            var listRequest = new ListRequest
            {
                ColumnSelection = this.attr.ColumnSelection,
                IncludeColumns = this.includeColumns,
                Criteria = foreignKeyCriteria == new ValueCriteria(idField.AsObject(handler.Row)) & filterCriteria
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = rowListFactory();
            foreach (var item in response.Entities)
                list.Add(item);

            Target.AsObject(handler.Row, list);
        }

        public void OnReturn(IListRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                !handler.ShouldSelectField(Target) ||
                handler.Response.Entities.IsEmptyOrNull())
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);

            var listHandler = listHandlerFactory();

            var listRequest = new ListRequest
            {
                ColumnSelection = this.attr.ColumnSelection,
                IncludeColumns = this.includeColumns
            };          

            var enumerator = handler.Response.Entities.Cast<Row>();
            while (true)
            {
                var part = enumerator.Take(1000);
                if (!part.Any())
                    break;

                enumerator = enumerator.Skip(1000);

                listRequest.Criteria = foreignKeyCriteria.In(
                    part.Select(x => idField.AsObject(x))) & filterCriteria;

                IListResponse response = listHandler.Process(
                    handler.Connection, listRequest);

                var lookup = response.Entities.Cast<Row>()
                    .ToLookup(x => foreignKeyField.AsObject(x).ToString());

                foreach (var row in part)
                {
                    var list = rowListFactory();
                    var matching = lookup[idField.AsObject(row).ToString()];
                    foreach (var x in matching)
                        list.Add(x);

                    Target.AsObject(row, list);
                }
            }
        }

        private void SaveDetail(IUnitOfWork uow, Row detail, object masterId, object detailId)
        {
            detail = detail.Clone();

            foreignKeyField.AsObject(detail, masterId);
            if (!ReferenceEquals(null, filterField))
                filterField.AsObject(detail, filterValue);

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

        private void DetailListSave(IUnitOfWork uow, object masterId, IList oldList, IList newList)
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
                oldById[AsString(rowIdField.AsObject(item))] = item;

            var newById = new Dictionary<string, Row>(newList.Count);
            foreach (Row item in newList)
            {
                var idStr = AsString(rowIdField.AsObject(item));

                if (!string.IsNullOrEmpty(idStr))
                    newById[idStr] = item;
            }

            foreach (Row item in oldList)
            {
                var id = rowIdField.AsObject(item);
                var idStr = AsString(id);
                if (!newById.ContainsKey(idStr))
                    DeleteDetail(uow, id);
            }

            foreach (Row item in newList)
            {
                var id = rowIdField.AsObject(item);
                var idStr = AsString(id);

                Row old;
                if (string.IsNullOrEmpty(idStr) || !oldById.TryGetValue(idStr, out old))
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
                var id = rowIdField.AsObject(item);
                var idStr = AsString(id);
                if (string.IsNullOrEmpty(idStr) || !oldById.ContainsKey(idStr))
                    SaveDetail(uow, item, masterId, null);
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
                        .Where(
                            foreignKeyField == new ValueCriteria(idField.AsObject(handler.Row)) &
                            filterField == new ValueCriteria(filterValue))
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
                    Criteria = foreignKeyCriteria == new ValueCriteria(idField.AsObject(handler.Row)) & filterCriteria
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

            var idField = (Field)((handler.Row as IIdRow).IdField);
            var row = rowFactory();
            var rowIdField = (Field)((row as IIdRow).IdField);

            var deleteHandler = deleteHandlerFactory();
            var deleteList = new List<object>();
            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select((Field)rowIdField)
                    .Where(
                            foreignKeyField == new ValueCriteria(idField.AsObject(handler.Row)) &
                            filterField == new ValueCriteria(filterValue))
                    .ForEach(handler.Connection, () =>
                    {
                        deleteList.Add(rowIdField.AsObject(row));
                    });

            foreach (var id in deleteList)
                DeleteDetail(handler.UnitOfWork, id);
        }
    }
}