import { Tooltip, getjQuery, isArrayLike, localText, notifyError } from "@serenity-is/base";
import { extend } from "./system-compat";
import { baseValidateOptions } from "./validation";

function getHighlightTarget(el: HTMLElement) {
    var hl = el.dataset.vxHighlight;
    if (hl)
        return document.getElementById(hl);
    else if (el.classList.contains("select2-offscreen") && el.id)
        return document.getElementById('s2id_' + el.id);
}

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

                new Tooltip(getHighlightTarget(el) ?? el, { title: validator.errorList[0].message })
                    .show().delayedDispose();
            }
        },
        success: function (label: ArrayLike<HTMLElement> | HTMLElement) {
            label = isArrayLike(label) ? label[0] : label;
            label && label.classList.add('checked');
        }
    }), options);
};
