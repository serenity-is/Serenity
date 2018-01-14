namespace Serenity {

    @Decorators.registerEditor('Serenity.DateTimeEditor', [IStringValue, IReadOnly])
    @Decorators.element('<input/>')
    export class DateTimeEditor extends Widget<DateTimeEditorOptions> implements IStringValue, IReadOnly {

        private minValue: string;
        private maxValue: string;
        private minDate: string;
        private maxDate: string;
        private sqlMinMax: boolean;
        private time: JQuery;

        constructor(input: JQuery, opt?: DateTimeEditorOptions) {
            super(input, opt);
        }

        get_value(): string {
            var value = this.element.val().trim();
            if (value != null && value.length === 0) {
                return null;
            }

			var datePart = Q.formatDate(value, 'yyyy-MM-dd');
            var timePart = this.time.val();
            var result = datePart + 'T' + timePart + ':00.000';

            if (this.options.useUtc) {
                result = Q.formatISODateTimeUTC(Q.parseISODateTime(result));
            }

			return result;
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string) {
            if (Q.isEmptyOrNull(value)) {
                this.element.val('');
                this.time.val('00:00');
            }
			else if (value.toLowerCase() === 'today') {
                this.element.val(Q.formatDate((ss as any).today(), null));
                this.time.val('00:00');
            }
			else {
                var val = ((value.toLowerCase() === 'now') ? new Date() : Q.parseISODateTime(value));
                val = Serenity.DateTimeEditor.roundToMinutes(val, Q.coalesce(this.options.intervalMinutes, 5));
                this.element.val(Q.formatDate(val, null));
                this.time.val(Q.formatDate(val, 'HH:mm'));
            }
        }

        set value(v: string) {
            this.set_value(v);
        }

        private get_valueAsDate(): Date {
            if (Q.isEmptyOrNull(this.get_value())) {
                return null;
            }

			return Q.parseISODateTime(this.get_value());
        }

        get valueAsDate() {
            return this.get_valueAsDate();
        }

        private set_valueAsDate(value: Date) {
            if (value == null) {
                this.set_value(null);
            }

			this.set_value(Q.formatDate(value, 'yyyy-MM-ddTHH:mm'));
        }

        set valueAsDate(value: Date) {
            this.set_valueAsDate(value);
        }

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
            return Q.parseISODateTime(this.get_minValue());
        }

        set_minDate(value: Date): void {
            this.set_minValue(Q.formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
        }

        @Decorators.option()
        get_maxDate(): Date {
            return Q.parseISODateTime(this.get_maxValue());
        }

        set_maxDate(value: Date) {
            this.set_maxValue(Q.formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
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
                Serenity.EditorUtils.setReadonly(this.time, value);
            }
        }

        static roundToMinutes(date: Date, minutesStep: number) {
            date = new Date(date.getTime());
            var m = (ss as any).Int32.trunc((ss as any).round(date.getMinutes() / minutesStep) * minutesStep);
            date.setMinutes(m);
            date.setSeconds(0);
            date.setMilliseconds(0);
            return date;
        }
    }

    export interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
        yearRange?: string;
        useUtc?: boolean;
    }
}