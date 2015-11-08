using Serenity;
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
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
        private Func<IList> rowListFactory;
        private Func<Row> rowFactory;
        private Func<IListRequestProcessor> listHandlerFactory;
        private Func<IDeleteRequestProcessor> deleteHandlerFactory;

        public bool ActivateFor(Row row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<MasterDetailRelationAttribute>();
            if (attr == null)
                return false;

            var rowListType = Target.ValueType;
            if (!rowListType.IsGenericType ||
                rowListType.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new ArgumentException(String.Format("Field '{0}' in row type '{1}' has a MasterDetailRelationAttribute " +
                    "but its property type is not a generic List (e.g. List<Row>)!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            rowType = rowListType.GetGenericArguments()[0];
            if (rowType.IsAbstract ||
                !typeof(Row).IsAssignableFrom(rowType))
            {
                throw new ArgumentException(String.Format(
                    "Field '{0}' in row type '{1}' has a MasterDetailRelationAttribute " +
                    "but its property type is not a generic list of rows (e.g. List<Row>)!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            rowListFactory = FastReflection.DelegateForConstructor<IList>(rowListType);
            rowFactory = FastReflection.DelegateForConstructor<Row>(rowType);

            listHandlerFactory = FastReflection.DelegateForConstructor<IListRequestProcessor>(
                typeof(ListRequestHandler<>).MakeGenericType(rowType));

            deleteHandlerFactory = FastReflection.DelegateForConstructor<IDeleteRequestProcessor>(
                typeof(DeleteRequestHandler<>).MakeGenericType(rowType));

            return true;
        }

        public override void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                return;

            var idField = (handler.Row as IIdRow).IdField;

            var row = rowFactory();
            var foreignKeyField = row.FindFieldByPropertyName(attr.ForeignKey) ??
                row.FindField(attr.ForeignKey);

            if (ReferenceEquals(foreignKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." + 
                    "This field is specified for a master detail relation in field '{2}' of row type '{3}'.",
                    attr.ForeignKey, row.GetType().FullName,
                    Target.PropertyName ?? Target.Name, handler.Row.GetType().FullName));

            var rowIdField = (row as IIdRow).IdField;

            var deleteHandler = deleteHandlerFactory();
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

            var listHandler = listHandlerFactory();

            var listRequest = new ListRequest
            {
                ColumnSelection = ColumnSelection.List,
                EqualityFilter = new Dictionary<string, object>
                {
                    { attr.ForeignKey, idField[handler.Row] }
                }
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = rowListFactory();
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