namespace Serenity.ComponentModel
{
    public class MultipleFileUploadEditorAttribute : ImageUploadEditorAttribute
    {
        public MultipleFileUploadEditorAttribute()
            : base("MultipleImageUpload")
        {
            AllowNonImage = true;
        }
    }
}