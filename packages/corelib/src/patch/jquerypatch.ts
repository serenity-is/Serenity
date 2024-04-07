import { faIcon, getCookie, getTypeFullName, getjQuery } from "../base";
import { isMobileView } from "../q";
import { getWidgetFrom, tryGetWidget } from "../ui/widgets/widgetutils";

function applyGetWidgetExtensions($: any) {
    if (!$ || !$.fn)
        return;

    $.fn.tryGetWidget = function tryGetWidget$<TWidget>(this: ArrayLike<HTMLElement>, type?: { new(...args: any[]): TWidget }): TWidget {
        return tryGetWidget(this[0], type);
    }

    $.fn.getWidget = function getWidget$<TWidget>(this: ArrayLike<HTMLElement>, type?: { new(...args: any[]): TWidget }): TWidget {
        if (!this?.length)
            throw new Error(`Searching for widget of type '${getTypeFullName(type)}' on a non-existent element! (${(this as any)?.selector})`);

        return getWidgetFrom(this[0], type);
    };
}

function applyJQueryUIFixes($: any): boolean {
    if (!$ || !$.ui || !$.ui.dialog || !$.ui.dialog.prototype)
        return false;

    $.ui.dialog.prototype._allowInteraction = function (event: any) {
        if ($(event.target).closest(".ui-dialog").length) {
            return true;
        }
        return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, .modal, #support-modal").length;
    };

    (function (orig) {
        $.ui.dialog.prototype._focusTabbable = function () {
            if (isMobileView) {
                this.uiDialog && this.uiDialog.focus();
                return;
            }
            orig.call(this);
        }
    })($.ui.dialog.prototype._focusTabbable);

    (function (orig) {
        $.ui.dialog.prototype._createTitlebar = function () {
            orig.call(this);
            this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html(`<i class="${faIcon("times")}" />`);
        }
    })($.ui.dialog.prototype._createTitlebar);
}

function applyCleanDataPatch($: any) {
    if (!$ || !$.fn || $.isMock)
        return;

    // for backward compatibility
    if (!$.toJSON)
        $.toJSON = JSON.stringify;
    if (!$.parseJSON)
        $.parseJSON = JSON.parse;

    $.cleanData = (function (orig) {
        return function (elems: any[]) {
            var events, elem, i, e;
            var cloned = elems;
            for (i = 0; (elem = cloned[i]) != null; i++) {
                try {
                    events = ($ as any)._data(elem, "events");
                    if (events && events.remove) {
                        let remove = events.remove;
                        delete events.remove;
                        for (var x of remove) {
                            if (typeof x?.handler === "function") {
                                try {
                                    x.handler.call(elem, ({ target: elem }));
                                }
                                catch {
                                }
                            }
                        }
                    }
                } catch (e) { }
            }
            orig(elems);
        };
    })(($ as any).cleanData);
}

function applyAjaxCSRFToken($: any) {
    $?.ajaxSetup?.({
        beforeSend: function (xhr: XMLHttpRequest, opt: any) {
            if (!opt || !opt.crossDomain) {
                var token = getCookie('CSRF-TOKEN');
                if (token)
                    xhr.setRequestHeader('X-CSRF-TOKEN', token);
            }
        }
    });
}

export function jQueryPatch(): boolean {
    let $ = getjQuery();
    if (!$)
        return false;
    applyJQueryUIFixes($);
    applyCleanDataPatch($);
    applyGetWidgetExtensions($);
    applyAjaxCSRFToken($);
    return true;
}