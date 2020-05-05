
namespace Q {

    export interface DialogButton {
        title?: string;
        hint?: string;
        icon?: string;
        onClick?: (e: JQueryEventObject) => void;
        cssClass?: string;
        htmlEncode?: boolean;
        result?: string;
    }

    export interface CommonDialogOptions  {
        onOpen?: () => void;
        onClose?: (result: string) => void;
        title?: string;
        message?: string;
        htmlEncode?: boolean;
        preWrap?: boolean;
        dialogClass?: string;
        buttons?: DialogButton[];
        modalClass?: string;
    }

    export interface AlertOptions extends CommonDialogOptions {
        okButton?: string | boolean;
    }

    function toIconClass(icon: string): string {
        if (!icon)
            return null;
        if (Q.startsWith(icon, 'fa-'))
           return 'fa ' + icon;
        if (Q.startsWith(icon, 'glyphicon-')) 
            return 'glyphicon ' + icon;
        return icon;
    }

    function uiDialog(options: CommonDialogOptions, message: string, dialogClass: string) {
        var opt = Q.extend(<JQueryUI.DialogOptions>{
            modal: true,
            dialogClass: 's-MessageDialog' + (dialogClass ? (' ' + dialogClass): ''),
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
                    options.onClose.call(this, options["result"]);
            }
        }, options);

        if (options.buttons) {
            opt.buttons = options.buttons.map(x => {
                var text = x.htmlEncode == null || x.htmlEncode ? Q.htmlEncode(x.title) : x.title;
                var iconClass = toIconClass(x.icon);
                if (iconClass != null)
                    text = '<i class="' + iconClass + "><i> ";
                return <JQueryUI.DialogButtonOptions>{
                    text: text,
                    click: function(e) {
                        options["result"] = x.result;
                        $(this).dialog('close');
                        x.onClick && x.onClick.call(this, e);
                    },
                    attr: !x.cssClass ? undefined : {
                        "class": x.cssClass
                    }
                 }
            });
        }

        return $('<div><div class="message"></div></div>')
            .children('.message')
            .html(message)
            .dialog(options);
    }

    let _isBS3: boolean;
    function isBS3() {
        if (_isBS3 != null)
            return _isBS3;
        // @ts-ignore
        return (_isBS3 = !!($.fn.modal && $.fn.modal.VERSION && $.fn.modal.VERSION.charAt(0) == '3'));
    }

    function bsModal(options: CommonDialogOptions, message: string, modalClass: string) {
        var closeButton = `<button type="button" class="close" data-dismiss="modal" aria-label="${Q.text('Dialogs.CloseButton')}">` + 
            `<span aria-hidden="true">&times;</span></button>`;
        var div = $(
`<div class="modal s-MessageModal ${modalClass}" tabindex="-1" role="dialog">
    <div class="modal-dialog ${options.dialogClass}" role="document">
        <div class="modal-content">
            <div class="modal-header">
                ${(isBS3 ? closeButton : "")}<h5 class="modal-title">${options.title}</h5>${(isBS3 ? "" : closeButton)}
            </div>
            <div class="modal-body">
                ${message}
            </div>
            <div class="modal-footer">${(options.buttons || []).map(x => {
                var text = x.htmlEncode == null || x.htmlEncode ? Q.htmlEncode(x.title) : x.title;
                var iconClass = toIconClass(x.icon);
                if (iconClass != null)
                    text = '<i class="' + iconClass + "><i>" + (text ? (" " + text) : "");
                return `<button class="btn ${x.cssClass ? x.cssClass : ''}"${x.hint ? (' title="' + Q.attrEncode(x.hint) + '"') : '' }>${text}</button>`;
            }).join('\n')}</div>
        </div>
    </div>
</div>`).eq(0).appendTo(document.body);
        
        if (options.onOpen)
            div.one('shown.bs.modal', options.onOpen);

        if (options.onClose)
            div.one('hidden.bs.modal', options.onOpen);

        div["modal"]('open');
    }

    let _useBrowserDialogs: boolean;
    function useBrowserDialogs() {
        if (_useBrowserDialogs == null) {
            _useBrowserDialogs = typeof $ === 'undefined' || ((!$.ui || !$.ui.dialog) && (!$.fn || !$.fn["modal"]));
        }
        return _useBrowserDialogs;
    }

    function useBSModal(options: CommonDialogOptions): boolean {
        return !!((!$.ui || !$.ui.dialog) || options.modalClass);
    }

    export function alert(message: string, options?: AlertOptions) {

        if (useBrowserDialogs()) {
            window.alert(message);
            return;
        }

        let useBS = useBSModal(options);

        options = Q.extend({
            htmlEncode: true,
            okButton: text('Dialogs.OkButton'),
            title: text('Dialogs.AlertTitle')
        }, options);

        if (options.buttons == null) {
            options.buttons = [];
            if (options.okButton == null || options.okButton) {
                options.buttons.push({
                    title: typeof options.okButton == "boolean" ? text('Dialogs.OkButton') : options.okButton,
                    cssClass: useBS ? 'btn-default' : undefined,
                    result: 'ok'
                });
            }
        }

        var message = options.message;
        if (options.htmlEncode != null && !options.htmlEncode)
            message = Q.htmlEncode(message); 

        if (options.preWrap)
            message = '<div style="white-space: pre-wrap">' + message + '</div>';

        if (useBS)
            bsModal(options, message, options.modalClass ?? "s-AlertModal");
        else
            uiDialog(options, message, options.dialogClass ?? "s-AlertDialog");
    }

    export interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string | boolean;
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

        options = Q.extend(<ConfirmOptions>{
            htmlEncode: true,
            yesButton: text('Dialogs.YesButton'),
            noButton: text('Dialogs.NoButton'),
            title: text('Dialogs.ConfirmationTitle')
        }, options);

        if (options.buttons == null) {
            options.buttons = [];
            if (options.yesButton == null || options.yesButton) {
                options.buttons.push({
                    title: typeof options.yesButton == "boolean" ? text('Dialogs.YesButton') : options.yesButton,
                    cssClass: useBS ? 'btn-success' : undefined,
                    result: 'yes',
                    onClick: onYes
                });
            }
            if (options.noButton == null || options.noButton) {
                options.buttons.push({
                    title: typeof options.noButton == "boolean" ? text('Dialogs.NoButton') : options.noButton,
                    cssClass: useBS ? 'btn-warning' : undefined,
                    result: 'no',
                    onClick: options.onNo
                });
            }
            if (options.cancelButton) {
                options.buttons.push({
                    title: typeof options.cancelButton == "boolean" ? text('Dialogs.CancelButton') : options.cancelButton,
                    cssClass: useBS ? 'btn-default' : undefined,
                    result: 'cancel',
                    onClick: options.onCancel
                });
            }
        }

        var message = options.message;
        if (options.htmlEncode != null && !options.htmlEncode)
            message = Q.htmlEncode(message); 

        if (options.preWrap)
            message = '<div style="white-space: pre-wrap">' + message + '</div>';

        if (useBS)
            bsModal(options, message, options.modalClass ?? "s-ConfirmModal");
        else
            uiDialog(options, message, options.dialogClass ?? "s-ConfirmDialog");
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
            bsModal({
                title: text('Dialogs.AlertTitle'),
                modalClass: 'modal-lg',
                onOpen: function() {
                    doc = (<HTMLIFrameElement>($(this).find('iframe').css({
                        border: 'none',
                        width: '100%',
                        height: '100%'
                    })[0])).contentDocument;
                    doc.open();
                    doc.write(settings.html);
                    doc.close();
                }
            }, '<div style="overflow: hidden"><iframe></iframe></div>', 's-IFrameModal');
            return;
        }

        let doc: any;
        let e = $('<div style="overflow: hidden"><iframe></iframe></div>');
        let settings: IFrameDialogOptions = Q.extend<any>(<JQueryUI.DialogOptions>{
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: text('Dialogs.AlertTitle'),
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

    export function information(message: string, onOk: () => void, options?: Q.ConfirmOptions) {
        if (useBrowserDialogs()) {
            window.alert(message);
            onOk && onOk();
            return;
        }

        confirm(message, onOk, Q.extend<Q.ConfirmOptions>(
            {
                title: text("Dialogs.InformationTitle"),
                dialogClass: "s-MessageDialog s-InformationDialog",
                yesButton: text("Dialogs.OkButton"),
                noButton: false,
            }, options));
    }

    export function warning(message: string, options?: Q.AlertOptions) {
        alert(message, Q.extend<Q.AlertOptions>(
            {
                title: text("Dialogs.WarningTitle"),
                dialogClass: "s-MessageDialog s-WarningDialog"
            }, options));
    }
}
