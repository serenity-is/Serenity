namespace Serenity.CodeGeneration;

public static class SkipPackages
{
    private static readonly string[] NotReferencingSerenity = new[]
    {
        "AspNetCore",
        "Azure",
        "AutoMapper",
        "Dapper",
        "DevExpress",
        "EPPlus",
        "FastMember",
        "FluentMigrator",
        "FirebirdSql.",
        "Lucene",
        "MailKit",
        "Mapster",
        "MySql",
        "Microsoft.",
        "Newtonsoft.",
        "NetStandard.",
        "Npgsql",
        "Nuglify.",
        "Serilog.",
        "StackExchange.",
        "System.",
        "X.PagedList",
        "Yaml"
    };

    public static bool ForRestore(string packageId)
    {
        return NotReferencingSerenity.Any(x => packageId.StartsWith(x, StringComparison.OrdinalIgnoreCase)) ||
            packageId.StartsWith("Serenity.Net.", StringComparison.OrdinalIgnoreCase);
    }

    public static bool ForAnnotations(string packageId)
    {
        return NotReferencingSerenity.Any(x => packageId.StartsWith(x, StringComparison.OrdinalIgnoreCase)) ||
            packageId.StartsWith("Serenity.Net.", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(packageId, "Serenity.Assets", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(packageId, "Serenity.Scripts");
    }
}