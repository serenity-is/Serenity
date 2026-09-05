using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="MasterDetailRelationAttribute"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="handlerFactory">Default handler factory</param>
/// <exception cref="ArgumentNullException"><paramref name="handlerFactory"/> is <c>null</c>.</exception>
public class MasterDetailRelationBehavior(IDefaultHandlerFactory handlerFactory) : BaseSaveDeleteBehaviorAsync,
    ISaveBehaviorSync, IDeleteBehaviorSync, IRetrieveBehaviorSync, IRetrieveBehaviorAsync,
    IListBehaviorSync, IListBehaviorAsync, IFieldBehavior, IImplicitBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private readonly IDefaultHandlerFactory handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    private MasterDetailRelationAttribute attr;
    private Func<IList> rowListFactory;
    private Func<IRow> rowFactory;
    private Type rowType;
    private Field foreignKeyField;
    private BaseCriteria foreignKeyCriteria;
    private Field filterField;
    private Field masterKeyField;
    private object filterValue;
    private BaseCriteria filterCriteria;
    private BaseCriteria queryCriteria;
    private HashSet<string> includeColumns;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        attr = Target.GetAttribute<MasterDetailRelationAttribute>();
        if (attr == null)
            return false;

        var rowListType = Target.ValueType;
        if (!rowListType.IsGenericType ||
            rowListType.GetGenericTypeDefinition() != typeof(List<>))
        {
            throw new ArgumentException(string.Format("Field '{0}' in row type '{1}' has a MasterDetailRelationAttribute " +
                "but its property type is not a generic List (e.g. List<IRow>)!",
                Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        rowType = rowListType.GetGenericArguments()[0];
        if (rowType.IsAbstract ||
            !typeof(IRow).IsAssignableFrom(rowType) ||
            rowType.IsInterface)
        {
            throw new ArgumentException(string.Format(
                "Field '{0}' in row type '{1}' has a MasterDetailRelationAttribute " +
                "but its property type is not a generic list of rows (e.g. List<IRow>)!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        rowListFactory = () => (IList)Activator.CreateInstance(rowListType);
        rowFactory = () => (IRow)Activator.CreateInstance(rowType);

        if (attr.MasterKeyField != null)
        {
            // Use field from AltIdField
            masterKeyField = row.FindFieldByPropertyName(attr.MasterKeyField) ??
                row.FindField(attr.MasterKeyField);

            if (masterKeyField is null)
                throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a master detail relation in field '{2}'.",
                    attr.MasterKeyField, row.GetType().FullName,
                    Target.PropertyName ?? Target.Name));
        }
        else
        {
            // Default behaviour: use id field
            masterKeyField = row.IdField;
        }

        var detailRow = rowFactory();
        foreignKeyField = detailRow.FindFieldByPropertyName(attr.ForeignKey) ??
            detailRow.FindField(attr.ForeignKey);

        if (foreignKeyField is null)
            throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                "This field is specified for a master detail relation in field '{2}' of row type '{3}'.",
                attr.ForeignKey, detailRow.GetType().FullName,
                Target.PropertyName ?? Target.Name, row.GetType().FullName));

        foreignKeyCriteria = new Criteria(foreignKeyField.PropertyName ?? foreignKeyField.Name);

        if (!string.IsNullOrEmpty(attr.FilterField))
        {
            filterField = detailRow.FindFieldByPropertyName(attr.FilterField) ?? detailRow.FindField(attr.FilterField);
            if (filterField is null)
                throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a master detail relation as FilterField in field '{2}' of row type '{3}'.",
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

        includeColumns = [];

        if (!string.IsNullOrEmpty(attr.IncludeColumns))
            foreach (var s in attr.IncludeColumns.Split(','))
            {
                var col = s.TrimToNull();
                if (col != null)
                    includeColumns.Add(col);
            }

        if (attr.IncludeColumnNames != null)
            foreach (var s in attr.IncludeColumnNames)
                includeColumns.Add(s);

        if (attr.ColumnsType != null)
            foreach (var p in attr.ColumnsType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public))
                includeColumns.Add(p.Name);

        return true;
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
        listRequest.ColumnSelection = attr.ColumnSelection;
        listRequest.IncludeColumns = includeColumns;
        listRequest.Criteria = foreignKeyCriteria == new ValueCriteria(masterKeyField.AsObject(handler.Row)) & filterCriteria;
        return listRequest;
    }

    private void FillRetrieveList(IRetrieveRequestHandler handler, IListResponse response)
    {
        var list = rowListFactory();
        foreach (var item in response.Entities)
            list.Add(item);

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

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(rowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = attr.ColumnSelection;
        listRequest.IncludeColumns = includeColumns;

        var enumerator = handler.Response.Entities.Cast<IRow>();
        while (true)
        {
            var part = enumerator.Take(1000);
            if (!part.Any())
                break;

            enumerator = enumerator.Skip(1000);

            listRequest.Criteria = foreignKeyCriteria.In(
                part.Select(x => masterKeyField.AsObject(x))) & filterCriteria;

            var response = listHandler.Process(handler.Connection, listRequest);
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
        var listHandler = handlerFactory.CreateHandler<IListRequestProcessorAsync>(rowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = attr.ColumnSelection;
        listRequest.IncludeColumns = includeColumns;

        var enumerator = handler.Response.Entities.Cast<IRow>();
        while (true)
        {
            var part = enumerator.Take(1000);
            if (!part.Any())
                break;

            enumerator = enumerator.Skip(1000);

            listRequest.Criteria = foreignKeyCriteria.In(
                part.Select(x => masterKeyField.AsObject(x))) & filterCriteria;

            var response = await listHandler.ProcessAsync(handler.Connection, listRequest, cancellationToken).ConfigureAwait(false);
            FillListPart(handler, part, response);
        }
    }

    private void FillListPart(IListRequestHandler handler, IEnumerable<IRow> part, IListResponse response)
    {
        var lookup = response.Entities.Cast<IRow>()
            .ToLookup(x => AsString(foreignKeyField.AsObject(x)));

        foreach (var row in part)
        {
            var list = rowListFactory();
            var matching = lookup[AsString(masterKeyField.AsObject(row))];
            foreach (var x in matching)
                list.Add(x);

            Target.AsObject(row, list);
        }
    }

    private void SaveDetail(IUnitOfWork uow, IRow detail, object masterId, object detailId)
    {
        detail = PrepareDetail(detail, masterId, detailId);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessor>(rowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = detail;
        saveHandler.Process(uow, saveRequest, detailId == null ? SaveRequestType.Create : SaveRequestType.Update);
    }

    private async Task SaveDetailAsync(IUnitOfWork uow, IRow detail, object masterId, object detailId,
        CancellationToken cancellationToken = default)
    {
        detail = PrepareDetail(detail, masterId, detailId);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessorAsync>(rowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = detail;
        await saveHandler.ProcessAsync(uow, saveRequest,
            detailId == null ? SaveRequestType.Create : SaveRequestType.Update, cancellationToken).ConfigureAwait(false);
    }

    private IRow PrepareDetail(IRow detail, object masterId, object detailId)
    {
        detail = detail.Clone();

        foreignKeyField.AsObject(detail, masterId);
        filterField?.AsObject(detail, filterValue);

        detail.IdField.AsObject(detail, detailId);
        return detail;
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

    private static string AsString(object obj)
    {
        if (obj == null)
            return null;

        return obj.ToString();
    }

    private void DetailListSave(IUnitOfWork uow, object masterId, IList oldList, IList newList)
    {
        var changes = ComputeDetailChanges(oldList, newList);
        if (changes == null)
            return;

        foreach (var row in changes.RowsToUpdate)
            SaveDetail(uow, row.Row, masterId, row.Id);

        foreach (var row in changes.RowsToDelete)
            DeleteDetail(uow, row.Id);

        foreach (var row in changes.RowsToInsert)
            SaveDetail(uow, row.Row, masterId, null);
    }

    private async Task DetailListSaveAsync(IUnitOfWork uow, object masterId, IList oldList, IList newList,
        CancellationToken cancellationToken = default)
    {
        var changes = ComputeDetailChanges(oldList, newList);
        if (changes == null)
            return;

        foreach (var row in changes.RowsToUpdate)
            await SaveDetailAsync(uow, row.Row, masterId, row.Id, cancellationToken).ConfigureAwait(false);

        foreach (var row in changes.RowsToDelete)
            await DeleteDetailAsync(uow, row.Id, cancellationToken).ConfigureAwait(false);

        foreach (var row in changes.RowsToInsert)
            await SaveDetailAsync(uow, row.Row, masterId, null, cancellationToken).ConfigureAwait(false);
    }

    private DetailChanges ComputeDetailChanges(IList oldList, IList newList)
    {
        if ((oldList.Count > 0 ? oldList[0] :
            (newList.Count > 0) ? newList[0] : null) is not IRow row)
            return null;

        var rowIdField = row.IdField;

        var changes = new DetailChanges();
        if (oldList.Count == 0)
        {
            foreach (IRow entity in newList)
                changes.RowsToInsert.Add(new DetailRow(entity, null));

            return changes;
        }

        if (newList.Count == 0)
        {
            foreach (IRow entity in oldList)
                changes.RowsToDelete.Add(new DetailRow(null, rowIdField.AsObject(entity)));

            return changes;
        }

        var oldById = new Dictionary<string, IRow>(oldList.Count);
        foreach (IRow item in oldList)
            oldById[AsString(rowIdField.AsObject(item))] = item;

        var newById = new Dictionary<string, IRow>(newList.Count);
        foreach (IRow item in newList)
        {
            var idStr = AsString(rowIdField.AsObject(item));

            if (!string.IsNullOrEmpty(idStr))
                newById[idStr] = item;
        }

        foreach (IRow item in oldList)
        {
            var id = rowIdField.AsObject(item);
            var idStr = AsString(id);
            if (!newById.ContainsKey(idStr))
                changes.RowsToDelete.Add(new DetailRow(null, id));
        }

        foreach (IRow item in newList)
        {
            var id = rowIdField.AsObject(item);
            var idStr = AsString(id);

            if (string.IsNullOrEmpty(idStr) || !oldById.TryGetValue(idStr, out IRow old))
                continue;

            if (attr.CheckChangesOnUpdate)
            {
                bool anyChanges = false;
                foreach (var field in item.GetFields())
                {
                    if (item.IsAssigned(field) &&
                        (field.Flags & FieldFlags.Updatable) == FieldFlags.Updatable &&
                        field.IndexCompare(old, item) != 0)
                    {
                        anyChanges = true;
                        break;
                    }
                }

                if (!anyChanges)
                    continue;
            }

            changes.RowsToUpdate.Add(new DetailRow(item, id));
        }

        foreach (IRow item in newList)
        {
            var id = rowIdField.AsObject(item);
            var idStr = AsString(id);
            if (string.IsNullOrEmpty(idStr) || !oldById.ContainsKey(idStr))
                changes.RowsToInsert.Add(new DetailRow(item, null));
        }

        return changes;
    }

    private sealed class DetailRow(IRow row, object id)
    {
        public IRow Row { get; } = row;
        public object Id { get; } = id;
    }

    private sealed class DetailChanges
    {
        public List<DetailRow> RowsToDelete { get; } = [];
        public List<DetailRow> RowsToUpdate { get; } = [];
        public List<DetailRow> RowsToInsert { get; } = [];
    }

    /// <inheritdoc/>
    public virtual void OnAfterSave(ISaveRequestHandler handler)
    {
        if (Target.AsObject(handler.Row) is not IList newList)
            return;

        var masterId = masterKeyField.AsObject(handler.Row);

        if (handler.IsCreate)
        {
            foreach (IRow entity in newList)
                SaveDetail(handler.UnitOfWork, entity, masterId, null);

            return;
        }

        var oldList = GetOldList(handler);
        DetailListSave(handler.UnitOfWork, masterId, oldList, newList);
    }

    /// <inheritdoc/>
    public override async Task OnAfterSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (Target.AsObject(handler.Row) is not IList newList)
            return;

        var masterId = masterKeyField.AsObject(handler.Row);

        if (handler.IsCreate)
        {
            foreach (IRow entity in newList)
                await SaveDetailAsync(handler.UnitOfWork, entity, masterId, null, cancellationToken).ConfigureAwait(false);

            return;
        }

        var oldList = await GetOldListAsync(handler, cancellationToken).ConfigureAwait(false);
        await DetailListSaveAsync(handler.UnitOfWork, masterId, oldList, newList, cancellationToken).ConfigureAwait(false);
    }

    private IList GetOldList(ISaveRequestHandler handler)
    {
        var oldList = new List<IRow>();

        if (!attr.CheckChangesOnUpdate)
        {
            var row = rowFactory();
            var rowIdField = row.IdField;

            // if we're not gonna compare old rows with new ones
            // no need to call list request handler

            BuildOldRowsQuery(handler, row, rowIdField)
                .ForEach(handler.Connection, () =>
                {
                    oldList.Add(row.Clone());
                });
        }
        else
        {
            var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(rowType);
            var listRequest = listHandler.CreateRequest();
            listRequest.ColumnSelection = ColumnSelection.List;
            listRequest.Criteria = foreignKeyCriteria == new ValueCriteria(masterKeyField.AsObject(handler.Row)) & filterCriteria;

            var entities = listHandler.Process(handler.Connection, listRequest).Entities;
            foreach (IRow entity in entities)
                oldList.Add(entity);
        }

        return oldList;
    }

    private async Task<IList> GetOldListAsync(ISaveRequestHandler handler, CancellationToken cancellationToken)
    {
        var oldList = new List<IRow>();

        if (!attr.CheckChangesOnUpdate)
        {
            var row = rowFactory();
            var rowIdField = row.IdField;

            // if we're not gonna compare old rows with new ones
            // no need to call list request handler

            await BuildOldRowsQuery(handler, row, rowIdField)
                .ForEachAsync(handler.Connection, () =>
                {
                    oldList.Add(row.Clone());
                }, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var listHandler = handlerFactory.CreateHandler<IListRequestProcessorAsync>(rowType);
            var listRequest = listHandler.CreateRequest();
            listRequest.ColumnSelection = ColumnSelection.List;
            listRequest.Criteria = foreignKeyCriteria == new ValueCriteria(masterKeyField.AsObject(handler.Row)) & filterCriteria;

            var entities = (await listHandler.ProcessAsync(handler.Connection, listRequest, cancellationToken).ConfigureAwait(false)).Entities;
            foreach (IRow entity in entities)
                oldList.Add(entity);
        }

        return oldList;
    }

    private SqlQuery BuildOldRowsQuery(ISaveRequestHandler handler, IRow row, Field rowIdField)
    {
        return new SqlQuery()
            .Dialect(handler.Connection.GetDialect())
            .From(row)
            .Select(rowIdField)
            .Where(
                foreignKeyField == new ValueCriteria(masterKeyField.AsSqlValue(handler.Row)) &
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

        var deleteList = GetDeleteList(handler);
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

        var deleteList = await GetDeleteListAsync(handler, cancellationToken).ConfigureAwait(false);
        foreach (var id in deleteList)
            await DeleteDetailAsync(handler.UnitOfWork, id, cancellationToken).ConfigureAwait(false);
    }

    private List<object> GetDeleteList(IDeleteRequestHandler handler)
    {
        var row = rowFactory();
        var rowIdField = row.IdField;

        var deleteList = new List<object>();
        BuildDeleteListQuery(handler, row, rowIdField)
            .ForEach(handler.Connection, () =>
            {
                deleteList.Add(rowIdField.AsObject(row));
            });

        return deleteList;
    }

    private async Task<List<object>> GetDeleteListAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken)
    {
        var row = rowFactory();
        var rowIdField = row.IdField;

        var deleteList = new List<object>();
        await BuildDeleteListQuery(handler, row, rowIdField)
            .ForEachAsync(handler.Connection, () =>
            {
                deleteList.Add(rowIdField.AsObject(row));
            }, cancellationToken).ConfigureAwait(false);

        return deleteList;
    }

    private SqlQuery BuildDeleteListQuery(IDeleteRequestHandler handler, IRow row, Field rowIdField)
    {
        return new SqlQuery()
            .Dialect(handler.Connection.GetDialect())
            .From(row)
            .Select(rowIdField)
            .Where(
                foreignKeyField == new ValueCriteria(masterKeyField.AsSqlValue(handler.Row)) &
                queryCriteria);
    }
}
