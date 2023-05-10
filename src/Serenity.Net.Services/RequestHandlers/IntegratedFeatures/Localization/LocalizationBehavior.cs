using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Behavior for handling localizable rows / properties
/// </summary>
public class LocalizationBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IRetrieveBehavior
{
    private readonly IDefaultHandlerFactory handlerFactory;
    private LocalizationRowAttribute attr;
    private int rowPrefixLength;
    private Func<IIdRow> rowFactory;
    private Type localRowType;
    private Func<ILocalizationRow> localRowFactory;
    private int localRowPrefixLength;
    private Field foreignKeyField;
    private Field localRowIdField;
    private Field cultureIdField;
    private ILocalizationRow localRowInstance;
    private BaseCriteria foreignKeyCriteria;
    private Func<IDictionary> dictionaryFactory;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="handlerFactory">Default handler factory</param>
    /// <exception cref="ArgumentNullException">handlerFactory is null</exception>
    public LocalizationBehavior(IDefaultHandlerFactory handlerFactory)
    {
        this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    }

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

    internal static Field GetLocalizationMatch(Field field, ILocalizationRow localRowInstance,
        int localRowPrefixLength, int rowPrefixLength)
    {
        if (!field.IsTableField())
            return null;

        var name = field.Name[rowPrefixLength..];
        name = localRowInstance.IdField.Name.Substring(0, localRowPrefixLength) + name;
        var match = localRowInstance.FindField(name);
        if (match is null && field.PropertyName != null)
            match = localRowInstance.FindFieldByPropertyName(field.PropertyName);

        if (match is null)
            return null;

        if (!match.IsTableField())
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

        if (localRowInstance is IInsertLogRow insertLog && (
            ReferenceEquals(match, insertLog.InsertUserIdField) ||
            ReferenceEquals(match, insertLog.InsertDateField)))
            return null;

        if (localRowInstance is IUpdateLogRow updateLog && (
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

    /// <inheritdoc/>
    public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
    /// <inheritdoc/>
    public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
    /// <inheritdoc/>
    public void OnValidateRequest(IRetrieveRequestHandler handler) { }

    /// <inheritdoc/>
    public void OnReturn(IRetrieveRequestHandler handler)
    {
        if (handler.Request == null ||
            handler.Request.IncludeColumns == null ||
            !handler.Request.IncludeColumns.Contains("Localizations"))
            return;

        var localIdField = handler.Row.IdField;

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(localRowType);
        var listRequest = listHandler.CreateRequest();

        listRequest.ColumnSelection = ColumnSelection.List;
        listRequest.Criteria = foreignKeyCriteria == new ValueCriteria(localIdField.AsObject(handler.Row));

        IListResponse response = listHandler.Process(handler.Connection, listRequest);

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

            var culture = cultureIdField.AsObject(localRow);
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

    private void DeleteLocalRow(IUnitOfWork uow, object detailId)
    {
        var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessor>(localRowType);
        var deleteRequest = deleteHandler.CreateRequest();
        deleteRequest.EntityId = detailId;
        deleteHandler.Process(uow, deleteRequest);
    }

    /// <inheritdoc/>
    public override void OnAfterSave(ISaveRequestHandler handler)
    {
        var localizations = handler.Request.Localizations;
        if (localizations == null)
            return;

        var idField = handler.Row.IdField;
        var masterId = idField.AsObject(handler.Row);

        foreach (DictionaryEntry pair in localizations)
        {
            var cultureId = cultureIdField.ConvertValue(pair.Key, CultureInfo.InvariantCulture);
            var oldId = handler.IsCreate ? null : GetOldLocalizationRowId(handler.UnitOfWork.Connection, masterId, cultureId);
            var localRow = localRowFactory();
            localRow.TrackAssignments = true;
            if (oldId == null)
                cultureIdField.AsObject(localRow, cultureId);

            var row = pair.Value as IRow;

            bool anyNonEmpty = false;

            foreach (var field in row.GetFields())
            {
                if (ReferenceEquals(field, idField))
                    continue;

                if (!row.IsAssigned(field))
                    continue;

                var match = GetLocalizationMatch(field);
                if (match is null)
                    throw new ValidationError("CantLocalize", field.Name, string.Format("{0} field is not localizable!",
                        field.PropertyName ?? field.Name));

                var value = field.AsObject(row);
                match.AsObject(localRow, value);

                if (value != null &&
                    (value is not string || !(value as string).IsTrimmedEmpty()))
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
    public override void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        if (ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

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

        foreach (var localId in deleteList)
            DeleteLocalRow(handler.UnitOfWork, localId);
    }
}