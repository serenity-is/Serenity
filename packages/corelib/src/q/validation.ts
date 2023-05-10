import { extend } from "./system";
import { Config } from "./config";
import { Exception } from "./system";
import { tryGetText } from "./localtext";
import { parseDate, parseHourAndMin, parseDayHourAndMin, parseDecimal, parseInteger } from "./formatting";
import { isBS3 } from "./dialogs";
import { htmlEncode } from "./html";
import validator from "@optionaldeps/jquery.validation";

if (validator && validator.methods && validator.addMethod) {

    if ((validator as any).prototype.showLabel) {
        var orgShowLabel = (validator as any).prototype.showLabel;
        (validator as any).prototype.showLabel = function(element: any, message: any) {
            if (message != null)
                message = htmlEncode(message);
            orgShowLabel.call(this, element, message);
        }
    }

    validator.addMethod('customValidate', function (value: any, element: any) {
        var result = this.optional(element) as any;
        if (element == null || !!result) {
            return result;
        }
        var events = ($ as any)._data(element, 'events');
        if (!events) {
            return true;
        }
        var handlers = events.customValidate;
        if (handlers == null || handlers.length === 0) {
            return true;
        }

        var el = $(element);
        for (var i = 0; !!(i < handlers.length); i++) {
            if ($.isFunction(handlers[i].handler)) {
                var message = handlers[i].handler(el);
                if (message != null) {
                    el.data('customValidationMessage', message);
                    return false;
                }
            }
        }
        return true;
    }, function (o: any, e: any) {
        return $(e).data('customValidationMessage');
    });

    validator.addMethod("dateQ", function (value, element) {
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

    validator.addMethod("dateTimeQ", function (value, element) {
        var o = this.optional(element);
        if (o)
            return o;

        var d = parseDate(value);
        if (!d || isNaN(d.valueOf()))
            return false;

        return true;
    });        

    validator.addMethod("hourAndMin", function (value, element) {
        return this.optional(element) || !isNaN(parseHourAndMin(value));
    });

    validator.addMethod("dayHourAndMin", function (value, element) {
        return this.optional(element) || !isNaN(parseDayHourAndMin(value));
    });

    validator.addMethod("decimalQ", function (value, element) {
        return this.optional(element) || !isNaN(parseDecimal(value));
    });

    validator.addMethod("integerQ", function (value, element) {
        return this.optional(element) || !isNaN(parseInteger(value));
    });

    let oldEmail = validator.methods['email'];
    let emailRegex: RegExp = null;
    validator.addMethod("email", function (value, element) {
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
}

export function loadValidationErrorMessages() {

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
}

function setTooltip(el: JQuery, val: string, show: boolean): JQuery {
    if (isBS3())
        (el.attr('data-original-title', val || '') as any)?.tooltip?.('fixTitle');
    else
        (el.attr('title', val || '') as any)?.tooltip?.('_fixTitle');
    if (show != null)
        (el as any).tooltip?.(show ? 'show' : 'hide');
    return el;
}

export function getHighlightTarget(el: HTMLElement) {
    var hl = el.dataset.vxHighlight;
    if (hl)
        return document.getElementById(hl);
    else if (el.classList.contains("select2-offscreen") && el.id)
        return document.getElementById('s2id_' + el.id);
}

export function baseValidateOptions(): JQueryValidation.ValidationOptions {
    return <any>{
        errorClass: 'error',
        ignore: ':hidden, .no-validate',
        ignoreTitle: true,
        normalizer: function (value: any) {
            return $.trim(value);
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
            if (($?.fn as any)?.tooltip) {
                var i: number, elements: any, error: any, $el: any, hl: any;
                for (i = 0; this.errorList[i]; i++) {
                    error = this.errorList[i];
                    hl = getHighlightTarget(error.element);
                    if (hl != null)
                        $el = $(hl);
                    else
                        $el = $(error.element);
                    if (i != 0)
                        setTooltip($el, '', false);
                    else
                        setTooltip($el, error.message, true);
                }

                for (i = 0, elements = this.validElements(); elements[i]; i++) {
                    hl = getHighlightTarget(error.element);
                    if (hl != null)
                        $el = $(hl);
                    else
                        $el = $(error.element);
                    setTooltip($el, '', false);
                }
            }

            (validator as any)["prototype"].defaultShowErrors.call(this);
        }
    }
}

export function validateForm(form: JQuery, opt: JQueryValidation.ValidationOptions): JQueryValidation.Validator {
    return form.validate(extend(baseValidateOptions(), opt));
}

export function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery {
    if (!element.length)
        return element;
    if (rule == null)
        throw new Exception('rule is null!');
    element.addClass('customValidate').bind('customValidate.' + eventClass, rule as any);
    return element;
}

export function removeValidationRule(element: JQuery, eventClass: string): JQuery {
    element.unbind('customValidate.' + eventClass);
    return element;
}