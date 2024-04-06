import { Fluent, Tooltip, Validator, ValidatorOptions, getjQuery, isArrayLike, localText, notifyError } from "../base";
import { extend } from "./system-compat";

export function validatorAbortHandler(validator: Validator) {
    delete validator.settings.abortHandler;
    validator.settings.submitHandler = function () {
        return false;
    };
};

export function validateOptions(options?: ValidatorOptions): ValidatorOptions {
    let opt: ValidatorOptions = {};
    return extend(extend(opt, {
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

                new Tooltip(Validator.getHighlightTarget(el) ?? el, { title: validator.errorList[0].message })
                    .show().delayedDispose();
            }
        },
        success: function (label: ArrayLike<HTMLElement> | HTMLElement) {
            label = isArrayLike(label) ? label[0] : label;
            label && label.classList.add('checked');
        }
    }), options);
};


export namespace ValidationHelper {
    export function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        var validator = Validator.getInstance(form);
        var valSettings = validator.settings;
        if (valSettings.abortHandler) {
            return false;
        }
        if (validateBeforeSave != null && validateBeforeSave() === false) {
            return false;
        }
        valSettings.abortHandler = validatorAbortHandler;
        valSettings.submitHandler = function () {
            submitHandler?.();
            return false;
        };
        Fluent.trigger(isArrayLike(form) ? form[0] : form , 'submit');
        return true;
    }

    export function submit(form: ArrayLike<HTMLElement> | HTMLElement,  validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        var validator = Validator.getInstance(form);
        var valSettings = validator.settings;
        if (valSettings.abortHandler) {
            return false;
        }
        if (validateBeforeSave != null && validateBeforeSave() === false) {
            return false;
        }
        if (!validator.form()) {
            return false;
        }
        submitHandler?.();
        return true;
    }

    export function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): Validator {
        return Validator.getInstance(elem);
    }

    export function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void {
        var validator = getValidator(elem);
        if (validator)
            validator.element(isArrayLike(elem) ? elem[0] : elem);
    }
}
