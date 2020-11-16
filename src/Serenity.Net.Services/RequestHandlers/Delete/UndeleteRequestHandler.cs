using Serenity.Abstractions;
using Serenity.Data;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;

namespace Serenity.Services
{
    public class UndeleteRequestHandler<TRow, TUndeleteResponse>
        where TRow : class, IRow, IIdRow, new()
        where TUndeleteResponse : UndeleteResponse, new()
    {
        protected IUnitOfWork UnitOfWork;
        protected TRow Row;
        protected TUndeleteResponse Response;
        protected UndeleteRequest Request;
        private static bool loggingInitialized;
        //protected static CaptureLogHandler<TRow> captureLogHandler;
        protected Lazy<IDeleteBehavior[]> behaviors;

        public UndeleteRequestHandler(IRequestHandlerContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected IDbConnection Connection
        {
            get { return UnitOfWork.Connection; }
        }

        protected virtual void OnBeforeUndelete()
        {
        }

        protected virtual BaseCriteria GetDisplayOrderFilter()
        {
            return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
        }

        protected virtual void OnAfterUndelete()
        {
            var displayOrderRow = Row as IDisplayOrderRow;
            if (displayOrderRow != null)
            {
                var filter = GetDisplayOrderFilter();
                DisplayOrderHelper.ReorderValues(Connection, displayOrderRow, filter,
                    Row.IdField[Row].Value, displayOrderRow.DisplayOrderField[Row].Value, false);
            }
        }

        protected virtual void ValidateRequest()
        {
        }

        protected virtual void DoCaptureLog()
        {
            var newRow = Row.Clone();
            if (newRow is IIsActiveRow)
                ((IIsActiveRow)newRow).IsActiveField[newRow] = 1;
            //captureLogHandler.Log(UnitOfWork, Row, newRow, 
            //    User.GetIdentifier());
        }

        protected virtual void DoAudit()
        {
            if (!loggingInitialized)
            {
                //var logTableAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>();
                //if (logTableAttr != null)
                //    captureLogHandler = new CaptureLogHandler<TRow>();

                loggingInitialized = true;
            }

            //if (captureLogHandler != null)
            //    DoCaptureLog();
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            query.SelectTableFields();
        }

        protected virtual void LoadEntity()
        {
            var idField = (Field)Row.IdField;
            var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

            var query = new SqlQuery()
                .Dialect(Connection.GetDialect())
                .From(Row)
                .WhereEqual(idField, id);

            PrepareQuery(query);

            if (!query.GetFirst(Connection))
                throw DataValidation.EntityNotFoundError(Row, id, Localizer);
        }

        protected virtual void OnReturn()
        {
        }

        protected virtual void ValidatePermissions()
        {
            var attr = (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<DeletePermissionAttribute>(false) ??
                typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(false);

            if (attr != null)
                Permissions.ValidatePermission(attr.Permission ?? "?", Localizer);
        }

        protected virtual void InvalidateCacheOnCommit()
        {
            BatchGenerationUpdater.OnCommit(this.UnitOfWork, Context.Cache, Row.GetFields().GenerationKey);
            var attr = typeof(TRow).GetCustomAttribute<TwoLevelCachedAttribute>(false);
            if (attr != null)
                foreach (var key in attr.GenerationKeys)
                    BatchGenerationUpdater.OnCommit(this.UnitOfWork, Context.Cache, key);
        }

        public TUndeleteResponse Process(IUnitOfWork unitOfWork, UndeleteRequest request)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            ValidatePermissions();

            UnitOfWork = unitOfWork;

            Request = request;
            Response = new TUndeleteResponse();

            if (request.EntityId == null)
                throw DataValidation.RequiredError("EntityId", Localizer);

            Row = new TRow();

            var isActiveDeletedRow = Row as IIsActiveDeletedRow;
            var isDeletedRow = Row as IIsDeletedRow;
            var deleteLogRow = Row as IDeleteLogRow;

            if (isActiveDeletedRow == null && isDeletedRow == null && deleteLogRow == null)
                throw new NotImplementedException();

            var idField = (Field)Row.IdField;
            var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

            LoadEntity();

            ValidateRequest();

            if ((isDeletedRow != null && isDeletedRow.IsDeletedField[Row] != true) ||
                (isActiveDeletedRow != null && isActiveDeletedRow.IsActiveField[Row] >= 0) ||
                (deleteLogRow != null && ((Field)deleteLogRow.DeleteUserIdField).IsNull(Row)))
                Response.WasNotDeleted = true;
            else
            {
                OnBeforeUndelete();

                var update = new SqlUpdate(Row.Table)
                    .WhereEqual(idField, id);

                if (isActiveDeletedRow != null)
                {
                    update.Set(isActiveDeletedRow.IsActiveField, 1)
                        .WhereEqual(isActiveDeletedRow.IsActiveField, -1);
                }
                else if (isDeletedRow != null)
                {
                    update.Set(isDeletedRow.IsDeletedField, false)
                        .WhereEqual(isDeletedRow.IsDeletedField, 1);
                }

                if (deleteLogRow != null)
                {
                    update.Set((Field)deleteLogRow.DeleteUserIdField, null)
                        .Set(deleteLogRow.DeleteDateField, null);

                    if (isActiveDeletedRow == null && isDeletedRow == null)
                        update.Where(((Field)deleteLogRow.DeleteUserIdField).IsNotNull());
                }

                if (update.Execute(Connection) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id, Localizer);

                InvalidateCacheOnCommit();

                OnAfterUndelete();

                DoAudit();
            }

            OnReturn();

            return Response;
        }

        public IRequestHandlerContext Context { get; private set; }
        public ITextLocalizer Localizer => Context.Localizer;
        public IPermissionService Permissions => Context.Permissions;
        public ClaimsPrincipal User => Context.User;
    }

    public class UndeleteRequestHandler<TRow> : UndeleteRequestHandler<TRow, UndeleteResponse>
        where TRow : class, IRow, IIdRow, new()
    {
        public UndeleteRequestHandler(IRequestHandlerContext context)
            : base(context)
        {
        }
    }
}