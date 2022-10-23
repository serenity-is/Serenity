import { Config } from "./config";
import { extend } from "./system";
import { attrEncode, htmlEncode } from "./html";
import { startsWith } from "./strings";
import { tryGetText } from "./localtext";

export interface DialogButton {
    text?: string;
    hint?: string;
    icon?: string;
    click?: (e: JQueryEventObject) => void;
    cssClass?: string;
    htmlEncode?: boolean;
    result?: string;
}

export interface CommonDialogOptions  {
    onOpen?: () => void;
    onClose?: (result: string) => void;
    title?: string;
    htmlEncode?: boolean;
    preWrap?: boolean;
    dialogClass?: string;
    buttons?: DialogButton[];
    modalClass?: string;
    bootstrap?: boolean;
    result?: string;
}

export interface AlertOptions extends CommonDialogOptions {
    okButton?: string | boolean;
    okButtonClass?: string;
}


// if both jQuery UI and bootstrap button exists, prefer jQuery UI button as UI dialog needs them
if (typeof $ !== "undefined" && $.fn && $.fn.button && $.ui && $.ui.button && ($.fn.button as any).noConflict) {
    ($.fn as any).btn = ($.fn.button as any).noConflict();
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
    var opt = extend(<JQueryUI.DialogOptions>{
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
            $(this).dialog('destroy');
            if (options.onClose)
                options.onClose.call(this, options.result);
        }
    }, options);

    opt.dialogClass = 's-MessageDialog' + (dialogClass ? (' ' + dialogClass): '');

    if (options.buttons) {
        opt.buttons = options.buttons.map(x => {
            var btn = dialogButtonToUI(x);
            btn.click = function(e) {
                options.result = x.result;
                $(this).dialog('close');
                x.click && x.click.call(this, e);
            }
            return btn;
        });
    }

    return $('<div>' + message + '</div>').dialog(opt);
}

let _isBS3: boolean;
let _isBS5Plus: boolean;

export function isBS3(): boolean {
    if (_isBS3 != null)
        return _isBS3;
    // @ts-ignore
    return (_isBS3 = !!($.fn.modal && $.fn.modal.Constructor && $.fn.modal.Constructor.VERSION && ($.fn.modal.Constructor.VERSION + "").charAt(0) == '3'));
}

export function isBS5Plus(): boolean {
    if (_isBS5Plus != null)
        return _isBS5Plus;
    // @ts-ignore
    return (_isBS5Plus = typeof bootstrap !== "undefined" && (!bootstrap.Modal || !bootstrap.Modal.VERSION || (!bootstrap.Modal.VERSION + "").charAt(0) != '4'));
}

const defaultTxt = {
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
    return tryGetText("Dialogs." + k) ?? defaultTxt[k];
}

export function bsModalMarkup(title: string, body: string, modalClass?: string) {
    var closeButton = `<button type="button" class="${isBS5Plus() ? "btn-" : ""}close" data-${isBS5Plus() ? "bs-" :  ""}dismiss="modal" aria-label="${txt('CloseButton')}">` + 
        `${isBS5Plus() ? "" : '<span aria-hidden="true">&times;</span>'}</button>`;
    return (
`<div class="modal ${modalClass}" tabindex="-1" role="dialog">
<div class="modal-dialog" role="document">
    <div class="modal-content">
        <div class="modal-header">
            ${(isBS3() ? closeButton : "")}<h5 class="modal-title">${title}</h5>${(isBS3() ? "" : closeButton)}
        </div>
        <div class="modal-body">${body}</div>
        <div class="modal-footer"></div>
    </div>
</div>
</div>`);
}

export function dialogButtonToBS(x: DialogButton) {
    var text = x.htmlEncode == null || x.htmlEncode ? htmlEncode(x.text) : x.text;
    var iconClass = toIconClass(x.icon);
    if (iconClass != null)
        text = '<i class="' + iconClass + "><i>" + (text ? (" " + text) : "");            
    return `<button class="btn ${x.cssClass ? x.cssClass : ''}"${x.hint ? (' title="' + attrEncode(x.hint) + '"') : '' }>${text}</button>`;
}

export function dialogButtonToUI(x: DialogButton) {
    var text = x.htmlEncode == null || x.htmlEncode ? htmlEncode(x.text) : x.text;
    var iconClass = toIconClass(x.icon);
    if (iconClass != null)
        text = '<i class="' + iconClass + "><i> ";
    return <JQueryUI.DialogButtonOptions>{
        text: text,
        attr: !x.cssClass ? undefined : {
            "class": x.cssClass
        },
        click: x.click
    }
}

function bsModalMessage(options: CommonDialogOptions, message: string, modalClass: string) {
    var markup = bsModalMarkup(options.title, message, "s-MessageModal" + (modalClass ? (" " + modalClass) : ""));
    var div = $(markup).eq(0).appendTo(document.body);
    
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

    var footer = div.find('.modal-footer');

    function createButton(x: DialogButton) {
        return $(dialogButtonToBS(x))
            .appendTo(footer)
            .click(e => {
                options.result = x.result;
                (div as any).modal('hide');
                x.click && x.click.call(this, e);       
            });
    }

    if (options.buttons)
        for (var button of options.buttons) 
            createButton(button);

    if (isBS5Plus()) {
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

let _useBrowserDialogs: boolean;
function useBrowserDialogs() {
    if (_useBrowserDialogs == null) {
        _useBrowserDialogs = typeof $ === 'undefined' || ((!$.ui || !$.ui.dialog) && (!$.fn || !(($.fn as any).modal)));
    }
    return _useBrowserDialogs;
}

function useBSModal(options: CommonDialogOptions): boolean {
    return !!((!$.ui || !$.ui.dialog) || Config.bootstrapMessages || (options && options.bootstrap));
}

function messageHtml(message: string, options?: CommonDialogOptions): string {
    var encode = options == null || options.htmlEncode == null || options.htmlEncode;
    if (encode)
        message = htmlEncode(message); 

    var preWrap = options == null || (options.preWrap == null && encode) || options.preWrap;
    return '<div class="message"' + (preWrap ? ' style="white-space: pre-wrap">' : '>') + message + '</div>';
}

export function alert(message: string, options?: AlertOptions) {

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
        bsModalMessage(options, message, options.modalClass ?? "s-AlertModal");
    else
        uiDialogMessage(options, message, options.dialogClass ?? "s-AlertDialog");
}

export interface ConfirmOptions extends CommonDialogOptions {
    yesButton?: string | boolean;
    yesButtonClass?: string;
    noButton?: string | boolean;
    cancelButton?: string | boolean;
    onCancel?: () => void;
    onNo?: () => void;
}

export function confirm(message: string, onYes: () => void, options?: ConfirmOptions): void {
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
                cssClass: useBS ? (isBS5Plus() ? 'btn-danger' : 'btn-default'): undefined,
                result: 'no',
                click: options.onNo
            });
        }
        if (options.cancelButton) {
            options.buttons.push({
                text: typeof options.cancelButton == "boolean" ? txt('CancelButton') : options.cancelButton,
                cssClass: useBS ? (isBS5Plus() ? 'btn-secondary': 'btn-default') : undefined,
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

export interface IFrameDialogOptions {
    html?: string;
}

export function iframeDialog(options: IFrameDialogOptions) {

    if (useBrowserDialogs()) {
        window.alert(options.html);
        return;
    }

    if (useBSModal(options as any)) {
        bsModalMessage({
            title: txt('AlertTitle'),
            modalClass: 'modal-lg',
            onOpen: function() {
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
    let settings: IFrameDialogOptions = extend<any>(<JQueryUI.DialogOptions>{
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
            e.dialog('destroy').html('');
        }
    }, options);
    e.dialog(settings as any);
}

export function information(message: string, onOk: () => void, options?: ConfirmOptions) {
    if (useBrowserDialogs()) {
        window.alert(message);
        onOk && onOk();
        return;
    }

    confirm(message, onOk, extend<ConfirmOptions>({
        title: txt("InformationTitle"),
        dialogClass: "s-InformationDialog",
        modalClass: "s-InformationModal",
        yesButton: txt("OkButton"),
        yesButtonClass: 'btn-info',
        noButton: false,
    }, options));
}


export function success(message: string, onOk: () => void, options?: ConfirmOptions) {
    if (useBrowserDialogs()) {
        window.alert(message);
        onOk && onOk();
        return;
    }

    confirm(message, onOk, extend<ConfirmOptions>({
        title: txt("SuccessTitle"),
        dialogClass: "s-SuccessDialog",
        modalClass: "s-SuccessModal",
        yesButton: txt("OkButton"),
        yesButtonClass: 'btn-success',
        noButton: false,
    }, options));
}

export function warning(message: string, options?: AlertOptions) {
    alert(message, extend<AlertOptions>({
        title: txt("WarningTitle"),
        dialogClass: "s-WarningDialog",
        modalClass: "s-WarningModal",
        okButtonClass: 'btn-warning'
    }, options));
}

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