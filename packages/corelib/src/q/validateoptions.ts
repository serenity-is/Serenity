import { Fluent, getjQuery, isArrayLike, isBS3, isBS5Plus, localText, notifyError } from "@serenity-is/base";
import { extend } from "./system-compat";
import { baseValidateOptions, getHighlightTarget } from "./validation";

let oldShowLabel: (e: HTMLElement, message: string) => void;

function validateShowLabel(element: HTMLElement, message: string) {
    oldShowLabel.call(this, element, message);
    this.errorsFor(element).each(function (i: number, e: any) {
        if (e.parent?.hasClass('vx')) {
            e.setAttribute('title', e.textContent);
            if (message && e.hasClass('error'))
                e.removeClass('checked');
        }
    });
};

function jQueryValidationInitialization(): boolean {

    let $ = getjQuery();
    if (!$ || !$.validator)
        return false;

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

    return true;
};

export function validatorAbortHandler(validator: any) {
    validator.settings.abortHandler = null;
    validator.settings.submitHandler = function () {
        return false;
    };
};

export function validateOptions(options?: any) {
    var opt = baseValidateOptions();
    delete opt.showErrors;
    return extend(extend(opt, {
        meta: 'v',
        errorPlacement: function (place: ArrayLike<HTMLElement> | HTMLElement, elem: ArrayLike<HTMLElement> | HTMLElement) {
            var element = isArrayLike(elem) ?  elem[0] : elem;
            let field: HTMLElement = null;
            let vx = element.getAttribute('data-vx-id');
            if (vx) {
                field = document.querySelector('#' + vx);
            }
            if (!field) {
                field = element.closest<HTMLElement>('div.field');
                if (field) {
                    let inner = field.querySelector<HTMLElement>('div.vx');
                    if (inner)
                        field = inner;
                }
                else
                    field = element.parentElement;
            }
            field?.append?.(isArrayLike(place) ? place[0] : place);
        },
        submitHandler: function () {
            return false;
        },
        invalidHandler: function (event: any, validator: any) {
            notifyError(localText("Validation.InvalidFormMessage"));

            validator.errorList.forEach((x: any) => {
                let element: HTMLElement = isArrayLike(x.element) ? x.element[0] : x.element;
                element.closest('.category.collapsed')?.querySelectorAll<HTMLAnchorElement>(
                    ":scope > .category-title")?.forEach(el => el.click());
            });

            if (validator.errorList.length)
            {
                var el = validator.errorList[0].element as HTMLElement;
                if (!el)
                    return;
                let $ = getjQuery();

                var bsPaneId = el.closest('.tab-content>.tab-pane[id]:not(.active)')?.getAttribute('id');
                if (bsPaneId) {
                    let selector = 'a[href="#' + bsPaneId + '"]';
                    $ && $(selector).click(); // bs3/bs4
                    (document.querySelector<HTMLAnchorElement>(selector))?.click(); // bs5+
                }

                var uiPaneId = el.closest('.ui-tabs-panel[id]:not(.ui-tabs-panel-active)')?.getAttribute('id');
                if (uiPaneId) {
                    let selector = 'a[href="#' + uiPaneId + '"]';
                    $ ? $(selector).click() : document.querySelector<HTMLAnchorElement>(selector)?.click();
                }

                var hl = getHighlightTarget(el) ?? el;
                if ($?.fn?.tooltip) {
                    $(hl).tooltip({
                        title: validator.errorList[0].message,
                        trigger: 'manual'
                    }).tooltip('show');

                    window.setTimeout(function () {
                        $(hl).tooltip(isBS3() ? 'destroy' : 'dispose');
                    }, 1500);
                }
                else if (isBS5Plus() && typeof bootstrap !== "undefined" && (bootstrap as any).Tooltip) {
                    let tooltip = new (bootstrap as any).Tooltip(hl, {
                        title: validator.errorList[0].message,
                        trigger: 'manual'
                    });
                    tooltip.show();
                    window.setTimeout(function () {
                        tooltip.dispose();
                    }, 1500);
                }
            }
        },
        success: function (label: ArrayLike<HTMLElement> | HTMLElement) {
            (isArrayLike(label) ? label[0] : label).classList.add('checked');
        }
    }), options);
};

!jQueryValidationInitialization() && Fluent.ready(jQueryValidationInitialization);