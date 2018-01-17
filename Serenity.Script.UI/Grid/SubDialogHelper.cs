using jQueryApi;
using jQueryApi.UI;
using jQueryApi.UI.Widgets;
using System;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public static class SubDialogHelper
    {
        [IncludeGenericArguments(false)]
        public static TDialog BindToDataChange<TDialog>(this TDialog dialog, Widget owner, Action<jQueryEvent, DataChangeInfo> dataChange, bool useTimeout = true)
            where TDialog: Widget
        {
            var widgetName = owner.WidgetName;
            dialog.Element.As<dynamic>().bind("ondatachange." + widgetName, new Action<jQueryEvent, DataChangeInfo>((e, dci) => {
                if (useTimeout)
                    Window.SetTimeout(delegate() {
                        dataChange(e, dci);
                    }, 0);
                else
                    dataChange(e, dci);
            })).bind("remove." + widgetName, new Action(delegate
            {
                dialog.Element.Unbind("ondatachange." + widgetName);
            }));

            return dialog;
        }

        [IncludeGenericArguments(false)]
        public static TDialog TriggerDataChange<TDialog>(this TDialog dialog)
            where TDialog : Widget
        {
            dialog.Element.TriggerHandler("ondatachange");
            return dialog;
        }

        [IncludeGenericArguments(false), ScriptName("triggerDataChanged")]
        public static jQueryObject TriggerDataChange(this jQueryObject element)
        {
            element.TriggerHandler("ondatachange");
            return element;
        }

        [IncludeGenericArguments(false)]
        public static TDialog BubbleDataChange<TDialog>(this TDialog dialog, Widget owner, bool useTimeout = true)
            where TDialog : Widget
        {
            return BindToDataChange(dialog, owner, (e, dci) => owner.Element.TriggerHandler("ondatachange"), useTimeout);
        }

        [IncludeGenericArguments(false)]
        public static TDialog Cascade<TDialog>(this TDialog cascadedDialog, jQueryObject ofElement)
            where TDialog : Widget, IDialog
        {
            cascadedDialog.Element.One("dialogopen", e => {
                cascadedDialog.Element.Dialog().Position = CascadedDialogOffset(ofElement);
            });

            return cascadedDialog;
        }

        public static object CascadedDialogOffset(jQueryObject element)
        {
            return new PositionOptions { My = "left top", At = "left+20 top+20", Of = element[0] };
        }
    }
}