namespace Serenity.Services
{
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public class ListRequestHandler<TRow, TListRequest, TListResponse>
        where TRow: Row, new()
        where TListRequest: ListRequest
        where TListResponse: ListResponse<TRow>, new()
    {
        protected IDbConnection Connection;
        protected TRow Row;
        protected TListRequest Request;
        protected TListResponse Response;

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

        protected virtual bool ShouldSelectField(Field field)
        {
            var mode = field.MinSelectLevel;
            
            if (mode == SelectLevel.Never ||
                (field.Flags & FieldFlags.ClientSide) == FieldFlags.ClientSide)
                return false;

            if (mode == SelectLevel.Always)
                return true;

            bool isPrimaryKey = (field.Flags & FieldFlags.PrimaryKey) == FieldFlags.PrimaryKey;
            if (isPrimaryKey && mode != SelectLevel.Explicit)
                return true;

            if (mode == SelectLevel.Default)
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
                if (ShouldSelectField(field))
                    SelectField(query, field);
            }
        }
    
        protected virtual void PrepareQuery(SqlQuery query)
        {
            SelectFields(query);
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

                    if (numericOnly == true && (id == null))
                    {
                        orFalse = true;
                        continue;
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
                                    continue;
                                }

                                criteria |= new Criteria(field) == (int)id;
                            }
                            else if (field is Int16Field)
                            {
                                if (id == null || id < Int16.MinValue || id > Int16.MaxValue)
                                {
                                    orFalse = true;
                                    continue;
                                }

                                criteria |= new Criteria(field) == (Int16)id;
                            }
                            else if (field is Int64Field)
                            {
                                if (id == null || id < Int64.MinValue || id > Int64.MaxValue)
                                {
                                    orFalse = true;
                                    continue;
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

                if (orFalse && criteria.IsEmpty)
                    criteria |= new Criteria("1 = 0");

                if (!criteria.IsEmpty)
                    query.Where(~(criteria));
            });
        }

        protected virtual void OnBeforeExecuteQuery()
        {
        }

        protected virtual void OnAfterExecuteQuery()
        {
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

            query.Where(criteria);
        }

        protected virtual void ApplyEqualityFilter(SqlQuery query)
        {
            if (Request.EqualityFilter != null)
            {
                foreach (var pair in Request.EqualityFilter)
                {
                    if (pair.Value == null)
                        continue;

                    if (pair.Value is string && ((string)pair.Value).Length == 0)
                        continue;

                    var field = Row.FindFieldByPropertyName(pair.Key) ?? Row.FindField(pair.Key);
                    if (!ReferenceEquals(null, field))
                    {
                        if (field.MinSelectLevel == SelectLevel.Never ||
                            field.Flags.HasFlag(FieldFlags.DenyFiltering))
                        {
                            throw new ArgumentOutOfRangeException("equalityFilter");
                        }

                        var value = field.ConvertValue(pair.Value, CultureInfo.InvariantCulture);
                        if (value == null)
                            continue;

                        query.WhereEqual(field, value);
                    }
                    else
                        throw new ArgumentOutOfRangeException(String.Format("Can't find field {0} in row for equality filter.", pair.Key));
                }
            }
        }

        protected virtual void ApplyFilters(SqlQuery query)
        {
            ApplyEqualityFilter(query);
            ApplyCriteria(query);
            ApplyIncludeDeletedFilter(query);
        }

        protected virtual void ValidatePermissions()
        {
            var readAttr = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(false);
            if (readAttr != null)
            {
                if (readAttr.Permission.IsNullOrEmpty())
                    Authorization.ValidateLoggedIn();
                else
                    Authorization.ValidatePermission(readAttr.Permission);
            }
        }

        protected virtual SqlQuery CreateQuery()
        {
            return new SqlQuery()
                .From(Row);
        }

        protected virtual void ApplySortBy(SqlQuery query, SortBy sortBy)
        {
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
            if (connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;
            Request = request;
            ValidatePermissions();

            Response = new TListResponse();
            Response.Entities = new List<TRow>();

            Row = new TRow();

            var query = CreateQuery();

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

            return Response;
        }
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
}