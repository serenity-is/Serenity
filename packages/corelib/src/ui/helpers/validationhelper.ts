import { Fluent, Validator, isArrayLike } from "@serenity-is/base";
import { validatorAbortHandler } from "../../q";

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