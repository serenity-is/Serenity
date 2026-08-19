import { executeEverytimeWhenVisible, executeOnceWhenVisible } from "../../compat";

/**
 * Helper functions for lazy loading content when it becomes visible.
 */
export namespace LazyLoadHelper {
    /**
     * Executes the given callback once when the element becomes visible.
     */
    export const executeOnceWhenShown = executeOnceWhenVisible
    /**
     * Executes the given callback every time the element becomes visible.
     */
    export const executeEverytimeWhenShown = executeEverytimeWhenVisible;
}