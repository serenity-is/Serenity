using System.Data;
using Serenity.Data;
using Serenity.Services;
using System;
using System.Reflection;

namespace Serenity.Services
{
    public class DeleteRequestHandler<TRow, TDeleteRequest, TDeleteResponse>
        where TRow : Row, IIdRow, new()
        where TDeleteRequest: DeleteRequest
        where TDeleteResponse : DeleteResponse, new()
    {
        protected bool ShouldActuallyDelete { get; set; }

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
                    if (!parentIdField.ForeignTable.IsEmptyOrNull())
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

        protected virtual void OnAfterDelete()
        {
        }

        protected virtual void ValidateRequest()
        {
        }

        protected virtual void PrepareQuery(SqlSelect query)
        {
            query.SelectTableFields();
        }

        protected virtual void LoadEntity()
        {
            var idField = (Field)Row.IdField;

            var query = new SqlSelect().FromAs(Row, 0)
                .WhereEqual(idField, Request.EntityId.Value);

            PrepareQuery(query);

            if (!query.GetFirst(Connection))
                throw DataValidation.EntityNotFoundError(Row, Request.EntityId.Value);
        }

        protected virtual void ExecuteDelete()
        {
            var isActiveRow = Row as IIsActiveRow;
            var idField = (Field)Row.IdField;
            var id = Request.EntityId.Value;

            if (ShouldActuallyDelete ||
                isActiveRow == null)
            {
                if (new SqlDelete(Row.Table)
                        .WhereEqual(idField, id)
                        .Execute(Connection) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id);
            }
            else
            {
                if (new SqlUpdate(Row.Table)
                        .Set(isActiveRow.IsActiveField, -1)
                        .WhereEqual(idField, id)
                        .Where(new Criteria(isActiveRow.IsActiveField) >= 0)
                        .Execute(Connection) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id);
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
                AuditLogService.AuditDelete(Connection, RowRegistry.GetSchemaName(Row), auditRequest);
        }

        protected virtual void DoCaptureLog()
        {
            captureLogHandler.Log(this.UnitOfWork, this.Row, SecurityHelper.CurrentUserId, isDelete: true);
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
            var modifyAttr = typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(false);
            if (modifyAttr != null)
            {
                if (modifyAttr.ModifyPermission.IsEmptyOrNull())
                    SecurityHelper.EnsureLoggedIn(RightErrorHandling.ThrowException);
                else
                    SecurityHelper.EnsurePermission(modifyAttr.ModifyPermission, RightErrorHandling.ThrowException);
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

            var isActiveRow = Row as IIsActiveRow;

            if (isActiveRow != null &&
                !ShouldActuallyDelete &&
                isActiveRow.IsActiveField[Row] < 0)
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