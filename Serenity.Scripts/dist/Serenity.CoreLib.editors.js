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
//# sourceMappingURL=Serenity.CoreLib.editors.js.map