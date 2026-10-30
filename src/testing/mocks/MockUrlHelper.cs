namespace Serenity.Testing;

public class MockUrlHelper(Func<string, bool> isLocalUri) : IUrlHelper
{
    public string Action(UrlActionContext actionContext)
    {
        throw new NotImplementedException();
    }

    public string Content(string contentPath)
    {
        throw new NotImplementedException();
    }

    public bool IsLocalUrl(string url)
    {
        return isLocalUri(url);
    }

    public string RouteUrl(UrlRouteContext routeContext)
    {
        throw new NotImplementedException();
    }

    public string Link(string routeName, object values)
    {
        throw new NotImplementedException();
    }

    public ActionContext ActionContext { get; }
}