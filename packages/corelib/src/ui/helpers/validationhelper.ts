import { validatorAbortHandler } from "../../q";

export namespace ValidationHelper {
    export function asyncSubmit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        var validator = form.validate();
        var valSettings = validator.settings;
        if ((valSettings as any).abortHandler) {
            return false;
        }
        if (validateBeforeSave != null && validateBeforeSave() === false) {
            return false;
        }
        (valSettings as any)['abortHandler'] = validatorAbortHandler;
        valSettings['submitHandler'] = function () {
            if (submitHandler != null) {
                submitHandler();
            }
            return false;
        };
        form.trigger('submit');
        return true;
    }

    export function submit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean {
        var validator = form.validate();
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
        if (submitHandler != null) {
            submitHandler();
        }
        return true;
    }

    export function getValidator(element: JQuery): JQueryValidation.Validator {
        var form = element.closest('form');
        if (form.length === 0) {
            return null;
        }
        return form.data('validator');
    }
}