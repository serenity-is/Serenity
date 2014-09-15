

namespace BasicApplication.Northwind.Endpoints
{
    using Serenity;
    using Serenity.Services;
    using System.Web.Mvc;
    using MyRepository = Repositories.CategoryRepository;
    using MyRow = Entities.CategoryRow;

    [ServiceAuthorize]
    [RoutePrefix("Services/Northwind/Category"), Route("{action}")]
    public class CategoryController : Controller
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
