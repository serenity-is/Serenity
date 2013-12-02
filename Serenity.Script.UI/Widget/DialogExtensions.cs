using jQueryApi;
using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public static class DialogExtensions
    {
        public static jQueryObject DialogFlexify(this jQueryObject dialog)
        {
            var flexify = new Flexify(dialog.Closest(".ui-dialog"), new FlexifyOptions { });
            return dialog;
        }

        public static jQueryObject DialogResizable(this jQueryObject dialog,
            int? w = null, int? h = null, int? mw = null, int? mh = null)
        {
            var dlg = dialog.Dialog();
            dlg.Resizable = true;
            if (mw != null)
                dlg.MinWidth = mw.Value;
            if (w != null)
                dlg.Width = w.Value;
            if (mh != null)
                dlg.MinHeight = mh.Value;
            if (h != null)
                dlg.Height = h.Value;
            return dialog;
        }

        public static jQueryObject DialogMaximizable(this jQueryObject dialog)
        {
            dialog.As<dynamic>().dialogExtend(new
            {
                closable = true,
                maximizable = true,
                dblclick = "maximize",
                icons = new
                {
                    maximize = "ui-icon-maximize-window"
                }
            });

            return dialog;
        }

        private const int EnterKeyCode = 13;

        public static jQueryObject DialogCloseOnEnter(this jQueryObject dialog)
        {
            dialog.Bind("keydown", delegate(jQueryEvent e)
            {
                if (e.Which != EnterKeyCode)
                    return;

                var tagName = e.Target.TagName.ToLowerCase();

                if (tagName == "button" ||
                    tagName == "select" || // dropdown popup açık olabilir?
                    tagName == "textarea" ||
                    tagName == "input" && (string)e.Target.GetAttribute("type") == "button")
                    return;

                var dlg = jQuery.FromObject(Script.This);
                if (!dlg.HasClass("ui-dialog"))
                    dlg = dlg.Closest(".ui-dialog");

                var buttons = dlg.Children(".ui-dialog-buttonpane")
                    .Find("button");

                if (buttons.Length > 0)
                {
                    var defaultButton = buttons.Find(".default-button");
                    if (defaultButton.Length > 0)
                        buttons = defaultButton;
                }

                var button = buttons.Eq(0);
                if (!button.Is(":disabled"))
                    button.Trigger("click");
            });

            return dialog;
        }
    }
}
