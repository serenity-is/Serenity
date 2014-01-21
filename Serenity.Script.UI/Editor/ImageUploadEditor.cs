using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Resim Yükleme", typeof(ImageUploadEditorOptions))]
    [Element("<div/>")]
    public class ImageUploadEditor : Widget<ImageUploadEditorOptions>, IGetEditValue, ISetEditValue, IReadOnly
    {
        private UploadedFile entity;
        private Toolbar toolbar;
        private jQueryObject symbolDiv;
        private jQueryObject uploadInput;

        public ImageUploadEditor(jQueryObject input, ImageUploadEditorOptions opt)
            : base(input, opt)
        {
            var self = this;

            toolbar = new Toolbar(J("<div/>").AppendTo(this.Element), new ToolbarOptions
            {
                Buttons = new List<ToolButton>
                {
                    new ToolButton {
                        Title = "Dosya Seç",
                        CssClass = "add-file-button",
                        OnClick = delegate {
                        }
                    },
                    new ToolButton {
                        Title = "Kaldır",
                        CssClass = "delete-button",
                        OnClick = delegate {
                            self.entity = null;
                            self.Populate();
                            self.UpdateInterface();
                        }
                    }
                }
            });

            var progress = J("<div><div></div></div>").AddClass("upload-progress")
                .PrependTo(toolbar.Element);

            var addFileButton = toolbar.FindButton("add-file-button");
            uploadInput = UploadHelper.AddUploadInput(new UploadInputOptions
            {
                Container = addFileButton,
                InputName = this.uniqueName,
                Progress = progress,
                FileDone = (response, name, data) =>
                {
                    if (!UploadHelper.CheckImageConstraints(response, options))
                        return;

                    var newEntity = new UploadedFile
                    {
                        Title = name,
                        OriginalFile = name,
                        Filename = response.TemporaryFile
                    };
                    
                    self.entity = newEntity;
                    self.Populate();
                    self.UpdateInterface();
                }
            });

            this.UpdateInterface();
        }

        private void Populate()
        {
        }

        private void UpdateInterface()
        {
            var addButton = this.toolbar.FindButton("add-file-button");
            var delButton = this.toolbar.FindButton("delete-button");
            addButton.ToggleClass("disabled", ReadOnly);
            delButton.ToggleClass("disabled", ReadOnly || entity == null);
        }

        public bool ReadOnly
        {
            get { return !Script.IsNullOrUndefined(uploadInput.GetAttribute("disabled")); }
            set
            {
                if (this.ReadOnly != value)
                {
                    if (value)
                        uploadInput.Attribute("disabled", "disabled");
                    else
                        uploadInput.RemoveAttr("disabled");
                    UpdateInterface();
                }
            }
        }

        public UploadedFile Value
        {
            get
            {
                if (entity == null)
                    return null;

                var copy = jQuery.ExtendObject(new UploadedFile(), this.entity);
                return copy;
            }
            set
            {
                if (value != null)
                    this.entity = jQuery.ExtendObject(new UploadedFile(), value);
                else
                    this.entity = null;

                Populate();
                UpdateInterface();
            }
        }

        void IGetEditValue.GetEditValue(PropertyItem property, dynamic target)
        {
            target[property.Name] = this.Value;
        }

        void ISetEditValue.SetEditValue(dynamic source, PropertyItem property)
        {
        }
    }

    public class UploadedFile
    {
        public string Title { get; set; }
        public string Filename { get; set; }
        public string OriginalFile { get; set; }
    }

    [Serializable, Reflectable]
    public class ImageUploadEditorOptions
    {
        [DisplayName("Minimum Genişlik")]
        public int MinWidth { get; set; }
        [DisplayName("Maksimum Genişlik")]
        public int MaxWidth { get; set; }
        [DisplayName("Minimum Yükseklik")]
        public int MinHeight { get; set; }
        [DisplayName("Maksimum Yükseklik")]
        public int MaxHeight { get; set; }
        [DisplayName("Minimum Boyut")]
        public int MinSize { get; set; }
        [DisplayName("Maksimum Boyut")]
        public int MaxSize { get; set; }
    }

}