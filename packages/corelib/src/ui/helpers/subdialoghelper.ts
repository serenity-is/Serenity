import { Fluent, isArrayLike } from "@serenity-is/base";
import { DataChangeInfo } from "../../types/datachangeinfo";
import { Widget } from "../widgets/widget";

export namespace SubDialogHelper {
    export function bindToDataChange(dialog: any, owner: Widget<any>,
        dataChange: (ev: DataChangeInfo) => void, useTimeout?: boolean): any {
        var widgetName = (owner as any).widgetName;
        Fluent(dialog.domNode).on('ondatachange.' + widgetName, function (e: DataChangeInfo) {
            if (useTimeout) {
                window.setTimeout(function () {
                    dataChange(e);
                }, 0);
            }
            else {
                dataChange(e);
            }
        }).on('remove.' + widgetName, function () {
            Fluent.off(dialog.domNode, 'ondatachange.' + widgetName);
        });
        return dialog;
    }

    export function triggerDataChange(dialog: Widget<any>): any {
        Fluent.trigger(dialog.domNode, "ondatachange");
        return dialog;
    }

    export function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void {
        Fluent.trigger(isArrayLike(element) ? element[0] : element, "ondatachange");
    }

    export function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any {
        return bindToDataChange(dialog, owner, function (e) {
            Fluent.trigger(owner.domNode, 'ondatachange');
        }, useTimeout);
    }

    export function cascade(cascadedDialog: any, ofElement: HTMLElement | ArrayLike<HTMLElement>): any {
        Fluent.one(cascadedDialog.domNode, 'dialogopen', function (e: Event) {
            cascadedDialog.element.dialog().dialog('option', 'position', cascadedDialogOffset(ofElement));
        });
        return cascadedDialog;
    }

    export function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any {
        return { my: 'left top', at: 'left+20 top+20', of: isArrayLike(element) ? element[0] : element };
    }
}