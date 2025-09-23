import { Dialog, Fluent, confirmDialog, isArrayLike, localText } from "@serenity-is/corelib";
import { DialogUtilsTexts } from "../ServerTypes/Texts";

export namespace DialogUtils {

    export function pendingChangesConfirmation(element: ArrayLike<HTMLElement> | HTMLElement, hasPendingChanges: () => boolean) {

        var el = isArrayLike(element) ? element[0] : element;
        if (!el)
            return;
        Dialog.onClose(el, (result, e: Event) => {

            if (result === "save-and-close" || result === "save" || result === "delete" || result === "done" ||
                (el.dataset.ackuntil &&
                    new Date().getTime() < parseInt(el.dataset.ackuntil, 10)) ||
                !hasPendingChanges()) {
                return;
            }

            e.preventDefault();
            e.stopImmediatePropagation();
            confirmDialog(DialogUtilsTexts.PendingChangesConfirmation,
                () => {
                    el.dataset.ackuntil = "" + new Date().getTime() + 1000;
                    el.querySelector<HTMLElement>('div.save-and-close-button')?.click()
                },
                {
                    onNo: function () {
                        el.dataset.ackuntil = "" + new Date().getTime() + 1000;
                        Dialog.getInstance(el)?.close(result);
                    }
                });
        }, { before: true });

        function beforeUnload(e: BeforeUnloadEvent) {
            if (!Fluent.isVisibleLike(el) || !hasPendingChanges())
                return;

            e.preventDefault();
            (e as any).returnValue = DialogUtilsTexts.PendingChangesUnloadWarning;
            return DialogUtilsTexts.PendingChangesUnloadWarning;
        }

        Fluent.on(window, "beforeunload", beforeUnload);
        Dialog.onClose(el, (() => Fluent.off(window, "beforeunload", beforeUnload)), { oneOff: true });
    }
}
