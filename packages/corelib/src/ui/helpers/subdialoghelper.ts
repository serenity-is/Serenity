import { DataChangeInfo } from "../../types/datachangeinfo";
import { Widget } from "../widgets/widget";

export namespace SubDialogHelper {
    export function bindToDataChange(dialog: any, owner: Widget<any>,
        dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any {
        var widgetName = (owner as any).widgetName;
        dialog.element.bind('ondatachange.' + widgetName, function (e: JQueryEventObject, dci: any) {
            if (useTimeout) {
                window.setTimeout(function () {
                    dataChange(e, dci);
                }, 0);
            }
            else {
                dataChange(e, dci);
            }
        }).bind('remove.' + widgetName, function () {
            dialog.element.unbind('ondatachange.' + widgetName);
        });
        return dialog;
    }

    export function triggerDataChange(dialog: Widget<any>): any {
        dialog.element.triggerHandler('ondatachange');
        return dialog;
    }

    export function triggerDataChanged(element: JQuery): JQuery {
        element.triggerHandler('ondatachange');
        return element;
    }

    export function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any {
        return bindToDataChange(dialog, owner, function (e, dci) {
            owner.element.triggerHandler('ondatachange');
        }, useTimeout);
    }

    export function cascade(cascadedDialog: any, ofElement: JQuery): any {
        cascadedDialog.element.one('dialogopen', function (e: JQueryEventObject) {
            cascadedDialog.element.dialog().dialog('option', 'position', cascadedDialogOffset(ofElement));
        });
        return cascadedDialog;
    }

    export function cascadedDialogOffset(element: JQuery): any {
        return { my: 'left top', at: 'left+20 top+20', of: element[0] };
    }
}