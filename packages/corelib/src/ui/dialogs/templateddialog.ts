import { Dialog, DialogButton, DialogOptions, Fluent, Validator, addClass, defaultNotifyOptions, getjQuery, positionToastContainer } from "../../base";
import { IDialog } from "../../interfaces";
import { isMobileView, layoutFillHeight, validateOptions } from "../../q";
import { CloseButtonAttribute, MaximizableAttribute, PanelAttribute, ResizableAttribute, StaticPanelAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { TabsExtensions } from "../helpers/tabsextensions";
import { TemplatedWidget } from "../widgets/templatedwidget";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { WidgetProps } from "../widgets/widget";
import { DialogExtensions } from "./dialogextensions";

@Decorators.registerClass('Serenity.TemplatedDialog', [IDialog])
export class TemplatedDialog<P> extends TemplatedWidget<P> {

    static override createDefaultElement() { return Fluent("div").class("hidden").appendTo(document.body).getNode(); }

    protected tabs: Fluent<HTMLElement>;
    protected toolbar: Toolbar;
    protected validator: any;
    protected dialog: Dialog;

    constructor(props?: WidgetProps<P>) {
        super(props);

        this.domNode.setAttribute("id", this.domNode.getAttribute("id") || this.uniqueName);
        this.initValidator();
        this.initTabs();
        this.initToolbar();
    }

    public destroy(): void {
        TabsExtensions.destroy(this.tabs);
        this.tabs = null;
        (this.toolbar as any)?.toolbar?.destroy?.();
        this.toolbar = null;
        this.validator && this.byId('Form').remove();
        this.validator = null;
        if (this.dialog) {
            this.dialog.dispose();
            this.dialog = null;
        }
        Fluent.off(window, '.' + this.uniqueName);
        super.destroy();
    }

    protected addCssClass(): void {
        // class goes to dialog / modal / panel element
    }

    protected getInitialDialogTitle() {
        return "";
    }
    
    protected isStaticPanel() {
        return this.getCustomAttribute(StaticPanelAttribute)?.value === true;
    }

    protected getDialogOptions(): DialogOptions {
        return {
            preferPanel: this.isStaticPanel() ? true : this.getCustomAttribute(PanelAttribute)?.value,
            autoOpen: false,
            buttons: this.getDialogButtons(),
            closeButton: this.isStaticPanel() ? false : this.getCustomAttribute(CloseButtonAttribute)?.value,
            dialogClass: (this.getCssClass() ?? "") + " flex-layout",
            element: this.domNode,
            size: "lg",
            onClose: (result) => {
                this.onDialogClose(result)
            },
            onOpen: () => {
                this.onDialogOpen()
            },
            width: Math.min(window.innerWidth, 920),
            providerOptions: (type, options) => {
                if (type === "uidialog") {
                    var opt: any = {};
                    applyCssSizes(opt, this.getCssClass());
                    opt.resizable = this.getCustomAttribute(ResizableAttribute)?.value;
                    return opt;
                }
            },
            title: this.dialogTitle ?? this.getInitialDialogTitle() ?? ''
        }
    }

    protected initDialog(): void {
        this.domNode.classList.remove("hidden");

        if (this.dialog?.type == "uidialog") {
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

        if (this.getCustomAttribute(MaximizableAttribute)?.value) {
            DialogExtensions.dialogMaximizable(element);
        }
    }

    public dialogOpen(asPanel?: boolean): void {
        if (!this.dialog) {
            let opt = this.getDialogOptions();
            if (asPanel != null)
                opt.preferPanel = asPanel;
            this.dialog = new Dialog(opt);
            this.initDialog();
        }
        this.dialog.open();
    }

    protected onDialogOpen(): void {
        if (!isMobileView())
            this.element.findFirst('input:not([type=hidden]), textarea, select').focus();
        this.arrange();
        TabsExtensions.selectTab(this.tabs, 0);
    }

    protected getToolbarButtons(): ToolButton[] {
        return [];
    }

    protected initToolbar(): void {
        var toolbarDiv = this.findById('Toolbar');
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
        var form = this.findById('Form');
        if (form instanceof HTMLFormElement) {
            var valOptions = this.getValidatorOptions();
            this.validator = new Validator(form, validateOptions(valOptions));
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
            Fluent.isVisibleLike(el) && Fluent.trigger(el, 'layout');
        });
    }

    protected onDialogClose(result?: string) {
        document.dispatchEvent(new Event('click'));

        window.setTimeout(() => {
            let domNode = this.domNode
            this.destroy();
            if (domNode) {
                this.element.remove();
            }
            positionToastContainer(defaultNotifyOptions, false);
        }, 0);
    }

    protected getDialogButtons(): DialogButton[] {
        return [];
    }

    public dialogClose(result?: string): void {
        this.dialog?.close(result ?? null);
    }

    public get dialogTitle(): string {
        return this.dialog?.title() ?? this.domNode.dataset.dialogtitle;
    }

    public set dialogTitle(value: string) {
        this.domNode.dataset.dialogtitle = value;
        this.dialog?.title(value ?? '');
    }

    protected initTabs(): void {
        var tabsDiv = this.findById('Tabs');
        if (!tabsDiv)
            return;
        this.tabs = TabsExtensions.initialize(tabsDiv, this.arrange.bind(this));
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
        dialog.style.display = "none";
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