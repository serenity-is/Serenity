// Adapted from https://github.com/6pac/SlickGrid/blob/master/src/slick.interactions.ts to replace jquery.event.drag

export interface DragItem {
    dragSource: HTMLElement | Document | null;
    dragHandle: HTMLElement | null;
    deltaX: number;
    deltaY: number;
    range: DragRange;
    target: HTMLElement;
    startX: number;
    startY: number;
}

export interface DragRange {
    start: {
        row?: number;
        cell?: number;
    };
    end: {
        row?: number;
        cell?: number;
    };
}

function windowScrollPosition() {
    return {
        left: window.scrollX ?? document.documentElement.scrollLeft ?? 0,
        top: window.scrollY ?? document.documentElement.scrollTop ?? 0
    };
}

export interface DragPosition {
    startX: number;
    startY: number;
    range: DragRange;
}

export interface DraggableOption {
    /** container DOM element, defaults to "document" */
    containerElement?: HTMLElement | Document;

    /** when defined, will allow dragging from a specific element by using the .matches() query selector. */
    allowDragFrom?: string;

    /** when defined, will allow dragging from a specific element or its closest parent by using the .closest() query selector. */
    allowDragFromClosest?: string;

    /** Defaults to `['ctrlKey', 'metaKey']`, list of keys that when pressed will prevent Draggable events from triggering (e.g. prevent onDrag when Ctrl key is pressed while dragging) */
    preventDragFromKeys?: Array<'altKey' | 'ctrlKey' | 'metaKey' | 'shiftKey'>;

    /** drag initialized callback */
    onDragInit?: (e: DragEvent, dd: DragPosition) => boolean | void;

    /** drag started callback */
    onDragStart?: (e: DragEvent, dd: DragPosition) => boolean | void;

    /** drag callback */
    onDrag?: (e: DragEvent, dd: DragPosition) => boolean | void;

    /** drag ended callback */
    onDragEnd?: (e: DragEvent, dd: DragPosition) => boolean | void;
}

export function Draggable(options: DraggableOption) {
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

    let originaldd: Partial<DragItem> = {
        dragSource: containerElement,
        dragHandle: null,
    };

    function init() {
        if (containerElement) {
            containerElement.addEventListener('mousedown', userPressed);
            containerElement.addEventListener('touchstart', userPressed, { passive: true });
        }
    }

    function executeDragCallbackWhenDefined(callback?: (e: DragEvent, dd: DragItem) => boolean | void, evt?: MouseEvent | Touch | TouchEvent | KeyboardEvent, dd?: DragItem) {
        if (typeof callback === 'function') {
            return callback(evt as DragEvent, dd);
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
            const { target } = targetEvent;

            if (!options.allowDragFrom || (options.allowDragFrom && (element.matches(options.allowDragFrom)) || (options.allowDragFromClosest && element.closest(options.allowDragFromClosest)))) {
                originaldd.dragHandle = element as HTMLElement;
                const winScrollPos = windowScrollPosition();
                startX = winScrollPos.left + targetEvent.clientX;
                startY = winScrollPos.top + targetEvent.clientY;
                deltaX = targetEvent.clientX - targetEvent.clientX;
                deltaY = targetEvent.clientY - targetEvent.clientY;
                originaldd = Object.assign(originaldd, { deltaX, deltaY, startX, startY, target });
                const result = executeDragCallbackWhenDefined(onDragInit as (e: DragEvent, dd: DragPosition) => boolean | void, event, originaldd as DragItem);

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
            const { target } = targetEvent;

            if (!dragStarted) {
                originaldd = Object.assign(originaldd, { deltaX, deltaY, startX, startY, target });
                executeDragCallbackWhenDefined(onDragStart, event, originaldd as DragItem);
                dragStarted = true;
            }

            originaldd = Object.assign(originaldd, { deltaX, deltaY, startX, startY, target });
            executeDragCallbackWhenDefined(onDrag, event, originaldd as DragItem);
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
            const { target } = event;
            originaldd = Object.assign(originaldd, { target });
            executeDragCallbackWhenDefined(onDragEnd, event, originaldd as DragItem);
            dragStarted = false;
        }
    }

    init();

    return { destroy };
}

