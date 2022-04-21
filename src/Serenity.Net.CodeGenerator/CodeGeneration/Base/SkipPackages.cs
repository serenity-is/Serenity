namespace Serenity.CodeGeneration
{
    public static class SkipPackages
    {
        private static readonly string[] NotReferencingSerenity = new[]
        {
            "Dapper",
            "EPPlus",
            "FastMember",
            "FluentMigrator",
            "FirebirdSql.",
            "MailKit",
            "Mapster",
            "MySql",
            "Microsoft.",
            "Newtonsoft.",
            "NetStandard.",
            "Npgsql",
            "Nuglify.",
            "StackExchange.",
            "System.",
            "X.PagedList"
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
}