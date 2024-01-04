import sQuery from "@optionaldeps/squery";
import { isArrayLike } from "@serenity-is/base";
import { DataChangeInfo } from "../../types/datachangeinfo";
import { Widget } from "../widgets/widget";

export namespace SubDialogHelper {
    export function bindToDataChange(dialog: any, owner: Widget<any>,
        dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any {
        var widgetName = (owner as any).widgetName;
        dialog.element.bind('ondatachange.' + widgetName, function (e: Event, dci: any) {
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
        sQuery(dialog.domNode).triggerHandler('ondatachange');
        return dialog;
    }

    export function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void {
        sQuery(element).triggerHandler('ondatachange');
    }

    export function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any {
        return bindToDataChange(dialog, owner, function (e, dci) {
            sQuery(owner.domNode).triggerHandler('ondatachange');
        }, useTimeout);
    }

    export function cascade(cascadedDialog: any, ofElement: HTMLElement | ArrayLike<HTMLElement>): any {
        cascadedDialog.element.one('dialogopen', function (e: Event) {
            cascadedDialog.element.dialog().dialog('option', 'position', cascadedDialogOffset(ofElement));
        });
        return cascadedDialog;
    }

    export function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any {
        return { my: 'left top', at: 'left+20 top+20', of: isArrayLike(element) ? element[0] : element };
    }
}