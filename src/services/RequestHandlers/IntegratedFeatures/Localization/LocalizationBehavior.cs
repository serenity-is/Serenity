using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Behavior for handling localizable rows / properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="handlerFactory">Default handler factory</param>
/// <exception cref="ArgumentNullException"><paramref name="handlerFactory"/> is <c>null</c>.</exception>
public class LocalizationBehavior(IDefaultHandlerFactory handlerFactory) : BaseSaveDeleteBehaviorAsync,
    ISaveBehaviorSync, IDeleteBehaviorSync, IRetrieveBehaviorSync, IRetrieveBehaviorAsync, IImplicitBehavior
{
    private readonly IDefaultHandlerFactory handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    private LocalizationRowAttribute attr;
    private int rowPrefixLength;
    private Func<IIdRow> rowFactory;
    private Type localRowType;
    private Func<ILocalizationRow> localRowFactory;
    private int localRowPrefixLength;
    private Field foreignKeyField;
    private Field localRowIdField;
    private StringField cultureIdField;
    private ILocalizationRow localRowInstance;
    private BaseCriteria foreignKeyCriteria;
    private Func<IDictionary> dictionaryFactory;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        attr = row.GetType().GetCustomAttribute<LocalizationRowAttribute>();
        if (attr == null)
            return false;

        localRowType = attr.LocalizationRow;
        if (!typeof(ILocalizationRow).IsAssignableFrom(localRowType))
        {
            throw new ArgumentException(string.Format(
                "Row type '{0}' has a LocalizationRowAttribute, " +
                "but its localization row type ('{1}') doesn't implement ILocalizationRow interface!",
                    row.GetType().FullName, localRowType.FullName));
        }

        if (!typeof(IIdRow).IsAssignableFrom(localRowType))
        {
            throw new ArgumentException(string.Format(
                "Row type '{0}' has a LocalizationRowAttribute, " +
                "but its localization row type ('{1}') doesn't implement IIdRow interface!",
                    row.GetType().FullName, localRowType.FullName));
        }

        if (row is not IIdRow)
        {
            throw new ArgumentException(string.Format(
                "Row type '{0}' has a LocalizationRowAttribute, " +
                "but row type itself doesn't implement IIdRow interface!",
                    row.GetType().FullName));
        }

        var rowType = row.GetType();
        rowFactory = () => (IIdRow)Activator.CreateInstance(rowType);
        localRowFactory = () => (ILocalizationRow)Activator.CreateInstance(localRowType);

        var localRow = localRowFactory();
        localRowInstance = localRow;

        rowPrefixLength = PrefixHelper.DeterminePrefixLength(row.EnumerateTableFields(),
            x => x.Name);
        localRowPrefixLength = PrefixHelper.DeterminePrefixLength(localRow.EnumerateTableFields(),
            x => x.Name);
        localRowIdField = localRow.IdField;
        cultureIdField = localRow.CultureIdField;

        var foreignKeyFieldName = attr.MappedIdField ?? row.IdField.PropertyName;
        foreignKeyField = localRow.FindFieldByPropertyName(foreignKeyFieldName) ??
            localRow.FindField(foreignKeyFieldName);

        if (foreignKeyField is null)
        {
            throw new ArgumentException(string.Format(
                "Row type '{0}' has a LocalizationRowAttribute, " +
                "but its localization row type ('{1}') doesn't have a field with name '{2}'!",
                    row.GetType().FullName, localRowType.FullName, foreignKeyFieldName));
        }

        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(typeof(string), row.GetType());
        dictionaryFactory = () => (IDictionary)Activator.CreateInstance(dictionaryType);

        foreignKeyCriteria = new Criteria(foreignKeyField.PropertyName ?? foreignKeyField.Name);
        return true;
    }

    private Field GetLocalizationMatch(Field field)
    {
        return GetLocalizationMatch(field, localRowInstance, localRowPrefixLength, rowPrefixLength);
    }

    /// <summary>
    /// Gets localization match for a field
    /// </summary>
    /// <param name="field">Field</param>
    /// <param name="localRowInstance">Local row instance</param>
    /// <param name="localRowPrefixLength">Local row field name prefix length</param>
    /// <param name="rowPrefixLength">Row field name prefix length</param>
    public static Field GetLocalizationMatch(Field field, ILocalizationRow localRowInstance,
        int localRowPrefixLength = 0, int rowPrefixLength = 0)
    {
        if (!field.IsTableField())
            return null;

        if (field.GetAttribute<LocalizableAttribute>()?.IsLocalizable == false)
            return null;

        if (ReferenceEquals(field, field.Fields.IdField))
            return null;

        var searchName = field.Name;
        if (rowPrefixLength > 0)
            searchName = searchName[rowPrefixLength..];

        if (localRowPrefixLength > 0)
            searchName = localRowInstance.IdField.Name[..localRowPrefixLength] + searchName;

        var match = localRowInstance.FindField(searchName);
        if (match is null && field.PropertyName != null)
            match = localRowInstance.FindFieldByPropertyName(field.PropertyName);

        if (match is null)
            return null;

        if (!match.IsTableField())
            return null;

        if (match.GetAttribute<LocalizableAttribute>()?.IsLocalizable == false)
            return null;

        if (ReferenceEquals(match, localRowInstance.IdField) ||
            ReferenceEquals(match, localRowInstance.CultureIdField))
            return null;

        if (localRowInstance is IIsActiveRow iar &&
            ReferenceEquals(match, iar.IsActiveField))
            return null;

        if (localRowInstance is IIsDeletedRow idr &&
            ReferenceEquals(match, idr.IsDeletedField))
            return null;

        if (localRowInstance is IInsertDateRow insertDateRow &&
            ReferenceEquals(match, insertDateRow.InsertDateField))
            return null;

        if (localRowInstance is IInsertUserIdRow insertUserIdRow &&
            ReferenceEquals(match, insertUserIdRow.InsertUserIdField))
            return null;

        if (localRowInstance is IUpdateDateRow updateDateRow &&
            ReferenceEquals(match, updateDateRow.UpdateDateField))
            return null;

        if (localRowInstance is IUpdateUserIdRow updateUserIdRow &&
            ReferenceEquals(match, updateUserIdRow.UpdateUserIdField))
            return null;

        return match;
    }

    private object GetOldLocalizationRowId(IDbConnection connection, object recordId, string cultureId)
    {
        var row = localRowInstance.CreateNew();
        if (BuildOldLocalizationRowQuery(row, recordId, cultureId)
                .GetFirst(connection))
            return localRowIdField.AsObject(row);

        return null;
    }

    private async Task<object> GetOldLocalizationRowIdAsync(IDbConnection connection, object recordId, string cultureId,
        CancellationToken cancellationToken = default)
    {
        var row = localRowInstance.CreateNew();
        if (await BuildOldLocalizationRowQuery(row, recordId, cultureId)
                .GetFirstAsync(connection, cancellationToken).ConfigureAwait(false))
            return localRowIdField.AsObject(row);

        return null;
    }

    private SqlQuery BuildOldLocalizationRowQuery(IRow row, object recordId, string cultureId)
    {
        return new SqlQuery()
            .From(row)
            .Select(localRowIdField)
            .WhereEqual(foreignKeyField, recordId)
            .WhereEqual(cultureIdField, cultureId);
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IRetrieveRequestHandler handler)
    {
        if (handler.Request == null ||
            handler.Request.IncludeColumns == null ||
            !handler.Request.IncludeColumns.Contains("Localizations"))
            return;

        var localIdField = handler.Row.IdField;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(localRowType);
        var listRequest = listHandler.CreateRequest();
        PopulateListRequest(listRequest, localIdField, handler);

        var response = listHandler.Process(handler.Connection, listRequest);
        FillLocalizations(handler, response);
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (handler.Request == null ||
            handler.Request.IncludeColumns == null ||
            !handler.Request.IncludeColumns.Contains("Localizations"))
            return Task.CompletedTask;

        return OnReturnAsyncCore(handler, cancellationToken);
    }

    private async Task OnReturnAsyncCore(IRetrieveRequestHandler handler, CancellationToken cancellationToken)
    {
        var localIdField = handler.Row.IdField;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessorAsync>(localRowType);
        var listRequest = listHandler.CreateRequest();
        PopulateListRequest(listRequest, localIdField, handler);

        var response = await listHandler.ProcessAsync(handler.Connection, listRequest, cancellationToken).ConfigureAwait(false);
        FillLocalizations(handler, response);
    }

    private void PopulateListRequest(ListRequest listRequest, Field localIdField, IRetrieveRequestHandler handler)
    {
        listRequest.ColumnSelection = ColumnSelection.List;
        listRequest.Criteria = foreignKeyCriteria == new ValueCriteria(localIdField.AsObject(handler.Row));
    }

    private void FillLocalizations(IRetrieveRequestHandler handler, IListResponse response)
    {
        var row = rowFactory();
        var rowIdField = row.IdField;
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
        foreach (IRow localRow in response.Entities)
        {
            row = rowFactory();
            row.TrackAssignments = true;
            rowIdField.AsObject(row, rowIdField.AsObject(row));

            for (var i = 0; i < fields.Count; i++)
            {
                var match = matches[i];
                if (match is not null)
                {
                    var field = fields[i];
                    var value = match.AsObject(localRow);
                    field.AsObject(row, value);
                }
            }

            var culture = cultureIdField[localRow];
            dictionary[culture == null ? "" : culture.ToString()] = row;
        }

        handler.Response.Localizations = dictionary;
    }

    private void SaveLocalRow(IUnitOfWork uow, ILocalizationRow localRow, object masterId, object localRowId)
    {
        localRow = localRow.Clone();

        foreignKeyField.AsObject(localRow, masterId);
        localRow.IdField.AsObject(localRow, localRowId);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessor>(localRowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = localRow;
        saveHandler.Process(uow, saveRequest, localRowId == null ? SaveRequestType.Create : SaveRequestType.Update);
    }

    private async Task SaveLocalRowAsync(IUnitOfWork uow, ILocalizationRow localRow, object masterId, object localRowId,
        CancellationToken cancellationToken = default)
    {
        localRow = localRow.Clone();

        foreignKeyField.AsObject(localRow, masterId);
        localRow.IdField.AsObject(localRow, localRowId);

        var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessorAsync>(localRowType);
        var saveRequest = saveHandler.CreateRequest();
        saveRequest.Entity = localRow;
        await saveHandler.ProcessAsync(uow, saveRequest,
            localRowId == null ? SaveRequestType.Create : SaveRequestType.Update, cancellationToken).ConfigureAwait(false);
    }

    private void DeleteLocalRow(IUnitOfWork uow, object detailId)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessor>(localRowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
        deleteHandler.Process(uow, deleteRequest);
    }

    private Task DeleteLocalRowAsync(IUnitOfWork uow, object detailId, CancellationToken cancellationToken = default)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessorAsync>(localRowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
        return deleteHandler.ProcessAsync(uow, deleteRequest, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual void OnAfterSave(ISaveRequestHandler handler)
    {
        var localizations = handler.Request.Localizations;
        if (localizations == null)
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        foreach (DictionaryEntry pair in localizations)
        {
            var cultureId = cultureIdField.ConvertValue(pair.Key, CultureInfo.InvariantCulture)?.ToString();
            var oldId = handler.IsCreate ? null : GetOldLocalizationRowId(handler.UnitOfWork.Connection, masterId, cultureId);
            var localRow = localRowFactory();
            localRow.TrackAssignments = true;
            if (oldId == null)
                cultureIdField[localRow] = cultureId;

            var row = pair.Value as IRow;

            bool anyNonEmpty = false;

            foreach (var field in row.GetFields())
            {
                if (ReferenceEquals(field, idField))
                    continue;

                if (!row.IsAssigned(field))
                    continue;

                var match = GetLocalizationMatch(field) ?? throw new ValidationError("CantLocalize", field.Name, string.Format("{0} field is not localizable!",
                        field.PropertyName ?? field.Name));
                var value = field.AsObject(row);
                match.AsObject(localRow, value);

                if (value != null &&
                    (value is not string || !string.IsNullOrWhiteSpace(value as string)))
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

    /// <inheritdoc/>
    public override async Task OnAfterSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        var localizations = handler.Request.Localizations;
        if (localizations == null)
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        foreach (DictionaryEntry pair in localizations)
        {
            var cultureId = cultureIdField.ConvertValue(pair.Key, CultureInfo.InvariantCulture)?.ToString();
            var oldId = handler.IsCreate ? null : await GetOldLocalizationRowIdAsync(handler.UnitOfWork.Connection, masterId, cultureId, cancellationToken).ConfigureAwait(false);
            var localRow = localRowFactory();
            localRow.TrackAssignments = true;
            if (oldId == null)
                cultureIdField[localRow] = cultureId;

            var row = pair.Value as IRow;

            bool anyNonEmpty = false;

            foreach (var field in row.GetFields())
            {
                if (ReferenceEquals(field, idField))
                    continue;

                if (!row.IsAssigned(field))
                    continue;

                var match = GetLocalizationMatch(field) ?? throw new ValidationError("CantLocalize", field.Name, string.Format("{0} field is not localizable!",
                        field.PropertyName ?? field.Name));
                var value = field.AsObject(row);
                match.AsObject(localRow, value);

                if (value != null &&
                    (value is not string || !string.IsNullOrWhiteSpace(value as string)))
                {
                    anyNonEmpty = true;
                }
            }

            if (anyNonEmpty)
                await SaveLocalRowAsync(handler.UnitOfWork, localRow, masterId, oldId, cancellationToken).ConfigureAwait(false);
            else if (oldId != null)
                await DeleteLocalRowAsync(handler.UnitOfWork, oldId, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public virtual void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        if (ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var deleteList = GetLocalRowIdList(handler);

        foreach (var localId in deleteList)
            DeleteLocalRow(handler.UnitOfWork, localId);
    }

    /// <inheritdoc/>
    public override async Task OnBeforeDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var deleteList = await GetLocalRowIdListAsync(handler, cancellationToken).ConfigureAwait(false);

        foreach (var localId in deleteList)
            await DeleteLocalRowAsync(handler.UnitOfWork, localId, cancellationToken).ConfigureAwait(false);
    }

    private List<object> GetLocalRowIdList(IDeleteRequestHandler handler)
    {
        var idField = handler.Row.IdField;
        var localRow = localRowFactory();

        var deleteList = new List<object>();
        new SqlQuery()
                .Dialect(handler.Connection.GetDialect())
                .From(localRow)
                .Select(localRowIdField)
                .Where(
                    foreignKeyField == new ValueCriteria(idField.AsSqlValue(handler.Row)))
                .ForEach(handler.Connection, () =>
                {
                    deleteList.Add(localRowIdField.AsObject(localRow));
                });

        return deleteList;
    }

    private async Task<List<object>> GetLocalRowIdListAsync(IDeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        var idField = handler.Row.IdField;
        var localRow = localRowFactory();

        var deleteList = new List<object>();
        await new SqlQuery()
                .Dialect(handler.Connection.GetDialect())
                .From(localRow)
                .Select(localRowIdField)
                .Where(
                    foreignKeyField == new ValueCriteria(idField.AsSqlValue(handler.Row)))
                .ForEachAsync(handler.Connection, () =>
                {
                    deleteList.Add(localRowIdField.AsObject(localRow));
                }, cancellationToken).ConfigureAwait(false);

        return deleteList;
    }
}
