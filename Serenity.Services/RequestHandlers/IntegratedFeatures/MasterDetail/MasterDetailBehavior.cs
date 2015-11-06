using Serenity;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public class MasterDetailBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IRetrieveBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private MasterDetailRelationAttribute attr;
        private Type rowType;
        private Type rowListType;
        private Type listHandlerType;
        private Type deleteHandlerType;

        public bool ActivateFor(Row row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<MasterDetailRelationAttribute>();
            if (attr == null || string.IsNullOrEmpty(attr.ForeignKey))
                return false;

            rowListType = Target.ValueType;
            if (!rowListType.IsGenericType ||
                rowListType.GetGenericTypeDefinition() != typeof(List<>))
                return false;

            rowType = rowListType.GetGenericArguments()[0];
            if (rowType.IsAbstract ||
                !typeof(Row).IsAssignableFrom(rowType))
                return false;

            listHandlerType = typeof(ListRequestHandler<>).MakeGenericType(rowType);
            deleteHandlerType = typeof(DeleteRequestHandler<>).MakeGenericType(rowType);

            return true;
        }

        public override void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                return;

            var idField = (handler.Row as IIdRow).IdField;

            var row = (Row)Activator.CreateInstance(rowType);
            var foreignKeyField = row.FindFieldByPropertyName(attr.ForeignKey) ??
                row.FindField(attr.ForeignKey);

            if (ReferenceEquals(foreignKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." + 
                    "This field is specified for a master detail relation in field '{2}' of row type '{3}'.",
                    attr.ForeignKey, row.GetType().FullName,
                    Target.PropertyName ?? Target.Name, handler.Row.GetType().FullName));

            var rowIdField = (row as IIdRow).IdField;

            var deleteHandler = (IDeleteRequestProcessor)Activator.CreateInstance(deleteHandlerType);
            var deleteList = new List<Int64>();
            new SqlQuery()
                    .From(row)
                    .Select((Field)rowIdField)
                    .Where(foreignKeyField == idField[handler.Row].Value)
                    .ForEach(handler.Connection, () =>
            {
                deleteList.Add(rowIdField[row].Value);
            });

            foreach (var id in deleteList)
                deleteHandler.Process(handler.UnitOfWork, new DeleteRequest { EntityId = id });
        }

        public void OnReturn(IRetrieveRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                !handler.ShouldSelectField(Target))
                return;

            var idField = (handler.Row as IIdRow).IdField;

            var listHandler = (IListRequestProcessor)Activator.CreateInstance(listHandlerType);

            var listRequest = new ListRequest
            {
                ColumnSelection = ColumnSelection.List,
                EqualityFilter = new Dictionary<string, object>
                {
                    { attr.ForeignKey, idField[handler.Row] }
                }
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = (IList)Activator.CreateInstance(rowListType);
            foreach (var item in response.Entities)
                list.Add(item);

            Target.AsObject(handler.Row, list);
        }

        public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
        public void OnValidateRequest(IRetrieveRequestHandler handler) { }
    }
}