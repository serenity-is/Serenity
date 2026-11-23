using Serenity.Localization;
using System.Runtime.CompilerServices;

namespace Serene.AppServices;

/// <summary>
/// Regenerates the JSON texts of the projects of this application: the base template of the texts of each project,
/// <c>*.texts.__.json</c>, from the local texts in its assembly, and the other languages from it, keeping the
/// translations. The projects are the ones that <see cref="GetTargets"/> returns, which are the ones that the
/// application has, e.g. the web project itself and the projects of the features that it uses.
/// </summary>
/// <remarks>
/// It is run from the command line, from anywhere in the repository, with <c>dotnet Serene.Web.dll json-texts</c>,
/// or <c>dotnet run --project Serene.Web --no-launch-profile -- json-texts</c>, which exits before the web host
/// is created, so nothing else is started. <c>node --run json-texts</c> at the root runs it from a build in a temporary
/// folder (build/run-web-command.mjs), so it works while the site is running, e.g. in the debugger.
/// </remarks>
public class LocalTextJsonGenerator : BaseJsonTextsGenerator
{
    /// <summary>
    /// Runs the generator and exits the process when the first command line argument is <c>json-texts</c>, which is
    /// before the application starts, so this needs nothing from <c>Program</c>, which is also in the templates.
    /// </summary>
    [ModuleInitializer]
    internal static void RunIfRequested()
    {
        if (Environment.GetCommandLineArgs() is not [_, Command, ..])
            return;

        Environment.Exit(new LocalTextJsonGenerator().RunIf([Command]) ?? 0);
    }

    /// <inheritdoc/>
    protected override IEnumerable<TextsTarget> GetFeatureTargets()
    {
#if (UseProjectRefs)
        // the texts of the framework and of the demo projects that this application uses, which are generated with their
        // own projects when it is built from their source, as a project that uses them as packages has their templates
        const string root = "../../..";
        return
        [
            new(WebTypeSource.SerenityNetCoreAssembly, $"{root}/src/core/texts/resources", "core.texts"),
            new(WebTypeSource.SerenityNetServicesAssembly, $"{root}/src/services/texts/resources", "services.texts"),
            new(WebTypeSource.SerenityNetWebAssembly, $"{root}/src/web/texts/resources", "web.texts"),
            new(ExtensionsTypeSource.SerenityExtensionsAssembly, $"{root}/common-features/src/extensions/texts/resources", "extensions.texts"),
            new(typeof(Serenity.Demo.Northwind.NorthwindModuleAttribute).Assembly, $"{root}/common-features/src/demo.northwind/texts/resources", "northwind.texts"),
            new(typeof(Serenity.Demo.BasicSamples.BasicSamplesHelper).Assembly, $"{root}/common-features/src/demo.basicsamples/texts/resources", "basicsamples.texts")
        ];
#else
        return [];
#endif
    }
}
