using Serenity.Localization;
using System.Reflection;

namespace Serene.AppServices;

public class TypeSource : DefaultTypeSource
{
    public TypeSource()
        : base(GetAssemblyList())
    {
    }

    private static Assembly[] GetAssemblyList()
    {
        return
        [
            typeof(LocalTextRegistry).Assembly,
            typeof(ISqlConnections).Assembly,
            typeof(IRow).Assembly,
            typeof(SaveRequestHandler<>).Assembly,
            typeof(IDynamicScriptManager).Assembly,
            typeof(EnvironmentSettings).Assembly,
#if (Northwind)
            typeof(Serenity.Demo.Northwind.CustomerPage).Assembly,
#endif
#if (BasicSamples)
            typeof(Serenity.Demo.BasicSamples.BasicSamplesPage).Assembly,
#endif
            typeof(Startup).Assembly
        ];
    }
}