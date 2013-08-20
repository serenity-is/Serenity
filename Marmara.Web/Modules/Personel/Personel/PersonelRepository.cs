
namespace Marmara.Personel.Repositories
{
    using Serenity;
    using Serenity.Data;
    using Serenity.Services;
    using System;
    using System.Data;
    using MyRow = Entities.PersonelRow;

    public class PersonelRepository
    {
        private static MyRow.RowFields fld { get { return MyRow.Fields; } }

        public CreateResponse Create(IUnitOfWork uow, SaveRequest<MyRow> request)
        {
            return new MyCreateHandler().Process(uow, request);
        }

        public UpdateResponse Update(IUnitOfWork uow, SaveRequest<MyRow> request)
        {
            return new MyUpdateHandler().Process(uow, request);
        }

        public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request)
        {
            return new MyDeleteHandler().Process(uow, request);
        }

        public UndeleteResponse Undelete(IUnitOfWork uow, UndeleteRequest request)
        {
            return new MyUndeleteHandler().Process(uow, request);
        }

        public RetrieveResponse<MyRow> Retrieve(IDbConnection connection, RetrieveRequest request)
        {
            return new MyRetrieveHandler().Process(connection, request);
        }

        public ListResponse<MyRow> List(IDbConnection connection, ListRequest request)
        {
            return new MyListHandler().Process(connection, request);
        }

        private class MyCreateHandler : CreateRequestHandler<MyRow> { }
        private class MyUpdateHandler : UpdateRequestHandler<MyRow> { }
        private class MyDeleteHandler : DeleteRequestHandler<MyRow> { }
        private class MyUndeleteHandler : UndeleteRequestHandler<MyRow> { }
        private class MyRetrieveHandler : RetrieveRequestHandler<MyRow> { }
        private class MyListHandler : ListRequestHandler<MyRow> { }
    }
}