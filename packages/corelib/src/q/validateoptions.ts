import sQuery from "@optionaldeps/squery";
import { isBS3, localText, notifyError } from "@serenity-is/base";
import { extend } from "./system-compat";
import { baseValidateOptions, getHighlightTarget } from "./validation";

let oldShowLabel: (e: HTMLElement, message: string) => void;

function validateShowLabel(element: HTMLElement, message: string) {
    oldShowLabel.call(this, element, message);
    this.errorsFor(element).each(function (i: number, e: any) {
        var $e = sQuery(e);
        if ($e.parent('.vx').length) {
            $e.attr('title', $e.text());
            if (message && $e.hasClass('error'))
                $e.removeClass('checked');
        }
    });
};

function jQueryValidationInitialization() {

    if (typeof sQuery === "undefined" ||
        !sQuery.validator)
        return;

    let p: any = sQuery.validator;
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
                field = sQuery('#' + vx);
                if (!field.length)
                    field = null;
                else
                    field = field[0];
            }
            if (field == null) {
                field = element.parents('div.field');
                if (field.length) {
                    let inner = sQuery('div.vx', field[0]);
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

            sQuery(validator.errorList.map(x => x.element))
                .closest('.category.collapsed')
                .children('.category-title')
                .each((i, x) => {
                    sQuery(x).click();
                });

            if (validator.errorList.length)
            {
                var el = validator.errorList[0].element;
                if (el) {
                    var bsPaneId = sQuery(el).closest('.tab-content>.tab-pane[id]:not(.active)').attr('id');
                    if (bsPaneId) {
                        let selector = 'a[href="#' + bsPaneId + '"]';
                        sQuery(selector).click(); // bs3/bs4
                        (document.querySelector(selector) as HTMLAnchorElement)?.click(); // bs5+
                    }

                    var uiPaneId = sQuery(el).closest('.ui-tabs-panel[id]:not(.ui-tabs-panel-active)').attr('id');
                    if (uiPaneId)
                        sQuery('a[href="#' + uiPaneId + '"]').click();

                    if ((sQuery.fn as any).tooltip) {
                        var $el: any;
                        var hl = getHighlightTarget(el);
                        if (hl)
                            $el = sQuery(hl);
                        else
                            $el = sQuery(el);

                        (sQuery.fn as any).tooltip && $el.tooltip({
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

if (typeof sQuery !== "undefined") {
    if (sQuery.validator)
        jQueryValidationInitialization();
    else
        sQuery(jQueryValidationInitialization);
}
else if (typeof document !== "undefined") {
    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', jQueryValidationInitialization);
    else
        setTimeout(jQueryValidationInitialization, 100);
}