namespace Serenity.Services
{
    using System;
    using System.Data;
    using Serenity.Data;
    using Serenity.Services;
    using System.Collections.Generic;
    using System.Reflection;

    public class RetrieveRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse>
        where TRow: Row, new()
        where TRetrieveRequest: RetrieveRequest
        where TRetrieveResponse: RetrieveResponse<TRow>, new()
    {
        protected IDbConnection Connection;
        protected TRow Row;
        protected TRetrieveResponse Response;
        protected TRetrieveRequest Request;

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
                    Request.ExcludeColumns.Contains(field.PropertyName));

            bool explicitlyIncluded = !explicitlyExcluded && Request.IncludeColumns != null &&
                (Request.IncludeColumns.Contains(field.Name) ||
                 Request.IncludeColumns.Contains(field.PropertyName));

            if (isPrimaryKey)
                return explicitlyIncluded;

            if (explicitlyExcluded)
                return false;

            if (explicitlyIncluded)
                return true;

            var selection = Request.ColumnSelection;

            switch (selection)
            {
                case RetrieveColumnSelection.Lookup:
                    return mode <= SelectLevel.Lookup;
                case RetrieveColumnSelection.List:
                    return mode <= SelectLevel.List;
                case RetrieveColumnSelection.Details:
                    return mode <= SelectLevel.Details;
                default:
                    return false;
            }
        }

        protected virtual void SelectField(SqlSelect query, Field field)
        {
            query.Select(field);
        }

        protected virtual void SelectFields(SqlSelect query)
        {
            foreach (var field in Row.GetFields())
            {
                if (ShouldSelectField(field))
                    SelectField(query, field);
            }
        }

        protected virtual void OnReturn()
        {
        }

        protected virtual void PrepareQuery(SqlSelect query)
        {
            SelectFields(query);
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
                if (readAttr.ReadPermission.IsEmptyOrNull())
                    SecurityHelper.EnsureLoggedIn(RightErrorHandling.ThrowException);
                else
                    SecurityHelper.EnsurePermission(readAttr.ReadPermission, RightErrorHandling.ThrowException);
            }
        }

        protected virtual SqlSelect CreateQuery()
        {
            var query = new SqlSelect()
                .FromAs(Row, 0);

            query.WhereEqual((Field)(((IIdRow)Row).IdField), Request.EntityId.Value);

            return query;
        }

        public TRetrieveResponse Process(IDbConnection connection, TRetrieveRequest request)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;

            Request = request;
            Response = new TRetrieveResponse();

            Row = new TRow();

            if (request.EntityId == null)
                throw DataValidation.RequiredError("EntityId");

            var query = CreateQuery();

            PrepareQuery(query);

            if (query.GetFirst(Connection))
                Response.Entity = Row;
            else
                throw DataValidation.EntityNotFoundError(Row, request.EntityId.Value);

            OnReturn();
            return Response;
        }
    }

    public class RetrieveRequestHandler<TRow> : RetrieveRequestHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>
        where TRow : Row, new()
    {
    }
}