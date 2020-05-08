var Serenity;
(function (Serenity) {
    var IDialog = /** @class */ (function () {
        function IDialog() {
        }
        IDialog = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IDialog')
        ], IDialog);
        return IDialog;
    }());
    Serenity.IDialog = IDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IEditDialog = /** @class */ (function () {
        function IEditDialog() {
        }
        IEditDialog = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IEditDialog')
        ], IEditDialog);
        return IEditDialog;
    }());
    Serenity.IEditDialog = IEditDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IBooleanValue = /** @class */ (function () {
        function IBooleanValue() {
        }
        IBooleanValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IBooleanValue);
        return IBooleanValue;
    }());
    Serenity.IBooleanValue = IBooleanValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IDoubleValue = /** @class */ (function () {
        function IDoubleValue() {
        }
        IDoubleValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IDoubleValue);
        return IDoubleValue;
    }());
    Serenity.IDoubleValue = IDoubleValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IStringValue = /** @class */ (function () {
        function IStringValue() {
        }
        IStringValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IStringValue);
        return IStringValue;
    }());
    Serenity.IStringValue = IStringValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IGetEditValue = /** @class */ (function () {
        function IGetEditValue() {
        }
        IGetEditValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IGetEditValue);
        return IGetEditValue;
    }());
    Serenity.IGetEditValue = IGetEditValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ISetEditValue = /** @class */ (function () {
        function ISetEditValue() {
        }
        ISetEditValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], ISetEditValue);
        return ISetEditValue;
    }());
    Serenity.ISetEditValue = ISetEditValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IReadOnly = /** @class */ (function () {
        function IReadOnly() {
        }
        IReadOnly = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IReadOnly')
        ], IReadOnly);
        return IReadOnly;
    }());
    Serenity.IReadOnly = IReadOnly;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IValidateRequired = /** @class */ (function () {
        function IValidateRequired() {
        }
        IValidateRequired = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IValidateRequired')
        ], IValidateRequired);
        return IValidateRequired;
    }());
    Serenity.IValidateRequired = IValidateRequired;
})(Serenity || (Serenity = {}));
var System;
(function (System) {
    var ComponentModel;
    (function (ComponentModel) {
        var DisplayNameAttribute = /** @class */ (function () {
            function DisplayNameAttribute(displayName) {
                this.displayName = displayName;
            }
            DisplayNameAttribute = __decorate([
                Serenity.Decorators.registerClass('System.DisplayNameAttribute')
            ], DisplayNameAttribute);
            return DisplayNameAttribute;
        }());
        ComponentModel.DisplayNameAttribute = DisplayNameAttribute;
    })(ComponentModel = System.ComponentModel || (System.ComponentModel = {}));
})(System || (System = {}));
var Serenity;
(function (Serenity) {
    var Decorators;
    (function (Decorators) {
        function registerEditor(nameOrIntf, intf2) {
            return Decorators.registerClass(nameOrIntf, intf2);
        }
        Decorators.registerEditor = registerEditor;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
    function Attr(name) {
        return Decorators.registerClass('Serenity.' + name + 'Attribute');
    }
    var CategoryAttribute = /** @class */ (function () {
        function CategoryAttribute(category) {
            this.category = category;
        }
        CategoryAttribute = __decorate([
            Attr('Category')
        ], CategoryAttribute);
        return CategoryAttribute;
    }());
    Serenity.CategoryAttribute = CategoryAttribute;
    var ColumnsKeyAttribute = /** @class */ (function () {
        function ColumnsKeyAttribute(value) {
            this.value = value;
        }
        ColumnsKeyAttribute = __decorate([
            Attr('ColumnsKey')
        ], ColumnsKeyAttribute);
        return ColumnsKeyAttribute;
    }());
    Serenity.ColumnsKeyAttribute = ColumnsKeyAttribute;
    var CssClassAttribute = /** @class */ (function () {
        function CssClassAttribute(cssClass) {
            this.cssClass = cssClass;
        }
        CssClassAttribute = __decorate([
            Attr('CssClass')
        ], CssClassAttribute);
        return CssClassAttribute;
    }());
    Serenity.CssClassAttribute = CssClassAttribute;
    var DefaultValueAttribute = /** @class */ (function () {
        function DefaultValueAttribute(value) {
            this.value = value;
        }
        DefaultValueAttribute = __decorate([
            Attr('DefaultValue')
        ], DefaultValueAttribute);
        return DefaultValueAttribute;
    }());
    Serenity.DefaultValueAttribute = DefaultValueAttribute;
    var DialogTypeAttribute = /** @class */ (function () {
        function DialogTypeAttribute(value) {
            this.value = value;
        }
        DialogTypeAttribute = __decorate([
            Attr('DialogType')
        ], DialogTypeAttribute);
        return DialogTypeAttribute;
    }());
    Serenity.DialogTypeAttribute = DialogTypeAttribute;
    var EditorAttribute = /** @class */ (function () {
        function EditorAttribute() {
        }
        EditorAttribute = __decorate([
            Attr('Editor')
        ], EditorAttribute);
        return EditorAttribute;
    }());
    Serenity.EditorAttribute = EditorAttribute;
    var EditorOptionAttribute = /** @class */ (function () {
        function EditorOptionAttribute(key, value) {
            this.key = key;
            this.value = value;
        }
        EditorOptionAttribute = __decorate([
            Attr('EditorOption')
        ], EditorOptionAttribute);
        return EditorOptionAttribute;
    }());
    Serenity.EditorOptionAttribute = EditorOptionAttribute;
    var EditorTypeAttributeBase = /** @class */ (function () {
        function EditorTypeAttributeBase(editorType) {
            this.editorType = editorType;
        }
        EditorTypeAttributeBase.prototype.setParams = function (editorParams) {
        };
        EditorTypeAttributeBase = __decorate([
            Decorators.registerClass('Serenity.EditorTypeAttributeBase')
        ], EditorTypeAttributeBase);
        return EditorTypeAttributeBase;
    }());
    Serenity.EditorTypeAttributeBase = EditorTypeAttributeBase;
    var EditorTypeAttribute = /** @class */ (function (_super) {
        __extends(EditorTypeAttribute, _super);
        function EditorTypeAttribute(editorType) {
            return _super.call(this, editorType) || this;
        }
        EditorTypeAttribute = __decorate([
            Attr('EditorType')
        ], EditorTypeAttribute);
        return EditorTypeAttribute;
    }(EditorTypeAttributeBase));
    Serenity.EditorTypeAttribute = EditorTypeAttribute;
    var ElementAttribute = /** @class */ (function () {
        function ElementAttribute(value) {
            this.value = value;
        }
        ElementAttribute = __decorate([
            Attr('Element')
        ], ElementAttribute);
        return ElementAttribute;
    }());
    Serenity.ElementAttribute = ElementAttribute;
    var EntityTypeAttribute = /** @class */ (function () {
        function EntityTypeAttribute(value) {
            this.value = value;
        }
        EntityTypeAttribute = __decorate([
            Attr('EntityType')
        ], EntityTypeAttribute);
        return EntityTypeAttribute;
    }());
    Serenity.EntityTypeAttribute = EntityTypeAttribute;
    var FilterableAttribute = /** @class */ (function () {
        function FilterableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        FilterableAttribute = __decorate([
            Attr('Filterable')
        ], FilterableAttribute);
        return FilterableAttribute;
    }());
    Serenity.FilterableAttribute = FilterableAttribute;
    var FormKeyAttribute = /** @class */ (function () {
        function FormKeyAttribute(value) {
            this.value = value;
        }
        FormKeyAttribute = __decorate([
            Attr('FormKey')
        ], FormKeyAttribute);
        return FormKeyAttribute;
    }());
    Serenity.FormKeyAttribute = FormKeyAttribute;
    var GeneratedCodeAttribute = /** @class */ (function () {
        function GeneratedCodeAttribute(origin) {
            this.origin = origin;
        }
        GeneratedCodeAttribute = __decorate([
            Attr('GeneratedCode')
        ], GeneratedCodeAttribute);
        return GeneratedCodeAttribute;
    }());
    Serenity.GeneratedCodeAttribute = GeneratedCodeAttribute;
    var HiddenAttribute = /** @class */ (function () {
        function HiddenAttribute() {
        }
        HiddenAttribute = __decorate([
            Attr('Hidden')
        ], HiddenAttribute);
        return HiddenAttribute;
    }());
    Serenity.HiddenAttribute = HiddenAttribute;
    var HintAttribute = /** @class */ (function () {
        function HintAttribute(hint) {
            this.hint = hint;
        }
        HintAttribute = __decorate([
            Attr('Hint')
        ], HintAttribute);
        return HintAttribute;
    }());
    Serenity.HintAttribute = HintAttribute;
    var IdPropertyAttribute = /** @class */ (function () {
        function IdPropertyAttribute(value) {
            this.value = value;
        }
        IdPropertyAttribute = __decorate([
            Attr('IdProperty')
        ], IdPropertyAttribute);
        return IdPropertyAttribute;
    }());
    Serenity.IdPropertyAttribute = IdPropertyAttribute;
    var InsertableAttribute = /** @class */ (function () {
        function InsertableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        InsertableAttribute = __decorate([
            Attr('Insertable')
        ], InsertableAttribute);
        return InsertableAttribute;
    }());
    Serenity.InsertableAttribute = InsertableAttribute;
    var IsActivePropertyAttribute = /** @class */ (function () {
        function IsActivePropertyAttribute(value) {
            this.value = value;
        }
        IsActivePropertyAttribute = __decorate([
            Attr('IsActiveProperty')
        ], IsActivePropertyAttribute);
        return IsActivePropertyAttribute;
    }());
    Serenity.IsActivePropertyAttribute = IsActivePropertyAttribute;
    var ItemNameAttribute = /** @class */ (function () {
        function ItemNameAttribute(value) {
            this.value = value;
        }
        ItemNameAttribute = __decorate([
            Attr('ItemName')
        ], ItemNameAttribute);
        return ItemNameAttribute;
    }());
    Serenity.ItemNameAttribute = ItemNameAttribute;
    var LocalTextPrefixAttribute = /** @class */ (function () {
        function LocalTextPrefixAttribute(value) {
            this.value = value;
        }
        LocalTextPrefixAttribute = __decorate([
            Attr('LocalTextPrefix')
        ], LocalTextPrefixAttribute);
        return LocalTextPrefixAttribute;
    }());
    Serenity.LocalTextPrefixAttribute = LocalTextPrefixAttribute;
    var MaximizableAttribute = /** @class */ (function () {
        function MaximizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        MaximizableAttribute = __decorate([
            Attr('Maximizable')
        ], MaximizableAttribute);
        return MaximizableAttribute;
    }());
    Serenity.MaximizableAttribute = MaximizableAttribute;
    var MaxLengthAttribute = /** @class */ (function () {
        function MaxLengthAttribute(maxLength) {
            this.maxLength = maxLength;
        }
        MaxLengthAttribute = __decorate([
            Attr('MaxLength')
        ], MaxLengthAttribute);
        return MaxLengthAttribute;
    }());
    Serenity.MaxLengthAttribute = MaxLengthAttribute;
    var NamePropertyAttribute = /** @class */ (function () {
        function NamePropertyAttribute(value) {
            this.value = value;
        }
        NamePropertyAttribute = __decorate([
            Attr('NameProperty')
        ], NamePropertyAttribute);
        return NamePropertyAttribute;
    }());
    Serenity.NamePropertyAttribute = NamePropertyAttribute;
    var OneWayAttribute = /** @class */ (function () {
        function OneWayAttribute() {
        }
        OneWayAttribute = __decorate([
            Attr('OneWay')
        ], OneWayAttribute);
        return OneWayAttribute;
    }());
    Serenity.OneWayAttribute = OneWayAttribute;
    var OptionAttribute = /** @class */ (function () {
        function OptionAttribute() {
        }
        OptionAttribute = __decorate([
            Attr('Option')
        ], OptionAttribute);
        return OptionAttribute;
    }());
    Serenity.OptionAttribute = OptionAttribute;
    var OptionsTypeAttribute = /** @class */ (function () {
        function OptionsTypeAttribute(value) {
            this.value = value;
        }
        OptionsTypeAttribute = __decorate([
            Attr('OptionsType')
        ], OptionsTypeAttribute);
        return OptionsTypeAttribute;
    }());
    Serenity.OptionsTypeAttribute = OptionsTypeAttribute;
    var PanelAttribute = /** @class */ (function () {
        function PanelAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        PanelAttribute = __decorate([
            Attr('Panel')
        ], PanelAttribute);
        return PanelAttribute;
    }());
    Serenity.PanelAttribute = PanelAttribute;
    var PlaceholderAttribute = /** @class */ (function () {
        function PlaceholderAttribute(value) {
            this.value = value;
        }
        PlaceholderAttribute = __decorate([
            Attr('Placeholder')
        ], PlaceholderAttribute);
        return PlaceholderAttribute;
    }());
    Serenity.PlaceholderAttribute = PlaceholderAttribute;
    var ReadOnlyAttribute = /** @class */ (function () {
        function ReadOnlyAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        ReadOnlyAttribute = __decorate([
            Attr('ReadOnly')
        ], ReadOnlyAttribute);
        return ReadOnlyAttribute;
    }());
    Serenity.ReadOnlyAttribute = ReadOnlyAttribute;
    var RequiredAttribute = /** @class */ (function () {
        function RequiredAttribute(isRequired) {
            if (isRequired === void 0) { isRequired = true; }
            this.isRequired = isRequired;
        }
        RequiredAttribute = __decorate([
            Attr('Required')
        ], RequiredAttribute);
        return RequiredAttribute;
    }());
    Serenity.RequiredAttribute = RequiredAttribute;
    var ResizableAttribute = /** @class */ (function () {
        function ResizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        ResizableAttribute = __decorate([
            Attr('Resizable')
        ], ResizableAttribute);
        return ResizableAttribute;
    }());
    Serenity.ResizableAttribute = ResizableAttribute;
    var ResponsiveAttribute = /** @class */ (function () {
        function ResponsiveAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        ResponsiveAttribute = __decorate([
            Attr('Responsive')
        ], ResponsiveAttribute);
        return ResponsiveAttribute;
    }());
    Serenity.ResponsiveAttribute = ResponsiveAttribute;
    var ServiceAttribute = /** @class */ (function () {
        function ServiceAttribute(value) {
            this.value = value;
        }
        ServiceAttribute = __decorate([
            Attr('Service')
        ], ServiceAttribute);
        return ServiceAttribute;
    }());
    Serenity.ServiceAttribute = ServiceAttribute;
    var UpdatableAttribute = /** @class */ (function () {
        function UpdatableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        UpdatableAttribute = __decorate([
            Attr('Updatable')
        ], UpdatableAttribute);
        return UpdatableAttribute;
    }());
    Serenity.UpdatableAttribute = UpdatableAttribute;
    (function (Decorators) {
        function option() {
            return function (target, propertyKey) {
                var isGetSet = Q.startsWith(propertyKey, 'get_') || Q.startsWith(propertyKey, 'set_');
                var memberName = isGetSet ? propertyKey.substr(4) : propertyKey;
                var type = target.constructor;
                type.__metadata = type.__metadata || {};
                type.__metadata.members = type.__metadata.members || [];
                var member = undefined;
                for (var _i = 0, _a = type.__metadata.members; _i < _a.length; _i++) {
                    var m = _a[_i];
                    if (m.name == memberName) {
                        member = m;
                        break;
                    }
                }
                if (!member) {
                    member = {
                        attr: [new Serenity.OptionAttribute()],
                        name: memberName
                    };
                    if (isGetSet) {
                        member.type = 16 /* property */;
                        member.getter = {
                            name: 'get_' + memberName
                        };
                        member.setter = {
                            name: 'set_' + memberName,
                        };
                    }
                    else {
                        member.type = 4 /* field */;
                    }
                    type.__metadata.members.push(member);
                }
                else {
                    member.attr = member.attr || [];
                    member.attr.push(new OptionAttribute());
                }
            };
        }
        Decorators.option = option;
        function registerFormatter(nameOrIntf, intf2) {
            if (nameOrIntf === void 0) { nameOrIntf = [Serenity.ISlickFormatter]; }
            if (intf2 === void 0) { intf2 = [Serenity.ISlickFormatter]; }
            return Decorators.registerClass(nameOrIntf, intf2);
        }
        Decorators.registerFormatter = registerFormatter;
        function dialogType(value) {
            return function (target) {
                Decorators.addAttribute(target, new DialogTypeAttribute(value));
            };
        }
        Decorators.dialogType = dialogType;
        function editor(key) {
            return function (target) {
                var attr = new EditorAttribute();
                if (key !== undefined)
                    attr.key = key;
                Decorators.addAttribute(target, attr);
            };
        }
        Decorators.editor = editor;
        function element(value) {
            return function (target) {
                Decorators.addAttribute(target, new ElementAttribute(value));
            };
        }
        Decorators.element = element;
        function filterable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                Decorators.addAttribute(target, new FilterableAttribute(value));
            };
        }
        Decorators.filterable = filterable;
        function itemName(value) {
            return function (target) {
                Decorators.addAttribute(target, new ItemNameAttribute(value));
            };
        }
        Decorators.itemName = itemName;
        function maximizable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                Decorators.addAttribute(target, new MaximizableAttribute(value));
            };
        }
        Decorators.maximizable = maximizable;
        function optionsType(value) {
            return function (target) {
                Decorators.addAttribute(target, new OptionsTypeAttribute(value));
            };
        }
        Decorators.optionsType = optionsType;
        function panel(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                Decorators.addAttribute(target, new PanelAttribute(value));
            };
        }
        Decorators.panel = panel;
        function resizable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                Decorators.addAttribute(target, new ResizableAttribute(value));
            };
        }
        Decorators.resizable = resizable;
        function responsive(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                Decorators.addAttribute(target, new ResponsiveAttribute(value));
            };
        }
        Decorators.responsive = responsive;
        function service(value) {
            return function (target) {
                Decorators.addAttribute(target, new ServiceAttribute(value));
            };
        }
        Decorators.service = service;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    if (typeof React === "undefined" && typeof window !== "undefined") {
        if (window['preact'] != null) {
            window['React'] = window['ReactDOM'] = window['preact'];
            React.Fragment = Q.coalesce(React.Fragment, "x-fragment");
        }
        else if (window['Nerv'] != null) {
            window['React'] = window['ReactDOM'] = window['Nerv'];
            React.Fragment = Q.coalesce(React.Fragment, "x-fragment");
        }
        else {
            window['React'] = {
                Component: function () { },
                Fragment: "x-fragment",
                createElement: function () { return { _reactNotLoaded: true }; }
            };
            window['ReactDOM'] = {
                render: function () { throw Error("To use React, it should be included before Serenity.CoreLib.js"); }
            };
        }
    }
    var Widget = /** @class */ (function (_super) {
        __extends(Widget, _super);
        function Widget(element, options) {
            var _this = _super.call(this, options) || this;
            _this.element = element;
            _this.options = options || {};
            _this.widgetName = Widget_1.getWidgetName(Q.getInstanceType(_this));
            _this.uniqueName = _this.widgetName + (Widget_1.nextWidgetNumber++).toString();
            if (element.data(_this.widgetName)) {
                throw new Q.Exception(Q.format("The element already has widget '{0}'!", _this.widgetName));
            }
            element.on('remove.' + _this.widgetName, function (e) {
                if (e.bubbles || e.cancelable) {
                    return;
                }
                _this.destroy();
            }).data(_this.widgetName, _this);
            _this.addCssClass();
            return _this;
        }
        Widget_1 = Widget;
        Widget.prototype.destroy = function () {
            this.element.removeClass('s-' + Q.getTypeName(Q.getInstanceType(this)));
            this.element.off('.' + this.widgetName).off('.' + this.uniqueName).removeData(this.widgetName);
            this.element = null;
        };
        Widget.prototype.addCssClass = function () {
            this.element.addClass(this.getCssClass());
        };
        Widget.prototype.getCssClass = function () {
            var type = Q.getInstanceType(this);
            var klass = 's-' + Q.getTypeName(type);
            var fullClass = Q.replaceAll(Q.getTypeFullName(type), '.', '-');
            for (var _i = 0, _a = Q.Config.rootNamespaces; _i < _a.length; _i++) {
                var k = _a[_i];
                if (Q.startsWith(fullClass, k + '-')) {
                    fullClass = fullClass.substr(k.length + 1);
                    break;
                }
            }
            fullClass = 's-' + fullClass;
            if (klass === fullClass) {
                return klass;
            }
            return klass + ' ' + fullClass;
        };
        Widget.getWidgetName = function (type) {
            return Q.replaceAll(Q.getTypeFullName(type), '.', '_');
        };
        Widget.elementFor = function (editorType) {
            var elementAttr = Q.getAttributes(editorType, Serenity.ElementAttribute, true);
            var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>');
            return $(elementHtml);
        };
        ;
        Widget.prototype.addValidationRule = function (eventClass, rule) {
            return Q.addValidationRule(this.element, eventClass, rule);
        };
        Widget.create = function (params) {
            var widget;
            if (Q.isAssignableFrom(Serenity.IDialog, params.type)) {
                widget = new params.type(params.options);
                if (params.container)
                    widget.element.appendTo(params.container);
                params.element && params.element(widget.element);
            }
            else {
                var e = Widget_1.elementFor(params.type);
                if (params.container)
                    e.appendTo(params.container);
                params.element && params.element(e);
                widget = new params.type(e, params.options);
            }
            widget.init(null);
            params.init && params.init(widget);
            return widget;
        };
        Widget.prototype.init = function (action) {
            action && action(this);
            return this;
        };
        var Widget_1;
        Widget.nextWidgetNumber = 0;
        Widget.__isWidgetType = true;
        Widget = Widget_1 = __decorate([
            Serenity.Decorators.registerClass()
        ], Widget);
        return Widget;
    }(React.Component));
    Serenity.Widget = Widget;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ReflectionUtils;
    (function (ReflectionUtils) {
        function getPropertyValue(o, property) {
            var d = o;
            var getter = d['get_' + property];
            if (!!!(typeof (getter) === 'undefined')) {
                return getter.apply(o);
            }
            var camelCase = makeCamelCase(property);
            getter = d['get_' + camelCase];
            if (!!!(typeof (getter) === 'undefined')) {
                return getter.apply(o);
            }
            return d[camelCase];
        }
        ReflectionUtils.getPropertyValue = getPropertyValue;
        function setPropertyValue(o, property, value) {
            var d = o;
            var setter = d['set_' + property];
            if (!!!(typeof (setter) === 'undefined')) {
                setter.apply(o, [value]);
                return;
            }
            var camelCase = makeCamelCase(property);
            setter = d['set_' + camelCase];
            if (!!!(typeof (setter) === 'undefined')) {
                setter.apply(o, [value]);
                return;
            }
            d[camelCase] = value;
        }
        ReflectionUtils.setPropertyValue = setPropertyValue;
        function makeCamelCase(s) {
            if (Q.isEmptyOrNull(s)) {
                return s;
            }
            if (s === 'ID') {
                return 'id';
            }
            var hasNonUppercase = false;
            var numUppercaseChars = 0;
            for (var index = 0; index < s.length; index++) {
                if (s.charCodeAt(index) >= 65 && s.charCodeAt(index) <= 90) {
                    numUppercaseChars++;
                }
                else {
                    hasNonUppercase = true;
                    break;
                }
            }
            if (!hasNonUppercase && s.length !== 1 || numUppercaseChars === 0) {
                return s;
            }
            else if (numUppercaseChars > 1) {
                return s.substr(0, numUppercaseChars - 1).toLowerCase() + s.substr(numUppercaseChars - 1);
            }
            else if (s.length === 1) {
                return s.toLowerCase();
            }
            else {
                return s.substr(0, 1).toLowerCase() + s.substr(1);
            }
        }
        ReflectionUtils.makeCamelCase = makeCamelCase;
    })(ReflectionUtils = Serenity.ReflectionUtils || (Serenity.ReflectionUtils = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var DialogTypeRegistry;
    (function (DialogTypeRegistry) {
        function search(typeName) {
            var dialogType = Q.getType(typeName);
            if (dialogType != null && Q.isAssignableFrom(Serenity.IDialog, dialogType)) {
                return dialogType;
            }
            for (var _i = 0, _a = Q.Config.rootNamespaces; _i < _a.length; _i++) {
                var ns = _a[_i];
                dialogType = Q.getType(ns + '.' + typeName);
                if (dialogType != null && Q.isAssignableFrom(Serenity.IDialog, dialogType)) {
                    return dialogType;
                }
            }
            return null;
        }
        var knownTypes = {};
        function tryGet(key) {
            if (knownTypes[key] == null) {
                var typeName = key;
                var dialogType = search(typeName);
                if (dialogType == null && !Q.endsWith(key, 'Dialog')) {
                    typeName = key + 'Dialog';
                    dialogType = search(typeName);
                }
                if (dialogType == null) {
                    return null;
                }
                knownTypes[key] = dialogType;
                return dialogType;
            }
            return knownTypes[key];
        }
        DialogTypeRegistry.tryGet = tryGet;
        function get(key) {
            var type = tryGet(key);
            if (type == null) {
                var message = key + ' dialog class is not found! Make sure there is a dialog class with this name, ' +
                    'it is under your project root namespace, and your namespace parts start with capital letters, ' +
                    'e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option ' +
                    'check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). ' +
                    "You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.";
                Q.notifyError(message, '', null);
                throw new Q.Exception(message);
            }
            return type;
        }
        DialogTypeRegistry.get = get;
    })(DialogTypeRegistry = Serenity.DialogTypeRegistry || (Serenity.DialogTypeRegistry = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var SubDialogHelper;
    (function (SubDialogHelper) {
        function bindToDataChange(dialog, owner, dataChange, useTimeout) {
            var widgetName = owner.widgetName;
            dialog.element.bind('ondatachange.' + widgetName, function (e, dci) {
                if (useTimeout) {
                    window.setTimeout(function () {
                        dataChange(e, dci);
                    }, 0);
                }
                else {
                    dataChange(e, dci);
                }
            }).bind('remove.' + widgetName, function () {
                dialog.element.unbind('ondatachange.' + widgetName);
            });
            return dialog;
        }
        SubDialogHelper.bindToDataChange = bindToDataChange;
        function triggerDataChange(dialog) {
            dialog.element.triggerHandler('ondatachange');
            return dialog;
        }
        SubDialogHelper.triggerDataChange = triggerDataChange;
        function triggerDataChanged(element) {
            element.triggerHandler('ondatachange');
            return element;
        }
        SubDialogHelper.triggerDataChanged = triggerDataChanged;
        function bubbleDataChange(dialog, owner, useTimeout) {
            return bindToDataChange(dialog, owner, function (e, dci) {
                owner.element.triggerHandler('ondatachange');
            }, useTimeout);
        }
        SubDialogHelper.bubbleDataChange = bubbleDataChange;
        function cascade(cascadedDialog, ofElement) {
            cascadedDialog.element.one('dialogopen', function (e) {
                cascadedDialog.element.dialog().dialog('option', 'position', cascadedDialogOffset(ofElement));
            });
            return cascadedDialog;
        }
        SubDialogHelper.cascade = cascade;
        function cascadedDialogOffset(element) {
            return { my: 'left top', at: 'left+20 top+20', of: element[0] };
        }
        SubDialogHelper.cascadedDialogOffset = cascadedDialogOffset;
    })(SubDialogHelper = Serenity.SubDialogHelper || (Serenity.SubDialogHelper = {}));
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-widget.js.map