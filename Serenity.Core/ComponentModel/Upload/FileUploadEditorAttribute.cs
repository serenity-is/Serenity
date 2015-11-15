namespace Serenity.ComponentModel
{
    public class FileUploadEditorAttribute : ImageUploadEditorAttribute
    {
        public FileUploadEditorAttribute()
            : base("ImageUpload")
        {
            AllowNonImage = true;
        }
    }
}