using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public interface IDialog
    {
    }

    public abstract class TemplatedDialog<TOptions> : TemplatedWidget<TOptions>, IDialog
        where TOptions : class, new()
    {
        protected jQueryValidator validator;
        protected TabsObject tabs;
        protected Toolbar toolbar;

        protected TemplatedDialog(TOptions opt = null)
            : base(Q.NewBodyDiv(), opt)
        {
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

            element.Dialog().Destroy();
            base.Destroy();
        }

        protected virtual void InitDialog()
        {
            element.Dialog(GetDialogOptions());

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
            Window.SetTimeout((Function)new Action(delegate()
            {
                var element = self.element;
                self.Destroy();
                element.Remove();
            }), 0);
        }

        protected override void AddCssClass()
        {
            // will add css class to ui-dialog container, not content element
        }

        protected virtual DialogOptions GetDialogOptions()
        {
            DialogOptions opt = new DialogOptions();

            opt.DialogClass = "s-Dialog " + "s-" + this.GetType().Name;
            opt.Width = 920;
            opt.AutoOpen = false;
            opt.Resizable = false;
            opt.Modal = true;
            opt.Position = new string[] { "center", "center" };

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