namespace Serenity.Services
{
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    public class RetrieveRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse> : IRetrieveRequestHandler
        where TRow: Row, new()
        where TRetrieveRequest: RetrieveRequest
        where TRetrieveResponse: RetrieveResponse<TRow>, new()
    {
        protected TRow Row;
        protected TRetrieveResponse Response;
        protected TRetrieveRequest Request;

        protected static IEnumerable<IRetrieveBehavior> cachedBehaviors;
        protected IEnumerable<IRetrieveBehavior> behaviors;

        public RetrieveRequestHandler()
        {
            this.StateBag = new Dictionary<string, object>();
            this.behaviors = GetBehaviors();
        }

        protected virtual IEnumerable<IRetrieveBehavior> GetBehaviors()
        {
            if (cachedBehaviors == null)
            {
                cachedBehaviors = RowRetrieveBehaviors<TRow>.Default.Concat(
                    this.GetType().GetCustomAttributes().OfType<IRetrieveBehavior>()).ToList();
            }

            return cachedBehaviors;
        }

        protected virtual bool CanSelectField(Field field)
        {
            if ((field.Flags & FieldFlags.ClientSide) == FieldFlags.ClientSide)
                return false;

            return true;
        }

        protected virtual bool ShouldSelectField(Field field)
        {
            var mode = field.MinSelectLevel;

            if (field.MinSelectLevel == SelectLevel.Never)
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
                case RetrieveColumnSelection.List:
                    return mode <= SelectLevel.List;
                case RetrieveColumnSelection.Details:
                    return mode <= SelectLevel.Details;
                default:
                    return false;
            }
        }

        protected virtual void SelectField(SqlQuery query, Field field)
        {
            query.Select(field);
        }

        protected virtual void SelectFields(SqlQuery query)
        {
            foreach (var field in Row.GetFields())
            {
                if (CanSelectField(field) && ShouldSelectField(field))
                    SelectField(query, field);
            }
        }

        protected virtual void OnReturn()
        {
            foreach (var behavior in behaviors)
                behavior.OnReturn(this);
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            SelectFields(query);

            foreach (var behavior in behaviors)
                behavior.OnPrepareQuery(this, query);
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

        protected virtual void ValidateRequest()
        {
            ValidatePermissions();

            foreach (var behavior in behaviors)
                behavior.OnValidateRequest(this);
        }

        protected virtual SqlQuery CreateQuery()
        {
            var query = new SqlQuery()
                .From(Row);

            query.WhereEqual((Field)(((IIdRow)Row).IdField), Request.EntityId.Value);

            return query;
        }

        public TRetrieveResponse Process(IDbConnection connection, TRetrieveRequest request)
        {
            StateBag.Clear();

            if (connection == null)
                throw new ArgumentNullException("connection");

            request.CheckNotNull();
            if (request.EntityId == null)
                throw DataValidation.RequiredError("entityId");

            Connection = connection;
            Request = request;
            ValidateRequest();

            Response = new TRetrieveResponse();
            Row = new TRow();
           
            this.Query = CreateQuery();

            PrepareQuery(Query);

            OnBeforeExecuteQuery();

            if (Query.GetFirst(Connection))
                Response.Entity = Row;
            else
                throw DataValidation.EntityNotFoundError(Row, request.EntityId.Value);

            OnAfterExecuteQuery();

            OnReturn();
            return Response;
        }

        public IDbConnection Connection { get; private set; }
        Row IRetrieveRequestHandler.Row { get { return this.Row; } }
        public SqlQuery Query { get; private set; }
        RetrieveRequest IRetrieveRequestHandler.Request { get { return this.Request; } }
        IRetrieveResponse IRetrieveRequestHandler.Response { get { return this.Response; } }
        bool IRetrieveRequestHandler.ShouldSelectField(Field field) { return ShouldSelectField(field); }
        public IDictionary<string, object> StateBag { get; private set; }
    }

    public class RetrieveRequestHandler<TRow> : RetrieveRequestHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>
        where TRow : Row, new()
    {
    }
}