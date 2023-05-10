namespace Serenity.Services;

/// <summary>
/// Behavior that handles <see cref="UpdatableExtensionAttribute"/>
/// </summary>
public class UpdatableExtensionBehavior : BaseSaveDeleteBehavior, IImplicitBehavior
{
    private class RelationInfo
    {
        public UpdatableExtensionAttribute Attr;
        public Func<IRow> RowFactory;
        public Field ThisKeyField;
        public Field OtherKeyField;
        public Field FilterField;
        public object FilterValue;
        public List<Tuple<Field, Field>> Mappings;
        public Field PresenceField;
        public object PresenceValue;
    }

    private readonly IDefaultHandlerFactory handlerFactory;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="handlerFactory">Default handler factory</param>
    /// <exception cref="ArgumentNullException">handlerFactory is null</exception>
    public UpdatableExtensionBehavior(IDefaultHandlerFactory handlerFactory)
    {
        this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    }

    private List<RelationInfo> infoList;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        var attrs = row.GetType().GetCustomAttributes<UpdatableExtensionAttribute>();

        if (attrs == null || !attrs.Any())
            return false;

        var sourceByExpression = row.GetFields().ToLookup(x =>
            BracketLocator.ReplaceBrackets(x.Expression.TrimToEmpty(), BracketRemoverDialect.Instance));

        infoList = attrs.Select(attr =>
        {
            var info = new RelationInfo
            {
                Attr = attr
            };

            var rowType = attr.RowType;
            if (rowType.IsAbstract ||
                !typeof(IRow).IsAssignableFrom(rowType) ||
                rowType.IsInterface)
            {
                throw new ArgumentException(string.Format(
                    "Row type '{1}' has an ExtensionRelation attribute " +
                    "but its specified extension row type '{0}' is not a valid row class!",
                        rowType.FullName,
                        row.GetType().FullName));
            }

            info.RowFactory = () => (IRow)Activator.CreateInstance(rowType);

            var thisKey = attr.ThisKey;
            if (string.IsNullOrEmpty(thisKey))
            {
                if (row is not IIdRow)
                {
                    throw new ArgumentException(string.Format(
                        "Row type '{0}' has an ExtensionRelation attribute " +
                        "but its ThisKey is not specified!",
                            row.GetType().FullName));
                }

                info.ThisKeyField = row.IdField;
            }
            else
            {
                info.ThisKeyField = row.FindFieldByPropertyName(attr.ThisKey) ?? 
                    row.FindField(attr.ThisKey);
                if (info.ThisKeyField is null)
                    throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                        "This field is specified for an ExtensionRelation attribute",
                        attr.ThisKey,
                        row.GetType().FullName));
            }

            var ext = info.RowFactory();

            var otherKey = attr.OtherKey;
            if (string.IsNullOrEmpty(otherKey))
            {
                info.OtherKeyField = ext.FindField(info.ThisKeyField.Name);

                if (info.OtherKeyField is null && ext is IIdRow)
                    info.OtherKeyField = row.IdField;

                if (info.OtherKeyField is null)
                    throw new ArgumentException(string.Format(
                        "Row type '{1}' has an ExtensionRelation attribute " +
                        "but its OtherKey is not specified!",
                            row.GetType().FullName));
            }
            else
            {
                info.OtherKeyField = ext.FindFieldByPropertyName(attr.OtherKey) ?? ext.FindField(attr.OtherKey);
                if (info.OtherKeyField is null)
                    throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                        "This field is specified for an ExtensionRelation attribute on '{2}'",
                        attr.OtherKey,
                        ext.GetType().FullName,
                        row.GetType().FullName));
            }

            if (!string.IsNullOrEmpty(attr.FilterField))
            {
                info.FilterField = ext.FindFieldByPropertyName(attr.FilterField) ?? ext.FindField(attr.FilterField);
                if (info.FilterField is null)
                    throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                        "This field is specified as FilterField for an ExtensionRelation attribute on '{2}'",
                        attr.OtherKey,
                        ext.GetType().FullName,
                        row.GetType().FullName));

                info.FilterValue = info.FilterField.ConvertValue(attr.FilterValue, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(attr.PresenceField))
            {
                info.PresenceField = row.FindFieldByPropertyName(attr.PresenceField) ?? row.FindField(attr.PresenceField);
                if (info.PresenceField is null)
                    throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                        "This field is specified as PresenceField as an ExtensionRelation attribute.",
                        attr.PresenceField,
                        row.GetType().FullName));

                info.PresenceValue = attr.PresenceValue;
            }

            var extFields = ext.GetFields();
            var alias = attr.Alias;
            var aliasPrefix = attr.Alias + "_";

            var joinByKey = new HashSet<string>(extFields.Joins.Keys, StringComparer.OrdinalIgnoreCase);

            string mapAlias(string x)
            {
                if (x == "t0" || x == "T0")
                    return alias;

                if (!joinByKey.Contains(x))
                    return x;

                return aliasPrefix + x;
            }

            string mapExpression(string x)
            {
                if (x == null)
                    return null;

                return JoinAliasLocator.ReplaceAliases(x, mapAlias);
            }

            info.Mappings = new List<Tuple<Field, Field>>();
            foreach (var field in extFields)
            {
                if (ReferenceEquals(info.OtherKeyField, field))
                    continue;

                if (ReferenceEquals(info.FilterField, field))
                    continue;

                var expression = field.Expression.TrimToEmpty();

                if (string.IsNullOrEmpty(expression))
                    continue;

                expression = mapExpression(expression);
                expression = BracketLocator.ReplaceBrackets(expression, 
                    BracketRemoverDialect.Instance);

                var match = sourceByExpression[expression].FirstOrDefault();
                if (match is null)
                    continue;

                if (match.IsTableField())
                    continue;

                if (ReferenceEquals(info.ThisKeyField, match))
                    continue;

                if (field.GetType() != match.GetType())
                    throw new ArgumentException(string.Format(
                        "Row type '{0}' has an ExtensionRelation attribute to '{1}'." +
                        "Their '{2}' and '{3}' fields are matched but they have different types ({4} and {5})!",
                            row.GetType().FullName,
                            ext.GetType().FullName,
                            field.PropertyName ?? field.Name,
                            match.PropertyName ?? match.Name,
                            field.GetType().Name,
                            match.GetType().Name));

                info.Mappings.Add(new Tuple<Field, Field>(match, field));
            }

            if (info.Mappings.Count == 0)
                throw new ArgumentException(string.Format(
                    "Row type '{0}' has an ExtensionRelation attribute " +
                    "but no view fields could be matched to extension row '{1}'!",
                        row.GetType().FullName,
                        ext.GetType().FullName));

            return info;
        }).ToList();

        return true;
    }

    /// <inheritdoc/>
    public override void OnBeforeSave(ISaveRequestHandler handler)
    {
        foreach (var info in infoList)
        {
            var mappings = info.Mappings.Where(x => handler.Row.IsAssigned(x.Item1)).ToList();

            if (!mappings.Any())
                continue;

            handler.StateBag["UpdatableExtensionBehavior_Assignments_" + info.Attr.Alias] =
                mappings;
        }
    }

    private object GetExistingID(IDbConnection connection, RelationInfo info,
        object thisKey)
    {
        var criteria = new Criteria(info.OtherKeyField.PropertyName ?? info.OtherKeyField.Name) ==
            new ValueCriteria(thisKey);

        if (info.FilterField is not null)
        {
            var flt = new Criteria(info.FilterField.PropertyName ?? info.FilterField.Name);
            if (info.FilterValue == null)
                criteria &= flt.IsNull();
            else
                criteria &= flt == new ValueCriteria(info.FilterValue);
        }

        var listHandler = handlerFactory.CreateHandler<IListRequestProcessor>(info.Attr.RowType);
        var listRequest = listHandler.CreateRequest();
        listRequest.ColumnSelection = ColumnSelection.KeyOnly;
        listRequest.Criteria = criteria;

        var existing = listHandler.Process(connection, listRequest).Entities;

        if (existing.Count > 1)
            throw new Exception(string.Format("Found multiple extension rows for UpdatableExtension '{0}'", 
                info.Attr.Alias));

        if (existing.Count == 0)
            return null;

        return ((IRow)existing[0]).IdField.AsObject((IRow)existing[0]);
    }

    private bool CheckPresenceValue(RelationInfo info, IRow row)
    {
        if (info.PresenceField is not null)
        {
            if (info.PresenceField is not BooleanField &&
                info.PresenceValue is bool b)
            {
                if (info.PresenceField.IsNull(row) == b)
                    return false;
            }
            else
            {
                var newRow = row.CreateNew();
                info.PresenceField.AsObject(newRow, info.PresenceField.ConvertValue(
                    info.PresenceValue, CultureInfo.InvariantCulture));
                if (info.PresenceField.IndexCompare(row, newRow) != 0)
                    return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override void OnAfterSave(ISaveRequestHandler handler)
    {
        foreach (var info in infoList)
        {
            if (!handler.StateBag.TryGetValue("UpdatableExtensionBehavior_Assignments_" + info.Attr.Alias, out object mappingsObj))
                continue;

            var mappings = (IEnumerable<Tuple<Field, Field>>)mappingsObj;
            if (mappings == null || !mappings.Any())
                continue;

            var thisKey = info.ThisKeyField.AsObject(handler.Row);
            if (thisKey is null)
                continue;

            object oldID = GetExistingID(handler.Connection, info, thisKey);
            if (oldID == null && !CheckPresenceValue(info, handler.Row))
                continue;

            var extension = info.RowFactory();

            if (oldID != null)
                ((IIdRow)extension).IdField.AsObject(extension, oldID);

            info.OtherKeyField.AsObject(extension, thisKey);
            info.FilterField?.AsObject(extension, info.FilterValue);

            var saveHandler = handlerFactory.CreateHandler<ISaveRequestProcessor>(info.Attr.RowType);
            var request = saveHandler.CreateRequest();
            request.Entity = extension;
            request.EntityId = oldID;

            foreach (var mapping in mappings)
                mapping.Item2.AsObject(extension, mapping.Item1.AsObject(handler.Row));

            saveHandler.Process(handler.UnitOfWork, 
                request, oldID == null ? SaveRequestType.Create : SaveRequestType.Update);
        }
    }

    /// <inheritdoc/>
    public override void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        foreach (var info in infoList)
        {
            if (!info.Attr.CascadeDelete)
                continue;

            var thisKey = info.ThisKeyField.AsObject(handler.Row);
            if (thisKey is null)
                continue;

            var oldID = GetExistingID(handler.Connection, info, thisKey);
            if (oldID == null)
                continue;

            var deleteHandler = handlerFactory.CreateHandler<IDeleteRequestProcessor>(info.Attr.RowType);
            var deleteRequest = deleteHandler.CreateRequest();
            deleteRequest.EntityId = oldID;
            deleteHandler.Process(handler.UnitOfWork, deleteRequest);
        }
    }
}