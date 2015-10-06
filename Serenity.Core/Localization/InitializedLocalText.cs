
namespace Serenity.Localization
{
    /// <summary>
    /// A subclass of LocalText class that contains a initial translation value.
    /// Some classes like NestedLocalTextRegistration uses this type to avoid re-registering
    /// a initialized local text object, when their Initialization method called more than once.
    /// </summary>
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