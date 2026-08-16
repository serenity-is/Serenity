import { Draggable, type DragItem, type DragPosition } from "../../src/core/draggable";

function makeContainer(): HTMLElement {
    const container = document.createElement("div");
    document.body.appendChild(container);
    return container;
}

function fireMouse(type: string, target: EventTarget, init: MouseEventInit = {}) {
    const ev = new MouseEvent(type, { bubbles: true, cancelable: true, ...init });
    target.dispatchEvent(ev);
    return ev;
}

function fireTouch(type: string, target: EventTarget, clientX = 0, clientY = 0) {
    const ev = new Event(type, { bubbles: true, cancelable: true });
    Object.defineProperty(ev, "touches", {
        value: [{ clientX, clientY }],
        configurable: true
    });
    target.dispatchEvent(ev);
    return ev;
}

describe('Draggable', () => {
    afterEach(() => {
        // clean up any listener left attached to document.body mid-drag
        fireMouse("mouseup", document.body);
        document.body.innerHTML = "";
    });

    describe('mouse drag flow', () => {
        it('performs init -> start -> drag -> end', () => {
            const container = makeContainer();
            const onDragInit = vi.fn();
            const onDragStart = vi.fn();
            const onDrag = vi.fn();
            const onDragEnd = vi.fn();
            const d = Draggable({ containerElement: container, onDragInit, onDragStart, onDrag, onDragEnd });

            fireMouse("mousedown", container, { clientX: 10, clientY: 20 });
            expect(onDragInit).toHaveBeenCalledTimes(1);
            expect(onDragStart).not.toHaveBeenCalled();

            fireMouse("mousemove", document.body, { clientX: 30, clientY: 40 });
            expect(onDragStart).toHaveBeenCalledTimes(1);
            expect(onDrag).toHaveBeenCalledTimes(1);

            fireMouse("mousemove", document.body, { clientX: 50, clientY: 60 });
            expect(onDragStart).toHaveBeenCalledTimes(1);
            expect(onDrag).toHaveBeenCalledTimes(2);

            fireMouse("mouseup", document.body, { clientX: 50, clientY: 60 });
            expect(onDragEnd).toHaveBeenCalledTimes(1);

            d.destroy();
        });

        it('passes the drag position and deltas to callbacks', () => {
            const container = makeContainer();
            const onDrag = vi.fn();
            const d = Draggable({ containerElement: container, onDrag });

            fireMouse("mousedown", container, { clientX: 10, clientY: 20 });
            fireMouse("mousemove", document.body, { clientX: 25, clientY: 35 });

            const dd = onDrag.mock.calls[0][1] as DragItem;
            expect(dd.startX).toBe(10);
            expect(dd.startY).toBe(20);
            expect(dd.deltaX).toBe(15);
            expect(dd.deltaY).toBe(15);

            d.destroy();
        });

        it('does not start dragging when onDragInit returns false', () => {
            const container = makeContainer();
            const onDragInit = vi.fn(() => false);
            const onDragStart = vi.fn();
            const onDrag = vi.fn();
            const onDragEnd = vi.fn();
            const d = Draggable({ containerElement: container, onDragInit, onDragStart, onDrag, onDragEnd });

            fireMouse("mousedown", container, { clientX: 10, clientY: 10 });
            fireMouse("mousemove", document.body, { clientX: 30, clientY: 30 });
            fireMouse("mouseup", document.body);

            expect(onDragInit).toHaveBeenCalledTimes(1);
            expect(onDragStart).not.toHaveBeenCalled();
            expect(onDrag).not.toHaveBeenCalled();
            expect(onDragEnd).not.toHaveBeenCalled();

            d.destroy();
        });

        it('does not call onDragEnd if the drag never started', () => {
            const container = makeContainer();
            const onDragEnd = vi.fn();
            const d = Draggable({ containerElement: container, onDragEnd });

            fireMouse("mousedown", container, { clientX: 10, clientY: 10 });
            fireMouse("mouseup", document.body);

            expect(onDragEnd).not.toHaveBeenCalled();

            d.destroy();
        });
    });

    describe('preventDragFromKeys', () => {
        it('does not initiate drag when a prevent key is pressed on mousedown', () => {
            const container = makeContainer();
            const onDragInit = vi.fn();
            const d = Draggable({ containerElement: container, onDragInit, preventDragFromKeys: ['ctrlKey'] });

            fireMouse("mousedown", container, { ctrlKey: true });
            expect(onDragInit).not.toHaveBeenCalled();

            d.destroy();
        });

        it('skips onDrag when a prevent key is pressed during movement', () => {
            const container = makeContainer();
            const onDragStart = vi.fn();
            const onDrag = vi.fn();
            const d = Draggable({ containerElement: container, onDragStart, onDrag, preventDragFromKeys: ['metaKey'] });

            fireMouse("mousedown", container);
            fireMouse("mousemove", document.body, { metaKey: true });
            expect(onDragStart).not.toHaveBeenCalled();
            expect(onDrag).not.toHaveBeenCalled();

            d.destroy();
        });

        it('checks several prevent keys', () => {
            const container = makeContainer();
            const onDragInit = vi.fn();
            const d = Draggable({ containerElement: container, onDragInit, preventDragFromKeys: ['altKey', 'shiftKey'] });

            fireMouse("mousedown", container, { shiftKey: true });
            expect(onDragInit).not.toHaveBeenCalled();

            fireMouse("mousedown", container, { altKey: true });
            expect(onDragInit).not.toHaveBeenCalled();

            d.destroy();
        });
    });

    describe('allowDragFrom / allowDragFromClosest', () => {
        it('allows drag only from elements matching allowDragFrom', () => {
            const container = makeContainer();
            const handle = document.createElement("div");
            handle.className = "handle";
            const other = document.createElement("div");
            container.appendChild(handle);
            container.appendChild(other);
            const onDragInit = vi.fn();
            const d = Draggable({ containerElement: container, allowDragFrom: ".handle", onDragInit });

            fireMouse("mousedown", other);
            expect(onDragInit).not.toHaveBeenCalled();

            fireMouse("mousedown", handle);
            expect(onDragInit).toHaveBeenCalledTimes(1);

            d.destroy();
        });

        it('allows drag from an element whose closest ancestor matches allowDragFromClosest', () => {
            const container = makeContainer();
            const parent = document.createElement("div");
            parent.className = "parent";
            const child = document.createElement("div");
            parent.appendChild(child);
            container.appendChild(parent);
            const onDragInit = vi.fn();
            const d = Draggable({ containerElement: container, allowDragFromClosest: ".parent", onDragInit });

            fireMouse("mousedown", child);
            expect(onDragInit).toHaveBeenCalledTimes(1);

            d.destroy();
        });

        it('allows drag when allowDragFrom fails but allowDragFromClosest matches', () => {
            const container = makeContainer();
            const parent = document.createElement("div");
            parent.className = "parent";
            const child = document.createElement("div");
            parent.appendChild(child);
            container.appendChild(parent);
            const onDragInit = vi.fn();
            const d = Draggable({
                containerElement: container,
                allowDragFrom: ".nope",
                allowDragFromClosest: ".parent",
                onDragInit
            });

            fireMouse("mousedown", child);
            expect(onDragInit).toHaveBeenCalledTimes(1);

            d.destroy();
        });

        it('does not allow drag when neither allowDragFrom nor allowDragFromClosest match', () => {
            const container = makeContainer();
            const other = document.createElement("div");
            container.appendChild(other);
            const onDragInit = vi.fn();
            const d = Draggable({
                containerElement: container,
                allowDragFrom: ".nope",
                allowDragFromClosest: ".nope2",
                onDragInit
            });

            fireMouse("mousedown", other);
            expect(onDragInit).not.toHaveBeenCalled();

            d.destroy();
        });
    });

    describe('touch flow', () => {
        it('handles touchstart -> touchmove -> touchend', () => {
            const container = makeContainer();
            const onDragStart = vi.fn();
            const onDrag = vi.fn();
            const onDragEnd = vi.fn();
            const d = Draggable({ containerElement: container, onDragStart, onDrag, onDragEnd });

            fireTouch("touchstart", container, 5, 6);
            fireTouch("touchmove", document.body, 15, 16);
            expect(onDragStart).toHaveBeenCalledTimes(1);
            expect(onDrag).toHaveBeenCalledTimes(1);

            fireTouch("touchend", document.body);
            expect(onDragEnd).toHaveBeenCalledTimes(1);

            d.destroy();
        });

        it('handles touchcancel as a release', () => {
            const container = makeContainer();
            const onDragEnd = vi.fn();
            const d = Draggable({ containerElement: container, onDragEnd });

            fireTouch("touchstart", container, 5, 6);
            fireTouch("touchmove", document.body, 15, 16);
            fireTouch("touchcancel", document.body);
            expect(onDragEnd).toHaveBeenCalledTimes(1);

            d.destroy();
        });
    });

    describe('lifecycle', () => {
        it('defaults the container to document.body', () => {
            const onDragInit = vi.fn();
            const d = Draggable({ onDragInit });

            fireMouse("mousedown", document.body, { clientX: 1, clientY: 2 });
            expect(onDragInit).toHaveBeenCalledTimes(1);

            d.destroy();
        });

        it('destroy removes the event listeners', () => {
            const container = makeContainer();
            const onDragInit = vi.fn();
            const d = Draggable({ containerElement: container, onDragInit });

            d.destroy();

            fireMouse("mousedown", container);
            expect(onDragInit).not.toHaveBeenCalled();
        });

        it('uses documentElement scroll position when window.scrollX/scrollY are undefined', () => {
            const container = makeContainer();
            const onDrag = vi.fn();
            const originalX = Object.getOwnPropertyDescriptor(window, "scrollX");
            const originalY = Object.getOwnPropertyDescriptor(window, "scrollY");
            Object.defineProperty(window, "scrollX", { get: () => undefined, configurable: true });
            Object.defineProperty(window, "scrollY", { get: () => undefined, configurable: true });
            document.documentElement.scrollLeft = 5;
            document.documentElement.scrollTop = 7;

            const d = Draggable({ containerElement: container, onDrag });
            try {
                fireMouse("mousedown", container, { clientX: 10, clientY: 20 });
                fireMouse("mousemove", document.body, { clientX: 15, clientY: 27 });
                const dd = onDrag.mock.calls[0][1] as DragPosition;
                expect(dd.startX).toBe(15); // 5 + 10
                expect(dd.startY).toBe(27); // 7 + 20
            } finally {
                d.destroy();
                if (originalX)
                    Object.defineProperty(window, "scrollX", originalX);
                if (originalY)
                    Object.defineProperty(window, "scrollY", originalY);
            }
        });
    });
});
