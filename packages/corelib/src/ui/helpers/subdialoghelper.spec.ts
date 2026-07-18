import { Widget } from "../widgets/widget";
import { SubDialogHelper } from "./subdialoghelper";

describe("SubDialogHelper", () => {
    describe("bindToDataChange", () => {
        it("binds to ondatachange event on dialog element", () => {
            const dialog = new Widget({});
            const owner = new Widget({});
            const dataChange = vi.fn();

            const result = SubDialogHelper.bindToDataChange(dialog, owner, dataChange);
            expect(result).toBe(dialog);

            // The fluent.on strips namespace, so dispatch just "ondatachange"
            dialog.domNode.dispatchEvent(new CustomEvent("ondatachange", { detail: { operationType: "update" } }));

            dialog.destroy();
            owner.destroy();
        });

        it("uses originalEvent if present", () => {
            const dialog = new Widget({});
            const owner = new Widget({});
            const dataChange = vi.fn();

            SubDialogHelper.bindToDataChange(dialog, owner, dataChange);

            // Dispatch with originalEvent that has operationType
            const originalEvent = new CustomEvent("test");
            (originalEvent as any).operationType = "update";
            dialog.domNode.dispatchEvent(new CustomEvent("ondatachange", {
                detail: { originalEvent }
            }));

            dialog.destroy();
            owner.destroy();
        });

        it("uses timeout when useTimeout is true", () => {
            vi.useFakeTimers();
            const dialog = new Widget({});
            const owner = new Widget({});
            const dataChange = vi.fn();

            SubDialogHelper.bindToDataChange(dialog, owner, dataChange, true);

            dialog.domNode.dispatchEvent(new CustomEvent("ondatachange", { detail: { operationType: "update" } }));
            expect(dataChange).not.toHaveBeenCalled();

            vi.runAllTimers();
            expect(dataChange).toHaveBeenCalled();

            vi.useRealTimers();
            dialog.destroy();
            owner.destroy();
        });
    });

    describe("triggerDataChange", () => {
        it("triggers ondatachange event on dialog domNode", () => {
            const dialog = new Widget({});
            const handler = vi.fn();
            dialog.domNode.addEventListener("ondatachange", handler);

            const result = SubDialogHelper.triggerDataChange(dialog);
            expect(result).toBe(dialog);
            expect(handler).toHaveBeenCalled();

            dialog.destroy();
        });
    });

    describe("triggerDataChanged", () => {
        it("triggers ondatachange on HTMLElement", () => {
            const element = document.createElement("div");
            const handler = vi.fn();
            element.addEventListener("ondatachange", handler);

            SubDialogHelper.triggerDataChanged(element);
            expect(handler).toHaveBeenCalled();
        });

        it("triggers ondatachange on ArrayLike element", () => {
            const element = document.createElement("div");
            const handler = vi.fn();
            element.addEventListener("ondatachange", handler);

            SubDialogHelper.triggerDataChanged([element] as any);
            expect(handler).toHaveBeenCalled();
        });
    });

    describe("bubbleDataChange", () => {
        it("bubbles data change from dialog to owner", () => {
            const dialog = new Widget({});
            const owner = new Widget({});
            const handler = vi.fn();
            owner.domNode.addEventListener("ondatachange", handler);

            SubDialogHelper.bubbleDataChange(dialog, owner);

            dialog.domNode.dispatchEvent(new CustomEvent("ondatachange", { detail: { operationType: "update" } }));
            expect(handler).toHaveBeenCalled();

            dialog.destroy();
            owner.destroy();
        });
    });

    describe("cascade", () => {
        it("sets up dialogopen handler for cascading", () => {
            const dialog = new Widget({});
            const element = document.createElement("div");

            const result = SubDialogHelper.cascade(dialog, element);
            expect(result).toBe(dialog);

            // Trigger dialogopen
            dialog.domNode.dispatchEvent(new Event("dialogopen"));

            dialog.destroy();
        });
    });

    describe("cascadedDialogOffset", () => {
        it("returns offset for HTMLElement", () => {
            const element = document.createElement("div");
            const offset = SubDialogHelper.cascadedDialogOffset(element);
            expect(offset).toEqual({
                my: 'left top',
                at: 'left+20 top+20',
                of: element
            });
        });

        it("returns offset for ArrayLike element", () => {
            const element = document.createElement("div");
            const offset = SubDialogHelper.cascadedDialogOffset([element] as any);
            expect(offset.of).toBe(element);
        });
    });
});
