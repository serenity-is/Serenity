
namespace Marmara.Personel.Endpoints
{
    using Serenity;
    using Serenity.Services;
    using System.Web.Mvc;
    using MyRepository = Repositories.PersonelRepository;
    using MyRow = Entities.PersonelRow;

    public class PersonelController : Controller
    {
        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<CreateResponse> Create(SaveRequest<MyRow> request)
        {
            return this.InTransaction("Personel", (uow) => new MyRepository().Create(uow, request));
        }

        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<UpdateResponse> Update(SaveRequest<MyRow> request)
        {
            return this.InTransaction("Personel", (uow) => new MyRepository().Update(uow, request));
        }
 
        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<DeleteResponse> Delete(DeleteRequest request)
        {
            return this.InTransaction("Personel", (uow) => new MyRepository().Delete(uow, request));
        }

        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<UndeleteResponse> Undelete(UndeleteRequest request)
        {
            return this.InTransaction("Personel", (uow) => new MyRepository().Undelete(uow, request));
        }

        [AcceptVerbs("GET", "POST")]
        [JsonFilter]
        public Result<RetrieveResponse<MyRow>> Retrieve(RetrieveRequest request)
        {
            return this.UseConnection("Personel", (cnn) => new MyRepository().Retrieve(cnn, request));
        }

        [AcceptVerbs("GET", "POST")]
        [JsonFilter]
        public Result<ListResponse<MyRow>> List(ListRequest request)
        {
            return this.UseConnection("Personel", (cnn) => new MyRepository().List(cnn, request));
        }
    }
}
