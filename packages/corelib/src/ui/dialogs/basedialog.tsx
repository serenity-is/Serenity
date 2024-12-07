import { Dialog, DialogButton, DialogOptions, Fluent, Validator, defaultNotifyOptions, getjQuery, positionToastContainer } from "../../base";
import { IDialog } from "../../interfaces";
import { isMobileView, validateOptions } from "../../compat";
import { CloseButtonAttribute, MaximizableAttribute, PanelAttribute, ResizableAttribute, StaticPanelAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { TabsExtensions } from "../helpers/tabsextensions";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { applyCssSizes, handleUIDialogResponsive } from "./basedialog-internal";
import { DialogExtensions } from "./dialogextensions";

@Decorators.registerClass('Serenity.BaseDialog', [IDialog])
export class BaseDialog<P> extends Widget<P> {

    static override createDefaultElement() { return document.body.appendChild(<div class="hidden" />) as HTMLDivElement; }

    declare protected tabs: Fluent<HTMLElement>;
    declare protected toolbar: Toolbar;
    declare protected validator: any;
    declare protected dialog: Dialog;

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

        if (this.toolbar) {
            this.toolbar.destroy();
            this.toolbar = null;
        }

        if (this.validator) {
            this.byId('Form').remove();
            this.validator = null;
        }

        const dialog = this.dialog;
        if (dialog) {
            Fluent.off(this.domNode, "." + this.uniqueName);
            this.dialog = null;
            dialog.dispose();
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
            providerOptions: (type) => {
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

    /** Attaches a dialog/modal/panel close event handler. See Dialog.close for more info. */
    onClose(handler: (result?: string, e?: Event) => void, opt?: { before?: boolean, oneOff?: boolean }) {
        Dialog.onClose(this.element, handler, opt);
    }

    /** Attaches a dialog/modal/panel open event handler. See Dialog.open for more info. */
    onOpen(handler: (e?: Event) => void, opt?: { before?: boolean, oneOff?: boolean }) {
        Dialog.onOpen(this.element, handler, opt);
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

        this.toolbar = new Toolbar({ element: toolbarDiv, buttons: this.getToolbarButtons(), hotkeyContext }).init();
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
            let domNode = this.domNode;
            this.destroy();
            if (domNode) {
                Fluent.remove(domNode);
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

/** @deprecated use BaseDialog */
export const TemplatedDialog = BaseDialog;