import { isMobileView } from "../q";

export function jQueryPatch(jQuery: any) {
    
    function applyJQueryUIFixes(): boolean {
        if (typeof jQuery == "undefined" || !jQuery.ui || !jQuery.ui.dialog || !jQuery.ui.dialog.prototype)
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

    !applyJQueryUIFixes() && typeof jQuery !== "undefined" && jQuery.fn && jQuery(applyJQueryUIFixes);

    if (typeof jQuery !== "undefined" && jQuery.fn) {

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
}