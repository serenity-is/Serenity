using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class DeleteRequestHandler<TRow, TDeleteRequest, TDeleteResponse> : IDeleteRequestHandler, IDeleteRequestProcessor
        where TRow : Row, IIdRow, new()
        where TDeleteRequest: DeleteRequest
        where TDeleteResponse : DeleteResponse, new()
    {
        protected TRow Row;
        protected TDeleteResponse Response;
        protected TDeleteRequest Request;
        protected static IEnumerable<IDeleteBehavior> cachedBehaviors;
        protected IEnumerable<IDeleteBehavior> behaviors;

        public DeleteRequestHandler()
        {
            this.StateBag = new Dictionary<string, object>();
            this.behaviors = GetBehaviors();
        }

        protected virtual IEnumerable<IDeleteBehavior> GetBehaviors()
        {
            if (cachedBehaviors == null)
            {
                cachedBehaviors = RowDeleteBehaviors<TRow>.Default.Concat(
                    this.GetType().GetCustomAttributes().OfType<IDeleteBehavior>()).ToList();
            }

            return cachedBehaviors;
        }

        public IDbConnection Connection
        {
            get { return UnitOfWork.Connection; }
        }

        protected virtual void OnBeforeDelete()
        {
            foreach (var behavior in behaviors)
                behavior.OnBeforeDelete(this);
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

            foreach (var behavior in behaviors)
                behavior.OnAfterDelete(this);
        }

        protected virtual void ValidateRequest()
        {
            foreach (var behavior in behaviors)
                behavior.OnValidateRequest(this);
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            query.SelectTableFields();

            foreach (var behavior in behaviors)
                behavior.OnPrepareQuery(this, query);
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
                throw DataValidation.EntityNotFoundError(Row, Request.EntityId);
        }

        protected virtual void ExecuteDelete()
        {
            var isActiveDeletedRow = Row as IIsActiveDeletedRow;
            var isDeletedRow = Row as IIsDeletedRow;
            var deleteLogRow = Row as IDeleteLogRow;
            var idField = (Field)Row.IdField;
            var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

            if (isActiveDeletedRow == null && isDeletedRow == null && deleteLogRow == null)
            {
                if (new SqlDelete(Row.Table)
                        .WhereEqual(idField, id)
                        .Execute(Connection) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id);
            }
            else
            {
                if (isDeletedRow != null || isActiveDeletedRow != null)
                {
                    
                    var updateLogRow = Row as IUpdateLogRow;
                    var update = new SqlUpdate(Row.Table)
                        .WhereEqual(idField, id)
                        .Where(ServiceQueryHelper.GetNotDeletedCriteria(Row));

                    if (isActiveDeletedRow != null)
                    {
                        update.Set(isActiveDeletedRow.IsActiveField, -1);
                    }
                    else if (isDeletedRow != null)
                    {
                        update.Set(isDeletedRow.IsDeletedField, true);
                    }

                    if (deleteLogRow != null)
                    {
                        update.Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now, 
                                        deleteLogRow.DeleteDateField.DateTimeKind))
                              .Set((Field)deleteLogRow.DeleteUserIdField, Authorization.UserId.TryParseID());
                    }
                    else if (updateLogRow != null)
                    {
                        update.Set(updateLogRow.UpdateDateField, DateTimeField.ToDateTimeKind(DateTime.Now, 
                                        updateLogRow.UpdateDateField.DateTimeKind))
                              .Set((Field)updateLogRow.UpdateUserIdField, Authorization.UserId.TryParseID());
                    }

                    if (update.Execute(Connection) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id);
                }
                else //if (deleteLogRow != null)
                {
                    if (new SqlUpdate(Row.Table)
                            .Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now, 
                                        deleteLogRow.DeleteDateField.DateTimeKind))
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
            BatchGenerationUpdater.OnCommit(this.UnitOfWork, Row);
        }

        protected virtual void DoAudit()
        {
            foreach (var behavior in behaviors)
                behavior.OnAudit(this);
        }

        protected virtual void OnReturn()
        {
            foreach (var behavior in behaviors)
                behavior.OnReturn(this);
        }

        protected virtual void ValidatePermissions()
        {
            var attr = (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<DeletePermissionAttribute>(true) ??
                (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(true) ??
                typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

            if (attr != null)
                Authorization.ValidatePermission(attr.Permission ?? "?");
        }

        public TDeleteResponse Process(IUnitOfWork unitOfWork, TDeleteRequest request)
        {
            StateBag.Clear();

            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            UnitOfWork = unitOfWork;
            Request = request;
            Response = new TDeleteResponse();

            if (request.EntityId == null)
                throw DataValidation.RequiredError("EntityId");

            Row = new TRow();

            var idField = (Field)Row.IdField;

            LoadEntity();
            ValidatePermissions();
            ValidateRequest();

            var isActiveDeletedRow = Row as IIsActiveDeletedRow;
            var isDeletedRow = Row as IIsDeletedRow;
            var deleteLogRow = Row as IDeleteLogRow;

            if ((isDeletedRow != null && isDeletedRow.IsDeletedField[Row] == true) ||
                (isActiveDeletedRow != null && isActiveDeletedRow.IsActiveField[Row] < 0) ||
                (deleteLogRow != null && !((Field)deleteLogRow.DeleteUserIdField).IsNull(Row)))
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

        DeleteResponse IDeleteRequestProcessor.Process(IUnitOfWork uow, DeleteRequest request)
        {
            return Process(uow, (TDeleteRequest)request);
        }

        public IUnitOfWork UnitOfWork { get; protected set; }  
        DeleteRequest IDeleteRequestHandler.Request { get { return this.Request; } }
        DeleteResponse IDeleteRequestHandler.Response { get { return this.Response; } }
        Row IDeleteRequestHandler.Row { get { return this.Row; } }
        public IDictionary<string, object> StateBag { get; private set; }
    }

    public class DeleteRequestHandler<TRow> : DeleteRequestHandler<TRow, DeleteRequest, DeleteResponse>
        where TRow : Row, IIdRow, new()
    {
    }

    public interface IDeleteRequestProcessor
    {
        DeleteResponse Process(IUnitOfWork uow, DeleteRequest request);
    }
}