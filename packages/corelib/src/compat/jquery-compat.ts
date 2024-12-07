import { Fluent, faIcon, getCookie, getTypeFullName, getjQuery } from "../base";
import { isMobileView } from ".";

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
        return function (elements: any[]) {
            var events, element, i;
            var cloned = elements;
            for (i = 0; (element = cloned[i]) != null; i++) {
                try {
                    events = ($ as any)._data(element, "events");
                    if (events && events.disposing) {
                        let handlers = events.disposing;
                        delete events.disposing;
                        for (var x of handlers) {
                            if (x && typeof x.handler === "function") {
                                try {
                                    x.handler.call(element, ({ target: element }));
                                }
                                catch {
                                }
                            }
                        }
                    }
                } catch (e) { }
            }
            orig(elements);
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
    applyAjaxCSRFToken($);
    return true;
}


!jQueryPatch() && Fluent.ready(jQueryPatch);