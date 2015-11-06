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

            return true;
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