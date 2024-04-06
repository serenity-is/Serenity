import { Fluent, getjQuery, isArrayLike } from "../../base";
import { DataChangeInfo } from "../../types/datachangeinfo";
import { Widget } from "../widgets/widget";

export namespace SubDialogHelper {
    export function bindToDataChange(dialog: any, owner: Widget<any>,
        dataChange: (ev: DataChangeInfo) => void, useTimeout?: boolean): any {
        var uniqueName = (owner as Widget<any>)["uniqueName"];
        dialog.element.on('ondatachange.' + uniqueName, function (e: DataChangeInfo) {
            if (typeof e.operationType === "undefined" &&
                (e as any).originalEvent &&
                typeof (e as any).originalEvent.operationType !== "undefined")
                e = (e as any).originalEvent;
            if (useTimeout) {
                window.setTimeout(function () {
                    dataChange(e);
                }, 0);
            }
            else {
                dataChange(e);
            }
        }).one('remove.' + uniqueName, function () {
            Fluent.off(dialog.domNode, 'ondatachange.' + uniqueName);
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

    export function cascade(cascadedDialog: { domNode: HTMLElement }, ofElement: HTMLElement | ArrayLike<HTMLElement>): any {
        Fluent.one(cascadedDialog.domNode, 'dialogopen', function (e: Event) {
            var $ = getjQuery();
            if ($ && $.fn && $.fn.dialog) {
                $(cascadedDialog.domNode).dialog('option', 'position', cascadedDialogOffset(ofElement));
            }
        });
        return cascadedDialog;
    }

    export function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any {
        return { my: 'left top', at: 'left+20 top+20', of: isArrayLike(element) ? element[0] : element };
    }
}