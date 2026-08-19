import { Fluent } from "../../base";
import { getWidgetFrom } from "./widgetutils";

/**
 * Provides a scoped context for resolving elements and widgets by an id prefix
 * within a given DOM node. Useful for panels and dialogs that need to look up
 * their child elements and widgets by short, prefix-relative ids.
 */
export class PrefixedContext {
    /** The id prefix used to resolve child element ids. */
    public readonly idPrefix: string;
    /** The DOM node that acts as the scope for lookups. */
    public readonly context: HTMLElement;

    /**
     * Creates a new prefixed context.
     * @param prefixOrWidget - Either a string id prefix, or an object exposing
     *   `idPrefix` and `domNode` (such as a widget) from which the prefix and
     *   context are derived.
     * @param context - Optional DOM node to scope lookups to; defaults to the
     *   `domNode` of `prefixOrWidget` when an object is provided.
     */
    constructor(prefixOrWidget: string | { idPrefix: string, domNode: HTMLElement }, context?: HTMLElement) {
        if (typeof prefixOrWidget === "string") {
            this.idPrefix = prefixOrWidget;
        } else {
            this.idPrefix = prefixOrWidget.idPrefix;
        }
        this.context = context ?? (typeof prefixOrWidget === "object" ? prefixOrWidget.domNode : void 0);
        this.initialize();
    }

    /**
     * Hook for subclasses to perform additional initialization.
     */
    protected initialize() {
    }

    /**
     * Resolves an element by its prefix-relative id.
     * @param id - The id relative to the context's id prefix.
     * @returns A {@link Fluent} wrapper for the matching element, or an empty
     *   Fluent object if no element matches.
     */
    byId(id: string): Fluent {
        return Fluent((this.context ?? document).querySelector('#' + this.idPrefix + id));
    }

    /**
     * Resolves a widget by its prefix-relative id and expected type.
     * @param id - The id relative to the context's id prefix.
     * @param type - The widget type to look up.
     * @returns The matching widget instance.
     */
    w<TWidget>(id: string, type: { new(...args: any[]): TWidget }): TWidget {
        return getWidgetFrom<TWidget>('#' + this.idPrefix + id, type, this.context);
    }
}