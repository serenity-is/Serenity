namespace Serenity.TypeScript;

public class Diagnostic
{
    public SourceFile File { get; set; }
    public string FileName { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DiagnosticMessage Message { get; set; }
    public object Argument { get; set; }
}