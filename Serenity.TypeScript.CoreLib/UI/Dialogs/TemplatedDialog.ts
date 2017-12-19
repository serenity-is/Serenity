﻿namespace Serenity {

    @Serenity.Decorators.registerClass([Serenity.IDialog])
    export class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {

        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;

        constructor(options?: TOptions) {
            super(Q.newBodyDiv().addClass('s-TemplatedDialog hidden'), options);

            this.element.attr("id", this.uniqueName);

            this.initValidator();
            this.initTabs();
            this.initToolbar();
        }

        private get isMarkedAsPanel() {
            var panelAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.PanelAttribute, true) as Serenity.PanelAttribute[];
            return panelAttr.length > 0 && panelAttr[panelAttr.length - 1].value !== false;
        }

        private get isResponsive() {
            return Q.Config.responsiveDialogs ||
                (ss as any).getAttributes((ss as any).getInstanceType(this), ResponsiveAttribute, true).length > 0;
        }

        private get isHideDelete() {
            var hideDelAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.HideDeleteAttribute, true) as Serenity.HideDeleteAttribute[];
            return hideDelAttr.length > 0 && hideDelAttr[hideDelAttr.length - 1].value !== false;
        }

        private get isHideApply() {
            var hideApplyAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.HideApplyAttribute, true) as Serenity.HideApplyAttribute[];
            return hideApplyAttr.length > 0 && hideApplyAttr[hideApplyAttr.length - 1].value !== false;
        }

        private get isHideSave() {
            var hideSaveAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.HideSaveAttribute, true) as Serenity.HideSaveAttribute[];
            return hideSaveAttr.length > 0 && hideSaveAttr[hideSaveAttr.length - 1].value !== false;
        }

        private get isHideUndo() {
            var hideUndoAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.HideUndoAttribute, true) as Serenity.HideUndoAttribute[];
            return hideUndoAttr.length > 0 && hideUndoAttr[hideUndoAttr.length - 1].value !== false;
        }

        private get isHideClone() {
            var hideCloneAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.HideCloneAttribute, true) as Serenity.HideCloneAttribute[];
            return hideCloneAttr.length > 0 && hideCloneAttr[hideCloneAttr.length - 1].value !== false;
        }

        private get isHideLocalization() {
            var hideLocAttr = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.HideLocalizationAttribute, true) as Serenity.HideLocalizationAttribute[];
            return hideLocAttr.length > 0 && hideLocAttr[hideLocAttr.length - 1].value !== false;
        }

        private static getCssSize(element: JQuery, name: string): number {
            var cssSize = element.css(name);
            if (cssSize == null) {
                return null;
            }

            if (!Q.endsWith(cssSize, 'px')) {
                return null;
            }

            cssSize = cssSize.substr(0, cssSize.length - 2);
            let i = Q.parseInteger(cssSize);
            if (i == null || isNaN(i) || i == 0)
                return null;

            return i;
        }

        private static applyCssSizes(opt: JQueryUI.DialogOptions, dialogClass: string) {
            let size: number;
            let dialog = $('<div/>').hide().addClass(dialogClass).appendTo(document.body);
            try {
                var sizeHelper = $('<div/>').addClass('size').appendTo(dialog);
                size = TemplatedDialog.getCssSize(sizeHelper, 'minWidth'); 
                if (size != null)
                    opt.minWidth = size;

                size = TemplatedDialog.getCssSize(sizeHelper, 'width');
                if (size != null)
                    opt.width = size;

                size = TemplatedDialog.getCssSize(sizeHelper, 'height');
                if (size != null)
                    opt.height = size;

                size = TemplatedDialog.getCssSize(sizeHelper, 'minHeight');
                if (size != null)
                    opt.minHeight = size;
            }
            finally {
                dialog.remove();
            }
        };

        public destroy(): void {
            this.tabs && this.tabs.tabs('destroy');
            this.tabs = null;
            this.toolbar && this.toolbar.destroy();
            this.toolbar = null;
            this.validator && this.byId('Form').remove();
            this.validator = null;
            if (this.element != null &&
                this.element.hasClass('ui-dialog-content')) {
                this.element.dialog('destroy');
                this.element.removeClass('ui-dialog-content');
            }
            $(window).unbind('.' + this.uniqueName);
            super.destroy();
        }

        protected initDialog(): void {
            if (this.element.hasClass('ui-dialog-content'))
                return;

            this.element.removeClass('hidden');

            this.element.dialog(this.getDialogOptions());
            this.element.closest('.ui-dialog').on('resize', e => this.arrange());

            let type = (ss as any).getInstanceType(this);

            if (this.isResponsive) {
                DialogExtensions.dialogResizable(this.element);

                $(window).bind('resize.' + this.uniqueName, e => {
                    if (this.element && this.element.is(':visible')) {
                        this.handleResponsive();
                    }
                });

                this.element.closest('.ui-dialog').addClass('flex-layout');
            }
            else if ((ss as any).getAttributes(type, FlexifyAttribute, true).length > 0) {
                DialogExtensions.dialogFlexify(this.element);
                DialogExtensions.dialogResizable(this.element);
            }

            if ((ss as any).getAttributes(type, MaximizableAttribute, true).length > 0) {
                DialogExtensions.dialogMaximizable(this.element);
            }

            var self = this;
            this.element.bind('dialogopen.' + this.uniqueName, () => {
                $(document.body).addClass('modal-dialog-open');

                if (this.isResponsive) {
                    this.handleResponsive();
                }

                self.onDialogOpen();
            });

            this.element.bind('dialogclose.' + this.uniqueName, () => {
                $(document.body).toggleClass('modal-dialog-open', $('.ui-dialog:visible').length > 0);
                self.onDialogClose();
            });
        }

        protected initToolbar(): void {
            var toolbarDiv = this.byId('Toolbar');
            if (toolbarDiv.length === 0) {
                return;
            }

            var hotkeyContext = this.element.closest('.ui-dialog');
            if (hotkeyContext.length === 0) {
                hotkeyContext = this.element;
            }

            var opt = { buttons: this.getToolbarButtons(), hotkeyContext: hotkeyContext[0] };

            if (this.isHideDelete)
                opt.buttons.splice(Q.indexOf(opt.buttons, x => x.cssClass == "delete-button"), 1);
            if (this.isHideApply)
                opt.buttons.splice(Q.indexOf(opt.buttons, x => x.cssClass == "apply-changes-button"), 1);
            if (this.isHideSave)
                opt.buttons.splice(Q.indexOf(opt.buttons, x => x.cssClass == "save-and-close-button"), 1);
            if (this.isHideUndo)
                opt.buttons.splice(Q.indexOf(opt.buttons, x => x.cssClass == "undo-delete-button"), 1);
            if (this.isHideClone)
                opt.buttons.splice(Q.indexOf(opt.buttons, x => x.cssClass == "clone-button"), 1);
            if (this.isHideLocalization)
                opt.buttons.splice(Q.indexOf(opt.buttons, x => x.cssClass == "localization-button"), 1);

            this.toolbar = new Toolbar(toolbarDiv, opt);
        }

        protected getToolbarButtons(): ToolButton[] {
            return [];
        }

        protected getValidatorOptions(): JQueryValidation.ValidationOptions {
            return {};
        }

        protected initValidator(): void {
            var form = this.byId('Form');
            if (form.length > 0) {
                var valOptions = this.getValidatorOptions();
                this.validator = form.validate(Q.validateOptions(valOptions));
            }
        }

        protected resetValidation() {
            this.validator && (this.validator as any).resetAll();
        }

        protected validateForm() {
            return this.validator == null || !!this.validator.form();
        }

        public dialogOpen(asPanel?: boolean): void {
            asPanel = Q.coalesce(asPanel, this.isMarkedAsPanel);
            if (asPanel) {
                if (!this.element.hasClass('s-Panel')) {
                    // so that panel title is created if needed
                    this.element.on('panelopen.' + this.uniqueName, () => {
                        this.onDialogOpen();
                    });
                    this.element.on('panelclose.' + this.uniqueName, () => {
                        this.onDialogClose();
                    });
                }

                TemplatedDialog.openPanel(this.element, this.uniqueName);
                this.setupPanelTitle();
            }
            else {
                if (!this.element.hasClass('ui-dialog-content'))
                    this.initDialog();

                this.element.dialog('open');
            }
        }

        public static openPanel(element: JQuery, uniqueName: string) {
            var container = $('.panels-container');
            if (!container.length)
                container = $('section.content');

            element.data('paneluniquename', uniqueName);

            if (container.length) {
                container = container.last();
                container.children()
                    .not(element)
                    .not('.panel-hidden')
                    .addClass('panel-hidden panel-hidden-' + uniqueName);

                if (element[0].parentElement !== container[0])
                    element.appendTo(container);
            }

            $('.ui-dialog:visible, .ui-widget-overlay:visible')
                .not(element)
                .addClass('panel-hidden panel-hidden-' + uniqueName);

            element
                .removeClass('hidden')
                .removeClass('panel-hidden')
                .addClass('s-Panel')
                .trigger('panelopen');
        }

        public static closePanel(element: JQuery, e?: JQueryEventObject) {
            if (!element.hasClass('s-Panel') || element.hasClass('hidden'))
                return;

            var query = $.Event(e as any);
            (query as any).type = 'panelbeforeclose';
            query.target = element[0];
            element.trigger(query);
            if (query.isDefaultPrevented())
                return;

            element.addClass('hidden');
            var uniqueName = element.data('paneluniquename') || new Date().getTime();
            var klass = 'panel-hidden-' + uniqueName;
            $('.' + klass).removeClass(klass).removeClass('panel-hidden');
            $(window).triggerHandler('resize');
            $('.require-layout:visible').triggerHandler('layout');

            var e = $.Event(e as any);
            (e as any).type = 'panelclose';
            (e as any).target = element[0];
            element.trigger(e);
        }

        protected onDialogOpen(): void {
            if (!$(document.body).hasClass('mobile-device'))
                $(':input:eq(0)', this.element).focus();
            this.arrange();
            this.tabs && this.tabs.tabs('option', 'active', 0);
        }

        protected arrange(): void {
            this.element.find('.require-layout').filter(':visible').each(function (i, e) {
                $(e).triggerHandler('layout');
            });
        }

        protected onDialogClose() {
            $(document).trigger('click');

            // for tooltips etc.
            if (($ as any).qtip) {
                $(document.body).children('.qtip').each(function (index, el) {
                    ($(el) as any).qtip('hide');
                });
            }

            window.setTimeout(() => {
                var element = this.element;
                this.destroy();
                element.remove();
                Q.positionToastContainer(false);
            }, 0);
        }

        protected addCssClass(): void {
            if (this.isMarkedAsPanel) {
                super.addCssClass();

                if (this.isResponsive)
                    this.element.addClass("flex-layout");
            }
        }

        protected getDialogOptions(): JQueryUI.DialogOptions {
            var opt: JQueryUI.DialogOptions = {};
            var dialogClass = 's-Dialog ' + this.getCssClass();
            opt.dialogClass = dialogClass;
            opt.width = 920;
            TemplatedDialog.applyCssSizes(opt, dialogClass);
            opt.autoOpen = false;
            let type = (ss as any).getInstanceType(this);
            opt.resizable = (ss as any).getAttributes(type, Serenity.ResizableAttribute, true).length > 0;
            opt.modal = true;
            opt.position = { my: 'center', at: 'center', of: $(window.window) };
            opt.title = Q.coalesce(this.element.data('dialogtitle'), this.getDialogTitle()) || '';
            return opt;
        }

        protected getDialogTitle() {
            return "";
        }

        public dialogClose(): void {
            if (this.element.hasClass('ui-dialog-content'))
                this.element.dialog().dialog('close');
            else if (this.element.hasClass('s-Panel') && !this.element.hasClass('hidden')) {
                TemplatedDialog.closePanel(this.element);
            }
        }

        public get dialogTitle(): string {
            if (this.element.hasClass('ui-dialog-content'))
                return this.element.dialog('option', 'title');

            return this.element.data('dialogtitle');
        }

        private setupPanelTitle() {
            var value = Q.coalesce(this.dialogTitle, this.getDialogTitle());
            var pt = this.element.children('.panel-titlebar');

            if (Q.isEmptyOrNull(value)) {
                pt.remove();
            }
            else {
                if (!this.element.children('.panel-titlebar').length) {
                    pt = $("<div class='panel-titlebar'><div class='panel-titlebar-text'></div></div>")
                        .prependTo(this.element);
                }
                pt.children('.panel-titlebar-text').text(value);

                if (this.element.hasClass('s-Panel')) {
                    if (!pt.children('.panel-titlebar-close').length) {
                        $('<button class="panel-titlebar-close">&nbsp;</button>')
                            .prependTo(pt)
                            .click(e => {
                                TemplatedDialog.closePanel(this.element, e);
                            });
                    }
                }
            }
        }

        public set dialogTitle(value: string) {
            var oldTitle = this.dialogTitle;
            this.element.data('dialogtitle', value);

            if (this.element.hasClass('ui-dialog-content'))
                this.element.dialog('option', 'title', value);
            else if (this.element.hasClass('s-Panel')) {
                if (oldTitle != this.dialogTitle) {
                    this.setupPanelTitle();
                    this.arrange();
                }

            }
        }

        public set_dialogTitle(value: string) {
            this.dialogTitle = value;
        }

        protected initTabs(): void {
            var tabsDiv = this.byId('Tabs');
            if (tabsDiv.length === 0) {
                return;
            }
            this.tabs = tabsDiv.tabs({});
            this.tabs.bind('tabsactivate', () => this.arrange());
        }

        protected handleResponsive(): void {
            var dlg = this.element.dialog();
            var uiDialog = this.element.closest('.ui-dialog');
            if ($(document.body).hasClass('mobile-device')) {
                var data = this.element.data('responsiveData');
                if (!data) {
                    data = {};
                    data.draggable = dlg.dialog('option', 'draggable');
                    data.resizable = dlg.dialog('option', 'resizable');
                    data.position = dlg.css('position');
                    var pos = uiDialog.position();
                    data.left = pos.left;
                    data.top = pos.top;
                    data.width = uiDialog.width();
                    data.height = uiDialog.height();
                    data.contentHeight = this.element.height();
                    this.element.data('responsiveData', data);
                    dlg.dialog('option', 'draggable', false);
                    dlg.dialog('option', 'resizable', false);
                }
                uiDialog.addClass('mobile-layout');
                uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px', position: 'fixed' });
                $(document.body).scrollTop(0);
                Q.layoutFillHeight(this.element);
            }
            else {
                var d = this.element.data('responsiveData');
                if (d) {
                    dlg.dialog('option', 'draggable', d.draggable);
                    dlg.dialog('option', 'resizable', d.resizable);
                    this.element.closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px', position: d.position });
                    this.element.height(d.contentHeight);
                    uiDialog.removeClass('mobile-layout');
                    this.element.removeData('responsiveData');
                }
            }
        }
    }
}