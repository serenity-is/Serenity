namespace Serenity.Services;

/// <summary>
/// Abstract base class for retrieve request handlers that share state and
/// mode neutral helper methods between synchronous and asynchronous
/// retrieve request handlers.
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TRetrieveRequest">Retrieve request type</typeparam>
/// <typeparam name="TRetrieveResponse">Retrieve response type</typeparam>
public abstract class RetrieveRequestHandlerBase<TRow, TRetrieveRequest, TRetrieveResponse> : IRetrieveRequestHandler
    where TRow : class, IRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    protected RetrieveRequestHandlerBase(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        StateBag = new Dictionary<string, object>();
    }

    /// <summary>
    /// Gets the list of retrieve behaviors.
    /// </summary>
    protected virtual IEnumerable<IRetrieveBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, IRetrieveBehavior>(GetType());
    }

    /// <summary>
    /// Returns true if the field should be allowed to be selected,
    /// based on its read permission and the SelectLevel.Never flag.
    /// </summary>
    /// <param name="field">The field.</param>
    protected virtual bool AllowSelectField(Field field)
    {
        if (field.MinSelectLevel == SelectLevel.Never)
            return false;

        if (field.ReadPermission != null &&
            !Permissions.HasPermission(field.ReadPermission))
            return false;

        return true;
    }

    /// <summary>
    /// Returns true if the field should be selected, based on
    /// current <see cref="ColumnSelection"/>, field <see cref="MinSelectLevelAttribute"/>,
    /// the field being a not mapped (<see cref="NotMappedAttribute"/>) field, table field,
    /// or a view / expression field.
    /// </summary>
    /// <param name="field">The field</param>
    protected virtual bool ShouldSelectField(Field field)
    {
        var mode = field.MinSelectLevel;

        if (field.MinSelectLevel == SelectLevel.Never)
            return false;

        if (mode == SelectLevel.Always)
            return true;

        if (mode == SelectLevel.Auto)
        {
            bool notMapped = (field.Flags & FieldFlags.NotMapped) == FieldFlags.NotMapped;
            if (notMapped)
            {
                // normally not-mapped fields are skipped in SelectFields method, 
                // but some relations like MasterDetailRelation etc. use this method (ShouldSelectFields)
                // to determine if they should populate those fields themselves.
                // so we return here Details so that edit forms works properly on default retrieve
                // mode (Details) without having to include such columns explicitly.
                mode = SelectLevel.Details;
            }
            else if (field.IsLookup)
                mode = SelectLevel.Lookup;
            else
            {
                // assume that non-foreign calculated and reflective fields should be selected in list mode
                bool isForeign = (field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign;
                mode = isForeign ? SelectLevel.Details : SelectLevel.List;
            }
        }

        bool explicitlyExcluded = Request.ExcludeColumns != null &&
            (Request.ExcludeColumns.Contains(field.Name) ||
                (field.PropertyName != null && Request.ExcludeColumns.Contains(field.PropertyName)));

        if (explicitlyExcluded)
            return false;

        bool explicitlyIncluded = !explicitlyExcluded && Request.IncludeColumns != null &&
            (Request.IncludeColumns.Contains(field.Name) ||
                (field.PropertyName != null && Request.IncludeColumns.Contains(field.PropertyName)));

        if (explicitlyIncluded)
            return true;

        var selection = Request.ColumnSelection;

        return selection switch
        {
            RetrieveColumnSelection.List => mode <= SelectLevel.List,
            RetrieveColumnSelection.KeyOnly => ReferenceEquals(field, Row.IdField) ||
                (field.Flags & FieldFlags.PrimaryKey) == FieldFlags.PrimaryKey,
            RetrieveColumnSelection.Details => mode <= SelectLevel.Details,
            RetrieveColumnSelection.None => false,
            RetrieveColumnSelection.IdOnly => ReferenceEquals(field, Row.IdField),
            RetrieveColumnSelection.Lookup => mode <= SelectLevel.Lookup,
            _ => false
        };
    }

    /// <summary>
    /// Returns true if field is included in <see cref="RetrieveRequest.IncludeColumns"/>
    /// </summary>
    /// <param name="field">Field</param>
    protected bool IsIncluded(Field field)
    {
        return Request.IncludeColumns != null &&
            (Request.IncludeColumns.Contains(field.Name) ||
             (field.PropertyName != null && Request.IncludeColumns.Contains(field.PropertyName)));
    }

    /// <summary>
    /// Returns true if field is included in <see cref="RetrieveRequest.IncludeColumns"/>
    /// </summary>
    /// <param name="column">Field</param>
    protected bool IsIncluded(string column)
    {
        return Request.IncludeColumns != null &&
            Request.IncludeColumns.Contains(column);
    }

    /// <summary>
    /// Calls query.Select(field)
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="field">field</param>
    protected virtual void SelectField(SqlQuery query, Field field)
    {
        query.Select(field);
    }

    /// <summary>
    /// Calls query.Select(field) for all the fields without <see cref="FieldFlags.NotMapped"/>,
    /// and if <see cref="AllowSelectField(Field)"/> and <see cref="ShouldSelectField(Field)"/>
    /// returns true.
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void SelectFields(SqlQuery query)
    {
        foreach (var field in Row.GetFields())
        {
            if ((field.Flags & FieldFlags.NotMapped) == FieldFlags.NotMapped)
                continue;

            if (AllowSelectField(field) && ShouldSelectField(field))
                SelectField(query, field);
        }
    }

    /// <summary>
    /// Validates if the user is allowed to query this entity type by checking <see cref="ReadPermissionAttribute"/>
    /// and <see cref="ServiceLookupPermissionAttribute"/> if the request is in lookup access mode.
    /// </summary>
    protected virtual void ValidatePermissions()
    {
        var readAttr = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);
        if (readAttr != null)
            Permissions.ValidatePermission(readAttr.Permission ?? "?", Localizer);
    }

    /// <summary>
    /// Creates a query instance with the dialect for current connection.
    /// </summary>
    protected virtual SqlQuery CreateQuery()
    {
        var query = new SqlQuery()
            .Dialect(Connection.GetDialect())
            .From(Row);

        var idField = ((IIdRow)Row).IdField;
        var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        query.WhereEqual(idField, id);

        return query;
    }

    /// <summary>
    /// Gets the two level cache from the request context.
    /// </summary>
    public ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context.
    /// </summary>
    public IRequestContext Context { get; private set; }

    /// <summary>
    /// Gets the localizer from the request context.
    /// </summary>
    public ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets the permission service from the request context.
    /// </summary>
    public IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets the current user from the request context.
    /// </summary>
    public ClaimsPrincipal User => Context.User;

    /// <summary>
    /// Gets the current connection.
    /// </summary>
    public IDbConnection Connection { get; protected set; }

    /// <summary>
    /// Gets the select query.
    /// </summary>
    public SqlQuery Query { get; protected set; }

    /// <summary>
    /// Gets the entity used for querying / metadata lookup.
    /// </summary>
    public TRow Row { get; protected set; }

    /// <summary>
    /// Gets the request object.
    /// </summary>
    public TRetrieveRequest Request { get; protected set; }

    /// <summary>
    /// Gets the response object.
    /// </summary>
    public TRetrieveResponse Response { get; protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object> StateBag { get; private set; }

    IRow IRetrieveRequestHandler.Row => Row;
    RetrieveRequest IRetrieveRequestHandler.Request => Request;
    IRetrieveResponse IRetrieveRequestHandler.Response => Response;
    bool IRetrieveRequestHandler.ShouldSelectField(Field field) { return ShouldSelectField(field); }
    bool IRetrieveRequestHandler.AllowSelectField(Field field) { return AllowSelectField(field); }
}
