namespace Serenity {

    import Option = Serenity.Decorators.option

    @Decorators.registerEditor('Serenity.DateTimeEditor', [IStringValue, IReadOnly])
    @Decorators.element('<input type="text"/>')
    export class DateTimeEditor extends Widget<DateTimeEditorOptions> implements IStringValue, IReadOnly {

        private minValue: string;
        private maxValue: string;
        private minDate: string;
        private maxDate: string;
        private sqlMinMax: boolean;
        private time: JQuery;

        constructor(input: JQuery, opt?: DateTimeEditorOptions) {
            super(input, opt);

            input.addClass('dateQ s-DateTimeEditor').datepicker({
                showOn: 'button',
                beforeShow: function () {
                    return !input.hasClass('readonly');
                } as any,
                yearRange: Q.coalesce(this.options.yearRange, '-100:+50')
            });

            input.bind('keyup.' + this.uniqueName, e => {
                if (e.which === 32 && !this.get_readOnly()) {
                    if (this.get_valueAsDate() !== new Date()) {
                        this.set_valueAsDate(new Date());
                        this.element.trigger('change');
                    }
                }
                else {
                    Serenity.DateEditor.dateInputKeyup(e);
                }
            });

            input.bind('change.' + this.uniqueName,
                DateEditor.dateInputChange);

            this.time = $('<select/>').addClass('editor s-DateTimeEditor time');
            var after = input.next('.ui-datepicker-trigger');
            if (after.length > 0) {
                this.time.insertAfter(after);
            }
            else {
                after = input.prev('.ui-datepicker-trigger');
                if (after.length > 0) {
                    this.time.insertBefore(after);
                }
                else {
                    this.time.insertAfter(input);
                }
            }

            var timeOpt = DateTimeEditor.getTimeOptions(
                Q.coalesce(this.options.startHour, 0), 0,
                Q.coalesce(this.options.endHour, 23), 59,
                Q.coalesce(this.options.intervalMinutes, 5));

            for (var t of timeOpt) {
                Q.addOption(this.time, t, t);
            }

            VX.addValidationRule(input, this.uniqueName, e1 => {
                var value = this.get_value();
                if (Q.isEmptyOrNull(value)) {
                    return null;
                }

                if (!Q.isEmptyOrNull(this.get_minValue()) &&
                    ss.compareStrings(value, this.get_minValue()) < 0) {
                    return Q.format(Q.text('Validation.MinDate'),
                        Q.formatDate(this.get_minValue(), null));
                }

                if (!Q.isEmptyOrNull(this.get_maxValue()) &&
                    ss.compareStrings(value, this.get_maxValue()) >= 0) {
                    return Q.format(Q.text('Validation.MaxDate'),
                        Q.formatDate(this.get_maxValue(), null));
                }
                return null;
            });

            this.set_sqlMinMax(true);

            $("<div class='inplace-button inplace-now'><b></b></div>")
                .attr('title', 'set to now')
                .insertAfter(this.time).click(e2 => {
                    if (this.element.hasClass('readonly')) {
                        return;
                    }
                    this.set_valueAsDate(new Date());
                    input.triggerHandler('change');
                });

            this.time.on('change', function (e3) {
                input.triggerHandler('change');
            });
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
                this.element.val(Q.formatDate(ss.today(), null));
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

        @Option()
        get_minValue(): string {
            return this.minValue;
        }

        set_minValue(value: string) {
            this.minValue = value;
        }

        @Option()
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

        @Option()
        get_maxDate(): Date {
            return Q.parseISODateTime(this.get_maxValue());
        }

        set_maxDate(value: Date) {
            this.set_maxValue(Q.formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
        }

        @Option()
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
                    this.element.nextAll('.inplace-now').css('opacity', '0.1');

                }
                else {
                    this.element.removeClass('readonly').removeAttr('readonly');
                    this.element.nextAll('.ui-datepicker-trigger').css('opacity', '1');
                    this.element.nextAll('.inplace-now').css('opacity', '1');
                }
                Serenity.EditorUtils.setReadonly(this.time, value);
            }
        }

        static roundToMinutes(date: Date, minutesStep: number) {
            date = new Date(date.getTime());
            var m = ss.trunc(ss.round(date.getMinutes() / minutesStep) * minutesStep);
            date.setMinutes(m);
            date.setSeconds(0);
            date.setMilliseconds(0);
            return date;
        }

        static getTimeOptions = function (fromHour: number, fromMin: number,
            toHour: number, toMin: number, stepMins: number) {
            var list = [];
            if (toHour >= 23) {
                toHour = 23;
            }
            if (toMin >= 60) {
                toMin = 59;
            }
            var hour = fromHour;
            var min = fromMin;
            while (true) {
                if (hour > toHour || hour === toHour && min > toMin) {
                    break;
                }
                var t = ((hour >= 10) ? '' : '0') + hour + ':' + ((min >= 10) ? '' : '0') + min;
                list.push(t);
                min += stepMins;
                if (min >= 60) {
                    min -= 60;
                    hour++;
                }
            }
            return list;
        };
    }

    export interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
        yearRange?: string;
        useUtc?: boolean;
    }
}
