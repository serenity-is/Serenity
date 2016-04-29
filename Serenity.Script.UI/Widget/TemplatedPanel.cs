using jQueryApi;
using jQueryApi.UI.Widgets;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Element("<div/>")]
    [IncludeGenericArguments(false), ScriptName("TemplatedPanel")]
    public abstract class TemplatedPanel<TOptions> : TemplatedWidget<TOptions>
        where TOptions : class, new()
    {
        protected bool isPanel;
        protected jQueryValidator validator;
        protected TabsObject tabs;
        protected Toolbar toolbar;

        protected TemplatedPanel(jQueryObject div, TOptions opt = null)
            : base(div, opt)
        {
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

            base.Destroy();
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

        protected virtual void Arrange()
        {
            this.element.Find(".require-layout").Filter(":visible").Each((i, e) =>
            {
                J(e).TriggerHandler("layout");
            });
        }

        protected virtual void InitTabs()
        {
            var tabsDiv = this.ById("Tabs");
            if (tabsDiv.Length == 0)
                return;

            tabs = tabsDiv.Tabs(new TabsOptions());
        }
    }

    [Imported, ScriptName("TemplatedPanel")]
    public abstract class TemplatedPanel : TemplatedPanel<object>
    {
        public TemplatedPanel(jQueryObject container)
            : base(container)
        {
        }
    }
}