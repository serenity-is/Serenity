import { isArrayLike, Tooltip, Validator } from "@serenity-is/base";
import { extend } from "./system-compat";

function getHighlightTarget(el: HTMLElement) {
    var hl = el.dataset.vxHighlight;
    if (hl)
        return document.getElementById(hl);
    else if (el.classList.contains("select2-offscreen") && el.id)
        return document.getElementById('s2id_' + el.id);
}

export function baseValidateOptions(): any {
    return {
        showErrors: function() {
            var error: any, hl: any;
            for (var i = 0; this.errorList[i]; i++) {
                error = this.errorList[i];
                hl = getHighlightTarget(error.element) ?? error.element;
                if (i) {
                    Tooltip.getInstance(hl)?.setTitle('').hide().dispose();
                }
                else
                    new Tooltip(hl).setTitle(error.message).show().delayedHide();
            }

            var valids = this.validElements();
            for (var element of valids) {
                hl = getHighlightTarget(element) ?? element;
                Tooltip.getInstance(hl)?.setTitle('').hide().dispose();
            }

        }
    };
}

export function validateForm(form: HTMLElement | ArrayLike<HTMLElement>, opt: any): any {
    return new Validator(isArrayLike(form) ? form[0] as HTMLFormElement : form as HTMLFormElement, extend(baseValidateOptions(), opt));
}