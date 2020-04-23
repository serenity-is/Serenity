namespace Serenity {

    import Option = Serenity.Decorators.option

    @Decorators.registerEditor('Serenity.DateEditor', [IStringValue, IReadOnly])
    @Decorators.element('<input type="text"/>')
    export class DateEditor extends Widget<any> implements IStringValue, IReadOnly {

        private minValue: string;
        private maxValue: string;
        private minDate: string;
        private maxDate: string;
        private sqlMinMax: boolean;

        constructor(input: JQuery) {
            super(input);

            input.addClass('dateQ');
            input.datepicker({
                showOn: 'button',
                beforeShow: (inp, inst) => {
                    return !input.hasClass('readonly') as any;
                },
                yearRange: Q.coalesce(this.yearRange, '-100:+50')
            });

            input.bind('keyup.' + this.uniqueName, e => {
                if (e.which === 32 && !this.get_readOnly()) {
                    if (this.get_valueAsDate() != Q.today()) {
                        this.set_valueAsDate(Q.today());
                        this.element.trigger('change');
                    }
                }
                else {
                    Serenity.DateEditor.dateInputKeyup(e);
                }
            });

            input.bind('change.' + this.uniqueName, Serenity.DateEditor.dateInputChange);

            Serenity.VX.addValidationRule(input, this.uniqueName, e1 => {
                var value = this.get_value();
                if (Q.isEmptyOrNull(value)) {
                    return null;
                }

                if (!Q.isEmptyOrNull(this.get_minValue()) && Q.compareStrings(value, this.get_minValue()) < 0) {
                    return Q.format(Q.text('Validation.MinDate'), Q.formatDate(this.get_minValue(), null));
                }

                if (!Q.isEmptyOrNull(this.get_maxValue()) && Q.compareStrings(value, this.get_maxValue()) >= 0) {
                    return Q.format(Q.text('Validation.MaxDate'), Q.formatDate(this.get_maxValue(), null));
                }

                return null;
            });

            this.set_sqlMinMax(true)
        }

        get_value(): string {
            var value = this.element.val().trim();
            if (value != null && value.length === 0) {
                return null;
            }

			return Q.formatDate(value, 'yyyy-MM-dd');
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string) {
            if (value == null) {
                this.element.val('');
            }
			else if (value.toLowerCase() === 'today' || value.toLowerCase() === 'now') {
                this.element.val(Q.formatDate(Q.today(), null));
            }
			else {
                this.element.val(Q.formatDate(value, null));
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

        get valueAsDate(): Date {
            return this.get_valueAsDate();
        }

        private set_valueAsDate(value: Date): void {
            if (value == null) {
                this.set_value(null);
            }

			this.set_value(Q.formatDate(value, 'yyyy-MM-dd'));
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

        @Option()
        public yearRange: string;

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
            this.set_minValue(Q.formatDate(value, 'yyyy-MM-dd'));
        }

        @Option()
        get_maxDate(): Date {
            return Q.parseISODateTime(this.get_maxValue());
        }

        set_maxDate(value: Date) {
            this.set_maxValue(Q.formatDate(value, 'yyyy-MM-dd'));
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

        static dateInputChange = function (e: JQueryEventObject) {
            if (Q.Culture.dateOrder !== 'dmy') {
                return;
            }
            var input = $(e.target);
            if (!input.is(':input')) {
                return;
            }
            var val = Q.coalesce(input.val(), '');
            var x = {};
            if (val.length >= 6 && !isNaN(parseInt(val, 10))) {
                input.val(val.substr(0, 2) + Q.Culture.dateSeparator + val.substr(2, 2) + Q.Culture.dateSeparator + val.substr(4));
            }
            val = Q.coalesce(input.val(), '');
            if (!!(val.length >= 5 && Q.parseDate(val) !== false)) {
                var d = Q.parseDate(val);
                input.val(Q.formatDate(d, null));
            }
        };

        static dateInputKeyup(e: JQueryEventObject) {

            if (Q.Culture.dateOrder !== 'dmy') {
                return;
            }

            var input = $(e.target);
            if (!input.is(':input')) {
                return;
            }

            if (input.is('[readonly]') || input.is(':disabled')) {
                return;
            }

            var val: string = Q.coalesce(input.val(), '');
            if (!!(val.length === 0 || (input[0] as any).selectionEnd !== val.length)) {
                return;
            }

            if (val.indexOf(Q.Culture.dateSeparator + Q.Culture.dateSeparator) !== -1) {
                input.val(Q.replaceAll(val, Q.Culture.dateSeparator + Q.Culture.dateSeparator,
                    Q.Culture.dateSeparator));
                return;
            }

            function isNumeric(c: number): boolean {
                return c >= 48 && c <= 57;
            }

            if (e.which === 47 || e.which === 111) {

                if (val.length >= 2 && val.charAt(val.length - 1) === Q.Culture.dateSeparator &&
                    val.charAt(val.length - 2) === Q.Culture.dateSeparator) {
                    input.val(val.substr(0, val.length - 1));
                    return;
                }

                if (val.charAt(val.length - 1) !== Q.Culture.dateSeparator) {
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
                            val.charAt(1) == Q.Culture.dateSeparator) {
                            val = '0' + val.charAt(0) + Q.Culture.dateSeparator + '0' +
                                val.charAt(2) + Q.Culture.dateSeparator;
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
                            val.charAt(1) === Q.Culture.dateSeparator) {
                            val = '0' + val;
                            break;
                        }
                        else if (isNumeric(val.charCodeAt(0)) &&
                            isNumeric(val.charCodeAt(1)) &&
                            isNumeric(val.charCodeAt(3)) &&
                            val.charAt(2) === Q.Culture.dateSeparator) {
                            val = val.charAt(0) + val.charAt(1) +
                                Q.Culture.dateSeparator + '0' + val.charAt(3) + Q.Culture.dateSeparator;
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
                            val.charAt(1) !== Q.Culture.dateSeparator ||
                            val.charCodeAt(2) <= 49) {
                            return;
                        }

                        val = '0' + val.charAt(0) + Q.Culture.dateSeparator + '0' + val.charAt(2);
                        break;
                    }

                    case 4: {
                        if (val.charAt(1) == Q.Culture.dateSeparator) {
                            if (!isNumeric(val.charCodeAt(0)) ||
                                !isNumeric(val.charCodeAt(2))) {
                                return;
                            }

                            val = '0' + val;
                            break;
                        }
                        else if (val.charAt(2) == Q.Culture.dateSeparator) {
                            if (!isNumeric(val.charCodeAt(0)) ||
                                !isNumeric(val.charCodeAt(1)) ||
                                val.charCodeAt(3) <= 49) {
                                return;
                            }

                            val = val.charAt(0) + val.charAt(1) + Q.Culture.dateSeparator +
                                '0' + val.charAt(3);
                            break;
                        }
                        else {
                            return;
                        }
                    }
                    case 5: {
                        if (val.charAt(2) !== Q.Culture.dateSeparator ||
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

                input.val(val + Q.Culture.dateSeparator);
            }
        };
    }
}