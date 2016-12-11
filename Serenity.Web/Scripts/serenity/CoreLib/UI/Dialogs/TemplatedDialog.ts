namespace Serenity {

    @Serenity.Decorators.registerClass([Serenity.IDialog])
    export class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {

        protected isPanel: boolean;
        protected responsive: boolean;
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;

        constructor(options?: TOptions) {
            super(Q.newBodyDiv(), options);

            this.isPanel = (ss as any).getAttributes((ss as any).getInstanceType(this),
                Serenity.PanelAttribute, true).length > 0;

            if (!this.isPanel) {
                this.initDialog();
            }

            this.initValidator();
            this.initTabs();
            this.initToolbar();
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
            !this.isPanel && this.element.dialog('destroy');
            $(window).unbind('.' + this.uniqueName);
            super.destroy();
        }

        protected initDialog(): void {
            this.element.dialog(this.getDialogOptions());
            this.element.closest('.ui-dialog').on('resize', e => this.arrange());

            let type = (ss as any).getInstanceType(this);

            this.responsive = Q.Config.responsiveDialogs ||
                (ss as any).getAttributes(type, ResponsiveAttribute, true).length > 0;

            if (this.responsive) {
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

                if (this.responsive) {
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

        public dialogOpen(): void {
            if (this.isPanel) {
                return;
            }

            this.element.dialog().dialog('open');
        }

        protected onDialogOpen(): void {
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
            let type = (ss as any).getInstanceType(this);
            if ((ss as any).getAttributes(type, PanelAttribute, true).length > 0) {
                super.addCssClass();
            }

            // will add css class to ui-dialog container, not content element
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
            return opt;
        }

        public dialogClose(): void {
            if (this.isPanel) {
                return;
            }

            this.element.dialog().dialog('close');
        }

        public get dialogTitle(): string {
            if (this.isPanel) {
                return null;
            }

            return this.element.dialog('option', 'title');
        }

        public set dialogTitle(value: string) {
            if (this.isPanel) {
                return;
            }

            this.element.dialog('option', 'title', value);
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
                uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px' });
                $(document.body).scrollTop(0);
                Q.layoutFillHeight(this.element);
            }
            else {
                var d = this.element.data('responsiveData');
                if (d) {
                    dlg.dialog('option', 'draggable', d.draggable);
                    dlg.dialog('option', 'resizable', d.resizable);
                    this.element.closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px' });
                    this.element.height(d.contentHeight);
                    uiDialog.removeClass('mobile-layout');
                    this.element.removeData('responsiveData');
                }
            }
        }
    }
}