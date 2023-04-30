import { Config } from "./config";
import { extend } from "./system";
import { htmlEncode, toggleClass } from "./html";
import { startsWith } from "./strings";
import { tryGetText } from "./localtext";
import $ from "@optionaldeps/jquery";

/**
 * Options for a message dialog button
 */
export interface DialogButton {
    /** Button text */
    text?: string;
    /** Button hint */
    hint?: string;
    /** Button icon */
    icon?: string;
    /** Click handler */
    click?: (e: MouseEvent) => void;
    /** CSS class for button */
    cssClass?: string;
    /** HTML encode button text. Default is true. */
    htmlEncode?: boolean;
    /** The code that is returned from message dialog function when this button is clicked */
    result?: string;
}

/**
 * Options that apply to all message dialog types
 */
export interface CommonDialogOptions {
    /** Event handler that is called when dialog is opened */
    onOpen?: () => void;
    /** Event handler that is called when dialog is closed */
    onClose?: (result: string) => void;
    /** Dialog title */
    title?: string;
    /** HTML encode the message, default is true */
    htmlEncode?: boolean;
    /** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
    preWrap?: boolean;
    /** Dialog css class. Default is based on the message dialog type */
    dialogClass?: string;
    /** List of buttons to show on the dialog */
    buttons?: DialogButton[];
    /** Class to use for the modal element for Bootstrap dialogs */
    modalClass?: string;
    /** True to use Bootstrap dialogs even when jQuery UI  present, default is based on `Q.Config.bootstrapMessages */
    bootstrap?: boolean;
    /** The result code of the button used to close the dialog is returned via this variable in the options object */
    result?: string;
}

// if both jQuery UI and bootstrap button exists, prefer jQuery UI button as UI dialog needs them
if (typeof $ !== "undefined" && ($.fn as any)?.button && ($ as any).ui?.button && ($ as any).fn?.button?.noConflict) {
    ($.fn as any).btn = ($ as any).fn.button.noConflict();
}

function toIconClass(icon: string): string {
    if (!icon)
        return null;
    if (startsWith(icon, 'fa-'))
        return 'fa ' + icon;
    if (startsWith(icon, 'glyphicon-'))
        return 'glyphicon ' + icon;
    return icon;
}

function uiDialogMessage(options: CommonDialogOptions, message: string, dialogClass: string) {
    var opt = extend(<any>{
        modal: true,
        width: '40%',
        maxWidth: 450,
        minWidth: 180,
        resizable: false,
        open: function () {
            if (options.onOpen)
                options.onOpen.call(this);
        },
        close: function () {
            ($(this) as any).dialog?.('destroy');
            if (options.onClose)
                options.onClose.call(this, options.result);
        }
    }, options);

    opt.dialogClass = 's-MessageDialog' + (dialogClass ? (' ' + dialogClass) : '');

    if (options.buttons) {
        opt.buttons = options.buttons.map(x => {
            var btn = dialogButtonToUI(x);
            btn.click = function (e: any) {
                options.result = x.result;
                ($(this) as any).dialog('close');
                x.click && x.click.call(this, e);
            }
            return btn;
        });
    }

    return ($('<div>' + message + '</div>') as any).dialog(opt);
}

let _isBS3: boolean;
let _isBS5Plus: boolean;

/** Returns true if Bootstrap 3 is loaded */
export function isBS3(): boolean {
    if (_isBS3 != null)
        return _isBS3;
    // @ts-ignore
    return (_isBS3 = !!($.fn.modal && $.fn.modal.Constructor && $.fn.modal.Constructor.VERSION && ($.fn.modal.Constructor.VERSION + "").charAt(0) == '3'));
}

/** Returns true if Bootstrap 5+ is loaded */
export function isBS5Plus(): boolean {
    if (_isBS5Plus != null)
        return _isBS5Plus;
    // @ts-ignore
    return (_isBS5Plus = typeof bootstrap !== "undefined" && (!bootstrap.Modal || !bootstrap.Modal.VERSION || (!bootstrap.Modal.VERSION + "").charAt(0) != '4'));
}

const defaultTxt: Record<string, string> = {
    AlertTitle: 'Alert',
    InformationTitle: 'Information',
    WarningTitle: 'Warning',
    ConfirmationTitle: 'Confirm',
    OkButton: 'OK',
    YesButton: 'Yes',
    NoButton: 'No',
    CancelButton: 'Cancel',
    CloseButton: 'Close'
};

function txt(k: string) {
    return htmlEncode(tryGetText("Dialogs." + k) ?? defaultTxt[k]);
}

/** 
 * Builds HTML DIV element for a Bootstrap modal dialog
 * @param title Modal title
 * @param body Modal body, it will not be HTML encoded, so make sure it is encoded 
 * @param modalClass Optional class to add to the modal element
 * @param escapeHtml True to html encode body, default is true
 * @returns 
 */
export function bsModalMarkup(title: string, body: string, modalClass?: string, escapeHtml: boolean = true): HTMLDivElement {
    var closeButton = `<button type="button" class="${isBS5Plus() ? "btn-" : ""}close" data-${isBS5Plus() ? "bs-" : ""}dismiss="modal" aria-label="${txt('CloseButton')}">` +
        `${isBS5Plus() ? "" : '<span aria-hidden="true">&times;</span>'}</button>`;
    var div = document.createElement("div");
    div.classList.add("modal");
    if (modalClass?.length)
        toggleClass(div, modalClass, true);
    div.setAttribute("tabindex", "-1");
    div.setAttribute("role", "dialog");
    div.innerHTML = `<div class="modal-dialog" role="document">
    <div class="modal-content">
        <div class="modal-header">
            ${(isBS3() ? closeButton : "")}<h5 class="modal-title">${htmlEncode(title)}</h5>${(isBS3() ? "" : closeButton)}
        </div>
        <div class="modal-body">${escapeHtml ? htmlEncode(body) : body}</div>
        <div class="modal-footer"></div>
    </div>
</div>`;
    return div;
}

export function dialogButtonToBS(x: DialogButton): HTMLButtonElement {
    var text = x.htmlEncode == null || x.htmlEncode ? htmlEncode(x.text) : x.text;
    var iconClass = toIconClass(x.icon);
    if (iconClass != null)
        text = '<i class="' + htmlEncode(iconClass) + "><i>" + (text ? (" " + text) : "");
    var button = document.createElement("button");
    button.classList.add("btn");
    if (x.cssClass?.length)
        toggleClass(button, x.cssClass, true);
    if (x.hint?.length)
        button.setAttribute("title", x.hint);
    button.innerHTML = text;
    return button;
}

export function dialogButtonToUI(x: DialogButton): any {
    var text = x.htmlEncode == null || x.htmlEncode ? htmlEncode(x.text) : x.text;
    var iconClass = toIconClass(x.icon);
    if (iconClass != null)
        text = '<i class="' + iconClass + "><i> ";
    return <any>{
        text: text,
        attr: !x.cssClass ? undefined : {
            "class": x.cssClass
        },
        click: x.click
    }
}

function bsModalMessage(options: CommonDialogOptions, message: string, modalClass: string, escapeHtml: boolean = true) {
    var modalDiv = bsModalMarkup(options.title, message, "s-MessageModal" + (modalClass ? (" " + modalClass) : ""), escapeHtml);
    var footer = modalDiv.querySelector('.modal-footer') as HTMLElement;
    var rawBS5 = typeof $ === "undefined" && isBS5Plus();

    function createButton(x: DialogButton) {

        if (!footer) {
            return;
        }

        var button = dialogButtonToBS(x);
        footer.append(button);
        button.addEventListener("click", e => {
            options.result = x.result;
            if (rawBS5) {
                // @ts-ignore 
                bootstrap.Modal.getInstance(modalDiv)?.hide();
            }
            else 
                ($(modalDiv) as any).modal('hide');
            x.click && x.click.call(this, e);
        });
    }

    if (options.buttons) {
        for (var button of options.buttons) {
            createButton(button);
        }
    }

    if (rawBS5) {
        if (options.onOpen)
            modalDiv.addEventListener("shown.bs.modal", options.onOpen);

        modalDiv.addEventListener('hidden.bs.modal', e => {
            try {
                options.onClose && options.onClose(options.result);
            }
            finally {
                modalDiv.remove();
            }
        });

        document.body.appendChild(modalDiv);
        // @ts-ignore
        new bootstrap.Modal(modalDiv, {
            backdrop: false,
        }).show();
    }
    else {
        var div = $(modalDiv).appendTo(document.body);

        if (options.onOpen)
            div.one('shown.bs.modal', options.onOpen);

        div.one('hidden.bs.modal', e => {
            try {
                options.onClose && options.onClose(options.result);
            }
            finally {
                div.remove();
            }
        });

        if (isBS5Plus()) {

            // @ts-ignore
            (div as any).modal({
                backdrop: false,
            });
            (div as any).modal('show');
        }
        else {
            (div as any).modal({
                backdrop: false,
                show: true
            } as any);
        }

        if (isBS5Plus())
            (div as any).modal('show');
    }
}

let _useBrowserDialogs: boolean;
function useBrowserDialogs() {
    if (_useBrowserDialogs == null) {
        _useBrowserDialogs = !isBS5Plus() && (typeof $ === 'undefined' || ((!($ as any).ui || !($ as any).ui?.dialog) && (!$.fn || !(($.fn as any).modal))));
    }
    return _useBrowserDialogs;
}

function useBSModal(options: CommonDialogOptions): boolean {
    return !!((!($ as any).ui || !($ as any).ui?.dialog) || Config.bootstrapMessages || (options && options.bootstrap));
}

function messageHtml(message: string, options?: CommonDialogOptions): string {
    var encode = options == null || options.htmlEncode == null || options.htmlEncode;
    if (encode)
        message = htmlEncode(message);

    var preWrap = options == null || (options.preWrap == null && encode) || options.preWrap;
    return '<div class="message"' + (preWrap ? ' style="white-space: pre-wrap">' : '>') + message + '</div>';
}

/**
 * Additional options for Alert dialogs
 */
export interface AlertOptions extends CommonDialogOptions {
    /** The title of OK button, or false to hide the OK button */
    okButton?: string | boolean;
    /** CSS class for OK button */
    okButtonClass?: string;
}

/** 
 * Displays an alert dialog 
 * @param message The message to display
 * @param options Additional options. 
 * @see AlertOptions 
 * @example 
 * alertDialog("An error occured!"); }
 */
export function alertDialog(message: string, options?: AlertOptions) {

    if (useBrowserDialogs()) {
        window.alert(message);
        return;
    }

    let useBS = useBSModal(options);

    options = extend({
        htmlEncode: true,
        okButton: txt('OkButton'),
        title: txt('AlertTitle')
    }, options);

    if (options.buttons == null) {
        options.buttons = [];
        if (options.okButton == null || options.okButton) {
            options.buttons.push({
                text: typeof options.okButton == "boolean" ? txt('OkButton') : options.okButton,
                cssClass: options.okButtonClass ?? (useBS ? 'btn-danger' : undefined),
                result: 'ok'
            });
        }
    }

    message = messageHtml(message, options);

    if (useBS)
        bsModalMessage(options, message, options.modalClass ?? "s-AlertModal", false);
    else
        uiDialogMessage(options, message, options.dialogClass ?? "s-AlertDialog");
}

/** @obsolete use alertDialog */
export const alert = alertDialog;

/** Additional options for confirm dialog */
export interface ConfirmOptions extends CommonDialogOptions {
    /** Title of the Yes button, or false to hide the Yes button. Default is value of local text: "Dialogs.YesButton" */
    yesButton?: string | boolean;
    /** CSS class for the Yes button. */
    yesButtonClass?: string;
    /** Title of the NO button, or false to hide the No button. Default is value of local text: "Dialogs.NoButton" */
    noButton?: string | boolean;
    /** Title of the CANCEL button, or false to hide the Cancel button. Default is value of local text: "Dialogs.NoButton" */
    cancelButton?: string | boolean;
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
export function confirmDialog(message: string, onYes: () => void, options?: ConfirmOptions): void {
    if (useBrowserDialogs()) {
        if (window.confirm(message))
            onYes && onYes();
        return;
    }

    let useBS = useBSModal(options);

    options = extend(<ConfirmOptions>{
        htmlEncode: true,
        yesButton: txt('YesButton'),
        noButton: txt('NoButton'),
        title: txt('ConfirmationTitle')
    }, options);

    if (options.buttons == null) {
        options.buttons = [];
        if (options.yesButton == null || options.yesButton) {
            options.buttons.push({
                text: typeof options.yesButton == "boolean" ? txt('YesButton') : options.yesButton,
                cssClass: options.yesButtonClass ?? (useBS ? 'btn-primary' : undefined),
                result: 'yes',
                click: onYes
            });
        }
        if (options.noButton == null || options.noButton) {
            options.buttons.push({
                text: typeof options.noButton == "boolean" ? txt('NoButton') : options.noButton,
                cssClass: useBS ? (isBS5Plus() ? 'btn-danger' : 'btn-default') : undefined,
                result: 'no',
                click: options.onNo
            });
        }
        if (options.cancelButton) {
            options.buttons.push({
                text: typeof options.cancelButton == "boolean" ? txt('CancelButton') : options.cancelButton,
                cssClass: useBS ? (isBS5Plus() ? 'btn-secondary' : 'btn-default') : undefined,
                result: 'cancel',
                click: options.onCancel
            });
        }
    }

    message = messageHtml(message, options);
    if (useBS)
        bsModalMessage(options, message, options.modalClass ?? "s-ConfirmModal");
    else
        uiDialogMessage(options, message, options.dialogClass ?? "s-ConfirmDialog");
}

/** @obsolete use confirmDialog */
export const confirm = confirmDialog;

/** Options for `iframeDialog` **/
export interface IFrameDialogOptions {
    html?: string;
}

/** 
 * Display a dialog that shows an HTML block, which is usually returned from server callbacks in an IFRAME 
 * @param options The options
 */
export function iframeDialog(options: IFrameDialogOptions) {

    if (useBrowserDialogs()) {
        window.alert(options.html);
        return;
    }

    if (useBSModal(options as any)) {
        bsModalMessage({
            title: txt('AlertTitle'),
            modalClass: 'modal-lg',
            onOpen: function () {
                doc = (<HTMLIFrameElement>($(this).find('iframe').css({
                    border: 'none',
                    width: '100%',
                    height: '100%'
                })[0])).contentDocument;
                doc.open();
                doc.write(options.html);
                doc.close();
            }
        }, '<div style="overflow: hidden"><iframe></iframe></div>', 's-IFrameModal');
        return;
    }

    let doc: any;
    let e = $('<div style="overflow: hidden"><iframe></iframe></div>');
    let settings: IFrameDialogOptions = extend<any>({
        autoOpen: true,
        modal: true,
        width: '60%',
        height: '400',
        title: txt('AlertTitle'),
        open: function () {
            doc = (<HTMLIFrameElement>(e.find('iframe').css({
                border: 'none',
                width: '100%',
                height: '100%'
            })[0])).contentDocument;
            doc.open();
            doc.write(settings.html);
            doc.close();
        },
        close: function () {
            doc.open();
            doc.write('');
            doc.close();
            (e as any).dialog?.('destroy').html('');
        }
    }, options);
    (e as any).dialog?.(settings as any);
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
export function informationDialog(message: string, onOk?: () => void, options?: ConfirmOptions) {
    if (useBrowserDialogs()) {
        window.alert(message);
        onOk && onOk();
        return;
    }

    confirmDialog(message, onOk, extend<ConfirmOptions>({
        title: txt("InformationTitle"),
        dialogClass: "s-InformationDialog",
        modalClass: "s-InformationModal",
        yesButton: txt("OkButton"),
        yesButtonClass: 'btn-info',
        noButton: false,
    }, options));
}

/** @obsolete use informationDialog */
export const information = informationDialog;

/** 
 * Display a success dialog 
 * @param message The message to display
 * @param onOk Callback for OK button click 
 * @param options Additional options. 
 * @see ConfirmOptions 
 * @example 
 * successDialog("Operation complete", () => { 
 *     // do something when OK is clicked
 * }
 */
export function successDialog(message: string, onOk?: () => void, options?: ConfirmOptions) {
    if (useBrowserDialogs()) {
        window.alert(message);
        onOk && onOk();
        return;
    }

    confirmDialog(message, onOk, extend<ConfirmOptions>({
        title: txt("SuccessTitle"),
        dialogClass: "s-SuccessDialog",
        modalClass: "s-SuccessModal",
        yesButton: txt("OkButton"),
        yesButtonClass: 'btn-success',
        noButton: false,
    }, options));
}

/** @obsolete use successDialog */
export const success = successDialog;

/** 
 * Display a warning dialog 
 * @param message The message to display
 * @param options Additional options. 
 * @see AlertOptions 
 * @example 
 * warningDialog("Something is odd!");
 */
export function warningDialog(message: string, options?: AlertOptions) {
    alertDialog(message, extend<AlertOptions>({
        title: txt("WarningTitle"),
        dialogClass: "s-WarningDialog",
        modalClass: "s-WarningModal",
        okButtonClass: 'btn-warning'
    }, options));
}

/** @obsolete use warningDialog */
export const warning = warningDialog;

/** 
 * Closes a panel, triggering panelbeforeclose and panelclose events.
 * If the panelbeforeclose prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param e  The event triggering the close
 */
export function closePanel(element: JQuery, e?: JQueryEventObject) {

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

    e = $.Event(e as any);
    (e as any).type = 'panelclose';
    (e as any).target = element[0];
    element.trigger(e);
}