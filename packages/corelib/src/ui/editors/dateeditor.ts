import { Decorators } from "../../decorators";
import { IReadOnly, IStringValue } from "../../interfaces";
import { addValidationRule, Culture, format, formatDate, Invariant, isEmptyOrNull, parseDate, parseISODateTime, replaceAll, localText, today } from "../../q";
import { Widget } from "../widgets/widget";

export let datePickerIconSvg = '<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 17 17"><g></g><path d="M14 2v-1h-3v1h-5v-1h-3v1h-3v15h17v-15h-3zM12 2h1v2h-1v-2zM4 2h1v2h-1v-2zM16 16h-15v-8.921h15v8.921zM1 6.079v-3.079h2v2h3v-2h5v2h3v-2h2v3.079h-15z" fill="currentColor"></path></svg>';

@Decorators.registerEditor('Serenity.DateEditor', [IStringValue, IReadOnly])
@Decorators.element('<input type="text"/>')
export class DateEditor extends Widget<any> implements IStringValue, IReadOnly {

    private minValue: string;
    private maxValue: string;

    constructor(input: JQuery) {
        super(input);

        // @ts-ignore
        if (typeof flatpickr !== "undefined" && (DateEditor.useFlatpickr || !$.fn.datepicker)) {
            // @ts-ignore
            flatpickr(input[0], DateEditor.flatPickrOptions(input));
        }
        else if (($.fn as any)?.datepicker) {
            (input as any).datepicker({
                showOn: 'button',
                beforeShow: (inp: any, inst: any) => {
                    if (input.hasClass('readonly') as any)
                        return false as any;
                    DateEditor.uiPickerZIndexWorkaround(this.element);
                    return true;
                },
                yearRange: (this.yearRange ?? '-100:+50')
            });

        }
        else {
            input.attr('type', 'date');
        }

        input.on('keyup.' + this.uniqueName, e => {
            if (e.which === 32 && !this.get_readOnly()) {
                if (this.get_valueAsDate() != today()) {
                    this.set_valueAsDate(today());
                    this.element.trigger('change');
                }
            }
            else {
                DateEditor.dateInputKeyup(e);
            }
        });

        input.on('change.' + this.uniqueName, DateEditor.dateInputChange);

        addValidationRule(input, this.uniqueName, e1 => {
            var value = this.get_value();
            if (isEmptyOrNull(value)) {
                return null;
            }

            if (!isEmptyOrNull(this.get_minValue()) && Invariant.stringCompare(formatDate(value, 'yyyy-MM-dd'), formatDate(this.get_minValue(), 'yyyy-MM-dd')) < 0) {
                return format(localText('Validation.MinDate'), formatDate(this.get_minValue(), null));
            }

            if (!isEmptyOrNull(this.get_maxValue()) && Invariant.stringCompare(formatDate(value, 'yyyy-MM-dd'), formatDate(this.get_maxValue(), 'yyyy-MM-dd')) > 0) {
                return format(localText('Validation.MaxDate'), formatDate(this.get_maxValue(), null));
            }

            return null;
        });

        this.set_sqlMinMax(true)
    }

    public static useFlatpickr: boolean;

    public static flatPickrOptions(input: JQuery) {
        return {
            clickOpens: true,
            allowInput: true,
            dateFormat: Culture.dateOrder.split('').join(Culture.dateSeparator).replace('y', 'Y'),
            onChange: function() {
                input.triggerHandler('change');
            }
        }
    }

    get_value(): string {
        var value = this.element.val()?.trim();
        if (!value?.length) {
            return null;
        }

        return formatDate(value, 'yyyy-MM-dd');
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {
        if (value == null) {
            this.element.val('');
        }
        else if (value.toLowerCase() === 'today' || value.toLowerCase() === 'now') {
            this.element.val(formatDate(today(), this.element.attr('type') === 'date' ? 'yyyy-MM-dd' : null));
        }
        else {
            this.element.val(formatDate(value, this.element.attr('type') === 'date' ? 'yyyy-MM-dd' : null));
        }
    }

    set value(v: string) {
        this.set_value(v);
    }

    private get_valueAsDate(): Date {
        if (isEmptyOrNull(this.get_value())) {
            return null;
        }

        return parseISODateTime(this.get_value());
    }

    get valueAsDate(): Date {
        return this.get_valueAsDate();
    }

    private set_valueAsDate(value: Date): void {
        if (value == null) {
            this.set_value(null);
        }

        this.set_value(formatDate(value, 'yyyy-MM-dd'));
    }

    set valueAsDate(v: Date) {
        this.set_valueAsDate(v);
    }

    get_readOnly(): boolean {
        return this.element.hasClass('readonly');
    }

    set_readOnly(value: boolean): void {

        if (value !== this.get_readOnly()) {
            if (value) {
                this.element.addClass('readonly').attr('readonly', 'readonly');
                this.element.nextAll('.ui-datepicker-trigger').css('opacity', '0.1');
            }
            else {
                this.element.removeClass('readonly').removeAttr('readonly');
                this.element.nextAll('.ui-datepicker-trigger').css('opacity', '1');
            }
        }
    }

    @Decorators.option()
    public yearRange: string;

    @Decorators.option()
    get_minValue(): string {
        return this.minValue;
    }

    set_minValue(value: string) {
        this.minValue = value;
    }

    @Decorators.option()
    get_maxValue(): string {
        return this.maxValue;
    }

    set_maxValue(value: string): void {
        this.maxValue = value;
    }

    get_minDate(): Date {
        return parseISODateTime(this.get_minValue());
    }

    set_minDate(value: Date): void {
        this.set_minValue(formatDate(value, 'yyyy-MM-dd'));
    }

    @Decorators.option()
    get_maxDate(): Date {
        return parseISODateTime(this.get_maxValue());
    }

    set_maxDate(value: Date) {
        this.set_maxValue(formatDate(value, 'yyyy-MM-dd'));
    }

    @Decorators.option()
    get_sqlMinMax(): boolean {
        return this.get_minValue() === '1753-01-01' && this.get_maxValue() === '9999-12-31';
    }

    set_sqlMinMax(value: boolean) {
        if (value) {
            this.set_minValue('1753-01-01');
            this.set_maxValue('9999-12-31');
        }
        else {
            this.set_minValue(null);
            this.set_maxValue(null);
        }
    }

    static dateInputChange = function (e: JQueryEventObject) {
        if (Culture.dateOrder !== 'dmy') {
            return;
        }
        var input = $(e.target);
        if (!input.is(':input') || input.attr("type") == "date") {
            return;
        }
        var val = input.val() ?? '';
        var x = {};
        if (val.length >= 6 && /^[0-9]*$/g.test(val)) {
            input.val(val.substr(0, 2) + Culture.dateSeparator + val.substr(2, 2) + Culture.dateSeparator + val.substr(4));
        }
        val = input.val() ?? '';
        if (val.length >= 5) {
            var d = parseDate(val);
            if (d && !isNaN(d.valueOf())) {
                input.val(formatDate(d, null));
            }
        }
    };

    static flatPickrTrigger(input: JQuery): JQuery {
        return $('<i class="ui-datepicker-trigger" href="javascript:;">' + datePickerIconSvg + '</i>')
            .insertAfter(input)
            .click(() => {
                if (!input.hasClass('readonly'))
                    (input[0] as any)._flatpickr.open();
            });
    }

    static dateInputKeyup(e: JQueryEventObject) {

        if (Culture.dateOrder !== 'dmy') {
            return;
        }

        var input = $(e.target);
        if (!input.is(':input') || input.attr("type") == "date") {
            // for browser date editors, format might not match culture setting
            return;
        }

        if (input.is('[readonly]') || input.is(':disabled')) {
            return;
        }

        var val: string = input.val() ?? '';
        if (!!(val.length === 0 || (input[0] as any).selectionEnd !== val.length)) {
            return;
        }

        if (val.indexOf(Culture.dateSeparator + Culture.dateSeparator) !== -1) {
            input.val(replaceAll(val, Culture.dateSeparator + Culture.dateSeparator,
                Culture.dateSeparator));
            return;
        }

        function isNumeric(c: number): boolean {
            return c >= 48 && c <= 57;
        }

        if (e.which === 47 || e.which === 111) {

            if (val.length >= 2 && val.charAt(val.length - 1) === Culture.dateSeparator &&
                val.charAt(val.length - 2) === Culture.dateSeparator) {
                input.val(val.substr(0, val.length - 1));
                return;
            }

            if (val.charAt(val.length - 1) !== Culture.dateSeparator) {
                return;
            }

            switch (val.length) {
                case 2: {
                    if (isNumeric(val.charCodeAt(0))) {
                        val = '0' + val;
                        break;
                    }
                    else {
                        return;
                    }
                }

                case 4: {
                    if (isNumeric(val.charCodeAt(0)) &&
                        isNumeric(val.charCodeAt(2)) &&
                        val.charAt(1) == Culture.dateSeparator) {
                        val = '0' + val.charAt(0) + Culture.dateSeparator + '0' +
                            val.charAt(2) + Culture.dateSeparator;
                        break;
                    }
                    else {
                        return;
                    }
                }

                case 5: {
                    if (isNumeric(val.charCodeAt(0)) &&
                        isNumeric(val.charCodeAt(2)) &&
                        isNumeric(val.charCodeAt(3)) &&
                        val.charAt(1) === Culture.dateSeparator) {
                        val = '0' + val;
                        break;
                    }
                    else if (isNumeric(val.charCodeAt(0)) &&
                        isNumeric(val.charCodeAt(1)) &&
                        isNumeric(val.charCodeAt(3)) &&
                        val.charAt(2) === Culture.dateSeparator) {
                        val = val.charAt(0) + val.charAt(1) +
                            Culture.dateSeparator + '0' + val.charAt(3) + Culture.dateSeparator;
                        break;
                    }
                    else {
                        break;
                    }
                }
                default: {
                    return;
                }
            }
            input.val(val);
        }

        if (val.length < 6 && (e.which >= 48 && e.which <= 57 || e.which >= 96 && e.which <= 105) &&
            isNumeric(val.charCodeAt(val.length - 1))) {
            switch (val.length) {
                case 1: {
                    if (val.charCodeAt(0) <= 51) {
                        return;
                    }
                    val = '0' + val;
                    break;
                }

                case 2: {
                    if (!isNumeric(val.charCodeAt(0))) {
                        return;
                    }
                    break;
                }

                case 3: {
                    if (!isNumeric(val.charCodeAt(0)) ||
                        val.charAt(1) !== Culture.dateSeparator ||
                        val.charCodeAt(2) <= 49) {
                        return;
                    }

                    val = '0' + val.charAt(0) + Culture.dateSeparator + '0' + val.charAt(2);
                    break;
                }

                case 4: {
                    if (val.charAt(1) == Culture.dateSeparator) {
                        if (!isNumeric(val.charCodeAt(0)) ||
                            !isNumeric(val.charCodeAt(2))) {
                            return;
                        }

                        val = '0' + val;
                        break;
                    }
                    else if (val.charAt(2) == Culture.dateSeparator) {
                        if (!isNumeric(val.charCodeAt(0)) ||
                            !isNumeric(val.charCodeAt(1)) ||
                            val.charCodeAt(3) <= 49) {
                            return;
                        }

                        val = val.charAt(0) + val.charAt(1) + Culture.dateSeparator +
                            '0' + val.charAt(3);
                        break;
                    }
                    else {
                        return;
                    }
                }
                case 5: {
                    if (val.charAt(2) !== Culture.dateSeparator ||
                        !isNumeric(val.charCodeAt(0)) ||
                        !isNumeric(val.charCodeAt(1)) ||
                        !isNumeric(val.charCodeAt(3))) {
                        return;
                    }

                    break;
                }

                default: {
                    return;
                }
            }

            input.val(val + Culture.dateSeparator);
        }
    };

    public static uiPickerZIndexWorkaround(input: JQuery) {
        if (!input?.closest('.ui-dialog').length) 
            return; 
        var dialogIndex = parseInt(input.closest('.ui-dialog').css('z-index'), 10);
        if (dialogIndex == null || isNaN(dialogIndex))
            return;
        setTimeout(() => {
            var widget = (input as any).datepicker('widget');
            if (!widget?.length)
                return
            var zIndex = parseInt(widget.css('z-index'));
            if (!isNaN(zIndex) && zIndex <= dialogIndex)
                widget.css('z-index', dialogIndex + 1);
        }, 0);
    }    
}

function jQueryDatepickerInitialization(): boolean {
    if (!($ as any).datepicker?.regional?.en)
        return false;
    let order = Culture.dateOrder;
    let s = Culture.dateSeparator;
    let culture = ($('html').attr('lang') || 'en').toLowerCase();
    if (!($ as any).datepicker.regional[culture]) {
        culture = culture.split('-')[0];
        if (!($ as any).datepicker.regional[culture]) {
            culture = 'en';
        }
    }
    ($ as any).datepicker.setDefaults(($ as any).datepicker.regional['en']);
    ($ as any).datepicker.setDefaults(($ as any).datepicker.regional[culture]);
    ($ as any).datepicker.setDefaults({
        dateFormat: (order == 'mdy' ? 'mm' + s + 'dd' + s + 'yy' :
            (order == 'ymd' ? 'yy' + s + 'mm' + s + 'dd' :
                'dd' + s + 'mm' + s + 'yy')),
        buttonImage: 'data:image/svg+xml,' + encodeURIComponent(datePickerIconSvg),
        buttonImageOnly: true,
        showOn: 'both',
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true
    });

    if (($ as any).ui && ($ as any).ui.version <= '1.12.1') {
        ($ as any).datepicker.setDefaults({
            buttonImage: null,
            buttonImageOnly: false,
            buttonText: '<i class="fa fa-calendar"></i>'
        });
    }

    return true;
};

typeof $ !== "undefined" && $.fn && !jQueryDatepickerInitialization() && $(jQueryDatepickerInitialization);
