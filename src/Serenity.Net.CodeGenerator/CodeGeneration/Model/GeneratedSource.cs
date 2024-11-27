namespace Serenity.CodeGeneration;

public class GeneratedSource(string filename, string text)
{
    public string Filename { get; private set; } = filename;
    public string Text { get; private set; } = text;

    public bool IsTypeScript =>
        Filename.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) ||
        Filename.EndsWith(".tsx", StringComparison.OrdinalIgnoreCase);
}