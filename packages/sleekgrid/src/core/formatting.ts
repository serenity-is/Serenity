import type { Column } from "./column";
import { gridDefaults, GridOptions } from "./gridoptions";
import { addClass, basicDOMSanitizer, escapeHtml, removeClass } from "./util";

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
     * This is set from grid options and defaults to true for backward compatibility.
     * When set to false, the formatter should return plain text and the result will be set using textContent
     * and the escape() method is a noop in that case.
     */
    readonly enableHtmlRendering: boolean;

	/**
	 * Returns html escaped ctx.value if called without arguments. Prefer this over
	 * ctx.value when returning as HTML string to avoid html injection attacks!
     * Note that when enableHtmlRendering is false, this is simply a noop and returns the value as string.
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
    grid?: any;

    /**
     * The item of the row.
     */
    item?: TItem;

    /**
     * Purpose of the call, e.g. "autowidth", "excelexport", "groupheader", "headerfilter", "pdfexport", "print".
     */
    purpose?: "autowidth" | "excelexport" | "groupheader" | "grand-totals" | "group-totals" | "headerfilter" | "pdfexport" | "print";

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

export type FormatterResult = (string | HTMLElement | SVGElement | DocumentFragment);
export type ColumnFormat<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;

export interface CompatFormatterResult {
    addClasses?: string;
    text?: FormatterResult;
    toolTip?: string;
}

export type CompatFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: any) => string | CompatFormatterResult;

export interface FormatterFactory<TItem = any> {
    getFormat?(column: Column<TItem>): ColumnFormat<TItem>;
    getFormatter?(column: Column<TItem>): CompatFormatter<TItem>;
}

export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;
export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;

export type CellStylesHash = { [row: number]: { [columnId: string]: string } }

export function defaultColumnFormat(ctx: FormatterContext) {
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

export function applyFormatterResultToCellNode(ctx: FormatterContext, fmtResult: FormatterResult, node: HTMLElement, opt?: { contentOnly?: boolean }) {
    if (!opt?.contentOnly) {
        var oldFmtAtt = node.dataset.fmtatt as string;
        if (oldFmtAtt?.length > 0) {
            for (var k of oldFmtAtt.split(','))
                node.removeAttribute(k);
            delete node.dataset.fmtatt;
        }

        var oldFmtCls = node.dataset.fmtcls;
        if (oldFmtCls?.length && (ctx.addClass != oldFmtCls)) {
            removeClass(node, oldFmtCls);
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
    else if (!ctx.enableHtmlRendering)
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
            addClass(node, ctx.addClass);
            node.dataset.fmtcls = ctx.addClass;
        }
    }
}

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
