using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="MasterDetailRelationAttribute"/>
/// </summary>
public class MasterDetailRelationBehavior : BaseSaveDeleteBehavior,
    IImplicitBehavior, IRetrieveBehavior, IListBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private readonly IDefaultHandlerFactory handlerFactory;
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

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="handlerFactory">Default handler factory</param>
    /// <exception cref="ArgumentNullException">handlerFactory is null</exception>
    public MasterDetailRelationBehavior(IDefaultHandlerFactory handlerFactory)
    {
        this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    }
    
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

        includeColumns = new HashSet<string>();

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
    public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
    /// <inheritdoc/>
    public void OnValidateRequest(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnPrepareQuery(IListRequestHandler handler, SqlQuery query) { }
    /// <inheritdoc/>
    public void OnValidateRequest(IListRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnApplyFilters(IListRequestHandler handler, SqlQuery query) { }
    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IListRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnAfterExecuteQuery(IListRequestHandler handler) { }

    /// <inheritdoc/>
    public void OnReturn(IRetrieveRequestHandler handler)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target))
            return;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(rowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = attr.ColumnSelection;
        listRequest.IncludeColumns = includeColumns;
        listRequest.Criteria = foreignKeyCriteria == new ValueCriteria(masterKeyField.AsObject(handler.Row)) & filterCriteria;

        IListResponse response = listHandler.Process(handler.Connection, listRequest);

        var list = rowListFactory();
        foreach (var item in response.Entities)
            list.Add(item);

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

            IListResponse response = listHandler.Process(
                handler.Connection, listRequest);

            var lookup = response.Entities.Cast<IRow>()
                .ToLookup(x => foreignKeyField.AsObject(x).ToString());

            foreach (var row in part)
            {
                var list = rowListFactory();
                var matching = lookup[masterKeyField.AsObject(row).ToString()];
                foreach (var x in matching)
                    list.Add(x);

                Target.AsObject(row, list);
            }
        }
    }

    private void SaveDetail(IUnitOfWork uow, IRow detail, object masterId, object detailId)
    {
        detail = detail.Clone();

        foreignKeyField.AsObject(detail, masterId);
        filterField?.AsObject(detail, filterValue);

        detail.IdField.AsObject(detail, detailId);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessor>(rowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = detail;
        saveHandler.Process(uow, saveRequest, detailId == null ? SaveRequestType.Create : SaveRequestType.Update);
    }

    private void DeleteDetail(IUnitOfWork uow, object detailId)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessor>(rowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
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
        if ((oldList.Count > 0 ? oldList[0] :
            (newList.Count > 0) ? newList[0] : null) is not IRow row)
            return;

        if (oldList.Count == 0)
        {
            foreach (IRow entity in newList)
                SaveDetail(uow, entity, masterId, null);

            return;
        }

        var rowIdField = row.IdField;

        if (newList.Count == 0)
        {
            foreach (IRow entity in oldList)
                DeleteDetail(uow, rowIdField.AsObject(entity));

            return;
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
                DeleteDetail(uow, id);
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

            SaveDetail(uow, item, masterId, id);
        }

        foreach (IRow item in newList)
        {
            var id = rowIdField.AsObject(item);
            var idStr = AsString(id);
            if (string.IsNullOrEmpty(idStr) || !oldById.ContainsKey(idStr))
                SaveDetail(uow, item, masterId, null);
        }
    }

    /// <inheritdoc/>
    public override void OnAfterSave(ISaveRequestHandler handler)
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

        var oldList = new List<IRow>();

        if (!attr.CheckChangesOnUpdate)
        {
            var row = rowFactory();
            var rowIdField = (row as IIdRow).IdField;

            // if we're not gonna compare old rows with new ones
            // no need to call list request handler

            new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select(rowIdField)
                    .Where(
                        foreignKeyField == new ValueCriteria(masterKeyField.AsSqlValue(handler.Row)) &
                        queryCriteria)
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

        DetailListSave(handler.UnitOfWork, masterId, oldList, newList);
    }

    /// <inheritdoc/>
    public override void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        if (Target is null ||
            (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
            return;

        if (!attr.ForceCascadeDelete && ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var row = rowFactory();
        var rowIdField = row.IdField;

        var deleteList = new List<object>();
        new SqlQuery()
                .Dialect(handler.Connection.GetDialect())
                .From(row)
                .Select(rowIdField)
                .Where(
                    foreignKeyField == new ValueCriteria(masterKeyField.AsSqlValue(handler.Row)) &
                    queryCriteria)
                .ForEach(handler.Connection, () =>
                {
                    deleteList.Add(rowIdField.AsObject(row));
                });

        foreach (var id in deleteList)
            DeleteDetail(handler.UnitOfWork, id);
    }
}
