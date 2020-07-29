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
    var TextAreaEditor = /** @class */ (function (_super) {
        __extends(TextAreaEditor, _super);
        function TextAreaEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            if (_this.options.cols !== 0) {
                input.attr('cols', Q.coalesce(_this.options.cols, 80));
            }
            if (_this.options.rows !== 0) {
                input.attr('rows', Q.coalesce(_this.options.rows, 6));
            }
            return _this;
        }
        Object.defineProperty(TextAreaEditor.prototype, "value", {
            get: function () {
                return this.element.val();
            },
            set: function (value) {
                this.element.val(value);
            },
            enumerable: true,
            configurable: true
        });
        TextAreaEditor.prototype.get_value = function () {
            return this.value;
        };
        TextAreaEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        TextAreaEditor = __decorate([
            Serenity.Decorators.registerEditor('Sereniy.TextAreaEditor', [Serenity.IStringValue]),
            Serenity.Decorators.element("<textarea />")
        ], TextAreaEditor);
        return TextAreaEditor;
    }(Serenity.Widget));
    Serenity.TextAreaEditor = TextAreaEditor;
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
            if (!Q.isEmptyOrNull(value) && value.toLowerCase() != 'today' && value.toLowerCase() != 'now') {
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
    var TimeEditor = /** @class */ (function (_super) {
        __extends(TimeEditor, _super);
        function TimeEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.addClass('editor s-TimeEditor hour');
            if (!_this.options.noEmptyOption) {
                Q.addOption(input, '', '--');
            }
            for (var h = (_this.options.startHour || 0); h <= (_this.options.endHour || 23); h++) {
                Q.addOption(input, h.toString(), ((h < 10) ? ('0' + h) : h.toString()));
            }
            _this.minutes = $('<select/>').addClass('editor s-TimeEditor minute').insertAfter(input);
            _this.minutes.change(function () { return _this.element.trigger("change"); });
            for (var m = 0; m <= 59; m += (_this.options.intervalMinutes || 5)) {
                Q.addOption(_this.minutes, m.toString(), ((m < 10) ? ('0' + m) : m.toString()));
            }
            return _this;
        }
        Object.defineProperty(TimeEditor.prototype, "value", {
            get: function () {
                var hour = Q.toId(this.element.val());
                var minute = Q.toId(this.minutes.val());
                if (hour == null || minute == null) {
                    return null;
                }
                return hour * 60 + minute;
            },
            set: function (value) {
                if (!value) {
                    if (this.options.noEmptyOption) {
                        this.element.val(this.options.startHour);
                        this.minutes.val('0');
                    }
                    else {
                        this.element.val('');
                        this.minutes.val('0');
                    }
                }
                else {
                    this.element.val(Math.floor(value / 60).toString());
                    this.minutes.val(value % 60);
                }
            },
            enumerable: true,
            configurable: true
        });
        TimeEditor.prototype.get_value = function () {
            return this.value;
        };
        TimeEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        TimeEditor.prototype.get_readOnly = function () {
            return this.element.hasClass('readonly');
        };
        TimeEditor.prototype.set_readOnly = function (value) {
            if (value !== this.get_readOnly()) {
                if (value) {
                    this.element.addClass('readonly').attr('readonly', 'readonly');
                }
                else {
                    this.element.removeClass('readonly').removeAttr('readonly');
                }
                Serenity.EditorUtils.setReadonly(this.minutes, value);
            }
        };
        TimeEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.TimeEditor', [Serenity.IDoubleValue, Serenity.IReadOnly]),
            Serenity.Decorators.element("<select />")
        ], TimeEditor);
        return TimeEditor;
    }(Serenity.Widget));
    Serenity.TimeEditor = TimeEditor;
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
                    return $.validator.methods.email.call(this, value + '@' + domain.val(), element);
                }
                else {
                    return $.validator.methods.email.call(this, value + '@dummy.com', element);
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
var Serenity;
(function (Serenity) {
    var URLEditor = /** @class */ (function (_super) {
        __extends(URLEditor, _super);
        function URLEditor(input) {
            var _this = _super.call(this, input) || this;
            input.addClass("url").attr("title", "URL should be entered in format: 'http://www.site.com/page'.");
            input.on("blur." + _this.uniqueName, function (e) {
                var validator = input.closest("form").data("validator");
                if (validator == null)
                    return;
                if (!input.hasClass("error"))
                    return;
                var value = Q.trimToNull(input.val());
                if (!value)
                    return;
                value = "http://" + value;
                if ($.validator.methods['url'].call(validator, value, input[0]) == true) {
                    input.val(value);
                    validator.element(input);
                }
            });
            return _this;
        }
        URLEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.URLEditor', [Serenity.IStringValue])
        ], URLEditor);
        return URLEditor;
    }(Serenity.StringEditor));
    Serenity.URLEditor = URLEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var RadioButtonEditor = /** @class */ (function (_super) {
        __extends(RadioButtonEditor, _super);
        function RadioButtonEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            if (Q.isEmptyOrNull(_this.options.enumKey) &&
                _this.options.enumType == null &&
                Q.isEmptyOrNull(_this.options.lookupKey)) {
                return _this;
            }
            if (!Q.isEmptyOrNull(_this.options.lookupKey)) {
                var lookup = Q.getLookup(_this.options.lookupKey);
                for (var _i = 0, _a = lookup.items; _i < _a.length; _i++) {
                    var item = _a[_i];
                    var textValue = item[lookup.textField];
                    var text = (textValue == null ? '' : textValue.toString());
                    var idValue = item[lookup.idField];
                    var id = (idValue == null ? '' : idValue.toString());
                    _this.addRadio(id, text);
                }
            }
            else {
                var enumType = _this.options.enumType || Serenity.EnumTypeRegistry.get(_this.options.enumKey);
                var enumKey = _this.options.enumKey;
                if (enumKey == null && enumType != null) {
                    var enumKeyAttr = Q.getAttributes(enumType, Serenity.EnumKeyAttribute, false);
                    if (enumKeyAttr.length > 0) {
                        enumKey = enumKeyAttr[0].value;
                    }
                }
                var values = Q.Enum.getValues(enumType);
                for (var _b = 0, values_1 = values; _b < values_1.length; _b++) {
                    var x = values_1[_b];
                    var name = Q.Enum.toString(enumType, x);
                    _this.addRadio(x.toString(), Q.coalesce(Q.tryGetText('Enums.' + enumKey + '.' + name), name));
                }
            }
            return _this;
        }
        RadioButtonEditor.prototype.addRadio = function (value, text) {
            var label = $('<label/>').text(text);
            $('<input type="radio"/>').attr('name', this.uniqueName)
                .attr('id', this.uniqueName + '_' + value)
                .attr('value', value).prependTo(label);
            label.appendTo(this.element);
        };
        RadioButtonEditor.prototype.get_value = function () {
            return this.element.find('input:checked').first().val();
        };
        Object.defineProperty(RadioButtonEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        RadioButtonEditor.prototype.set_value = function (value) {
            if (value !== this.get_value()) {
                var inputs = this.element.find('input');
                var checks = inputs.filter(':checked');
                if (checks.length > 0) {
                    checks[0].checked = false;
                }
                if (!Q.isEmptyOrNull(value)) {
                    checks = inputs.filter('[value=' + value + ']');
                    if (checks.length > 0) {
                        checks[0].checked = true;
                    }
                }
            }
        };
        RadioButtonEditor.prototype.get_readOnly = function () {
            return this.element.attr('disabled') != null;
        };
        RadioButtonEditor.prototype.set_readOnly = function (value) {
            if (this.get_readOnly() !== value) {
                if (value) {
                    this.element.attr('disabled', 'disabled')
                        .find('input').attr('disabled', 'disabled');
                }
                else {
                    this.element.removeAttr('disabled')
                        .find('input').removeAttr('disabled');
                }
            }
        };
        RadioButtonEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.RadioButtonEditor', [Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element('<div/>')
        ], RadioButtonEditor);
        return RadioButtonEditor;
    }(Serenity.Widget));
    Serenity.RadioButtonEditor = RadioButtonEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Select2Editor = /** @class */ (function (_super) {
        __extends(Select2Editor, _super);
        function Select2Editor(hidden, opt) {
            var _this = _super.call(this, hidden, opt) || this;
            _this._items = [];
            _this._itemById = {};
            var emptyItemText = _this.emptyItemText();
            if (emptyItemText != null) {
                hidden.attr('placeholder', emptyItemText);
            }
            var select2Options = _this.getSelect2Options();
            hidden.select2(select2Options);
            hidden.attr('type', 'text');
            // for jquery validate to work
            hidden.on('change.' + _this.uniqueName, function (e) {
                if (!$(e.target).hasClass('select2-change-triggered') &&
                    hidden.closest('form').data('validator')) {
                    hidden.valid();
                }
            });
            _this.setCascadeFrom(_this.options.cascadeFrom);
            if (_this.useInplaceAdd())
                _this.addInplaceCreate(Q.text('Controls.SelectEditor.InplaceAdd'), null);
            return _this;
        }
        Select2Editor_1 = Select2Editor;
        Select2Editor.prototype.destroy = function () {
            if (this.element != null) {
                this.element.select2('destroy');
            }
            _super.prototype.destroy.call(this);
        };
        Select2Editor.prototype.hasAsyncSource = function () {
            return false;
        };
        Select2Editor.prototype.asyncSearch = function (query, results) {
            results({
                items: [],
                more: false
            });
            return null;
        };
        Select2Editor.prototype.getTypeDelay = function () {
            return Q.coalesce(this.options['typeDelay'], 500);
        };
        Select2Editor.prototype.emptyItemText = function () {
            return Q.coalesce(this.element.attr('placeholder'), Q.text('Controls.SelectEditor.EmptyItemText'));
        };
        Select2Editor.prototype.getPageSize = function () {
            var _a;
            return (_a = this.options['pageSize']) !== null && _a !== void 0 ? _a : 100;
        };
        Select2Editor.prototype.getIdField = function () {
            return this.options['idField'];
        };
        Select2Editor.prototype.itemId = function (item) {
            var value = item[this.getIdField()];
            if (value == null)
                return '';
            return value.toString();
        };
        Select2Editor.prototype.getTextField = function () {
            var _a;
            return (_a = this.options['textField']) !== null && _a !== void 0 ? _a : this.getIdField();
        };
        Select2Editor.prototype.itemText = function (item) {
            var value = item[this.getTextField()];
            if (value == null)
                return '';
            return value.toString();
        };
        Select2Editor.prototype.itemDisabled = function (item) {
            return false;
        };
        Select2Editor.prototype.mapItem = function (item) {
            return {
                id: this.itemId(item),
                text: this.itemText(item),
                disabled: this.itemDisabled(item),
                source: item
            };
        };
        Select2Editor.prototype.mapItems = function (items) {
            return items.map(this.mapItem.bind(this));
        };
        Select2Editor.prototype.allowClear = function () {
            return this.options.allowClear != null ?
                !!this.options.allowClear : this.emptyItemText() != null;
        };
        Select2Editor.prototype.isMultiple = function () {
            return !!this.options.multiple;
        };
        Select2Editor.prototype.getSelect2Options = function () {
            var _this = this;
            var emptyItemText = this.emptyItemText();
            var opt = {
                multiple: this.isMultiple(),
                placeHolder: (!Q.isEmptyOrNull(emptyItemText) ? emptyItemText : null),
                allowClear: this.allowClear(),
                createSearchChoicePosition: 'bottom'
            };
            if (this.hasAsyncSource()) {
                var typeTimeout = 0;
                var queryPromise = null;
                opt.query = function (query) {
                    var pageSize = _this.getPageSize();
                    var searchQuery = {
                        searchTerm: Q.trimToNull(query.term),
                        skip: (query.page - 1) * pageSize,
                        take: pageSize,
                        checkMore: true
                    };
                    queryPromise && queryPromise.abort && queryPromise.abort();
                    queryPromise = null;
                    if (typeTimeout != null)
                        clearTimeout(typeTimeout);
                    var select2 = $(_this.element).data('select2');
                    select2 && select2.search && select2.search.removeClass('select2-active');
                    typeTimeout = setTimeout(function () {
                        queryPromise && queryPromise.abort && queryPromise.abort();
                        select2 && select2.search.addClass('select2-active');
                        queryPromise = _this.asyncSearch(searchQuery, function (result) {
                            queryPromise = null;
                            query.callback({
                                results: _this.mapItems(result.items),
                                more: result.more
                            });
                        });
                        (queryPromise && (queryPromise.catch || queryPromise.fail)).call(queryPromise, function () {
                            queryPromise = null;
                            select2 && select2.search && select2.search.removeClass('select2-active');
                        });
                    }, !query.term ? 0 : _this.getTypeDelay());
                };
                var initPromise = null;
                opt.initSelection = function (element, callback) {
                    var val = element.val();
                    if (val == null || val == '') {
                        callback(null);
                        return;
                    }
                    var isMultiple = _this.isMultiple();
                    var idList = isMultiple ? val.split(',') : [val];
                    var searchQuery = {
                        idList: idList
                    };
                    initPromise && initPromise.abort && initPromise.abort();
                    initPromise = _this.asyncSearch(searchQuery, function (result) {
                        initPromise = null;
                        if (isMultiple) {
                            var items = (result.items || []).map(function (x) { return _this.mapItem(x); });
                            _this._itemById = _this._itemById || {};
                            for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
                                var item = items_1[_i];
                                _this._itemById[item.id] = item;
                            }
                            if (_this.isAutoComplete &&
                                items.length != idList.length) {
                                for (var _a = 0, idList_1 = idList; _a < idList_1.length; _a++) {
                                    var v = idList_1[_a];
                                    if (!Q.any(items, function (z) { return z.id == v; })) {
                                        items.push({
                                            id: v,
                                            text: v
                                        });
                                    }
                                }
                            }
                            callback(items);
                        }
                        else if (!result.items || !result.items.length) {
                            if (_this.isAutoComplete) {
                                callback({
                                    id: val,
                                    text: val
                                });
                            }
                            else
                                callback(null);
                        }
                        else {
                            var item = _this.mapItem(result.items[0]);
                            _this._itemById = _this._itemById || {};
                            _this._itemById[item.id] = item;
                            callback(item);
                        }
                    });
                    (initPromise && (initPromise.catch || initPromise.fail)).call(initPromise, function () {
                        initPromise = null;
                    });
                };
            }
            else {
                opt.data = this._items;
                opt.query = function (query) {
                    var items = Select2Editor_1.filterByText(_this._items, function (x) { return x.text; }, query.term);
                    var pageSize = _this.getPageSize();
                    query.callback({
                        results: items.slice((query.page - 1) * pageSize, query.page * pageSize),
                        more: items.length >= query.page * pageSize
                    });
                };
                opt.initSelection = function (element, callback) {
                    var val = element.val();
                    var isAutoComplete = _this.isAutoComplete();
                    if (_this.isMultiple()) {
                        var list = [];
                        for (var _i = 0, _a = val.split(','); _i < _a.length; _i++) {
                            var z = _a[_i];
                            var item2 = _this._itemById[z];
                            if (item2 == null && isAutoComplete) {
                                item2 = { id: z, text: z };
                                _this.addItem(item2);
                            }
                            if (item2 != null) {
                                list.push(item2);
                            }
                        }
                        callback(list);
                        return;
                    }
                    var it = _this._itemById[val];
                    if (it == null && isAutoComplete) {
                        it = { id: val, text: val };
                        _this.addItem(it);
                    }
                    callback(it);
                };
            }
            if (this.options.minimumResultsForSearch != null)
                opt.minimumResultsForSearch = this.options.minimumResultsForSearch;
            if (this.isAutoComplete() || this.useInplaceAdd())
                opt.createSearchChoice = this.getCreateSearchChoice(null);
            return opt;
        };
        Select2Editor.prototype.get_delimited = function () {
            return !!this.options.delimited;
        };
        Object.defineProperty(Select2Editor.prototype, "items", {
            get: function () {
                if (this.hasAsyncSource())
                    throw new Error("Can't read items property of an async select editor!");
                return this._items || [];
            },
            set: function (value) {
                if (this.hasAsyncSource())
                    throw new Error("Can't set items of an async select editor!");
                this._items = value || [];
                this._itemById = {};
                for (var _i = 0, _a = this._items; _i < _a.length; _i++) {
                    var item = _a[_i];
                    this._itemById[item.id] = item;
                }
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(Select2Editor.prototype, "itemById", {
            get: function () {
                if (this.hasAsyncSource())
                    throw new Error("Can't read items property of an async select editor!");
                return this._itemById;
            },
            set: function (value) {
                if (this.hasAsyncSource())
                    throw new Error("Can't set itemById of an async select editor!");
                this._itemById = value || {};
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.clearItems = function () {
            if (this.hasAsyncSource())
                throw new Error("Can't clear items of an async select editor!");
            this._items.length = 0;
            this._itemById = {};
        };
        Select2Editor.prototype.addItem = function (item) {
            if (this.hasAsyncSource())
                throw new Error("Can't add item to an async select editor!");
            this._items.push(item);
            this._itemById[item.id] = item;
        };
        Select2Editor.prototype.addOption = function (key, text, source, disabled) {
            this.addItem({
                id: key,
                text: text,
                source: source,
                disabled: disabled
            });
        };
        Select2Editor.prototype.addInplaceCreate = function (addTitle, editTitle) {
            var _this = this;
            var self = this;
            addTitle = Q.coalesce(addTitle, Q.text('Controls.SelectEditor.InplaceAdd'));
            editTitle = Q.coalesce(editTitle, Q.text('Controls.SelectEditor.InplaceEdit'));
            var inplaceButton = $('<a><b/></a>')
                .addClass('inplace-button inplace-create')
                .attr('title', addTitle)
                .insertAfter(this.element).click(function (e) {
                self.inplaceCreateClick(e);
            });
            this.get_select2Container().add(this.element).addClass('has-inplace-button');
            this.element.change(function () {
                var isNew = _this.isMultiple() || Q.isEmptyOrNull(_this.get_value());
                inplaceButton.attr('title', (isNew ? addTitle : editTitle)).toggleClass('edit', !isNew);
            });
            this.element.change(function (e) {
                if ($(e.target).hasClass('select2-change-triggered'))
                    return;
                if (_this.isMultiple()) {
                    var values = _this.get_values();
                    if (values.length > 0 && values[values.length - 1] == (-2147483648).toString()) {
                        _this.set_values(values.slice(0, values.length - 1));
                        _this.inplaceCreateClick(e);
                    }
                }
                else if (_this.get_value() == (-2147483648).toString()) {
                    _this.set_value(null);
                    _this.inplaceCreateClick(e);
                }
            });
            if (this.isMultiple()) {
                this.get_select2Container().on('dblclick.' + this.uniqueName, '.select2-search-choice', function (e3) {
                    var q = $(e3.target);
                    if (!q.hasClass('select2-search-choice')) {
                        q = q.closest('.select2-search-choice');
                    }
                    var index = q.index();
                    var values1 = _this.get_values();
                    if (index < 0 || index >= _this.get_values().length) {
                        return;
                    }
                    e3['editItem'] = values1[index];
                    _this.inplaceCreateClick(e3);
                });
            }
        };
        Select2Editor.prototype.useInplaceAdd = function () {
            return !this.isAutoComplete() &&
                this.options.inplaceAdd &&
                (this.options.inplaceAddPermission == null ||
                    Q.Authorization.hasPermission(this.options.inplaceAddPermission));
        };
        Select2Editor.prototype.isAutoComplete = function () {
            return !!this.options.autoComplete;
        };
        Select2Editor.prototype.getCreateSearchChoice = function (getName) {
            var _this = this;
            return function (s) {
                _this.lastCreateTerm = s;
                s = Q.coalesce(Select2.util.stripDiacritics(s), '').toLowerCase();
                if (Q.isTrimmedEmpty(s)) {
                    return null;
                }
                var isAsyncSource = false;
                if (Q.any(_this._items || [], function (x) {
                    var text = getName ? getName(x.source) : x.text;
                    return Select2.util.stripDiacritics(Q.coalesce(text, '')).toLowerCase() == s;
                }))
                    return null;
                if (!Q.any(_this._items || [], function (x1) {
                    return Q.coalesce(Select2.util.stripDiacritics(x1.text), '').toLowerCase().indexOf(s) !== -1;
                })) {
                    if (_this.isAutoComplete()) {
                        return {
                            id: _this.lastCreateTerm,
                            text: _this.lastCreateTerm
                        };
                    }
                    return {
                        id: (-2147483648).toString(),
                        text: Q.text('Controls.SelectEditor.NoResultsClickToDefine')
                    };
                }
                if (_this.isAutoComplete()) {
                    return {
                        id: _this.lastCreateTerm,
                        text: _this.lastCreateTerm
                    };
                }
                return {
                    id: (-2147483648).toString(),
                    text: Q.text('Controls.SelectEditor.ClickToDefine')
                };
            };
        };
        Select2Editor.prototype.setEditValue = function (source, property) {
            var val = source[property.name];
            if (Q.isArray(val)) {
                this.set_values(val);
            }
            else {
                this.set_value((val == null ? null : val.toString()));
            }
        };
        Select2Editor.prototype.getEditValue = function (property, target) {
            if (!this.isMultiple() || this.get_delimited()) {
                target[property.name] = this.get_value();
            }
            else {
                target[property.name] = this.get_values();
            }
        };
        Select2Editor.prototype.get_select2Container = function () {
            return this.element.prevAll('.select2-container');
        };
        Select2Editor.prototype.get_items = function () {
            return this.items;
        };
        Select2Editor.prototype.get_itemByKey = function () {
            return this.itemById;
        };
        Select2Editor.filterByText = function (items, getText, term) {
            if (term == null || term.length == 0)
                return items;
            term = Select2.util.stripDiacritics(term).toUpperCase();
            var contains = [];
            function filter(item) {
                var text = getText(item);
                if (text == null || !text.length)
                    return false;
                text = Select2.util.stripDiacritics(text).toUpperCase();
                if (Q.startsWith(text, term))
                    return true;
                if (text.indexOf(term) >= 0)
                    contains.push(item);
                return false;
            }
            return items.filter(filter).concat(contains);
        };
        Select2Editor.prototype.get_value = function () {
            var val;
            if (this.element.data('select2')) {
                val = this.element.select2('val');
                if (val != null && Q.isArray(val)) {
                    return val.join(',');
                }
            }
            else
                val = this.element.val();
            return val;
        };
        Object.defineProperty(Select2Editor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_value = function (value) {
            if (value != this.get_value()) {
                var val = value;
                if (!Q.isEmptyOrNull(value) && this.isMultiple()) {
                    val = value.split(String.fromCharCode(44)).map(function (x) {
                        return Q.trimToNull(x);
                    }).filter(function (x1) {
                        return x1 != null;
                    });
                }
                var el = this.element;
                el.select2('val', val);
                el.data('select2-change-triggered', true);
                try {
                    el.triggerHandler('change');
                }
                finally {
                    el.data('select2-change-triggered', false);
                }
                this.updateInplaceReadOnly();
            }
        };
        Object.defineProperty(Select2Editor.prototype, "selectedItem", {
            get: function () {
                var selectedValue = this.get_value();
                if (selectedValue && this._itemById) {
                    var item = this._itemById[selectedValue];
                    if (item)
                        return item.source;
                }
                return null;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(Select2Editor.prototype, "selectedItems", {
            get: function () {
                var selectedValues = this.values;
                var result = [];
                for (var _i = 0, selectedValues_1 = selectedValues; _i < selectedValues_1.length; _i++) {
                    var value = selectedValues_1[_i];
                    if (value && this._itemById) {
                        var item = this._itemById[value];
                        if (item && item.source)
                            result.push(item.source);
                        else
                            result.push(null);
                    }
                }
                return result;
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.get_values = function () {
            var val = this.element.select2('val');
            if (val == null) {
                return [];
            }
            if (Q.isArray(val)) {
                return val;
            }
            var str = val;
            if (Q.isEmptyOrNull(str)) {
                return [];
            }
            return [str];
        };
        Object.defineProperty(Select2Editor.prototype, "values", {
            get: function () {
                return this.get_values();
            },
            set: function (value) {
                this.set_values(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_values = function (value) {
            if (value == null || value.length === 0) {
                this.set_value(null);
                return;
            }
            this.set_value(value.join(','));
        };
        Select2Editor.prototype.get_text = function () {
            return Q.coalesce(this.element.select2('data'), {}).text;
        };
        Object.defineProperty(Select2Editor.prototype, "text", {
            get: function () {
                return this.get_text();
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.get_readOnly = function () {
            return !Q.isEmptyOrNull(this.element.attr('readonly'));
        };
        Object.defineProperty(Select2Editor.prototype, "readOnly", {
            get: function () {
                return this.get_readOnly();
            },
            set: function (value) {
                this.set_readOnly(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.updateInplaceReadOnly = function () {
            var readOnly = this.get_readOnly() &&
                (this.isMultiple() || !this.value);
            this.element.nextAll('.inplace-create')
                .attr('disabled', (readOnly ? 'disabled' : ''))
                .css('opacity', (readOnly ? '0.1' : ''))
                .css('cursor', (readOnly ? 'default' : ''));
        };
        Select2Editor.prototype.set_readOnly = function (value) {
            if (value !== this.get_readOnly()) {
                this.element.attr("readonly", value ? "readonly" : null);
                this.updateInplaceReadOnly();
            }
        };
        Select2Editor.prototype.getCascadeFromValue = function (parent) {
            return Serenity.EditorUtils.getValue(parent);
        };
        Select2Editor.prototype.setCascadeFrom = function (value) {
            var _this = this;
            if (Q.isEmptyOrNull(value)) {
                if (this.cascadeLink != null) {
                    this.cascadeLink.set_parentID(null);
                    this.cascadeLink = null;
                }
                this.options.cascadeFrom = null;
                return;
            }
            this.cascadeLink = new Serenity.CascadedWidgetLink(Serenity.Widget, this, function (p) {
                _this.set_cascadeValue(_this.getCascadeFromValue(p));
            });
            this.cascadeLink.set_parentID(value);
            this.options.cascadeFrom = value;
        };
        Select2Editor.prototype.get_cascadeFrom = function () {
            return this.options.cascadeFrom;
        };
        Object.defineProperty(Select2Editor.prototype, "cascadeFrom", {
            get: function () {
                return this.get_cascadeFrom();
            },
            set: function (value) {
                this.set_cascadeFrom(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_cascadeFrom = function (value) {
            if (value !== this.options.cascadeFrom) {
                this.setCascadeFrom(value);
                this.updateItems();
            }
        };
        Select2Editor.prototype.get_cascadeField = function () {
            return Q.coalesce(this.options.cascadeField, this.options.cascadeFrom);
        };
        Object.defineProperty(Select2Editor.prototype, "cascadeField", {
            get: function () {
                return this.get_cascadeField();
            },
            set: function (value) {
                this.set_cascadeField(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_cascadeField = function (value) {
            this.options.cascadeField = value;
        };
        Select2Editor.prototype.get_cascadeValue = function () {
            return this.options.cascadeValue;
        };
        Object.defineProperty(Select2Editor.prototype, "cascadeValue", {
            get: function () {
                return this.get_cascadeValue();
            },
            set: function (value) {
                this.set_cascadeValue(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_cascadeValue = function (value) {
            if (this.options.cascadeValue !== value) {
                this.options.cascadeValue = value;
                this.set_value(null);
                this.updateItems();
            }
        };
        Select2Editor.prototype.get_filterField = function () {
            return this.options.filterField;
        };
        Object.defineProperty(Select2Editor.prototype, "filterField", {
            get: function () {
                return this.get_filterField();
            },
            set: function (value) {
                this.set_filterField(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_filterField = function (value) {
            this.options.filterField = value;
        };
        Select2Editor.prototype.get_filterValue = function () {
            return this.options.filterValue;
        };
        Object.defineProperty(Select2Editor.prototype, "filterValue", {
            get: function () {
                return this.get_filterValue();
            },
            set: function (value) {
                this.set_filterValue(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_filterValue = function (value) {
            if (this.options.filterValue !== value) {
                this.options.filterValue = value;
                this.set_value(null);
                this.updateItems();
            }
        };
        Select2Editor.prototype.cascadeItems = function (items) {
            var val = this.get_cascadeValue();
            if (val == null || val === '') {
                if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                    return [];
                }
                return items;
            }
            var key = val.toString();
            var fld = this.get_cascadeField();
            return items.filter(function (x) {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
                return !!(itemKey != null && itemKey.toString() === key);
            });
        };
        Select2Editor.prototype.filterItems = function (items) {
            var val = this.get_filterValue();
            if (val == null || val === '') {
                return items;
            }
            var key = val.toString();
            var fld = this.get_filterField();
            return items.filter(function (x) {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
                return !!(itemKey != null && itemKey.toString() === key);
            });
        };
        Select2Editor.prototype.updateItems = function () {
        };
        Select2Editor.prototype.getDialogTypeKey = function () {
            if (this.options.dialogType != null) {
                return this.options.dialogType;
            }
            return null;
        };
        Select2Editor.prototype.createEditDialog = function (callback) {
            var dialogTypeKey = this.getDialogTypeKey();
            var dialogType = Serenity.DialogTypeRegistry.get(dialogTypeKey);
            Serenity.Widget.create({
                type: dialogType,
                init: function (x) { return callback(x); }
            });
        };
        Select2Editor.prototype.initNewEntity = function (entity) {
            if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                entity[this.get_cascadeField()] = this.get_cascadeValue();
            }
            if (!Q.isEmptyOrNull(this.get_filterField())) {
                entity[this.get_filterField()] = this.get_filterValue();
            }
            if (this.onInitNewEntity != null) {
                this.onInitNewEntity(entity);
            }
        };
        Select2Editor.prototype.setEditDialogReadOnly = function (dialog) {
            // an ugly workaround
            dialog.element && dialog.element
                .find('.tool-button.delete-button')
                .addClass('disabled')
                .unbind('click');
        };
        Select2Editor.prototype.editDialogDataChange = function () {
        };
        Select2Editor.prototype.setTermOnNewEntity = function (entity, term) {
        };
        Select2Editor.prototype.inplaceCreateClick = function (e) {
            var _this = this;
            if (this.get_readOnly() &&
                ((this.isMultiple() && !e['editItem']) || !this.value))
                return;
            this.createEditDialog(function (dialog) {
                if (_this.get_readOnly())
                    _this.setEditDialogReadOnly(dialog);
                Serenity.SubDialogHelper.bindToDataChange(dialog, _this, function (x, dci) {
                    _this.editDialogDataChange();
                    _this.updateItems();
                    _this.lastCreateTerm = null;
                    if ((dci.type === 'create' || dci.type === 'update') &&
                        dci.entityId != null) {
                        var id = dci.entityId.toString();
                        if (_this.isMultiple()) {
                            var values = _this.get_values().slice();
                            if (values.indexOf(id) < 0) {
                                values.push(id);
                            }
                            _this.set_values(null);
                            _this.set_values(values.slice());
                        }
                        else {
                            _this.set_value(null);
                            _this.set_value(id);
                        }
                    }
                    else if (_this.isMultiple() && dci.type === 'delete' &&
                        dci.entityId != null) {
                        var id1 = dci.entityId.toString();
                        var values1 = _this.get_values().slice();
                        var idx1 = values1.indexOf(id1);
                        if (idx1 >= 0)
                            values1.splice(idx1, 1);
                        _this.set_values(values1.slice());
                    }
                    else if (!_this.isMultiple()) {
                        _this.set_value(null);
                    }
                }, true);
                var editItem = e['editItem'];
                if (editItem != null) {
                    dialog.load(editItem, function () {
                        dialog.dialogOpen(_this.openDialogAsPanel);
                    }, null);
                }
                else if (_this.isMultiple() || Q.isEmptyOrNull(_this.get_value())) {
                    var entity = {};
                    _this.setTermOnNewEntity(entity, Q.trimToEmpty(_this.lastCreateTerm));
                    _this.initNewEntity(entity);
                    dialog.load(entity, function () {
                        dialog.dialogOpen(_this.openDialogAsPanel);
                    }, null);
                }
                else {
                    dialog.load(_this.get_value(), function () {
                        dialog.dialogOpen(_this.openDialogAsPanel);
                    }, null);
                }
            });
        };
        var Select2Editor_1;
        Select2Editor = Select2Editor_1 = __decorate([
            Serenity.Decorators.registerClass('Serenity.Select2Editor', [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element("<input type=\"hidden\"/>")
        ], Select2Editor);
        return Select2Editor;
    }(Serenity.Widget));
    Serenity.Select2Editor = Select2Editor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var SelectEditor = /** @class */ (function (_super) {
        __extends(SelectEditor, _super);
        function SelectEditor(hidden, opt) {
            var _this = _super.call(this, hidden, opt) || this;
            _this.updateItems();
            return _this;
        }
        SelectEditor.prototype.getItems = function () {
            return this.options.items || [];
        };
        SelectEditor.prototype.emptyItemText = function () {
            if (!Q.isEmptyOrNull(this.options.emptyOptionText)) {
                return this.options.emptyOptionText;
            }
            return _super.prototype.emptyItemText.call(this);
        };
        SelectEditor.prototype.updateItems = function () {
            var items = this.getItems();
            this.clearItems();
            if (items.length > 0) {
                var isStrings = typeof (items[0]) === 'string';
                for (var _i = 0, items_2 = items; _i < items_2.length; _i++) {
                    var item = items_2[_i];
                    var key = isStrings ? item : item[0];
                    var text = isStrings ? item : Q.coalesce(item[1], item[0]);
                    this.addOption(key, text, item, false);
                }
            }
        };
        SelectEditor = __decorate([
            Serenity.Decorators.registerClass('Serenity.SelectEditor')
        ], SelectEditor);
        return SelectEditor;
    }(Serenity.Select2Editor));
    Serenity.SelectEditor = SelectEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var DateYearEditor = /** @class */ (function (_super) {
        __extends(DateYearEditor, _super);
        function DateYearEditor(hidden, opt) {
            var _this = _super.call(this, hidden, opt) || this;
            _this.updateItems();
            return _this;
        }
        DateYearEditor.prototype.getItems = function () {
            var opt = this.options;
            if (opt.items != null && opt.items.length >= 1) {
                return opt.items;
            }
            var years = [];
            var minYear = (new Date()).getFullYear();
            var maxYear = (new Date()).getFullYear();
            opt.minYear = Q.coalesce(opt.minYear, '-10').toString();
            if (Q.startsWith(opt.minYear, '-')) {
                minYear -= parseInt(opt.minYear.substr(1), 10);
            }
            else if (Q.startsWith(opt.minYear, '+')) {
                minYear += parseInt(opt.minYear.substr(1), 10);
            }
            else {
                minYear = parseInt(opt.minYear, 10);
            }
            opt.maxYear = Q.coalesce(opt.maxYear, '+10').toString();
            if (Q.startsWith(opt.maxYear, '-')) {
                maxYear -= parseInt(opt.maxYear.substr(1), 10);
            }
            else if (Q.startsWith(opt.maxYear, '+')) {
                maxYear += parseInt(opt.maxYear.substr(1), 10);
            }
            else {
                maxYear = parseInt(opt.maxYear, 10);
            }
            if (opt.descending) {
                for (var i = maxYear; i >= minYear; i--) {
                    years.push(i.toString());
                }
            }
            else {
                for (var i1 = minYear; i1 <= maxYear; i1++) {
                    years.push(i1.toString());
                }
            }
            return years;
        };
        DateYearEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.DateYearEditor')
        ], DateYearEditor);
        return DateYearEditor;
    }(Serenity.SelectEditor));
    Serenity.DateYearEditor = DateYearEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EnumEditor = /** @class */ (function (_super) {
        __extends(EnumEditor, _super);
        function EnumEditor(hidden, opt) {
            var _this = _super.call(this, hidden, opt) || this;
            _this.updateItems();
            return _this;
        }
        EnumEditor.prototype.updateItems = function () {
            this.clearItems();
            var enumType = this.options.enumType || Serenity.EnumTypeRegistry.get(this.options.enumKey);
            var enumKey = this.options.enumKey;
            if (enumKey == null && enumType != null) {
                var enumKeyAttr = Q.getAttributes(enumType, Serenity.EnumKeyAttribute, false);
                if (enumKeyAttr.length > 0) {
                    enumKey = enumKeyAttr[0].value;
                }
            }
            var values = Q.Enum.getValues(enumType);
            for (var _i = 0, values_2 = values; _i < values_2.length; _i++) {
                var x = values_2[_i];
                var name = Q.Enum.toString(enumType, x);
                this.addOption(parseInt(x, 10).toString(), Q.coalesce(Q.tryGetText('Enums.' + enumKey + '.' + name), name), null, false);
            }
        };
        EnumEditor.prototype.allowClear = function () {
            return Q.coalesce(this.options.allowClear, true);
        };
        EnumEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.EnumEditor')
        ], EnumEditor);
        return EnumEditor;
    }(Serenity.Select2Editor));
    Serenity.EnumEditor = EnumEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var LookupEditorBase = /** @class */ (function (_super) {
        __extends(LookupEditorBase, _super);
        function LookupEditorBase(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            if (!_this.hasAsyncSource()) {
                _this.updateItems();
                var self = _this;
                Q.ScriptData.bindToChange('Lookup.' + _this.getLookupKey(), _this.uniqueName, function () {
                    self.updateItems();
                });
            }
            return _this;
        }
        LookupEditorBase.prototype.hasAsyncSource = function () {
            return !!this.options.async;
        };
        LookupEditorBase.prototype.destroy = function () {
            if (!this.hasAsyncSource())
                Q.ScriptData.unbindFromChange(this.uniqueName);
            _super.prototype.destroy.call(this);
        };
        LookupEditorBase.prototype.getLookupKey = function () {
            if (this.options.lookupKey != null) {
                return this.options.lookupKey;
            }
            var key = Q.getTypeFullName(Q.getInstanceType(this));
            var idx = key.indexOf('.');
            if (idx >= 0) {
                key = key.substring(idx + 1);
            }
            if (Q.endsWith(key, 'Editor')) {
                key = key.substr(0, key.length - 6);
            }
            return key;
        };
        LookupEditorBase.prototype.getLookupAsync = function () {
            return Q.getLookupAsync(this.getLookupKey());
        };
        LookupEditorBase.prototype.getLookup = function () {
            return Q.getLookup(this.getLookupKey());
        };
        LookupEditorBase.prototype.getItems = function (lookup) {
            return this.filterItems(this.cascadeItems(lookup.items));
        };
        LookupEditorBase.prototype.getIdField = function () {
            return this.lookup != null ? this.lookup.idField : _super.prototype.getIdField.call(this);
        };
        LookupEditorBase.prototype.getItemText = function (item, lookup) {
            if (lookup == null)
                return _super.prototype.itemText.call(this, item);
            var textValue = lookup.textFormatter ? lookup.textFormatter(item) : item[lookup.textField];
            return textValue == null ? '' : textValue.toString();
        };
        LookupEditorBase.prototype.mapItem = function (item) {
            return {
                id: this.itemId(item),
                text: this.getItemText(item, this.lookup),
                disabled: this.getItemDisabled(item, this.lookup),
                source: item
            };
        };
        LookupEditorBase.prototype.getItemDisabled = function (item, lookup) {
            return _super.prototype.itemDisabled.call(this, item);
        };
        LookupEditorBase.prototype.updateItems = function () {
            this.clearItems();
            this.lookup = this.getLookup();
            var items = this.getItems(this.lookup);
            for (var _i = 0, items_3 = items; _i < items_3.length; _i++) {
                var item = items_3[_i];
                this.addItem(this.mapItem(item));
            }
        };
        LookupEditorBase.prototype.asyncSearch = function (query, results) {
            var _this = this;
            return this.getLookupAsync().then(function (lookup) {
                _this.lookup = lookup;
                var items = _this.getItems(_this.lookup);
                if (query.idList != null) {
                    items = items.filter(function (x) { return query.idList.indexOf(_this.itemId(x)) >= 0; });
                }
                function getText(item) {
                    return this.getItemText(item, this.lookup);
                }
                items = Serenity.Select2Editor.filterByText(items, getText.bind(_this), query.searchTerm);
                results({
                    items: items.slice(query.skip, query.take),
                    more: items.length >= query.take
                });
            });
        };
        LookupEditorBase.prototype.getDialogTypeKey = function () {
            var dialogTypeKey = _super.prototype.getDialogTypeKey.call(this);
            if (dialogTypeKey)
                return dialogTypeKey;
            return this.getLookupKey();
        };
        LookupEditorBase.prototype.setCreateTermOnNewEntity = function (entity, term) {
            entity[this.getLookup().textField] = term;
        };
        LookupEditorBase.prototype.editDialogDataChange = function () {
            Q.reloadLookup(this.getLookupKey());
        };
        LookupEditorBase = __decorate([
            Serenity.Decorators.registerEditor("Serenity.LookupEditorBase")
        ], LookupEditorBase);
        return LookupEditorBase;
    }(Serenity.Select2Editor));
    Serenity.LookupEditorBase = LookupEditorBase;
    var LookupEditor = /** @class */ (function (_super) {
        __extends(LookupEditor, _super);
        function LookupEditor(hidden, opt) {
            return _super.call(this, hidden, opt) || this;
        }
        LookupEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.LookupEditor')
        ], LookupEditor);
        return LookupEditor;
    }(LookupEditorBase));
    Serenity.LookupEditor = LookupEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ServiceLookupEditorBase = /** @class */ (function (_super) {
        __extends(ServiceLookupEditorBase, _super);
        function ServiceLookupEditorBase(input, opt) {
            return _super.call(this, input, opt) || this;
        }
        ServiceLookupEditorBase.prototype.getDialogTypeKey = function () {
            var dialogTypeKey = _super.prototype.getDialogTypeKey.call(this);
            if (dialogTypeKey)
                return dialogTypeKey;
            var service = this.getService();
            if (Q.startsWith(service, "~/Services/"))
                service = service.substr("~/Services/".length);
            if (service.split('/').length == 3)
                service = service.substr(0, service.lastIndexOf('/'));
            return service.replace("/", ".");
        };
        ServiceLookupEditorBase.prototype.getService = function () {
            return this.options.service;
        };
        ServiceLookupEditorBase.prototype.getServiceUrl = function () {
            var url = this.getService();
            if (url == null)
                throw new Error("ServiceLookupEditor requires 'service' option to be configured!");
            if (!Q.startsWith(url, "~") && !Q.startsWith(url, "/") && url.indexOf('://') < 0)
                url = "~/Services/" + url;
            if (Q.startsWith(url, "~"))
                url = Q.resolveUrl(url);
            return url;
        };
        ServiceLookupEditorBase.prototype.getIncludeColumns = function () {
            var include = this.options.includeColumns || [];
            var idField = this.getIdField();
            if (idField && include.indexOf(idField) < 0)
                include.push(idField);
            var textField = this.getTextField();
            if (textField && include.indexOf(textField) < 0)
                include.push(textField);
            return include;
        };
        ServiceLookupEditorBase.prototype.getSort = function () {
            return this.options.sort || (this.getTextField() ? [this.getTextField()] : null);
        };
        ServiceLookupEditorBase.prototype.getCascadeCriteria = function () {
            var val = this.get_cascadeValue();
            if (val == null || val === '') {
                if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                    return ['1', '=', '0'];
                }
                return null;
            }
            var fld = this.get_cascadeField();
            return [[fld], '=', val];
        };
        ServiceLookupEditorBase.prototype.getFilterCriteria = function () {
            var val = this.get_filterValue();
            if (val == null || val === '') {
                return null;
            }
            var fld = this.get_filterField();
            return [[fld], '=', val];
        };
        ServiceLookupEditorBase.prototype.getIdListCriteria = function (idList) {
            if (idList == null)
                return null;
            if (idList.length == 0)
                return ['0', '=', '1'];
            var idField = this.getIdField();
            if (idField == null)
                throw new Error("ServiceLookupEditor requires 'idField' option to be configured!");
            return [[idField], 'in', [idList]];
        };
        ServiceLookupEditorBase.prototype.getCriteria = function (query) {
            return Serenity.Criteria.and(Serenity.Criteria.and(this.getIdListCriteria(query.idList), this.options.criteria), Serenity.Criteria.and(this.getCascadeCriteria(), this.getFilterCriteria()));
        };
        ServiceLookupEditorBase.prototype.getListRequest = function (query) {
            var request = {};
            if (query.searchTerm)
                request.ContainsText = query.searchTerm;
            request.Sort = this.getSort();
            request.ColumnSelection = this.options.columnSelection || 1 /* KeyOnly */;
            request.IncludeColumns = this.getIncludeColumns();
            request.ExcludeColumns = this.options.excludeColumns;
            request.ContainsField = this.options.containsField;
            request.EqualityFilter = this.options.equalityFilter;
            request.Criteria = this.getCriteria(query);
            request.Skip = query.skip || 0;
            request.Take = query.take ? (query.checkMore ? query.take + 1 : query.take) : 0;
            request.IncludeDeleted = this.options.includeDeleted;
            request.ExcludeTotalCount = true;
            return request;
        };
        ServiceLookupEditorBase.prototype.getServiceCallOptions = function (query, results) {
            return {
                blockUI: false,
                url: this.getServiceUrl(),
                request: this.getListRequest(query),
                onSuccess: function (response) {
                    var items = response.Entities || [];
                    if (items && query.take && query.checkMore && response.Entities.length > items.length)
                        items = items.slice(0, query.take);
                    results({
                        items: items.slice(0, query.take),
                        more: query.checkMore && query.take && items.length > query.take
                    });
                }
            };
        };
        ServiceLookupEditorBase.prototype.hasAsyncSource = function () {
            return true;
        };
        ServiceLookupEditorBase.prototype.asyncSearch = function (query, results) {
            var opt = this.getServiceCallOptions(query, results);
            return Q.serviceCall(opt);
        };
        ServiceLookupEditorBase = __decorate([
            Serenity.Decorators.registerEditor("Serenity.ServiceLookupEditorBase")
        ], ServiceLookupEditorBase);
        return ServiceLookupEditorBase;
    }(Serenity.Select2Editor));
    Serenity.ServiceLookupEditorBase = ServiceLookupEditorBase;
    var ServiceLookupEditor = /** @class */ (function (_super) {
        __extends(ServiceLookupEditor, _super);
        function ServiceLookupEditor(hidden, opt) {
            return _super.call(this, hidden, opt) || this;
        }
        ServiceLookupEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.ServiceLookupEditor')
        ], ServiceLookupEditor);
        return ServiceLookupEditor;
    }(ServiceLookupEditorBase));
    Serenity.ServiceLookupEditor = ServiceLookupEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var HtmlContentEditor = /** @class */ (function (_super) {
        __extends(HtmlContentEditor, _super);
        function HtmlContentEditor(textArea, opt) {
            var _this = _super.call(this, textArea, opt) || this;
            _this._instanceReady = false;
            HtmlContentEditor_1.includeCKEditor();
            var id = textArea.attr('id');
            if (Q.isTrimmedEmpty(id)) {
                textArea.attr('id', _this.uniqueName);
                id = _this.uniqueName;
            }
            if (_this.options.cols != null)
                textArea.attr('cols', _this.options.cols);
            if (_this.options.rows != null)
                textArea.attr('rows', _this.options.rows);
            _this.addValidationRule(_this.uniqueName, function (e) {
                if (e.hasClass('required')) {
                    var value = Q.trimToNull(_this.get_value());
                    if (value == null)
                        return Q.text('Validation.Required');
                }
                return null;
            });
            Serenity.LazyLoadHelper.executeOnceWhenShown(_this.element, function () {
                var config = _this.getConfig();
                window['CKEDITOR'] && window['CKEDITOR'].replace(id, config);
            });
            return _this;
        }
        HtmlContentEditor_1 = HtmlContentEditor;
        HtmlContentEditor.prototype.instanceReady = function (x) {
            this._instanceReady = true;
            $(x.editor.container.$).addClass(this.element.attr('class'));
            this.element.addClass('select2-offscreen').css('display', 'block');
            // for validation to work
            x.editor.setData(this.element.val());
            x.editor.setReadOnly(this.get_readOnly());
        };
        HtmlContentEditor.prototype.getLanguage = function () {
            if (!window['CKEDITOR'])
                return 'en';
            var CKEDITOR = window['CKEDITOR'];
            var lang = Q.coalesce(Q.trimToNull($('html').attr('lang')), 'en');
            if (!!CKEDITOR.lang.languages[lang]) {
                return lang;
            }
            if (lang.indexOf(String.fromCharCode(45)) >= 0) {
                lang = lang.split(String.fromCharCode(45))[0];
            }
            if (!!CKEDITOR.lang.languages[lang]) {
                return lang;
            }
            return 'en';
        };
        HtmlContentEditor.prototype.getConfig = function () {
            var _this = this;
            return {
                customConfig: '',
                language: this.getLanguage(),
                bodyClass: 's-HtmlContentBody',
                on: {
                    instanceReady: function (x) { return _this.instanceReady(x); },
                    change: function (x1) {
                        x1.editor.updateElement();
                        _this.element.triggerHandler('change');
                    }
                },
                toolbarGroups: [
                    {
                        name: 'clipboard',
                        groups: ['clipboard', 'undo']
                    }, {
                        name: 'editing',
                        groups: ['find', 'selection', 'spellchecker']
                    }, {
                        name: 'insert',
                        groups: ['links', 'insert', 'blocks', 'bidi', 'list', 'indent']
                    }, {
                        name: 'forms',
                        groups: ['forms', 'mode', 'document', 'doctools', 'others', 'about', 'tools']
                    }, {
                        name: 'colors'
                    }, {
                        name: 'basicstyles',
                        groups: ['basicstyles', 'cleanup']
                    }, {
                        name: 'align'
                    }, {
                        name: 'styles'
                    }
                ],
                removeButtons: 'SpecialChar,Anchor,Subscript,Styles',
                format_tags: 'p;h1;h2;h3;pre',
                removeDialogTabs: 'image:advanced;link:advanced',
                removePlugins: 'uploadimage,image2',
                contentsCss: Q.resolveUrl('~/Content/site/site.htmlcontent.css'),
                entities: false,
                entities_latin: false,
                entities_greek: false,
                autoUpdateElement: true,
                height: (this.options.rows == null || this.options.rows === 0) ? null :
                    ((this.options.rows * 20) + 'px')
            };
        };
        HtmlContentEditor.prototype.getEditorInstance = function () {
            var id = this.element.attr('id');
            return window['CKEDITOR'].instances[id];
        };
        HtmlContentEditor.prototype.destroy = function () {
            var instance = this.getEditorInstance();
            instance && instance.destroy(true);
            _super.prototype.destroy.call(this);
        };
        HtmlContentEditor.prototype.get_value = function () {
            var instance = this.getEditorInstance();
            if (this._instanceReady && instance) {
                return instance.getData();
            }
            else {
                return this.element.val();
            }
        };
        Object.defineProperty(HtmlContentEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        HtmlContentEditor.prototype.set_value = function (value) {
            var instance = this.getEditorInstance();
            this.element.val(value);
            if (this._instanceReady && instance)
                instance.setData(value);
        };
        HtmlContentEditor.prototype.get_readOnly = function () {
            return !Q.isEmptyOrNull(this.element.attr('disabled'));
        };
        HtmlContentEditor.prototype.set_readOnly = function (value) {
            if (this.get_readOnly() !== value) {
                if (value) {
                    this.element.attr('disabled', 'disabled');
                }
                else {
                    this.element.removeAttr('disabled');
                }
                var instance = this.getEditorInstance();
                if (this._instanceReady && instance)
                    instance.setReadOnly(value);
            }
        };
        HtmlContentEditor.includeCKEditor = function () {
            if (window['CKEDITOR']) {
                return;
            }
            var script = $('#CKEditorScript');
            if (script.length > 0) {
                return;
            }
            $('<script/>').attr('type', 'text/javascript')
                .attr('id', 'CKEditorScript')
                .attr('src', Q.resolveUrl('~/Scripts/CKEditor/ckeditor.js?v=' +
                HtmlContentEditor_1.CKEditorVer))
                .appendTo(window.document.head);
        };
        ;
        var HtmlContentEditor_1;
        HtmlContentEditor.CKEditorVer = "4.7.1";
        HtmlContentEditor = HtmlContentEditor_1 = __decorate([
            Serenity.Decorators.registerEditor('Serenity.HtmlContentEditor', [Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element('<textarea/>')
        ], HtmlContentEditor);
        return HtmlContentEditor;
    }(Serenity.Widget));
    Serenity.HtmlContentEditor = HtmlContentEditor;
    var HtmlNoteContentEditor = /** @class */ (function (_super) {
        __extends(HtmlNoteContentEditor, _super);
        function HtmlNoteContentEditor(textArea, opt) {
            return _super.call(this, textArea, opt) || this;
        }
        HtmlNoteContentEditor.prototype.getConfig = function () {
            var config = _super.prototype.getConfig.call(this);
            config.removeButtons += ',Cut,Copy,Paste,BulletedList,NumberedList,' +
                'Indent,Outdent,SpecialChar,Subscript,Superscript,Styles,PasteText,' +
                'PasteFromWord,Strike,Link,Unlink,CreatePlaceholder,Image,Table,' +
                'HorizontalRule,Source,Maximize,Format,Font,FontSize,Anchor,Blockquote,' +
                'CreatePlaceholder,BGColor,JustifyLeft,JustifyCenter,' +
                'JustifyRight,JustifyBlock,Superscript,RemoveFormat';
            config.removePlugins = 'elementspath,uploadimage,image2';
            return config;
        };
        HtmlNoteContentEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.HtmlNoteContentEditor')
        ], HtmlNoteContentEditor);
        return HtmlNoteContentEditor;
    }(HtmlContentEditor));
    Serenity.HtmlNoteContentEditor = HtmlNoteContentEditor;
    var HtmlReportContentEditor = /** @class */ (function (_super) {
        __extends(HtmlReportContentEditor, _super);
        function HtmlReportContentEditor(textArea, opt) {
            return _super.call(this, textArea, opt) || this;
        }
        HtmlReportContentEditor.prototype.getConfig = function () {
            var config = _super.prototype.getConfig.call(this);
            config.removeButtons += ',Cut,Copy,Paste,BulletedList,NumberedList,' +
                'Indent,Outdent,SpecialChar,Subscript,Superscript,Styles,' +
                'PasteText,PasteFromWord,Strike,Link,Unlink,CreatePlaceholder,' +
                'Image,Table,HorizontalRule,Source,Maximize,Format,Font,FontSize,' +
                'Anchor,Blockquote,CreatePlaceholder,BGColor,JustifyLeft,JustifyCenter,' +
                'JustifyRight,JustifyBlock,Superscript,RemoveFormat';
            config.removePlugins = 'elementspath,uploadimage,image2';
            return config;
        };
        HtmlReportContentEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.HtmlReportContentEditor')
        ], HtmlReportContentEditor);
        return HtmlReportContentEditor;
    }(HtmlContentEditor));
    Serenity.HtmlReportContentEditor = HtmlReportContentEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    // http://digitalbush.com/projects/masked-input-plugin/
    var MaskedEditor = /** @class */ (function (_super) {
        __extends(MaskedEditor, _super);
        function MaskedEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.mask(_this.options.mask || '', {
                placeholder: Q.coalesce(_this.options.placeholder, '_')
            });
            return _this;
        }
        Object.defineProperty(MaskedEditor.prototype, "value", {
            get: function () {
                this.element.triggerHandler("blur.mask");
                return this.element.val();
            },
            set: function (value) {
                this.element.val(value);
            },
            enumerable: true,
            configurable: true
        });
        MaskedEditor.prototype.get_value = function () {
            return this.value;
        };
        MaskedEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        MaskedEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.MaskedEditor', [Serenity.IStringValue]),
            Serenity.Decorators.element("<input type=\"text\"/>")
        ], MaskedEditor);
        return MaskedEditor;
    }(Serenity.Widget));
    Serenity.MaskedEditor = MaskedEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Recaptcha = /** @class */ (function (_super) {
        __extends(Recaptcha, _super);
        function Recaptcha(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            _this.element.addClass('g-recaptcha').attr('data-sitekey', _this.options.siteKey);
            if (!!(window['grecaptcha'] == null && $('script#RecaptchaInclude').length === 0)) {
                var src = 'https://www.google.com/recaptcha/api.js';
                var lng = _this.options.language;
                if (lng == null) {
                    lng = Q.coalesce($('html').attr('lang'), '');
                }
                src += '?hl=' + lng;
                $('<script/>').attr('id', 'RecaptchaInclude').attr('src', src).appendTo(document.body);
            }
            var valInput = $('<input />').insertBefore(_this.element)
                .attr('id', _this.uniqueName + '_validate').val('x');
            var gro = {};
            gro['visibility'] = 'hidden';
            gro['width'] = '0px';
            gro['height'] = '0px';
            gro['padding'] = '0px';
            var input = valInput.css(gro);
            var self = _this;
            Q.addValidationRule(input, _this.uniqueName, function (e) {
                if (Q.isEmptyOrNull(_this.get_value())) {
                    return Q.text('Validation.Required');
                }
                return null;
            });
            return _this;
        }
        Recaptcha.prototype.get_value = function () {
            return this.element.find('.g-recaptcha-response').val();
        };
        Recaptcha.prototype.set_value = function (value) {
            // ignore
        };
        Recaptcha = __decorate([
            Serenity.Decorators.registerEditor('Serenity.Recaptcha', [Serenity.IStringValue]),
            Serenity.Decorators.element("<div/>")
        ], Recaptcha);
        return Recaptcha;
    }(Serenity.Widget));
    Serenity.Recaptcha = Recaptcha;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var FileUploadEditor = /** @class */ (function (_super) {
        __extends(FileUploadEditor, _super);
        function FileUploadEditor(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            if (!opt || opt.allowNonImage == null)
                _this.options.allowNonImage = true;
            div.addClass('s-FileUploadEditor');
            if (Q.isEmptyOrNull(_this.options.originalNameProperty))
                div.addClass('hide-original-name');
            _this.toolbar = new Serenity.Toolbar($('<div/>').appendTo(_this.element), {
                buttons: _this.getToolButtons()
            });
            var progress = $('<div><div></div></div>')
                .addClass('upload-progress')
                .prependTo(_this.toolbar.element);
            var uio = _this.getUploadInputOptions();
            _this.uploadInput = Serenity.UploadHelper.addUploadInput(uio);
            _this.fileSymbols = $('<ul/>').appendTo(_this.element);
            _this.updateInterface();
            return _this;
        }
        FileUploadEditor.prototype.getUploadInputOptions = function () {
            var _this = this;
            return {
                container: this.toolbar.findButton('add-file-button'),
                zone: this.element,
                inputName: this.uniqueName,
                progress: this.progress,
                fileDone: function (response, name, data) {
                    if (!Serenity.UploadHelper.checkImageConstraints(response, _this.options)) {
                        return;
                    }
                    var newEntity = {
                        OriginalName: name,
                        Filename: response.TemporaryFile
                    };
                    _this.entity = newEntity;
                    _this.populate();
                    _this.updateInterface();
                }
            };
        };
        FileUploadEditor.prototype.addFileButtonText = function () {
            return Q.text('Controls.ImageUpload.AddFileButton');
        };
        FileUploadEditor.prototype.getToolButtons = function () {
            var _this = this;
            return [
                {
                    title: this.addFileButtonText(),
                    cssClass: 'add-file-button',
                    onClick: function () {
                    }
                },
                {
                    title: '',
                    hint: Q.text('Controls.ImageUpload.DeleteButtonHint'),
                    cssClass: 'delete-button',
                    onClick: function () {
                        _this.entity = null;
                        _this.populate();
                        _this.updateInterface();
                    }
                }
            ];
        };
        FileUploadEditor.prototype.populate = function () {
            var displayOriginalName = this.options.displayFileName ||
                !Q.isTrimmedEmpty(this.options.originalNameProperty);
            if (this.entity == null) {
                Serenity.UploadHelper.populateFileSymbols(this.fileSymbols, null, displayOriginalName, this.options.urlPrefix);
            }
            else {
                Serenity.UploadHelper.populateFileSymbols(this.fileSymbols, [this.entity], displayOriginalName, this.options.urlPrefix);
            }
        };
        FileUploadEditor.prototype.updateInterface = function () {
            var addButton = this.toolbar.findButton('add-file-button');
            var delButton = this.toolbar.findButton('delete-button');
            addButton.toggleClass('disabled', this.get_readOnly());
            delButton.toggleClass('disabled', this.get_readOnly() ||
                this.entity == null);
        };
        FileUploadEditor.prototype.get_readOnly = function () {
            return this.uploadInput.attr('disabled') != null;
        };
        FileUploadEditor.prototype.set_readOnly = function (value) {
            if (this.get_readOnly() !== value) {
                if (value) {
                    this.uploadInput.attr('disabled', 'disabled').fileupload('disable');
                }
                else {
                    this.uploadInput.removeAttr('disabled').fileupload('enable');
                }
                this.updateInterface();
            }
        };
        FileUploadEditor.prototype.get_value = function () {
            if (this.entity == null) {
                return null;
            }
            var copy = Q.extend({}, this.entity);
            return copy;
        };
        Object.defineProperty(FileUploadEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        FileUploadEditor.prototype.set_value = function (value) {
            if (value != null) {
                if (value.Filename == null) {
                    this.entity = null;
                }
                else {
                    this.entity = Q.extend({}, value);
                }
            }
            else {
                this.entity = null;
            }
            this.populate();
            this.updateInterface();
        };
        FileUploadEditor.prototype.getEditValue = function (property, target) {
            target[property.name] = this.entity == null ? null :
                Q.trimToNull(this.entity.Filename);
        };
        FileUploadEditor.prototype.setEditValue = function (source, property) {
            var value = {};
            value.Filename = source[property.name];
            if (Q.isEmptyOrNull(this.options.originalNameProperty)) {
                if (this.options.displayFileName) {
                    var s = Q.coalesce(value.Filename, '');
                    var idx = Q.replaceAll(s, '\\', '/').lastIndexOf('/');
                    if (idx >= 0) {
                        value.OriginalName = s.substr(idx + 1);
                    }
                    else {
                        value.OriginalName = s;
                    }
                }
            }
            else {
                value.OriginalName = source[this.options.originalNameProperty];
            }
            this.set_value(value);
        };
        FileUploadEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.FileUploadEditor', [Serenity.IReadOnly]),
            Serenity.Decorators.element('<div/>')
        ], FileUploadEditor);
        return FileUploadEditor;
    }(Serenity.Widget));
    Serenity.FileUploadEditor = FileUploadEditor;
    var ImageUploadEditor = /** @class */ (function (_super) {
        __extends(ImageUploadEditor, _super);
        function ImageUploadEditor(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            if (opt && opt.allowNonImage == null)
                _this.options.allowNonImage = false;
            div.addClass('s-ImageUploadEditor');
            return _this;
        }
        ImageUploadEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.ImageUploadEditor')
        ], ImageUploadEditor);
        return ImageUploadEditor;
    }(FileUploadEditor));
    Serenity.ImageUploadEditor = ImageUploadEditor;
    var MultipleFileUploadEditor = /** @class */ (function (_super) {
        __extends(MultipleFileUploadEditor, _super);
        function MultipleFileUploadEditor(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            _this.entities = [];
            div.addClass('s-MultipleFileUploadEditor');
            var self = _this;
            _this.toolbar = new Serenity.Toolbar($('<div/>').appendTo(_this.element), {
                buttons: _this.getToolButtons()
            });
            var progress = $('<div><div></div></div>')
                .addClass('upload-progress').prependTo(_this.toolbar.element);
            var addFileButton = _this.toolbar.findButton('add-file-button');
            _this.uploadInput = Serenity.UploadHelper.addUploadInput({
                container: addFileButton,
                zone: _this.element,
                inputName: _this.uniqueName,
                progress: progress,
                fileDone: function (response, name, data) {
                    if (!Serenity.UploadHelper.checkImageConstraints(response, _this.options)) {
                        return;
                    }
                    var newEntity = { OriginalName: name, Filename: response.TemporaryFile };
                    self.entities.push(newEntity);
                    self.populate();
                    self.updateInterface();
                }
            });
            _this.fileSymbols = $('<ul/>').appendTo(_this.element);
            _this.updateInterface();
            return _this;
        }
        MultipleFileUploadEditor.prototype.addFileButtonText = function () {
            return Q.text('Controls.ImageUpload.AddFileButton');
        };
        MultipleFileUploadEditor.prototype.getToolButtons = function () {
            return [{
                    title: this.addFileButtonText(),
                    cssClass: 'add-file-button',
                    onClick: function () {
                    }
                }];
        };
        MultipleFileUploadEditor.prototype.populate = function () {
            var _this = this;
            Serenity.UploadHelper.populateFileSymbols(this.fileSymbols, this.entities, true, this.options.urlPrefix);
            this.fileSymbols.children().each(function (i, e) {
                var x = i;
                $("<a class='delete'></a>").appendTo($(e).children('.filename'))
                    .click(function (ev) {
                    ev.preventDefault();
                    _this.entities.splice(x, 1);
                    _this.populate();
                });
            });
        };
        MultipleFileUploadEditor.prototype.updateInterface = function () {
            var addButton = this.toolbar.findButton('add-file-button');
            addButton.toggleClass('disabled', this.get_readOnly());
            this.fileSymbols.find('a.delete').toggle(!this.get_readOnly());
        };
        MultipleFileUploadEditor.prototype.get_readOnly = function () {
            return this.uploadInput.attr('disabled') != null;
        };
        MultipleFileUploadEditor.prototype.set_readOnly = function (value) {
            if (this.get_readOnly() !== value) {
                if (value) {
                    this.uploadInput.attr('disabled', 'disabled').fileupload('disable');
                }
                else {
                    this.uploadInput.removeAttr('disabled').fileupload('enable');
                }
                this.updateInterface();
            }
        };
        MultipleFileUploadEditor.prototype.get_value = function () {
            return this.entities.map(function (x) {
                return Q.extend({}, x);
            });
        };
        Object.defineProperty(MultipleFileUploadEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        MultipleFileUploadEditor.prototype.set_value = function (value) {
            this.entities = (value || []).map(function (x) {
                return Q.extend({}, x);
            });
            this.populate();
            this.updateInterface();
        };
        MultipleFileUploadEditor.prototype.getEditValue = function (property, target) {
            if (this.jsonEncodeValue) {
                target[property.name] = $.toJSON(this.get_value());
            }
            else {
                target[property.name] = this.get_value();
            }
        };
        MultipleFileUploadEditor.prototype.setEditValue = function (source, property) {
            var val = source[property.name];
            if (Q.isInstanceOfType(val, String)) {
                var json = Q.coalesce(Q.trimToNull(val), '[]');
                if (Q.startsWith(json, '[') && Q.endsWith(json, ']')) {
                    this.set_value($.parseJSON(json));
                }
                else {
                    this.set_value([{
                            Filename: json,
                            OriginalName: 'UnknownFile'
                        }]);
                }
            }
            else {
                this.set_value(val);
            }
        };
        __decorate([
            Serenity.Decorators.option()
        ], MultipleFileUploadEditor.prototype, "jsonEncodeValue", void 0);
        MultipleFileUploadEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.MultipleFileUploadEditor', [Serenity.IReadOnly]),
            Serenity.Decorators.element('<div/>')
        ], MultipleFileUploadEditor);
        return MultipleFileUploadEditor;
    }(Serenity.Widget));
    Serenity.MultipleFileUploadEditor = MultipleFileUploadEditor;
    var MultipleImageUploadEditor = /** @class */ (function (_super) {
        __extends(MultipleImageUploadEditor, _super);
        function MultipleImageUploadEditor(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            div.addClass('s-MultipleImageUploadEditor');
            return _this;
        }
        MultipleImageUploadEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.MultipleImageUploadEditor')
        ], MultipleImageUploadEditor);
        return MultipleImageUploadEditor;
    }(MultipleFileUploadEditor));
    Serenity.MultipleImageUploadEditor = MultipleImageUploadEditor;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-editors.js.map