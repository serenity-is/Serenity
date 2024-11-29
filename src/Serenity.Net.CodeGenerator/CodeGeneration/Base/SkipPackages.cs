namespace Serenity.CodeGeneration;

public static class SkipPackages
{
    private static readonly string[] NotReferencingSerenity =
    [
        "AspNetCore",
        "Azure",
        "AutoMapper",
        "Bogus",
        "ClosedXML",
        "Dapper",
        "DevExpress",
        "DocumentFormat.",
        "EPPlus",
        "ExcelNumberFormat",
        "FastMember",
        "FluentMigrator",
        "FirebirdSql.",
        "Lucene",
        "MailKit",
        "Mapster",
        "Markdig",
        "MySql",
        "Microsoft.",
        "Newtonsoft.",
        "NetStandard.",
        "Npgsql",
        "Nuglify.",
        "OpenIddict.",
        "Polly",
        "RBush",
        "Serilog.",
        "SixLabors.",
        "StackExchange.",
        "Serenity.Exceptional.",
        "System.",
        "X.PagedList",
        "Yaml",
        "WaffleGenerator"
    ];

    public static bool ForAnnotations(string packageId)
    {
        return NotReferencingSerenity.Any(x => packageId.StartsWith(x, StringComparison.OrdinalIgnoreCase)) ||
            packageId.StartsWith("Serenity.Net.", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(packageId, "Serenity.Assets", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(packageId, "Serenity.Corelib") ||
            string.Equals(packageId, "Serenity.SleekGrid");
    }
}