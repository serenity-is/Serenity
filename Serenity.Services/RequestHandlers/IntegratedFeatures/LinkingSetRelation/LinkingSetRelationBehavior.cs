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
    public class LinkingSetRelationBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IRetrieveBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private LinkingSetRelationAttribute attr;
        private Func<IList> listFactory;
        private Func<Row> rowFactory;
        private Func<IListRequestProcessor> listHandlerFactory;
        private Func<ISaveRequestProcessor> saveHandlerFactory;
        private Func<IDeleteRequestProcessor> deleteHandlerFactory;
        private Func<ISaveRequest> saveRequestFactory;
        private Field thisKeyField;
        private Field itemKeyField;

        public bool ActivateFor(Row row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<LinkingSetRelationAttribute>();
            if (attr == null)
                return false;

            var listType = Target.ValueType;
            if (!listType.IsGenericType ||
                listType.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new ArgumentException(String.Format("Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                    "but its property type is not a generic List (e.g. List<int>)!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }

            var rowType = attr.RowType;
            if (rowType.IsAbstract ||
                !typeof(Row).IsAssignableFrom(rowType))
            {
                throw new ArgumentException(String.Format(
                    "Field '{0}' in row type '{1}' has a LinkingSetRelationBehavior " +
                    "but its row type is not valid!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));
            }
            
            listFactory = FastReflection.DelegateForConstructor<IList>(listType);
            rowFactory = FastReflection.DelegateForConstructor<Row>(rowType);

            listHandlerFactory = FastReflection.DelegateForConstructor<IListRequestProcessor>(
                typeof(ListRequestHandler<>).MakeGenericType(rowType));

            saveHandlerFactory = FastReflection.DelegateForConstructor<ISaveRequestProcessor>(
                typeof(SaveRequestHandler<>).MakeGenericType(rowType));

            saveRequestFactory = FastReflection.DelegateForConstructor<ISaveRequest>(
                typeof(SaveRequest<>).MakeGenericType(rowType));

            deleteHandlerFactory = FastReflection.DelegateForConstructor<IDeleteRequestProcessor>(
                typeof(DeleteRequestHandler<>).MakeGenericType(rowType));

            var detailRow = rowFactory();

            thisKeyField = detailRow.FindFieldByPropertyName(attr.ThisKey) ??
                detailRow.FindField(attr.ThisKey);

            if (ReferenceEquals(thisKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                    attr.ThisKey, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            itemKeyField = detailRow.FindFieldByPropertyName(attr.ItemKey) ??
                detailRow.FindField(attr.ItemKey);

            if (ReferenceEquals(itemKeyField, null))
                throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                    "This field is specified for a linking set relation in field '{2}' of row type '{3}'.",
                    attr.ItemKey, detailRow.GetType().FullName,
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

            return true;
        }

        public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
        public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
        public void OnValidateRequest(IRetrieveRequestHandler handler) { }

        public void OnReturn(IRetrieveRequestHandler handler)
        {
            if (ReferenceEquals(null, Target) ||
                !handler.ShouldSelectField(Target))
                return;

            var idField = (handler.Row as IIdRow).IdField;

            var listHandler = listHandlerFactory();

            var listRequest = new ListRequest
            {
                ColumnSelection = ColumnSelection.KeyOnly,
                IncludeColumns = new HashSet<string>
                {
                    itemKeyField.PropertyName ?? itemKeyField.Name
                },
                EqualityFilter = new Dictionary<string, object>
                {
                    { thisKeyField.PropertyName ?? thisKeyField.Name, idField[handler.Row] }
                }
            };

            IListResponse response = listHandler.Process(handler.Connection, listRequest);

            var list = listFactory();
            foreach (Row item in response.Entities)
                list.Add(itemKeyField.AsObject(item));

            Target.AsObject(handler.Row, list);
        }

        private void SaveDetail(IUnitOfWork uow, Row detail, Int64 masterId, Int64? detailId)
        {
            detail = detail.Clone();

            ((IIdRow)detail).IdField[detail] = detailId;
            ((IIdField)thisKeyField)[detail] = masterId;

            var saveHandler = saveHandlerFactory();
            var saveRequest = saveRequestFactory();
            saveRequest.Entity = detail;
            saveHandler.Process(uow, saveRequest, detailId == null ? SaveRequestType.Create : SaveRequestType.Update);
        }

        private void DeleteDetail(IUnitOfWork uow, Int64 detailId)
        {
            var deleteHandler = deleteHandlerFactory();
            var deleteRequest = new DeleteRequest { EntityId = detailId };
            deleteHandler.Process(uow, deleteRequest);
        }
    }
}