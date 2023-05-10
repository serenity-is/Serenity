import { extend } from "./system";
import { notifyError } from "./notify";
import { localText } from "./localtext";
import { baseValidateOptions, getHighlightTarget } from "./validation";
import { isBS3 } from "./dialogs";

let oldShowLabel: (e: HTMLElement, message: string) => void;

function validateShowLabel(element: HTMLElement, message: string) {
    oldShowLabel.call(this, element, message);
    this.errorsFor(element).each(function (i: number, e: any) {
        var $e = $(e);
        if ($e.parent('.vx').length) {
            $e.attr('title', $e.text());
            if (message && $e.hasClass('error'))
                $e.removeClass('checked');
        }
    });
};

function jQueryValidationInitialization() {

    if (typeof $ === "undefined" ||
        !$.validator)
        return;

    let p: any = $.validator;
    p = p.prototype;
    oldShowLabel = p.showLabel;
    p.showLabel = validateShowLabel;

    p.oldfocusInvalid = p.focusInvalid;
    p.focusInvalid = function () {
        if (this.settings.abortHandler)
            this.settings.abortHandler(this);
        this.oldfocusInvalid.call(this);
    };
    p.oldstopRequest = p.focusInvalid;
    p.stopRequest = function (element: any, valid: boolean) {
        let formSubmitted = this.formSubmitted;
        this.oldfocusInvalid.call(this, [element, valid]);
        if (!valid && this.pendingRequest == 0 && formSubmitted && this.settings.abortHandler) {
            this.settings.abortHandler(this);
        }
    };
    p.resetAll = function () {
        this.submitted = {};
        this.prepareForm();
        this.hideErrors();
        this.elements().removeClass(this.settings.errorClass);
    };
};

export function validatorAbortHandler(validator: any) {
    validator.settings.abortHandler = null;
    validator.settings.submitHandler = function () {
        return false;
    };
};

export function validateOptions(options?: JQueryValidation.ValidationOptions) {
    var opt = baseValidateOptions();
    delete opt.showErrors;
    return extend(extend(opt, {
        meta: 'v',
        errorPlacement: function (error: any, element: any) {
            let field: any = null;
            let vx = element.attr('data-vx-id');
            if (vx) {
                field = $('#' + vx);
                if (!field.length)
                    field = null;
                else
                    field = field[0];
            }
            if (field == null) {
                field = element.parents('div.field');
                if (field.length) {
                    let inner = $('div.vx', field[0]);
                    if (inner.length)
                        field = inner[0];
                }
                else
                    field = element.parent();
            }
            error.appendTo(field);
        },
        submitHandler: function () {
            return false;
        },
        invalidHandler: function (event: any, validator: JQueryValidation.Validator) {
            notifyError(localText("Validation.InvalidFormMessage"));

            $(validator.errorList.map(x => x.element))
                .closest('.category.collapsed')
                .children('.category-title')
                .each((i, x) => {
                    $(x).click();
                    return true;
                });

            if (validator.errorList.length)
            {
                var el = validator.errorList[0].element;
                if (el) {
                    var bsPane = $(el).closest('.tab-pane');
                    if (!bsPane.hasClass("active") &&
                        bsPane.parent().hasClass('tab-content')) {
                        var bsPaneId = bsPane.attr('id');
                        if (bsPaneId) {
                            $('a[href="#' + bsPaneId + '"]').click();
                        }
                    }

                    if (($.fn as any).tooltip) {
                        var $el: any;
                        var hl = getHighlightTarget(el);
                        if (hl)
                            $el = $(hl);
                        else
                            $el = $(el);

                        ($.fn as any).tooltip && $el.tooltip({
                            title: validator.errorList[0].message,
                            trigger: 'manual'
                        }).tooltip('show');

                        window.setTimeout(function () {
                            $el.tooltip(isBS3() ? 'destroy' : 'dispose');
                        }, 1500);
                    }
                }
            }
        },
        success: function (label: JQuery) {
            label.addClass('checked');
        }
    }), options);
};

if (typeof $ !== "undefined") {
    if ($.validator)
        jQueryValidationInitialization();
    else
        $(jQueryValidationInitialization);
}
else if (typeof document !== "undefined") {
    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', jQueryValidationInitialization);
    else
        setTimeout(jQueryValidationInitialization, 100);
}