using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="LinkingSetRelationAttribute"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="handlerFactory">Default handler factory</param>
/// <exception cref="ArgumentNullException"><paramref name="handlerFactory"/> is <c>null</c>.</exception>
public class LinkingSetRelationBehavior(IDefaultHandlerFactory handlerFactory) : BaseSaveDeleteBehaviorAsync,
    ISaveBehaviorSync, IDeleteBehaviorSync, IRetrieveBehaviorSync, IRetrieveBehaviorAsync,
    IListBehaviorSync, IListBehaviorAsync, IFieldBehavior, IImplicitBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private readonly IDefaultHandlerFactory handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
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
    public virtual void OnPrepareQuery(IListRequestHandler handler, SqlQuery query)
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
    public virtual Task OnPrepareQueryAsync(IListRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        OnPrepareQuery(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IRetrieveRequestHandler handler)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target))
            return;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(rowType);
        var listRequest = BuildRetrieveListRequest(listHandler.CreateRequest(), handler);
        var response = listHandler.Process(handler.Connection, listRequest);
        FillRetrieveList(handler, response);
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target))
            return Task.CompletedTask;

        return OnReturnAsyncCore(handler, cancellationToken);
    }

    private async Task OnReturnAsyncCore(IRetrieveRequestHandler handler, CancellationToken cancellationToken)
    {
        var listHandler = handlerFactory.CreateHandler<IListRequestProcessorAsync>(rowType);
        var listRequest = BuildRetrieveListRequest(listHandler.CreateRequest(), handler);
        var response = await listHandler.ProcessAsync(handler.Connection, listRequest, cancellationToken).ConfigureAwait(false);
        FillRetrieveList(handler, response);
    }

    private ListRequest BuildRetrieveListRequest(ListRequest listRequest, IRetrieveRequestHandler handler)
    {
        var idField = handler.Row.IdField;
        listRequest.ColumnSelection = ColumnSelection.KeyOnly;
        listRequest.IncludeColumns = [itemKeyField.PropertyName ?? itemKeyField.Name];
        listRequest.Criteria = thisKeyCriteria == new ValueCriteria(idField.AsObject(handler.Row)) & filterCriteria;
        return listRequest;
    }

    private void FillRetrieveList(IRetrieveRequestHandler handler, IListResponse response)
    {
        var list = listFactory();
        foreach (IRow item in response.Entities)
            list.Add(itemKeyField.AsObject(item));

        Target.AsObject(handler.Row, list);
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IListRequestHandler handler)
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
        listRequest.IncludeColumns =
        [
            itemKeyField.PropertyName ?? itemKeyField.Name,
            thisKeyField.PropertyName ?? thisKeyField.Name
        ];

        var enumerator = handler.Response.Entities.Cast<IRow>();
        while (true)
        {
            var part = enumerator.Take(1000);
            if (!part.Any())
                break;

            enumerator = enumerator.Skip(1000);

            listRequest.Criteria = thisKeyCriteria.In(
                part.Select(idField.AsObject)) & filterCriteria;

            IListResponse response = listHandler.Process(
                handler.Connection, listRequest);

            FillListPart(handler, part, response);
        }
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IListRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target) ||
            handler.Response.Entities.IsEmptyOrNull())
            return Task.CompletedTask;

        return OnReturnAsyncCore(handler, cancellationToken);
    }

    private async Task OnReturnAsyncCore(IListRequestHandler handler, CancellationToken cancellationToken)
    {
        var idField = handler.Row.IdField;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessorAsync>(rowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = ColumnSelection.KeyOnly;
        listRequest.IncludeColumns =
        [
            itemKeyField.PropertyName ?? itemKeyField.Name,
            thisKeyField.PropertyName ?? thisKeyField.Name
        ];

        var enumerator = handler.Response.Entities.Cast<IRow>();
        while (true)
        {
            var part = enumerator.Take(1000);
            if (!part.Any())
                break;

            enumerator = enumerator.Skip(1000);

            listRequest.Criteria = thisKeyCriteria.In(
                part.Select(idField.AsObject)) & filterCriteria;

            var response = await listHandler.ProcessAsync(
                handler.Connection, listRequest, cancellationToken).ConfigureAwait(false);

            FillListPart(handler, part, response);
        }
    }

    private void FillListPart(IListRequestHandler handler, IEnumerable<IRow> part, IListResponse response)
    {
        var idField = handler.Row.IdField;
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

    private void InsertDetail(IUnitOfWork uow, object masterId, object itemKey)
    {
        var detail = rowFactory();
        thisKeyField.AsObject(detail, masterId);
        itemKeyField.AsInvariant(detail, itemKey);
        filterField?.AsObject(detail, filterValue);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessor>(rowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = detail;
        saveHandler.Process(uow, saveRequest, SaveRequestType.Create);
    }

    private Task InsertDetailAsync(IUnitOfWork uow, object masterId, object itemKey,
        CancellationToken cancellationToken = default)
    {
        var detail = rowFactory();
        thisKeyField.AsObject(detail, masterId);
        itemKeyField.AsInvariant(detail, itemKey);
        filterField?.AsObject(detail, filterValue);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessorAsync>(rowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = detail;
        return saveHandler.ProcessAsync(uow, saveRequest, SaveRequestType.Create, cancellationToken);
    }

    private void DeleteDetail(IUnitOfWork uow, object detailId)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessor>(rowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
        deleteHandler.Process(uow, deleteRequest);
    }

    private Task DeleteDetailAsync(IUnitOfWork uow, object detailId, CancellationToken cancellationToken = default)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessorAsync>(rowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
        return deleteHandler.ProcessAsync(uow, deleteRequest, cancellationToken);
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

        var newKeys = newItemKeys.Where(x => x != null).Distinct().ToList();

        if (newKeys.Count == 0)
        {
            var rowIdField = ((IIdRow)rowFactory()).IdField;
            foreach (IRow entity in oldRows)
                DeleteDetail(uow, rowIdField.AsObject(entity));

            return;
        }

        var changes = ComputeDetailChanges(oldRows, newKeys);

        foreach (var row in changes.RowsToDelete)
            DeleteDetail(uow, ((IIdRow)row).IdField.AsObject(row));

        foreach (object itemKey in changes.KeysToInsert)
            InsertDetail(uow, masterId, itemKey);
    }

    private async Task DetailListSaveAsync(IUnitOfWork uow, object masterId, IList<IRow> oldRows,
        IList<object> newItemKeys, CancellationToken cancellationToken = default)
    {
        if (oldRows.Count == 0)
        {
            foreach (object itemKey in newItemKeys)
                await InsertDetailAsync(uow, masterId, itemKey, cancellationToken).ConfigureAwait(false);

            return;
        }

        var newKeys = newItemKeys.Where(x => x != null).Distinct().ToList();

        if (newKeys.Count == 0)
        {
            var rowIdField = ((IIdRow)rowFactory()).IdField;
            foreach (IRow entity in oldRows)
                await DeleteDetailAsync(uow, rowIdField.AsObject(entity), cancellationToken).ConfigureAwait(false);

            return;
        }

        var changes = ComputeDetailChanges(oldRows, newKeys);

        foreach (var row in changes.RowsToDelete)
            await DeleteDetailAsync(uow, ((IIdRow)row).IdField.AsObject(row), cancellationToken).ConfigureAwait(false);

        foreach (object itemKey in changes.KeysToInsert)
            await InsertDetailAsync(uow, masterId, itemKey, cancellationToken).ConfigureAwait(false);
    }

    private DetailChanges ComputeDetailChanges(IList<IRow> oldRows, IList<object> newItemKeys)
    {
        var oldByItemKey = BuildOldByItemKey(oldRows);
        var rowsToDelete = new List<IRow>();

        if (attr.PreserveOrder)
        {
            if (!newItemKeys.Take(oldRows.Count).SequenceEqual(
                    oldRows.Select(x => itemKeyField.AsObject(x))) &&
                newItemKeys.Any(x => !oldByItemKey.ContainsKey(x.ToString())))
            {
                rowsToDelete.AddRange(oldRows);
                oldByItemKey = [];
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
            if (rowsToDelete.Contains(item))
                continue;

            var itemKey = itemKeyField.AsObject(item);
            if (itemKey == null || !newByItemKey.Contains(itemKey.ToString()))
                rowsToDelete.Add(item);
        }

        var keysToInsert = new List<object>();
        foreach (object itemKey in newItemKeys)
        {
            if (oldByItemKey.ContainsKey(itemKey.ToString()))
                continue;

            keysToInsert.Add(itemKey);
        }

        return new DetailChanges(rowsToDelete, keysToInsert);
    }

    private sealed class DetailChanges(List<IRow> rowsToDelete, List<object> keysToInsert)
    {
        public List<IRow> RowsToDelete { get; } = rowsToDelete;
        public List<object> KeysToInsert { get; } = keysToInsert;
    }

    private Dictionary<string, IRow> BuildOldByItemKey(IList<IRow> oldRows)
    {
        var oldByItemKey = new Dictionary<string, IRow>(oldRows.Count);
        foreach (IRow item in oldRows)
        {
            var itemKey = itemKeyField.AsObject(item);
            if (itemKey != null)
                oldByItemKey[itemKey.ToString()] = item;
        }

        return oldByItemKey;
    }

    /// <inheritdoc/>
    public virtual void OnAfterSave(ISaveRequestHandler handler)
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

        var oldRows = GetOldRows(handler, masterId, handler.Connection);

        DetailListSave(handler.UnitOfWork, masterId, oldRows,
            newList.Cast<object>().ToList());
    }

    /// <inheritdoc/>
    public override async Task OnAfterSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (Target.AsObject(handler.Row) is not IList newList)
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        if (handler.IsCreate)
        {
            foreach (object itemKey in newList)
                if (itemKey != null)
                    await InsertDetailAsync(handler.UnitOfWork, masterId, itemKey, cancellationToken).ConfigureAwait(false);

            return;
        }

        var oldRows = await GetOldRowsAsync(handler, masterId, handler.Connection, cancellationToken).ConfigureAwait(false);

        await DetailListSaveAsync(handler.UnitOfWork, masterId, oldRows,
            newList.Cast<object>().ToList(), cancellationToken).ConfigureAwait(false);
    }

    private List<IRow> GetOldRows(ISaveRequestHandler handler, object masterId, IDbConnection connection)
    {
        var oldRows = new List<IRow>();

        var row = rowFactory();
        var rowIdField = row.IdField;

        BuildOldRowsQuery(connection, row, rowIdField, itemKeyField, masterId)
            .ForEach(connection, () =>
            {
                oldRows.Add(row.Clone());
            });

        return oldRows;
    }

    private async Task<List<IRow>> GetOldRowsAsync(ISaveRequestHandler handler, object masterId, IDbConnection connection,
        CancellationToken cancellationToken = default)
    {
        var oldRows = new List<IRow>();

        var row = rowFactory();
        var rowIdField = row.IdField;

        await BuildOldRowsQuery(connection, row, rowIdField, itemKeyField, masterId)
            .ForEachAsync(connection, () =>
            {
                oldRows.Add(row.Clone());
            }, cancellationToken).ConfigureAwait(false);

        return oldRows;
    }

    private SqlQuery BuildOldRowsQuery(IDbConnection connection, IRow row, Field rowIdField, Field itemKeyField, object masterId)
    {
        return new SqlQuery()
            .Dialect(connection.GetDialect())
            .From(row)
            .Select(rowIdField)
            .Select(itemKeyField)
            .OrderBy(rowIdField)
            .Where(
                thisKeyField == new ValueCriteria(masterId) &
                queryCriteria);
    }

    /// <inheritdoc/>
    public virtual void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        if (Target is null ||
            (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
            return;

        if (!attr.ForceCascadeDelete && ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        var deleteList = GetDeleteList(handler, masterId, handler.Connection);
        foreach (var id in deleteList)
            DeleteDetail(handler.UnitOfWork, id);
    }

    /// <inheritdoc/>
    public override async Task OnBeforeDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (Target is null ||
            (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
            return;

        if (!attr.ForceCascadeDelete && ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        var deleteList = await GetDeleteListAsync(handler, masterId, handler.Connection, cancellationToken).ConfigureAwait(false);
        foreach (var id in deleteList)
            await DeleteDetailAsync(handler.UnitOfWork, id, cancellationToken).ConfigureAwait(false);
    }

    private List<object> GetDeleteList(IDeleteRequestHandler handler, object masterId, IDbConnection connection)
    {
        var row = rowFactory();
        var rowIdField = row.IdField;

        var deleteList = new List<object>();
        BuildDeleteListQuery(connection, row, rowIdField, masterId)
            .ForEach(connection, () =>
            {
                deleteList.Add(rowIdField.AsObject(row));
            });

        return deleteList;
    }

    private async Task<List<object>> GetDeleteListAsync(IDeleteRequestHandler handler, object masterId, IDbConnection connection,
        CancellationToken cancellationToken = default)
    {
        var row = rowFactory();
        var rowIdField = row.IdField;

        var deleteList = new List<object>();
        await BuildDeleteListQuery(connection, row, rowIdField, masterId)
            .ForEachAsync(connection, () =>
            {
                deleteList.Add(rowIdField.AsObject(row));
            }, cancellationToken).ConfigureAwait(false);

        return deleteList;
    }

    private SqlQuery BuildDeleteListQuery(IDbConnection connection, IRow row, Field rowIdField, object masterId)
    {
        return new SqlQuery()
            .Dialect(connection.GetDialect())
            .From(row)
            .Select(rowIdField)
            .Where(
                thisKeyField == new ValueCriteria(masterId) &
                queryCriteria);
    }
}
