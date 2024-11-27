using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serenity.Navigation;
using Serene.Administration;

namespace Serene.AppServices;

public class NavigationModelFactory : INavigationModelFactory
{
    private readonly ITwoLevelCache cache;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IPermissionService permissions;
    private readonly IServiceProvider serviceProvider;
    private readonly ITypeSource typeSource;
    private readonly IUserAccessor userAccessor;

    public NavigationModelFactory(
        ITwoLevelCache cache,
        IHttpContextAccessor httpContextAccessor,
        IPermissionService permissions,
        IServiceProvider serviceProvider,
        ITypeSource typeSource,
        IUserAccessor userAccessor)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    }

    private string ToAbsolute(string url)
    {
        return url != null && url.StartsWith("~/", StringComparison.Ordinal) ?
            VirtualPathUtility.ToAbsolute(httpContextAccessor.HttpContext, url) : url;
    }

    private static string NormalizeUrl(string url)
    {
        if (url == null)
            return null;

        if (url.Length > 0 && url[^1] == '?')
            url = url[0..^1];

        if (url.IndexOf('?', StringComparison.Ordinal) < 0 &&
            !url.EndsWith("/", StringComparison.Ordinal))
            url += "/";

        return url;
    }

    public INavigationModel Create()
    {
        var model = new NavigationModel
        {
            Items = cache.GetLocalStoreOnly("LeftNavigationModel:NavigationItems:" +
                (userAccessor.User?.GetIdentifier() ?? "-1"), 
                TimeSpan.Zero,
                UserPermissionRow.Fields.GenerationKey,
                () => NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider, ToAbsolute))
        };

        CalcActivePath(model);

        return model;
    }

    private void CalcActivePath(NavigationModel model)
    {
        var currentUrl = httpContextAccessor.HttpContext?.Request.GetEncodedPathAndQuery();

        string bestMatch = null;
        int bestMatchLength = 0;
        NavigationItem bestLink = null;

        foreach (var item in model.Items)
            SearchActivePath(item, currentUrl, ref bestMatch, ref bestMatchLength, ref bestLink);

        model.ActiveItem = bestLink;
    }

    private void SearchActivePath(NavigationItem link, string currentUrl,
        ref string bestMatch, ref int bestMatchLength, ref NavigationItem bestLink)
    {
        if (link == null)
            throw new ArgumentNullException(nameof(link));

        var url = link.Url;

        if (url != null)
        {
            url = ToAbsolute(url);
            url = NormalizeUrl(url);
            if (url.StartsWith(currentUrl, StringComparison.OrdinalIgnoreCase) &&
                string.Compare(NormalizeUrl(url.Split('?')[0]),
                NormalizeUrl(currentUrl.Split('?')[0]), StringComparison.OrdinalIgnoreCase) == 0)
            {
                    if (bestMatchLength == 0 || url.Length < bestMatchLength)
                    {
                        bestMatch = link.FullPath;
                        bestMatchLength = url.Length;
                        bestLink = link;
                    }
            }
        }

        foreach (var child in link.Children)
            SearchActivePath(child, currentUrl, ref bestMatch, ref bestMatchLength, ref bestLink);
    }
}