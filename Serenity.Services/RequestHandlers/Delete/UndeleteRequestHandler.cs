using Serenity.Data;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Serenity.Services
{
    public class UndeleteRequestHandler<TRow, TUndeleteResponse>
        where TRow : Row, IIdRow, new()
        where TUndeleteResponse : UndeleteResponse, new()
    {
        protected IUnitOfWork UnitOfWork;
        protected TRow Row;
        protected TUndeleteResponse Response;
        protected UndeleteRequest Request;
        private static bool loggingInitialized;
        protected static CaptureLogHandler<TRow> captureLogHandler;
        protected static bool hasAuditLogAttribute;

        protected IDbConnection Connection
        {
            get { return UnitOfWork.Connection; }
        }

        protected virtual AuditUndeleteRequest GetAuditRequest()
        {
            //EntityType entityType;
            //if (SiteSchema.Instance.TableToType.TryGetValue(Row.Table, out entityType))
            {
                var auditRequest = new AuditUndeleteRequest(Row.Table, Row.IdField[Row].Value);

                var parentIdRow = Row as IParentIdRow;
                if (parentIdRow != null)
                {
                    var parentIdField = (Field)parentIdRow.ParentIdField;
                    //EntityType parentEntityType;
                    if (!parentIdField.ForeignTable.IsNullOrEmpty())// &&
                        //SiteSchema.Instance.TableToType.TryGetValue(parentIdField.ForeignTable, out parentEntityType))
                    {
                        auditRequest.ParentTypeId = parentIdField.ForeignTable;
                        auditRequest.ParentId = parentIdRow.ParentIdField[Row];
                    }
                }

                return auditRequest;
            }
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

        protected virtual void DoGenericAudit()
        {
            var auditRequest = GetAuditRequest();
            if (auditRequest != null)
                AuditLogService.AuditUndelete(Connection, Row.GetFields().ConnectionKey, auditRequest);
        }

        protected virtual void DoCaptureLog()
        {
            var newRow = Row.Clone();
            if (newRow is IIsActiveRow)
                ((IIsActiveRow)newRow).IsActiveField[newRow] = 1;
            captureLogHandler.Log(this.UnitOfWork, this.Row, newRow, Authorization.UserId);
        }

        protected virtual void DoAudit()
        {
            if (!loggingInitialized)
            {
                var logTableAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>();
                if (logTableAttr != null)
                    captureLogHandler = new CaptureLogHandler<TRow>();

                hasAuditLogAttribute = Row.IdField.IsIntegerType &&
                    typeof(TRow).GetCustomAttribute<AuditLogAttribute>(false) != null;

                loggingInitialized = true;
            }

            if (captureLogHandler != null)
                DoCaptureLog();
            else if (hasAuditLogAttribute)
                DoGenericAudit();
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
                throw DataValidation.EntityNotFoundError(Row, id);
        }

        protected virtual void OnReturn()
        {
        }

        protected virtual void ValidatePermissions()
        {
            var attr = (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<DeletePermissionAttribute>(false) ??
                typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(false);

            if (attr != null)
                Authorization.ValidatePermission(attr.Permission ?? "?");
        }

        protected virtual void InvalidateCacheOnCommit()
        {
            BatchGenerationUpdater.OnCommit(this.UnitOfWork, Row);
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
                throw DataValidation.RequiredError("EntityId");

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
                    throw DataValidation.EntityNotFoundError(Row, id);

                InvalidateCacheOnCommit();

                OnAfterUndelete();

                DoAudit();
            }

            OnReturn();

            return Response;
        }
    }

    public class UndeleteRequestHandler<TRow> : UndeleteRequestHandler<TRow, UndeleteResponse>
        where TRow : Row, IIdRow, new()
    {
    }
}