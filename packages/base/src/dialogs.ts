import { Config } from "./config";
import { Fluent, addClass, htmlEncode, toggleClass } from "./html";
import { iconClassName, type IconClassName } from "./icons";
import { localText } from "./localtext";
import { getjQuery, isArrayLike } from "./system";

export type DialogType = "bs3" | "bs4" | "bs5" | "jqueryui" | "panel";

/**
 * Options that apply to all dialog types
 */
export interface DialogOptions {
    /** True to open dialog as panel */
    asPanel?: boolean;
    /** True to auto open. Ignored for message dialogs. */
    autoOpen?: boolean;
    /** Prefer Bootstrap modals to jQuery UI dialogs when both are available */
    bootstrap?: boolean;
    /** List of buttons to show on the dialog */
    buttons?: DialogButton[];
    /** Show close button, default is true */
    closeButton?: boolean;
    /** CSS class to use for all dialog types. Is added to the top ui-dialog, panel or modal element */
    dialogClass?: string;
    /** Dialog content element, or callback that will populate the content */
    element?: HTMLElement | ((element: HTMLElement) => void);
    /** Is modal dialog, default is true, only used for jQuery UI */
    modal?: boolean;
    /** Additional CSS class to use only for BS modals, like modal-lg etc. */
    modalClass?: string;
    /** Event handler that is called when dialog is opened */
    onOpen?: () => void;
    /** Event handler that is called when dialog is closed */
    onClose?: (result: string) => void;
    /** Callback to get options specific to the dialog provider type */
    providerOptions?: (type: DialogType, opt: DialogOptions) => any;
    /** Dialog title */
    title?: string;
    /** Dialog width. Only used for jQuery UI dialogs */
    width?: number;
}

export interface ICommonDialog {
    /** Gets dialog provider type used */
    readonly type: DialogType;
    /** Opens the dialog */
    open(): void;
    /** Closes the dialog */
    close(result?: string): void;
    /** Sets the title of the dialog */
    title: string;
    /** Dispose the dialog instance */
    dispose(): void;
    /** The result code of the button that is clicked */
    readonly result?: string;
}

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
    click?: (e: MouseEvent) => void;
    /** CSS class for button */
    cssClass?: string;
    /** HTML encode button text. Default is true. */
    htmlEncode?: boolean;
    /** The code that is returned from message dialog function when this button is clicked.
     *  If this is set, and click event will not be defaultPrevented dialog will close.
     */
    result?: string;
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

(function () {
    const $ = getjQuery();

    // if both jQuery UI and bootstrap button exists, prefer jQuery UI button as UI dialog needs them
    if ($ && $.fn?.button?.noConflict && $.ui?.button) {
        $.fn.btn = $.fn.button.noConflict();
    }
})();


function hasBSModal() {
    return isBS5Plus() || !!(getjQuery()?.fn?.modal);
}

function hasUIDialog() {
    return !!(getjQuery()?.ui?.dialog);
}

/** Returns true if Bootstrap 3 is loaded */
export function isBS3(): boolean {
    return (getjQuery()?.fn?.modal?.Constructor?.VERSION + "").charAt(0) == '3';
}

/** Returns true if Bootstrap 5+ is loaded */
export function isBS5Plus(): boolean {
    return typeof bootstrap !== "undefined" && !!bootstrap.Modal && (bootstrap.Modal.VERSION + "").charAt(0) != '4';
}

export namespace DialogTexts {
    export declare const AlertTitle: string;
    export declare const CancelButton: string;
    export declare const CloseButton: string;
    export declare const ConfirmationTitle: string;
    export declare const InformationTitle: string;
    export declare const MaximizeHint: string;
    export declare const NoButton: string;
    export declare const OkButton: string;
    export declare const RestoreHint: string;
    export declare const SuccessTitle: string;
    export declare const WarningTitle: string;
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

function bsCreateButton(footer: HTMLElement, x: DialogButton, close: (result: string) => void) {

    if (!footer)
        return;

    let button = dialogButtonToBS(x);
    footer.append(button);
    Fluent.on(button, "click", e => {
        x.click && x.click.call(this, e);
        if (x.result && !((e as any)?.isDefaultPrevented?.() || e.defaultPrevented))
            close(x.result);
    });
}

function createPanel(options: DialogOptions): ICommonDialog {
    let result: string;
    let panelBody = options.element && typeof options.element !== "function" ? options.element :
        document.createElement("div");
    panelBody.classList.add("panel-body");
    let panel = document.createElement("div");
    panel.classList.add("s-Panel");
    panel.append(panelBody);
    if (typeof options.element === "function")
        options.element(panelBody);
    setPanelTitle(panel, options.title, options.closeButton ?? true);
    if (options.onOpen) {
        Fluent.on(panel, "panelopen", options.onOpen);
    }
    if (options.onClose) {
        Fluent.on(panel, "panelclose", () => options.onClose(result));
    }
    let close = (res?: string) => {
        if (res !== void 0)
            result = res;
        panel && closePanel(panel);
    }

    if (options.buttons) {
        let footer = panel.appendChild(document.createElement("div"));
        footer.classList.add("panel-footer");
        for (let button of options.buttons) {
            bsCreateButton(footer, button, close);
        }
    }

    if (options.title !== void 0)
        setPanelTitle(panel, options.title);

    return {
        type: "panel",
        open: () => panel && openPanel(panel),
        close,
        dispose: () => {
            if (!panel)
                return;
            Fluent(panel).remove();
            panel = null;
        },
        get title() { return panel?.querySelector(".panel-titlebar-text").textContent },
        set title(value: string) { panel && setPanelTitle(panel, value) },
        get result() { return result }
    }
}

function createUIDialog(options: DialogOptions): ICommonDialog {
    var result: string;
    let opt = {
        dialogClass: options.dialogClass,
        title: options.title,
        modal: options.modal ?? true,
        open: function () {
            options.onOpen?.();
        },
        close: function () {
            options.onClose?.(result);
        },
        width: options.width
    } as any;

    let content = options.element && typeof options.element !== "function" ? options.element :
        document.createElement("div");

    // templated dialog hides its content element before first open
    if (content.classList.contains("hidden"))
        content.classList.remove("hidden");

    if (typeof options.element === "function")
        options.element(content);

    let $ = getjQuery();
    let close = (res?: string) => {
        if (res !== "void 0")
            result = res;
        content && $(content).dialog("close");
    }

    if (options.buttons) {
        opt.buttons = options.buttons.map(x => {
            let btn = dialogButtonToUI(x);
            btn.click = function (e: Event) {
                e ??= new Event("click", {});
                x.click && x.click.call(this, e);
                if (x.result && !((e as any).isDefaultPrevented?.() || e.defaultPrevented))
                    close(x.result);
            }
            return btn;
        });
    }

    if (options.providerOptions)
        opt = Object.assign(opt, options.providerOptions("jqueryui", options));

    $(content).dialog(opt);

    return {
        type: "jqueryui",
        close,
        dispose: () => {
            if (content) {
                getjQuery()(content)?.dialog?.('destroy');
                (content.querySelector('.ui-dialog-content') as HTMLElement)?.classList.remove('ui-dialog-content');
                content.remove();
                content = null;
            }
        },
        open: () => content && getjQuery()(content).dialog("open"),
        get result() { return result; },
        get title() { return !content ? null : getjQuery()(content).dialog("option", "title"); },
        set title(value: string) { content && getjQuery()(content).dialog("option", "title", value ?? ''); }
    }
}

function createBSModal(options: DialogOptions): ICommonDialog {
    let modalDiv = bsModalMarkup(options.title, options.dialogClass + (options.modalClass ? ' ' + options.modalClass : ''));
    if (options.element) {
        if (typeof options.element === "function")
            options.element(modalDiv.querySelector(".modal-body"));
        else {
            modalDiv.querySelector(".modal-body")?.replaceWith(options.element);
            options.element.classList.add("modal-body");
        }
    }

    if (!getjQuery() && isBS5Plus())
        return createBS5RawModal(modalDiv, options);

    return createBSModalWithJQuery(modalDiv, options);
}

function createBS5RawModal(modalDiv: HTMLElement, options: DialogOptions): ICommonDialog {
    let footer = modalDiv.querySelector('.modal-footer') as HTMLElement;
    let result: string;
    document.body.appendChild(modalDiv);
    if (options.onOpen)
        modalDiv.addEventListener("shown.bs.modal", options.onOpen);

    modalDiv.addEventListener('hidden.bs.modal', e => {
        try {
            options.onClose?.(result);
        }
        finally {
            Fluent(modalDiv).remove();
        }
    });

    var modal = new bootstrap.Modal(modalDiv, {
        backdrop: false
    });

    let close = (res: string) => {
        if (res !== void 0)
            result = res;
        modal && modal.hide();
    }

    if (options.buttons) {
        for (let button of options.buttons) {
            bsCreateButton(footer, button, close);
        }
    }

    return {
        type: "bs5",
        open: () => modal && modal.show(),
        close: close,
        dispose: () => {
            if (modal) {
                modal.dispose?.();
                Fluent(modalDiv).remove();
                modal = null;
            }
        },
        get result() { return result; },
        get title() { return modal?.querySelector('.modal-title')?.textContent },
        set title(value: string) { let titleEl = modal?.querySelector('.modal-title'); titleEl && (titleEl.textContent = value ?? ''); },
    }
}

function createBSModalWithJQuery(modalDiv: HTMLElement, options: DialogOptions): ICommonDialog {
    let result: string;
    let footer = modalDiv.querySelector('.modal-footer') as HTMLElement;
    let modalDiv$ = getjQuery()(modalDiv).appendTo(document.body);

    if (options.onOpen)
        modalDiv$.one('shown.bs.modal', options.onOpen);

    modalDiv$.one('hidden.bs.modal', (e: any) => {
        try {
            options.onClose?.(result);
        }
        finally {
            Fluent(modalDiv$).remove();
        }
    });

    modalDiv$.modal({
        backdrop: false
    });

    let close = (res?: string) => {
        if (res !== void 0)
            result = res;
        modalDiv$ && modalDiv$.modal('hide');
    }

    if (options.buttons) {
        for (let button of options.buttons) {
            bsCreateButton(footer, button, close);
        }
    }

    return {
        type: isBS5Plus ? "bs5" : (isBS3 ? "bs3" : "bs4"),
        open: () => modalDiv$ && modalDiv$.modal('show'),
        close,
        dispose: () => {
            if (!modalDiv$)
                return;
            modalDiv$.data('bs.modal', null);
            modalDiv$.find(".modal-body").removeClass("modal-body");
            window.setTimeout(() => Fluent(modalDiv$).remove(), 0);
            Fluent(modalDiv$).remove();
            modalDiv$ = null;
        },
        get result() { return result; },
        get title() { return modalDiv$?.find('.modal-title').first().text() },
        set title(value: string) { modalDiv$?.find('.modal-title').first().text(value ?? '') }
    };
}

export function createCommonDialog(options: DialogOptions): ICommonDialog {

    let dialogLike: ICommonDialog;

    if (options.asPanel || (!hasBSModal() && !hasUIDialog))
        dialogLike = createPanel(options);
    else if (hasUIDialog() && (!hasBSModal() || !options.bootstrap))
        dialogLike = createUIDialog(options);
    else
        dialogLike = createBSModal(options);

    if (options.autoOpen)
        dialogLike.open();

    return dialogLike;
}

/** 
 * Builds HTML DIV element for a Bootstrap modal dialog
 * @param title Modal title
 * @param body Modal body, it will not be HTML encoded, so make sure it is encoded 
 * @param modalClass Optional class to add to the modal element
 * @param escapeHtml True to html encode body, default is true
 * @returns 
 */
function bsModalMarkup(title: string, modalClass?: string): HTMLDivElement {
    let closeButton = `<button type="button" class="${isBS5Plus() ? "btn-" : ""}close" data-${isBS5Plus() ? "bs-" : ""}dismiss="modal" aria-label="${DialogTexts.CloseButton}">` +
        `${isBS5Plus() ? "" : '<span aria-hidden="true">&times;</span>'}</button>`;
    let div = document.createElement("div");
    div.classList.add("modal");
    if (modalClass)
        addClass(div, modalClass);
    div.setAttribute("tabindex", "-1");
    div.setAttribute("role", "dialog");
    div.innerHTML = `<div class="modal-dialog" role="document">
    <div class="modal-content">
        <div class="modal-header">
            ${(isBS3() ? closeButton : "")}<h5 class="modal-title">${htmlEncode(title)}</h5>${(isBS3() ? "" : closeButton)}
        </div>
        <div class="modal-body"></div>
        <div class="modal-footer"></div>
    </div>
</div>`;
    return div;
}

/** Converts a `DialogButton` declaration to Bootstrap button element 
 * @param x Dialog button declaration
 * @returns Bootstrap button element
*/
export function dialogButtonToBS(x: DialogButton): HTMLButtonElement {
    let html = x.htmlEncode == null || x.htmlEncode ? htmlEncode(x.text) : x.text;
    let iconClass = iconClassName(x.icon);
    if (iconClass)
        html = '<i class="' + htmlEncode(iconClass) + '"><i>' + (html ? (" " + html) : "");
    let button = document.createElement("button");
    button.classList.add("btn");
    if (x.cssClass)
        toggleClass(button, x.cssClass, true);
    if (x.hint)
        button.setAttribute("title", x.hint);
    button.innerHTML = html;
    return button;
}

/** Converts a `DialogButton` declaration to jQuery UI button type
 * @param x Dialog button declaration
 * @returns jQuery UI button type
 */
export function dialogButtonToUI(x: DialogButton): any {
    let html = x.htmlEncode == null || x.htmlEncode ? htmlEncode(x.text) : x.text;
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
}) {

    if (!hasBSModal() && !hasUIDialog()) {
        var result = opt.native(opt.message);
        opt.options?.onClose(result);
        return {
            result: result
        }
    }

    let options: MessageDialogOptions = Object.assign({
        autoOpen: true,
        bootstrap: Config.bootstrapMessages,
        dialogClass: "s-MessageDialog" + (opt.cssClass ? " " + opt.cssClass : ""),
        htmlEncode: true,
        title: opt.title
    } satisfies MessageDialogOptions, opt.options);

    if (options.buttons == void 0) {
        options.buttons = opt.getButtons();
    }

    if (options.providerOptions === void 0) {
        options.providerOptions = (type: string) => {
            if (type === "jqueryui") {
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

    return createCommonDialog(options);
}


export function okDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.OkButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : 'btn-info',
        result: opt?.result != void 0 ? opt.result : 'ok',
        click: opt?.click
    }
}

export function yesDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.YesButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : 'btn-primary',
        result: opt?.result != void 0 ? opt.result : 'yes',
        click: opt?.click
    }
}

export function noDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.NoButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : isBS5Plus() ? 'btn-danger' : 'btn-default',
        result: opt?.result != void 0 ? opt.result : 'no',
        click: opt?.click
    }
}

export function cancelDialogButton(opt?: DialogButton): DialogButton {
    return {
        text: opt?.text != void 0 ? opt.text : DialogTexts.CancelButton,
        cssClass: opt?.cssClass != void 0 ? opt.cssClass : isBS5Plus() ? 'btn-secondary' : 'btn-default',
        result: 'cancel',
        click: opt?.click
    }
}

/** 
 * Displays an alert dialog 
 * @param message The message to display
 * @param options Additional options. 
 * @see AlertOptions 
 * @example 
 * alertDialog("An error occured!"); }
 */
export function alertDialog(message: string, options?: MessageDialogOptions): Partial<ICommonDialog> {
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
export function confirmDialog(message: string, onYes: () => void, options?: ConfirmDialogOptions): Partial<ICommonDialog> {
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
export function informationDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<ICommonDialog> {
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
export function successDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<ICommonDialog> {
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
export function warningDialog(message: string, options?: MessageDialogOptions): Partial<ICommonDialog> {
    return createMessageDialog({
        message,
        options,
        cssClass: "s-WarningDialog",
        title: DialogTexts.SuccessTitle,
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
export function iframeDialog(options: IFrameDialogOptions): Partial<ICommonDialog> {

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

    return createCommonDialog({
        title: DialogTexts.AlertTitle,
        dialogClass: "s-IFrameDialog",
        modalClass: "modal-lg",
        autoOpen: true,
        element: el => {
            let div = document.createElement("div");
            div.style.overflow = "hidden";
            el.append(div);
            onOpen(div);
        },
        providerOptions: (type) => {
            if (type == "jqueryui") {
                return {
                    width: '60%',
                    height: '400'
                }
            }
        }
    });
}

/** 
 * Closes a panel, triggering panelbeforeclose and panelclose events on the panel element.
 * If the panelbeforeclose prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param e  The event triggering the close
 */
export function closePanel(el: (HTMLElement | ArrayLike<HTMLElement>)) {

    let panel = getDialogRootElement(el);
    if (!panel || panel.classList.contains("hidden"))
        return;

    let event = Fluent.trigger(panel, "panelbeforeclose", { bubbles: true });
    if (event?.defaultPrevented || event?.isDefaultPrevented?.())
        return;
    panel.classList.add("hidden");

    let uniqueName = panel.dataset.paneluniquename;
    if (uniqueName) {
        document.querySelectorAll(`[data-hiddenby="${uniqueName}"]`).forEach(hiddenBy => {
            hiddenBy.removeAttribute("data-hiddenby");
        });
    }

    Fluent.trigger(window, "resize");
    document.querySelectorAll(".require-layout").forEach((rl: HTMLElement) => {
        if (rl.offsetWidth > 0 || rl.offsetHeight > 0)
            Fluent.trigger(rl, "layout");
    });
    Fluent.trigger(panel, "panelclose", { bubbles: true });
}

/** 
 * Opens a panel, triggering panelbeforeopen and panelopen events on the panel element,
 * and panelopening and panelopened events on the window.
 * If the panelbeforeopen prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param uniqueName A unique name for the panel. If not specified, the panel id is used. If the panel has no id, a timestamp is used.
 * @param e The event triggering the open
 */
export function openPanel(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName?: string) {

    let panel = getDialogRootElement(element);
    if (!panel)
        return;

    let container = document.querySelector('.panels-container') ?? document.querySelector('section.content') as HTMLElement;
    if (panel.parentElement !== container)
        container.appendChild(panel);

    let event = Fluent.trigger(panel, "panelbeforeopen", { bubbles: true });
    if (event?.defaultPrevented || event?.isDefaultPrevented?.())
        return;
        
    panel.dataset.paneluniquename = uniqueName || panel.id || new Date().getTime().toString();
    function setHideBy(e: HTMLElement) {
        if (e === panel ||
            e.tagName === "LINK" ||
            e.tagName === "SCRIPT" ||
            e.classList.contains("hidden") ||
            e.dataset.hiddenby ||
            (container && e.parentElement !== container) && (e.offsetWidth <= 0 && e.offsetHeight <= 0))
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
    panel.classList.add("s-Panel");

    Fluent.trigger(panel, "panelopen", { bubbles: true });
}

function setPanelTitle(element: HTMLElement, title: string, closeButton?: boolean) {
    var panelRoot = element?.closest('.s-Panel') ?? element;
    if (!panelRoot)
        return;

    var pt = panelRoot.querySelector(':scope > .panel-titlebar');

    if (!pt) {
        pt = document.createElement("div");
        pt.classList.add("panel-titlebar");
        let ptt2 = pt.appendChild(document.createElement("div"));
        ptt2.classList.add("panel-titlebar-text");
        panelRoot.prepend(pt);
    }

    let ptt = pt.querySelector(".panel-titlebar-text");
    if (ptt)
        ptt.textContent = title ?? "";

    let ptc = pt.querySelector(".panel-titlebar-close");
    if (closeButton && !ptc) {
        ptc = document.createElement("button");
        ptc.classList.add("panel-titlebar-close");
        ptc.addEventListener("click", e => {
            closePanel((e.target as HTMLElement).closest<HTMLElement>(".s-Panel"));
        });
        pt.prepend(ptc);
    }
}

/** Returns .s-Panel, .modal, .ui-dialog */
export function getDialogRootElement(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement {
    if (isArrayLike(element))
        element = element[0];
    if (!element)
        return null;
    return element.closest(".modal, .s-Panel, .ui-dialog");

}

/** Returns .panel-body, .modal-body, .ui-dialog-content */
export function getDialogBodyElement(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement {
    if (isArrayLike(element))
        element = element[0];
    if (!element)
        return null;
    return element.closest(".modal-body, .panel-body, .ui-dialog-content") as HTMLElement ??
        getDialogRootElement(element)?.querySelector(":scope > .ui-dialog-content, .modal-body, :scope > .panel-body");

}

/** Returns .s-Panel, .modal, .ui-dialog-content */
export function getDialogEventTarget(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement {
    if (isArrayLike(element))
        element = element[0];
    if (!element)
        return null;
    return element.closest(".modal, .s-Panel, .ui-dialog-content") as HTMLElement ??
        element.closest(".ui-dialog")?.querySelector(":scope > .ui-dialog-content");

}

/** Tries to close a ui-dialog, panel or modal */
export function closeDialog(element: HTMLElement | ArrayLike<HTMLElement>): void {
    element = getDialogEventTarget(element);
    if (!element)
        return;
    if (element.classList.contains("ui-dialog-content")) {
        getjQuery()?.(element)?.dialog?.("close");
    }
    else if (element.classList.contains("modal")) {
        let $ = getjQuery();
        if ($ && $.fn && $.fn.modal)
            $(element).modal('hide');
        else if (isBS5Plus()) {
            (bootstrap as any).Modal?.getInstance?.(element)?.hide?.();
        }
    }
    else if (element.classList.contains("s-Panel")) {
        closePanel(element);
    }

}

export function attachToDialogBeforeCloseEvent(element: HTMLElement | ArrayLike<HTMLElement>, handler: (e: Event) => void) {
    element = getDialogEventTarget(element);
    if (!element)
        return;

    if (element.classList.contains("ui-dialog-content"))
        Fluent.on(element, "dialogbeforeclose", handler);
    else if (element.classList.contains("s-Panel"))
        Fluent.on(element, "panelbeforeclose", handler);
    else if (element.classList.contains("modal"))
        Fluent.on(element, "hide.bs.modal", handler);
}

export function attachToDialogCloseEvent(element: HTMLElement | ArrayLike<HTMLElement>, handler: (e: Event) => void) {
    element = getDialogEventTarget(element);
    if (!element)
        return;

    if (element.classList.contains("ui-dialog-content"))
        Fluent.on(element, "dialogclose", handler);
    else if (element.classList.contains("s-Panel"))
        Fluent.on(element, "panelclose", handler);
    else if (element.classList.contains("modal"))
        Fluent.on(element, "hidden.bs.modal", handler);
}