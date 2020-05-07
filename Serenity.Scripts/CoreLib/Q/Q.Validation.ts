declare namespace JQueryValidation {
    interface ValidationOptions {
        normalizer?: (v: string) => string;
    }
}

namespace Q {

    if (typeof $ !== "undefined" && $.validator && $.validator.methods && $.validator.addMethod) {

        $.validator.addMethod('customValidate', function (value: any, element: any) {
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

        function addMsg(m: string, k: string) {
            var txt = Q.tryGetText("Validation." + k);
            if (txt)
                $.validator.messages[m] = txt;
            else if (!$.validator.messages[m]) 
                $.validator.messages[m] = k + "?";
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

    function setTooltip(el: JQuery, val: string): JQuery {
        if (Q.isBS3()) 
            el.attr('data-original-title', val || '').tooltip('fixTitle');
        else
            el.attr('title', val || '').tooltip('_fixTitle');
        return el;
    }

    const valOpt: JQueryValidation.ValidationOptions = {
        ignore: ':hidden, .no-validate',
        showErrors: function (errorMap, errorList) {
            $.each(this.validElements(), function (index, element) {
                var $element = $(element);

                setTooltip($element
                    .removeClass("error")
                    .addClass("valid"), '')
                    .tooltip('hide');
            });

            $.each(errorList, function (index: number, error) {
                var $element = $(error.element);

                setTooltip($element
                    .addClass("error"), error.message);

                if (index == 0)
                    $element.tooltip('show');
            });
        },
        normalizer: function (value) {
            return $.trim(value);
        }
    }

    export function validateTooltip(form: JQuery, opt: JQueryValidation.ValidationOptions): JQueryValidation.Validator {
        return form.validate(Q.extend(Q.extend({}, valOpt), opt));
    }
}