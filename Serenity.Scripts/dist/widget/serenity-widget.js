var Serenity;
(function (Serenity) {
    var Decorators;
    (function (Decorators) {
        function distinct(arr) {
            return arr.filter(function (item, pos) { return arr.indexOf(item) === pos; });
        }
        function merge(arr1, arr2) {
            if (!arr1 || !arr2)
                return (arr1 || arr2 || []).slice();
            return distinct(arr1.concat(arr2));
        }
        function registerType(target, name, intf) {
            if (name != null) {
                target.__typeName = name;
                Q.types[name] = target;
            }
            else if (!target.__typeName)
                target.__register = true;
            else
                Q.types[target.__typeName] = target;
            if (intf)
                target.__interfaces = merge(target.__interfaces, intf);
        }
        function registerClass(nameOrIntf, intf2) {
            return function (target) {
                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);
                target.__class = true;
            };
        }
        Decorators.registerClass = registerClass;
        function registerInterface(nameOrIntf, intf2) {
            return function (target) {
                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);
                target.__interface = true;
                target.isAssignableFrom = function (type) {
                    return type.__interfaces != null && type.__interfaces.indexOf(this) >= 0;
                };
            };
        }
        Decorators.registerInterface = registerInterface;
        function registerEditor(nameOrIntf, intf2) {
            return registerClass(nameOrIntf, intf2);
        }
        Decorators.registerEditor = registerEditor;
        function addAttribute(type, attr) {
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }
        Decorators.addAttribute = addAttribute;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
    Serenity.Decorators.registerInterface('Serenity.ISlickFormatter')(Serenity.ISlickFormatter);
    var EnumKeyAttribute = /** @class */ (function () {
        function EnumKeyAttribute(value) {
            this.value = value;
        }
        EnumKeyAttribute = __decorate([
            Serenity.Decorators.registerClass('Serenity.EnumKeyAttribute')
        ], EnumKeyAttribute);
        return EnumKeyAttribute;
    }());
    Serenity.EnumKeyAttribute = EnumKeyAttribute;
    (function (Decorators) {
        function enumKey(value) {
            return function (target) {
                Decorators.addAttribute(target, new EnumKeyAttribute(value));
            };
        }
        Decorators.enumKey = enumKey;
        function registerEnum(target, enumKey, name) {
            if (!target.__enum) {
                Object.defineProperty(target, '__enum', {
                    get: function () {
                        return true;
                    }
                });
                target.prototype = target.prototype || {};
                for (var _i = 0, _a = Object.keys(target); _i < _a.length; _i++) {
                    var k = _a[_i];
                    if (isNaN(Q.parseInteger(k)) && target[k] != null && !isNaN(Q.parseInteger(target[k])))
                        target.prototype[k] = target[k];
                }
                if (name != null) {
                    target.__typeName = name;
                    Q.types[name] = target;
                }
                else if (!target.__typeName)
                    target.__register = true;
                if (enumKey)
                    Decorators.addAttribute(target, new EnumKeyAttribute(enumKey));
            }
        }
        Decorators.registerEnum = registerEnum;
        function registerEnumType(target, name, enumKey) {
            registerEnum(target, Q.coalesce(enumKey, name), name);
        }
        Decorators.registerEnumType = registerEnumType;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
    Serenity.Decorators.registerEnum(Serenity.SummaryType, 'Serenity.SummaryType');
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
(function (Serenity) {
    function Attr(name) {
        return Serenity.Decorators.registerClass('Serenity.' + name + 'Attribute');
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
            Serenity.Decorators.registerClass('Serenity.EditorTypeAttributeBase')
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
    var FlexifyAttribute = /** @class */ (function () {
        function FlexifyAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        FlexifyAttribute = __decorate([
            Attr('Flexify')
        ], FlexifyAttribute);
        return FlexifyAttribute;
    }());
    Serenity.FlexifyAttribute = FlexifyAttribute;
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
    var Decorators;
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
        function flexify(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                Decorators.addAttribute(target, new FlexifyAttribute(value));
            };
        }
        Decorators.flexify = flexify;
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
    var EditorTypeRegistry;
    (function (EditorTypeRegistry) {
        var knownTypes;
        function get(key) {
            var _a;
            if (Q.isEmptyOrNull(key)) {
                throw new Q.ArgumentNullException('key');
            }
            initialize();
            var editorType = knownTypes[key.toLowerCase()];
            if (editorType == null) {
                var type = (_a = Q.getType(key)) !== null && _a !== void 0 ? _a : Q.getType(key, globalObj);
                if (type != null) {
                    knownTypes[key.toLowerCase()] = type;
                    return type;
                }
                throw new Q.Exception(Q.format("Can't find {0} editor type!", key));
            }
            return editorType;
        }
        EditorTypeRegistry.get = get;
        function initialize() {
            if (knownTypes != null)
                return;
            knownTypes = {};
            for (var _i = 0, _a = Q.getTypes(); _i < _a.length; _i++) {
                var type = _a[_i];
                var fullName = Q.getTypeFullName(type).toLowerCase();
                knownTypes[fullName] = type;
                var editorAttr = Q.getAttributes(type, Serenity.EditorAttribute, false);
                if (editorAttr != null && editorAttr.length > 0) {
                    var attrKey = editorAttr[0].key;
                    if (!Q.isEmptyOrNull(attrKey)) {
                        knownTypes[attrKey.toLowerCase()] = type;
                    }
                }
                for (var _b = 0, _c = Q.Config.rootNamespaces; _b < _c.length; _b++) {
                    var k = _c[_b];
                    if (Q.startsWith(fullName, k.toLowerCase() + '.')) {
                        var kx = fullName.substr(k.length + 1).toLowerCase();
                        if (knownTypes[kx] == null) {
                            knownTypes[kx] = type;
                        }
                    }
                }
            }
            setTypeKeysWithoutEditorSuffix();
        }
        function reset() {
            knownTypes = null;
        }
        EditorTypeRegistry.reset = reset;
        function setTypeKeysWithoutEditorSuffix() {
            var suffix = 'editor';
            var keys = Object.keys(knownTypes);
            for (var _i = 0, keys_1 = keys; _i < keys_1.length; _i++) {
                var k = keys_1[_i];
                setWithoutSuffix(k, knownTypes[k]);
            }
        }
        function setWithoutSuffix(key, t) {
            var suffix = 'editor';
            if (!Q.endsWith(key, suffix))
                return;
            var p = key.substr(0, key.length - suffix.length);
            if (Q.isEmptyOrNull(p))
                return;
            if (knownTypes[p] != null)
                return;
            knownTypes[p] = knownTypes[key];
        }
    })(EditorTypeRegistry = Serenity.EditorTypeRegistry || (Serenity.EditorTypeRegistry = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EnumTypeRegistry;
    (function (EnumTypeRegistry) {
        var knownTypes;
        function tryGet(key) {
            if (knownTypes == null) {
                knownTypes = {};
                for (var _i = 0, _a = Q.getTypes(); _i < _a.length; _i++) {
                    var type = _a[_i];
                    if (Q.isEnum(type)) {
                        var fullName = Q.getTypeFullName(type);
                        knownTypes[fullName] = type;
                        var enumKeyAttr = Q.getAttributes(type, Serenity.EnumKeyAttribute, false);
                        if (enumKeyAttr != null && enumKeyAttr.length > 0) {
                            knownTypes[enumKeyAttr[0].value] = type;
                        }
                        for (var _b = 0, _c = Q.Config.rootNamespaces; _b < _c.length; _b++) {
                            var k = _c[_b];
                            if (Q.startsWith(fullName, k + '.')) {
                                knownTypes[fullName.substr(k.length + 1)] = type;
                            }
                        }
                    }
                }
            }
            if (knownTypes[key] == null)
                return null;
            return knownTypes[key];
        }
        EnumTypeRegistry.tryGet = tryGet;
        function get(key) {
            var type = EnumTypeRegistry.tryGet(key);
            if (type == null) {
                var message = Q.format("Can't find {0} enum type! If you have recently defined this enum type " +
                    "in server side code, make sure your project builds successfully and transform T4 templates. " +
                    "Also make sure that enum is under your project root namespace, and your namespace parts starts " +
                    "with capital letters, e.g.MyProject.Pascal.Cased namespace", key);
                Q.notifyError(message, '', null);
                throw new Q.Exception(message);
            }
            return type;
        }
        EnumTypeRegistry.get = get;
    })(EnumTypeRegistry = Serenity.EnumTypeRegistry || (Serenity.EnumTypeRegistry = {}));
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
            if (this.element) {
                this.element.removeClass('s-' + Q.getTypeName(Q.getInstanceType(this)));
                this.element.off('.' + this.widgetName).off('.' + this.uniqueName).removeData(this.widgetName);
                this.element = null;
            }
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
        Widget.prototype.getGridField = function () {
            return this.element.closest('.field');
        };
        Widget.prototype.change = function (handler) {
            this.element.on('change.' + this.uniqueName, handler);
        };
        ;
        Widget.prototype.changeSelect2 = function (handler) {
            this.element.on('change.' + this.uniqueName, function (e) {
                if (!$(e.target).hasClass('select2-change-triggered'))
                    handler(e);
            });
        };
        ;
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
        Widget.prototype.initialize = function () {
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
    if (typeof $ !== "undefined" && $.fn) {
        $.fn.tryGetWidget = function (type) {
            var element = this;
            var w;
            if (Q.isAssignableFrom(Serenity.Widget, type)) {
                var widgetName = Widget.getWidgetName(type);
                w = element.data(widgetName);
                if (w != null && !Q.isAssignableFrom(type, Q.getInstanceType(w))) {
                    w = null;
                }
                if (w != null) {
                    return w;
                }
            }
            var data = element.data();
            if (data == null) {
                return null;
            }
            for (var _i = 0, _a = Object.keys(data); _i < _a.length; _i++) {
                var key = _a[_i];
                w = data[key];
                if (w != null && Q.isAssignableFrom(type, Q.getInstanceType(w))) {
                    return w;
                }
            }
            return null;
        };
        $.fn.getWidget = function (type) {
            if (this == null) {
                throw new Q.ArgumentNullException('element');
            }
            if (this.length === 0) {
                throw new Q.Exception(Q.format("Searching for widget of type '{0}' on a non-existent element! ({1})", Q.getTypeFullName(type), this.selector));
            }
            var w = this.tryGetWidget(type);
            if (w == null) {
                var message = Q.format("Element has no widget of type '{0}'! If you have recently changed " +
                    "editor type of a property in a form class, or changed data type in row (which also changes " +
                    "editor type) your script side Form definition might be out of date. Make sure your project " +
                    "builds successfully and transform T4 templates", Q.getTypeFullName(type));
                Q.notifyError(message, '', null);
                throw new Q.Exception(message);
            }
            return w;
        };
    }
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedWidget = /** @class */ (function (_super) {
        __extends(TemplatedWidget, _super);
        function TemplatedWidget(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.idPrefix = _this.uniqueName + '_';
            var widgetMarkup = _this.getTemplate().replace(new RegExp('~_', 'g'), _this.idPrefix);
            // for compatibility with older templates based on JsRender
            var end = 0;
            while (true) {
                var idx = widgetMarkup.indexOf('{{text:"', end);
                if (idx < 0)
                    break;
                var end = widgetMarkup.indexOf('"}}', idx);
                if (end < 0)
                    break;
                var key = widgetMarkup.substr(idx + 8, end - idx - 8);
                var text = Q.text(key);
                widgetMarkup = widgetMarkup.substr(0, idx) + text + widgetMarkup.substr(end + 3);
                end = idx + text.length;
            }
            _this.element.html(widgetMarkup);
            return _this;
        }
        TemplatedWidget_1 = TemplatedWidget;
        TemplatedWidget.prototype.byId = function (id) {
            return $('#' + this.idPrefix + id);
        };
        TemplatedWidget.prototype.byID = function (id, type) {
            return this.byId(id).getWidget(type);
        };
        TemplatedWidget.noGeneric = function (s) {
            var dollar = s.indexOf('$');
            if (dollar >= 0) {
                return s.substr(0, dollar);
            }
            return s;
        };
        TemplatedWidget.prototype.getDefaultTemplateName = function () {
            return TemplatedWidget_1.noGeneric(Q.getTypeName(Q.getInstanceType(this)));
        };
        TemplatedWidget.prototype.getTemplateName = function () {
            var type = Q.getInstanceType(this);
            var fullName = Q.getTypeFullName(type);
            var templateNames = TemplatedWidget_1.templateNames;
            var cachedName = TemplatedWidget_1.templateNames[fullName];
            if (cachedName != null) {
                return cachedName;
            }
            while (type && type !== Serenity.Widget) {
                var name = TemplatedWidget_1.noGeneric(Q.getTypeFullName(type));
                for (var _i = 0, _a = Q.Config.rootNamespaces; _i < _a.length; _i++) {
                    var k = _a[_i];
                    if (Q.startsWith(name, k + '.')) {
                        name = name.substr(k.length + 1);
                        break;
                    }
                }
                if (Q.canLoadScriptData('Template.' + name)) {
                    templateNames[fullName] = name;
                    return name;
                }
                name = Q.replaceAll(name, '.', '_');
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    templateNames[fullName] = name;
                    return name;
                }
                name = TemplatedWidget_1.noGeneric(Q.getTypeName(type));
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    TemplatedWidget_1.templateNames[fullName] = name;
                    return name;
                }
                type = Q.getBaseType(type);
            }
            templateNames[fullName] = cachedName = this.getDefaultTemplateName();
            return cachedName;
        };
        TemplatedWidget.prototype.getFallbackTemplate = function () {
            return null;
        };
        TemplatedWidget.prototype.getTemplate = function () {
            var templateName = this.getTemplateName();
            var script = $('script#Template_' + templateName);
            if (script.length > 0) {
                return script.html();
            }
            var template;
            if (!Q.canLoadScriptData('Template.' + templateName) &&
                this.getDefaultTemplateName() == templateName) {
                template = this.getFallbackTemplate();
                if (template != null)
                    return template;
            }
            template = Q.getTemplate(templateName);
            if (template == null) {
                throw new Error(Q.format("Can't locate template for widget '{0}' with name '{1}'!", Q.getTypeName(Q.getInstanceType(this)), templateName));
            }
            return template;
        };
        var TemplatedWidget_1;
        TemplatedWidget.templateNames = {};
        TemplatedWidget = TemplatedWidget_1 = __decorate([
            Serenity.Decorators.registerClass()
        ], TemplatedWidget);
        return TemplatedWidget;
    }(Serenity.Widget));
    Serenity.TemplatedWidget = TemplatedWidget;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var CascadedWidgetLink = /** @class */ (function () {
        function CascadedWidgetLink(parentType, widget, parentChange) {
            var _this = this;
            this.parentType = parentType;
            this.widget = widget;
            this.parentChange = parentChange;
            this.bind();
            this.widget.element.bind('remove.' + widget.uniqueName + 'cwh', function (e) {
                _this.unbind();
                _this.widget = null;
                _this.parentChange = null;
            });
        }
        CascadedWidgetLink.prototype.bind = function () {
            var _this = this;
            if (Q.isEmptyOrNull(this._parentID)) {
                return null;
            }
            var parent = Q.findElementWithRelativeId(this.widget.element, this._parentID)
                .tryGetWidget(this.parentType);
            if (parent != null) {
                parent.element.bind('change.' + this.widget.uniqueName, function () {
                    _this.parentChange(parent);
                });
                return parent;
            }
            else {
                Q.notifyError("Can't find cascaded parent element with ID: " + this._parentID + '!', '', null);
                return null;
            }
        };
        CascadedWidgetLink.prototype.unbind = function () {
            if (Q.isEmptyOrNull(this._parentID)) {
                return null;
            }
            var parent = Q.findElementWithRelativeId(this.widget.element, this._parentID).tryGetWidget(this.parentType);
            if (parent != null) {
                parent.element.unbind('.' + this.widget.uniqueName);
            }
            return parent;
        };
        CascadedWidgetLink.prototype.get_parentID = function () {
            return this._parentID;
        };
        CascadedWidgetLink.prototype.set_parentID = function (value) {
            if (this._parentID !== value) {
                this.unbind();
                this._parentID = value;
                this.bind();
            }
        };
        CascadedWidgetLink = __decorate([
            Serenity.Decorators.registerClass('Serenity.CascadedWidgetLink')
        ], CascadedWidgetLink);
        return CascadedWidgetLink;
    }());
    Serenity.CascadedWidgetLink = CascadedWidgetLink;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var UploadHelper;
    (function (UploadHelper) {
        function addUploadInput(options) {
            options.container.addClass('fileinput-button');
            var uploadInput = $('<input/>').attr('type', 'file')
                .attr('name', options.inputName + '[]')
                .attr('data-url', Q.resolveUrl('~/File/TemporaryUpload'))
                .attr('multiple', 'multiple').appendTo(options.container);
            if (options.allowMultiple) {
                uploadInput.attr('multiple', 'multiple');
            }
            uploadInput.fileupload({
                dataType: 'json',
                dropZone: options.zone,
                pasteZone: options.zone,
                done: function (e, data) {
                    var response = data.result;
                    if (options.fileDone != null) {
                        options.fileDone(response, data.files[0].name, data);
                    }
                },
                start: function () {
                    Q.blockUI(null);
                    if (options.progress != null) {
                        options.progress.show();
                    }
                },
                stop: function () {
                    Q.blockUndo();
                    if (options.progress != null) {
                        options.progress.hide();
                    }
                },
                progress: function (e1, data1) {
                    if (options.progress != null) {
                        var percent = data1.loaded / data1.total * 100;
                        options.progress.children().css('width', percent.toString() + '%');
                    }
                }
            });
            return uploadInput;
        }
        UploadHelper.addUploadInput = addUploadInput;
        function checkImageConstraints(file, opt) {
            if (!file.IsImage && !opt.allowNonImage) {
                Q.alert(Q.text('Controls.ImageUpload.NotAnImageFile'));
                return false;
            }
            if (opt.minSize > 0 && file.Size < opt.minSize) {
                Q.alert(Q.format(Q.text('Controls.ImageUpload.UploadFileTooSmall'), UploadHelper.fileSizeDisplay(opt.minSize)));
                return false;
            }
            if (opt.maxSize > 0 && file.Size > opt.maxSize) {
                Q.alert(Q.format(Q.text('Controls.ImageUpload.UploadFileTooBig'), UploadHelper.fileSizeDisplay(opt.maxSize)));
                return false;
            }
            if (!file.IsImage) {
                return true;
            }
            if (opt.minWidth > 0 && file.Width < opt.minWidth) {
                Q.alert(Q.format(Q.text('Controls.ImageUpload.MinWidth'), opt.minWidth));
                return false;
            }
            if (opt.maxWidth > 0 && file.Width > opt.maxWidth) {
                Q.alert(Q.format(Q.text('Controls.ImageUpload.MaxWidth'), opt.maxWidth));
                return false;
            }
            if (opt.minHeight > 0 && file.Height < opt.minHeight) {
                Q.alert(Q.format(Q.text('Controls.ImageUpload.MinHeight'), opt.minHeight));
                return false;
            }
            if (opt.maxHeight > 0 && file.Height > opt.maxHeight) {
                Q.alert(Q.format(Q.text('Controls.ImageUpload.MaxHeight'), opt.maxHeight));
                return false;
            }
            return true;
        }
        UploadHelper.checkImageConstraints = checkImageConstraints;
        function fileNameSizeDisplay(name, bytes) {
            return name + ' (' + fileSizeDisplay(bytes) + ')';
        }
        UploadHelper.fileNameSizeDisplay = fileNameSizeDisplay;
        function fileSizeDisplay(bytes) {
            var byteSize = Q.round(bytes * 100 / 1024) * 0.01;
            var suffix = 'KB';
            if (byteSize >= 1024) {
                byteSize = Q.round(byteSize * 100 / 1024) * 0.01;
                suffix = 'MB';
            }
            var sizeParts = byteSize.toString().split(String.fromCharCode(46));
            var value;
            if (sizeParts.length > 1) {
                value = sizeParts[0] + '.' + sizeParts[1].substr(0, 2);
            }
            else {
                value = sizeParts[0];
            }
            return value + ' ' + suffix;
        }
        UploadHelper.fileSizeDisplay = fileSizeDisplay;
        function hasImageExtension(filename) {
            if (Q.isEmptyOrNull(filename)) {
                return false;
            }
            filename = filename.toLowerCase();
            return Q.endsWith(filename, '.jpg') || Q.endsWith(filename, '.jpeg') ||
                Q.endsWith(filename, '.gif') || Q.endsWith(filename, '.png');
        }
        UploadHelper.hasImageExtension = hasImageExtension;
        function thumbFileName(filename) {
            filename = Q.coalesce(filename, '');
            var idx = filename.lastIndexOf('.');
            if (idx >= 0) {
                filename = filename.substr(0, idx);
            }
            return filename + '_t.jpg';
        }
        UploadHelper.thumbFileName = thumbFileName;
        function dbFileUrl(filename) {
            filename = Q.replaceAll(Q.coalesce(filename, ''), '\\', '/');
            return Q.resolveUrl('~/upload/') + filename;
        }
        UploadHelper.dbFileUrl = dbFileUrl;
        function colorBox(link, options) {
            link.colorbox({
                current: Q.text('Controls.ImageUpload.ColorboxCurrent'),
                previous: Q.text('Controls.ImageUpload.ColorboxPrior'),
                next: Q.text('Controls.ImageUpload.ColorboxNext'),
                close: Q.text('Controls.ImageUpload.ColorboxClose')
            });
        }
        UploadHelper.colorBox = colorBox;
        function populateFileSymbols(container, items, displayOriginalName, urlPrefix) {
            items = items || [];
            container.html('');
            for (var index = 0; index < items.length; index++) {
                var item = items[index];
                var li = $('<li/>').addClass('file-item').data('index', index);
                var isImage = hasImageExtension(item.Filename);
                if (isImage) {
                    li.addClass('file-image');
                }
                else {
                    li.addClass('file-binary');
                }
                var editLink = '#' + index;
                var thumb = $('<a/>').addClass('thumb').appendTo(li);
                var originalName = Q.coalesce(item.OriginalName, '');
                var fileName = item.Filename;
                if (urlPrefix != null && fileName != null &&
                    !Q.startsWith(fileName, 'temporary/')) {
                    fileName = urlPrefix + fileName;
                }
                thumb.attr('href', dbFileUrl(fileName));
                thumb.attr('target', '_blank');
                if (!Q.isEmptyOrNull(originalName)) {
                    thumb.attr('title', originalName);
                }
                if (isImage) {
                    thumb.css('backgroundImage', "url('" + dbFileUrl(thumbFileName(item.Filename)) + "')");
                    colorBox(thumb, new Object());
                }
                if (displayOriginalName) {
                    $('<div/>').addClass('filename').text(originalName)
                        .attr('title', originalName).appendTo(li);
                }
                li.appendTo(container);
            }
        }
        UploadHelper.populateFileSymbols = populateFileSymbols;
    })(UploadHelper = Serenity.UploadHelper || (Serenity.UploadHelper = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var PopupMenuButton = /** @class */ (function (_super) {
        __extends(PopupMenuButton, _super);
        function PopupMenuButton(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            div.addClass('s-PopupMenuButton');
            div.click(function (e) {
                e.preventDefault();
                e.stopPropagation();
                if (_this.options.onPopup != null) {
                    _this.options.onPopup();
                }
                var menu = _this.options.menu;
                menu.show().position({
                    my: Q.coalesce(_this.options.positionMy, 'left top'),
                    at: Q.coalesce(_this.options.positionAt, 'left bottom'),
                    of: _this.element
                });
                var uq = _this.uniqueName;
                $(document).one('click.' + uq, function (x) {
                    menu.hide();
                });
            });
            _this.options.menu.hide().appendTo(document.body)
                .addClass('s-PopupMenu').menu();
            return _this;
        }
        PopupMenuButton.prototype.destroy = function () {
            if (this.options.menu != null) {
                this.options.menu.remove();
                this.options.menu = null;
            }
            _super.prototype.destroy.call(this);
        };
        PopupMenuButton = __decorate([
            Serenity.Decorators.registerEditor('Serenity.PopupMenuButton')
        ], PopupMenuButton);
        return PopupMenuButton;
    }(Serenity.Widget));
    Serenity.PopupMenuButton = PopupMenuButton;
    var PopupToolButton = /** @class */ (function (_super) {
        __extends(PopupToolButton, _super);
        function PopupToolButton(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            div.addClass('s-PopupToolButton');
            $('<b/>').appendTo(div.children('.button-outer').children('span'));
            return _this;
        }
        PopupToolButton = __decorate([
            Serenity.Decorators.registerEditor('Serenity.PopupToolButton')
        ], PopupToolButton);
        return PopupToolButton;
    }(PopupMenuButton));
    Serenity.PopupToolButton = PopupToolButton;
    var Toolbar = /** @class */ (function (_super) {
        __extends(Toolbar, _super);
        function Toolbar(div, options) {
            var _this = _super.call(this, div, options) || this;
            _this.element.addClass('s-Toolbar clearfix')
                .html('<div class="tool-buttons"><div class="buttons-outer">' +
                '<div class="buttons-inner"></div></div></div>');
            var container = $('div.buttons-inner', _this.element);
            var buttons = _this.options.buttons;
            for (var i = 0; i < buttons.length; i++) {
                _this.createButton(container, buttons[i]);
            }
            return _this;
        }
        Toolbar.prototype.destroy = function () {
            this.element.find('div.tool-button').unbind('click');
            if (this.mouseTrap) {
                if (!!this.mouseTrap.destroy) {
                    this.mouseTrap.destroy();
                }
                else {
                    this.mouseTrap.reset();
                }
                this.mouseTrap = null;
            }
            _super.prototype.destroy.call(this);
        };
        Toolbar.prototype.createButton = function (container, b) {
            var cssClass = Q.coalesce(b.cssClass, '');
            if (b.separator === true || b.separator === 'left' || b.separator === 'both') {
                $('<div class="separator"></div>').appendTo(container);
            }
            var btn = $('<div class="tool-button"><div class="button-outer">' +
                '<span class="button-inner"></span></div></div>')
                .appendTo(container);
            if (b.separator === 'right' || b.separator === 'both') {
                $('<div class="separator"></div>').appendTo(container);
            }
            if (cssClass.length > 0) {
                btn.addClass(cssClass);
            }
            if (!Q.isEmptyOrNull(b.hint)) {
                btn.attr('title', b.hint);
            }
            btn.click(function (e) {
                if (btn.hasClass('disabled')) {
                    return;
                }
                b.onClick(e);
            });
            var text = b.title;
            if (b.htmlEncode !== false) {
                text = Q.htmlEncode(b.title);
            }
            if (!Q.isEmptyOrNull(b.icon)) {
                btn.addClass('icon-tool-button');
                var klass = b.icon;
                if (Q.startsWith(klass, 'fa-')) {
                    klass = 'fa ' + klass;
                }
                else if (Q.startsWith(klass, 'glyphicon-')) {
                    klass = 'glyphicon ' + klass;
                }
                text = "<i class='" + klass + "'></i> " + text;
            }
            if (text == null || text.length === 0) {
                btn.addClass('no-text');
            }
            else {
                btn.find('span').html(text);
            }
            if (b.visible === false)
                btn.hide();
            if (b.disabled != null && typeof b.disabled !== "function")
                btn.toggleClass('disabled', !!b.disabled);
            if (typeof b.visible === "function" || typeof b.disabled == "function") {
                btn.on('updateInterface', function () {
                    if (typeof b.visible === "function")
                        btn.toggle(!!b.visible());
                    if (typeof b.disabled === "function")
                        btn.toggleClass("disabled", !!b.disabled());
                });
            }
            if (!!(!Q.isEmptyOrNull(b.hotkey) && window['Mousetrap'] != null)) {
                this.mouseTrap = this.mouseTrap || window['Mousetrap'](b.hotkeyContext || this.options.hotkeyContext || window.document.documentElement);
                this.mouseTrap.bind(b.hotkey, function (e1, action) {
                    if (btn.is(':visible')) {
                        btn.triggerHandler('click');
                    }
                    return b.hotkeyAllowDefault;
                });
            }
        };
        Toolbar.prototype.findButton = function (className) {
            if (className != null && Q.startsWith(className, '.')) {
                className = className.substr(1);
            }
            return $('div.tool-button.' + className, this.element);
        };
        Toolbar.prototype.updateInterface = function () {
            this.element.find('.tool-button').each(function (i, el) {
                $(el).triggerHandler('updateInterface');
            });
        };
        Toolbar = __decorate([
            Serenity.Decorators.registerClass('Serenity.Toolbar')
        ], Toolbar);
        return Toolbar;
    }(Serenity.Widget));
    Serenity.Toolbar = Toolbar;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EditorUtils;
    (function (EditorUtils) {
        function getDisplayText(editor) {
            var select2 = editor.element.data('select2');
            if (select2 != null) {
                var data = editor.element.select2('data');
                if (data == null)
                    return '';
                return Q.coalesce(data.text, '');
            }
            var value = getValue(editor);
            if (value == null) {
                return '';
            }
            if (typeof value === "string")
                return value;
            if (value instanceof Boolean)
                return (!!value ? Q.coalesce(Q.tryGetText('Controls.FilterPanel.OperatorNames.true'), 'True') :
                    Q.coalesce(Q.tryGetText('Controls.FilterPanel.OperatorNames.true'), 'False'));
            return value.toString();
        }
        EditorUtils.getDisplayText = getDisplayText;
        var dummy = { name: '_' };
        function getValue(editor) {
            var target = {};
            saveValue(editor, dummy, target);
            return target['_'];
        }
        EditorUtils.getValue = getValue;
        function saveValue(editor, item, target) {
            var getEditValue = Q.safeCast(editor, Serenity.IGetEditValue);
            if (getEditValue != null) {
                getEditValue.getEditValue(item, target);
                return;
            }
            var stringValue = Q.safeCast(editor, Serenity.IStringValue);
            if (stringValue != null) {
                target[item.name] = stringValue.get_value();
                return;
            }
            var booleanValue = Q.safeCast(editor, Serenity.IBooleanValue);
            if (booleanValue != null) {
                target[item.name] = booleanValue.get_value();
                return;
            }
            var doubleValue = Q.safeCast(editor, Serenity.IDoubleValue);
            if (doubleValue != null) {
                var value = doubleValue.get_value();
                target[item.name] = (isNaN(value) ? null : value);
                return;
            }
            if (editor.getEditValue != null) {
                editor.getEditValue(item, target);
                return;
            }
            if (editor.element.is(':input')) {
                target[item.name] = editor.element.val();
                return;
            }
        }
        EditorUtils.saveValue = saveValue;
        function setValue(editor, value) {
            var source = { _: value };
            loadValue(editor, dummy, source);
        }
        EditorUtils.setValue = setValue;
        function loadValue(editor, item, source) {
            var setEditValue = Q.safeCast(editor, Serenity.ISetEditValue);
            if (setEditValue != null) {
                setEditValue.setEditValue(source, item);
                return;
            }
            var stringValue = Q.safeCast(editor, Serenity.IStringValue);
            if (stringValue != null) {
                var value = source[item.name];
                if (value != null) {
                    value = value.toString();
                }
                stringValue.set_value(Q.cast(value, String));
                return;
            }
            var booleanValue = Q.safeCast(editor, Serenity.IBooleanValue);
            if (booleanValue != null) {
                var value1 = source[item.name];
                if (typeof (value1) === 'number') {
                    booleanValue.set_value(value1 > 0);
                }
                else {
                    booleanValue.set_value(!!value1);
                }
                return;
            }
            var doubleValue = Q.safeCast(editor, Serenity.IDoubleValue);
            if (doubleValue != null) {
                var d = source[item.name];
                if (!!(d == null || Q.isInstanceOfType(d, String) && Q.isTrimmedEmpty(Q.cast(d, String)))) {
                    doubleValue.set_value(null);
                }
                else if (Q.isInstanceOfType(d, String)) {
                    doubleValue.set_value(Q.cast(Q.parseDecimal(Q.cast(d, String)), Number));
                }
                else if (Q.isInstanceOfType(d, Boolean)) {
                    doubleValue.set_value((!!d ? 1 : 0));
                }
                else {
                    doubleValue.set_value(Q.cast(d, Number));
                }
                return;
            }
            if (editor.setEditValue != null) {
                editor.setEditValue(source, item);
                return;
            }
            if (editor.element.is(':input')) {
                var v = source[item.name];
                if (v == null) {
                    editor.element.val('');
                }
                else {
                    editor.element.val(v);
                }
                return;
            }
        }
        EditorUtils.loadValue = loadValue;
        function setReadonly(elements, isReadOnly) {
            elements.each(function (index, el) {
                var elx = $(el);
                var type = elx.attr('type');
                if (elx.is('select') || type === 'radio' || type === 'checkbox') {
                    if (isReadOnly) {
                        elx.addClass('readonly').attr('disabled', 'disabled');
                    }
                    else {
                        elx.removeClass('readonly').removeAttr('disabled');
                    }
                }
                else if (isReadOnly) {
                    elx.addClass('readonly').attr('readonly', 'readonly');
                }
                else {
                    elx.removeClass('readonly').removeAttr('readonly');
                }
                return true;
            });
            return elements;
        }
        EditorUtils.setReadonly = setReadonly;
        function setReadOnly(widget, isReadOnly) {
            var readOnly = Q.safeCast(widget, Serenity.IReadOnly);
            if (readOnly != null) {
                readOnly.set_readOnly(isReadOnly);
            }
            else if (widget.element.is(':input')) {
                setReadonly(widget.element, isReadOnly);
            }
        }
        EditorUtils.setReadOnly = setReadOnly;
        function setRequired(widget, isRequired) {
            var req = Q.safeCast(widget, Serenity.IValidateRequired);
            if (req != null) {
                req.set_required(isRequired);
            }
            else if (widget.element.is(':input')) {
                widget.element.toggleClass('required', !!isRequired);
            }
            var gridField = widget.element.closest('.field');
            var hasSupItem = gridField.find('sup').get().length > 0;
            if (isRequired && !hasSupItem) {
                $('<sup>*</sup>').attr('title', Q.text('Controls.PropertyGrid.RequiredHint'))
                    .prependTo(gridField.find('.caption')[0]);
            }
            else if (!isRequired && hasSupItem) {
                $(gridField.find('sup')[0]).remove();
            }
        }
        EditorUtils.setRequired = setRequired;
        function setContainerReadOnly(container, readOnly) {
            if (!readOnly) {
                if (!container.hasClass('readonly-container'))
                    return;
                container.removeClass('readonly-container').find(".editor.container-readonly")
                    .removeClass('container-readonly').each(function (i, e) {
                    var w = $(e).tryGetWidget(Serenity.Widget);
                    if (w != null)
                        Serenity.EditorUtils.setReadOnly(w, false);
                    else
                        Serenity.EditorUtils.setReadonly($(e), false);
                });
                return;
            }
            container.addClass('readonly-container').find(".editor")
                .not('.container-readonly')
                .each(function (i, e) {
                var w = $(e).tryGetWidget(Serenity.Widget);
                if (w != null) {
                    if (w['get_readOnly']) {
                        if (w['get_readOnly']())
                            return;
                    }
                    else if ($(e).is('[readonly]') || $(e).is('[disabled]') || $(e).is('.readonly') || $(e).is('.disabled'))
                        return;
                    $(e).addClass('container-readonly');
                    Serenity.EditorUtils.setReadOnly(w, true);
                }
                else {
                    if ($(e).is('[readonly]') || $(e).is('[disabled]') || $(e).is('.readonly') || $(e).is('.disabled'))
                        return;
                    Serenity.EditorUtils.setReadonly($(e).addClass('container-readonly'), true);
                }
            });
        }
        EditorUtils.setContainerReadOnly = setContainerReadOnly;
    })(EditorUtils = Serenity.EditorUtils || (Serenity.EditorUtils = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var LazyLoadHelper;
    (function (LazyLoadHelper) {
        var autoIncrement = 0;
        function executeOnceWhenShown(element, callback) {
            var el = element && element[0];
            if (!el)
                return;
            if (el.offsetWidth > 0 && el.offsetHeight > 0) {
                callback();
                return;
            }
            var timer = Q.LayoutTimer.onShown(function () { return el; }, function () {
                Q.LayoutTimer.off(timer);
                callback();
            });
        }
        LazyLoadHelper.executeOnceWhenShown = executeOnceWhenShown;
        function executeEverytimeWhenShown(element, callback, callNowIfVisible) {
            var el = element && element[0];
            if (!el)
                return;
            if (callNowIfVisible && el.offsetWidth > 0 && el.offsetHeight > 0) {
                callback();
            }
            Q.LayoutTimer.onShown(function () { return el; }, function () {
                callback();
            });
        }
        LazyLoadHelper.executeEverytimeWhenShown = executeEverytimeWhenShown;
    })(LazyLoadHelper = Serenity.LazyLoadHelper || (Serenity.LazyLoadHelper = {}));
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
var Serenity;
(function (Serenity) {
    var PrefixedContext = /** @class */ (function () {
        function PrefixedContext(idPrefix) {
            this.idPrefix = idPrefix;
        }
        PrefixedContext.prototype.byId = function (id) {
            return $('#' + this.idPrefix + id);
        };
        PrefixedContext.prototype.w = function (id, type) {
            return $('#' + this.idPrefix + id).getWidget(type);
        };
        return PrefixedContext;
    }());
    Serenity.PrefixedContext = PrefixedContext;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    function applyJQueryUIFixes() {
        if (typeof $ == "undefined" || !$.ui || !$.ui.dialog || !$.ui.dialog.prototype)
            return false;
        $.ui.dialog.prototype._allowInteraction = function (event) {
            if ($(event.target).closest(".ui-dialog").length) {
                return true;
            }
            return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
        };
        (function (orig) {
            $.ui.dialog.prototype._focusTabbable = function () {
                if ($(document.body).hasClass('mobile-device')) {
                    this.uiDialog && this.uiDialog.focus();
                    return;
                }
                orig.call(this);
            };
        })($.ui.dialog.prototype._focusTabbable);
        (function (orig) {
            $.ui.dialog.prototype._createTitlebar = function () {
                orig.call(this);
                this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html('<i class="fa fa-times" />');
            };
        })($.ui.dialog.prototype._createTitlebar);
    }
    !applyJQueryUIFixes() && typeof $ !== "undefined" && $(applyJQueryUIFixes);
    if (typeof $ !== "undefined") {
        $.cleanData = (function (orig) {
            return function (elems) {
                var events, elem, i, e;
                var cloned = elems;
                for (i = 0; (elem = cloned[i]) != null; i++) {
                    try {
                        events = $._data(elem, "events");
                        if (events && events.remove) {
                            // html collection might change during remove event, so clone it!
                            if (cloned === elems)
                                cloned = Array.prototype.slice.call(elems);
                            $(elem).triggerHandler("remove");
                            delete events.remove;
                        }
                    }
                    catch (e) { }
                }
                orig(elems);
            };
        })($.cleanData);
    }
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-widget.js.map