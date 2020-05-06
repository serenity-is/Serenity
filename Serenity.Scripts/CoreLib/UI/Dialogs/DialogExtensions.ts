namespace Serenity.DialogExtensions {
    export function dialogFlexify(dialog: JQuery): JQuery {
        var flexify = new Serenity.Flexify(dialog.closest('.ui-dialog'), {});
        return dialog;
    }

    export function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery {
        var dlg = dialog.dialog();
        dlg.dialog('option', 'resizable', true);
        if (mw != null) {
            dlg.dialog('option', 'minWidth', mw);
        }
        if (w != null) {
            dlg.dialog('option', 'width', w);
        }
        if (mh != null) {
            dlg.dialog('option', 'minHeight', mh);
        }
        if (h != null) {
            dlg.dialog('option', 'height', h);
        }
        return dialog;
    }

    export function dialogMaximizable(dialog: JQuery): JQuery {
        (dialog as any).dialogExtend({
            closable: true,
            maximizable: true,
            dblclick: 'maximize',
            icons: { maximize: 'ui-icon-maximize-window' }
        });

        return dialog;
    }

    export function dialogCloseOnEnter(dialog: JQuery): JQuery {
        dialog.bind('keydown', function (e) {
            if (e.which !== 13) {
                return;
            }
            var tagName = e.target.tagName.toLowerCase();
            if (tagName === 'button' || tagName === 'select' || tagName === 'textarea' ||
                tagName === 'input' && e.target.getAttribute('type') === 'button') {
                return;
            }
            var dlg = $(this);
            if (!dlg.hasClass('ui-dialog')) {
                dlg = dlg.closest('.ui-dialog');
            }
            var buttons = dlg.children('.ui-dialog-buttonpane').find('button');
            if (buttons.length > 0) {
                var defaultButton = buttons.find('.default-button');
                if (defaultButton.length > 0) {
                    buttons = defaultButton;
                }
            }
            var button = buttons.eq(0);
            if (!button.is(':disabled')) {
                e.preventDefault();
                button.trigger('click');
            }
        });
        return dialog;
    }
}    