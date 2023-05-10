namespace Serenity.CodeGeneration;

public class GeneratedSource
{
    public GeneratedSource(string filename, string text, bool module = false)
    {
        Filename = filename;
        Text = text;
        Module = module;
    }

    public string Filename { get; private set; }
    public string Text { get; private set; }
    public bool Module { get; private set; }
}