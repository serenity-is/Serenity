import { bindThis } from "@serenity-is/domwise";
import { getjQuery, isBS3, isBS5Plus } from "./environment";
import { Fluent } from "./fluent";
import { htmlEncode, sanitizeHtml, type RenderableContent } from "./html";
import { iconClassName, type IconClassName } from "./icons";
import { localText } from "./localtext";
import { isArrayLike, isPromiseLike, omitUndefined } from "./system";

/**
 * Options that describe a single button rendered in a {@link Dialog} footer.
 * @remarks
 * Buttons are rendered as Bootstrap `btn` or jQuery UI button elements depending
 * on the active dialog provider. When {@link DialogButton.result} is set and the
 * click handler does not cancel the event, the dialog automatically closes with
 * that result code.
 */
export interface DialogButton {
    /** Visible caption rendered inside the button. Defaults to a localized value when created via helper factories. */
    text?: string;
    /** Tooltip / `title` attribute shown on hover. */
    hint?: string;
    /** Optional icon displayed before the text; resolved via {@link iconClassName}. */
    icon?: IconClassName;
    /**
     * Click handler invoked when the button is activated.
     * @param e - The originating mouse event.
     * @returns `false` to prevent the automatic close, or a `Promise` that resolves to `false` to cancel asynchronously.
     */
    click?: (e: MouseEvent) => void | false | Promise<void | false>;
    /** Additional CSS class(es) added to the button element (e.g. `"btn-primary"`, `"btn-danger"`). */
    cssClass?: string;
    /**
     * Result code assigned to the dialog when this button is clicked.
     * The value is stored in `dataset.dialogResult` and passed to `onClose` handlers.
     * If set and the click handler does not call `preventDefault()` / return `false`, the dialog closes automatically.
     */
    result?: string;
}

/**
 * Identifies the underlying UI provider that backs a {@link Dialog} instance.
 * - `"bsmodal"` — Bootstrap modal (`.modal`).
 * - `"uidialog"` — jQuery UI dialog (`.ui-dialog`).
 * - `"panel"` — Inline Serenity panel (`.s-Panel`).
 */
export type DialogProviderType = "bsmodal" | "uidialog" | "panel";

/**
 * Options that configure a {@link Dialog} instance across all providers.
 * @remarks
 * The dialog provider is chosen automatically from {@link DialogOptions.preferPanel},
 * {@link DialogOptions.preferBSModal}, and feature detection (`hasBSModal()` / `hasUIDialog()`).
 * Provider-specific options can be injected via {@link DialogOptions.providerOptions}.
 */
export interface DialogOptions {
    /** When `true`, {@link Dialog.dispose} is called automatically on close. @defaultValue `true` */
    autoDispose?: boolean;
    /** When `true`, the dialog opens immediately after construction. @defaultValue `true` */
    autoOpen?: boolean;
    /** Backdrop behavior for Bootstrap modals; `"static"` prevents closing on outside click. @defaultValue `false` */
    backdrop?: boolean | "static"
    /** Buttons rendered in the dialog footer. */
    buttons?: DialogButton[];
    /** Vertically centers a Bootstrap modal via `modal-dialog-centered`. @defaultValue `true` */
    centered?: boolean;
    /** Whether to render the header close (`×` / `btn-close`) button. @defaultValue `true` */
    closeButton?: boolean;
    /** Whether pressing <kbd>Escape</kbd> closes the dialog. Message dialogs default to `true`. */
    closeOnEscape?: boolean;
    /** Extra CSS class(es) added to the root dialog element (`.modal`, `.ui-dialog`, or `.s-Panel`). */
    dialogClass?: string;
    /** Body element or a callback that populates the freshly created body element. Array-like values are treated as the content node. */
    element?: HTMLElement | ArrayLike<HTMLElement> | ((element: HTMLElement) => void);
    /** Enables fade animation for Bootstrap modals. @defaultValue `false` for message dialogs, `true` otherwise */
    fade?: boolean;
    /** Applies a `modal-fullscreen[-{breakpoint}-down]` class. Only effective for Bootstrap modals. */
    fullScreen?: boolean | "sm-down" | "md-down" | "lg-down" | "xl-down" | "xxl-down",
    /** jQuery UI `modal` flag. Retained for backward compatibility; does not affect Bootstrap modals. */
    modal?: boolean;
    /** Callback invoked after the dialog is opened. */
    onOpen?: (e?: Event) => void;
    /** Callback invoked after the dialog is closed, receiving the result code. */
    onClose?: (result: string, e?: Event) => void;
    /** When both providers are available, prefer Bootstrap modal over jQuery UI dialog. @defaultValue `true` */
    preferBSModal?: boolean;
    /** Force inline panel mode even when modal / jQuery UI providers are available. */
    preferPanel?: boolean;
    /** Returns provider-specific options merged into the underlying call (Bootstrap modal options or jQuery UI dialog options). @param type - Resolved provider type. @param opt - The resolved dialog options. @returns Provider-specific options object. */
    providerOptions?: (type: DialogProviderType, opt: DialogOptions) => any;
    /** Makes the Bootstrap modal body scrollable via `modal-dialog-scrollable`. */
    scrollable?: boolean;
    /** Bootstrap modal size. @defaultValue `"lg"` for regular dialogs, `"md"` for message dialogs */
    size?: "sm" | "md" | "lg" | "xl";
    /** Title text shown in the dialog header. */
    title?: string;
    /** Initial width in pixels; only used by the jQuery UI dialog provider. */
    width?: number;
}

/**
 * Unified wrapper over jQuery UI dialogs, Bootstrap modals, and Serenity inline panels.
 * @remarks
 * Provider selection is automatic: `preferPanel` wins, otherwise jQuery UI vs. Bootstrap
 * is chosen via {@link hasUIDialog}, {@link hasBSModal}, and `preferBSModal`.
 * Lifecycle events (`panel*`, `dialog*`, `show.bs.modal` / `hide.bs.modal`) are normalized
 * so that {@link Dialog.onOpen} / {@link Dialog.onClose} work uniformly across providers.
 * @example
 * ```ts
 * const dlg = new Dialog({
 *   title: "Hello",
 *   element: el => el.append("Content"),
 *   buttons: [okDialogButton(), cancelDialogButton()]
 * });
 * dlg.onClose(result => console.log(result));
 * ```
 */
export class Dialog {

    declare private el: HTMLElement;
    declare private dialogResult: string;

    /**
     * Creates a new dialog.
     * @param opt - Configuration for the dialog; merged over {@link Dialog.defaults}.
     * @param create - When `false`, skips DOM creation and only binds to an existing element. Used internally by {@link Dialog.getInstance}.
     * @remarks
     * The concrete provider (panel / jQuery UI / Bootstrap) is resolved from availability
     * and `preferPanel` / `preferBSModal` flags.
     */
    constructor(opt?: DialogOptions);
    constructor(opt?: DialogOptions, create = true) {

        if (isArrayLike(opt?.element))
            this.el = opt.element[0];
        else if (typeof opt?.element !== "function")
            this.el = opt?.element;
        this.dialogResult = this.el?.dataset.dialogResult;

        if (!create) {
            return;
        }

        this.el ??= document.createElement("div");
        opt = Object.assign({}, Dialog.defaults, omitUndefined(opt));
        if (opt.closeOnEscape === void 0 && opt.closeButton)
            opt.closeOnEscape = true;
        if (typeof opt.element === "function")
            opt.element(this.el);

        if (this.el.hidden &&
            typeof opt.element !== "function")
            this.el.hidden = false;

        if (opt.preferPanel || (!hasBSModal() && !hasUIDialog()))
            this.createPanel(opt);
        else if (hasUIDialog() && (!hasBSModal() || !opt.preferBSModal))
            this.createUIDialog(opt);
        else {
            this.createBSModal(opt);
        }

        if (opt.onOpen)
            this.onOpen(opt.onOpen);

        if (opt.onClose)
            this.onClose(opt.onClose);

        if (opt.autoDispose)
            this.onClose(() => setTimeout(bindThis(this).dispose, 0));

        if (opt.title !== void 0) {
            this.title(opt.title);
        }

        if (opt.autoOpen)
            this.open();
    }

    /** Default options applied to every {@link Dialog} before caller-supplied `opt` is merged. */
    static defaults: DialogOptions = {
        autoDispose: true,
        autoOpen: true,
        backdrop: false,
        centered: true,
        closeButton: true,
        fade: false,
        fullScreen: "md-down",
        modal: true,
        preferBSModal: true,
        size: "lg"
    }

    /** Default options applied to helper message dialogs (`alertDialog`, `confirmDialog`, etc.). */
    static messageDefaults: MessageDialogOptions = {
        autoDispose: true,
        autoOpen: true,
        backdrop: false,
        centered: true,
        closeButton: true,
        closeOnEscape: true,
        fade: true,
        fullScreen: null,
        htmlEncode: true,
        modal: true,
        preferBSModal: true,
        preWrap: true,
        size: "md"
    }

    /**
     * Gets the dialog instance for the specified element.
     * @param el The dialog body element (.s-Panel, .ui-dialog-content, or .modal-body) or the root element (.modal, .ui-dialog, .s-Panel)
     * @returns The dialog instance, or null if the element is not a dialog.
     */
    static getInstance(el: HTMLElement | ArrayLike<HTMLElement>): Dialog {
        el = getDialogContentNode(el);
        if (!el)
            return null;
        return new (Dialog as any)({ element: el }, false);
    }

    /**
     * Result code of the last button that closed the dialog.
     * @remarks Mirrors `element.dataset.dialogResult`; survives disposal via the fallback field.
     * @returns The result string (e.g. `"ok"`, `"yes"`, `"cancel"`) or `null`/`undefined` when not yet closed.
     */
    get result(): string {
        return this.el ? this.el.dataset.dialogResult : this.dialogResult;
    }

    /** Closes the dialog and reports a `null` result. @returns The dialog instance for chaining. */
    close(): this;
    /**
     * Closes the dialog with an explicit result code.
     * @param result - Value stored in `dataset.dialogResult` and passed to `onClose` handlers.
     * @returns The dialog instance for chaining.
     */
    close(result: string): this;
    close(result?: string): this {
        this.el && (this.el.dataset.dialogResult = result ?? null);
        this.dialogResult = result ?? null;

        const target = getDialogEventsNode(this.el);
        if (!target)
            return;

        if (target.classList.contains("panel-body"))
            closePanel(this.el);
        else if (target.classList.contains("ui-dialog-content"))
            getjQuery()?.(this.el).dialog?.("close");
        else if (target.classList.contains("modal")) {
            if (isBS5Plus()) {
                bootstrap?.Modal?.getInstance?.(target)?.hide?.();
            } else {
                let $ = getjQuery();
                if ($?.fn?.modal)
                    $(target).modal?.("close");
            }
        }

        return this;
    }

    /**
     * Subscribes to the dialog close event.
     * @param handler - Callback invoked with the dialog result and the close event. Call `preventDefault()` on the event to cancel closing when `opt.before` is `true`.
     * @param opt - Subscription options.
     * @param opt.before - When `true`, listens to the cancellable *before-close* event (`panelbeforeclose` / `dialogbeforeclose` / `hide.bs.modal`).
     * @param opt.oneOff - When `true`, the handler is removed after the first invocation. Defaults to `true` unless `before` is `true`.
     * @returns The dialog instance for chaining.
     */

    onClose(handler: (result?: string, e?: Event) => void, opt?: { before?: boolean, oneOff?: boolean }): this {
        const target = getDialogEventsNode(this.el);
        if (!target)
            return;
        const before = opt?.before ?? false;
        const onOrOne = (opt?.oneOff ?? !opt?.before) ? Fluent.one : Fluent.on;
        if (target.classList.contains("panel-body"))
            onOrOne(target, before ? "panelbeforeclose" : "panelclose", e => handler(this.result, e));
        else if (target.classList.contains("ui-dialog-content"))
            onOrOne(target, before ? "dialogbeforeclose" : "dialogclose", e => handler(this.result, e));
        else if (target.classList.contains("modal"))
            onOrOne(target, before ? "hide.bs.modal" : "hidden.bs.modal", e => handler(this.result, e));

        return this;
    }

    /**
     * Static helper that subscribes to the close event for a dialog element that may not yet be instantiated.
     * @param el - Dialog body element (`.s-Panel`, `.ui-dialog-content`, or `.modal-body`) or an array-like wrapper.
     * @param handler - Callback invoked with the dialog result and the close event; `preventDefault()` cancels the close when `opt.before` is `true`.
     * @param opt - Subscription options.
     * @param opt.before - Listen to the cancellable *before-close* event.
     * @param opt.oneOff - Auto-remove after first invocation. Defaults to `true` unless `before` is `true`.
     */
    static onClose(el: HTMLElement | ArrayLike<HTMLElement>, handler: (result?: string, e?: Event) => void, opt?: { before?: boolean, oneOff?: boolean }) {
        const instance = Dialog.getInstance(el);
        if (instance) {
            instance.onClose(handler, opt);
            return;
        }

        const target = isArrayLike(el) ? el[0] : el;
        if (target) {
            const before = opt?.before ?? false;
            const events = [before ? "panelbeforeclose" : "panelclose", before ? "dialogbeforeclose" : "dialogclose", before ? "modalbeforeclose" : "modalclose"];
            const wrapper = (e: Event) => {
                handler(Dialog.getInstance(el)?.result, e);
                if (opt?.oneOff ?? !before) {
                    events.forEach(type => Fluent.off(target, type, wrapper));
                }
            }
            // don't know which mode the dialog will be opened, so we need to listen to all events
            events.forEach(type => Fluent.on(target, type, wrapper));
        }
    }

    /**
     * Subscribes to the dialog open event.
     * @param handler - Callback invoked when the dialog is opened; `preventDefault()` cancels the open when `opt.before` is `true`.
     * @param opt - Subscription options.
     * @param opt.before - When `true`, listens to the cancellable *before-open* event (`panelbeforeopen` / `dialogbeforeopen` / `show.bs.modal`).
     * @param opt.oneOff - Auto-remove after first invocation. Defaults to `true` unless `before` is `true`.
     * @returns The dialog instance for chaining.
     */
    onOpen(handler: (e?: Event) => void, opt?: { before?: boolean, oneOff?: boolean }): this {
        const target = getDialogEventsNode(this.el);
        if (!target)
            return;
        const before = opt?.before ?? false;
        const onOrOne = (opt?.oneOff ?? !opt?.before) ? Fluent.one : Fluent.on;
        if (target.classList.contains("panel-body"))
            onOrOne(target, before ? "panelbeforeopen" : "panelopen", handler);
        else if (target.classList.contains("ui-dialog-content"))
            onOrOne(target, before ? "dialogbeforeopen" : "dialogopen", handler);
        else if (target.classList.contains("modal"))
            onOrOne(target, before ? "show.bs.modal" : "shown.bs.modal", handler);
        return this;
    }

    /**
     * Static helper that subscribes to the open event for a dialog element that may not yet be instantiated.
     * @param el - Dialog body element (`.s-Panel`, `.ui-dialog-content`, or `.modal-body`) or an array-like wrapper.
     * @param handler - Callback invoked when the dialog is opened; `preventDefault()` cancels the open when `opt.before` is `true`.
     * @param opt - Subscription options.
     * @param opt.before - Listen to the cancellable *before-open* event.
     * @param opt.oneOff - Auto-remove after first invocation. Defaults to `true` unless `before` is `true`.
     */
    static onOpen(el: HTMLElement | ArrayLike<HTMLElement>, handler: (e?: Event) => void, opt?: { before?: boolean, oneOff?: boolean }) {
        const instance = Dialog.getInstance(el);
        if (instance) {
            instance.onOpen(handler, opt);
            return;
        }

        const target = isArrayLike(el) ? el[0] : el;
        if (target) {
            const before = opt?.before ?? false;
            const events = [before ? "panelbeforeopen" : "panelopen", before ? "dialogbeforeopen" : "dialogopen", before ? "modalbeforeopen" : "modalopen"];
            const wrapper = (e: Event) => {
                handler(e);
                if (opt?.oneOff ?? !before) {
                    events.forEach(type => Fluent.off(target, type, wrapper));
                }
            }
            // don't know which mode the dialog will be opened, so we need to listen to all events
            events.forEach(type => Fluent.on(target, type, wrapper));
        }
    }

    /**
     * Opens the dialog.
     * @remarks Dispatches the provider-specific show command (`openPanel`, `jQuery.dialog("open")`, or `bootstrap.Modal.show`).
     * @returns The dialog instance for chaining.
     */
    open() {
        const target = getDialogEventsNode(this.el);
        if (!target)
            return;
        if (target.classList.contains("panel-body"))
            openPanel(this.el);
        else if (target.classList.contains("ui-dialog-content"))
            getjQuery()?.(target).dialog("open");
        else if (target.classList.contains("modal")) {
            if (isBS5Plus()) {
                bootstrap?.Modal?.getInstance?.(target)?.show?.();
            } else {
                let $ = getjQuery();
                if ($?.fn?.modal)
                    $(target).modal?.("show");
            }
        }

        return this;
    }

    /**
     * Gets the current title text of the dialog.
     * @returns The header title text, or `undefined` when the dialog has no header.
     */
    title(): string;
    /**
     * Sets the title text of the dialog.
     * @param value - New title text to display in the header.
     * @returns The dialog instance for chaining.
     */
    title(value: string): this;
    title(value?: string): string | this {
        let title = this.getHeaderNode()?.querySelector(".modal-title, .panel-titlebar-text, .ui-dialog-title");
        if (value === void 0 && !arguments.length)
            return title?.textContent;

        title && (title.textContent = value);
        return this;
    }

    /**
     * Identifies the backing provider for this instance.
     * @returns `"bsmodal"`, `"uidialog"`, or `"panel"`, or `null` when the element is not attached or the dialog was disposed.
     */
    get type(): DialogProviderType {
        const root = getDialogNode(this.el);
        if (!root)
            return null;
        if (root.classList.contains("modal"))
            return "bsmodal";
        if (root.classList.contains("ui-dialog"))
            return "uidialog";
        if (root.classList.contains("s-Panel"))
            return "panel";
        return null;
    }

    /**
     * Gets the body / content node of the dialog (`.modal-body`, `.panel-body`, or `.ui-dialog-content`).
     * @returns The content element, or the raw `el` supplied at construction.
     */
    getContentNode(): HTMLElement {
        return this.el;
    }

    /**
     * Gets the root dialog element (`.modal`, `.ui-dialog`, or `.s-Panel`).
     * @returns The root element, or `null` when not found.
     */
    getDialogNode(): HTMLElement {
        return getDialogNode(this.el);
    }

    /**
     * Gets the node that receives open/close lifecycle events (`.modal`, `.panel-body`, or `.ui-dialog-content`).
     * @returns The events node, or a fallback lookup via the root element.
     */
    getEventsNode(): HTMLElement {
        return getDialogEventsNode(this.el);
    }

    /**
     * Gets the footer element (`.modal-footer`, `.panel-footer`, or `.ui-dialog-footer`), if present.
     * @returns The footer element or `null` when there is none.
     */
    getFooterNode(): HTMLElement {
        return this.getDialogNode()?.querySelector(".modal-footer, .panel-footer, .ui-dialog-footer");
    }

    /**
     * Gets the header element (`.modal-header`, `.panel-titlebar`, or `.ui-dialog-titlebar`), if present.
     * @returns The header element or `null` when there is none.
     */
    getHeaderNode(): HTMLElement {
        return this.getDialogNode()?.querySelector(".modal-header, .panel-titlebar, .ui-dialog-titlebar");
    }

    private onButtonClick(e: MouseEvent, btn: DialogButton) {
        e ??= new Event("click") as MouseEvent;
        if (!btn.click) {
            if (btn.result)
                this.close(btn.result);
            return;
        }

        const value = btn.click(e);
        if (!btn.result)
            return;

        if (isPromiseLike(value))
            value.then(value => value !== false && !Fluent.isDefaultPrevented(e) && this.close(btn.result));
        else if (value !== false && !Fluent.isDefaultPrevented(e))
            this.close(btn.result);
    }

    private createBSButtons(footer: Element, buttons: DialogButton[]) {
        for (let btn of buttons) {
            Fluent(dialogButtonToBS(btn))
                .appendTo(footer)
                .on("click", e => this.onButtonClick(e, btn));
        }
    }

    private createBSModal(opt: DialogOptions): void {

        const header = <div class="modal-header"><h5 class="modal-title"></h5></div> as HTMLDivElement;

        let bs5 = isBS5Plus();
        if (opt.closeButton) {
            const closeButton = <button type="button" class={bs5 ? "btn-close" : "close"} aria-label={DialogTexts.CloseButton}>
                {!bs5 && <span aria-hidden="true">✕</span>}
            </button> as HTMLButtonElement;
            closeButton.dataset[bs5 ? "bsDismiss" : "dismiss"] = "modal";

            if (isBS3()) {
                header.prepend(closeButton);
            } else {
                header.appendChild(closeButton);
            }
        }

        this.el.classList.add("modal-body");

        const footer = <div class="modal-footer"></div> as HTMLDivElement;
        if (opt.buttons) {
            this.createBSButtons(footer, opt.buttons);
        }

        const modal = document.body.appendChild(
            <div class={["modal", opt.dialogClass, opt.fade && "fade"]} tabindex={-1}>
                <div class={[
                    "modal-dialog",
                    opt.size && "modal-" + opt.size,
                    opt.fullScreen && "modal-fullscreen" + (typeof opt.fullScreen === "string" ? `-${opt.fullScreen}` : ""),
                    opt.centered && "modal-dialog-centered",
                    opt.scrollable && "modal-scrollable"]}>
                    <div class="modal-content">
                        {header}
                        {this.el}
                        {footer}
                    </div>
                </div>
            </div>
        );

        let modalOpt = {
            backdrop: opt.backdrop,
            keyboard: opt.closeOnEscape
        };

        if (opt.providerOptions)
            Object.assign(modalOpt, opt.providerOptions("bsmodal", opt));

        if (bs5 && bootstrap.Modal) {
            const modalObj = new bootstrap.Modal(modal, modalOpt);
            if (modalObj && modalObj._focustrap && modalObj._focustrap._handleFocusin) {
                const org: Function = modalObj._focustrap._handleFocusin;
                modalObj._focustrap._handleFocusin = function (event: Event) {
                    if (event.target &&
                        (event.target as any).closest('.dropdown-menu, .s-dropdown-menu, .ui-datepicker, .select2-drop, .cke, .cke_dialog, .flatpickr-calendar'))
                        return;
                    org.apply(this, arguments);
                }
            }
        }
        else {
            getjQuery()?.(modal)?.modal?.(modalOpt);
        };
    }

    private createPanel(opt: DialogOptions) {

        const panel = <div hidden class={["s-Panel", opt.dialogClass]}>
            <div class="panel-titlebar">
                <div class="panel-titlebar-text"></div>
                {opt.closeButton && <button type="button" class="panel-titlebar-close" onClick={this.close.bind(this, null)}></button>}
            </div>
        </div> as HTMLDivElement;

        this.el.classList.add("panel-body");

        if (this.el.parentElement &&
            this.el.parentElement !== document.body) {
            this.el.parentElement.insertBefore(panel, this.el);
        }

        panel.appendChild(this.el);

        if (opt.buttons) {
            this.createBSButtons(panel.appendChild(<div class="panel-footer"></div>) as Element, opt.buttons);
        }
    }

    private createUIDialog(opt: DialogOptions): void {

        let uiOpt: any = {
            autoOpen: opt.autoOpen,
            dialogClass: opt.dialogClass,
            title: opt.title,
            modal: opt.modal,
            width: opt.width,
            resizable: false
        } as any;

        if (opt.centered)
            uiOpt.position = { my: 'center', at: 'center', of: window };

        if (opt.buttons) {
            uiOpt.buttons = opt.buttons.map(btn => {
                let uiButton = dialogButtonToUI(btn);
                uiButton.click = (e: MouseEvent) => this.onButtonClick(e, btn);
                return uiButton;
            });
        }

        if (opt.providerOptions)
            uiOpt = Object.assign(uiOpt, opt.providerOptions("uidialog", omitUndefined(opt)));

        getjQuery()?.(this.el).dialog(uiOpt);
    }


    /**
     * Disposes the dialog, removing it from the DOM and unbinding event handlers.
     * @remarks
     * Handles all three providers: destroys a jQuery UI dialog, disposes a Bootstrap modal instance, or removes the panel markup. Falls back to plain DOM removal when no library is present. Safe to call multiple times.
     */
    dispose(): void {
        try {
            let target = getDialogEventsNode(this.el) ?? this.el;
            if (!target)
                return;

            try {
                if (target.classList.contains("ui-dialog-content")) {
                    getjQuery?.()(target)?.dialog?.('destroy');
                    target.classList.remove("ui-dialog-content");
                    target = target.closest(".ui-dialog") ?? target;
                }
                else if (target.classList.contains("modal")) {
                    if (!getjQuery() && isBS5Plus()) {
                        if (typeof bootstrap !== "undefined")
                            bootstrap.Modal?.getInstance(target)?.dispose?.();
                    }
                    else {
                        getjQuery()?.(target)?.modal?.(isBS3() ? "destroy" : "dispose");
                    }
                    this.el?.classList.remove("modal-body");
                }
                else {
                    this.el?.classList.remove("panel-body");
                    target = target.closest(".s-Panel") ?? target;
                }
            }
            finally {
                Fluent.remove(target);
            }
        }
        finally {
            this.el = null;
        }
    }
}

/**
 * Determines whether a Bootstrap modal provider is available.
 * @returns `true` when Bootstrap 5+ `bootstrap.Modal` or jQuery `fn.modal` (Bootstrap 3/4) is loaded.
 */
export function hasBSModal() {
    return isBS5Plus() || !!(getjQuery()?.fn?.modal);
}

/**
 * Determines whether the jQuery UI dialog provider is available.
 * @returns `true` when `jQuery.ui.dialog` is loaded.
 */
export function hasUIDialog() {
    return !!(getjQuery()?.ui?.dialog);
}

/**
 * Resolves the jQuery UI / Bootstrap button name collision.
 * @remarks
 * When both `$.fn.button` (Bootstrap) and `$.ui.button` (jQuery UI) are present, this moves Bootstrap's implementation to `$.fn.btn` via `noConflict()` so jQuery UI dialogs keep their button widget. Invoked automatically on module load.
 */
export function uiAndBSButtonNoConflict() {
    const $ = getjQuery();

    // if both jQuery UI and bootstrap button exists, prefer jQuery UI button as UI dialog needs them
    if ($ && $.fn?.button?.noConflict && $.ui?.button) {
        $.fn.btn = $.fn.button.noConflict();
    }
}

uiAndBSButtonNoConflict();

function dialogButtonToBS(x: DialogButton): HTMLButtonElement {
    let html = htmlEncode(x.text);
    let iconClass = iconClassName(x.icon);
    if (iconClass)
        html = '<i class="' + htmlEncode(iconClass) + '"></i>' + (html ? (" " + html) : "");
    let button = document.createElement("button");
    button.type = "button";
    button.classList.add("btn");
    Fluent.addClass(button, x.cssClass ?? "btn-secondary");
    if (x.hint)
        button.setAttribute("title", x.hint);
    button.innerHTML = html;
    return button;
}

function dialogButtonToUI(x: DialogButton): any {
    let html = htmlEncode(x.text);
    let iconClass = iconClassName(x.icon);
    if (iconClass)
        html = '<i class="' + htmlEncode(iconClass) + '"></i>' + (html ? (" " + html) : "");
    let button = {
        text: html,
        click: x.click
    } as any;
    if (x.cssClass)
        button.cssClass = x.cssClass;
    return button;
}


/**
 * Creates an "OK" dialog button.
 * @param opt - Optional overrides for {@link DialogButton} properties. Only `text`, `cssClass`, `result`, and `click` are respected; unspecified fields fall back to localized defaults.
 * @returns A {@link DialogButton} with `text` defaulting to `DialogTexts.OkButton`, `cssClass` to `"btn-info"`, and `result` to `"ok"`.
 * @example
 * ```ts
 * new Dialog({ buttons: [okDialogButton({ click: () => save() })] });
 * ```
 */
export function okDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.OkButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : 'btn-info',
        result: opt?.result != void 0 ? opt.result : 'ok',
        click: opt?.click
    }
}

/**
 * Creates a dialog button which, by default, has "Yes" as the caption (localized) and "yes" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
 */
export function yesDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.YesButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : 'btn-primary',
        result: opt?.result != void 0 ? opt.result : 'yes',
        click: opt?.click
    }
}

/**
 * Creates a dialog button which, by default, has "No" as the caption (localized) and "no" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
 */
export function noDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.NoButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : isBS5Plus() ? 'btn-danger' : 'btn-default',
        result: opt?.result != void 0 ? opt.result : 'no',
        click: opt?.click
    }
}

/**
 * Creates a dialog button which, by default, has "Cancel" as the caption (localized) and "cancel" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
 */
export function cancelDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.CancelButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : isBS5Plus() ? 'btn-secondary' : 'btn-default',
        result: opt?.result != void 0 ? opt.result : 'cancel',
        click: opt?.click
    }
}


/**
 * Localizable text constants for dialogs.
 * @remarks
 * Each property is a getter that calls `localText("Dialogs." + key, defaultValue)` and HTML-encodes the result. Defaults are in English; override via `Texts.Dialogs.*` localizations.
 */
export namespace DialogTexts {
    /**
     * Title for alert dialogs.
     */
    export declare const AlertTitle: string;

    /**
     * Text for the cancel button in dialogs.
     */
    export declare const CancelButton: string;

    /**
     * Text for the close button in dialogs.
     */
    export declare const CloseButton: string;

    /**
     * Title for confirmation dialogs.
     */
    export declare const ConfirmationTitle: string;

    /**
     * Title for information dialogs.
     */
    export declare const InformationTitle: string;

    /**
     * Hint for maximizing dialogs.
     */
    export declare const MaximizeHint: string;

    /**
     * Text for the "No" button in dialogs.
     */
    export declare const NoButton: string;

    /**
     * Text for the "OK" button in dialogs.
     */
    export declare const OkButton: string;

    /**
     * Title for the prompt dialog.
     */
    export declare const PromptTitle: string;

    /**
     * Hint for restoring dialogs.
     */
    export declare const RestoreHint: string;

    /**
     * Title for success dialogs.
     */
    export declare const SuccessTitle: string;

    /**
     * Title for warning dialogs.
     */
    export declare const WarningTitle: string;

    /**
     * Text for the "Yes" button in dialogs.
     */
    export declare const YesButton: string;

    const defaultTxt: Record<string, string> = {
        AlertTitle: 'Alert',
        CancelButton: 'Cancel',
        CloseButton: 'Close',
        ConfirmationTitle: 'Confirm',
        InformationTitle: 'Information',
        MaximizeHint: 'Maximize',
        NoButton: 'No',
        OkButton: 'OK',
        PromptTitle: 'Prompt',
        RestoreHint: 'Restore',
        SuccessTitle: 'Success',
        WarningTitle: 'Warning',
        YesButton: 'Yes'
    };

    function get(this: string) {
        return htmlEncode(localText("Dialogs." + this, defaultTxt[this]));
    }

    for (let k of Object.keys(defaultTxt)) {
        Object.defineProperty(DialogTexts, k, {
            get: get.bind(k)
        });
    }
}

function closePanel(el: (HTMLElement | ArrayLike<HTMLElement>)) {

    let panel = getDialogNode(el);
    if (!panel || panel.hidden)
        return;

    const eventsNode = getDialogEventsNode(el) ?? panel;

    let event = Fluent.trigger(eventsNode, "panelbeforeclose");
    if (Fluent.isDefaultPrevented(event))
        return;
    panel.hidden = true;

    let uniqueName = panel.dataset.paneluniquename;
    if (uniqueName) {
        document.querySelectorAll(`[data-hiddenby="${uniqueName}"]`).forEach(hiddenBy => {
            hiddenBy.removeAttribute("data-hiddenby");
        });
    }

    Fluent.trigger(window, "resize");
    document.querySelectorAll(".require-layout").forEach((rl: HTMLElement) => Fluent.isVisibleLike(rl) && Fluent.trigger(rl, "layout"));
    Fluent.trigger(eventsNode, "panelclose");
}

function openPanel(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName?: string) {

    let panel = getDialogNode(element);
    if (!panel)
        return;

    let container = panel.parentElement && panel.parentElement !== document.body ? panel.parentElement :
        (document.querySelector('.panels-container') ?? document.querySelector('section.content') as HTMLElement ?? panel.parentElement ?? document.body);

    if (panel.parentElement !== container) {
        container.appendChild(panel);
    }

    let eventNode = getDialogEventsNode(element) ?? panel;

    let event = Fluent.trigger(eventNode, "panelbeforeopen");
    if (Fluent.isDefaultPrevented(event))
        return;

    panel.dataset.paneluniquename = uniqueName || panel.id || new Date().getTime().toString();
    function setHideBy(e: HTMLElement) {
        if (e === panel ||
            e.tagName === "LINK" ||
            e.tagName === "SCRIPT" ||
            e.hidden ||
            e.dataset.hiddenby ||
            (container && e.parentElement !== container) && !Fluent.isVisibleLike(e))
            return;

        e.dataset.hiddenby = panel.dataset.paneluniquename;
    }

    if (container) {
        let c = container.children;
        const cl = c.length;
        for (let i = 0; i < cl; i++) {
            setHideBy(c[i] as HTMLElement);
        }
    }

    document.querySelectorAll('.ui-dialog, .ui-widget-overlay, .modal.show, .modal.in').forEach(setHideBy);

    panel.hidden = false;
    delete panel.dataset.hiddenby;

    Fluent.trigger(eventNode, "panelopen");
}

/** Returns .s-Panel, .modal, .ui-dialog */
function getDialogNode(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement {
    if (isArrayLike(element))
        element = element[0];
    if (!element)
        return null;
    return element.closest(".modal, .s-Panel, .ui-dialog");

}

/** Returns .panel-body, .modal, .ui-dialog-content */
function getDialogEventsNode(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement {
    if (isArrayLike(element))
        element = element[0];
    if (!element)
        return null;
    return element.closest(".modal, .panel-body, .ui-dialog-content") as HTMLElement ??
        getDialogNode(element)?.querySelector(".panel-body, .ui-dialog-content");
}

/** Returns .panel-body, .modal, .ui-dialog-content */
function getDialogContentNode(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement {
    if (isArrayLike(element))
        element = element[0];
    if (!element)
        return null;
    return element.closest<HTMLElement>(".modal-body, .panel-body, .ui-dialog-content") ??
        getDialogNode(element)?.querySelector(".modal-body, .panel-body, .ui-dialog-content");
}

/**
 * Options for helper message dialogs (`alertDialog`, `confirmDialog`, etc.).
 * @remarks Extends {@link DialogOptions} with message-specific rendering flags.
 */
export interface MessageDialogOptions extends DialogOptions {
    /**
     * Whether to HTML-encode string messages. @defaultValue `true`
     * @deprecated Prefer passing a `RenderableContent` node or pre-sanitized HTML. When `false`, the string is sanitized via `sanitizeHtml`.
     */
    htmlEncode?: boolean;
    /**
     * Whether to preserve line breaks via `white-space: pre-wrap` on the message container. @defaultValue `true`
     * @remarks Only applied when the message is a string; element messages manage their own styling.
     */
    preWrap?: boolean;
}

function getMessageBodyHtml(message: RenderableContent, options?: MessageDialogOptions): HTMLElement {
    const div = document.createElement("div");
    div.className = "message";
    let preWrap = options == null || (options.preWrap == null && typeof message === "string") || options.preWrap;
    if (preWrap)
        div.style.whiteSpace = "pre-wrap";

    if (typeof message === "string" && message.length) {
        let encode = options == null || (options as any).htmlEncode == null || (options as any).htmlEncode;
        if (encode)
            div.innerText = message;
        else {
            div.innerHTML = sanitizeHtml(message);
        }
    }
    else {
        div.append(message ?? "");
    }

    return div;
}

function createMessageDialog(opt: {
    cssClass: string,
    title: string,
    getButtons: () => DialogButton[],
    native: (msg: string) => string,
    message: RenderableContent,
    options: MessageDialogOptions
}): Partial<Dialog> {

    if (!hasBSModal() && !hasUIDialog()) {
        const msg: string = opt.message == null ? "" : typeof opt.message === "string" ? opt.message : opt.message.textContent;
        const result = opt.native(msg);
        opt.options?.onClose?.(result);
        return {
            result
        }
    }

    let options: MessageDialogOptions = Object.assign({}, Dialog.messageDefaults, {
        dialogClass: "s-MessageDialog" + (opt.cssClass ? " " + opt.cssClass : ""),
        title: opt.title
    } satisfies MessageDialogOptions, opt.options);

    if (options.buttons == void 0) {
        options.buttons = opt.getButtons();
    }

    if (options.providerOptions === void 0) {
        options.providerOptions = (type) => {
            if (type === "uidialog") {
                return {
                    width: '40%',
                    maxWidth: 450,
                    minWidth: 180,
                    resizable: false
                }
            }
        }
    }

    if (options.element === void 0) {
        options.element = el => el.append(getMessageBodyHtml(opt.message, options));
    }

    return new Dialog(options);
}

/**
 * Displays a modal alert dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`), whose `result` is `"ok"`.
 * @remarks Falls back to the native `alert()` when neither Bootstrap modal nor jQuery UI dialog is available.
 * @example
 * ```ts
 * alertDialog("An error occurred!");
 * ```
 */
export function alertDialog(message: RenderableContent, options?: MessageDialogOptions): Partial<Dialog> {
    return createMessageDialog({
        message,
        options,
        cssClass: "s-AlertDialog",
        title: DialogTexts.AlertTitle,
        getButtons: () => [okDialogButton({ cssClass: 'btn-danger' })],
        native: (msg) => {
            alert(msg);
            return "ok";
        }
    });
}

/**
 * Additional options for {@link confirmDialog}.
 * @remarks Extends {@link MessageDialogOptions} with callbacks for the secondary buttons.
 */
export interface ConfirmDialogOptions extends MessageDialogOptions {
    /** When `true`, an extra Cancel button (`result` `"cancel"`) is rendered alongside Yes/No. */
    cancelButton?: boolean;
    /** Callback invoked when the Cancel button is clicked (only when `cancelButton` is `true`). */
    onCancel?: () => void;
    /** Callback invoked when the No button is clicked. */
    onNo?: () => void;
}

/**
 * Displays a confirmation dialog with Yes / No (and optional Cancel) buttons.
 * @param message - Text or renderable content shown in the dialog body.
 * @param onYes - Callback invoked when the Yes button is clicked.
 * @param options - Additional {@link ConfirmDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `confirm()`), whose `result` is `"yes"`, `"no"`, or `"cancel"`.
 * @remarks Falls back to the native `confirm()` when neither Bootstrap modal nor jQuery UI dialog is available.
 * @example
 * ```ts
 * confirmDialog("Are you sure you want to delete?", () => {
 *   // do something when yes is clicked
 * });
 * ```
 */
export function confirmDialog(message: RenderableContent, onYes: () => void, options?: ConfirmDialogOptions): Partial<Dialog> {
    return createMessageDialog({
        message,
        options,
        cssClass: "s-ConfirmDialog",
        title: DialogTexts.ConfirmationTitle,
        getButtons: () => {
            let buttons = [yesDialogButton({ click: onYes }), noDialogButton({ click: options?.onNo })];
            if (options?.cancelButton)
                buttons.push(cancelDialogButton({ click: options?.onCancel }));
            return buttons;
        },
        native: (msg) => {
            const result = window.confirm(msg);
            if (result) {
                onYes?.();
                return 'yes';
            }
            else {
                options?.onNo();
                return result === false ? "no" : "";
            }
        }
    });
}

/**
 * Displays an informational dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param onOk - Optional callback invoked when OK is clicked.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`).
 * @example
 * ```ts
 * informationDialog("Operation complete", () => {
 *   // do something when OK is clicked
 * });
 * ```
 */
export function informationDialog(message: RenderableContent, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog> {
    return createMessageDialog({
        message,
        options,
        cssClass: "s-InformationDialog",
        title: DialogTexts.InformationTitle,
        getButtons: () => [okDialogButton({ click: onOk, cssClass: 'btn-info' })],
        native: (msg) => {
            alert(msg);
            onOk?.();
            return "ok";
        }
    });
}

/**
 * Displays a success dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param onOk - Optional callback invoked when OK is clicked.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`).
 * @example
 * ```ts
 * successDialog("Operation complete", () => {
 *   // do something when OK is clicked
 * });
 * ```
 */
export function successDialog(message: RenderableContent, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog> {
    return createMessageDialog({
        message,
        options,
        cssClass: "s-SuccessDialog",
        title: DialogTexts.SuccessTitle,
        getButtons: () => [okDialogButton({ click: onOk, cssClass: 'btn-success' })],
        native: (msg) => {
            alert(msg);
            onOk?.();
            return "ok";
        }
    });
}

/**
 * Displays a warning dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`).
 * @example
 * ```ts
 * warningDialog("Something is odd!");
 * ```
 */
export function warningDialog(message: RenderableContent, options?: MessageDialogOptions): Partial<Dialog> {
    return createMessageDialog({
        message,
        options,
        cssClass: "s-WarningDialog",
        title: DialogTexts.WarningTitle,
        getButtons: () => [okDialogButton({ cssClass: 'btn-warning' })],
        native: (msg) => {
            alert(msg);
            return "ok";
        }
    });
}

/**
 * Options for {@link iframeDialog}.
 */
export interface IFrameDialogOptions {
    /** HTML string rendered inside the sandboxed `iframe` via `srcdoc`. Sanitized before injection. */
    html?: string;
}

/**
 * Displays a dialog whose content is an `iframe` rendering arbitrary HTML.
 * @param options - Configuration containing the HTML to display.
 * @param options.html - Raw HTML placed in the `iframe` `srcdoc` attribute after sanitization; wrapped in `<html>/<body>` when those tags are absent.
 * @returns A {@link Dialog} handle (partial when falling back to `alert` without modal support).
 * @remarks Falls back to `window.alert` with sanitized HTML when neither Bootstrap modal nor jQuery UI dialog is available.
 */
export function iframeDialog(options: IFrameDialogOptions): Partial<Dialog> {

    if (!hasBSModal() && !hasUIDialog()) {
        window.alert(sanitizeHtml(options.html));
        return {
            result: "ok"
        }
    }

    function onOpen(div: HTMLElement) {
        if (div) {
            let iframe = div.appendChild(document.createElement('iframe'));
            iframe.style.border = "none";
            iframe.style.width = "100%";
            let content = sanitizeHtml(options.html);
            if (content.indexOf('<body') < 0)
                content = "<body>" + content + "</body>";
            if (content.indexOf('<html') < 0)
                content = "<html>" + content + "</html>";
            iframe.srcdoc = content;
        }
    }

    return new Dialog({
        title: DialogTexts.AlertTitle,
        dialogClass: "s-IFrameDialog",
        size: "lg",
        autoOpen: true,
        element: el => {
            let div = document.createElement("div");
            div.style.overflow = "hidden";
            div.style.minHeight = "50vh";
            div.style.display = "flex";
            el.append(div);
            onOpen(div);
        },
        providerOptions: (type) => {
            if (type == "uidialog") {
                return {
                    width: '60%',
                    height: '400'
                }
            }
        }
    });
}

const modalShow = (e: Event) => {
    const body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        const evt = Fluent.trigger(body, "modalbeforeopen");
        if (Fluent.isDefaultPrevented(evt))
            e.preventDefault();
    }
}

const modalShown = (e: Event) => {
    const body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        Fluent.trigger(body, "modalopen");
    }
}

const modalHide = (e: Event) => {
    const body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        const evt = Fluent.trigger(body, "modalbeforeclose");
        if (Fluent.isDefaultPrevented(evt))
            e.preventDefault();
    }
}

const modalHidden = (e: Event) => {
    const instance = Dialog.getInstance(e.target as HTMLElement);
    try {
        if (document.activeElement && instance && instance.getDialogNode()?.contains(document.activeElement))
            (document.activeElement as HTMLElement).blur?.();
    } catch { }
    const body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        Fluent.trigger(body, "modalclose");
    }
}

function installBsModalEventPropagation() {
    uninstallBsModalEventPropagation();
    if (typeof document === "undefined" || typeof document.addEventListener !== "function")
        return;
    document.addEventListener("show.bs.modal", modalShow);
    document.addEventListener("shown.bs.modal", modalShown);
    document.addEventListener("hide.bs.modal", modalHide);
    document.addEventListener("hidden.bs.modal", modalHidden);
}

function uninstallBsModalEventPropagation() {
    if (typeof document === "undefined" || typeof document.removeEventListener !== "function")
        return;
    document.removeEventListener("show.bs.modal", modalShow);
    document.removeEventListener("shown.bs.modal", modalShown);
    document.removeEventListener("hide.bs.modal", modalHide);
    document.removeEventListener("hidden.bs.modal", modalHidden);
}

installBsModalEventPropagation();