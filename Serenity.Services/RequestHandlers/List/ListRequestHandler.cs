namespace Serenity.Services
{
    using ComponentModel;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public class ListRequestHandler<TRow, TListRequest, TListResponse> : IListRequestHandler, IListRequestProcessor
        where TRow: Row, new()
        where TListRequest: ListRequest
        where TListResponse: ListResponse<TRow>, new()
    {
        protected TRow Row;
        protected TListRequest Request;
        protected TListResponse Response;

        protected static IEnumerable<IListBehavior> cachedBehaviors;
        protected IEnumerable<IListBehavior> behaviors;
        protected HashSet<string> ignoredEqualityFilters;

        public ListRequestHandler()
        {
            this.StateBag = new Dictionary<string, object>();
            this.behaviors = GetBehaviors();
        }

        protected virtual IEnumerable<IListBehavior> GetBehaviors()
        {
            if (cachedBehaviors == null)
            {
                cachedBehaviors = RowListBehaviors<TRow>.Default.Concat(
                    this.GetType().GetCustomAttributes().OfType<IListBehavior>()).ToList();
            }

            return cachedBehaviors;
        }

        protected virtual SortBy[] GetNativeSort()
        {
            var sortOrders = Row.GetFields().SortOrders;
            if (!sortOrders.IsEmptyOrNull())
                return sortOrders.Select(x => new SortBy(x.Item1.PropertyName ?? x.Item1.Name, x.Item2)).ToArray();

            var nameRow = Row as INameRow;
            if (nameRow != null)
                return new SortBy[] { new SortBy(nameRow.NameField.Name, false) };

            return null;
        }

        protected virtual bool AllowSelectField(Field field)
        {
            if (field.MinSelectLevel == SelectLevel.Never)
                return false;

            if (field.ReadPermission != null &&
                !Authorization.HasPermission(field.ReadPermission))
                return false;

            return true;
        }

        protected virtual bool ShouldSelectField(Field field)
        {
            var mode = field.MinSelectLevel;
            
            if (mode == SelectLevel.Always)
                return true;

            bool isPrimaryKey = (field.Flags & FieldFlags.PrimaryKey) == FieldFlags.PrimaryKey;
            if (isPrimaryKey && mode != SelectLevel.Explicit)
                return true;

            if (mode == SelectLevel.Auto)
            {
                // assume that non-foreign calculated and reflective fields should be selected in list mode
                bool isForeign = (field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign;
                mode = isForeign ? SelectLevel.Details : SelectLevel.List;
            }

            bool explicitlyExcluded = Request.ExcludeColumns != null &&
                (Request.ExcludeColumns.Contains(field.Name) ||
                    (field.PropertyName != null && Request.ExcludeColumns.Contains(field.PropertyName)));

            bool explicitlyIncluded = !explicitlyExcluded && Request.IncludeColumns != null &&
                (Request.IncludeColumns.Contains(field.Name) ||
                    (field.PropertyName != null && Request.IncludeColumns.Contains(field.PropertyName)));

            if (isPrimaryKey)
                return explicitlyIncluded;

            if (explicitlyExcluded)
                return false;

            if (explicitlyIncluded)
                return true;

            var selection = Request.ColumnSelection;

            switch (selection)
            {
                case ColumnSelection.List:
                    return mode <= SelectLevel.List;
                case ColumnSelection.Details:
                    return mode <= SelectLevel.Details;
                default:
                    return false;
            }
        }

        protected bool IsIncluded(Field field)
        {
            return Request.IncludeColumns != null &&
                (Request.IncludeColumns.Contains(field.Name) ||
                 (field.PropertyName != null && Request.IncludeColumns.Contains(field.PropertyName)));
        }

        protected bool IsIncluded(string column)
        {
            return Request.IncludeColumns != null &&
                Request.IncludeColumns.Contains(column);
        }

        protected virtual void SelectField(SqlQuery query, Field field)
        {
            query.Select(field);
        }

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
    
        protected virtual void PrepareQuery(SqlQuery query)
        {
            SelectFields(query);

            foreach (var behavior in behaviors)
                behavior.OnPrepareQuery(this, query);
        }

        protected virtual void ApplyKeyOrder(SqlQuery query)
        {
            var idRow = Row as IIdRow;
            if (idRow != null)
                query.OrderBy((Field)idRow.IdField);
        }

        protected virtual IEnumerable<Field> GetQuickSearchFields(string containsField)
        {
            if (containsField == null)
            {
                var fields = Row.GetFields().Where(x =>
                {
                    if (x.CustomAttributes == null)
                        return false;

                    var attr = x.CustomAttributes.OfType<QuickSearchAttribute>()
                        .FirstOrDefault();

                    if (attr == null || attr.IsExplicit)
                        return false;

                    return true;
                });

                if (!fields.Any() && Row is INameRow)
                    return new Field[] { ((INameRow)Row).NameField };

                return fields;
            }

            var field = Row.FindField(containsField) ?? Row.FindFieldByPropertyName(containsField);
            if (ReferenceEquals(null, field) ||
                ((field.MinSelectLevel == SelectLevel.Never) && 
                    (field.CustomAttributes == null || 
                     !field.CustomAttributes.OfType<QuickSearchAttribute>().Any())))
            {
                throw new ArgumentOutOfRangeException("containsField");
            }

            return new Field[] { field };
        }

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

                case SearchType.StartsWith:
                    criteria |= new Criteria(field).StartsWith(containsText);
                    break;

                case SearchType.Equals:
                    if (field is Int32Field)
                    {
                        if (id == null || id < Int32.MinValue || id > Int32.MaxValue)
                        {
                            orFalse = true;
                            return;
                        }

                        criteria |= new Criteria(field) == (int)id;
                    }
                    else if (field is Int16Field)
                    {
                        if (id == null || id < Int16.MinValue || id > Int16.MaxValue)
                        {
                            orFalse = true;
                            return;
                        }

                        criteria |= new Criteria(field) == (Int16)id;
                    }
                    else if (field is Int64Field)
                    {
                        if (id == null || id < Int64.MinValue || id > Int64.MaxValue)
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

        protected virtual void ApplyFieldContainsText(Field field, string containsText, long? id,
            ref BaseCriteria criteria, ref bool orFalse)
        {
            var attr = field.CustomAttributes == null ? null : field.CustomAttributes.OfType<QuickSearchAttribute>().FirstOrDefault();
            var searchType = attr == null ? SearchType.Auto : attr.SearchType;
            var numericOnly = attr == null ? null : attr.NumericOnly;

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

        protected virtual void OnBeforeExecuteQuery()
        {
            foreach (var behavior in behaviors)
                behavior.OnBeforeExecuteQuery(this);
        }

        protected virtual void OnAfterExecuteQuery()
        {
            foreach (var behavior in behaviors)
                behavior.OnAfterExecuteQuery(this);
        }

        protected virtual void OnReturn()
        {
            foreach (var behavior in behaviors)
                behavior.OnReturn(this);
        }

        protected virtual TRow ProcessEntity(TRow row)
        {
            return row;
        }

        protected virtual void ApplyIncludeDeletedFilter(SqlQuery query)
        {
            if (!Request.IncludeDeleted)
            {
                var isDeletedRow = Row as IIsActiveDeletedRow;
                if (isDeletedRow != null)
                    query.Where(new Criteria(isDeletedRow.IsActiveField) >= 0);
                else
                {
                    var deleteLogRow = Row as IDeleteLogRow;
                    if (deleteLogRow != null)
                        query.Where(new Criteria((Field)deleteLogRow.DeleteUserIdField).IsNull());
                }
            }
        }

        protected virtual BaseCriteria ReplaceFieldExpressions(BaseCriteria criteria)
        {
            return new CriteriaFieldExpressionReplacer(this.Row)
                .Process(criteria);
        }

        protected virtual void ApplyCriteria(SqlQuery query)
        {
            if (Object.ReferenceEquals(null, Request.Criteria) ||
                Request.Criteria.IsEmpty)
            {
                return;
            }

            var criteria = ReplaceFieldExpressions(Request.Criteria);

            query.Where(~(criteria));
        }

        protected virtual void ApplyFieldEqualityFilter(SqlQuery query, Field field, object value)
        {
            if (field.MinSelectLevel == SelectLevel.Never ||
                field.Flags.HasFlag(FieldFlags.DenyFiltering) ||
                field.Flags.HasFlag(FieldFlags.NotMapped))
            {
                throw new ArgumentOutOfRangeException(field.PropertyName ?? field.Name, 
                    String.Format("Can't apply equality filter on field {0}", field.PropertyName ?? field.Name));
            }

            if (!(value is string) && value is IEnumerable)
            {
                var values = new List<object>();
                foreach (var val in (IEnumerable)value)
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

        protected bool IsEmptyEqualityFilterValue(object value)
        {
            if (value == null)
                return true;

            if (value is string && ((string)value).Length == 0)
                return true;

            if (!(value is string) && value is IEnumerable && 
                !((value as IEnumerable).GetEnumerator().MoveNext()))
                return true;

            return false;
        }

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
                if (ReferenceEquals(null, field))
                    throw new ArgumentOutOfRangeException(pair.Key, 
                        String.Format("Can't find field {0} in row for equality filter.", pair.Key));

                ApplyFieldEqualityFilter(query, field, pair.Value);
            }
        }

        protected virtual void ApplyFilters(SqlQuery query)
        {
            ApplyEqualityFilter(query);
            ApplyCriteria(query);
            ApplyIncludeDeletedFilter(query);

            foreach (var behavior in behaviors)
                behavior.OnApplyFilters(this, query);
        }

        protected virtual void ValidatePermissions()
        {
            var readAttr = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(false);
            if (readAttr != null)
                Authorization.ValidatePermission(readAttr.Permission ?? "?");
        }

        protected virtual void ValidateRequest()
        {
            ValidatePermissions();

            foreach (var behavior in behaviors)
                behavior.OnValidateRequest(this);
        }

        protected virtual SqlQuery CreateQuery()
        {
            return new SqlQuery()
                .Dialect(Connection.GetDialect())
                .From(Row);
        }

        protected virtual void ApplySortBy(SqlQuery query, SortBy sortBy)
        {
            var field = this.Row.FindField(sortBy.Field) ??
                this.Row.FindFieldByPropertyName(sortBy.Field);

            if (!ReferenceEquals(null, field))
            {
                if (field.Flags.HasFlag(FieldFlags.NotMapped))
                    return;

                var sortable = field.GetAttribute<SortableAttribute>();
                if (sortable != null && !sortable.Value)
                    return;
            }

            query.ApplySort(sortBy.Field, sortBy.Descending);
        }

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

        public TListResponse Process(IDbConnection connection, TListRequest request)
        {
            StateBag.Clear();

            if (connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;
            Request = request;
            ValidateRequest();

            Response = new TListResponse();
            Response.Entities = new List<TRow>();

            Row = new TRow();

            var query = CreateQuery();
            this.Query = query;

            PrepareQuery(query);

            ApplyKeyOrder(query);

            query.ApplySkipTakeAndCount(request.Skip, request.Take, request.ExcludeTotalCount);

            ApplyContainsText(query, request.ContainsText);

            ApplySort(query);

            ApplyFilters(query);

            OnBeforeExecuteQuery();

            Response.TotalCount = query.ForEach(Connection, delegate()
            {
                var clone = ProcessEntity(Row.Clone());

                if (clone != null)
                    Response.Entities.Add(clone);
            });

            Response.SetSkipTakeTotal(query);

            OnAfterExecuteQuery();

            OnReturn();

            return Response;
        }

        IListResponse IListRequestProcessor.Process(IDbConnection connection, ListRequest request)
        {
            return Process(connection, (TListRequest)request);
        }

        public void IgnoreEqualityFilter(string field)
        {
            if (ignoredEqualityFilters == null)
                ignoredEqualityFilters = new HashSet<string>();

            ignoredEqualityFilters.Add(field);
        }

        public IDbConnection Connection { get; private set; }
        Row IListRequestHandler.Row { get { return this.Row; } }
        public SqlQuery Query { get; private set; }
        ListRequest IListRequestHandler.Request { get { return this.Request; } }
        IListResponse IListRequestHandler.Response { get { return this.Response; } }
        public IDictionary<string, object> StateBag { get; private set; }
        bool IListRequestHandler.AllowSelectField(Field field) { return AllowSelectField(field); }
        bool IListRequestHandler.ShouldSelectField(Field field) { return ShouldSelectField(field); }
    }

    public class ListRequestHandler<TRow> : ListRequestHandler<TRow, ListRequest, ListResponse<TRow>>
        where TRow : Row, new()
    {
    }

    public class ListRequestHandler<TRow, TListRequest> : ListRequestHandler<TRow, TListRequest, ListResponse<TRow>>
        where TRow : Row, new()
        where TListRequest: ListRequest
    {
    }

    public interface IListRequestProcessor
    {
        IListResponse Process(IDbConnection connection, ListRequest request);
    }
}