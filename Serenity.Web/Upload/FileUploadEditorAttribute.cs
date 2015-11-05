using Serenity.Web;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class FileUploadEditorAttribute : EditorTypeAttribute
    {
        protected FileUploadEditorAttribute(string type, int minBytes, int maxBytes)
            : base(type)
        {
            MinBytes = minBytes;
            MaxBytes = maxBytes;
        }

        public FileUploadEditorAttribute(int minBytes = 0, int maxBytes = 0, bool allowMultiple = false)
            : base(allowMultiple ? "MultipleImageUpload" : "ImageUpload")
        {
        }

        public override void SetParams(IDictionary<string, object> editorParams)
        {
            base.SetParams(editorParams);

            editorParams["allowNonImage"] = true;
            editorParams["MinBytes"] = MinBytes;
            editorParams["MaxBytes"] = MaxBytes;
        }

        public int MinBytes { get; private set; }
        public int MaxBytes { get; private set; }
    }
}