using Microsoft.AspNetCore.Hosting;
using MyRepository = Serene.Administration.Repositories.TranslationRepository;

namespace Serene.Administration.Endpoints;

[Route("Services/Administration/Translation/[action]")]
[ServiceAuthorize(PermissionKeys.Translation)]
public class TranslationEndpoint(IWebHostEnvironment hostEnvironment,
    ILocalTextRegistry localTextRegistry, ITypeSource typeSource) : ServiceEndpoint
{
    protected IWebHostEnvironment HostEnvironment { get; } = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    protected ILocalTextRegistry LocalTextRegistry { get; } = localTextRegistry ?? throw new ArgumentNullException(nameof(localTextRegistry));
    protected ITypeSource TypeSource { get; } = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    private MyRepository NewRepository()
    {
        return new MyRepository(Context, HostEnvironment, LocalTextRegistry, TypeSource);
    }

    public ListResponse<TranslationItem> List(TranslationListRequest request)
    {
        return NewRepository().List(request);
    }

    [HttpPost]
    public SaveResponse Update(TranslationUpdateRequest request)
    {
        return NewRepository().Update(request, HttpContext.RequestServices);
    }
}