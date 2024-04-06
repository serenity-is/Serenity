/*
 * Serenity validator implementation inspired from:
 * jQuery Validation Plugin, https://jqueryvalidation.org/) 
 * - and -
 * https://raw.githubusercontent.com/haacked/aspnet-client-validation
 */

import { Config } from "./config";
import { Fluent } from "./fluent";
import { parseDate, parseDecimal, parseInteger, stringFormat } from "./formatting";
import { localText } from "./localtext";
import { isArrayLike } from "./system";

/**
 * An `HTMLElement` that can be validated (`input`, `select`, `textarea`, or [contenteditable).
 */
export interface ValidatableElement extends HTMLElement {
    form?: HTMLFormElement;
    name?: string;
    type?: string;
    value?: string;
}

export type ValidationValue = string | string[] | number | boolean;

/**
 * Validation plugin signature with multitype return.
 * Boolean return signifies the validation result, which uses the default validation error message read from the element attribute.
 * String return signifies failed validation, which then will be used as the validation error message.
 * Promise return signifies asynchronous plugin behavior, with same behavior as Boolean or String.
 */
export type ValidationProvider = (value: ValidationValue, element: ValidatableElement, params?: any) => boolean | string | Promise<boolean | string>;

export interface ValidationErrorMap {
    [name: string]: (string | boolean);
}

export interface ValidationErrorItem {
    message: string;
    element: ValidatableElement;
    method?: string;
}

export type ValidationErrorList = ValidationErrorItem[];

export type ValidationRules = Record<string, any>;

export interface ValidationRulesMap {
    [name: string]: ValidationRules;
}

export type ValidateEventDelegate = (element: ValidatableElement, event: Event, validator: Validator) => void;

function messageKey(method: string) {
    return "msg" + method.charAt(0).toUpperCase() + method.substring(1).toLowerCase()
}

export interface ValidatorOptions {
    /** True for logging debug info */
    debug?: boolean;

    /**
     * Use this class to create error labels, to look for existing error labels and to add it to invalid elements.
     *
     * default: "error"
     */
    errorClass?: string | undefined;

    /**
     * Use this element type to create error messages and to look for existing error messages. The default, "label",
     * has the advantage of creating a meaningful link between error message and invalid field using the for attribute (which is always used, regardless of element type).
     *
     * default: "label"
     */
    errorElement?: string | undefined;

    /**
     * Customize placement of created error labels. First argument: The created error label. Second argument: The invalid element.
     *
     * default: Places the error label after the invalid element
     */
    errorPlacement?(error: HTMLElement, element: ValidatableElement, validator: Validator): void;

    /**
     * Focus the last active or first invalid element on submit via validator.focusInvalid(). The last active element is the one
     * that had focus when the form was submitted, avoiding stealing its focus. If there was no element focused, the first one
     * in the form gets it, unless this option is turned off.
     *
     * default: true
     */
    focusInvalid?: boolean | undefined;

    /**
     * How to highlight invalid fields. Override to decide which fields and how to highlight.
     *
     * default: Adds errorClass (see the option) to the element
     */
    highlight?(element: ValidatableElement, errorClass: string, validClass: string): void;

    /**
     * Elements to ignore when validating, simply filtering them out. CSS not-method is used, therefore everything that is
     * accepted by not() can be passed as this option. Inputs of type submit and reset are always ignored, so are disabled elements.
     */
    ignore?: string | undefined;

    /**
     * Callback for custom code when an invalid form is submitted. Called with an event object as the first argument, and the validator
     * as in the second.
     */
    invalidHandler?(event: Event, validator: Validator): void;
    /**
     * Key/value pairs defining custom messages. Key is the name of an element, value the message to display for that element. Instead
     * of a plain message, another map with specific messages for each rule can be used. Overrides the title attribute of an element or
     * the default message for the method (in that order). Each message can be a String or a Callback. The callback is called in the scope
     * of the validator, with the rule's parameters as the first argument and the element as the second, and must return a String to display
     * as the message.
     *
     * default: the default message for the method used
     */
    messages?: Record<string, string> | undefined;

    normalizer?: (val: ValidationValue, element: ValidatableElement) => string;

    /**
     * Boolean or Function. Validate checkboxes and radio buttons on click. Set to false to disable.
     *
     * Set to a Function to decide for yourself when to run validation.
     * A boolean true is not a valid value.
     */
    onclick?: ValidateEventDelegate | boolean | undefined;

    /**
     * Function. Validate elements when user focuses in. If omitted hides all other fields marked as invalid.
     *
     * Set to a custom Function to decide for yourself when to run validation.
     */
    onfocusin?: ValidateEventDelegate | undefined;

    /**
     * Boolean or Function. Validate elements (except checkboxes/radio buttons) on blur. If nothing is entered, all rules are skipped, except when the field was already marked as invalid.
     *
     * Set to a Function to decide for yourself when to run validation.
     * A boolean true is not a valid value.
     */
    onfocusout?: ValidateEventDelegate | undefined;

    /**
     * Boolean or Function. Validate elements on keyup. As long as the field is not marked as invalid, nothing happens.
     * Otherwise, all rules are checked on each key up event. Set to false to disable.
     *
     * Set to a Function to decide for yourself when to run validation.
     * A boolean true is not a valid value.
     */
    onkeyup?: ValidateEventDelegate | undefined;

    /**
     * Validate the form on submit. Set to false to use only other events for validation.
     * Set to a Function to decide for yourself when to run validation.
     * A boolean true is not a valid value.
     *
     * default: true
     */
    onsubmit?: boolean | undefined;

    /**
     * Pending class
     * default: "pending"
     */
    pendingClass?: string | undefined;

    /**
     * A custom message display handler. Gets the map of errors as the first argument and an array of errors as the second,
     * called in the context of the validator object. The arguments contain only those elements currently validated,
     * which can be a single element when doing validation onblur/keyup. You can trigger (in addition to your own messages)
     * the default behaviour by calling this.defaultShowErrors().
     */
    rules?: ValidationRulesMap | undefined;

    /**
     * A custom message display handler. Gets the map of errors as the first argument and an array of errors as the second,
     * called in the context of the validator object. The arguments contain only those elements currently validated, which can
     * be a single element when doing validation onblur/keyup. You can trigger (in addition to your own messages) the default
     * behaviour by calling this.defaultShowErrors().
     */
    showErrors?(errorMap: ValidationErrorMap, errorList: ValidationErrorList, validator: Validator): void;

    abortHandler?(validator: Validator): void;

    /**
     * Callback for handling the actual submit when the form is valid. Gets the form and the event object. Replaces the default submit.
     * The right place to submit a form via Ajax after it is validated.
     */
    submitHandler?(form: HTMLFormElement, event: Event, validator: Validator): void | boolean;

    /**
     * String or Function. If specified, the error label is displayed to show a valid element. If a String is given, it is added as
     * a class to the label. If a Function is given, it is called with the label and the validated input (as a DOM element).
     * The label can be used to add a text like "ok!".
     */
    success?: string | ((label: HTMLElement, validatedInput: ValidatableElement) => void) | undefined;

    /**
     * Called to revert changes made by option highlight, same arguments as highlight.
     *
     * default: Removes the errorClass
     */
    unhighlight?(element: ValidatableElement, errorClass: string, validClass: string, validator: Validator): void;

    /**
     * This class is added to an element after it was validated and considered valid.
     *
     * default: "valid"
     */
    validClass?: string | undefined;
}

let validatorMap: WeakMap<HTMLFormElement, Validator> = new WeakMap();

function validatorEventDelegate(event: Event) {
    let isContentEditable = Validator.isContentEditable(this);

    // Set form expando on contenteditable
    if (!this.form && isContentEditable) {
        this.form = this.closest("form");
        this.name = this.getAttribute("name");
    }

    // Ignore the element if it belongs to another form. This will happen mainly
    // when setting the `form` attribute of an input to the id of another form.
    //if (currentForm !== this.form) {
    //    return;
    //}

    const validator = validatorMap.get(this.form);
    if (!validator)
        return;

    const eventType = "on" + event.type.replace(/^validate/, "");
    const settings = validator.settings;

    if (event.type == "focusin" && Validator.isValidatableElement(this))
        validator.lastActive = this;

    if (typeof (settings as any)[eventType] === "function" &&
        (!settings.ignore || !(this as HTMLElement).matches(settings.ignore))) {
        ((settings as any)[eventType] as ValidateEventDelegate)(this, event, validator);
    }
}

let customValidateRules: WeakMap<ValidatableElement, { [key: string]: ((input: ValidatableElement) => string)[] }> = new WeakMap();

export class Validator {

    static optional(element: ValidatableElement) {
        var val = Validator.elementValue(element);
        return !Validator.methods.required(val, element) && "dependency-mismatch";
    }

    static autoCreateRanges: boolean = false;

    static defaults: ValidatorOptions = {
        messages: {},
        rules: {},
        errorClass: "error",
        pendingClass: "pending",
        validClass: "valid",
        errorElement: "label",
        focusInvalid: true,
        onsubmit: true,
        ignore: '[style*="display:none"], [style*="display: none"] *, .hidden *, input[type=hidden], .no-validate',
        normalizer: function (value: string) {
            return typeof value === "string" ? value.trim() : value;
        },
        onfocusin: function (element: ValidatableElement, event: Event, validator: Validator) {
            validator.lastActive = element;
        },
        onfocusout: function (element: ValidatableElement, event: Event, validator: Validator) {
            if (!Validator.isCheckOrRadio(element) && (element.name in validator.submitted || !Validator.optional(element))) {
                validator.element(element);
            }
        },
        onkeyup: function (element: ValidatableElement, event: KeyboardEvent, validator: Validator) {

            // Avoid revalidate the field when pressing one of the following keys
            var excludedKeys = ["Shift", "Control", "Alt", "CapsLock", "End", "Home",
                "ArrowLeft", "ArrowUp", "ArrowRight", "ArrowDown", "Insert", "NumLock", "AltGr"];

            if (event.key === "Tab" && Validator.elementValue(element) === "" || excludedKeys.includes(event.key)) {
                return;
            } else if (element.name in validator.submitted || element.name in validator.invalid) {
                validator.element(element);
            }
        },
        onclick: function (element: ValidatableElement, event: MouseEvent, validator: Validator) {

            // Click on selects, radiobuttons and checkboxes
            if (element.name in validator.submitted) {
                validator.element(element);

                // Or option elements, check parent select in that case
            } else if ((element.parentNode as HTMLSelectElement).name in validator.submitted) {
                validator.element(element.parentNode as ValidatableElement);
            }
        },
        highlight: function (this: Validator, element, errorClass, validClass) {
            if (element.type === "radio") {
                this.findByName(element.name).forEach(x => { x.classList.add(errorClass); x.classList.remove(validClass); });
            } else {
                if (errorClass != null && errorClass.length)
                    element.classList.add(errorClass);
                if (validClass != null && validClass.length)
                    element.classList.remove(validClass);
                var hl = Validator.getHighlightTarget(element);
                if (hl && hl.classList) {
                    if (errorClass != null && errorClass.length)
                        hl.classList.add(errorClass);
                    if (validClass != null && validClass.length)
                        hl.classList.remove(validClass);
                }
            }
        },
        unhighlight: function (this: Validator, element, errorClass, validClass) {
            if (element.type === "radio") {
                this.findByName(element.name).forEach(x => { x.classList.remove(errorClass); x.classList.add(validClass); });
            } else {
                if (errorClass != null && errorClass.length)
                    element.classList.remove(errorClass);
                if (validClass != null && validClass.length)
                    element.classList.add(validClass);
                var hl = Validator.getHighlightTarget(element);
                if (hl && hl.classList) {
                    if (errorClass != null && errorClass.length)
                        hl.classList.remove(errorClass);
                    if (validClass != null && validClass.length)
                        hl.classList.add(validClass);
                }
            }
        }
    }

    static readonly messages: Record<string, string | Function> = {
        required: "Validation.Required",
        remote: "Please fix this field.",
        email: "Validation.Email",
        dateQ: "Validation.DateInvalid",
        decimalQ: "Validation.Decimal",
        dateISO: "Please enter a valid date (ISO).",
        integerQ: "Validation.Integer",
        number: "Please enter a valid number.",
        digits: "Validation.Digits",
        equalTo: "Please enter the same value again.",
        maxlength: "Validation.MaxLength",
        minlength: "Validation.MinLength",
        rangelength: "Please enter a value between {0} and {1} characters long.",
        range: "Validation.Range",
        max: "Please enter a value less than or equal to {0}.",
        min: "Please enter a value greater than or equal to {0}.",
        step: "Please enter a multiple of {0}.",
        url: "Validation.Url",
        xss: "Validation.Xss"
    }

    static readonly methods: Record<string, ValidationProvider> = {

        required: function (value, element) {
            if (element instanceof HTMLSelectElement) {

                // Could be an array for select-multiple or a string, both are fine this way
                var val = Validator.elementValue(element);
                return val && (val as any).length > 0;
            }

            if (Validator.isCheckOrRadio(element)) {
                return Validator.getLength(value, element) > 0;
            }

            return value != null && (typeof value === "number" || (value as any).length > 0);
        },

        customValidate: function (value: ValidationValue, element: ValidatableElement) {
            let result = Validator.optional(element);
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
                            return message;
                        }
                    }
                }
            }

            return true;
        },

        dateQ: function (value: string, element: any) {
            var o = Validator.optional(element);
            if (o)
                return o;

            var d = parseDate(value);
            if (!d || isNaN(d.valueOf()))
                return false;

            var z = new Date(d);
            z.setHours(0, 0, 0, 0);
            return z.getTime() === d.getTime();
        },

        dateTimeQ: function (value: string, element: any) {
            var o = Validator.optional(element);
            if (o)
                return o;

            var d = parseDate(value);
            if (!d || isNaN(d.valueOf()))
                return false;

            return true;
        },

        /*
        hourAndMin: function (value: string, element: any) {
            return Validator.optional(element) || !isNaN(parseHourAndMin(value));
        }
    
        dayHourAndMin: function (value: string, element: any) {
            return Validator.optional(element) || !isNaN(parseDayHourAndMin(value));
        });
        */

        decimalQ: function (value: string, element: any) {
            return Validator.optional(element) || !isNaN(parseDecimal(value));
        },

        integerQ: function (value: string, element: any) {
            return Validator.optional(element) || !isNaN(parseInteger(value));
        },

        /**
         * Validates whether the input value is an email in accordance to RFC822 specification, with a top level domain.
         */
        email: (value) => {
            if (!value || typeof value !== "string") {
                return true;
            }

            if (!Config.emailAllowOnlyAscii) {
                return new RegExp("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|" +
                    "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|" +
                    "[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|" +
                    "((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|" +
                    "\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|" +
                    "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|" +
                    "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])" +
                    "([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)" +
                    "+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|" +
                    "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))$", "i")
                    .test(value);
            }

            // RFC822 email address with .TLD validation
            // (c) Richard Willis, Chris Ferdinandi, MIT Licensed
            // https://gist.github.com/badsyntax/719800
            // https://gist.github.com/cferdinandi/d04aad4ce064b8da3edf21e26f8944c4

            let r = /^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*(\.\w{2,})+$/;
            return r.test(value);
        },

        minlength: function (value, element, param: number) {
            var length = Array.isArray(value) ? value.length : Validator.getLength(value, element);
            return Validator.optional(element) || length >= param;
        },

        maxlength: function (value, element, param: number) {
            var length = Array.isArray(value) ? value.length : Validator.getLength(value, element);
            return Validator.optional(element) || length <= param;
        },

        rangelength: function (value, element, param: number[]) {
            var length = Array.isArray(value) ? value.length : Validator.getLength(value, element);
            return Validator.optional(element) || (length >= param[0] && length <= param[1]);
        },

        min: function (value, element, param) {
            return Validator.optional(element) || value >= param;
        },

        max: function (value, element, param) {
            return Validator.optional(element) || value <= param;
        },

        range: function (value, element, param) {
            return Validator.optional(element) || (value >= param[0] && value <= param[1]);
        },

        url: function (value, element) {
            return Validator.optional(element) || /^(?:(?:(?:https?|ftp):)?\/\/)(?:(?:[^\]\[?\/<~#`!@$^&*()+=}|:";',>{ ]|%[0-9A-Fa-f]{2})+(?::(?:[^\]\[?\/<~#`!@$^&*()+=}|:";',>{ ]|%[0-9A-Fa-f]{2})*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z0-9\u00a1-\uffff][a-z0-9\u00a1-\uffff_-]{0,62})?[a-z0-9\u00a1-\uffff]\.)+(?:[a-z\u00a1-\uffff]{2,}\.?))(?::\d{2,5})?(?:[/?#]\S*)?$/i.test(value?.toString());
        }
    }

    readonly settings: ValidatorOptions;
    public lastActive: ValidatableElement;
    private cancelSubmit: boolean;
    private currentElements: ValidatableElement[];
    private currentForm: HTMLFormElement;
    private errorMap: ValidationErrorMap;
    private errorList: ValidationErrorList;
    private formSubmitted: boolean;
    private submitted: Record<string, boolean>;
    private submitButton: HTMLInputElement | HTMLButtonElement;
    private pendingRequest: number;
    private invalid: ValidationErrorMap;
    private pending: Record<string, (false | AbortController)>;
    private successList: ValidatableElement[];
    private toHide: HTMLElement[];
    private toShow: HTMLElement[];

    constructor(form: HTMLFormElement, options: ValidatorOptions) {
        if (validatorMap.get(form))
            throw "Form already has a Validator instance!";

        validatorMap.set(form, this);
        form.setAttribute("novalidate", "novalidate");

        this.settings = Object.assign(true, {}, Validator.defaults, options);
        this.currentForm = form;

        if (this.settings.onsubmit) {

            var selector = "[type=submit],button:not([type])";
            Fluent.on(this.currentForm, "click.validator", selector, (event: Event) => {

                // jquery and dom returns different results for currentTarget
                var button = ((event.currentTarget as HTMLElement)?.matches?.(selector) ? event.currentTarget : (event.target as HTMLElement).closest?.(selector) ?? event.target) as HTMLButtonElement;;

                // Track the used submit button to properly handle scripted
                // submits later.
                this.submitButton = button;

                // Allow suppressing validation by adding a cancel class to the submit button
                if (button.classList?.contains("cancel")) {
                    this.cancelSubmit = true;
                }

                // Allow suppressing validation by adding the html5 formnovalidate attribute to the submit button
                if (button.getAttribute("formnovalidate") != null) {
                    this.cancelSubmit = true;
                }
            });

            // Validate the form on submit
            Fluent.on(this.currentForm, "submit.validator", (event: Event) => {
                if (this.settings.debug) {

                    // Prevent form submit to be able to see console output
                    event.preventDefault();
                }

                const stopSubmit = () => {
                    event.preventDefault();
                    event.stopPropagation();
                    return false;
                }

                const handle = () => {
                    var hidden, result;

                    // Insert a hidden input as a replacement for the missing submit button
                    // The hidden input is inserted in two cases:
                    //   - A user defined a `submitHandler`
                    //   - There was a pending request due to `remote` method and `stopRequest()`
                    //     was called to submit the form in case it's valid
                    if (this.submitButton && (this.settings.submitHandler || this.formSubmitted)) {
                        hidden = Fluent("input").attr("type", "hidden")
                            .attr("name", this.submitButton.name)
                            .val(Fluent(this.submitButton).val())
                            .appendTo(this.currentForm);
                    }

                    if (this.settings.submitHandler && !this.settings.debug) {
                        result = this.settings.submitHandler(this.currentForm, event, this);
                        if (hidden) {

                            // And clean up afterwards; thanks to no-block-scope, hidden can be referenced
                            hidden.remove();
                        }
                        if (result !== undefined) {
                            if (result === false)
                                return stopSubmit();
                            return result;
                        }
                        return stopSubmit();
                    }
                    return true;
                }

                // Prevent submit for invalid forms or custom submit handlers
                if (this.cancelSubmit) {
                    this.cancelSubmit = false;
                    return handle();
                }
                if (this.form()) {
                    if (this.pendingRequest) {
                        this.formSubmitted = true;
                        return stopSubmit();
                    }
                    return handle();
                } else {
                    this.focusInvalid();
                    return stopSubmit();
                }
            });
        }

        this.init();
    }

    static getInstance(element: HTMLFormElement | Node | ArrayLike<HTMLElement>) {
        element = isArrayLike(element) ? element[0] : element;
        if (!element)
            return null;
        if (element instanceof HTMLFormElement)
            return validatorMap.get(element);
        else if ((element as any).form instanceof HTMLFormElement)
            return validatorMap.get((element as any).form);

        let form = (element as any).closest?.("form");
        if (form instanceof HTMLFormElement)
            return validatorMap.get(form);

        return null;
    }

    private init() {
        this.submitted = {};
        this.pendingRequest = 0;
        this.pending = {};
        this.invalid = {};
        this.reset();

        ["focusin.validator", "focusout.validator", "keyup.validator"].forEach((type) => {
            Fluent.on(this.currentForm, type, "[type='text'], [type='password'], [type='file'], select, textarea, [type='number'], [type='search'], " +
                "[type='tel'], [type='url'], [type='email'], [type='datetime'], [type='date'], [type='month'], " +
                "[type='week'], [type='time'], [type='datetime-local'], [type='range'], [type='color'], " +
                "[type='radio'], [type='checkbox'], [contenteditable], [type='button']", validatorEventDelegate);
        });

        // Support: Chrome, oldIE
        // "select" is provided as event.target when clicking a option
        Fluent.on(this.currentForm, "click.validator", "select, option, [type='radio'], [type='checkbox']", validatorEventDelegate);

        if (this.settings.invalidHandler) {
            Fluent.on(this.currentForm, "invalid-form.validator", (e) => this.settings.invalidHandler(e, this));
        }
    }

    /**
     * Checks if `element` is validatable (`input`, `select`, `textarea`).
     * @param element The element to check.
     * @returns `true` if validatable, otherwise `false`.
     */
    static isValidatableElement(element: EventTarget): element is ValidatableElement {
        return element instanceof HTMLInputElement || element instanceof HTMLSelectElement || element instanceof HTMLTextAreaElement;
    }

    static isCheckOrRadio(element: Node): element is HTMLInputElement {
        return element instanceof HTMLInputElement && (/radio|checkbox/i).test(element.type);
    }

    static getLength(value: ValidationValue, element: HTMLElement): number {
        if (element instanceof HTMLSelectElement)
            return element.querySelectorAll("option:selected").length;

        if (element instanceof HTMLInputElement && Validator.isCheckOrRadio(element)) {
            if (!element.name)
                return element.checked ? 1 : 0;
            return Array.from(element.form?.querySelectorAll(`[name=${element.name}]`)).filter(x => (x as any).checked).length || 0;
        }

        return typeof value === "number" ? ("" + value).length : (value as any).length;
    }

    static isContentEditable(element: HTMLElement) {
        let val = element.getAttribute("contenteditable");
        return val != null && val !== "false"
    }

    static elementValue(element: HTMLElement) {
        if (element instanceof Element && Validator.isContentEditable(element)) {
            return element.textContent;
        }

        if (element instanceof HTMLInputElement) {
            if (element.type === "radio" || element.type === "checkbox") {
                if (element.name && element.form) {
                    var values = Array.from(element.form.querySelectorAll<HTMLInputElement>(`input[name=${CSS.escape(element.name)}]`))
                        .map(el => el.checked ? null : el.value);

                    if (values.length > 1)
                        return values.filter(x => x != null);

                    return values[0];
                }

                return element.checked ? element.value : null;
            }

            if (element.type === "number") {
                if (typeof element.validity !== "undefined")
                    return element.validity.badInput ? NaN : element.valueAsNumber;
                else
                    return element.valueAsNumber;
            }

            var val;
            if (element.type === "file") {

                val = element.value ?? "";
                // Modern browser (chrome & safari)
                if (val.substring(0, 12) === "C:\\fakepath\\") {
                    return val.substring(12);
                }

                // Legacy browsers
                // Unix-based path
                var idx = val.lastIndexOf("/");
                if (idx >= 0) {
                    return val.substr(idx + 1);
                }

                // Windows-based path
                idx = val.lastIndexOf("\\");
                if (idx >= 0) {
                    return val.substring(idx + 1);
                }

                // Just the file name
                return val;
            }

            val = element.value;
        } else if (element instanceof HTMLTextAreaElement) {
            val = element.value;
        } else if (element instanceof HTMLSelectElement) {
            val = element.value;
        }

        if (typeof val === "string")
            return val.replace(/\r/g, "");

        return val;
    }

    static valid(element: HTMLFormElement | ValidatableElement | ArrayLike<ValidatableElement>): boolean {
        element = element instanceof HTMLFormElement ? element : isArrayLike(element) ? element[0] : element;
        if (!element)
            return false;

        let validator = Validator.getInstance(element);
        if (!validator)
            return false;

        if (element instanceof HTMLFormElement)
            return validator.form();

        return validator.element(element);
    }

    static rules(element: ValidatableElement, command?: "add" | "remove", argument?: any) {
        let isContentEditable = Validator.isContentEditable(element);
        //settings, staticRules, existingRules, data, param, filtered;

        // If nothing is selected, return empty object; can't chain anyway
        if (element == null) {
            return;
        }

        if (!element.form && isContentEditable) {
            (element as any).form = element.closest("form");
            element.name = element.getAttribute("name");
        }

        if (element.form == null) {
            return;
        }

        if (command) {
            var validator = Validator.getInstance(element.form);
            if (validator) {
                let settings = validator.settings;
                let staticRules = settings.rules;
                let existingRules = Validator.staticRules(element);
                switch (command) {
                    case "add":
                        Object.assign(existingRules, argument);

                        // Remove messages from rules, but allow them to be set separately
                        delete existingRules.messages;
                        staticRules[element.name] = existingRules;
                        if (argument.messages) {
                            settings.messages[element.name] = Object.assign(settings.messages[element.name], argument.messages);
                        }
                        break;
                    case "remove":
                        if (!argument) {
                            delete staticRules[element.name];
                            return existingRules;
                        }
                        var filtered: Record<string, any> = {};
                        argument.split(/\s/).forEach((method: string) => {
                            filtered[method] = existingRules[method];
                            delete existingRules[method];
                        });
                        return filtered;
                }
            }
        }

        var data = Validator.normalizeRules(
            Object.assign({},
                Validator.classRules(element),
                Validator.attributeRules(element),
                Validator.dataRules(element),
                Validator.staticRules(element)
            ), element);

        // Make sure required is at front
        if (data.required) {
            var param = data.required;
            delete data.required;
            data = Object.assign({ required: param }, data);
        }

        // Make sure remote is at back
        if (data.remote) {
            param = data.remote;
            delete data.remote;
            data = Object.assign(data, { remote: param });
        }

        return data;
    }

    form() {
        this.checkForm();
        Object.assign(this.submitted, this.errorMap);
        this.invalid = Object.assign({}, this.errorMap);
        if (!this.valid()) {
            Fluent.trigger(this.currentForm, "invalid-form", { validator: this });
        }
        this.showErrors();
        return this.valid();
    }

    checkForm() {
        this.prepareForm();
        for (var i = 0, elements = (this.currentElements = this.elements()); elements[i]; i++) {
            this.check(elements[i]);
        }
        return this.valid();
    }

    element(element: ValidatableElement) {
        var checkElement = this.validationTargetFor(element),
            result = true,
            rs;

        if (checkElement === void 0) {
            delete this.invalid[element.name];
        } else {
            this.prepareElement(checkElement);
            this.currentElements = [checkElement];

            rs = this.check(checkElement) !== false;
            result = result && rs;
            if (rs) {
                this.invalid[checkElement.name] = false;
            } else {
                this.invalid[checkElement.name] = true;
            }

            this.showErrors();

            // Add aria-invalid status for screen readers
            if (rs)
                element.setAttribute("aria-invalid", "true");
            else
                element.removeAttribute("aria-invalid");
        }

        return result;
    }

    showErrors(errors?: ValidationErrorMap) {
        if (errors) {
            // Add items to error list and map
            Object.assign(this.errorMap, errors);
            this.errorList = Object.keys(this.errorMap).map(name => ({
                message: this.errorMap[name] as string,
                element: this.findByName(name)[0]
            }));

            // Remove items from success list
            this.successList = this.successList.filter(function (element) {
                return !(element.name in errors);
            });
        }
        if (this.settings.showErrors) {
            this.settings.showErrors(this.errorMap, this.errorList, this);
        } else {
            this.defaultShowErrors();
        }
    }

    resetForm() {
        this.invalid = {};
        this.submitted = {};
        this.prepareForm();
        this.hideErrors();
        var elements = this.elements();
        elements.forEach(x => {
            delete (x as any).previousValue;
            x.removeAttribute("aria-invalid");
        });

        this.resetElements(elements);
    }

    resetElements(elements: ValidatableElement[]) {
        var i;

        if (this.settings.unhighlight) {
            for (i = 0; elements[i]; i++) {
                this.settings.unhighlight(elements[i], this.settings.errorClass, "", this);
                this.findByName(elements[i].name).forEach(x => x.classList.remove(this.settings.validClass));
            }
        } else {
            elements.forEach(x => {
                x.classList.remove(this.settings.errorClass);
                x.classList.remove(this.settings.validClass);
            });
        }
    }

    numberOfInvalids() {
        return Validator.objectLength(this.invalid);
    }

    private static objectLength(obj: Record<string, any>) {
        /* jshint unused: false */
        var count = 0,
            i;
        for (i in obj) {

            // This check allows counting elements with empty error
            // message as invalid elements
            if (obj[i] !== undefined && obj[i] !== null && obj[i] !== false) {
                count++;
            }
        }
        return count;
    }

    hideErrors() {
        this.hideThese(this.toHide);
    }

    hideThese(errors: HTMLElement[]) {
        errors.forEach(x => {
            x.textContent = "";
            (x as HTMLElement).style.display = "none";
        });
    }

    valid() {
        return this.size() === 0;
    }

    size() {
        return this.errorList.length;
    }

    focusInvalid() {
        if (this.settings.abortHandler)
            this.settings.abortHandler(this);

        if (this.settings.focusInvalid) {
            try {
                var lastActive = this.findLastActive() || (this.errorList.length && this.errorList[0].element);
                if (lastActive && Fluent.isVisibleLike(lastActive)) {
                    (lastActive as any).focus?.();
                    // Manually trigger focusin event; without it, focusin handler isn't called, findLastActive won't have anything to find
                    Fluent.trigger(lastActive, "focusin");
                }
            } catch (e) {
                // Ignore IE throwing errors when focusing hidden elements
            }
        }
    }

    findLastActive() {
        var lastActive = this.lastActive;
        return lastActive && this.errorList.filter(n => n.element.name === lastActive.name).length === 1 && lastActive;
    }

    elements(): ValidatableElement[] {
        var rulesCache: Record<string, boolean> = {};

        // Select all valid inputs inside the form (no submit or reset buttons)
        return Array.from(this.currentForm.querySelectorAll<HTMLElement>("input, select, textarea, [contenteditable]"))
            .filter(x => {

                if (((x instanceof HTMLButtonElement) || (x instanceof HTMLInputElement)) &&
                    (x.type === "submit" || x.type === "reset" || x.type === "image"))
                    return false;

                if ((x as any).disabled || x.closest("fieldset")?.disabled)
                    return false;

                if (x.matches(this.settings.ignore))
                    return false;

                var name = (x as any).name || x.getAttribute("name"); // For contenteditable
                var isContentEditable = Validator.isContentEditable(x);

                if (!name && this.settings.debug && window.console) {
                    console.error("%o has no name assigned", this);
                }

                // Set form expando on contenteditable
                if (isContentEditable) {
                    (x as any).form = x.closest("form");
                    (x as any).name = name;
                }

                // Ignore elements that belong to other/nested forms
                if ((x as any).form !== this.currentForm) {
                    return false;
                }

                // Select only the first element for each name, and only those with rules specified
                if (name in rulesCache || !Validator.objectLength(Validator.rules(x))) {
                    return false;
                }

                rulesCache[name] = true;
                return true;
            });
    }

    errors() {
        var errorClass = this.settings.errorClass.split(" ").join(".");
        return Array.from(this.currentForm.querySelectorAll<HTMLElement>(this.settings.errorElement + "." + errorClass));
    }

    resetInternals() {
        this.successList = [];
        this.errorList = [];
        this.errorMap = {};
        this.toShow = [];
        this.toHide = [];
    }

    reset() {
        this.resetInternals();
        this.currentElements = [];
    }

    resetAll() {
        this.resetForm();
    };

    prepareForm() {
        this.reset();
        this.toHide = this.errors();
    }

    prepareElement(element: ValidatableElement) {
        this.reset();
        this.toHide = this.errorsFor(element);
    }

    check(element: ValidatableElement) {
        element = this.validationTargetFor(element);

        var rules = Validator.rules(element),
            rulesCount = Object.keys(rules).length,
            dependencyMismatch = false,
            val = Validator.elementValue(element),
            result, method, rule, normalizer;

        // Abort any pending Ajax request from a previous call to this method.
        this.abortRequest(element);

        // Prioritize the local normalizer defined for this element over the global one
        // if the former exists, otherwise user the global one in case it exists.
        if (typeof rules.normalizer === "function") {
            normalizer = rules.normalizer;
        } else if (typeof this.settings.normalizer === "function") {
            normalizer = this.settings.normalizer;
        }

        // If normalizer is defined, then call it to retreive the changed value instead
        // of using the real one.
        // Note that `this` in the normalizer is `element`.
        if (normalizer) {
            val = normalizer(val, element);

            // Delete the normalizer from rules to avoid treating it as a pre-defined method.
            delete rules.normalizer;
        }

        for (method in rules) {
            rule = { method: method, parameters: rules[method] };
            try {
                result = Validator.methods[method].call(this, val, element, rule.parameters);

                // If a method indicates that the field is optional and therefore valid,
                // don't mark it as valid when there are no other rules
                if (result === "dependency-mismatch" && rulesCount === 1) {
                    dependencyMismatch = true;
                    continue;
                }
                dependencyMismatch = false;

                if (result === "pending") {
                    var errorsFor = this.errorsFor(element)
                    this.toHide = this.toHide.filter(x => !errorsFor.includes(x));
                    return;
                }

                if (typeof result === "string" && result !== "dependency-mismatch" && result !== "pending") {
                    element.dataset[messageKey(method)] = result;
                    result = false;
                }

                if (!result) {
                    this.formatAndAdd(element, rule);
                    return false;
                }
            } catch (e) {
                if (this.settings.debug && window.console) {
                    console.log("Exception occurred when checking element " + element.id + ", check the '" + rule.method + "' method.", e);
                }
                if (e instanceof TypeError) {
                    e.message += ".  Exception occurred when checking element " + element.id + ", check the '" + rule.method + "' method.";
                }

                throw e;
            }
        }
        if (dependencyMismatch) {
            return;
        }
        if (Validator.objectLength(rules)) {
            this.successList.push(element);
        }
        return true;
    }

    // Return the custom message for the given element and validation method
    // specified in the element's HTML5 data attribute
    // return the generic message if present and no method specific message is present
    customDataMessage(element: ValidatableElement, method: string) {
        return element.dataset[messageKey(method)] || element.dataset.msg;
    }

    // Return the custom message for the given element name and validation method
    customMessage(name: string, method: string) {
        var m = this.settings.messages[name];
        return m && (typeof m == "string" ? m : (m as any)[method]);
    }

    // Return the first defined argument, allowing empty strings
    findDefined(...args: any[]) {
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i] !== undefined) {
                return arguments[i];
            }
        }
        return undefined;
    }

    defaultMessage(element: ValidatableElement, rule: { method: string, parameters?: any }) {

        var message = this.findDefined(
            this.customMessage(element.name, rule.method),
            this.customDataMessage(element, rule.method),
            undefined,
            Validator.messages[rule.method],
            "Warning: No message defined for " + element.name
        ),
            theregex = /\$?\{(\d+)\}/g;

        if (typeof message === "function")
            return message.call(this, rule.parameters, element);

        message = localText(message, message);

        if (theregex.test(message))
            return stringFormat(message, rule.parameters);

        return message;
    }

    formatAndAdd(element: ValidatableElement, rule: { method: string, parameters: any }) {
        var message = this.defaultMessage(element, rule);

        this.errorList.push({
            message: message,
            element: element,
            method: rule.method
        });

        this.errorMap[element.name] = message;
        this.submitted[element.name] = message;
    }

    defaultShowErrors() {
        var i, elements, error;
        for (i = 0; this.errorList[i]; i++) {
            error = this.errorList[i];
            if (this.settings.highlight) {
                this.settings.highlight.call(this, error.element, this.settings.errorClass, this.settings.validClass);
            }
            this.showLabel(error.element, error.message);
        }
        if (this.errorList.length) {
            this.toShow = this.toShow;
        }
        if (this.settings.success) {
            for (i = 0; this.successList[i]; i++) {
                this.showLabel(this.successList[i]);
            }
        }
        if (this.settings.unhighlight) {
            for (i = 0, elements = this.validElements(); elements[i]; i++) {
                this.settings.unhighlight.call(this, elements[i], this.settings.errorClass, this.settings.validClass);
            }
        }
        this.toHide = this.toHide.filter(x => !this.toShow.includes(x));
        this.hideErrors();
        this.toShow.forEach(x => Fluent.toggle(x, true));
    }

    validElements() {
        let invalids = this.invalidElements();
        return this.currentElements.filter(x => !invalids.includes(x));
    }

    invalidElements() {
        return this.errorList.map(x => x.element);
    }

    showLabel(element: ValidatableElement, message?: string) {
        var errors = this.errorsFor(element),
            elementID = this.idOrName(element),
            describedBy = element.getAttribute("aria-describedby");

        if (errors.length) {

            // Refresh error/success class
            errors.forEach(x => { x.classList.remove(this.settings.validClass); x.classList.add(this.settings.errorClass) });

            // Replace message on existing label
            errors.forEach(x => { x.textContent = message || ""; });
        } else {

            // Create error element
            var error = Fluent<HTMLElement>(this.settings.errorElement as any)
                .class(this.settings.errorClass)
                .attr("id", elementID + "-error")
                .getNode();

            error.textContent = message || "";

            // Maintain reference to the element to be placed into the DOM
            var place = error;
            if (this.settings.errorPlacement) {
                this.settings.errorPlacement(place, element, this);
            } else {
                Fluent(place).insertAfter(element);
            }

            // Link error back to the element
            if (error.nodeName == "LABEL") {

                // If the error is a label, then associate using 'for'
                error.setAttribute("for", elementID);

                // If the element is not a child of an associated label, then it's necessary
                // to explicitly apply aria-describedby
            } else if (!error.closest("label[for='" + CSS.escape(elementID) + "']")) {
                var errorID = error.getAttribute("id");

                // Respect existing non-error aria-describedby
                if (!describedBy) {
                    describedBy = errorID;
                } else if (!describedBy.match(new RegExp("\\b" + CSS.escape(errorID) + "\\b"))) {

                    // Add to end of list if not already present
                    describedBy += " " + errorID;
                }
                element.setAttribute("aria-describedby", describedBy);
            }
            errors = [error];
        }
        if (!message && this.settings.success) {
            errors.forEach(x => {
                x.textContent = "";
            });
            if (typeof this.settings.success === "string") {
                errors.forEach(x => {
                    x.classList.add(this.settings.success as string);
                });
            } else {
                this.settings.success(error, element);
            }
        }
        errors.forEach(x => { this.toShow.push(x); });

        this.errorsFor(element).forEach((element: HTMLElement) => {
            if ((element?.parentNode as HTMLElement)?.classList?.contains('vx')) {
                element.setAttribute('title', element.textContent);
                if (message && element.classList.contains('error'))
                    element.classList.remove('checked');
            }
        });
    }

    errorsFor(element: ValidatableElement) {
        var name = CSS.escape(this.idOrName(element)),
            describer = element.getAttribute("aria-describedby"),
            selector = "label[for='" + name + "'], label[for='" + name + "'] *";

        // 'aria-describedby' should directly reference the error element
        if (describer) {
            selector = selector + ", #" + CSS.escape(describer)
                .replace(/\s+/g, ", #");
        }

        return this.errors()
            .filter(x => x.matches(selector));
    }

    idOrName(element: ValidatableElement) {
        return Validator.isCheckOrRadio(element) ? element.name : element.id || element.name;
    }

    validationTargetFor(element: ValidatableElement) {

        var elements = [element];
        // If radio/checkbox, validate first element in group instead
        if (Validator.isCheckOrRadio(element)) {
            elements = this.findByName(element.name);
        }

        // Always apply ignore filter
        return elements.filter(x => !x.matches(this.settings.ignore))[0];
    }

    findByName(name: string): ValidatableElement[] {
        return Array.from(this.currentForm.querySelectorAll<ValidatableElement>("[name='" + CSS.escape(name) + "']"));
    }

    dependTypes = {
        "boolean": function (param: any) {
            return param;
        },
        "string": function (param: any, element: ValidatableElement) {
            return !!element.form.querySelector(param);
        },
        "function": function (param: any, element: ValidatableElement) {
            return param(element);
        }
    }

    depend(param: any, element: ValidatableElement) {
        return (this.dependTypes as any)[typeof param] ? (this.dependTypes as any)[typeof param](param, element) : true;
    }

    startRequest(element: ValidatableElement) {
        if (!this.pending[element.name]) {
            this.pendingRequest++;
            element.classList.add(this.settings.pendingClass);
            this.pending[element.name] = new AbortController();
        }
    }

    stopRequest(element: ValidatableElement, valid: boolean) {
        let formSubmitted = this.formSubmitted;

        this.pendingRequest--;

        // Sometimes synchronization fails, make sure pendingRequest is never < 0
        if (this.pendingRequest < 0) {
            this.pendingRequest = 0;
        }
        delete this.pending[element.name];
        element.classList.remove(this.settings.pendingClass);
        if (valid && this.pendingRequest === 0 && this.formSubmitted && this.form() && this.pendingRequest === 0) {
            Fluent.trigger(this.currentForm, "submit");
            this.formSubmitted = false;
        } else if (!valid && this.pendingRequest === 0 && this.formSubmitted) {
            Fluent.trigger(this.currentForm, "invalid-form", { validator: this });
            this.formSubmitted = false;
        }

        if (!valid && this.pendingRequest == 0 && formSubmitted && this.settings.abortHandler) {
            this.settings.abortHandler(this);
        }
    }

    abortRequest(element: ValidatableElement) {
        if (this.pending[element.name]) {
            (this.pending[element.name] as AbortController).abort();
            this.pendingRequest--;

            // Sometimes synchronization fails, make sure pendingRequest is never < 0
            if (this.pendingRequest < 0) {
                this.pendingRequest = 0;
            }

            delete this.pending[element.name];
            element.classList.remove(this.settings.pendingClass);
        }
    }

    previousValue(element: ValidatableElement, method: string) {
        method = typeof method === "string" && method || "remote";

        return (element as any).previousValue || ((element as any).previousValue = {
            old: null,
            valid: true,
            message: this.defaultMessage(element, { method: method })
        });
    }

    // Cleans up all forms and elements, removes validator-specific events
    destroy() {
        this.resetForm();
        Fluent.off(this.currentForm, ".validator");
        validatorMap.delete(this.currentForm);
        delete this.currentForm;
    }

    static classRuleSettings: Record<string, ValidationRules> = {
        required: { required: true },
        email: { email: true },
        url: { url: true },
        date: { date: true },
        dateISO: { dateISO: true },
        number: { number: true },
        digits: { digits: true },
        creditcard: { creditcard: true },
        customValidate: { customValidate: true }
    }

    static addClassRules(className: (string | any), rules: ValidationRules) {
        if (typeof className === "string") {
            this.classRuleSettings[className] = rules;
        } else {
            Object.assign(this.classRuleSettings, className);
        }
    }

    static classRules(element: ValidatableElement) {
        let rules: ValidationRules = {};
        let classes = element.getAttribute("class");

        if (classes) {
            classes.split(" ").forEach(klass => {
                if (klass in Validator.classRuleSettings) {
                    Object.assign(rules, Validator.classRuleSettings[klass]);
                }
            });
        }
        return rules;
    }

    static normalizeAttributeRule(rules: ValidationRules, type: string, method: string, value: ValidationValue) {

        // Convert the value to a number for number inputs, and for text for backwards compability
        // allows type="date" and others to be compared as strings
        if (/min|max|step/.test(method) && (type === null || /number|range|text/.test(type))) {
            value = Number(value);

            // Support Opera Mini, which returns NaN for undefined minlength
            if (isNaN(value)) {
                value = undefined;
            }
        }

        if (value || value === 0) {
            rules[method] = value;
        } else if (type === method && type !== "range") {

            // Exception: the jquery validate 'range' method
            // does not test for the html5 'range' type
            rules[type === "date" ? "dateISO" : method] = true;
        }
    }

    static attributeRules(element: ValidatableElement) {
        var rules: ValidationRules = {};
        var type = element.getAttribute("type");

        for (var method in Validator.methods) {

            // Support for <input required> in both html5 and older browsers
            if (method === "required") {
                var value = element.getAttribute(method) as any;

                // Some browsers return an empty string for the required attribute
                // and non-HTML5 browsers might have required="" markup
                if (value === "") {
                    value = true;
                }

                // Force non-HTML5 browsers to return bool
                value = !!value;
            } else {
                value = element.getAttribute(method);
            }

            if (value == null)
                continue;

            Validator.normalizeAttributeRule(rules, type, method, value);
        }

        // 'maxlength' may be returned as -1, 2147483647 ( IE ) and 524288 ( safari ) for text inputs
        if (rules.maxlength && /-1|2147483647|524288/.test(rules.maxlength)) {
            delete rules.maxlength;
        }

        return rules;
    }

    static dataRules(element: ValidatableElement) {
        var rules = {},
            type = element.getAttribute("type"),
            method, value;

        for (method in Validator.methods) {
            value = element.dataset["rule" + method.charAt(0).toUpperCase() + method.substring(1).toLowerCase()];

            // Cast empty attributes like `data-rule-required` to `true`
            if (value === "") {
                value = true;
            }

            this.normalizeAttributeRule(rules, type, method, value);
        }
        return rules;
    }

    static staticRules(element: ValidatableElement): ValidationRules {
        var rules: ValidationRules = {};
        var validator = Validator.getInstance(element.form);

        if (validator.settings.rules) {
            rules = validator.settings.rules[element.name] || {};
        }

        return rules;
    }

    static normalizeRules(rules: ValidationRules, element: ValidatableElement) {

        // Handle dependency check
        Object.keys(rules).forEach(prop => {
            let val = rules[prop];

            // Ignore rule when param is explicitly false, eg. required:false
            if (val === false) {
                delete rules[prop];
                return;
            }
            if (val.param || val.depends) {
                var keepRule = true;
                switch (typeof val.depends) {
                    case "string":
                        keepRule = !!element.form.querySelector(val.depends);
                        break;
                    case "function":
                        keepRule = val.depends.call(element, element);
                        break;
                }
                if (keepRule) {
                    rules[prop] = val.param !== undefined ? val.param : true;
                } else {
                    var validator = Validator.getInstance(element.form);
                    if (validator)
                        validator.resetElements([element]);
                    delete rules[prop];
                }
            }
        });

        Object.keys(rules).forEach(rule => {
            let parameter = rules[rule];
            rules[rule] = typeof parameter === "function" && rule !== "normalizer" ? parameter(element) : parameter;
        });

        // Clean number parameters
        ["minlength", "maxlength"].forEach(x => {
            if (rules[x]) {
                rules[x] = Number(rules[x]);
            }
        });
        ["rangelength", "range"].forEach(x => {
            var parts;
            if (rules[x]) {
                if (Array.isArray(rules[x])) {
                    rules[x] = [Number(rules[x][0]), Number(rules[x][1])];
                } else if (typeof rules[x] === "string") {
                    parts = rules[x].replace(/[\[\]]/g, "").split(/[\s,]+/);
                    rules[x] = [Number(parts[0]), Number(parts[1])];
                }
            }
        });

        if (Validator.autoCreateRanges) {

            // Auto-create ranges
            if (rules.min != null && rules.max != null) {
                rules.range = [rules.min, rules.max];
                delete rules.min;
                delete rules.max;
            }
            if (rules.minlength != null && rules.maxlength != null) {
                rules.rangelength = [rules.minlength, rules.maxlength];
                delete rules.minlength;
                delete rules.maxlength;
            }
        }

        return rules;
    }

    static addMethod(name: string, method: ValidationProvider, message?: string) {
        Validator.methods[name] = method;
        Validator.messages[name] = message !== undefined ? message : Validator.messages[name];
        if (method.length < 3) {
            Validator.addClassRules(name, { [name]: true });
        }
    }

    static getHighlightTarget(el: HTMLElement) {
        var hl = el.dataset.vxHighlight;
        if (hl)
            return document.getElementById(hl);
        else if (el.classList.contains("select2-offscreen") && el.id)
            return document.getElementById('s2id_' + el.id);
    }
}

export function addValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: ValidatableElement) => string,
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
