using Serenity;
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if !NET45
using System.Reflection;
#endif

namespace Serenity.Services
{
    public class LinkingSetRelationBehavior : BaseSaveDeleteBehavior, 
        IImplicitBehavior, IRetrieveBehavior, IListBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private LinkingSetRelationAttribute attr;
        private Func<IList> listFactory;
        private Func<Row> rowFactory;
        private Type rowType;
        private Field thisKeyField;
        private Criteria thisKeyCriteria;
        private Field itemKeyField;
        private Field filterField;
        private object filterValue;
        private BaseCriteria filterCriteria;
        private BaseCriteria queryCriteria;

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

            rowType = attr.RowType;
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

            var detailRow = rowFactory();

            thisKeyField = detailRow.FindFieldByPropertyName(attr.ThisKey) ??
                detailRow.FindField(attr.ThisKey);

            if (ReferenceEquals(thisKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                    attr.ThisKey, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            this.thisKeyCriteria = new Criteria(thisKeyField.PropertyName ?? thisKeyField.Name);

            itemKeyField = detailRow.FindFieldByPropertyName(attr.ItemKey) ??
                detailRow.FindField(attr.ItemKey);

            if (ReferenceEquals(itemKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                    attr.ItemKey, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            if (!string.IsNullOrEmpty(attr.FilterField))
            {
                this.filterField = detailRow.FindFieldByPropertyName(attr.FilterField) ?? detailRow.FindField(attr.FilterField);
                if (ReferenceEquals(null, this.filterField))
                    throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                        "This field is specified for a linking set relation as FilterField in field '{2}' of row type '{3}'.",
                        attr.FilterField, detailRow.GetType().FullName,
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

                this.filterCriteria = new Criteria(filterField.PropertyName ?? filterField.Name);
                this.filterValue = filterField.ConvertValue(attr.FilterValue, CultureInfo.InvariantCulture);
                if (this.filterValue == null)
                {
                    this.filterCriteria = this.filterCriteria.IsNull();
                    this.queryCriteria = this.filterField.IsNull();
                }
                else
                {
                    this.filterCriteria = this.filterCriteria == new ValueCriteria(this.filterValue);
                    this.queryCriteria = this.filterField == new ValueCriteria(this.filterValue);
                }
            }

            queryCriteria = queryCriteria & ServiceQueryHelper.GetNotDeletedCriteria(detailRow);

            return true;
        }

        public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
        public void OnValidateRequest(IRetrieveRequestHandler handler) { }
        public void OnValidateRequest(IListRequestHandler handler) { }
        public void OnApplyFilters(IListRequestHandler handler, SqlQuery query) { }
        public void OnBeforeExecuteQuery(IListRequestHandler handler) { }
        public void OnAfterExecuteQuery(IListRequestHandler handler) { }

        public void OnPrepareQuery(IListRequestHandler handler, SqlQuery query)
        {
            if (ReferenceEquals(null, Target) ||
                handler.Request.EqualityFilter == null ||
                !attr.HandleEqualityFilter)
                return;

            object value;
            if (handler.Request.EqualityFilter.TryGetValue(Target.PropertyName, out value) ||
                handler.Request.EqualityFilter.TryGetValue(Target.Name, out value))
            {
                if (value == null || value as string == "")
                    return;

                var values = new List<object>();

                if (!(value is string) && value is IEnumerable)
                {
                    foreach (var val in (IEnumerable)value)
                        values.Add(itemKeyField.ConvertValue(val, CultureInfo.InvariantCulture));
                }
                else
                {
                    values.Add(itemKeyField.ConvertValue(value, CultureInfo.InvariantCulture));
                }

                if (values.Count > 0)
                {
                    var ls = new Alias(itemKeyField.Fields.TableName, "__ls");

                    query.Where(Criteria.Exists(
                        query.SubQuery()
                            .From(ls)
                            .Select("1")
                            .Where(
                                new Criteria(ls[thisKeyField]) == new Criteria((Field)((IIdRow)handler.Row).IdField) &
                                new Criteria(ls[itemKeyField]).In(values))
                            .ToString()));
                }

                handler.IgnoreEqualityFilter(Target.PropertyName);
                handler.IgnoreEqualityFilter(Target.Name);
            }
        }

        public void OnReturn(IRetrieveRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                !handler.AllowSelectField(Target) ||
                !handler.ShouldSelectField(Target))
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);

            var listHandler = DefaultHandlerFactory.ListHandlerFor(rowType);
            var listRequest = DefaultHandlerFactory.ListRequestFor(rowType);
            listRequest.ColumnSelection = ColumnSelection.KeyOnly;
            listRequest.IncludeColumns = new HashSet<string>
            {
                    itemKeyField.PropertyName ?? itemKeyField.Name
            };
            listRequest.Criteria = thisKeyCriteria == new ValueCriteria(idField.AsObject(handler.Row)) & filterCriteria;

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = listFactory();
            foreach (Row item in response.Entities)
                list.Add(itemKeyField.AsObject(item));

            Target.AsObject(handler.Row, list);
        }

        public void OnReturn(IListRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                !handler.AllowSelectField(Target) ||
                !handler.ShouldSelectField(Target) ||
                handler.Response.Entities.IsEmptyOrNull())
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);

            var listHandler = DefaultHandlerFactory.ListHandlerFor(rowType);
            var listRequest = DefaultHandlerFactory.ListRequestFor(rowType);
            listRequest.ColumnSelection = ColumnSelection.KeyOnly;
            listRequest.IncludeColumns = new HashSet<string>
            {
                itemKeyField.PropertyName ?? itemKeyField.Name,
                thisKeyField.PropertyName ?? thisKeyField.Name
            };

            var enumerator = handler.Response.Entities.Cast<Row>();
            while (true)
            {
                var part = enumerator.Take(1000);
                if (!part.Any())
                    break;

                enumerator = enumerator.Skip(1000);

                listRequest.Criteria = thisKeyCriteria.In(
                    part.Select(x => idField.AsObject(x))) & filterCriteria;

                IListResponse response = listHandler.Process(
                    handler.Connection, listRequest);

                var lookup = response.Entities.Cast<Row>()
                    .ToLookup(x => thisKeyField.AsObject(x).ToString());

                foreach (var row in part)
                {
                    var list = listFactory();
                    var matching = lookup[idField.AsObject(row).ToString()];
                    foreach (var x in matching)
                        list.Add(itemKeyField.AsObject(x));

                    Target.AsObject(row, list);
                }
            }
        }

        private void InsertDetail(IUnitOfWork uow, object masterId, object itemKey)
        {
            var detail = rowFactory();
            thisKeyField.AsObject(detail, masterId);
            itemKeyField.AsObject(detail, itemKeyField.ConvertValue(itemKey, CultureInfo.InvariantCulture));
            if (!ReferenceEquals(null, filterField))
                filterField.AsObject(detail, filterValue);

            var saveHandler = DefaultHandlerFactory.SaveHandlerFor(rowType);
            var saveRequest = DefaultHandlerFactory.SaveRequestFor(rowType);
            saveRequest.Entity = detail;
            saveHandler.Process(uow, saveRequest, SaveRequestType.Create);
        }

        private void DeleteDetail(IUnitOfWork uow, object detailId)
        {
            var deleteHandler = DefaultHandlerFactory.DeleteHandlerFor(rowType);
            var deleteRequest = DefaultHandlerFactory.DeleteRequestFor(rowType);
            deleteRequest.EntityId = detailId;
            deleteHandler.Process(uow, deleteRequest);
        }

        private void DetailListSave(IUnitOfWork uow, object masterId, IList<Row> oldRows, 
            IList<object> newItemKeys)
        {
            if (oldRows.Count == 0)
            {
                foreach (object itemKey in newItemKeys)
                    InsertDetail(uow, masterId, itemKey);

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

                InsertDetail(uow, masterId, itemKey);
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
                        InsertDetail(handler.UnitOfWork, masterId, itemKey);

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
                    .Where(
                        thisKeyField == new ValueCriteria(masterId) & 
                        queryCriteria)
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

            if (!attr.ForceCascadeDelete && ServiceQueryHelper.UseSoftDelete(handler.Row))
                return;

            var idField = (Field)((handler.Row as IIdRow).IdField);
            var masterId = idField.AsObject(handler.Row);
            var row = rowFactory();
            var rowIdField = (Field)((row as IIdRow).IdField);

            var deleteList = new List<object>();
            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select(rowIdField)
                    .Where(
                        thisKeyField == new ValueCriteria(masterId) &
                        queryCriteria)
                    .ForEach(handler.Connection, () =>
                    {
                        deleteList.Add(rowIdField.AsObject(row));
                    });

            foreach (var id in deleteList)
                DeleteDetail(handler.UnitOfWork, id);
        }
    }
}