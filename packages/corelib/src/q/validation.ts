import { Config, Fluent, getjQuery, htmlEncode, isArrayLike, isBS3, isBS5Plus, parseDate, parseDecimal, parseInteger, tryGetText } from "@serenity-is/base";
import { parseDayHourAndMin, parseHourAndMin } from "./formatting-compat";
import { extend } from "./system-compat";

function initValidatorMethods(): boolean {
    let $ = getjQuery();
    if (!$ || !$.validator || !$.validator.methods || !$.validator.addMethod)
        return false;
    let validator = $.validator;

    if (validator.prototype.showLabel) {
        var orgShowLabel = validator.prototype.showLabel;
        validator.prototype.showLabel = function(element: any, message: any) {
            if (message != null)
                message = htmlEncode(message);
            orgShowLabel.call(this, element, message);
        }
    }

    validator.addMethod('customValidate', function (value: any, element: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) {
        let result = this.optional(element) as any;
        if (!element || !!result)
            return result;

        let rules = customValidateRules.get(element);
        if (!rules)
            return true;

        for (let k in rules) {
            let handlers = rules[k];
            for (let handler of handlers) {
                if (typeof handler === "function") {
                    let message = handler(element);
                    if (message != null) {
                        element.dataset.customvalidationmessage = message;
                        return false;
                    }
                }
            }
        }

        return true;

    }, function (_: any, element: any) {
        return element?.dataset?.customvalidationmessage;
    });

    validator.addMethod("dateQ", function (value: string, element: any) {
        var o = this.optional(element);
        if (o)
            return o;

        var d = parseDate(value);
        if (!d || isNaN(d.valueOf()))
            return false;

        var z = new Date(d);
        z.setHours(0, 0, 0, 0);
        return z.getTime() === d.getTime();
    });

    validator.addMethod("dateTimeQ", function (value: string, element: any) {
        var o = this.optional(element);
        if (o)
            return o;

        var d = parseDate(value);
        if (!d || isNaN(d.valueOf()))
            return false;

        return true;
    });        

    validator.addMethod("hourAndMin", function (value: string, element: any) {
        return this.optional(element) || !isNaN(parseHourAndMin(value));
    });

    validator.addMethod("dayHourAndMin", function (value: string, element: any) {
        return this.optional(element) || !isNaN(parseDayHourAndMin(value));
    });

    validator.addMethod("decimalQ", function (value: string, element: any) {
        return this.optional(element) || !isNaN(parseDecimal(value));
    });

    validator.addMethod("integerQ", function (value: string, element: any) {
        return this.optional(element) || !isNaN(parseInteger(value));
    });

    let oldEmail = validator.methods['email'];
    let emailRegex: RegExp = null;
    validator.addMethod("email", function (value: string, element: any) {
        if (Config.emailAllowOnlyAscii)
            return oldEmail.call(this, value, element);

        if (emailRegex == null) 
            emailRegex = new RegExp("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|" +
                "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|" +
                "[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|" +
                "((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|" +
                "\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|" +
                "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|" +
                "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])" +
                "([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)" +
                "+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|" +
                "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))$", "i");
                
        return emailRegex.test(value);
    });

    $(loadValidationErrorMessages);
    return true;
}

!initValidatorMethods() && Fluent.ready(initValidatorMethods);

export function loadValidationErrorMessages(): boolean {

    let $ = getjQuery();
    if (!$ || !$.validator || !$.validator.methods || !$.validator.addMethods)
        return false;
    let validator = $.validator;    
    const addMsg = (m: string, k: string) => {
        var txt = tryGetText("Validation." + k);
        if (txt)
            validator.messages[m] = txt;
        else if (!validator.messages[m])
            validator.messages[m] = k + "?";
    }

    addMsg("required", "Required");
    addMsg("email", "Email");
    addMsg("minlength", "MinLength");
    addMsg("maxlength", "MaxLength");
    addMsg("digits", "Digits");
    addMsg("range", "Range");
    addMsg("xss", "Xss");
    addMsg("dateQ", "DateInvalid");
    addMsg("decimalQ", "Decimal");
    addMsg("integerQ", "Integer");
    addMsg("url", "Url");
    return true;
}

function setTooltip(el: ArrayLike<HTMLElement> | HTMLElement, val: string, show: boolean): void {
    var element = isArrayLike(el) ? el[0] : el;
    if (!element)
        return;
    let $ = getjQuery();
    if (isBS3()) {
        element.setAttribute('data-original-title', val || '');
        $?.(element)?.tooltip?.('fixTitle');
    }
    else {
        element.setAttribute('title', val || '');
        if ($ && $.fn.tooltip) {
            $(element)?.tooltip?.('_fixTitle');
        }
    }
    if (show != null) {
        if ($?.fn?.tooltip)
            $(element).tooltip?.(show ? 'show' : 'hide');
        else if (isBS5Plus() && typeof bootstrap !== "undefined" && (bootstrap as any).Tooltip) {
            var inst = (bootstrap as any).Tooltip.getInstance?.();
            inst && (inst[show ? "show" : "hide"])?.();
        }
    }
}

export function getHighlightTarget(el: HTMLElement) {
    var hl = el.dataset.vxHighlight;
    if (hl)
        return document.getElementById(hl);
    else if (el.classList.contains("select2-offscreen") && el.id)
        return document.getElementById('s2id_' + el.id);
}

export function baseValidateOptions(): any {
    return <any>{
        errorClass: 'error',
        ignore: '[style*="display:none"], [style*="display: none"] *, .hidden *, input[type=hidden], .no-validate',
        ignoreTitle: true,
        normalizer: function (value: string) {
            return value?.trim();
        },
        highlight: function (element: HTMLElement, errorClass: string, validClass: string) {
            if ((element as any).type === "radio") {
                this.findByName((element as any).name).addClass(errorClass).removeClass(validClass);
            } else {
                if (errorClass != null && errorClass.length)
                    element.classList.add(errorClass);
                if (validClass != null && validClass.length)
                    element.classList.remove(validClass);
                var hl = getHighlightTarget(element);
                if (hl && hl.classList) {
                    if (errorClass != null && errorClass.length)
                        hl.classList.add(errorClass);
                    if (validClass != null && validClass.length)
                        hl.classList.remove(validClass);
                }
            }
        },
        unhighlight: function (element: HTMLElement, errorClass: string, validClass: string) {
            if ((element as any).type === "radio") {
                this.findByName((element as any).name).removeClass(errorClass).addClass(validClass);
            } else {
                if (errorClass != null && errorClass.length)
                    element.classList.remove(errorClass);
                if (validClass != null && validClass.length)
                    element.classList.add(validClass);
                var hl = getHighlightTarget(element);
                if (hl && hl.classList) {
                    if (errorClass != null && errorClass.length)
                        hl.classList.remove(errorClass);
                    if (validClass != null && validClass.length)
                        hl.classList.add(validClass);
                }
            }
        },
        showErrors: function () {
            let $ = getjQuery();
            if ($?.fn?.tooltip) {
                var i: number, elements: any, error: any, $el: any, hl: any;
                for (i = 0; this.errorList[i]; i++) {
                    error = this.errorList[i];
                    hl = getHighlightTarget(error.element) ?? error.element;
                    setTooltip(hl, i != 0 ? '' : error.message, false);
                }

                for (i = 0, elements = this.validElements(); elements[i]; i++) {
                    hl = getHighlightTarget(error.element) ?? error.element;
                    setTooltip(hl, '', false);
                }
            }

            $.validator.prototype.defaultShowErrors?.call(this);
        }
    }
}

export function validateForm(form: HTMLElement | ArrayLike<HTMLElement>, opt: any): any {
    return getjQuery()?.(isArrayLike(form) ? form[0] : form)?.validate?.(extend(baseValidateOptions(), opt));
}

let customValidateRules: WeakMap<HTMLElement, { [key: string]: ((input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string)[] }> = new WeakMap();

export function addValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string,
    uniqueName?: string): void {
    element = isArrayLike(element) ? element[0] : element;
    if (!element)
        return;
    element.classList.add('customValidate');
    var rules = customValidateRules.get(element);
    if (!rules)
        customValidateRules.set(element, rules = {});
    uniqueName ??= '';
    rules[uniqueName] ??= [];
    rules[uniqueName].push(rule);
}

export function removeValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName: string): void {
    element = isArrayLike(element) ? element[0] : element;
    if (!element)
        return;
    var rules = customValidateRules.get(element);
    if (rules) {
        delete rules[uniqueName];
        if (!Object.keys(rules).length)
            customValidateRules.delete(element);
    }
}