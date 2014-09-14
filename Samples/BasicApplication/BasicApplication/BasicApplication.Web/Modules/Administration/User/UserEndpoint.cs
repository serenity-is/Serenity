

namespace BasicApplication.Administration.Endpoints
{
    using Serenity.Services;
    using System.Web.Mvc;
    using MyRepository = Repositories.UserRepository;
    using MyRow = Entities.UserRow;

    [ServiceAuthorize]
    [RoutePrefix("Services/Administration/User"), Route("{action}")]
    public class UserController : Controller
    {
        [AcceptVerbs("POST"), JsonFilter]
        public Result<SaveResponse> Create(SaveRequest<MyRow> request)
        {
            return this.InTransaction("Default", (uow) => new MyRepository().Create(uow, request));
        }

        [AcceptVerbs("POST"), JsonFilter]
        public Result<SaveResponse> Update(SaveRequest<MyRow> request)
        {
            return this.InTransaction("Default", (uow) => new MyRepository().Update(uow, request));
        }
 
        [AcceptVerbs("POST"), JsonFilter]
        public Result<DeleteResponse> Delete(DeleteRequest request)
        {
            return this.InTransaction("Default", (uow) => new MyRepository().Delete(uow, request));
        }

        [AcceptVerbs("POST"), JsonFilter]
        public Result<UndeleteResponse> Undelete(UndeleteRequest request)
        {
            return this.InTransaction("Default", (uow) => new MyRepository().Undelete(uow, request));
        }

        [AcceptVerbs("GET", "POST"), JsonFilter]
        public Result<RetrieveResponse<MyRow>> Retrieve(RetrieveRequest request)
        {
            return this.UseConnection("Default", (cnn) => new MyRepository().Retrieve(cnn, request));
        }

        [AcceptVerbs("GET", "POST"), JsonFilter]
        public Result<ListResponse<MyRow>> List(ListRequest request)
        {
            return this.UseConnection("Default", (cnn) => new MyRepository().List(cnn, request));
        }
    }
}
