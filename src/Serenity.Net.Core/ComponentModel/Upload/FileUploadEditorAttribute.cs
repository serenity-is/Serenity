namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets editor type as "ImageUpload" (single), while allowing non-image files.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.ImageUploadEditorAttribute" />
    public class FileUploadEditorAttribute : ImageUploadEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadEditorAttribute"/> class.
        /// </summary>
        public FileUploadEditorAttribute()
            : base("ImageUpload")
        {
            AllowNonImage = true;
        }
    }
}