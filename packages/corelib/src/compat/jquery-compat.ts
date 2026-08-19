import { invokeDisposingListeners } from "@serenity-is/domwise";
import { isMobileView } from ".";
import { Fluent, faIcon, getCookie, getjQuery } from "../base";

function applyJQueryUIFixes($: any): boolean {
    if (!$ || !$.ui || !$.ui.dialog || !$.ui.dialog.prototype)
        return false;

    $.ui.dialog.prototype._allowInteraction = function (event: any) {
        if (event.target?.closest?.(".ui-dialog")) {
            return true;
        }
        return !!(event.target?.closest?.(".dropdown-menu, .s-dropdown-menu, .ui-datepicker, .select2-drop, .cke, .cke_dialog, .modal, #support-modal"));
    };

    (function (orig) {
        $.ui.dialog.prototype._focusTabbable = function () {
            if (isMobileView()) {
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
            let events, element, i;
            const cloned = elements;
            for (i = 0; (element = cloned[i]) != null; i++) {
                try {
                    invokeDisposingListeners(element);
                    events = ($ as any)._data(element, "events");
                    if (events && events.disposing) {
                        let handlers = events.disposing;
                        delete events.disposing;
                        for (const x of handlers) {
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
                const token = getCookie('CSRF-TOKEN');
                if (token)
                    xhr.setRequestHeader('X-CSRF-TOKEN', token);
            }
        }
    });
}

/**
 * Applies legacy jQuery compatibility patches to the globally registered jQuery instance.
 *
 * Patches applied (when applicable):
 * - **jQuery UI Dialog fixes** — allows interaction with overlays such as dropdowns,
 *   datepickers, Select2, CKEditor and modals; suppresses tabbable focusing in mobile
 *   view; injects a FontAwesome close icon into the title bar.
 * - **cleanData patch** — invokes {@link invokeDisposingListeners} and any `disposing`
 *   event handlers before the original `$.cleanData`, then polyfills `$.toJSON` /
 *   `$.parseJSON` when missing.
 * - **CSRF token hook** — registers a global `beforeSend` via `$.ajaxSetup` that
 *   attaches the `CSRF-TOKEN` cookie as `X-CSRF-TOKEN` on same-origin requests.
 *
 * This is a compatibility shim for legacy pages that still load jQuery / jQuery UI.
 * New code should avoid a jQuery dependency.
 * @returns `true` if a jQuery instance was found and patches were applied; `false` if no jQuery is registered.
 */
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