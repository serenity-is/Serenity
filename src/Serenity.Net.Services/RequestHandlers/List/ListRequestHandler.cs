using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Generic base class for list request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <typeparam name="TListResponse">List response type</typeparam>
public class ListRequestHandler<TRow, TListRequest, TListResponse> : IListRequestProcessor,
    IListHandler<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    /// <summary>
    /// Set of ignored equality filter entries
    /// </summary>
    protected HashSet<string> ignoredEqualityFilters;

    /// <summary>
    /// Lazy list of behaviors that is activated for this request
    /// </summary>
    protected Lazy<IListBehavior[]> behaviors;

    /// <summary>
    /// True if the list handler is in lookup access mode, e.g. it only
    /// allows access to lookup fields
    /// </summary>
    protected bool lookupAccessMode;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException">Context is null</exception>
    public ListRequestHandler(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        StateBag = new Dictionary<string, object>();
        behaviors = new Lazy<IListBehavior[]>(() => GetBehaviors().ToArray());
    }

    /// <summary>
    /// Gets the list of list behaviors
    /// </summary>
    protected virtual IEnumerable<IListBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, IListBehavior>(GetType());
    }

    /// <summary>
    /// Gets the native sort order, which includes name field by default,
    /// unless the row has [SortOrder] attributes.
    /// </summary>
    /// <returns></returns>
    protected virtual SortBy[] GetNativeSort()
    {
        var sortOrders = Row.GetFields().SortOrders;
        if (!sortOrders.IsEmptyOrNull())
            return sortOrders.Select(x => new SortBy(x.Item1.PropertyName ?? x.Item1.Name, x.Item2)).ToArray();

        var nameField = Row.NameField;
        if (nameField is not null)
            return new SortBy[] { new SortBy(nameField.Name, false) };

        return null;
    }

    /// <summary>
    /// Returns true if the field should be allowed to be selected,
    /// based on is read permission, selectlevel.never flag, and lookup
    /// access mode
    /// </summary>
    /// <param name="field"></param>
    protected virtual bool AllowSelectField(Field field)
    {
        if (field.MinSelectLevel == SelectLevel.Never)
            return false;

        if (field.ReadPermission != null &&
            !Permissions.HasPermission(field.ReadPermission))
            return false;

        if (field.ReadPermission == null &&
            lookupAccessMode &&
            !field.IsLookup)
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
        if (DistinctFields != null)
            return DistinctFields.Contains(field);

        var mode = field.MinSelectLevel;

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
                // so we return here Explicit so that they only populate them if such 
                // fields are explicitly included (e.g. related column is visible)
                mode = SelectLevel.Explicit;
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
            ColumnSelection.List => mode <= SelectLevel.List,
            ColumnSelection.KeyOnly => ReferenceEquals(field, Row.IdField) ||
                (field.Flags & FieldFlags.PrimaryKey) == FieldFlags.PrimaryKey,
            ColumnSelection.Details => mode <= SelectLevel.Details,
            ColumnSelection.None => false,
            ColumnSelection.IdOnly => ReferenceEquals(field, Row.IdField),
            ColumnSelection.Lookup => mode <= SelectLevel.Lookup,
            _ => false
        };
    }

    /// <summary>
    /// Returns true if field is included in <see cref="ListRequest.IncludeColumns"/>
    /// </summary>
    /// <param name="field">Field</param>
    protected bool IsIncluded(Field field)
    {
        return Request.IncludeColumns != null &&
            (Request.IncludeColumns.Contains(field.Name) ||
             (field.PropertyName != null && Request.IncludeColumns.Contains(field.PropertyName)));
    }

    /// <summary>
    /// Returns true if field is included in <see cref="ListRequest.IncludeColumns"/>
    /// </summary>
    /// <param name="column">Column</param>
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
    /// Prepares query by calling <see cref="SelectFields(SqlQuery)"/>.
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void PrepareQuery(SqlQuery query)
    {
        SelectFields(query);

        foreach (var behavior in behaviors.Value)
            behavior.OnPrepareQuery(this, query);
    }

    /// <summary>
    /// Applies the key order which is ID field (<see cref="IIdRow"/>)
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void ApplyKeyOrder(SqlQuery query)
    {
        if (Row is IIdRow idRow)
            query.OrderBy(idRow.IdField);
    }

    /// <summary>
    /// Gets the list of quick search fields (<see cref="QuickSearchAttribute"/>) based
    /// on the containsField argument.
    /// </summary>
    /// <param name="containsField">Contains field argument, can be null</param>
    /// <exception cref="ArgumentOutOfRangeException">The containsField has <see cref="SelectLevel.Never" />
    /// or it does not have a <see cref="QuickSearchAttribute"/>
    /// </exception>
    protected virtual IEnumerable<Field> GetQuickSearchFields(string containsField)
    {
        if (containsField == null)
        {
            var fields = Row.GetFields().Where(x =>
            {
                var attr = x.CustomAttributes.OfType<QuickSearchAttribute>()
                    .FirstOrDefault();

                if (attr == null || attr.IsExplicit)
                    return false;

                return true;
            });

            if (!fields.Any())
            {
                var nameField = Row.NameField;
                if (nameField is not null)
                    return new Field[] { nameField };
            }

            return fields;
        }

        var field = Row.FindField(containsField) ?? Row.FindFieldByPropertyName(containsField);
        if (field is null ||
            ((field.MinSelectLevel == SelectLevel.Never) &&
                (!field.CustomAttributes.OfType<QuickSearchAttribute>().Any())))
        {
            throw new ArgumentOutOfRangeException("containsField");
        }

        return new Field[] { field };
    }

    /// <summary>
    /// Applies a contains criteria based on containsText, id (if the contains text can be parsed to 
    /// an integer ID), searchType and numericOnly parameters.
    /// </summary>
    /// <param name="field">Search field</param>
    /// <param name="containsText">Contains text</param>
    /// <param name="id">The containsText argument parsed to a long if possible</param>
    /// <param name="searchType">Search type</param>
    /// <param name="numericOnly">True if numeric, e.g. equality to the id is requested</param>
    /// <param name="criteria">Current contains criteria build up from other fields if any.
    /// The result should be returned via this argument.</param>
    /// <param name="orFalse">Should return true from this parameter if this contains criteria
    /// should cause search to return no records</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected virtual void AddFieldContainsCriteria(Field field, string containsText, long? id,
        SearchType searchType, bool numericOnly, ref BaseCriteria criteria, ref bool orFalse)
    {
        if (numericOnly == true && (id == null))
        {
            orFalse = true;
            return;
        }

        switch (searchType)
        {
            case SearchType.Contains:
                criteria |= new Criteria(field).Contains(containsText);
                break;

            case SearchType.FullTextContains:
                criteria |= new Criteria("CONTAINS(" + field.Expression + ", " + 
                    containsText.ToSql(Connection.GetDialect()) + ")");
                break;

            case SearchType.StartsWith:
                criteria |= new Criteria(field).StartsWith(containsText);
                break;

            case SearchType.Equals:
                if (field is Int32Field)
                {
                    if (id == null || id < int.MinValue || id > int.MaxValue)
                    {
                        orFalse = true;
                        return;
                    }

                    criteria |= new Criteria(field) == (int)id;
                }
                else if (field is Int16Field)
                {
                    if (id == null || id < short.MinValue || id > short.MaxValue)
                    {
                        orFalse = true;
                        return;
                    }

                    criteria |= new Criteria(field) == (short)id;
                }
                else if (field is Int64Field)
                {
                    if (id == null)
                    {
                        orFalse = true;
                        return;
                    }

                    criteria |= new Criteria(field) == id.Value;
                }
                else
                {
                    criteria |= new Criteria(field) == containsText;
                }
                break;

            default:
                throw new ArgumentOutOfRangeException("searchType");
        }
    }

    /// <summary>
    /// Applies a field contains criteria to the query by calling 
    /// <see cref="AddFieldContainsCriteria"/>
    /// </summary>
    /// <param name="field">Field</param>
    /// <param name="containsText">Contains text</param>
    /// <param name="id">containsText parsed as a long if possible</param>
    /// <param name="criteria">Current contains criteria build up from other fields if any.
    /// The result should be returned via this argument.</param>
    /// <param name="orFalse">Should return true from this parameter if this contains criteria
    /// should cause search to return no records</param>
    protected virtual void ApplyFieldContainsText(Field field, string containsText, long? id,
        ref BaseCriteria criteria, ref bool orFalse)
    {
        var attr = field.CustomAttributes.OfType<QuickSearchAttribute>().FirstOrDefault();
        var searchType = attr == null ? SearchType.Auto : attr.SearchType;
        var numericOnly = attr?.NumericOnly;

        if (numericOnly == null)
            numericOnly = field is Int32Field || field is Int16Field || field is Int64Field;

        if (searchType == SearchType.Auto)
        {
            if (field is Int32Field || field is Int16Field || field is Int64Field)
                searchType = SearchType.Equals;
            else
                searchType = SearchType.Contains;
        }

        AddFieldContainsCriteria(field, containsText, id, searchType, numericOnly ?? false,
            ref criteria, ref orFalse);
    }

    /// <summary>
    /// Applies contains text filter to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="containsText">Contains text</param>
    /// <exception cref="ArgumentOutOfRangeException">There are no quick search fields
    /// (<see cref="QuickSearchAttribute"/></exception>
    protected virtual void ApplyContainsText(SqlQuery query, string containsText)
    {
        query.ApplyContainsText(containsText, (text, id) =>
        {
            var fields = GetQuickSearchFields(Request.ContainsField.TrimToNull());
            if (fields == null || !fields.Any())
                throw new ArgumentOutOfRangeException("containsField");

            var criteria = Criteria.Empty;

            bool orFalse = false;

            foreach (var field in fields)
                ApplyFieldContainsText(field, containsText, id, ref criteria, ref orFalse);

            if (orFalse && criteria.IsEmpty)
                criteria |= new Criteria("1 = 0");

            if (!criteria.IsEmpty)
                query.Where(~(criteria));
        });
    }

    /// <summary>
    /// Called before executing the list query
    /// </summary>
    protected virtual void OnBeforeExecuteQuery()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeExecuteQuery(this);
    }

    /// <summary>
    /// Called after executing the list query
    /// </summary>
    protected virtual void OnAfterExecuteQuery()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnAfterExecuteQuery(this);
    }

    /// <summary>
    /// Called just before returning the response
    /// </summary>
    protected virtual void OnReturn()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnReturn(this);
    }

    /// <summary>
    /// Can be overridden in a derived class to make
    /// some changes in a returned entity just before it gets
    /// added to the Response.Entities list
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    protected virtual TRow ProcessEntity(TRow row)
    {
        return row;
    }

    /// <summary>
    /// Applies include deleted filter to the query if Request.IncludeDeleted is true
    /// </summary>
    /// <param name="query"></param>
    protected virtual void ApplyIncludeDeletedFilter(SqlQuery query)
    {
        if (!Request.IncludeDeleted)
            query.Where(ServiceQueryHelper.GetNotDeletedCriteria(Row));
    }

    /// <summary>
    /// Replaces field references, e.g. property names and field name with
    /// their corresponding expression by using the <see cref="CriteriaFieldExpressionReplacer"/>
    /// </summary>
    /// <param name="criteria">Input criteria</param>
    /// <returns>Processed criteria</returns>
    protected virtual BaseCriteria ReplaceFieldExpressions(BaseCriteria criteria)
    {
        return new CriteriaFieldExpressionReplacer(Row, Permissions, lookupAccessMode)
            .Process(criteria);
    }

    /// <summary>
    /// Applies the Request.Criteria to the query if it is not null or empty,
    /// and replaced field references with their corresponding expressions by
    /// calling <see cref="ReplaceFieldExpressions(BaseCriteria)"/>
    /// </summary>
    /// <param name="query"></param>
    protected virtual void ApplyCriteria(SqlQuery query)
    {
        if (Request.Criteria is null ||
            Request.Criteria.IsEmpty)
        {
            return;
        }

        var criteria = ReplaceFieldExpressions(Request.Criteria);

        query.Where(~(criteria));
    }

    /// <summary>
    /// Applies a field equality filter, e.g. one that is passed via Request.EqualityFilter
    /// to the query. It validates field flags like <see cref="FieldFlags.DenyFiltering"/> and
    /// <see cref="FieldFlags.NotMapped"/> and <see cref="SelectLevel.Never"/> to check 
    /// if the field is allowed to be filtered.
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="field">Field</param>
    /// <param name="value">Equality value. Can be a enumerable for multi value filtering.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected virtual void ApplyFieldEqualityFilter(SqlQuery query, Field field, object value)
    {
        if (field.MinSelectLevel == SelectLevel.Never ||
            field.Flags.HasFlag(FieldFlags.DenyFiltering) ||
            field.Flags.HasFlag(FieldFlags.NotMapped))
        {
            throw new ArgumentOutOfRangeException(field.PropertyName ?? field.Name,
                $"Can't apply equality filter on field {field.PropertyName ?? field.Name}");
        }

        if (value is not string && value is IEnumerable enumerable)
        {
            var values = new List<object>();
            foreach (var val in enumerable)
                values.Add(field.ConvertValue(val, CultureInfo.InvariantCulture));
            if (values.Count > 0)
                query.Where(field.In(values));
        }
        else
        {
            value = field.ConvertValue(value, CultureInfo.InvariantCulture);
            if (value == null)
                return;
            query.WhereEqual(field, value);
        }
    }

    /// <summary>
    /// Checks if the equality filter value is empty. It returns true
    /// for null, empty string, or empty IEnumerable.
    /// </summary>
    /// <param name="value">Value to check</param>
    protected bool IsEmptyEqualityFilterValue(object value)
    {
        if (value == null)
            return true;

        if (value is string str && str.Length == 0)
            return true;

        if (value is not string && value is IEnumerable &&
            !(value as IEnumerable).GetEnumerator().MoveNext())
            return true;

        return false;
    }

    /// <summary>
    /// Applies the Request.Equality filter to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <exception cref="ArgumentOutOfRangeException">A field name in the Request.EqualityFilter
    /// could not be matched with a field in current row</exception>
    protected virtual void ApplyEqualityFilter(SqlQuery query)
    {
        if (Request.EqualityFilter == null)
            return;

        foreach (var pair in Request.EqualityFilter)
        {
            if (ignoredEqualityFilters != null &&
                ignoredEqualityFilters.Contains(pair.Key))
                continue;

            if (IsEmptyEqualityFilterValue(pair.Value))
                continue;

            var field = Row.FindFieldByPropertyName(pair.Key) ?? Row.FindField(pair.Key);
            if (field is null)
                throw new ArgumentOutOfRangeException(pair.Key,
                    string.Format("Can't find field {0} in row for equality filter.", pair.Key));

            ApplyFieldEqualityFilter(query, field, pair.Value);
        }
    }

    /// <summary>
    /// Adds the field to the list of ignored equality filters, so that
    /// having this field in the Request.EqualityFilter won't raise an exception
    /// </summary>
    /// <param name="field"></param>
    public void IgnoreEqualityFilter(string field)
    {
        ignoredEqualityFilters ??= new HashSet<string>();
        ignoredEqualityFilters.Add(field);
    }

    /// <summary>
    /// Applies all the filters including Request.EqualityFilter, Request.Criteria and
    /// Request.IncludeDeleted to the query.
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void ApplyFilters(SqlQuery query)
    {
        ApplyEqualityFilter(query);
        ApplyCriteria(query);
        ApplyIncludeDeletedFilter(query);

        foreach (var behavior in behaviors.Value)
            behavior.OnApplyFilters(this, query);
    }

    /// <summary>
    /// Validates if the user is allowed to query this entity type by checking <see cref="ReadPermissionAttribute"/>
    /// and <see cref="ServiceLookupPermissionAttribute"/> if the request is in lookup access mode.
    /// </summary>
    protected virtual void ValidatePermissions()
    {
        var readAttr = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);
        if (readAttr != null)
        {
            var permission = readAttr.Permission ?? "?";
            if (!Permissions.HasPermission(permission))
            {
                var lookupPermission = typeof(TRow).GetCustomAttribute<ServiceLookupPermissionAttribute>()?.Permission;
                if (!string.IsNullOrEmpty(lookupPermission) &&
                    Permissions.HasPermission(lookupPermission))
                {
                    lookupAccessMode = true;
                    return;
                }

                Permissions.ValidatePermission(permission, Localizer);
            }
        }
    }

    /// <summary>
    /// Validates the request by calling <see cref="ValidatePermissions"/>
    /// </summary>
    protected virtual void ValidateRequest()
    {
        ValidatePermissions();

        foreach (var behavior in behaviors.Value)
            behavior.OnValidateRequest(this);
    }

    /// <summary>
    /// Creates a query instance with the dialect for current connection.
    /// </summary>
    protected virtual SqlQuery CreateQuery()
    {
        return new SqlQuery()
            .Dialect(Connection.GetDialect())
            .From(Row);
    }

    /// <summary>
    /// Applies a sort order to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="sortBy">Sort order</param>
    protected virtual void ApplySortBy(SqlQuery query, SortBy sortBy)
    {
        var field = Row.FindField(sortBy.Field) ??
            Row.FindFieldByPropertyName(sortBy.Field);

        if (field is not null)
        {
            if (field.Flags.HasFlag(FieldFlags.NotMapped))
                return;

            var sortable = field.GetAttribute<SortableAttribute>();
            if (sortable != null && !sortable.Value)
                return;
        }

        query.ApplySort(sortBy.Field, sortBy.Descending);
    }

    /// <summary>
    /// Applies the Request.Sort order to the query. Sorts by <see cref="GetNativeSort"/>
    /// if no sort columns are passed, or the list is empty.
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void ApplySort(SqlQuery query)
    {
        var sortByList = Request.Sort;

        if (sortByList == null || sortByList.Length == 0)
            sortByList = GetNativeSort();

        if (sortByList != null)
            for (var i = sortByList.Length - 1; i >= 0; i--)
            {
                var sortBy = sortByList[i];
                ApplySortBy(query, sortBy);
            }
    }

    /// <summary>
    /// Gets an array of distinct fields by checking Request.DistinctFields. It might be 
    /// different than the passed list, if the list contains an invalid field or a field
    /// that is not allowed to be selected, like <see cref="FieldFlags.NotMapped"/> and
    /// <see cref="SelectLevel.Never"/> etc.
    /// </summary>
    public Field[] GetDistinctFields()
    {
        if (!Request.DistinctFields.IsEmptyOrNull())
        {
            Query.Distinct(true);
            Query.ApplySort(Request.DistinctFields);

            var result = Request.DistinctFields.Select(x =>
            {
                var field = Row.FindFieldByPropertyName(x.Field) ??
                    Row.FindField(x.Field);

                if (field is null ||
                    (field.Flags & FieldFlags.NotMapped) == FieldFlags.NotMapped ||
                    !AllowSelectField(field))
                    return null;

                return field;
            }).ToArray();

            // if any of fields are invalid, return an empty array to avoid errors
            if (result.Any(x => x is null))
                return new Field[0];

            return result;
        }

        return null;
    }

    /// <summary>
    /// Processes the list request. This is the entry point for the handler.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException">connection or the request is null</exception>
    public TListResponse Process(IDbConnection connection, TListRequest request)
    {
        StateBag.Clear();
        lookupAccessMode = false;
        ignoredEqualityFilters = null;
        Connection = connection ?? throw new ArgumentNullException("connection");
        Request = request ?? throw new ArgumentNullException(nameof(request));
        ValidateRequest();

        Response = new TListResponse
        {
            Entities = new List<TRow>()
        };

        Row = new TRow();

        var query = CreateQuery();
        Query = query;

        DistinctFields = GetDistinctFields();
        if (DistinctFields != null)
            Response.Values = new List<object>();

        PrepareQuery(query);

        if (DistinctFields == null)
            ApplyKeyOrder(query);

        query.ApplySkipTakeAndCount(request.Skip, request.Take,
            request.ExcludeTotalCount || DistinctFields != null);

        ApplyContainsText(query, request.ContainsText);

        if (DistinctFields == null)
            ApplySort(query);

        ApplyFilters(query);

        OnBeforeExecuteQuery();

        if (DistinctFields == null || DistinctFields.Length > 0)
        {
            Response.TotalCount = query.ForEach(Connection, delegate ()
            {
                var clone = ProcessEntity(Row.Clone());

                if (clone != null)
                {
                    if (DistinctFields != null)
                    {
                        foreach (var field in DistinctFields)
                            Response.Values.Add(field.AsObject(clone));
                    }
                    else
                        Response.Entities.Add(clone);
                }
            });
        }
        else
        {
            // mark response to specify that one or more fields are invalid
            Response.Values = null;
        }

        Response.SetSkipTakeTotal(query);

        OnAfterExecuteQuery();

        OnReturn();

        return Response;
    }
    
    IListResponse IListRequestProcessor.Process(IDbConnection connection, ListRequest request)
    {
        return Process(connection, (TListRequest)request);
    }

    /// <inheritdoc/>
    public TListResponse List(IDbConnection connection, TListRequest request)
    {
        return Process(connection, request);
    }

    /// <summary>
    /// Gets the two level cache from the request context
    /// </summary>
    public ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context
    /// </summary>
    public IRequestContext Context { get; private set; }

    /// <summary>
    /// Gets localizer from the request context
    /// </summary>
    public ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets permission service from the request context
    /// </summary>
    public IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets current user from the request context
    /// </summary>
    public ClaimsPrincipal User => Context.User;

    /// <summary>
    /// Gets list of distinct fields
    /// </summary>
    public Field[] DistinctFields { get; private set; }

    /// <summary>
    /// Gets current connection
    /// </summary>
    public IDbConnection Connection { get; private set; }

    /// <summary>
    /// Gets the select query
    /// </summary>
    public SqlQuery Query { get; private set; }

    /// <summary>
    /// The entity used for querying / metadata lookup
    /// </summary>
    public TRow Row { get; protected set; }

    /// <summary>
    /// Response object
    /// </summary>
    public TListRequest Request { get; protected set; }

    /// <summary>
    /// Response object
    /// </summary>
    public TListResponse Response { get; protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object> StateBag { get; private set; }

    IRow IListRequestHandler.Row => Row;
    ListRequest IListRequestHandler.Request => Request;
    IListResponse IListRequestHandler.Response => Response;
    bool IListRequestHandler.AllowSelectField(Field field) { return AllowSelectField(field); }
    bool IListRequestHandler.ShouldSelectField(Field field) { return ShouldSelectField(field); }
}