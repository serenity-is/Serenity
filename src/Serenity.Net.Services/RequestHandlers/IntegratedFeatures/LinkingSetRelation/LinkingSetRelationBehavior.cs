using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="LinkingSetRelationAttribute"/>
/// </summary>
public class LinkingSetRelationBehavior : BaseSaveDeleteBehavior,
    IImplicitBehavior, IRetrieveBehavior, IListBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private readonly IDefaultHandlerFactory handlerFactory;
    private LinkingSetRelationAttribute attr;
    private Type rowType;
    private Field thisKeyField;
    private Criteria thisKeyCriteria;
    private Field itemKeyField;
    private Field filterField;
    private object filterValue;
    private BaseCriteria filterCriteria;
    private BaseCriteria queryCriteria;
    private Func<IRow> rowFactory;
    private Func<IList> listFactory;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="handlerFactory">Default handler factory</param>
    /// <exception cref="ArgumentNullException">handlerFactory is null</exception>
    public LinkingSetRelationBehavior(IDefaultHandlerFactory handlerFactory)
    {
        this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    }

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        attr = Target.GetAttribute<LinkingSetRelationAttribute>();
        if (attr == null)
            return false;

        if (row is not IIdRow)
        {
            throw new ArgumentException(string.Format("Field '{0}' in row type '{1}' has a [LinkingSetRelation] attribute " +
                "but it doesn't implement IIdRow!",
                Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        var listType = Target.ValueType;
        if (!listType.IsGenericType ||
            listType.GetGenericTypeDefinition() != typeof(List<>))
        {
            throw new ArgumentException(string.Format("Field '{0}' in row type '{1}' has a [LinkingSetRelation] attribute " +
                "but its property type is not a generic List (e.g. List<int>)!",
                Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        rowType = attr.RowType;
        if (rowType.IsAbstract ||
            !typeof(IRow).IsAssignableFrom(rowType) ||
            rowType.IsInterface)
        {
            throw new ArgumentException(string.Format(
                "Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                "but specified row type is not valid row class!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        if (!typeof(IIdRow).IsAssignableFrom(rowType))
        {
            throw new ArgumentException(string.Format(
                "Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                "but specified row type doesn't implement IIdRow!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        listFactory = () => (IList)Activator.CreateInstance(listType);
        rowFactory = () => (IRow)Activator.CreateInstance(rowType);

        var detailRow = rowFactory();

        thisKeyField = detailRow.FindFieldByPropertyName(attr.ThisKey) ??
            detailRow.FindField(attr.ThisKey);

        if (thisKeyField is null)
            throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                attr.ThisKey, detailRow.GetType().FullName,
                Target.PropertyName ?? Target.Name, row.GetType().FullName));

        thisKeyCriteria = new Criteria(thisKeyField.PropertyName ?? thisKeyField.Name);

        itemKeyField = detailRow.FindFieldByPropertyName(attr.ItemKey) ??
            detailRow.FindField(attr.ItemKey);

        if (itemKeyField is null)
            throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                attr.ItemKey, detailRow.GetType().FullName,
                Target.PropertyName ?? Target.Name, row.GetType().FullName));

        if (!string.IsNullOrEmpty(attr.FilterField))
        {
            filterField = detailRow.FindFieldByPropertyName(attr.FilterField) ?? detailRow.FindField(attr.FilterField);
            if (filterField is null)
                throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation as FilterField in field '{2}' of row type '{3}'.",
                    attr.FilterField, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            filterCriteria = new Criteria(filterField.PropertyName ?? filterField.Name);
            filterValue = filterField.ConvertValue(attr.FilterValue, CultureInfo.InvariantCulture);
            if (filterValue == null)
            {
                filterCriteria = filterCriteria.IsNull();
                queryCriteria = filterField.IsNull();
            }
            else
            {
                filterCriteria = filterCriteria == new ValueCriteria(filterValue);
                queryCriteria = filterField == new ValueCriteria(filterValue);
            }
        }

        queryCriteria &= ServiceQueryHelper.GetNotDeletedCriteria(detailRow);

        return true;
    }

    /// <inheritdoc/>
    public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }        
    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
    /// <inheritdoc/>
    public void OnValidateRequest(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnValidateRequest(IListRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnApplyFilters(IListRequestHandler handler, SqlQuery query) { }
    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IListRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnAfterExecuteQuery(IListRequestHandler handler) { }

    /// <inheritdoc/>
    public void OnPrepareQuery(IListRequestHandler handler, SqlQuery query)
    {
        if (Target is null ||
            handler.Request.EqualityFilter == null ||
            !attr.HandleEqualityFilter)
            return;

        if (handler.Request.EqualityFilter.TryGetValue(Target.PropertyName, out object value) ||
            handler.Request.EqualityFilter.TryGetValue(Target.Name, out value))
        {
            if (value == null || value as string == "")
                return;

            var values = new List<object>();

            if (value is not string && value is IEnumerable enumerable)
            {
                foreach (var val in enumerable)
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
                            new Criteria(ls[thisKeyField]) == new Criteria(handler.Row.IdField) &
                            new Criteria(ls[itemKeyField]).In(values))
                        .ToString()));
            }

            handler.IgnoreEqualityFilter(Target.PropertyName);
            handler.IgnoreEqualityFilter(Target.Name);
        }
    }

    /// <inheritdoc/>
    public void OnReturn(IRetrieveRequestHandler handler)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target))
            return;

        var idField = handler.Row.IdField;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(rowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = ColumnSelection.KeyOnly;
        listRequest.IncludeColumns = new HashSet<string>
        {
                itemKeyField.PropertyName ?? itemKeyField.Name
        };
        listRequest.Criteria = thisKeyCriteria == new ValueCriteria(idField.AsObject(handler.Row)) & filterCriteria;

        IListResponse response = listHandler.Process(handler.Connection, listRequest);

        var list = listFactory();
        foreach (IRow item in response.Entities)
            list.Add(itemKeyField.AsObject(item));

        Target.AsObject(handler.Row, list);
    }

    /// <inheritdoc/>
    public void OnReturn(IListRequestHandler handler)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target) ||
            handler.Response.Entities.IsEmptyOrNull())
            return;

        var idField = handler.Row.IdField;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(rowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = ColumnSelection.KeyOnly;
        listRequest.IncludeColumns = new HashSet<string>
        {
            itemKeyField.PropertyName ?? itemKeyField.Name,
            thisKeyField.PropertyName ?? thisKeyField.Name
        };

        var enumerator = handler.Response.Entities.Cast<IRow>();
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

            var lookup = response.Entities.Cast<IRow>()
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
        filterField?.AsObject(detail, filterValue);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessor>(rowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = detail;
        saveHandler.Process(uow, saveRequest, SaveRequestType.Create);
    }

    private void DeleteDetail(IUnitOfWork uow, object detailId)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessor>(rowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
        deleteHandler.Process(uow, deleteRequest);
    }

    private void DetailListSave(IUnitOfWork uow, object masterId, IList<IRow> oldRows,
        IList<object> newItemKeys)
    {
        if (oldRows.Count == 0)
        {
            foreach (object itemKey in newItemKeys)
                InsertDetail(uow, masterId, itemKey);

            return;
        }

        var row = rowFactory();
        var rowIdField = row.IdField;

        newItemKeys = newItemKeys.Where(x => x != null).Distinct().ToList();

        if (newItemKeys.Count == 0)
        {
            foreach (IRow entity in oldRows)
                DeleteDetail(uow, rowIdField.AsObject(entity));

            return;
        }

        var oldByItemKey = new Dictionary<string, IRow>(oldRows.Count);
        foreach (IRow item in oldRows)
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
                foreach (IRow item in oldRows)
                {
                    var id = rowIdField.AsObject(item);
                    DeleteDetail(uow, id);
                }

                oldRows = new List<IRow>();
                oldByItemKey = new Dictionary<string, IRow>();
            }
        }

        var newByItemKey = new HashSet<string>();
        foreach (object item in newItemKeys)
        {
            if (item != null)
                newByItemKey.Add(item.ToString());
        }

        foreach (IRow item in oldRows)
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

    /// <inheritdoc/>
    public override void OnAfterSave(ISaveRequestHandler handler)
    {
        if (Target.AsObject(handler.Row) is not IList newList)
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        if (handler.IsCreate)
        {
            foreach (object itemKey in newList)
                if (itemKey != null)
                    InsertDetail(handler.UnitOfWork, masterId, itemKey);

            return;
        }

        var oldRows = new List<IRow>();

        var row = rowFactory();
        var rowIdField = row.IdField;

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

    /// <inheritdoc/>
    public override void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        if (Target is null ||
            (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
            return;

        if (!attr.ForceCascadeDelete && ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);
        var row = rowFactory();
        var rowIdField = row.IdField;

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