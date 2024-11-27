using Microsoft.AspNetCore.Mvc;

namespace Serenity.Demo.BasicSamples;

[PageAuthorize, Route("BasicSamples/[action]")]
public partial class BasicSamplesPage : Controller
{
}

[Obsolete("Use BasicSamplesPage")]
public abstract class BasicSamplesController : BasicSamplesPage
{
}
