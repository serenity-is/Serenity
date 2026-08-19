/**
 * Describes the current view/buffer window that is (or should be) rendered.
 * Row bounds are view indices; column bounds are pixel offsets into the virtual canvas.
 */
export interface ViewRange {
    /** Top row index of the range (inclusive). */
    top?: number;
    /** Bottom row index of the range (exclusive or inclusive depending on caller; typically exclusive). */
    bottom?: number;
    /** Left pixel offset of the visible buffer window. */
    leftPx?: number;
    /** Right pixel offset of the visible buffer window. */
    rightPx?: number;
}
