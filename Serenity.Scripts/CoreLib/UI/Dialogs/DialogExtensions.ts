namespace Serenity.DialogExtensions {

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
}    