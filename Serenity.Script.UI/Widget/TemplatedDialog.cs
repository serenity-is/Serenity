using jQueryApi;
using jQueryApi.UI;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public interface IDialog
    {
        void DialogOpen();
    }

    public abstract class TemplatedDialog<TOptions> : TemplatedWidget<TOptions>, IDialog
        where TOptions : class, new()
    {
        protected bool isPanel;
        protected jQueryValidator validator;
        protected TabsObject tabs;
        protected Toolbar toolbar;

        protected TemplatedDialog(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
            isPanel = this.GetType().GetCustomAttributes(typeof(PanelAttribute), true).Length > 0;
            
            if (!isPanel)
                InitDialog();

            InitValidator();
            InitTabs();
            InitToolbar();
        }

        protected TemplatedDialog(TOptions opt)
            : this(Q.NewBodyDiv(), opt)
        {
        }

        protected TemplatedDialog()
            : this(Q.NewBodyDiv(), null)
        {
        }

        public override void Destroy()
        {
            if (tabs != null)
                tabs.Destroy();

            if (toolbar != null)
            {
                toolbar.Destroy();
                toolbar = null;
            }

            if (validator != null)
            {
                this.ById("Form").Remove();
                validator = null;
            }

            if (!isPanel)
                element.Dialog().Destroy();

            base.Destroy();
        }

        protected virtual void InitDialog()
        {
            element.Dialog(GetDialogOptions());

            if (this.GetType().GetCustomAttributes(typeof(FlexifyAttribute), true).Length > 0)
            {
                element.DialogFlexify();
                element.DialogResizable();
            }

            if (this.GetType().GetCustomAttributes(typeof(MaximizableAttribute), true).Length > 0)
                element.DialogMaximizable();

            var self = this;
            element.Bind("dialogopen." + this.uniqueName, delegate
            {
                self.OnDialogOpen();
            });

            element.Bind("dialogclose." + this.uniqueName, delegate
            {
                self.OnDialogClose();
            });
        }

        protected virtual void InitToolbar()
        {
            var toolbarDiv = this.ById("Toolbar");
            if (toolbarDiv.Length == 0)
                return;

            var opt = new ToolbarOptions
            {
                Buttons = GetToolbarButtons()
            };

            toolbar = new Toolbar(toolbarDiv, opt);
        }

        protected virtual List<ToolButton> GetToolbarButtons()
        {
            return new List<ToolButton>();
        }

        protected virtual jQueryValidatorOptions GetValidatorOptions()
        {
            return new jQueryValidatorOptions();
        }

        protected virtual void InitValidator()
        {
            var form = this.ById("Form");
            if (form.Length > 0)
            {
                var valOptions = GetValidatorOptions();
                validator = form.As<jQueryValidationObject>().Validate(Q.Externals.ValidateOptions(valOptions));
            }
        }

        protected virtual void ResetValidation()
        {
            if (validator != null)
                ((dynamic)validator).resetAll();
        }

        protected virtual bool ValidateForm()
        {
            return validator == null || Q.IsTrue(validator.ValidateForm());
        }

        public void DialogOpen()
        {
            element.Dialog().Open();
        }

        protected virtual void OnDialogOpen()
        {
            J(":input:eq(0)", element).Focus();
            this.Arrange();

            if (tabs != null)
                tabs.Active = 0;
        }

        protected virtual void Arrange()
        {
            this.element.Find(".require-layout").Filter(":visible").Each((i, e) =>
            {
                J(e).TriggerHandler("layout");
            });
        }

        [InlineCode("$.qtip")]
        private object GetQTipPlugin()
        {
            return false;
        }

        protected virtual void OnDialogClose()
        {
            jQuery.Document.Trigger("click"); // for tooltips etc.

            if (GetQTipPlugin() != null)
            {
                J(Document.Body).Children(".qtip").Each(delegate(int index, Element el)
                {
                    ((dynamic)J(el)).qtip("hide");
                });
            }

            var self = this;
            Window.SetTimeout(delegate
            {
                var element = self.element;
                self.Destroy();
                element.Remove();
                Q.PositionToastContainer(false);
            }, 0);
        }

        protected override void AddCssClass()
        {
            if (this.GetType().GetCustomAttributes(typeof(PanelAttribute), true).Length > 0)
                base.AddCssClass();

            // will add css class to ui-dialog container, not content element
        }

        private static bool GetCssSize(jQueryObject element, string name, out int size)
        {
            size = 0;
            var cssSize = element.GetCSS(name);
            if (!Script.IsValue(cssSize))
                return false;

            if (!cssSize.EndsWith("px"))
                return false;

            cssSize = cssSize.Substr(0, cssSize.Length - 2);

            if (!Int32.TryParse(cssSize, out size) || size == 0)
                return false;

            return true;
        }

        private static void ApplyCssSizes(DialogOptions opt, string dialogClass)
        {
            int size;
            var dialog = J("<div/>")
                .Hide()
                .AddClass(dialogClass)
                .AppendTo(Document.Body);

            try
            {
                var sizeHelper = J("<div/>")
                    .AddClass("size")
                    .AppendTo(dialog);
                
                if (GetCssSize(sizeHelper, "minWidth", out size))
                    opt.MinWidth = size;

                if (GetCssSize(sizeHelper, "width", out size))
                    opt.Width = size;

                if (GetCssSize(sizeHelper, "height", out size))
                    opt.Height = size;

                if (GetCssSize(sizeHelper, "minHeight", out size))
                    opt.MinHeight = size;
            }
            finally
            {
                dialog.Remove();
            }
        }

        protected virtual DialogOptions GetDialogOptions()
        {
            DialogOptions opt = new DialogOptions();

            var dialogClass = "s-Dialog " + GetCssClass();
            opt.DialogClass = dialogClass;

            opt.Width = 920;
            ApplyCssSizes(opt, dialogClass);

            opt.AutoOpen = false;
            opt.Resizable = this.GetType().GetCustomAttributes(typeof(ResizableAttribute), true).Length > 0;
            opt.Modal = true;
            opt.Position = new PositionOptions
            {
                My = "center",
                At = "center",
                Of = J(Window.Instance)
            };

            return opt;
        }

        public void DialogClose()
        {
            this.element.Dialog().Close();
        }

        protected virtual void InitTabs()
        {
            var tabsDiv = this.ById("Tabs");
            if (tabsDiv.Length == 0)
                return;

            tabs = tabsDiv.Tabs(new TabsOptions());
        }

        public string IdPrefix { get { return idPrefix; } }
    }

    public abstract class TemplatedDialog : TemplatedDialog<object>
    {
        public TemplatedDialog()
            : base(null)
        {
        }
    }
}