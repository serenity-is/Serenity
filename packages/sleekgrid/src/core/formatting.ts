import type { Column } from "./column";
import { gridDefaults, GridOptions } from "./gridoptions";
import type { ISleekGrid } from "./isleekgrid";
import { addCssClass, basicDOMSanitizer, escapeHtml, removeCssClass } from "./util";

/**
 * Context object for column formatters. It provides access to the
 * current cell value, row index, column index, etc.
 * Use grid.getFormatterContext() or the @see formatterContext helper to create a new instance.
 */
export interface FormatterContext<TItem = any> {

    /**
     * Additional attributes to be added to the cell node.
     */
    addAttrs?: { [key: string]: string; };

    /**
     * Additional classes to be added to the cell node.
     */
    addClass?: string;

    /**
     * True if the formatter is allowed to return raw HTML that will be set using innerHTML.
     * This is set from grid options and defaults to false which means the formatter
     * should return plain text and the result will be set using textContent and
     * the escape() method is a noop. If true, the formatter can return HTML strings but should
     * take care to avoid script injection attacks by using ctx.escape() method.
     */
    readonly enableHtmlRendering: boolean;

	/**
     * When enableHtmlRendering is false (default), this simply returns the value as string.
	 * When enableHtmlRendering is true, returns html escaped value / ctx.value if called without
     * arguments. Prefer this over ctx.value when returning HTML strings to avoid html injection
     * attacks when enableHtmlRendering is true. You don't have to use this inside JSX
     * style formatters as JSX automatically escapes values.
	 */
    escape(value?: any): string;

    /**
     * The row index of the cell.
     */
    row?: number;

    /**
     * The column index of the cell.
     */
    cell?: number;

    /**
     * The column definition of the cell.
     */
    column?: Column<TItem>;

    /**
     * The grid instance.
     */
    grid?: ISleekGrid;

    /**
     * The item of the row.
     */
    item?: TItem;

    /**
     * Purpose of the call, e.g. "auto-width", "excel-export", "group-header", "header-filter", "pdf-export", "print".
     */
    purpose?: "auto-width" | "excel-export" | "group-header" | "grand-totals" | "group-totals" | "header-filter" | "pdf-export" | "print";

    /**
     * Sanitizer function to clean up dirty HTML.
     */
    sanitizer: (dirtyHtml: string) => string;

    /**
     * Tooltip text to be added to the cell node as title attribute.
     */
    tooltip?: string;

    /** when returning a formatter result as HTML string, prefer ctx.escape() to avoid script injection attacks! */
    value?: any;
}

/**
 * Value returned by a formatter. Strings are treated as text or HTML depending on
 * `enableHtmlRendering`; DOM nodes are appended directly.
 */
export type FormatterResult = (string | HTMLElement | SVGElement | MathMLElement | DocumentFragment);

/**
 * Modern formatter signature; receives a {@link FormatterContext} and returns a {@link FormatterResult}.
 * @template TItem - Row item type.
 * @param ctx - Formatter context containing value, row/cell coordinates, column, grid and helpers.
 * @returns Renderable result for the cell.
 */
export type ColumnFormat<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;

/**
 * Structured result for legacy formatters that need to convey extra metadata.
 */
export interface CompatFormatterResult {
    /** Extra CSS classes to add to the cell node. */
    addClasses?: string;
    /** Main cell content. */
    text?: FormatterResult;
    /** Tooltip text for the cell node. */
    toolTip?: string;
}

/**
 * Legacy formatter signature kept for backward compatibility.
 * @template TItem - Row item type.
 * @param row - Row index.
 * @param cell - Cell/column index.
 * @param value - Raw cell value.
 * @param column - Column definition.
 * @param item - Row data item.
 * @param grid - Grid instance, if available.
 * @returns Plain string or structured result with classes/tooltip.
 */
export type CompatFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: ISleekGrid) => string | CompatFormatterResult;

/**
 * Factory that can provide formatters for columns, allowing centralized formatter resolution.
 * @template TItem - Row item type.
 */
export interface FormatterFactory<TItem = any> {
    /**
     * Returns the modern {@link ColumnFormat} for the given column, if any.
     * @param column - Column to resolve a formatter for.
     * @returns Formatter function or `undefined`.
     */
    getFormat?(column: Column<TItem>): ColumnFormat<TItem>;
    /**
     * Returns the legacy {@link CompatFormatter} for the given column, if any.
     * @param column - Column to resolve a formatter for.
     * @returns Legacy formatter or `undefined`.
     */
    getFormatter?(column: Column<TItem>): CompatFormatter<TItem>;
}

/**
 * Callback invoked asynchronously after a cell node has been rendered and attached.
 * @template TItem - Row item type.
 * @param cellNode - Rendered cell DOM node.
 * @param row - Row index.
 * @param item - Row data item.
 * @param column - Column definition.
 * @param reRender - Whether the call is due to a re-render of an already visible row.
 */
export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;

/**
 * Cleanup counterpart to {@link AsyncPostRender}; invoked before the cell node is removed.
 * @template TItem - Row item type.
 * @param cellNode - Cell DOM node being cleaned up.
 * @param row - Row index, if known.
 * @param column - Column definition, if known.
 */
export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;

/** Hash mapping `row -> columnId -> cssClass` for per-cell styling via `setCellCssStyles`. */
export type CellStylesHash = { [row: number]: { [columnId: string]: string } }

/**
 * Default column formatter; escapes or returns the value based on `enableHtmlRendering`.
 * Use as a safe fallback when no custom formatter is provided.
 * @param ctx - Formatter context whose `value` is rendered.
 * @returns Escaped or raw string representation of `ctx.value`.
 */
export function defaultColumnFormat(ctx: FormatterContext): FormatterResult {
    if (ctx?.escape)
        return ctx.escape();

    if (!ctx?.enableHtmlRendering) {
        if (ctx.value == null)
            return "";
        if (typeof ctx.value !== "string")
            return "" + ctx.value;
        return ctx.value;
    }

    return escapeHtml(ctx?.value);
}

/**
 * Wraps a legacy {@link CompatFormatter} as a modern {@link ColumnFormat} by adapting
 * the argument list and lifting `addClasses`/`toolTip` onto the context.
 * @param compatFormatter - Legacy formatter to convert.
 * @returns A {@link ColumnFormat} equivalent, or `null` if input was `null`.
 */
export function convertCompatFormatter(compatFormatter: CompatFormatter): ColumnFormat {
    if (compatFormatter == null)
        return null;

    return function (ctx: FormatterContext): FormatterResult {
        var fmtResult = compatFormatter(ctx.row, ctx.cell, ctx.value, ctx.column, ctx.item, ctx.grid);
        if (fmtResult != null && typeof fmtResult !== 'string' && Object.prototype.toString.call(fmtResult) === '[object Object]') {
            ctx.addClass = fmtResult.addClasses;
            ctx.tooltip = fmtResult.toolTip;
            return fmtResult.text;
        }
        return fmtResult as string;
    }
}

/**
 * Applies a formatter result to a DOM cell node, handling content, CSS classes,
 * attributes and tooltips tracked via the formatter context.
 * @param ctx - Active formatter context carrying `addClass`/`addAttrs`/`tooltip` and sanitizer flags.
 * @param fmtResult - Value returned by the formatter.
 * @param node - Cell DOM node to update.
 * @param opt - When `contentOnly` is `true`, only the inner content is updated; decoration cleanup is skipped.
 */
export function applyFormatterResultToCellNode(ctx: FormatterContext, fmtResult: FormatterResult, node: HTMLElement, opt?: { contentOnly?: boolean }): void {
    if (!opt?.contentOnly) {
        var oldFmtAtt = node.dataset.fmtatt as string;
        if (oldFmtAtt?.length > 0) {
            for (var k of oldFmtAtt.split(','))
                node.removeAttribute(k);
            delete node.dataset.fmtatt;
        }

        var oldFmtCls = node.dataset.fmtcls;
        if (oldFmtCls?.length && (ctx.addClass != oldFmtCls)) {
            removeCssClass(node, oldFmtCls);
            if (!ctx.addClass?.length)
                delete node.dataset.fmtcls;
        }

        var oldTooltip = node.getAttribute('tooltip');
        if (oldTooltip != null && ctx.tooltip != oldTooltip)
            node.removeAttribute('tooltip');

        if (ctx.tooltip !== undefined && oldTooltip != ctx.tooltip)
            node.setAttribute('tooltip', ctx.tooltip);
    }

    if (fmtResult == void 0)
        node.innerHTML = "";
    else if (fmtResult instanceof Node) {
        node.appendChild(fmtResult);
    }
    else if (ctx.enableHtmlRendering)
        node.innerHTML = (ctx.sanitizer ?? escapeHtml)(("" + fmtResult));
    else
        node.textContent = "" + fmtResult;

    if (!opt?.contentOnly) {
        if (ctx.addAttrs != null) {
            var keys = Object.keys(ctx.addAttrs);
            if (keys.length) {
                for (var k of keys) {
                    node.setAttribute(k, ctx.addAttrs[k]);
                }
                node.dataset.fmtatt = keys.join(',');
            }
        }

        if (ctx.addClass?.length) {
            addCssClass(node, ctx.addClass);
            node.dataset.fmtcls = ctx.addClass;
        }
    }
}

/**
 * Creates a {@link FormatterContext} populated with sensible defaults from the grid
 * options and DOMPurify (when available).
 * @template TItem - Row item type.
 * @param opt - Partial context fields to pre-fill; `addAttrs`/`addClass`/`tooltip` are managed by the formatter itself.
 * @returns Fully initialized formatter context ready to pass to a {@link ColumnFormat}.
 */
export function formatterContext<TItem = any>(opt?: Partial<Exclude<FormatterContext<TItem>, "addAttrs" | "addClass" | "tooltip">>): FormatterContext<TItem> {
    const gridOptions: GridOptions = opt?.grid?.getOptions?.();
    return {
        ...opt,
        enableHtmlRendering: opt?.enableHtmlRendering ?? gridOptions?.enableHtmlRendering ?? gridDefaults.enableHtmlRendering ?? false,
        escape: opt?.escape ?? escapeHtml,
        sanitizer: opt?.sanitizer ?? gridOptions?.sanitizer ?? gridDefaults.sanitizer ??
            // @ts-ignore
            ((typeof DOMPurify !== "undefined" && typeof DOMPurify.sanitize == "function") ? DOMPurify.sanitize
                : basicDOMSanitizer),
        value: opt?.value
    };
}
