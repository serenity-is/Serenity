namespace Serenity {

    @Decorators.registerClass('PropertyGrid')
    export class PropertyGrid extends Widget<PropertyGridOptions> {

        private editors: Widget<any>[];
        private items: PropertyItem[];

        constructor(div: JQuery, opt: PropertyGridOptions) {
            super(div, opt);

            if (opt.mode == null)
                opt.mode = 1;

            div.addClass('s-PropertyGrid');
            this.editors = [];
            this.items = this.options.items || [];

            var useTabs = Q.any(this.items, function (x) {
                return !Q.isEmptyOrNull(x.tab);
            });

            if (useTabs) {
                var ul = $("<ul class='nav nav-tabs property-tabs' role='tablist'></ul>")
                    .appendTo(this.element);

                var tc = $("<div class='tab-content property-panes'></div>")
                    .appendTo(this.element);

                var tabIndex = 0;
                var i = 0;
                while (i < this.items.length) {
                    var tab = { $: Q.trimToEmpty(this.items[i].tab) };
                    var tabItems = [];

                    var j = i;
                    do {
                        tabItems.push(this.items[j]);
                    } while (++j < this.items.length &&
                        Q.trimToEmpty(this.items[j].tab) === tab.$);
                    i = j;

                    var li = $("<li><a data-toggle='tab' role='tab'></a></li>")
                        .appendTo(ul);

                    if (tabIndex === 0) {
                        li.addClass('active');
                    }

                    var tabID = this.uniqueName + '_Tab' + tabIndex;

                    li.children('a').attr('href', '#' + tabID)
                        .text(this.determineText(tab.$, (ss as any).mkdel({
                            tab: tab
                        }, function (prefix: string) {
                            return prefix + 'Tabs.' + this.tab.$;
                            })));

                    var pane = $("<div class='tab-pane fade' role='tabpanel'>")
                        .appendTo(tc);

                    if (tabIndex === 0) {
                        pane.addClass('in active');
                    }

                    pane.attr('id', tabID);
                    this.createItems(pane, tabItems);
                    tabIndex++;
                }
            }
            else {
                this.createItems(this.element, this.items);
            }

            this.updateInterface();
        }

        destroy() {
            if (this.editors != null) {
                for (var i = 0; i < this.editors.length; i++) {
                    this.editors[i].destroy();
                }
                this.editors = null;
            }
            this.element.find('a.category-link').unbind('click',
                this.categoryLinkClick).remove();

            Serenity.Widget.prototype.destroy.call(this);
        }

        private createItems(container: JQuery, items: PropertyItem[]) {
            var categoryIndexes = {};
            var categoriesDiv = container;

            var useCategories = this.options.useCategories && Q.any(items, function (x) {
                return !Q.isEmptyOrNull(x.category);
            });

            if (this.options.useCategories) {
                var linkContainer = $('<div/>').addClass('category-links');
                categoryIndexes = this.createCategoryLinks(linkContainer, items);
                if (Object.keys(categoryIndexes).length > 1) {
                    linkContainer.appendTo(container);
                }
                else {
                    linkContainer.find('a.category-link').unbind('click',
                        this.categoryLinkClick).remove();
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
                    var categoryDiv = this.createCategoryDiv(categoriesDiv,
                        categoryIndexes, category,
                        ((item.collapsible !== true) ? null :
                            Q.coalesce(item.collapsed, false)));

                    if (priorCategory == null) {
                        categoryDiv.addClass('first-category');
                    }
                    priorCategory = category;
                    fieldContainer = categoryDiv;
                }
                var editor = this.createField(fieldContainer, item);
                this.editors.push(editor);
            }
        }

        private createCategoryDiv(categoriesDiv: JQuery,
            categoryIndexes: Q.Dictionary<number>, category: string, collapsed: boolean) {

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
        }

        private categoryLinkClick = (e: JQueryEventObject) => {
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
                (intoView as any).scrollintoview({
                    duration: 'fast',
                    direction: 'y',
                    complete: animate
                });
            }
        }

        private determineText(text: string, getKey: (s: string) => string) {
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
        }

        private createField(container: JQuery, item: PropertyItem) {
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
                $('<sup>*</sup>').attr('title',
                    Q.text('Controls.PropertyGrid.RequiredHint'))
                    .prependTo(label);
            }

            var editorType = Serenity.EditorTypeRegistry
                .get(Q.coalesce(item.editorType, 'String'));
            var elementAttr = (ss as any).getAttributes(editorType,
                Serenity.ElementAttribute, true);
            var elementHtml = ((elementAttr.length > 0) ?
                elementAttr[0].value : '<input/>');

            var element = Widget.elementFor(editorType as any)
                .addClass('editor').addClass('flexify')
                .attr('id', editorId).appendTo(fieldDiv);

            if (element.is(':input')) {
                element.attr('name', Q.coalesce(item.name, ''));
            }

            if (!Q.isEmptyOrNull(placeHolder)) {
                element.attr('placeholder', placeHolder);
            }
            var editorParams = item.editorParams;
            var optionsType = null;
            var optionsAttr = (ss as any).getAttributes(editorType,
                Serenity.OptionsTypeAttribute, true);

            if (optionsAttr != null && optionsAttr.length > 0) {
                optionsType = optionsAttr[0].optionsType;
            }
            var editor;
            if (optionsType != null) {
                editorParams = $.extend((ss as any).createInstance(optionsType),
                    item.editorParams);

                editor = new (editorType as any)(element, editorParams);
            }
            else {
                editorParams = $.extend(new Object(), item.editorParams);
                editor = new (editorType as any)(element, editorParams);
            }

            editor.initialize();

            if ((ss as any).isInstanceOfType(editor, BooleanEditor) &&
                (item.editorParams == null || !!!item.editorParams['labelFor'])) {
                label.removeAttr('for');
            }

            if ((ss as any).isInstanceOfType(editor, RadioButtonEditor) &&
                (item.editorParams == null || !!!item.editorParams['labelFor'])) {
                label.removeAttr('for');
            }

            if (item.maxLength != null) {
                PropertyGrid.setMaxLength(editor, item.maxLength);
            }

            if (item.editorParams != null) {
                ReflectionOptionsSetter.set(editor, item.editorParams);
            }

            $('<div/>').addClass('vx').appendTo(fieldDiv);
            $('<div/>').addClass('clear').appendTo(fieldDiv);

            return editor;
        }

        private getCategoryOrder(items: PropertyItem[]) {
            var order = 0;
            var result = {};
            var categoryOrder = Q.trimToNull(this.options.categoryOrder);
            if (categoryOrder != null) {
                var split = categoryOrder.split(';');
                for (var s of split) {
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

            for (var x1 of items) {
                var category = x1.category;
                if (category == null) {
                    category = Q.coalesce(this.options.defaultCategory, '');
                }
                if (result[category] == null) {
                    result[category] = order++;
                }
            }

            return result;
        }

        private createCategoryLinks(container: JQuery, items: PropertyItem[]) {
			var idx = 0;
			var itemIndex = {};
			var itemCategory = {};
            for (var x of items) {
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
				if (!(ss as any).referenceEquals(xcategory, ycategory)) {
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
					c = (ss as any).compareStrings(xcategory, ycategory);
				}
				if (c === 0) {
					c = (ss as any).compare(itemIndex[x1.name], itemIndex[y.name]);
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
                    $('<a/>').addClass('category-link').text(
                        this.determineText(category.$,
                            (ss as any).mkdel({ category: category },
                                function (prefix: string) {
                                    return prefix + 'Categories.' + this.category.$;
                                })))
                        .attr('tabindex', '-1')
                        .attr('href', '#' + this.options.idPrefix +
                            'Category' + index.toString())
                        .click(this.categoryLinkClick)
                        .prependTo(container);
				}
            }

			$('<div/>').addClass('clear').appendTo(container);
			return categoryIndexes;
        }

        get_editors(): Widget<any>[] {
            return this.editors;
        }

        get_items(): PropertyItem[] {
            return this.items;
        }

        get_idPrefix(): string {
            return this.options.idPrefix;
        }

        get_mode(): PropertyGridMode {
            return this.options.mode;
        }

        set_mode(value: PropertyGridMode) {
            if(this.options.mode !== value) {
                this.options.mode = value;
                this.updateInterface();
            }
        }

        // Obsolete
        static loadEditorValue(editor: Serenity.Widget<any>,
            item: PropertyItem, source: any): void {
        }

        // Obsolete
        static saveEditorValue(editor: Serenity.Widget<any>,
            item: PropertyItem, target: any): void {

            EditorUtils.saveValue(editor, item, target);
        }

        // Obsolete
        private static setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void {
            EditorUtils.setReadOnly(widget, isReadOnly);
        }

        // Obsolete
        private static setReadonly(elements: JQuery, isReadOnly: boolean): JQuery {
            return EditorUtils.setReadonly(elements, isReadOnly);
        }

        // Obsolete
        private static setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void {
            EditorUtils.setRequired(widget, isRequired);
        }

        private static setMaxLength(widget: Widget<any>, maxLength: number) {
            if (widget.element.is(':input')) {
                if (maxLength > 0) {
                    widget.element.attr('maxlength', maxLength);
                }
                else {
                    widget.element.removeAttr('maxlength');
                }
            }
        }

        load(source: any): void {
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                var editor = this.editors[i];
                if (!!(this.get_mode() === 1 && item.defaultValue != null) &&
                    typeof (source[item.name]) === 'undefined') {
                    source[item.name] = item.defaultValue;
                }

                Serenity.EditorUtils.loadValue(editor, item, source);
            }
        }

        save(target: any): void {
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                if (item.oneWay !== true && this.canModifyItem(item)) {
                    var editor = this.editors[i];
                    Serenity.EditorUtils.saveValue(editor, item, target);
                }
            }
        }

        private canModifyItem(item: PropertyItem) {
            if (this.get_mode() === PropertyGridMode.insert) {
                if (item.insertable === false) {
                    return false;
                }

                if (item.insertPermission == null) {
                    return true;
                }

                return Q.Authorization.hasPermission(item.insertPermission);
            }
            else if (this.get_mode() === PropertyGridMode.update) {
                if (item.updatable === false) {
                    return false;
                }

                if (item.updatePermission == null) {
                    return true;
                }

                return Q.Authorization.hasPermission(item.updatePermission);
            }
			return true;
        }

        updateInterface() {
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
                        (this.get_mode() === PropertyGridMode.insert && item.hideOnInsert === true) ||
                        (this.get_mode() === 2 && item.hideOnUpdate === true);

                    editor.getGridField().toggle(!hidden);
                }
            }
        }

        enumerateItems(callback: (p1: PropertyItem, p2: Serenity.Widget<any>) => void): void {
            for (var i = 0; i < this.editors.length; i++) {
                var item = this.items[i];
                var editor = this.editors[i];
                callback(item, editor);
            }
        }
    }

    export declare const enum PropertyGridMode {
        insert = 1,
        update = 2
    }

    export interface PropertyGridOptions {
        idPrefix?: string;
        items?: PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
    }

    export namespace PropertyItemHelper {
        export function getPropertyItemsFor(type: Function): PropertyItem[] {
            if (type == null) {
                throw new ss.Exception('type');
            }
            var list = [];
            var $t1 = (ss as any).getMembers(type, 31, 20);
            for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                var member = $t1[$t2];
                var pi: PropertyItem = {};
                if (member.type !== 16 && member.type !== 4) {
                    continue;
                }
                var hiddenAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.HiddenAttribute);
                });
                if (hiddenAttribute.length > 0) {
                    continue;
                }
                var displayNameAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, System.ComponentModel.DisplayNameAttribute);
                });
                var hintAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.HintAttribute);
                });
                var placeholderAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.PlaceholderAttribute);
                });
                var memberType = ((member.type === 16) ? (ss as any).cast(member, member != null && member.type === 16)
                    .returnType : (ss as any).cast(member, member != null && member.type === 4).returnType);
                if (member.type === 16) {
                    var p = (ss as any).cast(member, member && member.type === 16);
                    if (p.fname == null) {
                        continue;
                    }
                    pi.name = p.fname;
                }
                else if (member.type === 4) {
                    var f = (ss as any).cast(member, member != null && member.type === 4);
                    pi.name = f.sname;
                }
                else {
                    continue;
                }
                var categoryAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.CategoryAttribute);
                });
                if (categoryAttribute.length > 0) {
                    pi.category = (ss as any).cast(categoryAttribute[0], Serenity.CategoryAttribute).category;
                }
                else if (list.length > 0) {
                    pi.category = list[list.length - 1].category;
                }
                var collapsibleAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.CollapsibleAttribute);
                });
                if (collapsibleAttribute.length > 0 && (ss as any).cast(collapsibleAttribute[0],
                        Serenity.CollapsibleAttribute).value) {
                    pi.collapsible = true;
                    pi.collapsed = (ss as any).cast(collapsibleAttribute[0], Serenity.CollapsibleAttribute).collapsed;
                }
                var cssClassAttr = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.CssClassAttribute);
                });
                if (cssClassAttr.length > 0) {
                    pi.cssClass = (ss as any).cast(cssClassAttr[0], Serenity.CssClassAttribute).cssClass;
                }
                if ((member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.OneWayAttribute);
                }).length > 0) {
                    pi.oneWay = true;
                }
                if ((member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.ReadOnlyAttribute);
                }).length > 0) {
                    pi.readOnly = true;
                }
                if (displayNameAttribute.length > 0) {
                    pi.title = (ss as any).cast(displayNameAttribute[0], System.ComponentModel.DisplayNameAttribute).displayName;
                }
                if (hintAttribute.length > 0) {
                    pi.hint = (ss as any).cast(hintAttribute[0], Serenity.HintAttribute).hint;
                }
                if (placeholderAttribute.length > 0) {
                    pi.placeholder = (ss as any).cast(placeholderAttribute[0], Serenity.PlaceholderAttribute).value;
                }
                if (pi.title == null) {
                    pi.title = pi.name;
                }
                var defaultValueAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.DefaultValueAttribute);
                });
                if (defaultValueAttribute.length === 1) {
                    pi.defaultValue = (ss as any).cast(defaultValueAttribute[0], Serenity.DefaultValueAttribute).value;
                }
                var insertableAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.InsertableAttribute);
                });
                if (insertableAttribute.length > 0) {
                    pi.insertable = insertableAttribute[0].value;
                }
                else {
                    pi.insertable = true;
                }
                var updatableAttribute = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.UpdatableAttribute);
                });
                if (updatableAttribute.length > 0) {
                    pi.updatable = updatableAttribute[0].value;
                }
                else {
                    pi.updatable = true;
                }
                var typeAttrArray = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.EditorTypeAttribute);
                });
                var nullableType = memberType;
                //Nullable.GetUnderlyingType(memberType);
                var enumType = null;
                if ((ss as any).isEnum(memberType)) {
                    enumType = memberType;
                }
                else if (nullableType && (ss as any).isEnum(nullableType)) {
                    enumType = nullableType;
                }
                if (typeAttrArray.length === 0) {
                    if (enumType) {
                        pi.editorType = 'Select';
                    }
                    else if (memberType === (ss as any).JsDate) {
                        pi.editorType = 'Date';
                    }
                    else if (memberType === Boolean) {
                        pi.editorType = 'Boolean';
                    }
                    else if (memberType === Number) {
                        pi.editorType = 'Decimal';
                    }
                    else if (memberType === (ss as any).Int32) {
                        pi.editorType = 'Integer';
                    }
                    else {
                        pi.editorType = 'String';
                    }
                }
                else {
                    var et = (ss as any).cast(typeAttrArray[0], Serenity.EditorTypeAttribute);
                    pi.editorType = et.editorType;
                    et.setParams(pi.editorParams);
                }
                var reqAttr = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.RequiredAttribute);
                });
                if (reqAttr.length > 0) {
                    pi.required = reqAttr[0].isRequired;
                }
                var maxLengthAttr = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.MaxLengthAttribute);
                });
                if (maxLengthAttr.length > 0) {
                    pi.maxLength = maxLengthAttr.maxLength;
                    pi.editorParams['maxLength'] = pi.maxLength;
                }
                var $t3 = (member.attr || []).filter(function (a: any) {
                    return (ss as any).isInstanceOfType(a, Serenity.EditorOptionAttribute);
                });
                for (var $t4 = 0; $t4 < $t3.length; $t4++) {
                    var param = $t3[$t4];
                    var key = param.key;
                    if (key != null && key.length >= 1) {
                        key = key.substr(0, 1).toLowerCase() + key.substring(1);
                    }
                    pi.editorParams[key] = param.value;
                }
                list.push(pi);
            }
            return list;
        }
    }
}