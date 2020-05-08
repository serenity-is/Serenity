var Serenity;
(function (Serenity) {
    var StringEditor = /** @class */ (function (_super) {
        __extends(StringEditor, _super);
        function StringEditor(input) {
            return _super.call(this, input) || this;
        }
        Object.defineProperty(StringEditor.prototype, "value", {
            get: function () {
                return this.element.val();
            },
            set: function (value) {
                this.element.val(value);
            },
            enumerable: true,
            configurable: true
        });
        StringEditor.prototype.get_value = function () {
            return this.value;
        };
        StringEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        StringEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.StringEditor', [Serenity.IStringValue]),
            Serenity.Decorators.element("<input type=\"text\"/>")
        ], StringEditor);
        return StringEditor;
    }(Serenity.Widget));
    Serenity.StringEditor = StringEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var PasswordEditor = /** @class */ (function (_super) {
        __extends(PasswordEditor, _super);
        function PasswordEditor(input) {
            var _this = _super.call(this, input) || this;
            input.attr('type', 'password');
            return _this;
        }
        PasswordEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.PasswordEditor')
        ], PasswordEditor);
        return PasswordEditor;
    }(Serenity.StringEditor));
    Serenity.PasswordEditor = PasswordEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var BooleanEditor = /** @class */ (function (_super) {
        __extends(BooleanEditor, _super);
        function BooleanEditor() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(BooleanEditor.prototype, "value", {
            get: function () {
                return this.element.is(":checked");
            },
            set: function (value) {
                this.element.prop("checked", !!value);
            },
            enumerable: true,
            configurable: true
        });
        BooleanEditor.prototype.get_value = function () {
            return this.value;
        };
        BooleanEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        BooleanEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.BooleanEditor', [Serenity.IBooleanValue]),
            Serenity.Decorators.element('<input type="checkbox"/>')
        ], BooleanEditor);
        return BooleanEditor;
    }(Serenity.Widget));
    Serenity.BooleanEditor = BooleanEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var DecimalEditor = /** @class */ (function (_super) {
        __extends(DecimalEditor, _super);
        function DecimalEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.addClass('decimalQ');
            var numericOptions = Q.extend(Serenity.DecimalEditor.defaultAutoNumericOptions(), {
                vMin: Q.coalesce(_this.options.minValue, _this.options.allowNegatives ? (_this.options.maxValue != null ? ("-" + _this.options.maxValue) : '-999999999999.99') : '0.00'),
                vMax: Q.coalesce(_this.options.maxValue, '999999999999.99')
            });
            if (_this.options.decimals != null) {
                numericOptions.mDec = _this.options.decimals;
            }
            if (_this.options.padDecimals != null) {
                numericOptions.aPad = _this.options.padDecimals;
            }
            if ($.fn.autoNumeric)
                input.autoNumeric(numericOptions);
            return _this;
        }
        DecimalEditor.prototype.get_value = function () {
            if ($.fn.autoNumeric) {
                var val = this.element.autoNumeric('get');
                if (!!(val == null || val === ''))
                    return null;
                return parseFloat(val);
            }
            var val = this.element.val();
            return Q.parseDecimal(val);
        };
        Object.defineProperty(DecimalEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        DecimalEditor.prototype.set_value = function (value) {
            if (value == null || value === '') {
                this.element.val('');
            }
            else if ($.fn.autoNumeric) {
                this.element.autoNumeric('set', value);
            }
            else
                this.element.val(Q.formatNumber(value));
        };
        DecimalEditor.prototype.get_isValid = function () {
            return !isNaN(this.get_value());
        };
        DecimalEditor.defaultAutoNumericOptions = function () {
            return {
                aDec: Q.Culture.decimalSeparator,
                altDec: ((Q.Culture.decimalSeparator === '.') ? ',' : '.'),
                aSep: ((Q.Culture.decimalSeparator === '.') ? ',' : '.'),
                aPad: true
            };
        };
        DecimalEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.DecimalEditor', [Serenity.IDoubleValue]),
            Serenity.Decorators.element('<input type="text"/>')
        ], DecimalEditor);
        return DecimalEditor;
    }(Serenity.Widget));
    Serenity.DecimalEditor = DecimalEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IntegerEditor = /** @class */ (function (_super) {
        __extends(IntegerEditor, _super);
        function IntegerEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.addClass('integerQ');
            var numericOptions = Q.extend(Serenity.DecimalEditor.defaultAutoNumericOptions(), {
                vMin: Q.coalesce(_this.options.minValue, _this.options.allowNegatives ? (_this.options.maxValue != null ? ("-" + _this.options.maxValue) : '-2147483647') : '0'),
                vMax: Q.coalesce(_this.options.maxValue, 2147483647),
                aSep: null
            });
            if ($.fn.autoNumeric)
                input.autoNumeric(numericOptions);
            return _this;
        }
        IntegerEditor.prototype.get_value = function () {
            if ($.fn.autoNumeric) {
                var val = this.element.autoNumeric('get');
                if (!!Q.isTrimmedEmpty(val))
                    return null;
                else
                    return parseInt(val, 10);
            }
            else {
                var val = Q.trimToNull(this.element.val());
                if (val == null)
                    return null;
                return Q.parseInteger(val);
            }
        };
        Object.defineProperty(IntegerEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        IntegerEditor.prototype.set_value = function (value) {
            if (value == null || value === '')
                this.element.val('');
            else if ($.fn.autoNumeric)
                this.element.autoNumeric('set', value);
            else
                this.element.val(Q.formatNumber(value));
        };
        IntegerEditor.prototype.get_isValid = function () {
            return !isNaN(this.get_value());
        };
        IntegerEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.IntegerEditor', [Serenity.IDoubleValue]),
            Serenity.Decorators.element('<input type="text"/>')
        ], IntegerEditor);
        return IntegerEditor;
    }(Serenity.Widget));
    Serenity.IntegerEditor = IntegerEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Option = Serenity.Decorators.option;
    Serenity.datePickerIconSvg = '<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 17 17"><g></g><path d="M14 2v-1h-3v1h-5v-1h-3v1h-3v15h17v-15h-3zM12 2h1v2h-1v-2zM4 2h1v2h-1v-2zM16 16h-15v-8.921h15v8.921zM1 6.079v-3.079h2v2h3v-2h5v2h3v-2h2v3.079h-15z" fill="#000000"></path></svg>';
    var DateEditor = /** @class */ (function (_super) {
        __extends(DateEditor, _super);
        function DateEditor(input) {
            var _this = _super.call(this, input) || this;
            if (typeof flatpickr !== "undefined" && (DateEditor_1.useFlatpickr || !$.fn.datepicker)) {
                flatpickr(input[0], DateEditor_1.flatPickrOptions(input));
            }
            else if ($.fn.datepicker) {
                input.datepicker({
                    showOn: 'button',
                    beforeShow: function (inp, inst) {
                        return !input.hasClass('readonly');
                    },
                    yearRange: Q.coalesce(_this.yearRange, '-100:+50')
                });
            }
            else {
                input.attr('type', 'date');
            }
            input.on('keyup.' + _this.uniqueName, function (e) {
                if (e.which === 32 && !_this.get_readOnly()) {
                    if (_this.get_valueAsDate() != Q.today()) {
                        _this.set_valueAsDate(Q.today());
                        _this.element.trigger('change');
                    }
                }
                else {
                    Serenity.DateEditor.dateInputKeyup(e);
                }
            });
            input.on('change.' + _this.uniqueName, Serenity.DateEditor.dateInputChange);
            Q.addValidationRule(input, _this.uniqueName, function (e1) {
                var value = _this.get_value();
                if (Q.isEmptyOrNull(value)) {
                    return null;
                }
                if (!Q.isEmptyOrNull(_this.get_minValue()) && Q.Invariant.stringCompare(value, _this.get_minValue()) < 0) {
                    return Q.format(Q.text('Validation.MinDate'), Q.formatDate(_this.get_minValue(), null));
                }
                if (!Q.isEmptyOrNull(_this.get_maxValue()) && Q.Invariant.stringCompare(value, _this.get_maxValue()) >= 0) {
                    return Q.format(Q.text('Validation.MaxDate'), Q.formatDate(_this.get_maxValue(), null));
                }
                return null;
            });
            _this.set_sqlMinMax(true);
            return _this;
        }
        DateEditor_1 = DateEditor;
        DateEditor.flatPickrOptions = function (input) {
            return {
                clickOpens: true,
                allowInput: true,
                dateFormat: Q.Culture.dateOrder.split('').join(Q.Culture.dateSeparator).replace('y', 'Y'),
                onChange: function () {
                    input.triggerHandler('change');
                }
            };
        };
        DateEditor.prototype.get_value = function () {
            var value = this.element.val().trim();
            if (value != null && value.length === 0) {
                return null;
            }
            return Q.formatDate(value, 'yyyy-MM-dd');
        };
        Object.defineProperty(DateEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        DateEditor.prototype.set_value = function (value) {
            if (value == null) {
                this.element.val('');
            }
            else if (value.toLowerCase() === 'today' || value.toLowerCase() === 'now') {
                this.element.val(Q.formatDate(Q.today(), null));
            }
            else {
                this.element.val(Q.formatDate(value, null));
            }
        };
        DateEditor.prototype.get_valueAsDate = function () {
            if (Q.isEmptyOrNull(this.get_value())) {
                return null;
            }
            return Q.parseISODateTime(this.get_value());
        };
        Object.defineProperty(DateEditor.prototype, "valueAsDate", {
            get: function () {
                return this.get_valueAsDate();
            },
            set: function (v) {
                this.set_valueAsDate(v);
            },
            enumerable: true,
            configurable: true
        });
        DateEditor.prototype.set_valueAsDate = function (value) {
            if (value == null) {
                this.set_value(null);
            }
            this.set_value(Q.formatDate(value, 'yyyy-MM-dd'));
        };
        DateEditor.prototype.get_readOnly = function () {
            return this.element.hasClass('readonly');
        };
        DateEditor.prototype.set_readOnly = function (value) {
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
        };
        DateEditor.prototype.get_minValue = function () {
            return this.minValue;
        };
        DateEditor.prototype.set_minValue = function (value) {
            this.minValue = value;
        };
        DateEditor.prototype.get_maxValue = function () {
            return this.maxValue;
        };
        DateEditor.prototype.set_maxValue = function (value) {
            this.maxValue = value;
        };
        DateEditor.prototype.get_minDate = function () {
            return Q.parseISODateTime(this.get_minValue());
        };
        DateEditor.prototype.set_minDate = function (value) {
            this.set_minValue(Q.formatDate(value, 'yyyy-MM-dd'));
        };
        DateEditor.prototype.get_maxDate = function () {
            return Q.parseISODateTime(this.get_maxValue());
        };
        DateEditor.prototype.set_maxDate = function (value) {
            this.set_maxValue(Q.formatDate(value, 'yyyy-MM-dd'));
        };
        DateEditor.prototype.get_sqlMinMax = function () {
            return this.get_minValue() === '1753-01-01' && this.get_maxValue() === '9999-12-31';
        };
        DateEditor.prototype.set_sqlMinMax = function (value) {
            if (value) {
                this.set_minValue('1753-01-01');
                this.set_maxValue('9999-12-31');
            }
            else {
                this.set_minValue(null);
                this.set_maxValue(null);
            }
        };
        DateEditor.flatPickrTrigger = function (input) {
            return $('<i class="ui-datepicker-trigger" href="javascript:;">' + Serenity.datePickerIconSvg + '</i>')
                .insertAfter(input)
                .click(function () {
                if (!input.hasClass('readonly'))
                    // @ts-ignore
                    input[0]._flatpickr.open();
            });
        };
        DateEditor.dateInputKeyup = function (e) {
            if (Q.Culture.dateOrder !== 'dmy') {
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
            var val = Q.coalesce(input.val(), '');
            if (!!(val.length === 0 || input[0].selectionEnd !== val.length)) {
                return;
            }
            if (val.indexOf(Q.Culture.dateSeparator + Q.Culture.dateSeparator) !== -1) {
                input.val(Q.replaceAll(val, Q.Culture.dateSeparator + Q.Culture.dateSeparator, Q.Culture.dateSeparator));
                return;
            }
            function isNumeric(c) {
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
        ;
        var DateEditor_1;
        DateEditor.dateInputChange = function (e) {
            if (Q.Culture.dateOrder !== 'dmy') {
                return;
            }
            var input = $(e.target);
            if (!input.is(':input') || input.attr("type") == "date") {
                return;
            }
            var val = Q.coalesce(input.val(), '');
            var x = {};
            if (val.length >= 6 && /^[0-9]*$/g.test(val)) {
                input.val(val.substr(0, 2) + Q.Culture.dateSeparator + val.substr(2, 2) + Q.Culture.dateSeparator + val.substr(4));
            }
            val = Q.coalesce(input.val(), '');
            if (!!(val.length >= 5 && Q.parseDate(val) !== false)) {
                var d = Q.parseDate(val);
                input.val(Q.formatDate(d, null));
            }
        };
        __decorate([
            Option()
        ], DateEditor.prototype, "yearRange", void 0);
        __decorate([
            Option()
        ], DateEditor.prototype, "get_minValue", null);
        __decorate([
            Option()
        ], DateEditor.prototype, "get_maxValue", null);
        __decorate([
            Option()
        ], DateEditor.prototype, "get_maxDate", null);
        __decorate([
            Option()
        ], DateEditor.prototype, "get_sqlMinMax", null);
        DateEditor = DateEditor_1 = __decorate([
            Serenity.Decorators.registerEditor('Serenity.DateEditor', [Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element('<input type="text"/>')
        ], DateEditor);
        return DateEditor;
    }(Serenity.Widget));
    Serenity.DateEditor = DateEditor;
    function jQueryDatepickerInitialization() {
        if (!$.datepicker || !$.datepicker.regional || !$.datepicker.regional.en)
            return false;
        var order = Q.Culture.dateOrder;
        var s = Q.Culture.dateSeparator;
        var culture = ($('html').attr('lang') || 'en').toLowerCase();
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
            buttonImage: 'data:image/svg+xml,' + encodeURI(Serenity.datePickerIconSvg),
            buttonImageOnly: true,
            showOn: 'both',
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true
        });
        return true;
    }
    ;
    typeof $ !== "undefined" && !jQueryDatepickerInitialization() && $(jQueryDatepickerInitialization);
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Option = Serenity.Decorators.option;
    var DateTimeEditor = /** @class */ (function (_super) {
        __extends(DateTimeEditor, _super);
        function DateTimeEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.addClass('s-DateTimeEditor');
            if (_this.options.inputOnly) {
                input.addClass('dateTimeQ');
                // just a basic input, usually read only display
            }
            else if (typeof flatpickr !== "undefined" && (Serenity.DateEditor.useFlatpickr || !$.fn.datepicker || _this.options.seconds)) {
                input.addClass('dateTimeQ');
                flatpickr(input[0], _this.getFlatpickrOptions());
            }
            else if ($.fn.datepicker) {
                input.addClass('dateQ');
                input.datepicker({
                    showOn: 'button',
                    beforeShow: function () {
                        return !input.hasClass('readonly');
                    },
                    yearRange: Q.coalesce(_this.options.yearRange, '-100:+50')
                });
                input.bind('change.' + _this.uniqueName, function (e) {
                    _this.lastSetValue = null;
                    Serenity.DateEditor.dateInputChange(e);
                });
                _this.time = $('<select/>').addClass('editor s-DateTimeEditor time');
                var after = input.next('.ui-datepicker-trigger');
                if (after.length > 0) {
                    _this.time.insertAfter(after);
                }
                else {
                    after = input.prev('.ui-datepicker-trigger');
                    if (after.length > 0) {
                        _this.time.insertBefore(after);
                    }
                    else {
                        _this.time.insertAfter(input);
                    }
                }
                _this.time.on('change', function (e3) {
                    this.lastSetValue = null;
                    input.triggerHandler('change');
                });
                var timeOpt = DateTimeEditor_1.getTimeOptions(Q.coalesce(_this.options.startHour, 0), 0, Q.coalesce(_this.options.endHour, 23), 59, Q.coalesce(_this.options.intervalMinutes, 5));
                for (var _i = 0, timeOpt_1 = timeOpt; _i < timeOpt_1.length; _i++) {
                    var t = timeOpt_1[_i];
                    Q.addOption(_this.time, t, t);
                }
                Q.addValidationRule(input, _this.uniqueName, function (e1) {
                    var value = _this.get_value();
                    if (Q.isEmptyOrNull(value)) {
                        return null;
                    }
                    if (!Q.isEmptyOrNull(_this.get_minValue()) &&
                        Q.Invariant.stringCompare(value, _this.get_minValue()) < 0) {
                        return Q.format(Q.text('Validation.MinDate'), Q.formatDate(_this.get_minValue(), null));
                    }
                    if (!Q.isEmptyOrNull(_this.get_maxValue()) &&
                        Q.Invariant.stringCompare(value, _this.get_maxValue()) >= 0) {
                        return Q.format(Q.text('Validation.MaxDate'), Q.formatDate(_this.get_maxValue(), null));
                    }
                    return null;
                });
            }
            else
                input.attr('type', 'datetime').addClass('dateTimeQ');
            input.bind('keyup.' + _this.uniqueName, function (e) {
                if (_this.get_readOnly())
                    return;
                if (_this.time) {
                    if (e.which === 32) {
                        if (_this.get_valueAsDate() !== new Date()) {
                            _this.set_valueAsDate(new Date());
                            _this.element.trigger('change');
                        }
                    }
                    else {
                        var before = _this.element.val();
                        Serenity.DateEditor.dateInputKeyup(e);
                        if (before != _this.element.val())
                            _this.lastSetValue = null;
                    }
                }
            });
            _this.set_sqlMinMax(true);
            if (!_this.options.inputOnly) {
                $("<i class='inplace-button inplace-now'><b></b></div>")
                    .attr('title', 'set to now')
                    .insertAfter(_this.time).click(function (e2) {
                    if (_this.element.hasClass('readonly')) {
                        return;
                    }
                    _this.lastSetValue = null;
                    _this.set_valueAsDate(new Date());
                    input.triggerHandler('change');
                });
            }
            return _this;
        }
        DateTimeEditor_1 = DateTimeEditor;
        DateTimeEditor.prototype.getFlatpickrOptions = function () {
            var _this = this;
            var _a;
            return {
                clickOpens: true,
                allowInput: true,
                enableTime: true,
                time_24hr: true,
                enableSeconds: !!this.options.seconds,
                minuteIncrement: (_a = this.options.intervalMinutes) !== null && _a !== void 0 ? _a : 5,
                dateFormat: Q.Culture.dateOrder.split('').join(Q.Culture.dateSeparator).replace('y', 'Y') + " H:i" + (this.options.seconds ? ":S" : ""),
                onChange: function () {
                    _this.lastSetValue = null;
                    _this.element && _this.element.triggerHandler('change');
                }
            };
        };
        DateTimeEditor.prototype.get_value = function () {
            var value = this.element.val().trim();
            if (value != null && value.length === 0) {
                return null;
            }
            var result;
            if (this.time) {
                var datePart = Q.formatDate(value, 'yyyy-MM-dd');
                var timePart = this.time.val();
                result = datePart + 'T' + timePart + ':00.000';
            }
            else
                result = Q.formatDate(Q.parseDate(this.element.val()), "yyyy-MM-ddTHH:mm:ss.fff");
            if (this.options.useUtc)
                result = Q.formatISODateTimeUTC(Q.parseISODateTime(result));
            if (this.lastSetValue != null &&
                this.lastSetValueGet == result)
                return this.lastSetValue;
            return result;
        };
        Object.defineProperty(DateTimeEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        DateTimeEditor.prototype.set_value = function (value) {
            if (Q.isEmptyOrNull(value)) {
                this.element.val('');
                this.time && this.time.val('00:00');
            }
            else if (value.toLowerCase() === 'today') {
                if (this.time) {
                    this.element.val(Q.formatDate(Q.today(), null));
                    this.time.val('00:00');
                }
                else {
                    this.element.val(this.getDisplayFormat());
                }
            }
            else {
                var val = ((value.toLowerCase() === 'now') ? new Date() : Q.parseISODateTime(value));
                if (this.time) {
                    val = Serenity.DateTimeEditor.roundToMinutes(val, Q.coalesce(this.options.intervalMinutes, 5));
                    this.element.val(Q.formatDate(val, null));
                    this.time.val(Q.formatDate(val, 'HH:mm'));
                }
                else
                    this.element.val(Q.formatDate(val, this.getDisplayFormat()));
            }
            this.lastSetValue = null;
            if (!Q.isEmptyOrNull(value)) {
                this.lastSetValueGet = this.get_value();
                this.lastSetValue = value;
            }
        };
        DateTimeEditor.prototype.getDisplayFormat = function () {
            return (this.options.seconds ? Q.Culture.dateTimeFormat : Q.Culture.dateTimeFormat.replace(':ss', ''));
        };
        DateTimeEditor.prototype.get_valueAsDate = function () {
            if (Q.isEmptyOrNull(this.get_value())) {
                return null;
            }
            return Q.parseISODateTime(this.get_value());
        };
        Object.defineProperty(DateTimeEditor.prototype, "valueAsDate", {
            get: function () {
                return this.get_valueAsDate();
            },
            set: function (value) {
                this.set_valueAsDate(value);
            },
            enumerable: true,
            configurable: true
        });
        DateTimeEditor.prototype.set_valueAsDate = function (value) {
            if (value == null) {
                this.set_value(null);
            }
            this.set_value(Q.formatDate(value, 'yyyy-MM-ddTHH:mm' + (this.options.seconds ? ':ss' : '')));
        };
        DateTimeEditor.prototype.get_minValue = function () {
            return this.minValue;
        };
        DateTimeEditor.prototype.set_minValue = function (value) {
            this.minValue = value;
        };
        DateTimeEditor.prototype.get_maxValue = function () {
            return this.maxValue;
        };
        DateTimeEditor.prototype.set_maxValue = function (value) {
            this.maxValue = value;
        };
        DateTimeEditor.prototype.get_minDate = function () {
            return Q.parseISODateTime(this.get_minValue());
        };
        DateTimeEditor.prototype.set_minDate = function (value) {
            this.set_minValue(Q.formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
        };
        DateTimeEditor.prototype.get_maxDate = function () {
            return Q.parseISODateTime(this.get_maxValue());
        };
        DateTimeEditor.prototype.set_maxDate = function (value) {
            this.set_maxValue(Q.formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
        };
        DateTimeEditor.prototype.get_sqlMinMax = function () {
            return this.get_minValue() === '1753-01-01' && this.get_maxValue() === '9999-12-31';
        };
        DateTimeEditor.prototype.set_sqlMinMax = function (value) {
            if (value) {
                this.set_minValue('1753-01-01');
                this.set_maxValue('9999-12-31');
            }
            else {
                this.set_minValue(null);
                this.set_maxValue(null);
            }
        };
        DateTimeEditor.prototype.get_readOnly = function () {
            return this.element.hasClass('readonly');
        };
        DateTimeEditor.prototype.set_readOnly = function (value) {
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
                this.time && this.time.attr('readonly', value ? "readonly" : null);
            }
        };
        DateTimeEditor.roundToMinutes = function (date, minutesStep) {
            date = new Date(date.getTime());
            var m = Q.trunc(Q.round(date.getMinutes() / minutesStep) * minutesStep);
            date.setMinutes(m);
            date.setSeconds(0);
            date.setMilliseconds(0);
            return date;
        };
        var DateTimeEditor_1;
        DateTimeEditor.getTimeOptions = function (fromHour, fromMin, toHour, toMin, stepMins) {
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
        __decorate([
            Option()
        ], DateTimeEditor.prototype, "get_minValue", null);
        __decorate([
            Option()
        ], DateTimeEditor.prototype, "get_maxValue", null);
        __decorate([
            Option()
        ], DateTimeEditor.prototype, "get_maxDate", null);
        __decorate([
            Option()
        ], DateTimeEditor.prototype, "get_sqlMinMax", null);
        DateTimeEditor = DateTimeEditor_1 = __decorate([
            Serenity.Decorators.registerEditor('Serenity.DateTimeEditor', [Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element('<input type="text"/>')
        ], DateTimeEditor);
        return DateTimeEditor;
    }(Serenity.Widget));
    Serenity.DateTimeEditor = DateTimeEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EmailAddressEditor = /** @class */ (function (_super) {
        __extends(EmailAddressEditor, _super);
        function EmailAddressEditor(input) {
            var _this = _super.call(this, input) || this;
            input.attr('type', 'email')
                .addClass('email');
            return _this;
        }
        EmailAddressEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.EmailAddressEditor')
        ], EmailAddressEditor);
        return EmailAddressEditor;
    }(Serenity.StringEditor));
    Serenity.EmailAddressEditor = EmailAddressEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EmailEditor = /** @class */ (function (_super) {
        __extends(EmailEditor, _super);
        function EmailEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            EmailEditor_1.registerValidationMethods();
            input.addClass('emailuser');
            var spanAt = $('<span/>').text('@').addClass('emailat').insertAfter(input);
            var domain = $('<input type="text"/>').addClass('emaildomain').insertAfter(spanAt);
            domain.bind('blur.' + _this.uniqueName, function () {
                var validator = domain.closest('form').data('validator');
                if (validator != null) {
                    validator.element(input[0]);
                }
            });
            if (!Q.isEmptyOrNull(_this.options.domain)) {
                domain.val(_this.options.domain);
            }
            if (_this.options.readOnlyDomain) {
                domain.attr('readonly', 'readonly').addClass('disabled').attr('tabindex', '-1');
            }
            input.bind('keypress.' + _this.uniqueName, function (e) {
                if (e.which === 64) {
                    e.preventDefault();
                    if (!_this.options.readOnlyDomain) {
                        domain.focus();
                        domain.select();
                    }
                }
            });
            domain.bind('keypress.' + _this.uniqueName, function (e1) {
                if (e1.which === 64) {
                    e1.preventDefault();
                }
            });
            if (!_this.options.readOnlyDomain) {
                input.change(function (e2) { return _this.set_value(input.val()); });
            }
            return _this;
        }
        EmailEditor_1 = EmailEditor;
        EmailEditor.registerValidationMethods = function () {
            var _a;
            if (!$.validator || !$.validator.methods || $.validator.methods['emailuser'] != null)
                return;
            $.validator.addMethod('emailuser', function (value, element) {
                var domain = $(element).nextAll('.emaildomain');
                if (domain.length > 0 && domain.attr('readonly') == null) {
                    if (this.optional(element) && this.optional(domain[0])) {
                        return true;
                    }
                    return $.validator.methods.email.call(value + '@' + domain.val());
                }
            }, (_a = Q.tryGetText("Validation.Email")) !== null && _a !== void 0 ? _a : $.validator.messages.email);
        };
        EmailEditor.prototype.get_value = function () {
            var domain = this.element.nextAll('.emaildomain');
            var value = this.element.val();
            var domainValue = domain.val();
            if (Q.isEmptyOrNull(value)) {
                if (this.options.readOnlyDomain || Q.isEmptyOrNull(domainValue)) {
                    return '';
                }
                return '@' + domainValue;
            }
            return value + '@' + domainValue;
        };
        Object.defineProperty(EmailEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        EmailEditor.prototype.set_value = function (value) {
            var domain = this.element.nextAll('.emaildomain');
            value = Q.trimToNull(value);
            if (value == null) {
                if (!this.options.readOnlyDomain)
                    domain.val('');
                this.element.val('');
            }
            else {
                var parts = value.split('@');
                if (parts.length > 1) {
                    if (!this.options.readOnlyDomain) {
                        domain.val(parts[1]);
                        this.element.val(parts[0]);
                    }
                    else if (!Q.isEmptyOrNull(this.options.domain)) {
                        if (parts[1] !== this.options.domain)
                            this.element.val(value);
                        else
                            this.element.val(parts[0]);
                    }
                    else
                        this.element.val(parts[0]);
                }
                else
                    this.element.val(parts[0]);
            }
        };
        EmailEditor.prototype.get_readOnly = function () {
            var domain = this.element.nextAll('.emaildomain');
            return !(this.element.attr('readonly') == null &&
                (!this.options.readOnlyDomain || domain.attr('readonly') == null));
        };
        EmailEditor.prototype.set_readOnly = function (value) {
            var domain = this.element.nextAll('.emaildomain');
            if (value) {
                this.element.attr('readonly', 'readonly').addClass('readonly');
                if (!this.options.readOnlyDomain) {
                    domain.attr('readonly', 'readonly').addClass('readonly');
                }
            }
            else {
                this.element.removeAttr('readonly').removeClass('readonly');
                if (!this.options.readOnlyDomain) {
                    domain.removeAttr('readonly').removeClass('readonly');
                }
            }
        };
        var EmailEditor_1;
        EmailEditor = EmailEditor_1 = __decorate([
            Serenity.Decorators.registerEditor('Serenity.EmailEditor', [Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element('<input type="text"/>')
        ], EmailEditor);
        return EmailEditor;
    }(Serenity.Widget));
    Serenity.EmailEditor = EmailEditor;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-editors.js.map