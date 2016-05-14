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

    [Imported]
    [IncludeGenericArguments(false), ScriptName("TemplatedDialog")]
    public abstract class TemplatedDialog<TOptions> : TemplatedWidget<TOptions>, IDialog
        where TOptions : class, new()
    {
        protected bool isPanel;
        protected bool responsive;
        protected jQueryValidator validator;
        protected TabsObject tabs;
        protected Toolbar toolbar;

        protected TemplatedDialog(TOptions opt = null)
            : base(Q.NewBodyDiv(), opt)
        {
            isPanel = this.GetType().GetCustomAttributes(typeof(PanelAttribute), true).Length > 0;
            
            if (!isPanel)
                InitDialog();

            InitValidator();
            InitTabs();
            InitToolbar();
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

            jQuery.Window.Unbind("." + this.uniqueName);

            base.Destroy();
        }

        protected virtual void InitDialog()
        {
            element.Dialog(GetDialogOptions());

            responsive = Q.Config.ResponsiveDialogs || this.GetType().GetCustomAttributes(
                typeof(ResponsiveAttribute), true).Length > 0;

            if (responsive)
            {
                element.DialogResizable();

                jQuery.Window.Bind("resize." + this.uniqueName, e => {
                    if (this.Element != null && this.Element.Is(":visible"))
                        HandleResponsive();
                });

                this.Element.Closest(".ui-dialog").AddClass("flex-layout");
            }
            else if (this.GetType().GetCustomAttributes(
                typeof(FlexifyAttribute), true).Length > 0)
            {
                element.DialogFlexify();
                element.DialogResizable();
            }

            if (this.GetType().GetCustomAttributes(typeof(MaximizableAttribute), true).Length > 0)
                element.DialogMaximizable();

            var self = this;
            element.Bind("dialogopen." + this.uniqueName, delegate
            {
                J(Document.Body).AddClass("modal-dialog-open");

                if (responsive)
                    HandleResponsive();

                self.OnDialogOpen();
            });

            element.Bind("dialogclose." + this.uniqueName, delegate
            {
                J(Document.Body).ToggleClass("modal-dialog-open", 
                    jQuery.Select(".ui-dialog:visible").Length > 0);

                self.OnDialogClose();
            });
        }

        protected virtual void InitToolbar()
        {
            var toolbarDiv = this.ById("Toolbar");
            if (toolbarDiv.Length == 0)
                return;

            var hotkeyContext = this.Element.Closest(".ui-dialog");
            if (hotkeyContext.Length == 0)
                hotkeyContext = this.element;

            var opt = new ToolbarOptions
            {
                Buttons = GetToolbarButtons(),
                HotkeyContext = hotkeyContext[0]
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
            if (isPanel)
                return;

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
            if (isPanel)
                return;

            this.element.Dialog().Close();
        }

        [IntrinsicProperty]
        public string DialogTitle
        {
            get { if (isPanel) return null; return element.Dialog().Option("title") as string; }
            set { if (isPanel) return; element.Dialog().Option("title", value); }
        }

        protected virtual void InitTabs()
        {
            var tabsDiv = this.ById("Tabs");
            if (tabsDiv.Length == 0)
                return;

            tabs = tabsDiv.Tabs(new TabsOptions());
        }

        protected void HandleResponsive()
        {
            var dlg = this.Element.Dialog();
            var uiDialog = this.Element.Closest(".ui-dialog");

            if (J(Document.Body).HasClass("mobile-device"))
            {
                var data = this.Element.GetDataValue("responsiveData").As<ResponsiveData>();
                if (data == null)
                {
                    data = new ResponsiveData();
                    data.draggable = dlg.Draggable;
                    data.resizable = dlg.Resizable;
                    var pos = uiDialog.Position();
                    data.left = pos.Left;
                    data.top = pos.Top;
                    data.width = uiDialog.GetWidth();
                    data.height = uiDialog.GetHeight();
                    data.contentHeight = this.Element.GetHeight();
                    this.Element.Data("responsiveData", data);

                    dlg.Draggable = false;
                    dlg.Resizable = false;
                }

                uiDialog.AddClass("mobile-layout");
                uiDialog.CSS(new
                {
                    left = "0px",
                    top = "0px",
                    width = jQuery.Window.GetWidth() + "px",
                    height = jQuery.Window.GetHeight() + "px",
                }.As<JsDictionary<string, object>>());

                J(Document.Body).ScrollTop(0);
                Q.LayoutFillHeight(this.Element);
            }
            else
            {
                var data = this.Element.GetDataValue("responsiveData").As<ResponsiveData>();
                if (data != null)
                {
                    dlg.Draggable = data.draggable;
                    dlg.Resizable = data.resizable;
                    this.Element.Closest(".ui-dialog").CSS(new
                    {
                        left = "0px",
                        top = "0px",
                        width = data.width + "px",
                        height = data.height + "px"
                    }.As<JsDictionary<string, object>>());

                    this.element.Height(data.contentHeight);

                    uiDialog.RemoveClass("mobile-layout");
                    this.element.RemoveData("responsiveData");
                }
            }
        }
    }

    [Imported, Serializable]
    internal class ResponsiveData
    {
        public bool draggable;
        public bool resizable;
        public int width;
        public int height;
        public int left;
        public int top;
        public int contentHeight;
    }

    [Imported, ScriptName("TemplatedDialog")]
    public abstract class TemplatedDialog : TemplatedDialog<object>
    {
        public TemplatedDialog()
            : base(null)
        {
        }
    }
}