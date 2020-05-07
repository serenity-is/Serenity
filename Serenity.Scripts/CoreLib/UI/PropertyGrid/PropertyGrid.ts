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
            var items = this.options.items || [];
            this.items = [];

            var useTabs = Q.any(items, function (x) {
                return !Q.isEmptyOrNull(x.tab);
            });

            if (useTabs) {
                var itemsWithoutTab = items.filter(f => Q.isEmptyOrNull(f.tab));
                if (itemsWithoutTab.length > 0) {
                    this.createItems(this.element, itemsWithoutTab);

                    $("<div class='pad'></div>").appendTo(this.element);
                }

                var itemsWithTab = items.filter(f => !Q.isEmptyOrNull(f.tab));

                var ul = $("<ul class='nav nav-tabs property-tabs' role='tablist'></ul>")
                    .appendTo(this.element);

                var tc = $("<div class='tab-content property-panes'></div>")
                    .appendTo(this.element);

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

                    var tabID = this.uniqueName + '_Tab' + tabIndex;

                    li.children('a').attr('href', '#' + tabID)
                        .text(this.determineText(tab.$, function (prefix: string) {
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
                    this.createItems(pane, tabItems);
                    tabIndex++;
                }
            }
            else {
                this.createItems(this.element, items);
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
                this.items.push(item);
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
            var elementAttr = Q.getAttributes(editorType,
                Serenity.ElementAttribute, true);
            var elementHtml = ((elementAttr.length > 0) ?
                elementAttr[0].value : '<input/>');

            var element = Widget.elementFor(editorType as any)
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
            var optionsAttr = Q.getAttributes(editorType,
                Serenity.OptionsTypeAttribute, true);

            if (optionsAttr != null && optionsAttr.length > 0) {
                optionsType = optionsAttr[0].optionsType;
            }
            var editor;
            if (optionsType != null) {
                editorParams = Q.extend(new optionsType(), item.editorParams);
                editor = new (editorType as any)(element, editorParams);
            }
            else {
                editorParams = Q.extend(new Object(), item.editorParams);
                editor = new (editorType as any)(element, editorParams);
            }

            editor.initialize();

            if (Q.isInstanceOfType(editor, BooleanEditor) &&
                (item.editorParams == null || !!!item.editorParams['labelFor'])) {
                label.removeAttr('for');
            }

            if (Q.isInstanceOfType(editor, RadioButtonEditor) &&
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
					c = itemIndex[x1.name] < itemIndex[y.name] ? -1 : (itemIndex[x1.name] > itemIndex[y.name] ? 1 : 0)
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
                            function (prefix: string) {
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

        save(target?: any): any {
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
        }

        public get value(): any {
            return this.save();
        }

        public set value(val: any) {
            if (val == null)
                val = Object.create(null);
            this.load(val);
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
}