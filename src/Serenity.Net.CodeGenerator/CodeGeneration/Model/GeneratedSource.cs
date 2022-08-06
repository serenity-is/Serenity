namespace Serenity.CodeGeneration
{
    public class GeneratedSource
    {
        public GeneratedSource(string text, bool module = false)
        {
            Text = text;
            Module = module;
        }

        public string Text { get; private set; }
        public bool Module { get; private set; }
    }
}