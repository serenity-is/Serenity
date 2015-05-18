using System.Data;
using Serenity.Data;
using Serenity.Services;
using System;
using System.Reflection;
using System.Globalization;

namespace Serenity.Services
{
    public class DeleteRequestHandler<TRow, TDeleteRequest, TDeleteResponse>
        where TRow : Row, IIdRow, new()
        where TDeleteRequest: DeleteRequest
        where TDeleteResponse : DeleteResponse, new()
    {
        protected IUnitOfWork UnitOfWork;
        protected TRow Row;
        protected TDeleteResponse Response;
        protected TDeleteRequest Request;
        private static bool loggingInitialized;
        protected static CaptureLogHandler<TRow> captureLogHandler;
        protected static bool hasAuditLogAttribute;

        protected IDbConnection Connection
        {
            get { return UnitOfWork.Connection; }
        }

        protected virtual AuditDeleteRequest GetAuditRequest()
        {
            //EntityType entityType;
            //if (SiteSchema.Instance.TableToType.TryGetValue(Row.Table, out entityType))
            {
                var auditRequest = new AuditDeleteRequest(Row.Table, Row.IdField[Row].Value);

                var parentIdRow = Row as IParentIdRow;
                if (parentIdRow != null)
                {
                    var parentIdField = (Field)parentIdRow.ParentIdField;
                    //EntityType parentEntityType;
                    if (!parentIdField.ForeignTable.IsNullOrEmpty())
                        //SiteSchema.Instance.TableToType.TryGetValue(parentIdField.ForeignTable, out parentEntityType))
                    {
                        auditRequest.ParentTypeId = parentIdField.ForeignTable;
                        auditRequest.ParentId = parentIdRow.ParentIdField[Row];
                    }
                }

                return auditRequest;
            }

            //return null;
        }

        protected virtual void OnBeforeDelete()
        {
        }

        protected virtual BaseCriteria GetDisplayOrderFilter()
        {
            return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
        }

        protected virtual void OnAfterDelete()
        {
            var displayOrderRow = Row as IDisplayOrderRow;
            if (displayOrderRow != null)
            {
                var filter = GetDisplayOrderFilter();
                DisplayOrderHelper.ReorderValues(Connection, displayOrderRow, filter, -1, 1, false);
            }
        }

        protected virtual void ValidateRequest()
        {
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            query.SelectTableFields();
        }

        protected virtual void LoadEntity()
        {
            var idField = (Field)Row.IdField;

            var query = new SqlQuery().From(Row)
                .WhereEqual(idField, Request.EntityId.Value);

            PrepareQuery(query);

            if (!query.GetFirst(Connection))
                throw DataValidation.EntityNotFoundError(Row, Request.EntityId.Value);
        }

        protected virtual void ExecuteDelete()
        {
            var isDeletedRow = Row as IIsActiveDeletedRow;
            var deleteLogRow = Row as IDeleteLogRow;
            var idField = (Field)Row.IdField;
            var id = Request.EntityId.Value;

            if (isDeletedRow == null && deleteLogRow == null)
            {
                if (new SqlDelete(Row.Table)
                        .WhereEqual(idField, id)
                        .Execute(Connection) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id);
            }
            else
            {
                if (isDeletedRow != null)
                {
                    if (new SqlUpdate(Row.Table)
                            .Set(isDeletedRow.IsActiveField, -1)
                            .WhereEqual(idField, id)
                            .Where(new Criteria(isDeletedRow.IsActiveField) >= 0)
                            .Execute(Connection) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id);
                }
                else //if (deleteLogRow != null)
                {
                    if (new SqlUpdate(Row.Table)
                            .Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now, deleteLogRow.DeleteDateField.DateTimeKind))
                            .Set((Field)deleteLogRow.DeleteUserIdField, Authorization.UserId.TryParseID())
                            .WhereEqual(idField, id)
                            .Where(new Criteria((Field)deleteLogRow.DeleteUserIdField).IsNull())
                            .Execute(Connection) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id);
                }
            }

            InvalidateCacheOnCommit();
        }

        protected virtual void InvalidateCacheOnCommit()
        {
            var attr = typeof(TRow).GetCustomAttribute<TwoLevelCachedAttribute>(false);
            if (attr != null)
            {
                BatchGenerationUpdater.OnCommit(this.UnitOfWork, Row.GetFields().GenerationKey);
                foreach (var key in attr.GenerationKeys)
                    BatchGenerationUpdater.OnCommit(this.UnitOfWork, key);
            }
        }

        protected virtual void DoGenericAudit()
        {
            var auditRequest = GetAuditRequest();
            if (auditRequest != null)
                AuditLogService.AuditDelete(Connection, RowRegistry.GetConnectionKey(Row), auditRequest);
        }

        protected virtual void DoCaptureLog()
        {
            captureLogHandler.Log(this.UnitOfWork, this.Row, Authorization.UserId.TryParseID().Value, isDelete: true);
        }

        protected virtual void DoAudit()
        {
            if (!loggingInitialized)
            {
                var logTableAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>();
                if (logTableAttr != null)
                    captureLogHandler = new CaptureLogHandler<TRow>();

                hasAuditLogAttribute = typeof(TRow).GetCustomAttribute<AuditLogAttribute>(false) != null;

                loggingInitialized = true;
            }

            if (captureLogHandler != null)
                DoCaptureLog();
            else if (hasAuditLogAttribute)
                DoGenericAudit();
        }

        protected virtual void OnReturn()
        {
        }

        protected virtual void ValidatePermissions()
        {
            var attr = (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<DeletePermissionAttribute>(false) ??
                typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(false); 

            if (attr != null)
            {
                if (attr.Permission.IsNullOrEmpty())
                    Authorization.ValidateLoggedIn();
                else
                    Authorization.ValidatePermission(attr.Permission);
            }
        }

        public TDeleteResponse Process(IUnitOfWork unitOfWork, TDeleteRequest request)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            ValidatePermissions();

            UnitOfWork = unitOfWork;

            Request = request;
            Response = new TDeleteResponse();

            if (request.EntityId == null)
                throw DataValidation.RequiredError("EntityId");

            Row = new TRow();

            var idField = (Field)Row.IdField;

            LoadEntity();

            ValidateRequest();

            var isDeletedRow = Row as IIsActiveDeletedRow;
            var deleteLogRow = Row as IDeleteLogRow;

            if ((isDeletedRow != null &&
                 isDeletedRow.IsActiveField[Row] < 0) ||
                (deleteLogRow != null &&
                 deleteLogRow.DeleteUserIdField[Row] != null))
                Response.WasAlreadyDeleted = true;
            else
            {
                OnBeforeDelete();

                ExecuteDelete();

                OnAfterDelete();

                DoAudit();
            }

            OnReturn();

            return Response;
        }
    }

    public class DeleteRequestHandler<TRow> : DeleteRequestHandler<TRow, DeleteRequest, DeleteResponse>
        where TRow : Row, IIdRow, new()
    {
    }
}