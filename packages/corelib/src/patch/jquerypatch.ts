import { getInstanceType, getTypeFullName, isAssignableFrom, notifyError, stringFormat } from "@serenity-is/base";
import { isMobileView } from "../q";

function applyGetWidgetExtensions(jQuery: any, widgetType: { getWidgetName(type: any): string }) {
    if (!widgetType || !jQuery || !jQuery.fn || jQuery.isMock)
        return;

    jQuery.fn.tryGetWidget = function tryGetWidget(this: JQuery, type?: any) {
        var element = this;
        var w;
        type ??= widgetType;
        if (isAssignableFrom(widgetType, type)) {
            var widgetName = widgetType.getWidgetName(type);
            w = element.data(widgetName);
            if (w != null && !isAssignableFrom(type, getInstanceType(w))) {
                w = null;
            }
            if (w != null) {
                return w;
            }
        }

        var data = element.data();
        if (data == null) {
            return null;
        }

        for (var key of Object.keys(data)) {
            w = data[key];
            if (w != null && isAssignableFrom(type, getInstanceType(w))) {
                return w;
            }
        }

        return null;
    }

    jQuery.fn.getWidget = function getWidget<TWidget>(this: JQuery, type: { new(...args: any[]): TWidget }) {
        if (this == null)
            throw new Error("element argument is required for getting widgets!");

        if (this.length === 0) {
            throw new Error(stringFormat("Searching for widget of type '{0}' on a non-existent element! ({1})",
                getTypeFullName(type), (this as any).selector));
        }

        var w = (this as any).tryGetWidget(type);
        if (w == null) {
            var message = stringFormat("Element has no widget of type '{0}'! If you have recently changed " +
                "editor type of a property in a form class, or changed data type in row (which also changes " +
                "editor type) your script side Form definition might be out of date. Make sure your project " +
                "builds successfully and transform T4 templates", getTypeFullName(type));
            notifyError(message, '', null);
            throw new Error(message);
        }
        return w;
    };
}

function applyJQueryUIFixes(jQuery: any): boolean {
    if (!jQuery || !jQuery.ui || !jQuery.ui.dialog || !jQuery.ui.dialog.prototype)
        return false;

    jQuery.ui.dialog.prototype._allowInteraction = function (event: any) {
        if (jQuery(event.target).closest(".ui-dialog").length) {
            return true;
        }
        return !!jQuery(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, .modal, #support-modal").length;
    };

    (function (orig) {
        jQuery.ui.dialog.prototype._focusTabbable = function () {
            if (isMobileView) {
                this.uiDialog && this.uiDialog.focus();
                return;
            }
            orig.call(this);
        }
    })(jQuery.ui.dialog.prototype._focusTabbable);

    (function (orig) {
        jQuery.ui.dialog.prototype._createTitlebar = function () {
            orig.call(this);
            this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html('<i class="fa fa-times" />');
        }
    })(jQuery.ui.dialog.prototype._createTitlebar);
}

function applyCleanDataPatch(jQuery: any) {
    if (!jQuery || !jQuery.fn || jQuery.isMock)
        return;

    // for backward compatibility
    if (!jQuery.toJSON)
        jQuery.toJSON = JSON.stringify;
    if (!jQuery.parseJSON)
        jQuery.parseJSON = JSON.parse;

    (jQuery as any).cleanData = (function (orig) {
        return function (elems: any[]) {
            var events, elem, i, e;
            var cloned = elems;
            for (i = 0; (elem = cloned[i]) != null; i++) {
                try {
                    events = (jQuery as any)._data(elem, "events");
                    if (events && events.remove) {
                        // html collection might change during remove event, so clone it!
                        if (cloned === elems)
                            cloned = Array.prototype.slice.call(elems);
                        jQuery(elem).triggerHandler("remove");
                        delete events.remove;
                    }
                } catch (e) { }
            }
            orig(elems);
        };
    })((jQuery as any).cleanData);
}

export function jQueryPatch(jQuery: any, widgetType: { getWidgetName(type: any): string }) {
    !applyJQueryUIFixes(jQuery) && jQuery && jQuery(applyJQueryUIFixes);
    applyCleanDataPatch(jQuery);
    applyGetWidgetExtensions(jQuery, widgetType);
}