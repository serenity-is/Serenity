
namespace Q {
    export interface CommonDialogOptions extends JQueryUI.DialogOptions {
        onOpen?: () => void;
        onClose?: () => void;
        htmlEncode?: boolean;
        dialogClass?: string;
        title?: string;
    }

    export interface AlertOptions extends CommonDialogOptions {
        okButton?: string;
    }

    export function alert(message: string, options?: AlertOptions) {
        let dialog: JQuery;
        options = <Q.AlertOptions>$.extend({
            htmlEncode: true,
            okButton: text('Dialogs.OkButton'),
            title: text('Dialogs.AlertTitle'),
            onClose: null,
            onOpen: null,
            autoOpen: false,
            dialogClass: 's-MessageDialog s-AlertDialog',
            modal: true,
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (options.onClose)
                    options.onClose();
            }
        }, options);

        if (options.htmlEncode)
            message = htmlEncode(message);
        if (!options.buttons) {
            let buttons: any[] = [];
            buttons.push({
                text: options.okButton,
                click: function () {
                    dialog.dialog('close');
                }
            });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }

    export interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string;
        noButton?: string;
        cancelButton?: string;
        onCancel?: () => void;
        onNo?: () => void;
    }

    export function confirm(message: string, onYes: () => void, options?: ConfirmOptions): void {
        let dialog: JQuery;
        options = $.extend({
            htmlEncode: true,
            yesButton: text('Dialogs.YesButton'),
            noButton: text('Dialogs.NoButton'),
            title: text('Dialogs.ConfirmationTitle'),
            onNo: null,
            onCancel: null,
            onClose: null,
            autoOpen: false,
            modal: true,
            dialogClass: 's-MessageDialog s-ConfirmDialog',
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (!clicked && options.onCancel)
                    options.onCancel();
            },
            overlay: {
                opacity: 0.77,
                background: "black"
            }
        }, options);
        if (options.htmlEncode)
            message = htmlEncode(message);
        let clicked = false;
        if (!options.buttons) {
            let buttons: any[] = [];
            buttons.push({
                text: options.yesButton,
                click: function () {
                    clicked = true;
                    dialog.dialog('close');
                    if (onYes)
                        onYes();
                }
            });
            if (options.noButton)
                buttons.push({
                    text: options.noButton,
                    click: function () {
                        clicked = true;
                        dialog.dialog('close');
                        if (options.onNo)
                            options.onNo();
                        else if (options.onCancel)
                            options.onCancel();
                    }
                });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }

    export interface IFrameDialogOptions {
        html?: string;
    }

    export function iframeDialog(options: IFrameDialogOptions) {
        let doc: any;
        let e = $('<div><iframe></iframe></div>');
        let settings: IFrameDialogOptions = $.extend(<JQueryUI.DialogOptions>{
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
        confirm(message, onOk, $.extend<Q.ConfirmOptions>(
            {
                title: text("Dialogs.InformationTitle"),
                dialogClass: "s-MessageDialog s-InformationDialog",
                yesButton: text("Dialogs.OkButton"),
                noButton: null,
            }, options));
    }

    export function warning(message: string, options?: Q.AlertOptions) {
        alert(message, $.extend<Q.AlertOptions>(
            {
                title: text("Dialogs.WarningTitle"),
                dialogClass: "s-MessageDialog s-WarningDialog"
            }, options));
    }
}
