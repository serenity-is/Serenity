using Serenity.Web;

namespace Serenity.Demo.BasicSamples;

/// <summary>
/// Uses <see cref="MVC.Views.BasicSamplesLayout"/> for the module pages of a controller, which show the pages of a
/// sample along with the styles of the samples, e.g. the heading of a sample and its source links.
/// </summary>
public class BasicSamplesLayoutAttribute()
    : ModulePageLayoutAttribute(MVC.Views.BasicSamplesLayout)
{
}
