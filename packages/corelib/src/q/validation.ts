import { isArrayLike, Validator } from "@serenity-is/base";
import { extend } from "./system-compat";

export function baseValidateOptions(): any {
    return {};
}

export function validateForm(form: HTMLElement | ArrayLike<HTMLElement>, opt: any): any {
    return new Validator(isArrayLike(form) ? form[0] as HTMLFormElement : form as HTMLFormElement, extend(baseValidateOptions(), opt));
}