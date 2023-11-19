namespace Serenity.CodeGeneration;

public class GeneratedSource(string filename, string text, bool module = false)
{
    public string Filename { get; private set; } = filename;
    public string Text { get; private set; } = text;
    public bool Module { get; private set; } = module;
}