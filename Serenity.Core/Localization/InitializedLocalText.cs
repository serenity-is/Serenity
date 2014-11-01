
namespace Serenity.Localization
{
    public class InitializedLocalText : LocalText
    {
        private string initialText;

        public InitializedLocalText(string key, string initialText)
            : base(key)
        {
            this.initialText = initialText;
        }

        public string InitialText
        {
            get { return initialText; }
        }
    }
}