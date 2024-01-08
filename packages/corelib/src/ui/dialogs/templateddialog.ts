import { CommonDialogOptions, Config, DialogButton, Fluent, ICommonDialog, addClass, createCommonDialog, defaultNotifyOptions, getInstanceType, getjQuery, positionToastContainer } from "@serenity-is/base";
import { Decorators, MaximizableAttribute, PanelAttribute, ResizableAttribute } from "../../decorators";
import { IDialog } from "../../interfaces";
import { getAttributes, isMobileView, layoutFillHeight, validateOptions } from "../../q";
import { TemplatedWidget } from "../widgets/templatedwidget";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { WidgetProps } from "../widgets/widget";
import { DialogExtensions } from "./dialogextensions";

@Decorators.registerClass('Serenity.TemplatedDialog', [IDialog])
export class TemplatedDialog<P> extends TemplatedWidget<P> {

    protected tabs: any;
    protected toolbar: Toolbar;
    protected validator: any;
    protected commonDialog: ICommonDialog;

    constructor(props?: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add("s-TemplatedDialog");
        this.domNode.setAttribute("id", this.domNode.getAttribute("id") || this.uniqueName);
        this.initValidator();
        this.initTabs();
        this.initToolbar();
    }

    static override createDefaultElement() {
        var div = document.body.appendChild(document.createElement("div"));
        div.classList.add("hidden");
        return div;
    }

    private get isMarkedAsPanel(): boolean {
        var panelAttr = getAttributes(getInstanceType(this),
            PanelAttribute, true) as PanelAttribute[];
        return panelAttr.length > 0 && panelAttr[panelAttr.length - 1].value !== false;
    }

    public destroy(): void {
        (this.tabs as any)?.tabs?.('destroy');
        this.tabs = null;
        (this.toolbar as any)?.toolbar?.destroy?.();
        this.toolbar = null;
        this.validator && this.byId('Form').remove();
        this.validator = null;
        if (this.commonDialog) {
            this.commonDialog.dispose();
            this.commonDialog = null;
        }
        Fluent.off(window, '.' + this.uniqueName);
        super.destroy();
    }

    protected addCssClass(): void {
        // class goes to dialog / modal / panel element
    }

    protected getDialogTitle() {
        return "";
    }

    protected getCommonDialogOptions(asPanel?: boolean): CommonDialogOptions {
        return {
            asPanel: asPanel ?? this.isMarkedAsPanel,
            autoOpen: false,
            bootstrap: this.preferBSModal(),
            buttons: this.getDialogButtons(),
            dialogClass: (this.getCssClass() ?? "") + " flex-layout",
            element: this.domNode,
            modalClass: "modal-lg",
            onClose: (result) => {
                this.onDialogClose(result)
            },
            onOpen: () => {
                this.onDialogOpen()
            },
            providerOptions: (type) => {
                if (type?.startsWith("bs"))
                    return this.getModalOptions();

                if (type === "jqueryui")
                    return this.getDialogOptions();
            },
            title: this.domNode.getAttribute('data-dialogtitle') ?? this.getDialogTitle() ?? ''
        }
    }

    /** This one returns jQuery UI Dialog specific options */
    protected getDialogOptions(): any {
        var opt: any = {};
        opt.width = 920;
        applyCssSizes(opt, this.getCssClass());
        let type = getInstanceType(this);
        opt.resizable = getAttributes(type, ResizableAttribute, true).length > 0;
        opt.modal = true;
        opt.position = { my: 'center', at: 'center', of: window.window };
        opt.title = this.domNode.dataset.dialogtitle ?? this.getDialogTitle() ?? '';
        return opt;
    }

    protected preferBSModal() {
        return Config.bootstrapDialogs;
    }

    protected getModalOptions(): any {
        return {
            backdrop: false,
            keyboard: false
        }
    }

    protected initCommonDialog(): void {
        this.domNode.classList.remove("hidden");

        if (this.commonDialog?.type == "jqueryui") {
            this.initUIDialog();
            Fluent.on(this.domNode.closest(".ui-dialog"), "resize." + this.uniqueName, this.arrange.bind(this));
        }
    }

    protected initUIDialog(): void {
        let element = getjQuery()(this.domNode);
        DialogExtensions.dialogResizable(element);
        Fluent.on(window, "resize." + this.uniqueName, () => {
            if (element.width() > 0 && element.height() > 0)
                this.handleResponsive();
        });
        Fluent.on(this.domNode, "dialogopen." + this.uniqueName, this.handleResponsive.bind(this));

        if (getAttributes(getInstanceType(this), MaximizableAttribute, true).length > 0) {
            DialogExtensions.dialogMaximizable(element);
        }
    }

    public dialogOpen(asPanel?: boolean): void {
        if (!this.commonDialog) {
            this.commonDialog = createCommonDialog(this.getCommonDialogOptions(asPanel));
            this.initCommonDialog();
        }
        this.commonDialog.open();
    }

    protected onDialogOpen(): void {
        if (!isMobileView())
            (this.domNode.querySelector('input:not([type=hidden]), textarea, select') as HTMLElement)?.focus();
        this.arrange();
        this.tabs && (this.tabs as any).tabs?.('option', 'active', 0);
    }

    protected getToolbarButtons(): ToolButton[] {
        return [];
    }

    protected initToolbar(): void {
        var toolbarDiv = this.byId('Toolbar');
        if (!toolbarDiv)
            return;

        var hotkeyContext = this.domNode.closest('.ui-dialog') ??
            this.domNode.closest('.modal') ?? this.domNode;

        this.toolbar = new Toolbar({ element: toolbarDiv, buttons: this.getToolbarButtons(), hotkeyContext });
    }

    protected getValidatorOptions(): any {
        return {};
    }

    protected initValidator(): void {
        var form = this.byId('Form');
        if (form) {
            let $ = getjQuery();
            if ($?.fn?.validate) {
                var valOptions = this.getValidatorOptions();
                this.validator = $(form).validate(validateOptions(valOptions));
            }
        }
    }

    protected resetValidation() {
        this.validator && (this.validator as any).resetAll();
    }

    protected validateForm() {
        return this.validator == null || !!this.validator.form();
    }

    public arrange(): void {
        this.domNode.querySelectorAll(".require-layout").forEach((el: HTMLElement) => {
            if (el.offsetWidth > 0 && el.offsetHeight > 0)
                Fluent.trigger(el, 'layout', { bubbles: false });
        });
    }

    protected onDialogClose(result?: string) {
        document.dispatchEvent(new Event('click'));

        window.setTimeout(() => {
            this.destroy();
            if (this.domNode) {
                let $ = getjQuery();
                $ ? $(this.domNode).remove() : this.domNode.remove();
            }
            positionToastContainer(defaultNotifyOptions, false);
        }, 0);
    }

    protected getDialogButtons(): DialogButton[] {
        return [];
    }

    public dialogClose(): void {
        this.commonDialog?.close();
    }

    public get dialogTitle(): string {
        return this.commonDialog?.title ?? this.domNode.dataset.dialogtitle;
    }

    public set dialogTitle(value: string) {
        this.domNode.dataset.dialogtitle = value;
        this.commonDialog && (this.commonDialog.title = value ?? '');
    }

    protected initTabs(): void {
        var tabsDiv = this.byId('Tabs');
        if (!tabsDiv)
            return;
        let $ = getjQuery();
        if (!$)
            return;
        this.tabs = $(tabsDiv).tabs?.({});
        this.tabs.on('tabsactivate', () => this.arrange());
    }

    protected handleResponsive(): void {
        handleUIDialogResponsive(this.domNode);
    }
}

function getCssSize(element: HTMLElement, name: string): number {
    var cssSize = getComputedStyle(element).getPropertyValue(name);
    if (cssSize == null || !cssSize.endsWith('px'))
        return null;

    cssSize = cssSize.substring(0, cssSize.length - 2);
    let i = parseInt(cssSize, 10);
    if (i == null || isNaN(i) || i == 0)
        return null;

    return i;
}

function applyCssSizes(opt: any, dialogClass: string) {
    let size: number;
    let dialog = document.createElement("div");
    try {
        dialog.style.visibility = "hidden !important";
        addClass(dialog, dialogClass);
        document.body.append(dialog);

        var sizeHelper = document.createElement("div");
        sizeHelper.classList.add("size");
        dialog.append(sizeHelper);
        size = getCssSize(sizeHelper, 'minWidth');
        if (size != null)
            opt.minWidth = size;

        size = getCssSize(sizeHelper, 'width');
        if (size != null)
            opt.width = size;

        size = getCssSize(sizeHelper, 'height');
        if (size != null)
            opt.height = size;

        size = getCssSize(sizeHelper, 'minHeight');
        if (size != null)
            opt.minHeight = size;
    }
    finally {
        dialog.remove();
    }
};

function handleUIDialogResponsive(domNode: HTMLElement) {
    let $ = getjQuery();
    if (!$)
        return;

    var dlg = ($(domNode) as any)?.dialog();
    var uiDialog = $(domNode).closest('.ui-dialog');
    if (!uiDialog.length)
        return;

    if (isMobileView()) {
        var data = $(domNode).data('responsiveData');
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
            data.contentHeight = $(domNode).height();
            $(domNode).data('responsiveData', data);
            dlg.dialog('option', 'draggable', false);
            dlg.dialog('option', 'resizable', false);
        }
        uiDialog.addClass('mobile-layout');
        uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px', position: 'fixed' });
        $(document.body).scrollTop(0);
        layoutFillHeight(domNode);
    }
    else {
        var d = $(domNode).data('responsiveData');
        if (d) {
            dlg.dialog('option', 'draggable', d.draggable);
            dlg.dialog('option', 'resizable', d.resizable);
            $(domNode).closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px', position: d.position });
            $(domNode).height(d.contentHeight);
            uiDialog.removeClass('mobile-layout');
            $(domNode).removeData('responsiveData');
        }
    }
}