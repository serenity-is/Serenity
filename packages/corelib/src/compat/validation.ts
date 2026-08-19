import { Fluent, FormValidationTexts, Tooltip, Validator, ValidatorOptions, getjQuery, isArrayLike, notifyError } from "../base";

/**
 * Abort handler that disables further validation submission for the given validator.
 * Clears `settings.abortHandler` and replaces `settings.submitHandler` with a no-op that returns `false`.
 * Intended for use as `ValidatorOptions.abortHandler` during async form submission flows.
 * @param validator - The {@link Validator} instance whose settings should be reset to abort state.
 */
export function validatorAbortHandler(validator: Validator) {
    delete validator.settings.abortHandler;
    validator.settings.submitHandler = function () {
        return false;
    };
};

/**
 * Merges caller-supplied {@link ValidatorOptions} with Serenity's default validation behaviour.
 * Default handlers include: generic `errorPlacement` that targets `data-vx-id` / `.field` containers,
 * a `submitHandler` that prevents native submit, an `invalidHandler` that shows {@link FormValidationTexts.InvalidFormMessage},
 * expands collapsed categories/tabs and shows a tooltip on the first error, and a `success` handler that marks labels as `checked`.
 * Caller options override the defaults via `Object.assign`.
 * @param options - Optional overrides to merge on top of the defaults.
 * @returns A new {@link ValidatorOptions} object with defaults applied.
 */
export function validateOptions(options?: ValidatorOptions): ValidatorOptions {
    return Object.assign({} as ValidatorOptions, {
        errorPlacement: function (place: ArrayLike<HTMLElement> | HTMLElement, elem: ArrayLike<HTMLElement> | HTMLElement) {
            const element = isArrayLike(elem) ? elem[0] : elem;
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
            notifyError(FormValidationTexts.InvalidFormMessage);

            validator.errorList.forEach((x: any) => {
                let element: HTMLElement = isArrayLike(x.element) ? x.element[0] : x.element;
                element.closest('.category.collapsed')?.querySelectorAll<HTMLAnchorElement>(
                    ":scope > .category-title")?.forEach(el => el.click());
            });

            if (validator.errorList.length) {
                const el = validator.errorList[0].element as HTMLElement;
                if (!el)
                    return;
                let $ = getjQuery();

                const bsPaneId = el.closest('.tab-content>.tab-pane[id]:not(.active)')?.getAttribute('id');
                if (bsPaneId) {
                    let selector = 'a[href="#' + bsPaneId + '"]';
                    $ && $(selector).click(); // bs3/bs4
                    (document.querySelector<HTMLAnchorElement>(selector))?.click(); // bs5+
                }

                const uiPaneId = el.closest('.ui-tabs-panel[id]:not(.ui-tabs-panel-active)')?.getAttribute('id');
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
    }, options);
};


/**
 * Legacy helper namespace for imperative form-validation flows.
 * Wraps the underlying {@link Validator} to provide `asyncSubmit` / `submit` patterns
 * that were used by Serenity dialogs before promise-based service calls became standard.
 */
export namespace ValidationHelper {
    /**
     * Initiates an asynchronous submit flow: validates the form (if `validateBeforeSave` allows),
     * then triggers a `submit` event so the validator's `submitHandler` invokes `submitHandler`.
     * Sets `abortHandler` to allow cancellation via {@link validatorAbortHandler}.
     * @param form - The form element or array-like wrapper containing the form.
     * @param validateBeforeSave - Optional pre-validation callback; when it returns `false` the submit is cancelled.
     * @param submitHandler - Callback invoked by the validator's `submitHandler` after validation passes.
     * @returns `true` if the submit was initiated; `false` if aborted or pre-validation failed.
     */
    export function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        const validator = Validator.getInstance(form);
        const valSettings = validator.settings;
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
        Fluent.trigger(isArrayLike(form) ? form[0] : form, 'submit');
        return true;
    }

    /**
     * Synchronously validates the form and, if valid, invokes `submitHandler` directly.
     * Unlike {@link ValidationHelper.asyncSubmit}, this path calls `validator.form()` inline
     * instead of triggering a `submit` event.
     * @param form - The form element or array-like wrapper containing the form.
     * @param validateBeforeSave - Optional pre-validation callback; when it returns `false` the submit is cancelled.
     * @param submitHandler - Callback invoked when the form is valid.
     * @returns `true` if validation passed and `submitHandler` was invoked; `false` otherwise.
     */
    export function submit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        const validator = Validator.getInstance(form);
        const valSettings = validator.settings;
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

    /**
     * Gets the {@link Validator} instance associated with the given element.
     * @param elem - The form/element (or array-like wrapper) to look up the validator for.
     * @returns The existing {@link Validator} instance, or `null`/`undefined` if none is attached.
     */
    export function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): Validator {
        return Validator.getInstance(elem);
    }

    /**
     * Validates a single element using its associated {@link Validator}.
     * No-ops if no validator is attached to the element's form.
     * @param elem - The element (or array-like wrapper) to validate.
     */
    export function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void {
        const validator = getValidator(elem);
        if (validator)
            validator.element(isArrayLike(elem) ? elem[0] : elem);
    }
}
