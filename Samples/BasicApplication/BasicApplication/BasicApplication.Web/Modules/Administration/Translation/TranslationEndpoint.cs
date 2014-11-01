
namespace BasicApplication.Administration.Endpoints
{
    using Serenity.Data;
    using Serenity.Services;
    using System.Web.Mvc;
    using MyRepository = Repositories.TranslationRepository;

    [RoutePrefix("Services/Administration/Translation"), Route("{action}")]
    [ConnectionKey("Default"), ServiceAuthorize("Administration")]
    public class TranslationController : ServiceEndpoint
    {
        public ListResponse<TranslationItem> List(TranslationListRequest request)
        {
            return new MyRepository().List(request);
        }

        [HttpPost]
        public SaveResponse Update(TranslationUpdateRequest request)
        {
            return new MyRepository().Update(request);
        }
    }
}
