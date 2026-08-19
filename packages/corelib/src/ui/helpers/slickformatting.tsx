import { applyFormatterResultToCellNode, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { replaceAll } from "../../compat";
import { Format, IRemoteView } from "../../slick";
import { skipEditLinkFormatPurposes } from "./editlink";

/**
 * Formatting helpers for sleek grids.
 */
export namespace SlickFormatting {
    
    /**
     * Returns a formatter that renders an edit link for an item.
     * @typeParam TItem - The type of the row item.
     * @param itemType - The type of the item, e.g. "Northwind.Customer".
     * @param idField - The name of the field holding the item id.
     * @param getText - A formatter that produces the link text, or null to use the raw value.
     * @param cssClass - Optional function returning an extra CSS class for the link.
     * @param encode - Whether to HTML-encode the text. Defaults to true.
     * @returns The item link formatter.
     */
    export function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>,
        cssClass?: (ctx: FormatterContext<TItem>) => string, encode: boolean = true): Format<TItem> {
        return function (ctx: FormatterContext<TItem>) {
            let fmtResult: FormatterResult;
            if (getText == null) {
                encode = true;
                fmtResult = ctx.value;
            }
            else {
                fmtResult = getText(ctx);
            }

            fmtResult = fmtResult instanceof Node ? fmtResult : encode ? ctx.escape(fmtResult) : (fmtResult ?? '');

            if ((ctx.item as any)?.__nonDataRow ||
                (ctx.purpose && skipEditLinkFormatPurposes.has(ctx.purpose))) {
                return fmtResult;
            }

            const itemId = (ctx.item as any)?.[idField];
            const extraClass = cssClass == null ? '' : cssClass(ctx)
            const encItemType = encodeURIComponent(replaceAll(itemType, '.', '-'));
            const encItemId = itemId != null ? encodeURIComponent(itemId.toString()) : null;

            const link = <a class={[`s-EditLink s-${replaceAll(itemType, '.', '-')}Link`, extraClass]}
                href={itemId != null ? '#' + encItemType + '/' + encItemId : null}
                data-item-type={itemType} data-item-id={itemId} /> as HTMLAnchorElement;

            applyFormatterResultToCellNode(ctx, fmtResult, link, contentOnly);
            return link;
        }
    }

    /**
     * Returns a formatter that renders a tree toggle (expand/collapse) control
     * with indentation based on the item's hierarchy.
     * @param getView - A function that returns the remote view.
     * @param getId - A function that returns the id of an item.
     * @param formatter - The formatter used to render the item content.
     * @returns The tree toggle formatter.
     */
    export function treeToggle(getView: () => IRemoteView<any>, getId: (x: any) => any,
        formatter: Format): Format {
        return function (ctx: FormatterContext): FormatterResult {
            const text = formatter(ctx);
            const view = getView();
            const indent = (ctx.item as any)._indent ?? 0;
            const spacer = <span class="s-TreeIndent" style={{ width: (15 * indent) + 'px' }} /> as HTMLSpanElement;
            const toggle = <span class="s-TreeToggle" /> as HTMLSpanElement;
            const id = getId(ctx.item);
            const idx = view.getIdxById(id);
            let next = view.getItemByIdx(idx + 1);
            if (next != null) {
                var nextIndent = next._indent ?? 0;
                if (nextIndent > indent) {
                    if (!!(ctx.item as any)._collapsed) {
                        toggle.classList.add("s-TreeExpand");
                    }
                    else {
                        toggle.classList.add("s-TreeCollapse");
                    }
                }
            }

            if (ctx.enableHtmlRendering && typeof text === "string" && text.length) {
                return (spacer.outerHTML + toggle.outerHTML + text);
            }
            else {
                return <>{spacer}{toggle}{text}</>
            }
        };
    }
}

const contentOnly = { contentOnly: true };