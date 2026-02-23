import { debounce, type Dialog } from "@serenity-is/corelib";
import "./modal-utils.css";

interface DragPosition {
    startX: number;
    startY: number;
}

interface DragItem extends DragPosition {
    dragSource: HTMLElement | Document | null;
    dragHandle: HTMLElement | null;
    deltaX: number;
    deltaY: number;
    dragTarget: HTMLElement;
}

function windowScrollPosition() {
    return {
        left: window.scrollX ?? document.documentElement.scrollLeft ?? 0,
        top: window.scrollY ?? document.documentElement.scrollTop ?? 0
    };
}

interface DraggableOption {
    containerElement?: HTMLElement | Document;
    allowDragFrom?: string;
    allowDragFromClosest?: string;
    preventDragFromKeys?: Array<'altKey' | 'ctrlKey' | 'metaKey' | 'shiftKey'>;
    onDragInit?: (e: DragEvent, dd: DragPosition) => boolean | void;
    onDragStart?: (e: DragEvent, dd: DragPosition) => boolean | void;
    onDrag?: (e: DragEvent, dd: DragPosition) => boolean | void;
    onDragEnd?: (e: DragEvent, dd: DragPosition) => boolean | void;
}

function Draggable(options: DraggableOption) {
    let { containerElement } = options;
    const { onDragInit, onDragStart, onDrag, onDragEnd, preventDragFromKeys } = options;
    let element: HTMLElement | null;
    let startX: number;
    let startY: number;
    let deltaX: number;
    let deltaY: number;
    let dragStarted: boolean;

    if (!containerElement) {
        containerElement = document.body;
    }

    let dragData: Partial<DragItem> = {
        dragSource: containerElement,
        dragHandle: null,
    };

    function init() {
        if (containerElement) {
            containerElement.addEventListener('mousedown', userPressed);
            containerElement.addEventListener('touchstart', userPressed, { passive: true });
        }
    }

    function executeDragCallbackWhenDefined(callback?: (e: UIEvent, dragData: DragPosition) => boolean | void, evt?: MouseEvent | TouchEvent | KeyboardEvent, dragData?: Partial<DragPosition>) {
        if (typeof callback === 'function') {
            return callback(evt, dragData as DragPosition);
        }
    }

    function destroy() {
        if (containerElement) {
            containerElement.removeEventListener('mousedown', userPressed);
            containerElement.removeEventListener('touchstart', userPressed);
        }
    }

    /** Do we want to prevent Drag events from happening (for example prevent onDrag when Ctrl key is pressed while dragging) */
    function preventDrag(event: MouseEvent | TouchEvent | KeyboardEvent) {
        let eventPrevented = false;
        if (preventDragFromKeys) {
            preventDragFromKeys.forEach(key => {
                if ((event as any)[key]) {
                    eventPrevented = true;
                }
            });
        }
        return eventPrevented;
    }

    function userPressed(event: MouseEvent | TouchEvent | KeyboardEvent) {
        if (!preventDrag(event)) {
            element = event.target as HTMLElement;
            const targetEvent: MouseEvent | Touch = ((event as TouchEvent)?.touches?.[0] ?? event) as any;

            if (!options.allowDragFrom ||
                (options.allowDragFrom && (element.matches(options.allowDragFrom)) ||
                    (options.allowDragFromClosest && element.closest(options.allowDragFromClosest)))) {
                dragData.dragHandle = element as HTMLElement;
                const winScrollPos = windowScrollPosition();
                startX = winScrollPos.left + targetEvent.clientX;
                startY = winScrollPos.top + targetEvent.clientY;
                deltaX = targetEvent.clientX - targetEvent.clientX;
                deltaY = targetEvent.clientY - targetEvent.clientY;
                dragData = Object.assign(dragData, { deltaX, deltaY, startX, startY, dragTarget: targetEvent.target });
                const result = executeDragCallbackWhenDefined(onDragInit, event, dragData);

                if (result !== false) {
                    document.body.addEventListener('mousemove', userMoved);
                    document.body.addEventListener('touchmove', userMoved, { passive: true });
                    document.body.addEventListener('mouseup', userReleased);
                    document.body.addEventListener('touchend', userReleased, { passive: true });
                    document.body.addEventListener('touchcancel', userReleased, { passive: true });
                }
            }
        }
    }

    function userMoved(event: MouseEvent | TouchEvent | KeyboardEvent) {
        if (!preventDrag(event)) {
            const targetEvent: MouseEvent | Touch = (event as TouchEvent)?.touches?.[0] ?? event as any;
            deltaX = targetEvent.clientX - startX;
            deltaY = targetEvent.clientY - startY;

            if (!dragStarted) {
                dragData = Object.assign(dragData, { deltaX, deltaY, startX, startY, dragTarget: targetEvent.target });
                executeDragCallbackWhenDefined(onDragStart, event, dragData);
                dragStarted = true;
            }

            dragData = Object.assign(dragData, { deltaX, deltaY, startX, startY, dragTarget: targetEvent.target });
            executeDragCallbackWhenDefined(onDrag, event, dragData);
        }
    }

    function userReleased(event: MouseEvent | TouchEvent) {
        document.body.removeEventListener('mousemove', userMoved);
        document.body.removeEventListener('touchmove', userMoved);
        document.body.removeEventListener('mouseup', userReleased);
        document.body.removeEventListener('touchend', userReleased);
        document.body.removeEventListener('touchcancel', userReleased);

        // trigger a dragEnd event only after dragging started and stopped
        if (dragStarted) {
            dragData = Object.assign(dragData, { dragTarget: event.target });
            executeDragCallbackWhenDefined(onDragEnd, event, dragData as DragItem);
            dragStarted = false;
        }
    }

    init();

    return { destroy };
}

export function makeModalMaximizable(dialog: Dialog) {
    if (!dialog || dialog.type !== "bsmodal")
        return;

    const header = dialog.getHeaderNode();
    if (!header)
        return;

    header.classList.add("has-maximize-button");

    const closeButton = header?.querySelector<HTMLElement>(".btn-close");

    const maximizeButton = <button type="button" class="btn btn-window-maximize" style={{
        marginLeft: "auto"
    }} onClick={(e) => {
        const dlg = header.closest(".modal-dialog");
        dlg.classList.toggle("modal-fullscreen");
    }}>
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-window-fullscreen maximize-image" viewBox="0 0 16 16" style={{ transform: "scale(-1,1)" }}>
            <path d="M3 3.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0m1.5 0a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0m1 .5a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1" />
            <path d="M.5 1a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 .5.5h15a.5.5 0 0 0 .5-.5v-13a.5.5 0 0 0-.5-.5zM1 5V2h14v3zm0 1h14v8H1z" />
        </svg>
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-window-stack restore-image" viewBox="0 0 16 16" style={{ transform: "scale(-1,1)" }}>
            <path d="M4.5 6a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1M6 6a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1m2-.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0" />
            <path d="M12 1a2 2 0 0 1 2 2 2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2 2 2 0 0 1-2-2V3a2 2 0 0 1 2-2zM2 12V5a2 2 0 0 1 2-2h9a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1m1-4v5a1 1 0 0 0 1 1h10a1 1 0 0 0 1-1V8zm12-1V5a1 1 0 0 0-1-1H4a1 1 0 0 0-1 1v2z" />
        </svg>
    </button>

    if (closeButton)
        header.insertBefore(maximizeButton, closeButton);
    else
        header.appendChild(maximizeButton);
}

/**
 * Makes a modal dialog draggable by its header. Note that Bootstrap's modal implementation
 * is not normally designed for dragging, so this is a best effort approach and may
 * not work perfectly in all scenarios.
 */
export function makeModalDraggable(dialog: Dialog) {
    if (!dialog || dialog.type !== "bsmodal")
        return;

    const header = dialog.getHeaderNode();
    if (!header)
        return;

    function isFullScreen(el: HTMLElement) {
        const r = el.getBoundingClientRect();
        return Math.abs(r.width - window.innerWidth) < 1 &&
            Math.abs(r.height - window.innerHeight) < 1;
    }

    const dlg = header.closest<HTMLElement>(".modal-dialog");
    if (!dlg)
        return;

    dlg.classList.add("draggable");

    function setPosition(left: number, top: number) {
        const content = dlg.querySelector<HTMLElement>(".modal-content") ?? dlg;
        const bounds = content.getBoundingClientRect();
        dlg.style.left = Math.max(Math.min(left, window.innerWidth - bounds.width), 0) + "px";
        dlg.style.top = Math.max(Math.min(top, window.innerHeight - bounds.height), 0) + "px";
    }

    let draggable = Draggable({
        containerElement: header,
        onDragStart: (e, data) => {
            if (!dlg || isFullScreen(dlg))
                return;
            e.preventDefault();
            const content = dlg.querySelector<HTMLElement>(".modal-content") ?? dlg;
            const bounds = content.getBoundingClientRect();
            (data as any).origX = dlg.style.left ? parseFloat(dlg.style.left) : bounds.left;
            (data as any).origY = dlg.style.top ? parseFloat(dlg.style.top) : bounds.top;
            header.style.userSelect = "none";
        },
        onDrag: (e, data: DragItem) => {
            if (!dlg || isFullScreen(dlg))
                return;
            e.preventDefault();
            setPosition((data as any).origX + data.deltaX, (data as any).origY + data.deltaY);
            dlg.classList.add("dragged");
            
        },
        onDragEnd: () => {
            if (!dlg || isFullScreen(dlg))
                return;
            header.style.userSelect = "";
        }
    });

    const resizeHandler = () => {
        if (!dlg || isFullScreen(dlg) || !dlg.classList.contains("dragged"))
            return;
        setPosition(parseFloat(dlg.style.left), parseFloat(dlg.style.top));
    };

    window.addEventListener("resize", resizeHandler);

    dialog.onClose(() => {
        dlg?.classList.remove("draggable");
        draggable?.destroy();
        window.removeEventListener("resize", resizeHandler);
        draggable = null;
        dialog = null;
    });
}