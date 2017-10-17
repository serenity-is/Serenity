using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public interface IDialog
    {
        void DialogOpen(bool? asPanel);
    }

    [Imported]
    [IncludeGenericArguments(false), ScriptName("TemplatedDialog")]
    public abstract class TemplatedDialog<TOptions> : TemplatedWidget<TOptions>, IDialog
        where TOptions : class, new()
    {
        protected bool responsive;
        protected jQueryValidator validator;
        protected TabsObject tabs;
        protected Toolbar toolbar;

        protected TemplatedDialog(TOptions opt = null)
            : base(Q.NewBodyDiv(), opt)
        {
        }

        public override void Destroy()
        {
        }

        protected virtual void InitDialog()
        {
        }

        protected virtual void InitToolbar()
        {
        }

        protected virtual List<ToolButton> GetToolbarButtons()
        {
            return new List<ToolButton>();
        }

        protected virtual jQueryValidatorOptions GetValidatorOptions()
        {
            return null;
        }

        protected virtual void InitValidator()
        {
        }

        protected virtual void ResetValidation()
        {
        }

        protected virtual bool ValidateForm()
        {
            return true;
        }

        public void DialogOpen(bool? asPanel = null)
        {
        }

        protected virtual void OnDialogOpen()
        {
        }

        protected virtual void Arrange()
        {
        }

        [InlineCode("$.qtip")]
        private object GetQTipPlugin()
        {
            return null;
        }

        protected virtual void OnDialogClose()
        {
        }

        protected override void AddCssClass()
        {
        }

        private static bool GetCssSize(jQueryObject element, string name, out int size)
        {
            size = 0;
            return false;
        }

        private static void ApplyCssSizes(DialogOptions opt, string dialogClass)
        {
        }

        protected virtual DialogOptions GetDialogOptions()
        {
            return null;
        }

        public void DialogClose()
        {
        }

        [IntrinsicProperty]
        public string DialogTitle
        {
            get { return null; }
            set { }
        }

        protected virtual void InitTabs()
        {
        }

        protected void HandleResponsive()
        {
        }
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