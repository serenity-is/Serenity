import { isMobileView } from "../q";

export function jQueryPatch($: any) {
    
    function applyJQueryUIFixes(): boolean {
        if (typeof $ == "undefined" || !$.ui || !$.ui.dialog || !$.ui.dialog.prototype)
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
                this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html('<i class="fa fa-times" />');
            }
        })($.ui.dialog.prototype._createTitlebar);
    }

    !applyJQueryUIFixes() && typeof $ !== "undefined" && $.fn && $(applyJQueryUIFixes);

    if (typeof $ !== "undefined" && $.fn) {

        // for backward compatibility
        if (!$.toJSON)
            $.toJSON = JSON.stringify;
        if (!$.parseJSON)
            $.parseJSON = JSON.parse;

        ($ as any).cleanData = (function (orig) {
            return function (elems: any[]) {
                var events, elem, i, e;
                var cloned = elems;
                for (i = 0; (elem = cloned[i]) != null; i++) {
                    try {
                        events = ($ as any)._data(elem, "events");
                        if (events && events.remove) {
                            // html collection might change during remove event, so clone it!
                            if (cloned === elems)
                                cloned = Array.prototype.slice.call(elems);
                            $(elem).triggerHandler("remove");
                            delete events.remove;
                        }
                    } catch (e) { }
                }
                orig(elems);
            };
        })(($ as any).cleanData);
    }
}