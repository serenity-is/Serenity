import { applyFormatterResultToCellNode, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { htmlEncode, sanitizeHtml } from "../../base";
import { replaceAll } from "../../compat";
import { Format, IRemoteView } from "../../slick";

export namespace SlickFormatting {
    
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

            if ((ctx.item as any)?.__nonDataRow) {
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
                    if (!!!!(ctx.item as any)._collapsed) {
                        toggle.classList.add("s-TreeExpand");
                    }
                    else {
                        toggle.classList.add("s-TreeCollapse");
                    }
                }
            }

            if (text instanceof Element) {
                var fragment = document.createDocumentFragment();
                fragment.appendChild(spacer);
                fragment.appendChild(toggle);
                fragment.appendChild(text);
                return fragment;
            }
            else if (text instanceof DocumentFragment) {
                text.prepend(toggle);
                text.prepend(spacer);
                return text;
            }
            else
                return (spacer.outerHTML + toggle.outerHTML + (text ?? ""));
        };
    }
}

const contentOnly = { contentOnly: true };