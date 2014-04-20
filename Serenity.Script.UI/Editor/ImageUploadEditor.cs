using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Resim Yükleme"), OptionsType(typeof(ImageUploadEditorOptions))]
    [Element("<div/>")]
    public class ImageUploadEditor : Widget<ImageUploadEditorOptions>, IGetEditValue, ISetEditValue, IReadOnly
    {
        private UploadedFile entity;
        private Toolbar toolbar;
        private jQueryObject fileSymbols;
        private jQueryObject uploadInput;

        public ImageUploadEditor(jQueryObject div, ImageUploadEditorOptions opt)
            : base(div, opt)
        {
            div.AddClass("s-ImageUploadEditor");

            if (options.OriginalNameProperty.IsEmptyOrNull())
                div.AddClass("hide-original-name");

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
                        OriginalName = name,
                        Filename = response.TemporaryFile
                    };
                    
                    self.entity = newEntity;
                    self.Populate();
                    self.UpdateInterface();
                }
            });

            fileSymbols = jQuery.FromHtml("<ul/>")
                .AppendTo(this.element);

            this.UpdateInterface();
        }

        private void Populate()
        {
            bool displayOriginalName = !options.OriginalNameProperty.IsTrimmedEmpty();

            if (entity == null)
                UploadHelper.PopulateFileSymbols(fileSymbols, null, displayOriginalName);
            else
                UploadHelper.PopulateFileSymbols(fileSymbols, new List<UploadedFile> { entity }, displayOriginalName);
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
                {
                    if (Script.IsNullOrUndefined(value.Filename))
                        this.entity = null;
                    else
                        this.entity = jQuery.ExtendObject(new UploadedFile(), value);
                }
                else
                    this.entity = null;

                Populate();
                UpdateInterface();
            }
        }

        void IGetEditValue.GetEditValue(PropertyItem property, dynamic target)
        {
            target[property.Name] = this.entity == null ? null : this.entity.Filename.TrimToNull();
        }

        void ISetEditValue.SetEditValue(dynamic source, PropertyItem property)
        {
            var value = new UploadedFile();
            value.Filename = source[property.Name];
            value.OriginalName = source[options.OriginalNameProperty];
            this.Value = value;
        }
    }

    public class UploadedFile
    {
        public string Filename { get; set; }
        public string OriginalName { get; set; }
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
        [DisplayName("Orjinal Dosya Adı Alanı")]
        public string OriginalNameProperty { get; set; }
    }
}