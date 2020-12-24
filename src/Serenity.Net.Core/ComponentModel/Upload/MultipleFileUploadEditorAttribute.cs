namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets editor type to "MultipleImageUpload" while allowing non-image files.
    /// </summary>
    /// <seealso cref="ImageUploadEditorAttribute" />
    public class MultipleFileUploadEditorAttribute : ImageUploadEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleFileUploadEditorAttribute"/> class.
        /// </summary>
        public MultipleFileUploadEditorAttribute()
            : base("MultipleImageUpload")
        {
            AllowNonImage = true;
            JsonEncodeValue = true;
        }
    }
}