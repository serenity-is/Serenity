namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets editor type to "MultipleImageUpload" which doesn't allow
    /// non-image file types by default.
    /// </summary>
    /// <seealso cref="ImageUploadEditorAttribute" />
    public class MultipleImageUploadEditorAttribute : ImageUploadEditorAttribute
    {
        /// <summary>
        /// Editor type key
        /// </summary>
        public new const string Key = "MultipleImageUpload";
        
        /// <inheritdoc />
        public override bool IsMultiple => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleImageUploadEditorAttribute"/> class.
        /// </summary>
        public MultipleImageUploadEditorAttribute()
            : base(Key)
        {
            JsonEncodeValue = true;
        }
    }
}