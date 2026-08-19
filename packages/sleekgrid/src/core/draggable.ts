// Adapted from https://github.com/6pac/SlickGrid/blob/master/src/slick.interactions.ts to replace jquery.event.drag

/**
 * Starting scroll-adjusted position and logical range for a drag interaction.
 */
export interface DragPosition {
    /** Scroll-adjusted X coordinate where the drag started. */
    startX: number;
    /** Scroll-adjusted Y coordinate where the drag started. */
    startY: number;
    /** Logical grid range derived from pointer movement, if applicable. */
    range: DragRange;
}

/**
 * Full drag-state payload passed to drag callbacks; extends {@link DragPosition}
 * with live deltas and DOM references.
 */
export interface DragItem extends DragPosition {
    /** Element or document that the draggable was bound to. */
    dragSource: HTMLElement | Document | null;
    /** Element on which the drag gesture started. */
    dragHandle: HTMLElement | null;
    /** Horizontal delta in pixels since drag start. */
    deltaX: number;
    /** Vertical delta in pixels since drag start. */
    deltaY: number;
    /** Current DOM element under the pointer during the drag. */
    dragTarget: HTMLElement;
}

/**
 * Logical start/end row/cell range accumulated during a drag (e.g. for column reordering).
 */
export interface DragRange {
    /** Starting row/cell for the drag range. */
    start: {
        row?: number;
        cell?: number;
    };
    /** Ending row/cell for the drag range. */
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


/**
 * Options for the {@link Draggable} helper.
 */
export interface DraggableOption {
    /**
     * Container DOM element to listen for mousedown/touchstart on.
     * Defaults to `document.body` when omitted.
     */
    containerElement?: HTMLElement | Document;

    /**
     * When defined, dragging is only allowed when the mousedown target
     * matches this CSS selector (checked via `Element.matches()`).
     */
    allowDragFrom?: string;

    /**
     * When defined, dragging is allowed when the mousedown target or one of its
     * closest ancestors matches this selector (checked via `Element.closest()`).
     */
    allowDragFromClosest?: string;

    /**
     * Keys that, when pressed during the interaction, prevent draggable events from firing.
     * Defaults to `['ctrlKey', 'metaKey']` at the call site (e.g. prevents drag when Ctrl is held).
     */
    preventDragFromKeys?: Array<'altKey' | 'ctrlKey' | 'metaKey' | 'shiftKey'>;

    /**
     * Invoked on mousedown before any dragging starts. Return `false` to cancel the drag.
     * @param e - Native drag/mouse event.
     * @param dd - Current drag position and range.
     */
    onDragInit?: (e: DragEvent, dd: DragPosition) => boolean | void;

    /**
     * Invoked the first time the pointer moves after mousedown.
     * @param e - Native drag/mouse event.
     * @param dd - Current drag position and range.
     */
    onDragStart?: (e: DragEvent, dd: DragPosition) => boolean | void;

    /**
     * Invoked on every pointer move while dragging.
     * @param e - Native drag/mouse event.
     * @param dd - Current drag position and range including live deltas.
     */
    onDrag?: (e: DragEvent, dd: DragPosition) => boolean | void;

    /**
     * Invoked when the pointer is released after a drag has started.
     * @param e - Native drag/mouse event.
     * @param dd - Final drag position and range.
     */
    onDragEnd?: (e: DragEvent, dd: DragPosition) => boolean | void;
}

/**
 * Attaches lightweight mouse/touch drag handling to a container element without jQuery.
 * Listens for mousedown/touchstart on `containerElement` and translates movements
 * into the `onDrag*` callbacks in {@link DraggableOption}.
 * @param options - Configuration controlling drag source, filters and callbacks.
 * @returns Handle with a `destroy` method to remove all listeners.
 */
export function Draggable(options: DraggableOption): {
    /** Removes all event listeners installed by this draggable instance. */
    destroy: () => void;
} {
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

    function destroy(): void {
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

