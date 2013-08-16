//using System.Web.Mvc;
using Serenity.Data;
using Serenity.Services;

namespace Serenity.Services
{
    /*
    public class GenericEntityController<TRow, TService, TListRequest> : Controller
        where TRow: Row, new()
        where TService : IEntityService<TRow, TListRequest>, new()
        where TListRequest: ListRequest
    {
        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<CreateResponse> Create(SaveRequest<TRow> request)
        {
            return this.Respond(() => new TService().Create(request));
        }

        [AcceptVerbs("GET", "POST")]
        [JsonFilter]
        public Result<ListResponse<MyRow>> ListTListRequest request)
        {
            return this.Respond(() => new TService().List(request));
        }

        [AcceptVerbs("GET", "POST")]
        [JsonFilter]
        public Result<RetrieveResponse<MyRow>> Retrieve(RetrieveRequest request)
        {
            return this.Respond(() => new TService().Retrieve(request));
        }

        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<UpdateResponse> Update(SaveRequest<TRow> request)
        {
            return this.Respond(() => new TService().Update(request));
        }

        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<DeleteResponse> Delete(DeleteRequest request)
        {
            return this.Respond(() => new TService().Delete(request));
        }

        [AcceptVerbs("POST")]
        [JsonFilter]
        public Result<UndeleteResponse> Undelete(UndeleteRequest request)
        {
            return this.Respond(() => new TService().Undelete(request));
        }
    }*/
}