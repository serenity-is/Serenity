var Q;
(function (Q) {
    var oldShowLabel;
    function validateShowLabel(element, message) {
        oldShowLabel.call(this, element, message);
        this.errorsFor(element).each(function (i, e) {
            if ($(element).hasClass('error'))
                $(e).removeClass('checked');
            $(e).attr('title', $(e).text());
        });
    }
    ;
    function jQueryValidationInitialization() {
        var p = $.validator;
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
        p.stopRequest = function (element, valid) {
            var formSubmitted = this.formSubmitted;
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
    }
    ;
    function validatorAbortHandler(validator) {
        validator.settings.abortHandler = null;
        validator.settings.submitHandler = function () {
            return false;
        };
    }
    Q.validatorAbortHandler = validatorAbortHandler;
    ;
    function validateOptions(options) {
        var opt = Q.baseValidateOptions();
        delete opt.showErrors;
        return Q.extend(Q.extend(opt, {
            meta: 'v',
            errorPlacement: function (error, element) {
                var field = null;
                var vx = element.attr('data-vx-id');
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
                        var inner = $('div.vx', field[0]);
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
            invalidHandler: function (event, validator) {
                Q.notifyError(Q.text("Validation.InvalidFormMessage"));
                $(validator.errorList.map(function (x) { return x.element; }))
                    .closest('.category.collapsed')
                    .children('.category-title')
                    .each(function (i, x) {
                    $(x).click();
                    return true;
                });
                if (validator.errorList.length) {
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
                            var $el = $(el);
                            if ($el.hasClass('select2-offscreen') &&
                                el.id) {
                                $el = $('#s2id_' + el.id);
                            }
                            $.fn.tooltip && $el.tooltip({
                                title: validator.errorList[0].message,
                                trigger: 'manual'
                            }).tooltip('show');
                            window.setTimeout(function () {
                                $el.tooltip('destroy');
                            }, 1500);
                        }
                    }
                }
            },
            success: function (label) {
                label.addClass('checked');
            }
        }), options);
    }
    Q.validateOptions = validateOptions;
    ;
    if ($.validator)
        jQueryValidationInitialization();
    else
        $(function () {
            $.validator && jQueryValidationInitialization();
        });
})(Q || (Q = {}));
var Serenity;
(function (Serenity) {
    var ValidationHelper;
    (function (ValidationHelper) {
        function asyncSubmit(form, validateBeforeSave, submitHandler) {
            var validator = form.validate();
            var valSettings = validator.settings;
            if (valSettings.abortHandler) {
                return false;
            }
            if (validateBeforeSave != null && validateBeforeSave() === false) {
                return false;
            }
            valSettings['abortHandler'] = Q.validatorAbortHandler;
            valSettings['submitHandler'] = function () {
                if (submitHandler != null) {
                    submitHandler();
                }
                return false;
            };
            form.trigger('submit');
            return true;
        }
        ValidationHelper.asyncSubmit = asyncSubmit;
        function submit(form, validateBeforeSave, submitHandler) {
            var validator = form.validate();
            var valSettings = validator.settings;
            if (valSettings.abortHandler != null) {
                return false;
            }
            if (validateBeforeSave != null && validateBeforeSave() === false) {
                return false;
            }
            if (!validator.form()) {
                return false;
            }
            if (submitHandler != null) {
                submitHandler();
            }
            return true;
        }
        ValidationHelper.submit = submit;
        function getValidator(element) {
            var form = element.closest('form');
            if (form.length === 0) {
                return null;
            }
            return form.data('validator');
        }
        ValidationHelper.getValidator = getValidator;
    })(ValidationHelper = Serenity.ValidationHelper || (Serenity.ValidationHelper = {}));
    var VX;
    (function (VX) {
        VX.addValidationRule = Q.addValidationRule;
        VX.removeValidationRule = Q.removeValidationRule;
        function validateElement(validator, widget) {
            return validator.element(widget.element[0]);
        }
        VX.validateElement = validateElement;
    })(VX = Serenity.VX || (Serenity.VX = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedPanel = /** @class */ (function (_super) {
        __extends(TemplatedPanel, _super);
        function TemplatedPanel(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.initValidator();
            _this.initTabs();
            _this.initToolbar();
            return _this;
        }
        TemplatedPanel.prototype.destroy = function () {
            if (this.tabs) {
                this.tabs.tabs('destroy');
                this.tabs = null;
            }
            if (this.toolbar) {
                this.toolbar.destroy();
                this.toolbar = null;
            }
            if (this.validator) {
                this.byId('Form').remove();
                this.validator = null;
            }
            _super.prototype.destroy.call(this);
        };
        TemplatedPanel.prototype.arrange = function () {
            this.element.find('.require-layout').filter(':visible').each(function (i, e) {
                $(e).triggerHandler('layout');
            });
        };
        TemplatedPanel.prototype.getToolbarButtons = function () {
            return [];
        };
        TemplatedPanel.prototype.getValidatorOptions = function () {
            return {};
        };
        TemplatedPanel.prototype.initTabs = function () {
            var tabsDiv = this.byId('Tabs');
            if (tabsDiv.length === 0) {
                return;
            }
            this.tabs = tabsDiv.tabs({});
        };
        TemplatedPanel.prototype.initToolbar = function () {
            var toolbarDiv = this.byId('Toolbar');
            if (toolbarDiv.length === 0) {
                return;
            }
            var opt = { buttons: this.getToolbarButtons() };
            this.toolbar = new Serenity.Toolbar(toolbarDiv, opt);
        };
        TemplatedPanel.prototype.initValidator = function () {
            var form = this.byId('Form');
            if (form.length > 0) {
                var valOptions = this.getValidatorOptions();
                this.validator = form.validate(Q.validateOptions(valOptions));
            }
        };
        TemplatedPanel.prototype.resetValidation = function () {
            if (this.validator) {
                this.validator.resetAll();
            }
        };
        TemplatedPanel.prototype.validateForm = function () {
            return this.validator == null || !!this.validator.form();
        };
        return TemplatedPanel;
    }(Serenity.TemplatedWidget));
    Serenity.TemplatedPanel = TemplatedPanel;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TabsExtensions;
    (function (TabsExtensions) {
        function setDisabled(tabs, tabKey, isDisabled) {
            if (!tabs)
                return;
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
            if (isDisabled && index === tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', 0);
            }
            tabs.tabs(isDisabled ? 'disable' : 'enable', index);
        }
        TabsExtensions.setDisabled = setDisabled;
        function toggle(tabs, tabKey, visible) {
            if (!tabs)
                return;
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
            if (!visible && index === tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', 0);
            }
            tabs.children('ul').children('li').eq(index).toggle(visible);
        }
        TabsExtensions.toggle = toggle;
        function activeTabKey(tabs) {
            var href = tabs.children('ul')
                .children('li')
                .eq(tabs.tabs('option', 'active'))
                .children('a')
                .attr('href')
                .toString();
            var prefix = '_Tab';
            var lastIndex = href.lastIndexOf(prefix);
            if (lastIndex >= 0) {
                href = href.substr(lastIndex + prefix.length);
            }
            return href;
        }
        TabsExtensions.activeTabKey = activeTabKey;
        function indexByKey(tabs) {
            var indexByKey = tabs.data('indexByKey');
            if (!indexByKey) {
                indexByKey = {};
                tabs.children('ul').children('li').children('a').each(function (index, el) {
                    var href = el.getAttribute('href').toString();
                    var prefix = '_Tab';
                    var lastIndex = href.lastIndexOf(prefix);
                    if (lastIndex >= 0) {
                        href = href.substr(lastIndex + prefix.length);
                    }
                    indexByKey[href] = index;
                });
                tabs.data('indexByKey', indexByKey);
            }
            return indexByKey;
        }
        TabsExtensions.indexByKey = indexByKey;
        function selectTab(tabs, tabKey) {
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
            if (index !== tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', index);
            }
        }
        TabsExtensions.selectTab = selectTab;
    })(TabsExtensions = Serenity.TabsExtensions || (Serenity.TabsExtensions = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ReflectionOptionsSetter;
    (function (ReflectionOptionsSetter) {
        function set(target, options) {
            if (options == null) {
                return;
            }
            var type = Q.getInstanceType(target);
            if (type === Object) {
                return;
            }
            var propByName = type.__propByName;
            var fieldByName = type.__fieldByName;
            if (propByName == null) {
                var props = Q.getMembers(type, 16 /* property */);
                var propList = props.filter(function (x) {
                    return !!x.setter && ((x.attr || []).filter(function (a) {
                        return Q.isInstanceOfType(a, Serenity.OptionAttribute);
                    }).length > 0 || (x.attr || []).filter(function (a) {
                        return Q.isInstanceOfType(a, System.ComponentModel.DisplayNameAttribute);
                    }).length > 0);
                });
                propByName = {};
                for (var _i = 0, propList_1 = propList; _i < propList_1.length; _i++) {
                    var k = propList_1[_i];
                    propByName[Serenity.ReflectionUtils.makeCamelCase(k.name)] = k;
                }
                type.__propByName = propByName;
            }
            if (fieldByName == null) {
                var fields = Q.getMembers(type, 4 /* field */);
                var fieldList = fields.filter(function (x1) {
                    return (x1.attr || []).filter(function (a) {
                        return Q.isInstanceOfType(a, Serenity.OptionAttribute);
                    }).length > 0 || (x1.attr || []).filter(function (a) {
                        return Q.isInstanceOfType(a, System.ComponentModel.DisplayNameAttribute);
                    }).length > 0;
                });
                fieldByName = {};
                for (var $t2 = 0; $t2 < fieldList.length; $t2++) {
                    var k1 = fieldList[$t2];
                    fieldByName[Serenity.ReflectionUtils.makeCamelCase(k1.name)] = k1;
                }
                type.__fieldByName = fieldByName;
            }
            var keys = Object.keys(options);
            for (var _a = 0, keys_1 = keys; _a < keys_1.length; _a++) {
                var k2 = keys_1[_a];
                var v = options[k2];
                var cc = Serenity.ReflectionUtils.makeCamelCase(k2);
                var p = propByName[cc] || propByName[k2];
                if (p != null) {
                    var func = target[p.setter];
                    func && func.call(target, v);
                }
                else {
                    var f = fieldByName[cc] || fieldByName[k2];
                    f && (target[f.name] = v);
                }
            }
        }
        ReflectionOptionsSetter.set = set;
    })(ReflectionOptionsSetter = Serenity.ReflectionOptionsSetter || (Serenity.ReflectionOptionsSetter = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var PropertyGrid = /** @class */ (function (_super) {
        __extends(PropertyGrid, _super);
        function PropertyGrid(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            _this.categoryLinkClick = function (e) {
                e.preventDefault();
                var title = $('a[name=' + e.target.getAttribute('href')
                    .toString().substr(1) + ']');
                if (title.closest('.category').hasClass('collapsed'))
                    title.closest('.category').children('.category-title').click();
                var animate = function () {
                    title.fadeTo(100, 0.5, function () {
                        title.fadeTo(100, 1, function () {
                        });
                    });
                };
                var intoView = title.closest('.category');
                if (intoView.closest(':scrollable(both)').length === 0)
                    animate();
                else {
                    intoView.scrollintoview({
                        duration: 'fast',
                        direction: 'y',
                        complete: animate
                    });
                }
            };
            if (opt.mode == null)
                opt.mode = 1;
            div.addClass('s-PropertyGrid');
            _this.editors = [];
            var items = _this.options.items || [];
            _this.items = [];
            var useTabs = Q.any(items, function (x) {
                return !Q.isEmptyOrNull(x.tab);
            });
            if (useTabs) {
                var itemsWithoutTab = items.filter(function (f) { return Q.isEmptyOrNull(f.tab); });
                if (itemsWithoutTab.length > 0) {
                    _this.createItems(_this.element, itemsWithoutTab);
                    $("<div class='pad'></div>").appendTo(_this.element);
                }
                var itemsWithTab = items.filter(function (f) { return !Q.isEmptyOrNull(f.tab); });
                var ul = $("<ul class='nav nav-tabs property-tabs' role='tablist'></ul>")
                    .appendTo(_this.element);
                var tc = $("<div class='tab-content property-panes'></div>")
                    .appendTo(_this.element);
                var tabIndex = 0;
                var i = 0;
                while (i < itemsWithTab.length) {
                    var tab = { $: Q.trimToEmpty(itemsWithTab[i].tab) };
                    var tabItems = [];
                    var j = i;
                    do {
                        tabItems.push(itemsWithTab[j]);
                    } while (++j < itemsWithTab.length &&
                        Q.trimToEmpty(itemsWithTab[j].tab) === tab.$);
                    i = j;
                    var li = $("<li><a data-toggle='tab' role='tab'></a></li>")
                        .appendTo(ul);
                    if (tabIndex === 0) {
                        li.addClass('active');
                    }
                    var tabID = _this.uniqueName + '_Tab' + tabIndex;
                    li.children('a').attr('href', '#' + tabID)
                        .text(_this.determineText(tab.$, function (prefix) {
                        return prefix + 'Tabs.' + this.tab.$;
                    }.bind({
                        tab: tab
                    })));
                    var pane = $("<div class='tab-pane fade' role='tabpanel'>")
                        .appendTo(tc);
                    if (tabIndex === 0) {
                        pane.addClass('in active');
                    }
                    pane.attr('id', tabID);
                    _this.createItems(pane, tabItems);
                    tabIndex++;
                }
            }
            else {
                _this.createItems(_this.element, items);
            }
            _this.updateInterface();
            return _this;
        }
        PropertyGrid_1 = PropertyGrid;
        PropertyGrid.prototype.destroy = function () {
            if (this.editors != null) {
                for (var i = 0; i < this.editors.length; i++) {
                    this.editors[i].destroy();
                }
                this.editors = null;
            }
            this.element.find('a.category-link').unbind('click', this.categoryLinkClick).remove();
            Serenity.Widget.prototype.destroy.call(this);
        };
        PropertyGrid.prototype.createItems = function (container, items) {
            var categoryIndexes = {};
            var categoriesDiv = container;
            var useCategories = this.options.useCategories !== false && Q.any(items, function (x) {
                return !Q.isEmptyOrNull(x.category);
            });
            if (useCategories) {
                var linkContainer = $('<div/>').addClass('category-links');
                categoryIndexes = this.createCategoryLinks(linkContainer, items);
                if (Object.keys(categoryIndexes).length > 1) {
                    linkContainer.appendTo(container);
                }
                else {
                    linkContainer.find('a.category-link').unbind('click', this.categoryLinkClick).remove();
                }
            }
            categoriesDiv = $('<div/>').addClass('categories').appendTo(container);
            var fieldContainer;
            if (useCategories) {
                fieldContainer = categoriesDiv;
            }
            else {
                fieldContainer = $('<div/>').addClass('category').appendTo(categoriesDiv);
            }
            var priorCategory = null;
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var category = item.category;
                if (category == null) {
                    category = Q.coalesce(this.options.defaultCategory, '');
                }
                if (useCategories && priorCategory !== category) {
                    var categoryDiv = this.createCategoryDiv(categoriesDiv, categoryIndexes, category, ((item.collapsible !== true) ? null :
                        Q.coalesce(item.collapsed, false)));
                    if (priorCategory == null) {
                        categoryDiv.addClass('first-category');
                    }
                    priorCategory = category;
                    fieldContainer = categoryDiv;
                }
                var editor = this.createField(fieldContainer, item);
                this.items.push(item);
                this.editors.push(editor);
            }
        };
        PropertyGrid.prototype.createCategoryDiv = function (categoriesDiv, categoryIndexes, category, collapsed) {
            var categoryDiv = $('<div/>').addClass('category')
                .appendTo(categoriesDiv);
            var title = $('<div/>').addClass('category-title')
                .append($('<a/>').addClass('category-anchor')
                .text(this.determineText(category, function (prefix) {
                return prefix + 'Categories.' + category;
            }))
                .attr('name', this.options.idPrefix +
                'Category' + categoryIndexes[category].toString()))
                .appendTo(categoryDiv);
            if (collapsed != null) {
                categoryDiv.addClass(((collapsed === true) ?
                    'collapsible collapsed' : 'collapsible'));
                var img = $('<i/>').addClass(((collapsed === true) ?
                    'fa fa-plus' : 'fa fa-minus')).appendTo(title);
                title.click(function (e) {
                    categoryDiv.toggleClass('collapsed');
                    img.toggleClass('fa-plus').toggleClass('fa-minus');
                });
            }
            return categoryDiv;
        };
        PropertyGrid.prototype.determineText = function (text, getKey) {
            if (text != null && !Q.startsWith(text, '`')) {
                var local = Q.tryGetText(text);
                if (local != null) {
                    return local;
                }
            }
            if (text != null && Q.startsWith(text, '`')) {
                text = text.substr(1);
            }
            if (!Q.isEmptyOrNull(this.options.localTextPrefix)) {
                var local1 = Q.tryGetText(getKey(this.options.localTextPrefix));
                if (local1 != null) {
                    return local1;
                }
            }
            return text;
        };
        PropertyGrid.prototype.createField = function (container, item) {
            var fieldDiv = $('<div/>').addClass('field')
                .addClass(item.name).data('PropertyItem', item).appendTo(container);
            if (!Q.isEmptyOrNull(item.cssClass)) {
                fieldDiv.addClass(item.cssClass);
            }
            if (!Q.isEmptyOrNull(item.formCssClass)) {
                fieldDiv.addClass(item.formCssClass);
                if (item.formCssClass.indexOf('line-break-') >= 0) {
                    var splitted = item.formCssClass.split(String.fromCharCode(32));
                    if (splitted.indexOf('line-break-xs') >= 0) {
                        $("<div class='line-break' style='width: 100%' />")
                            .insertBefore(fieldDiv);
                    }
                    else if (splitted.indexOf('line-break-sm') >= 0) {
                        $("<div class='line-break hidden-xs' style='width: 100%' />")
                            .insertBefore(fieldDiv);
                    }
                    else if (splitted.indexOf('line-break-md') >= 0) {
                        $("<div class='line-break hidden-sm' style='width: 100%' />")
                            .insertBefore(fieldDiv);
                    }
                    else if (splitted.indexOf('line-break-lg') >= 0) {
                        $("<div class='line-break hidden-md' style='width: 100%' />")
                            .insertBefore(fieldDiv);
                    }
                }
            }
            var editorId = this.options.idPrefix + item.name;
            var title = this.determineText(item.title, function (prefix) {
                return prefix + item.name;
            });
            var hint = this.determineText(item.hint, function (prefix1) {
                return prefix1 + item.name + '_Hint';
            });
            var placeHolder = this.determineText(item.placeholder, function (prefix2) {
                return prefix2 + item.name + '_Placeholder';
            });
            if (hint == null) {
                hint = Q.coalesce(title, '');
            }
            var label = $('<label/>')
                .addClass('caption').attr('for', editorId)
                .attr('title', hint).html(Q.coalesce(title, ''))
                .appendTo(fieldDiv);
            if (!Q.isEmptyOrNull(item.labelWidth)) {
                if (item.labelWidth === '0') {
                    label.hide();
                }
                else {
                    label.css('width', item.labelWidth);
                }
            }
            if (item.required === true) {
                $('<sup>*</sup>').attr('title', Q.text('Controls.PropertyGrid.RequiredHint'))
                    .prependTo(label);
            }
            var editorType = Serenity.EditorTypeRegistry
                .get(Q.coalesce(item.editorType, 'String'));
            var elementAttr = Q.getAttributes(editorType, Serenity.ElementAttribute, true);
            var elementHtml = ((elementAttr.length > 0) ?
                elementAttr[0].value : '<input/>');
            var element = Serenity.Widget.elementFor(editorType)
                .addClass('editor')
                .attr('id', editorId).appendTo(fieldDiv);
            if (element.is(':input')) {
                element.attr('name', Q.coalesce(item.name, ''));
            }
            if (!Q.isEmptyOrNull(placeHolder)) {
                element.attr('placeholder', placeHolder);
            }
            var editorParams = item.editorParams;
            var optionsType = null;
            var optionsAttr = Q.getAttributes(editorType, Serenity.OptionsTypeAttribute, true);
            if (optionsAttr != null && optionsAttr.length > 0) {
                optionsType = optionsAttr[0].optionsType;
            }
            var editor;
            if (optionsType != null) {
                editorParams = Q.extend(new optionsType(), item.editorParams);
                editor = new editorType(element, editorParams);
            }
            else {
                editorParams = Q.extend(new Object(), item.editorParams);
                editor = new editorType(element, editorParams);
            }
            editor.initialize();
            if (Q.getTypeName(editor) == "BooleanEditor" &&
                (item.editorParams == null || !!!item.editorParams['labelFor'])) {
                label.removeAttr('for');
            }
            if (Q.getTypeName(editor) == "RadioButtonEditor" &&
                (item.editorParams == null || !!!item.editorParams['labelFor'])) {
                label.removeAttr('for');
            }
            if (item.maxLength != null) {
                PropertyGrid_1.setMaxLength(editor, item.maxLength);
            }
            if (item.editorParams != null) {
                Serenity.ReflectionOptionsSetter.set(editor, item.editorParams);
            }
            $('<div/>').addClass('vx').appendTo(fieldDiv);
            $('<div/>').addClass('clear').appendTo(fieldDiv);
            return editor;
        };
        PropertyGrid.prototype.getCategoryOrder = function (items) {
            var order = 0;
            var result = {};
            var categoryOrder = Q.trimToNull(this.options.categoryOrder);
            if (categoryOrder != null) {
                var split = categoryOrder.split(';');
                for (var _i = 0, split_1 = split; _i < split_1.length; _i++) {
                    var s = split_1[_i];
                    var x = Q.trimToNull(s);
                    if (x == null) {
                        continue;
                    }
                    if (result[x] != null) {
                        continue;
                    }
                    result[x] = order++;
                }
            }
            for (var _a = 0, items_1 = items; _a < items_1.length; _a++) {
                var x1 = items_1[_a];
                var category = x1.category;
                if (category == null) {
                    category = Q.coalesce(this.options.defaultCategory, '');
                }
                if (result[category] == null) {
                    result[category] = order++;
                }
            }
            return result;
        };
        PropertyGrid.prototype.createCategoryLinks = function (container, items) {
            var idx = 0;
            var itemIndex = {};
            var itemCategory = {};
            for (var _i = 0, items_2 = items; _i < items_2.length; _i++) {
                var x = items_2[_i];
                var name1 = x.name;
                var cat1 = x.category;
                if (cat1 == null) {
                    cat1 = Q.coalesce(this.options.defaultCategory, '');
                }
                itemCategory[name1] = cat1;
                itemIndex[x.name] = idx++;
            }
            var self = this;
            var categoryOrder = this.getCategoryOrder(items);
            items.sort(function (x1, y) {
                var c = 0;
                var xcategory = itemCategory[x1.name];
                var ycategory = itemCategory[y.name];
                if (xcategory != ycategory) {
                    var c1 = categoryOrder[xcategory];
                    var c2 = categoryOrder[ycategory];
                    if (c1 != null && c2 != null) {
                        c = c1 - c2;
                    }
                    else if (c1 != null) {
                        c = -1;
                    }
                    else if (c2 != null) {
                        c = 1;
                    }
                }
                if (c === 0) {
                    c = Q.Culture.stringCompare(xcategory, ycategory);
                }
                if (c === 0) {
                    c = itemIndex[x1.name] < itemIndex[y.name] ? -1 : (itemIndex[x1.name] > itemIndex[y.name] ? 1 : 0);
                }
                return c;
            });
            var categoryIndexes = {};
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var category = { $: itemCategory[item.name] };
                if (categoryIndexes[category.$] == null) {
                    var index = Object.keys(categoryIndexes).length + 1;
                    categoryIndexes[category.$] = index;
                    if (index > 1) {
                        $('<span/>').addClass('separator').text('|').prependTo(container);
                    }
                    $('<a/>').addClass('category-link').text(this.determineText(category.$, function (prefix) {
                        return prefix + 'Categories.' + this.category.$;
                    }.bind({ category: category })))
                        .attr('tabindex', '-1')
                        .attr('href', '#' + this.options.idPrefix +
                        'Category' + index.toString())
                        .click(this.categoryLinkClick)
                        .prependTo(container);
                }
            }
            $('<div/>').addClass('clear').appendTo(container);
            return categoryIndexes;
        };
        PropertyGrid.prototype.get_editors = function () {
            return this.editors;
        };
        PropertyGrid.prototype.get_items = function () {
            return this.items;
        };
        PropertyGrid.prototype.get_idPrefix = function () {
            return this.options.idPrefix;
        };
        PropertyGrid.prototype.get_mode = function () {
            return this.options.mode;
        };
        PropertyGrid.prototype.set_mode = function (value) {
            if (this.options.mode !== value) {
                this.options.mode = value;
                this.updateInterface();
            }
        };
        // Obsolete
        PropertyGrid.loadEditorValue = function (editor, item, source) {
        };
        // Obsolete
        PropertyGrid.saveEditorValue = function (editor, item, target) {
            Serenity.EditorUtils.saveValue(editor, item, target);
        };
        // Obsolete
        PropertyGrid.setReadOnly = function (widget, isReadOnly) {
            Serenity.EditorUtils.setReadOnly(widget, isReadOnly);
        };
        // Obsolete
        PropertyGrid.setReadonly = function (elements, isReadOnly) {
            return Serenity.EditorUtils.setReadonly(elements, isReadOnly);
        };
        // Obsolete
        PropertyGrid.setRequired = function (widget, isRequired) {
            Serenity.EditorUtils.setRequired(widget, isRequired);
        };
        PropertyGrid.setMaxLength = function (widget, maxLength) {
            if (widget.element.is(':input')) {
                if (maxLength > 0) {
                    widget.element.attr('maxlength', maxLength);
                }
                else {
                    widget.element.removeAttr('maxlength');
                }
            }
        };
        PropertyGrid.prototype.load = function (source) {
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                var editor = this.editors[i];
                if (!!(this.get_mode() === 1 && item.defaultValue != null) &&
                    typeof (source[item.name]) === 'undefined') {
                    source[item.name] = item.defaultValue;
                }
                Serenity.EditorUtils.loadValue(editor, item, source);
            }
        };
        PropertyGrid.prototype.save = function (target) {
            if (target == null)
                target = Object.create(null);
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                if (item.oneWay !== true && this.canModifyItem(item)) {
                    var editor = this.editors[i];
                    Serenity.EditorUtils.saveValue(editor, item, target);
                }
            }
            return target;
        };
        Object.defineProperty(PropertyGrid.prototype, "value", {
            get: function () {
                return this.save();
            },
            set: function (val) {
                if (val == null)
                    val = Object.create(null);
                this.load(val);
            },
            enumerable: true,
            configurable: true
        });
        PropertyGrid.prototype.canModifyItem = function (item) {
            if (this.get_mode() === 1 /* insert */) {
                if (item.insertable === false) {
                    return false;
                }
                if (item.insertPermission == null) {
                    return true;
                }
                return Q.Authorization.hasPermission(item.insertPermission);
            }
            else if (this.get_mode() === 2 /* update */) {
                if (item.updatable === false) {
                    return false;
                }
                if (item.updatePermission == null) {
                    return true;
                }
                return Q.Authorization.hasPermission(item.updatePermission);
            }
            return true;
        };
        PropertyGrid.prototype.updateInterface = function () {
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                var editor = this.editors[i];
                var readOnly = item.readOnly === true || !this.canModifyItem(item);
                Serenity.EditorUtils.setReadOnly(editor, readOnly);
                Serenity.EditorUtils.setRequired(editor, !readOnly &&
                    !!item.required && item.editorType !== 'Boolean');
                if (item.visible === false || item.readPermission != null ||
                    item.insertPermission != null || item.updatePermission != null ||
                    item.hideOnInsert === true || item.hideOnUpdate === true) {
                    var hidden = (item.readPermission != null &&
                        !Q.Authorization.hasPermission(item.readPermission)) ||
                        item.visible === false ||
                        (this.get_mode() === 1 /* insert */ && item.hideOnInsert === true) ||
                        (this.get_mode() === 2 && item.hideOnUpdate === true);
                    editor.getGridField().toggle(!hidden);
                }
            }
        };
        PropertyGrid.prototype.enumerateItems = function (callback) {
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                var editor = this.editors[i];
                callback(item, editor);
            }
        };
        var PropertyGrid_1;
        PropertyGrid = PropertyGrid_1 = __decorate([
            Serenity.Decorators.registerClass('PropertyGrid')
        ], PropertyGrid);
        return PropertyGrid;
    }(Serenity.Widget));
    Serenity.PropertyGrid = PropertyGrid;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var PropertyPanel = /** @class */ (function (_super) {
        __extends(PropertyPanel, _super);
        function PropertyPanel(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.initPropertyGrid();
            _this.loadInitialEntity();
            return _this;
        }
        PropertyPanel.prototype.destroy = function () {
            if (this.propertyGrid) {
                this.propertyGrid.destroy();
                this.propertyGrid = null;
            }
            if (this.validator) {
                this.byId('Form').remove();
                this.validator = null;
            }
            _super.prototype.destroy.call(this);
        };
        PropertyPanel.prototype.initPropertyGrid = function () {
            var pgDiv = this.byId('PropertyGrid');
            if (pgDiv.length <= 0) {
                return;
            }
            var pgOptions = this.getPropertyGridOptions();
            this.propertyGrid = (new Serenity.PropertyGrid(pgDiv, pgOptions)).init(null);
            if (this.element.closest('.ui-Panel').hasClass('s-Flexify')) {
                this.propertyGrid.element.children('.categories').flexHeightOnly(1);
            }
        };
        PropertyPanel.prototype.loadInitialEntity = function () {
            if (this.propertyGrid) {
                this.propertyGrid.load(new Object());
            }
        };
        PropertyPanel.prototype.getFormKey = function () {
            var attributes = Q.getAttributes(Q.getInstanceType(this), Serenity.FormKeyAttribute, true);
            if (attributes.length >= 1) {
                return attributes[0].value;
            }
            var name = Q.getTypeFullName(Q.getInstanceType(this));
            var px = name.indexOf('.');
            if (px >= 0) {
                name = name.substring(px + 1);
            }
            if (Q.endsWith(name, 'Panel')) {
                name = name.substr(0, name.length - 6);
            }
            else if (Q.endsWith(name, 'Panel')) {
                name = name.substr(0, name.length - 5);
            }
            return name;
        };
        PropertyPanel.prototype.getPropertyGridOptions = function () {
            return {
                idPrefix: this.idPrefix,
                items: this.getPropertyItems(),
                mode: 1,
                useCategories: false,
                localTextPrefix: 'Forms.' + this.getFormKey() + '.'
            };
        };
        PropertyPanel.prototype.getPropertyItems = function () {
            var formKey = this.getFormKey();
            return Q.getForm(formKey);
        };
        PropertyPanel.prototype.getSaveEntity = function () {
            var entity = new Object();
            if (this.propertyGrid) {
                this.propertyGrid.save(entity);
            }
            return entity;
        };
        PropertyPanel.prototype.get_entity = function () {
            return this._entity;
        };
        PropertyPanel.prototype.get_entityId = function () {
            return this._entityId;
        };
        PropertyPanel.prototype.set_entity = function (value) {
            this._entity = Q.coalesce(value, new Object());
        };
        PropertyPanel.prototype.set_entityId = function (value) {
            this._entityId = value;
        };
        PropertyPanel.prototype.validateBeforeSave = function () {
            return this.validator.form();
        };
        PropertyPanel = __decorate([
            Serenity.Decorators.registerClass('Serenity.PropertyPanel')
        ], PropertyPanel);
        return PropertyPanel;
    }(Serenity.TemplatedPanel));
    Serenity.PropertyPanel = PropertyPanel;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    function vueIntegration() {
        // @ts-ignore
        if (typeof Vue === "undefined")
            return false;
        // @ts-ignore
        Vue.component('editor', {
            props: {
                type: {
                    type: String,
                    required: true,
                },
                id: {
                    type: String,
                    required: false
                },
                name: {
                    type: String,
                    required: false
                },
                placeholder: {
                    type: String,
                    required: false
                },
                value: {
                    required: false
                },
                options: {
                    required: false
                },
                maxLength: {
                    required: false
                }
            },
            render: function (createElement) {
                var editorType = Serenity.EditorTypeRegistry.get(this.type);
                var elementAttr = Q.getAttributes(editorType, Serenity.ElementAttribute, true);
                var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>');
                var domProps = {};
                var element = $(elementHtml)[0];
                var attrs = element.attributes;
                for (var i = 0; i < attrs.length; i++) {
                    var attr = attrs.item(i);
                    domProps[attr.name] = attr.value;
                }
                if (this.id != null)
                    domProps.id = this.id;
                if (this.name != null)
                    domProps.name = this.name;
                if (this.placeholder != null)
                    domProps.placeholder = this.placeholder;
                var editorParams = this.options;
                var optionsType = null;
                var self = this;
                var el = createElement(element.tagName, {
                    domProps: domProps
                });
                this.$editorType = editorType;
                return el;
            },
            watch: {
                value: function (v) {
                    Serenity.EditorUtils.setValue(this.$widget, v);
                }
            },
            mounted: function () {
                var self = this;
                this.$widget = new this.$editorType($(this.$el), this.options);
                this.$widget.initialize();
                if (this.maxLength) {
                    Serenity.PropertyGrid.$setMaxLength(this.$widget, this.maxLength);
                }
                if (this.options)
                    Serenity.ReflectionOptionsSetter.set(this.$widget, this.options);
                if (this.value != null)
                    Serenity.EditorUtils.setValue(this.$widget, this.value);
                if ($(this.$el).data('select2'))
                    this.$widget.changeSelect2(function () {
                        self.$emit('input', Serenity.EditorUtils.getValue(self.$widget));
                    });
                else
                    this.$widget.change(function () {
                        self.$emit('input', Serenity.EditorUtils.getValue(self.$widget));
                    });
            },
            destroyed: function () {
                if (this.$widget) {
                    this.$widget.destroy();
                    this.$widget = null;
                }
            }
        });
        return true;
    }
    // @ts-ignore
    !vueIntegration() && typeof $ !== "undefined" && $(vueIntegration);
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-forms.js.map