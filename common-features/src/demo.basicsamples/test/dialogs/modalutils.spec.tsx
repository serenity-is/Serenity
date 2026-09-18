import { makeModalDraggable, makeModalMaximizable } from "../../Modules/Dialogs/ChartInDialog/modal-utils";

function createModal() {
    const modal = document.createElement("div");
    modal.className = "modal-dialog";
    const header = document.createElement("div");
    header.className = "modal-header";
    const close = document.createElement("button");
    close.className = "btn-close";
    header.append(close);
    const content = document.createElement("div");
    content.className = "modal-content";
    modal.append(header, content);
    document.body.append(modal);
    return { modal, header, close, content };
}

function createDialog(header: HTMLElement | null, onClose: Array<(result?: any) => void> = []) {
    return {
        type: "bsmodal",
        getHeaderNode: () => header,
        onClose: (cb: (result?: any) => void) => { onClose.push(cb); }
    } as any;
}

afterEach(() => {
    document.body.innerHTML = "";
});

describe("makeModalMaximizable", () => {
    it("does nothing for non bootstrap dialogs", () => {
        const { header } = createModal();
        makeModalMaximizable({ type: "other", getHeaderNode: () => header } as any);
        expect(header.classList.contains("has-maximize-button")).toBe(false);
    });

    it("does nothing when there is no header", () => {
        expect(() => makeModalMaximizable(createDialog(null))).not.toThrow();
    });

    it("adds a maximize button that toggles fullscreen", () => {
        const { modal, header, close } = createModal();
        makeModalMaximizable(createDialog(header));

        expect(header.classList.contains("has-maximize-button")).toBe(true);
        const maximize = header.querySelector(".btn-window-maximize") as HTMLElement;
        expect(maximize).toBeTruthy();
        expect(maximize.nextElementSibling).toBe(close);

        maximize.click();
        expect(modal.classList.contains("modal-fullscreen")).toBe(true);
        maximize.click();
        expect(modal.classList.contains("modal-fullscreen")).toBe(false);
    });

    it("appends the maximize button when there is no close button", () => {
        const { header } = createModal();
        header.querySelector(".btn-close").remove();
        makeModalMaximizable(createDialog(header));
        expect(header.lastElementChild.className).toContain("btn-window-maximize");
    });
});

describe("makeModalDraggable", () => {
    it("does nothing for non bootstrap dialogs", () => {
        const { modal, header } = createModal();
        makeModalDraggable({ type: "other", getHeaderNode: () => header } as any);
        expect(modal.classList.contains("draggable")).toBe(false);
    });

    it("does nothing when there is no header", () => {
        expect(() => makeModalDraggable(createDialog(null))).not.toThrow();
    });

    it("drags the dialog and cleans up on close", () => {
        const { modal, header } = createModal();
        const onClose: Array<() => void> = [];
        makeModalDraggable(createDialog(header, onClose));

        expect(modal.classList.contains("draggable")).toBe(true);

        header.dispatchEvent(new MouseEvent("mousedown", { bubbles: true, clientX: 10, clientY: 10 }));
        document.body.dispatchEvent(new MouseEvent("mousemove", { bubbles: true, clientX: 60, clientY: 70 }));
        expect(modal.classList.contains("dragged")).toBe(true);
        expect(modal.style.left).toBeTruthy();

        document.body.dispatchEvent(new MouseEvent("mouseup", { bubbles: true, clientX: 60, clientY: 70 }));

        expect(onClose.length).toBe(1);
        onClose.forEach(cb => cb());
        expect(modal.classList.contains("draggable")).toBe(false);
    });

    it("ignores drag attempts not starting from the header", () => {
        const { modal, header } = createModal();
        makeModalDraggable(createDialog(header));

        document.body.dispatchEvent(new MouseEvent("mousedown", { bubbles: true, clientX: 1, clientY: 1 }));
        document.body.dispatchEvent(new MouseEvent("mousemove", { bubbles: true, clientX: 50, clientY: 50 }));
        expect(modal.classList.contains("dragged")).toBe(false);
    });

    it("does nothing when the header is not inside a modal dialog", () => {
        const header = document.createElement("div");
        document.body.append(header);
        makeModalDraggable(createDialog(header));
        expect(header.classList.contains("draggable")).toBe(false);
    });

    it("skips dragging when the dialog is fullscreen", () => {
        const { modal, header } = createModal();
        vi.spyOn(modal, "getBoundingClientRect").mockReturnValue({
            width: window.innerWidth, height: window.innerHeight, left: 0, top: 0, right: 0, bottom: 0, x: 0, y: 0, toJSON: () => ({ })
        } as any);
        makeModalDraggable(createDialog(header));

        header.dispatchEvent(new MouseEvent("mousedown", { bubbles: true, clientX: 10, clientY: 10 }));
        document.body.dispatchEvent(new MouseEvent("mousemove", { bubbles: true, clientX: 60, clientY: 70 }));
        expect(modal.classList.contains("dragged")).toBe(false);
    });

    it("repositions on window resize after dragging", () => {
        const { modal, header } = createModal();
        makeModalDraggable(createDialog(header));

        header.dispatchEvent(new MouseEvent("mousedown", { bubbles: true, clientX: 10, clientY: 10 }));
        document.body.dispatchEvent(new MouseEvent("mousemove", { bubbles: true, clientX: 60, clientY: 70 }));
        document.body.dispatchEvent(new MouseEvent("mouseup", { bubbles: true, clientX: 60, clientY: 70 }));

        window.dispatchEvent(new Event("resize"));
        expect(modal.classList.contains("dragged")).toBe(true);
    });
});
