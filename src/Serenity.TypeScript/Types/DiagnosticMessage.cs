namespace Serenity.TypeScript;

public class DiagnosticMessage
{
    public string Key { get; set; }
    public DiagnosticCategory Category { get; set; }
    public int Code { get; set; }
    public string Message { get; set; }
    public Dictionary<string, bool> ReportsUnnecessary { get; set; }
    public Dictionary<string, bool> ReportsDeprecated { get; set; }
    internal bool? ElidedInCompatabilityPyramid { get; set; }
}