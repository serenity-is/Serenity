namespace Serenity.ComponentModel
{
    public class MultipleImageUploadEditorAttribute : ImageUploadEditorAttribute
    {
        public MultipleImageUploadEditorAttribute()
            : base("MultipleImageUpload")
        {
            JsonEncodeValue = true;
        }
    }
}