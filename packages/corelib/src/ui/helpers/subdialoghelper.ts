import { addDisposingListener } from "@serenity-is/domwise";
import { Fluent, getjQuery, isArrayLike } from "../../base";
import { DataChangeInfo } from "../../types/datachangeinfo";
import { Widget } from "../widgets/widget";

/**
 * Helper functions for coordinating data changes between dialogs and their
 * owner widgets.
 */
export namespace SubDialogHelper {
    /**
     * Binds a data change handler to a dialog so it is invoked when the dialog
     * raises a data change event.
     * @param dialog - The dialog to bind to.
     * @param owner - The owner widget.
     * @param dataChange - The handler invoked on data change.
     * @param useTimeout - Whether to invoke the handler asynchronously via a timeout.
     * @returns The dialog.
     */
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
        });
        addDisposingListener(dialog.node, function () {
            Fluent.off(dialog.domNode, 'ondatachange.' + uniqueName);
        }, dialog.uniqueName);
        return dialog;
    }

    /**
     * Triggers a data change event on the given dialog.
     * @param dialog - The dialog to trigger the event on.
     * @returns The dialog.
     */
    export function triggerDataChange(dialog: Widget<any>): any {
        Fluent.trigger(dialog.domNode, "ondatachange");
        return dialog;
    }

    /**
     * Triggers a data change event on the given element.
     * @param element - The element (or array-like of elements) to trigger the event on.
     */
    export function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void {
        Fluent.trigger(isArrayLike(element) ? element[0] : element, "ondatachange");
    }

    /**
     * Binds a dialog's data change event so it bubbles up to the owner widget.
     * @param dialog - The dialog to bind to.
     * @param owner - The owner widget to bubble the event to.
     * @param useTimeout - Whether to invoke the handler asynchronously via a timeout.
     * @returns The dialog.
     */
    export function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any {
        return bindToDataChange(dialog, owner, function (e) {
            Fluent.trigger(owner.domNode, 'ondatachange');
        }, useTimeout);
    }

    /**
     * Positions a cascaded dialog relative to the element that opened it.
     * @param cascadedDialog - The cascaded dialog to position.
     * @param ofElement - The element (or array-like of elements) the dialog is cascaded from.
     * @returns The cascaded dialog.
     */
    export function cascade(cascadedDialog: { domNode: HTMLElement }, ofElement: HTMLElement | ArrayLike<HTMLElement>): any {
        Fluent.one(cascadedDialog.domNode, 'dialogopen', function (e: Event) {
            var $ = getjQuery();
            if ($ && $.fn && $.fn.dialog) {
                $(cascadedDialog.domNode).dialog('option', 'position', cascadedDialogOffset(ofElement));
            }
        });
        return cascadedDialog;
    }

    /**
     * Returns the jQuery dialog position options used to cascade a dialog from
     * the given element.
     * @param element - The element (or array-like of elements) to cascade from.
     * @returns The dialog position options.
     */
    export function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any {
        return { my: 'left top', at: 'left+20 top+20', of: isArrayLike(element) ? element[0] : element };
    }
}