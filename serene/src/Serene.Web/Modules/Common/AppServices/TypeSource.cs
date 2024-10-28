namespace Serene.AppServices;

public class TypeSource() : ExtensionsTypeSource([
#if (Northwind)
    typeof(Serenity.Demo.Northwind.CustomerPage).Assembly, // Serenity.Demo.Northwind
#endif
#if (BasicSamples)
    typeof(Serenity.Demo.BasicSamples.BasicSamplesPage).Assembly, // Serenity.Demo.BasicSamples
#endif
    typeof(Startup).Assembly // Serene.Web
])
{
}