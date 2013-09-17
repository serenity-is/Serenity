using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static class SubDialogHelper
    {
        [IncludeGenericArguments(false)]
        public static TDialog BindToDataChange<TDialog>(this TDialog dialog, Widget owner, Action dataChange, bool useTimeout = true)
            where TDialog: Widget
        {
            var widgetName = owner.WidgetName;
            dialog.Element.Bind("ondatachange." + widgetName, (e) => {
                if (useTimeout)
                    Window.SetTimeout(dataChange, 0);
                else
                    dataChange();
            }).Bind("remove." + widgetName, delegate
            {
                dialog.Element.Unbind("ondatachange." + widgetName);
            });

            return dialog;
        }

        [IncludeGenericArguments(false)]
        public static TDialog BubbleDataChange<TDialog>(this TDialog dialog, Widget owner, bool useTimeout = true)
            where TDialog : Widget
        {
            return BindToDataChange(dialog, owner, () => owner.Element.TriggerHandler("ondatachange"), useTimeout);
        }

        [IncludeGenericArguments(false)]
        public static TDialog Cascade<TDialog>(this TDialog cascadedDialog, jQueryObject ofElement)
            where TDialog: Widget, IDialog
        {
            cascadedDialog.Element.Dialog().Position = CascadedDialogOffset(ofElement);
            return cascadedDialog;
        }

        public static object CascadedDialogOffset(jQueryObject element)
        {
            return new { my = "left top", at = "left+20 top+20", of = element[0] };
        }
    }
}