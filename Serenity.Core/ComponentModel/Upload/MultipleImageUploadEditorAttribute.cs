namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets editor type to "MultipleImageUpload" which doesn't allow
    /// non-image file types by default.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.ImageUploadEditorAttribute" />
    public class MultipleImageUploadEditorAttribute : ImageUploadEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleImageUploadEditorAttribute"/> class.
        /// </summary>
        public MultipleImageUploadEditorAttribute()
            : base("MultipleImageUpload")
        {
            JsonEncodeValue = true;
        }
    }
}