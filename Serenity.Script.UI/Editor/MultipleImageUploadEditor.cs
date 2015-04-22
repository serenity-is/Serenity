using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("MultipleImage Upload"), OptionsType(typeof(ImageUploadEditorOptions))]
    [Element("<div/>")]
    public class MultipleImageUploadEditor : Widget<ImageUploadEditorOptions>, IGetEditValue, ISetEditValue, IReadOnly
    {
        protected List<UploadedFile> entities;
        protected Toolbar toolbar;
        protected jQueryObject fileSymbols;
        protected jQueryObject uploadInput;

        public MultipleImageUploadEditor(jQueryObject div, ImageUploadEditorOptions opt)
            : base(div, opt)
        {
            entities = new List<UploadedFile>();

            div.AddClass("s-MultipleImageUploadEditor");

            var self = this;

            toolbar = new Toolbar(J("<div/>").AppendTo(this.Element), new ToolbarOptions
            {
                Buttons = GetToolButtons()
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

                    self.entities.Add(newEntity);
                    self.Populate();
                    self.UpdateInterface();
                }
            });

            fileSymbols = jQuery.FromHtml("<ul/>")
                .AppendTo(this.element);

            this.UpdateInterface();
        }

        protected virtual string AddFileButtonText()
        {
            return Q.Text("Controls.ImageUpload.AddFileButton");
        }

        protected virtual List<ToolButton> GetToolButtons()
        {
            var self = this;

            return new List<ToolButton>
            {
                new ToolButton
                {
                    Title = AddFileButtonText(),
                    CssClass = "add-file-button",
                    OnClick = delegate {
                    }
                }
            };
        }

        protected virtual void Populate()
        {
            UploadHelper.PopulateFileSymbols(fileSymbols, entities, true, options.UrlPrefix);
        }

        protected virtual void UpdateInterface()
        {
            var addButton = this.toolbar.FindButton("add-file-button");
            addButton.ToggleClass("disabled", ReadOnly);
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

        public virtual List<UploadedFile> Value
        {
            get
            {
                return this.entities.Select(x => jQuery.ExtendObject(new UploadedFile(), x)).ToList();
            }
            set
            {
                this.entities = (value ?? new List<UploadedFile>()).Select(x => jQuery.ExtendObject(new UploadedFile(), x)).ToList();
                Populate();
                UpdateInterface();
            }
        }

        void IGetEditValue.GetEditValue(PropertyItem property, dynamic target)
        {
            if (JsonEncodeValue)
            {
                target[property.Name] = Q.ToJSON(this.Value);
            }
            else
            {
                target[property.Name] = this.Value;
            }

        }

        void ISetEditValue.SetEditValue(dynamic source, PropertyItem property)
        {
            if (JsonEncodeValue)
            {
                var json = (source[property.Name] as string).TrimToNull() ?? "[]";
                this.Value = jQuery.ParseJson(json).As<List<UploadedFile>>();
            }
            else
                this.Value = source[property.Name];
        }

        [Option]
        public bool JsonEncodeValue { get; set; }
    }
}