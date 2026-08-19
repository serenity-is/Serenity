import { bindThis } from "@serenity-is/domwise";
import { Dialog, DialogButton, DialogOptions, Fluent, Validator, defaultNotifyOptions, getjQuery, nsSerenity, positionToastContainer } from "../../base";
import { isMobileView, validateOptions } from "../../compat";
import { IDialog } from "../../interfaces";
import { CloseButtonAttribute, MaximizableAttribute, PanelAttribute, ResizableAttribute, StaticPanelAttribute } from "../../types/attributes";
import { TabsExtensions } from "../helpers/tabsextensions";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { applyCssSizes, handleUIDialogResponsive } from "./basedialog-internal";
import { DialogExtensions } from "./dialogextensions";

/**
 * Base class for dialog widgets, providing dialog/modal/panel behavior,
 * validation, tabs, and toolbar integration.
 * @typeParam P - Widget props type.
 */
export class BaseDialog<P> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [IDialog]);

    static override createDefaultElement() { return document.body.appendChild(<div hidden />) as HTMLDivElement; }

    declare protected tabs: Fluent<HTMLElement>;
    declare protected toolbar: Toolbar;
    declare protected validator: any;
    declare protected dialog: Dialog;

    /**
     * Creates a base dialog widget.
     * @param props - Widget props forwarded to the base widget.
     */
    constructor(props?: WidgetProps<P>) {
        super(props);

        this.domNode.setAttribute("id", this.domNode.getAttribute("id") || this.uniqueName);
        this.initValidator();
        this.initTabs();
        this.initToolbar();
    }

    /**
     * Cleans up tabs, toolbar, validator, and dialog resources.
     */
    public override destroy(): void {
        TabsExtensions.destroy(this.tabs);
        this.tabs = null;

        if (this.toolbar) {
            this.toolbar.destroy();
            this.toolbar = null;
        }

        if (this.validator) {
            this.validator?.destroy?.();
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

    /**
     * Hook for subclasses to add CSS classes; the class goes to the dialog/modal/panel element.
     */
    protected override addCssClass(): void {
        // class goes to dialog / modal / panel element
    }

    /**
     * Returns the initial dialog title.
     * @returns The initial title text.
     */
    protected getInitialDialogTitle() {
        return "";
    }

    /**
     * Whether the dialog renders as a static panel.
     * @returns True when static.
     */
    protected isStaticPanel() {
        return this.getCustomAttribute(StaticPanelAttribute)?.value === true;
    }

    /**
     * Returns the options used to create the underlying dialog.
     * @returns Dialog options.
     */
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

    /**
     * Initializes the underlying dialog element.
     */
    protected initDialog(): void {
        this.domNode.hidden = false;

        if (this.dialog?.type == "uidialog") {
            this.initUIDialog();
            Fluent.on(this.domNode.closest(".ui-dialog"), "resize." + this.uniqueName, bindThis(this).arrange);
        }
    }

    /**
     * Initializes jQuery UI dialog-specific behavior.
     */
    protected initUIDialog(): void {
        let element = getjQuery()(this.domNode);
        DialogExtensions.dialogResizable(element);
        Fluent.on(window, "resize." + this.uniqueName, () => {
            if (element.width() > 0 && element.height() > 0)
                this.handleResponsive();
        });
        Fluent.on(this.domNode, "dialogopen." + this.uniqueName, bindThis(this).handleResponsive);

        if (this.getCustomAttribute(MaximizableAttribute)?.value) {
            DialogExtensions.dialogMaximizable(element);
        }
    }

    /**
     * Opens the dialog, optionally as a panel.
     * @param asPanel - When true, opens as a panel instead of a modal dialog.
     */
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

    /**
     * Hook invoked when the dialog opens; focuses the first input and arranges layout.
     */
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

    /**
     * Returns the toolbar buttons for this dialog.
     * @returns Tool button definitions.
     */
    protected getToolbarButtons(): ToolButton[] {
        return [];
    }

    /**
     * Initializes the toolbar from the Toolbar element.
     */
    protected initToolbar(): void {
        var toolbarDiv = this.findById('Toolbar');
        if (!toolbarDiv)
            return;

        var hotkeyContext = this.domNode.closest('.ui-dialog') ??
            this.domNode.closest('.modal') ?? this.domNode;

        this.toolbar = new Toolbar({ element: toolbarDiv, buttons: this.getToolbarButtons(), hotkeyContext }).init();
    }

    /**
     * Returns the validator options for the form.
     * @returns Validator options.
     */
    protected getValidatorOptions(): any {
        return {};
    }

    /**
     * Initializes the form validator.
     */
    protected initValidator(): void {
        var form = this.findById('Form');
        if (form instanceof HTMLFormElement) {
            var valOptions = this.getValidatorOptions();
            this.validator = new Validator(form, validateOptions(valOptions));
        }
    }

    /**
     * Resets all validation state.
     */
    protected resetValidation() {
        this.validator && (this.validator as any).resetAll();
    }

    /**
     * Validates the form.
     * @returns True when the form is valid.
     */
    protected validateForm() {
        return this.validator == null || !!this.validator.form();
    }

    /**
     * Triggers layout on all elements that require it.
     */
    public arrange(): void {
        this.domNode.querySelectorAll(".require-layout").forEach((el: HTMLElement) => {
            Fluent.isVisibleLike(el) && Fluent.trigger(el, 'layout');
        });
    }

    /**
     * Hook invoked when the dialog closes; destroys the dialog and removes its element.
     * @param result - The close result.
     */
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

    /**
     * Returns the dialog buttons for this dialog.
     * @returns Dialog button definitions.
     */
    protected getDialogButtons(): DialogButton[] {
        return [];
    }

    /**
     * Closes the dialog with the given result.
     * @param result - The close result.
     */
    public dialogClose(result?: string): void {
        this.dialog?.close(result ?? null);
    }

    /**
     * Returns the current dialog title.
     * @returns The dialog title.
     */
    public get dialogTitle(): string {
        return this.dialog?.title() ?? this.domNode.dataset.dialogtitle;
    }

    /** Sets the dialog title. */
    public set dialogTitle(value: string) {
        this.domNode.dataset.dialogtitle = value;
        this.dialog?.title(value ?? '');
    }

    /**
     * Initializes the tabs from the Tabs element.
     */
    protected initTabs(): void {
        var tabsDiv = this.findById('Tabs');
        if (!tabsDiv)
            return;
        this.tabs = TabsExtensions.initialize(tabsDiv, bindThis(this).arrange);
    }

    /**
     * Handles responsive layout for the dialog.
     */
    protected handleResponsive(): void {
        handleUIDialogResponsive(this.domNode);
    }
}

/** @deprecated use BaseDialog */
export const TemplatedDialog = BaseDialog;