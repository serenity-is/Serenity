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
    }
}
