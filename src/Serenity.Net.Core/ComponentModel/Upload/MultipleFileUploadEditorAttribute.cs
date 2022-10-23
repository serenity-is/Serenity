namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets editor type to "MultipleImageUpload" while allowing non-image files.
    /// </summary>
    /// <seealso cref="ImageUploadEditorAttribute" />
    public class MultipleFileUploadEditorAttribute : ImageUploadEditorAttribute
    {
        /// <summary>
        /// Editor type key
        /// </summary>
        public new const string Key = "MultipleImageUpload";

        /// <inheritdoc />
        public override bool IsMultiple => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleFileUploadEditorAttribute"/> class.
        /// </summary>
        public MultipleFileUploadEditorAttribute()
            : base(Key)
        {
            AllowNonImage = true;
            JsonEncodeValue = true;
        }
    }
}