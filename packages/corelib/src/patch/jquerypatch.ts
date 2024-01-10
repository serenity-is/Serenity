import { Fluent, faIcon, getTypeFullName, getjQuery } from "@serenity-is/base";
import { isMobileView } from "../q";
import { getWidgetFrom, tryGetWidget } from "../ui/widgets/widgetutils";

function applyGetWidgetExtensions(jQuery: any) {
    if (!jQuery || !jQuery.fn)
        return;

    jQuery.fn.tryGetWidget = function tryGetWidget$<TWidget>(this: ArrayLike<HTMLElement>, type?: { new (...args: any[]): TWidget }): TWidget {
        return tryGetWidget(this[0], type);
    }

    jQuery.fn.getWidget = function getWidget$<TWidget>(this: ArrayLike<HTMLElement>,  type?: { new (...args: any[]): TWidget }): TWidget {
        if (!this?.length)
            throw new Error(`Searching for widget of type '${getTypeFullName(type)}' on a non-existent element! (${(this as any)?.selector})`);

        return getWidgetFrom(this[0], type);
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
            this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html(`<i class="${faIcon("times")}" />`);
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
                        Fluent.trigger(elem, "remove", { bubbles: false});
                        delete events.remove;
                    }
                } catch (e) { }
            }
            orig(elems);
        };
    })((jQuery as any).cleanData);
}

export function jQueryPatch(): boolean {
    let $ = getjQuery();
    if (!$)
        return false;
    applyJQueryUIFixes($);
    applyCleanDataPatch($);
    applyGetWidgetExtensions($);
    return true;
}