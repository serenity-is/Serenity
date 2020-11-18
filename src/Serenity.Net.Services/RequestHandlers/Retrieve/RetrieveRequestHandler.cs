namespace Serenity.Services
{
    using Serenity.Abstractions;
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;

    public class RetrieveRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse> : IRetrieveRequestProcessor,
        IRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse>
        where TRow: class, IRow, new()
        where TRetrieveRequest: RetrieveRequest
        where TRetrieveResponse: RetrieveResponse<TRow>, new()
    {
        protected TRow Row;
        protected TRetrieveResponse Response;
        protected TRetrieveRequest Request;

        protected Lazy<IRetrieveBehavior[]> behaviors;

        public RetrieveRequestHandler(IRequestContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            StateBag = new Dictionary<string, object>();
            behaviors = new Lazy<IRetrieveBehavior[]>(() => GetBehaviors().ToArray());
        }

        protected virtual IEnumerable<IRetrieveBehavior> GetBehaviors()
        {
            return Context.Behaviors.Resolve<TRow, IRetrieveBehavior>(GetType());
        }

        protected virtual bool AllowSelectField(Field field)
        {
            if (field.MinSelectLevel == SelectLevel.Never)
                return false;

            if (field.ReadPermission != null &&
                !Permissions.HasPermission(field.ReadPermission))
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

            return selection switch
            {
                RetrieveColumnSelection.List => mode <= SelectLevel.List,
                RetrieveColumnSelection.Details => mode <= SelectLevel.Details,
                _ => false,
            };
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

        protected virtual void OnReturn()
        {
            foreach (var behavior in behaviors.Value)
                behavior.OnReturn(this);
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            SelectFields(query);

            foreach (var behavior in behaviors.Value)
                behavior.OnPrepareQuery(this, query);
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
            var readAttr = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);
            if (readAttr != null)
                Permissions.ValidatePermission(readAttr.Permission ?? "?", Localizer);
        }

        protected virtual void ValidateRequest()
        {
            ValidatePermissions();

            foreach (var behavior in behaviors.Value)
                behavior.OnValidateRequest(this);
        }

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

        public TRetrieveResponse Process(IDbConnection connection, TRetrieveRequest request)
        {
            StateBag.Clear();

            Connection = connection ?? throw new ArgumentNullException("connection");
            Request = request ?? throw new ArgumentNullException(nameof(request));

            if (request.EntityId == null)
                throw DataValidation.RequiredError("entityId", Localizer);

            ValidateRequest();

            Response = new TRetrieveResponse();
            Row = new TRow();
           
            Query = CreateQuery();

            PrepareQuery(Query);

            OnBeforeExecuteQuery();

            if (Query.GetFirst(Connection))
                Response.Entity = Row;
            else
                throw DataValidation.EntityNotFoundError(Row, request.EntityId, Localizer);

            OnAfterExecuteQuery();

            OnReturn();
            return Response;
        }

        public IRequestContext Context { get; private set; }
        public ITextLocalizer Localizer => Context.Localizer;
        public IPermissionService Permissions => Context.Permissions;
        public ClaimsPrincipal User => Context.User;

        public IDbConnection Connection { get; private set; }
        IRow IRetrieveRequestHandler.Row => Row;
        public SqlQuery Query { get; private set; }
        RetrieveRequest IRetrieveRequestHandler.Request { get { return Request; } }
        IRetrieveResponse IRetrieveRequestHandler.Response { get { return Response; } }
        bool IRetrieveRequestHandler.ShouldSelectField(Field field) { return ShouldSelectField(field); }
        bool IRetrieveRequestHandler.AllowSelectField(Field field) { return AllowSelectField(field); }

        IRetrieveResponse IRetrieveRequestProcessor.Process(IDbConnection connection, RetrieveRequest request)
        {
            return Process(connection, (TRetrieveRequest)request);
        }

        public IDictionary<string, object> StateBag { get; private set; }
    }
}
