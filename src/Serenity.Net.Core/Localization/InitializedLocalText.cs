
namespace Serenity.Localization
{
    /// <summary>
    /// A subclass of LocalText class that contains a initial translation value.
    /// Some classes like NestedLocalTextRegistration uses this type to avoid re-registering
    /// a initialized local text object, when their Initialization method called more than once.
    /// </summary>
    public class InitializedLocalText : LocalText
    {
        private readonly string? initialText;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializedLocalText"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="initialText">The initial text.</param>
        public InitializedLocalText(string key, string? initialText)
            : base(key)
        {
            this.initialText = initialText;
        }

        /// <summary>
        /// Gets the initial text.
        /// </summary>
        /// <value>
        /// The initial text.
        /// </value>
        public string? InitialText => initialText;
    }
}