using Microsoft.Extensions.Caching.Memory;

namespace Serene.Administration;

[DataScript("Administration.ImplicitPermissions", Permission = PermissionKeys.Security)]
public class ImplicitPermissionsDataScript(IMemoryCache cache, ITypeSource typeSource) : DataScript<IDictionary<string, HashSet<string>>>
{
    private readonly IMemoryCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    protected override IDictionary<string, HashSet<string>> GetData()
    {
        return BasePermissionService.GetImplicitPermissions(cache, typeSource);
    }
}