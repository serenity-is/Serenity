import { Fluent, getjQuery, isArrayLike } from "@serenity-is/base";
import { validatorAbortHandler } from "../../q";

export namespace ValidationHelper {
    export function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        let $ = getjQuery();
        if (!$ || !$.validator) {
            submitHandler?.();
            return true;
        }
        var validator = $(form).validate();
        var valSettings = validator.settings;
        if ((valSettings as any).abortHandler) {
            return false;
        }
        if (validateBeforeSave != null && validateBeforeSave() === false) {
            return false;
        }
        (valSettings as any)['abortHandler'] = validatorAbortHandler;
        valSettings['submitHandler'] = function () {
            submitHandler?.();
            return false;
        };
        Fluent.trigger(isArrayLike(form) ? form[0] : form , 'submit');
        return true;
    }

    export function submit(form: ArrayLike<HTMLElement> | HTMLElement,  validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        let $ = getjQuery();
        if (!$ || !$.validator) {
            submitHandler?.();
            return true;        
        }
        var validator = $(form).validate();
        var valSettings = validator.settings;
        if ((valSettings as any).abortHandler != null) {
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

    export function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): any {
        var element = isArrayLike(elem) ? elem[0] : elem;
        if (!element)
            return null;
        var form = element.closest('form');
        if (!form)
            return null;
        let $ = getjQuery();
        if (!$)
            return null;
        return $(form).data('validator');
    }

    export function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void {
        var element = isArrayLike(elem) ? elem[0] : elem;
        var validator = getValidator(element);
        if (validator)
            return validator.element(element);
    }
}