
namespace Q {

    let oldShowLabel: (e: HTMLElement, message: string) => void;

    function validateShowLabel(element: HTMLElement, message: string) {
        oldShowLabel.call(this, element, message);
        this.errorsFor(element).each(function (i: number, e: any) {

            if ($(element).hasClass('error'))
                $(e).removeClass('checked');

            $(e).attr('title', $(e).text());
        });
    };

    function registerCustomValidationMethods() {
        if ($.validator.methods['customValidate'] == null) {
            ($.validator as any).addMethod('customValidate', function (value: any, element: any) {
                var result = this.optional(element);
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
                    var handler = (ss as any).safeCast(handlers[i].handler, Function);
                    if (handler) {
                        var message = handler(el);
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
        }
    }

    function jQueryValidationInitialization() {
        registerCustomValidationMethods();

        let p: any = $.validator;
        p = p.prototype;
        oldShowLabel = p.showLabel;
        p.showLabel = validateShowLabel;

        $.validator.addMethod("dateQ", function (value, element) {
            var o = this.optional(element);
            if (o)
                return o;

            var d = parseDate(value);
            if (!d)
                return false;

            var z = new Date(d);
            z.setHours(0, 0, 0, 0);
            return z.getTime() === d.getTime();
        });

        $.validator.addMethod("hourAndMin", function (value, element) {
            return this.optional(element) || !isNaN(parseHourAndMin(value));
        });

        $.validator.addMethod("dayHourAndMin", function (value, element) {
            return this.optional(element) || !isNaN(parseDayHourAndMin(value));
        });

        $.validator.addMethod("decimalQ", function (value, element) {
            return this.optional(element) || !isNaN(parseDecimal(value));
        });

        $.validator.addMethod("integerQ", function (value, element) {
            return this.optional(element) || !isNaN(parseInteger(value));
        });

        let oldEmail = $.validator.methods['email'];
        $.validator.addMethod("email", function (value, element) {
            if (!Q.Config.emailAllowOnlyAscii)
                return oldEmail.call(this, value, element);
            return this.optional(element) || /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/.test(value);
        });

        $.validator.addMethod("emailMultiple", function (value, element) {
            let result = this.optional(element);
            if (result)
                return result;
            if (value.indexOf(';') >= 0)
                value = value.split(';');
            else
                value = value.split(',');
            for (let i = 0; i < value.length; i++) {
                result = $.validator.methods['email'].call(this, value[i], element);
                if (!result)
                    return result;
            }
            return result;
        });

        $.validator.addMethod("anyvalue", function (value, element) {
            return true;
        });

        let d = (<any>$.validator).defaults;

        d.ignoreTitle = true;
        d.onchange = function (element: any) {
            this.element(element);
        };
        p.oldinit = p.init;
        p.init = function () {
            p.oldinit.call(this);
            function changeDelegate(event: any) {
                if (this.form == null)
                    return;
                let validator = $.data(this.form, "validator"), eventType = "on" + event.type.replace(/^validate/, "");
                validator && validator.settings[eventType] && validator.settings[eventType].call(validator, this);
            }
            function delegate(event: any) {
                let el = this[0];
                if (!$.data(el, 'changebound')) {
                    $(el).change(changeDelegate);
                    $.data(el, 'changebound', true);
                }
            }
            $(this.currentForm)
                .on(":text, :password, :file, select, textarea", "focusin.validate", delegate);
        };
        p.oldfocusInvalid = p.focusInvalid;
        p.focusInvalid = function () {
            if (this.settings.abortHandler)
                this.settings.abortHandler(this);
            this.oldfocusInvalid.call(this);
        };
        p.oldstopRequest = p.focusInvalid;
        p.stopRequest = function (element: any, valid: boolean) {
            let formSubmitted = this.formSubmitted;
            this.oldfocusInvalid.call(this, [element, valid]);
            if (!valid && this.pendingRequest == 0 && formSubmitted && this.settings.abortHandler) {
                this.settings.abortHandler(this);
            }
        };
        p.resetAll = function () {
            this.submitted = {};
            this.prepareForm();
            this.hideErrors();
            this.elements().removeClass(this.settings.errorClass);
        };
        jQuery(function () {
            $.extend($.validator.messages, {
                email: Q.text("Validation.Email"),
                required: Q.text("Validation.Required"),
                minlength: Q.text("Validation.MinLength"),
                maxlength: Q.text("Validation.MaxLength"),
                digits: Q.text("Validation.Digits"),
                range: Q.text("Validation.Range"),
                xss: Q.text("Validation.Xss"),
                dateQ: Q.text("Validation.DateInvalid"),
                decimalQ: Q.text("Validation.Decimal"),
                integerQ: Q.text("Validation.Integer"),
                url: Q.text("Validation.Url")
            });
        });
    };

    export function validatorAbortHandler(validator: any) {
        validator.settings.abortHandler = null;
        validator.settings.submitHandler = function () {
            return false;
        };
    };

    export function validateOptions(options: JQueryValidation.ValidationOptions) {
        return $.extend({
            ignore: ":hidden",
            meta: 'v',
            errorClass: 'error',
            errorPlacement: function (error: any, element: any) {
                let field: any = null;
                let vx = element.attr('data-vx-id');
                if (vx) {
                    field = $('#' + vx);
                    if (!field.length)
                        field = null;
                    else
                        field = field[0];
                }
                if (field == null) {
                    field = element.parents('div.field');
                    if (field.length) {
                        let inner = $('div.vx', field[0]);
                        if (inner.length)
                            field = inner[0];
                    }
                    else
                        field = element.parent();
                }
                error.appendTo(field);
            },
            submitHandler: function () {
                return false;
            },
            invalidHandler: function (event: any, validator: JQueryValidation.Validator) {
                Q.notifyError(Q.text("Validation.InvalidFormMessage"));

                $(validator.errorList.map(x => x.element))
                    .closest('.category.collapsed')
                    .children('.category-title')
                    .each((i, x) => $(x).click());
            },
            success: function (label: JQuery) {
                label.addClass('checked');
            }
        }, options);
    };

    if (window['jQuery'] && window['jQuery']['validator'])
        jQueryValidationInitialization();
    else if (window['jQuery']) {
        jQuery(function ($) {
            if ($.validator)
                jQueryValidationInitialization();
        });
    }

    function jQueryDatepickerInitialization(): void {
        let order = Q.Culture.dateOrder;
        let s = Q.Culture.dateSeparator;
        let culture = ($('html').attr('lang') || 'en').toLowerCase();
        if (!$.datepicker.regional[culture]) {
            culture = culture.split('-')[0];
            if (!$.datepicker.regional[culture]) {
                culture = 'en';
            }
        }
        $.datepicker.setDefaults($.datepicker.regional['en']);
        $.datepicker.setDefaults($.datepicker.regional[culture]);
        $.datepicker.setDefaults({
            dateFormat: (order == 'mdy' ? 'mm' + s + 'dd' + s + 'yy' :
                (order == 'ymd' ? 'yy' + s + 'mm' + s + 'dd' :
                    'dd' + s + 'mm' + s + 'yy')),
            buttonImage: Q.resolveUrl('~/content/serenity/images/datepicker.png'),
            buttonImageOnly: true,
            showOn: 'both',
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true
        });
    };

    if (window['jQuery'] &&
        window['jQuery']['datepicker'] &&
        window['jQuery']['datepicker']['regional'] &&
        window['jQuery']['datepicker']['regional']['en']) {
        jQueryDatepickerInitialization();
    }
    else {
        jQuery(function ($) {
            if ($.datepicker)
                jQueryDatepickerInitialization();
        });
    }

    function jQuerySelect2Initialization(): void {
        $.ui.dialog.prototype._allowInteraction = function (event: any) {
            if ($(event.target).closest(".ui-dialog").length) {
                return true;
            }
            return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
        };
    };

    if (window['jQuery'] && window['jQuery']['ui']) {
        jQuerySelect2Initialization();
    }
    else {
        jQuery(function ($) {
            if (window['jQuery']['ui'])
                jQuerySelect2Initialization();
        });
    }

    function ssExceptionInitialization() {
        ss.Exception.prototype.toString = function () {
            return this.get_message();
        };
    };

    if (ss && ss.Exception)
        ssExceptionInitialization();
    else {
        jQuery(function ($) {
            if (ss && ss.Exception)
                ssExceptionInitialization();
        });
    }
}