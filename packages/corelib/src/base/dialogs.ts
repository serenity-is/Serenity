import { getjQuery, isBS3, isBS5Plus } from "./environment";
import { Fluent } from "./fluent";
import { htmlEncode } from "./html";
import { iconClassName, type IconClassName } from "./icons";
import { localText } from "./localtext";
import { isArrayLike, isPromiseLike, omitUndefined } from "./system";

/**
 * Options for a message dialog button
 */
export interface DialogButton {
    /** Button text */
    text?: string;
    /** Button hint */
    hint?: string;
    /** Button icon */
    icon?: IconClassName;
    /** Click handler */
    click?: (e: MouseEvent) => void | false | Promise<void | false>;
    /** CSS class for button */
    cssClass?: string;
    /** The code that is returned from message dialog function when this button is clicked.
     *  If this is set, and click event will not be defaultPrevented dialog will close.
     */
    result?: string;
}

export type DialogType = "bsmodal" | "uidialog" | "panel";

/**
 * Options that apply to all dialog types
 */
export interface DialogOptions {
    /** Auto dispose dialog on close, default is true */
    autoDispose?: boolean;
    /** True to auto open dialog */
    autoOpen?: boolean;
    /** Backdrop type, static to make it modal, e.g. can't be closed by clicking outside */
    backdrop?: boolean | "static"
    /** List of buttons to show on the dialog */
    buttons?: DialogButton[];
    /** Vertically center modal */
    centered?: boolean;
    /** Show close button, default is true */
    closeButton?: boolean;
    /** Close dialog on escape key. Default is true for message dialogs. */
    closeOnEscape?: boolean;
    /** CSS class to use for all dialog types. Is added to the top ui-dialog, panel or modal element */
    dialogClass?: string;
    /** Dialog content/body element, or callback that will populate the content element */
    element?: HTMLElement | ArrayLike<HTMLElement> | ((element: HTMLElement) => void);
    /** Enable / disable animation. Default is false for message dialogs, true for other dialogs */
    fade?: boolean;
    /** Sets one of modal-fullscreen{-...-down} classes. Only used for bootstrap modals */
    fullScreen?: boolean | "sm-down" | "md-down" | "lg-down" | "xl-down" | "xxl-down",
    /** Modal option for jQuery UI dialog compatibility only. Not to be confused with Bootstrap modal. */
    modal?: boolean;
    /** Event handler that is called when dialog is opened */
    onOpen?: (e?: Event) => void;
    /** Event handler that is called when dialog is closed */
    onClose?: (result: string, e?: Event) => void;
    /** Prefer Bootstrap modals to jQuery UI dialogs when both are available */
    preferBSModal?: boolean;
    /** Prefer Panel even when Modal / jQuery UI is available */
    preferPanel?: boolean;
    /** Callback to get options specific to the dialog provider type */
    providerOptions?: (type: DialogType, opt: DialogOptions) => any;
    /** Scrollable, sets content of the modal to scrollable, only for Bootstrap */
    scrollable?: boolean;
    /** Size. Default is null for (500px) message dialogs, lg for normal dialogs */
    size?: "sm" | "md" | "lg" | "xl";
    /** Dialog title */
    title?: string;
    /** Only used for jQuery UI dialogs for backwards compatibility */
    width?: number;
}

/**
 * Wrapper for different types of dialogs, including jQuery UI, Bootstrap modals, and Panels.
 */
export class Dialog {

    private el: HTMLElement;
    private dialogResult: string;

    /**
     * Creates a new dialog. The type of the dialog will be determined based on 
     * the availability of jQuery UI, Bootstrap, and the options provided.
     * @param opt Optional configuration for the dialog
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

        if (this.el.classList.contains("hidden") &&
            typeof opt.element !== "function")
            this.el.classList.remove("hidden");

        if (opt.preferPanel || (!hasBSModal() && !hasUIDialog))
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
            this.onClose(() => setTimeout(this.dispose.bind(this), 0));

        if (opt.title !== void 0) {
            this.title(opt.title);
        }

        if (opt.autoOpen)
            this.open();
    }

    /** Default set of dialog options */
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

    /** Default set of message dialog options */
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

    /** The result code of the button that is clicked. Also attached to the dialog element as data-dialog-result */
    get result(): string {
        return this.el ? this.el.dataset.dialogResult : this.dialogResult;
    }

    /** Closes dialog setting the result to null */
    close(): this;
    /** Closes dialog with the result set to value */
    close(result: string): this;
    close(result?: string): this {
        this.el && (this.el.dataset.dialogResult = result ?? null);
        this.dialogResult = result ?? null;

        var target = getDialogEventsNode(this.el);
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
     * Adds an event handler that is called when the dialog is closed. If the second parameter is true, the handler is called before the dialog is closed and
     * the closing can be cancelled by calling preventDefault on the event object.
     * @param handler The event handler function
     * @param before Indicates whether the handler should be called before the dialog is closed
     * @returns The dialog instance
     */
    onClose(handler: (result?: string, e?: Event) => void, before = false) {
        var target = getDialogEventsNode(this.el);
        if (!target)
            return;
        if (target.classList.contains("panel-body"))
            Fluent.on(target, before ? "panelbeforeclose" : "panelclose", e => handler(this.result, e));
        else if (target.classList.contains("ui-dialog-content"))
            Fluent.on(target, before ? "dialogbeforeclose" : "dialogclose", e => handler(this.result, e));
        else if (target.classList.contains("modal"))
            Fluent.on(target, before ? "hide.bs.modal" : "hidden.bs.modal", e => handler(this.result, e));
    }

    /**
     * Adds an event handler that is called when the dialog is opened. If the second parameter is true, the handler is called before the dialog is opened and
     * the opening can be cancelled by calling preventDefault on the event object.
     * @param handler The event handler function
     * @param before Indicates whether the handler should be called before the dialog is opened
     * @returns The dialog instance
     */    
    onOpen(handler: (e?: Event) => void, before = false): this {
        var target = getDialogEventsNode(this.el);
        if (!target)
            return;
        if (target.classList.contains("panel-body"))
            Fluent.on(target, before ? "panelbeforeopen" : "panelopen", handler);
        else if (target.classList.contains("ui-dialog-content"))
            Fluent.on(target, before ? "dialogbeforeopen" : "dialogopen", handler);
        else if (target.classList.contains("modal"))
            Fluent.on(target, before ? "show.bs.modal" : "shown.bs.modal", handler);
        return this;
    }

    /** Opens the dialog */
    open() {
        var target = getDialogEventsNode(this.el);
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

    /** Gets the title text of the dialog */
    title(): string;
    /** Sets the title text of the dialog. */
    title(value: string): this;
    title(value?: string): string | this {
        let title = this.getHeaderNode()?.querySelector(".modal-title, .panel-titlebar-text, .ui-dialog-title");
        if (value === void 0 && !arguments.length)
            return title?.textContent;

        title && (title.textContent = value);
        return this;
    }

    /** Returns the type of the dialog, or null if no dialog on the current element or if the element is null, e.g. dialog was disposed  */
    get type(): DialogType {
        var root = getDialogNode(this.el);
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

    /** Gets the body/content element of the dialog */
    getContentNode(): HTMLElement {
        return this.el;
    }

    /** Gets the dialog element of the dialog */
    getDialogNode(): HTMLElement {
        return getDialogNode(this.el);
    }

    /** Gets the node that receives events for the dialog. It's .ui-dialog-content, .modal, or .panel-body */
    getEventsNode(): HTMLElement {
        return getDialogEventsNode(this.el);
    }

    /** Gets the footer element of the dialog */
    getFooterNode(): HTMLElement {
        return this.getDialogNode()?.querySelector(".modal-footer, .panel-footer, .ui-dialog-footer");
    }

    /** Gets the header element of the dialog */
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

        var value = btn.click(e);
        if (!btn.result)
            return;

        if (isPromiseLike(value))
            value.then(value => value !== false && !Fluent.isDefaultPrevented(e) && this.close(btn.result));
        else if (value !== false && !Fluent.isDefaultPrevented(e))
            this.close(btn.result);
    }

    private createBSButtons(footer: Fluent, buttons: DialogButton[]) {
        for (let btn of buttons) {
            Fluent(dialogButtonToBS(btn))
                .appendTo(footer)
                .on("click", e => this.onButtonClick(e, btn));
        }
    }

    private createBSModal(opt: DialogOptions): void {

        var modal = Fluent("div")
            .class(["modal", opt.dialogClass, opt.fade && "fade"])
            .attr("tabindex", "-1")
            .appendTo(document.body);

        let header = Fluent("div")
            .class("modal-header")
            .append(Fluent("h5").class("modal-title"));

        let bs5 = isBS5Plus();
        if (opt.closeButton) {
            let closeButton = Fluent("button")
                .class(bs5 ? "btn-close" : "close")
                .attr("type", "button")
                .data(`${bs5 ? "bs-" : ""}dismiss`, "modal")
                .attr("aria-label", DialogTexts.CloseButton);

            if (!bs5) {
                closeButton.append(Fluent("span").attr("aria-hidden", "true").text("\u2715"));
            }

            if (isBS3()) {
                closeButton.prependTo(header);
            } else {
                closeButton.appendTo(header);
            }
        }

        this.el.classList.add("modal-body");

        let footer = Fluent("div")
            .class("modal-footer");

        Fluent("div")
            .class([
                "modal-dialog",
                opt.size && "modal-" + opt.size,
                opt.fullScreen && "modal-fullscreen" + (typeof opt.fullScreen === "string" ? `-${opt.fullScreen}` : ""),
                opt.centered && "modal-dialog-centered",
                opt.scrollable && "modal-scrollable"])
            .append(Fluent("div")
                .class("modal-content")
                .append(header)
                .append(this.el)
                .append(footer))
            .appendTo(modal);


        if (opt.buttons) {
            this.createBSButtons(footer, opt.buttons);
        }

        let modalOpt = {
            backdrop: opt.backdrop,
            keyboard: opt.closeOnEscape
        };

        if (opt.providerOptions)
            Object.assign(modalOpt, opt.providerOptions("bsmodal", opt));

        if (bs5 && bootstrap.Modal) {
            var modalObj = new bootstrap.Modal(modal.getNode(), modalOpt);
            if (modalObj && modalObj._focustrap && modalObj._focustrap._handleFocusin) {
                var org: Function = modalObj._focustrap._handleFocusin;
                modalObj._focustrap._handleFocusin = function (event: Event) {
                    if (event.target &&
                        (event.target as any).closest('.ui-datepicker, .select2-drop, .cke, .cke_dialog, .flatpickr-calendar'))
                        return;
                    org.apply(this, arguments);
                }
            }
        }
        else {
            getjQuery()?.(modal.getNode())?.modal?.(modalOpt);
        };
    }

    private createPanel(opt: DialogOptions) {

        let titlebar = Fluent("div")
            .class("panel-titlebar")
            .append(Fluent("div")
                .class("panel-titlebar-text"));

        let panel = Fluent("div")
            .class(["s-Panel", "hidden", opt.dialogClass])
            .append(titlebar)

        this.el.classList.add("panel-body");

        if (this.el.parentElement &&
            this.el.parentElement !== document.body) {
            this.el.parentElement.insertBefore(panel.getNode(), this.el);
        }

        panel.append(this.el);

        if (opt.closeButton) {
            Fluent("button")
                .class("panel-titlebar-close")
                .attr("type", "button")
                .on("click", this.close.bind(this, null))
                .appendTo(titlebar);
        }

        opt.buttons && this.createBSButtons(Fluent("div")
            .class("panel-footer")
            .appendTo(panel), opt.buttons);

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
     * Disposes the dialog, removing it from the DOM and unbinding all event handlers.
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

/** Returns true if Bootstrap modal is available */
export function hasBSModal() {
    return isBS5Plus() || !!(getjQuery()?.fn?.modal);
}

/** Returns true if jQuery UI dialog is available */
export function hasUIDialog() {
    return !!(getjQuery()?.ui?.dialog);
}

/** Calls Bootstrap button.noConflict method if both jQuery UI and Bootstrap buttons are available in the page */
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
        html = '<i class="' + htmlEncode(iconClass) + '"><i>' + (html ? (" " + html) : "");
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
 * Creates a dialog button which, by default, has "Yes" as caption (localized) and "ok" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
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
 * Namespace containing localizable text constants for dialogs.
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
    if (!panel || panel.classList.contains("hidden"))
        return;

    var eventsNode = getDialogEventsNode(el) ?? panel;

    let event = Fluent.trigger(eventsNode, "panelbeforeclose");
    if (Fluent.isDefaultPrevented(event))
        return;
    panel.classList.add("hidden");

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
            e.classList.contains("hidden") ||
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

    panel.classList.remove("hidden");
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
 * Options that apply to all message dialog types
 */
export interface MessageDialogOptions extends DialogOptions {
    /** HTML encode the message, default is true */
    htmlEncode?: boolean;
    /** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
    preWrap?: boolean;
}

function getMessageBodyHtml(message: string, options?: MessageDialogOptions): string {
    let encode = options == null || options.htmlEncode == null || options.htmlEncode;
    if (encode)
        message = htmlEncode(message);

    let preWrap = options == null || (options.preWrap == null && encode) || options.preWrap;
    return '<div class="message"' + (preWrap ? ' style="white-space: pre-wrap">' : '>') + message + '</div>';
}

function createMessageDialog(opt: {
    cssClass: string,
    title: string,
    getButtons: () => DialogButton[],
    native: (msg: string) => string,
    message: string,
    options: MessageDialogOptions
}): Partial<Dialog> {

    if (!hasBSModal() && !hasUIDialog()) {
        var result = opt.native(opt.message);
        opt.options?.onClose(result);
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
        options.element = el => el.innerHTML = getMessageBodyHtml(opt.message, options);
    }

    return new Dialog(options);
}

/** 
 * Displays an alert dialog 
 * @param message The message to display
 * @param options Additional options. 
 * @see AlertOptions 
 * @example 
 * alertDialog("An error occured!"); }
 */
export function alertDialog(message: string, options?: MessageDialogOptions): Partial<Dialog> {
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

/** Additional options for confirm dialog */
export interface ConfirmDialogOptions extends MessageDialogOptions {
    /** True to also add a cancel button */
    cancelButton?: boolean;
    /** Event handler for cancel button click */
    onCancel?: () => void;
    /** Event handler for no button click */
    onNo?: () => void;
}

/** 
 * Display a confirmation dialog 
 * @param message The message to display
 * @param onYes Callback for Yes button click 
 * @param options Additional options. 
 * @see ConfirmOptions 
 * @example 
 * confirmDialog("Are you sure you want to delete?", () => { 
 *     // do something when yes is clicked
 * }
 */
export function confirmDialog(message: string, onYes: () => void, options?: ConfirmDialogOptions): Partial<Dialog> {
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
            var result = window.confirm(msg);
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
 * Display an information dialog 
 * @param message The message to display
 * @param onOk Callback for OK button click 
 * @param options Additional options. 
 * @see ConfirmOptions 
 * @example 
 * informationDialog("Operation complete", () => { 
 *     // do something when OK is clicked
 * }
 */
export function informationDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog> {
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
 * Display a success dialog 
 * @param message The message to display
 * @param onOk Callback for OK button click 
 * @param options Additional options. 
 * @see MessageDialogOptions 
 * @example 
 * successDialog("Operation complete", () => { 
 *     // do something when OK is clicked
 * }
 */
export function successDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog> {
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
 * Display a warning dialog 
 * @param message The message to display
 * @param options Additional options. 
 * @see MessageDialogOptions 
 * @example 
 * warningDialog("Something is odd!");
 */
export function warningDialog(message: string, options?: MessageDialogOptions): Partial<Dialog> {
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

/** Options for `iframeDialog` **/
export interface IFrameDialogOptions {
    html?: string;
}

/** 
 * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
 * @param options The options
 */
export function iframeDialog(options: IFrameDialogOptions): Partial<Dialog> {

    if (!hasBSModal() && !hasUIDialog()) {
        window.alert(options.html);
        return {
            result: "ok"
        }
    }

    let doc: Document;
    function onOpen(div: HTMLElement) {
        if (div) {
            let iframe = div.appendChild(document.createElement('iframe'));
            iframe.setAttribute("style", "border: none; width: 100%; height: 100%;");
            doc = iframe.contentDocument;
            if (doc) {
                doc.open();
                doc.write(options.html);
                doc.close();
            }
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
    var body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        var evt = Fluent.trigger(body, "modalbeforeopen");
        if (Fluent.isDefaultPrevented(evt))
            e.preventDefault();
    }
}

const modalShown = (e: Event) => {
    var body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        Fluent.trigger(body, "modalopen");
    }
}

const modalHide = (e: Event) => {
    var body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
    if (body) {
        var evt = Fluent.trigger(body, "modalbeforeclose");
        if (Fluent.isDefaultPrevented(evt))
            e.preventDefault();
    }
}

const modalHidden = (e: Event) => {
    var body = Dialog.getInstance(e.target as HTMLElement)?.getContentNode();
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