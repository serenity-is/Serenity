using Serenity.ComponentModel;
using Serenity.Abstractions;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace Serenity.Services
{
    public class ListRequestHandler<TRow, TListRequest, TListResponse> : IListRequestProcessor,
        IListHandler<TRow, TListRequest, TListResponse>
        where TRow : class, IRow, new()
        where TListRequest : ListRequest
        where TListResponse : ListResponse<TRow>, new()
    {
        protected TRow Row;
        protected TListRequest Request;
        protected TListResponse Response;

        protected HashSet<string> ignoredEqualityFilters;
        protected Lazy<IListBehavior[]> behaviors;
        protected bool lookupAccessMode;

        public ListRequestHandler(IRequestContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            StateBag = new Dictionary<string, object>();
            behaviors = new Lazy<IListBehavior[]>(() => GetBehaviors().ToArray());
        }

        protected virtual IEnumerable<IListBehavior> GetBehaviors()
        {
            return Context.Behaviors.Resolve<TRow, IListBehavior>(GetType());
        }

        protected virtual SortBy[] GetNativeSort()
        {
            var sortOrders = Row.GetFields().SortOrders;
            if (!sortOrders.IsEmptyOrNull())
                return sortOrders.Select(x => new SortBy(x.Item1.PropertyName ?? x.Item1.Name, x.Item2)).ToArray();

            var nameField = Row.NameField;
            if (nameField is object)
                return new SortBy[] { new SortBy(nameField.Name, false) };

            return null;
        }

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

            foreach (var behavior in behaviors.Value)
                behavior.OnPrepareQuery(this, query);
        }

        protected virtual void ApplyKeyOrder(SqlQuery query)
        {
            if (Row is IIdRow idRow)
                query.OrderBy(idRow.IdField);
        }

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
                    if (nameField is object)
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
                        if (id == null || id < long.MinValue || id > long.MaxValue)
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
            foreach (var behavior in behaviors.Value)
                behavior.OnBeforeExecuteQuery(this);
        }

        protected virtual void OnAfterExecuteQuery()
        {
            foreach (var behavior in behaviors.Value)
                behavior.OnAfterExecuteQuery(this);
        }

        protected virtual void OnReturn()
        {
            foreach (var behavior in behaviors.Value)
                behavior.OnReturn(this);
        }

        protected virtual TRow ProcessEntity(TRow row)
        {
            return row;
        }

        protected virtual void ApplyIncludeDeletedFilter(SqlQuery query)
        {
            if (!Request.IncludeDeleted)
                query.Where(ServiceQueryHelper.GetNotDeletedCriteria(Row));
        }

        protected virtual BaseCriteria ReplaceFieldExpressions(BaseCriteria criteria)
        {
            return new CriteriaFieldExpressionReplacer(Row, Permissions, lookupAccessMode)
                .Process(criteria);
        }

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

        protected virtual void ApplyFieldEqualityFilter(SqlQuery query, Field field, object value)
        {
            if (field.MinSelectLevel == SelectLevel.Never ||
                field.Flags.HasFlag(FieldFlags.DenyFiltering) ||
                field.Flags.HasFlag(FieldFlags.NotMapped))
            {
                throw new ArgumentOutOfRangeException(field.PropertyName ?? field.Name,
                    $"Can't apply equality filter on field {field.PropertyName ?? field.Name}");
            }

            if (!(value is string) && value is IEnumerable enumerable)
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

        protected bool IsEmptyEqualityFilterValue(object value)
        {
            if (value == null)
                return true;

            if (value is string str && str.Length == 0)
                return true;

            if (!(value is string) && value is IEnumerable &&
                !(value as IEnumerable).GetEnumerator().MoveNext())
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
                if (field is null)
                    throw new ArgumentOutOfRangeException(pair.Key,
                        string.Format("Can't find field {0} in row for equality filter.", pair.Key));

                ApplyFieldEqualityFilter(query, field, pair.Value);
            }
        }

        protected virtual void ApplyFilters(SqlQuery query)
        {
            ApplyEqualityFilter(query);
            ApplyCriteria(query);
            ApplyIncludeDeletedFilter(query);

            foreach (var behavior in behaviors.Value)
                behavior.OnApplyFilters(this, query);
        }

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

        protected virtual void ValidateRequest()
        {
            ValidatePermissions();

            foreach (var behavior in behaviors.Value)
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
            var field = Row.FindField(sortBy.Field) ??
                Row.FindFieldByPropertyName(sortBy.Field);

            if (field is object)
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

        public TListResponse Process(IDbConnection connection, TListRequest request)
        {
            StateBag.Clear();
            lookupAccessMode = false;
            ignoredEqualityFilters = null;
            Connection = connection ?? throw new ArgumentNullException("connection");
            Request = request;
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

        public void IgnoreEqualityFilter(string field)
        {
            if (ignoredEqualityFilters == null)
                ignoredEqualityFilters = new HashSet<string>();

            ignoredEqualityFilters.Add(field);
        }

        public ITwoLevelCache Cache => Context.Cache;
        public IRequestContext Context { get; private set; }
        public ITextLocalizer Localizer => Context.Localizer;
        public IPermissionService Permissions => Context.Permissions;
        public ClaimsPrincipal User => Context.User;

        public Field[] DistinctFields { get; private set; }
        public IDbConnection Connection { get; private set; }
        public SqlQuery Query { get; private set; }
        public IDictionary<string, object> StateBag { get; private set; }

        IRow IListRequestHandler.Row => Row;
        ListRequest IListRequestHandler.Request => Request;
        IListResponse IListRequestHandler.Response => Response;
        bool IListRequestHandler.AllowSelectField(Field field) { return AllowSelectField(field); }
        bool IListRequestHandler.ShouldSelectField(Field field) { return ShouldSelectField(field); }

        public TListResponse List(IDbConnection connection, TListRequest request)
        {
            return Process(connection, request);
        }
    }
}