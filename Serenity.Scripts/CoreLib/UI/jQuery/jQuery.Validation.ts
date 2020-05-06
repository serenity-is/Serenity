

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

    function jQueryValidationInitialization() {

        let p: any = $.validator;
        p = p.prototype;
        oldShowLabel = p.showLabel;
        p.showLabel = validateShowLabel;

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
    };

    export function validatorAbortHandler(validator: any) {
        validator.settings.abortHandler = null;
        validator.settings.submitHandler = function () {
            return false;
        };
    };

    export function validateOptions(options: JQueryValidation.ValidationOptions) {
        return Q.extend({
            ignore: ":hidden",
            ignoreTitle: true,
            meta: 'v',
            normalizer: function (value: any) {
                return $.trim(value);
            },
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
                    .each((i, x) => {
                        $(x).click();
                        return true;
                    });

                if (validator.errorList.length)
                {
                    var el = validator.errorList[0].element;
                    if (el) {
                        var bsPane = $(el).closest('.tab-pane');
                        if (!bsPane.hasClass("active") &&
                            bsPane.parent().hasClass('tab-content')) {
                            var bsPaneId = bsPane.attr('id');
                            if (bsPaneId) {
                                $('a[href="#' + bsPaneId + '"]').click();
                            }
                        }

                        if ($.fn.tooltip) {
                            $.fn.tooltip && ($(el) as any).tooltip({
                                title: validator.errorList[0].message,
                                trigger: 'manual'
                            }).tooltip('show');

                            window.setTimeout(function () {
                                $(el).tooltip('destroy');
                            }, 1500);
                        }
                    }
                }
            },
            success: function (label: JQuery) {
                label.addClass('checked');
            }
        }, options);
    };

    if ($.validator)
        jQueryValidationInitialization()
    else $(function () {
        $.validator && jQueryValidationInitialization();
    });
}
