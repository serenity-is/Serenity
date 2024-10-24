namespace Serene.AppServices;

public class TypeSource : DefaultTypeSource
{
    public TypeSource()
        : base(GetAssemblyList())
    {
    }

    private static System.Reflection.Assembly[] GetAssemblyList()
    {
        return
        [
            typeof(Serenity.Localization.ILocalText).Assembly, // Serenity.Core
            typeof(DefaultSqlConnections).Assembly, // Serenity.Net.Data
            typeof(Row<>).Assembly, // Serenity.Net.Entity
            typeof(SaveRequestHandler<>).Assembly, // Serenity.Net.Services
            typeof(HttpContextItemsAccessor).Assembly, // Serenity.Net.Web
            typeof(Serenity.Reporting.ExcelExporter).Assembly, // Serenity.Extensions
#if (Northwind)
            typeof(Serenity.Demo.Northwind.CustomerPage).Assembly, // Serenity.Demo.Northwind
#endif
#if (BasicSamples)
            typeof(Serenity.Demo.BasicSamples.BasicSamplesPage).Assembly, // Serenity.Demo.BasicSamples
#endif
            typeof(Startup).Assembly // Serene.Web
        ];
    }
}