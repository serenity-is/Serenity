import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { Culture, htmlEncode, isArrayLike } from "../../base";
import { replaceAll } from "../../compat";
import { Format, RemoteView } from "../../slick";
import { DateFormatter, EnumFormatter, NumberFormatter } from "../formatters/formatters";

export namespace SlickFormatting {
    export function getEnumText(enumKey: string, name: string): string {
        return EnumFormatter.getText(enumKey, name);
    }

    export function treeToggle(getView: () => RemoteView<any>, getId: (x: any) => any,
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

    export function date(format?: string): Format {
        if (format == null) {
            format = Culture.dateFormat;
        }

        return function (ctx: FormatterContext) {
            return ctx.escape(DateFormatter.format(ctx.value, format));
        };
    }

    export function dateTime(format?: string): Format {
        if (format == null) {
            format = Culture.dateTimeFormat;
        }
        return function (ctx: FormatterContext) {
            return ctx.escape(DateFormatter.format(ctx.value, format));
        };
    }

    export function checkBox(): Format {
        return function (ctx: FormatterContext) {
            return '<span class="check-box no-float ' + (!!ctx.value ? ' checked' : '') + '"></span>';
        };
    }

    export function number(format: string): Format {
        return function (ctx: FormatterContext) {
            return NumberFormatter.format(ctx.value, format);
        };
    }

    export function getItemType(link: HTMLElement | ArrayLike<HTMLElement>): string {
        return (isArrayLike(link) ? link[0] : link)?.getAttribute('data-item-type');
    }

    export function getItemId(link: HTMLElement | ArrayLike<HTMLElement>): string {
        var value = (isArrayLike(link) ? link[0] : link)?.getAttribute('data-item-id');
        return value == null ? null : value.toString();
    }

    export function itemLinkText(itemType: string, id: any, text: FormatterResult,
        extraClass: string, encode: boolean): FormatterResult {
        const link = <a class={[`s-EditLink s-${replaceAll(itemType, '.', '-')}Link`, extraClass]}
            href={id != null ? "#" + replaceAll(itemType, '.', '-') + '/' + id : ''}
            data-item-type={itemType} data-item-id={id} /> as HTMLAnchorElement;

        if (text instanceof Node) {
            link.append(text);
            return link;
        }

        if (text != null && text !== "") {
            if (encode) {
                link.textContent = text;
            }
            else {
                link.innerHTML = text;
            }
        }

        return link.outerHTML;
    }

    export function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>,
        cssClass?: (ctx: FormatterContext<TItem>) => string, encode?: boolean): Format<TItem> {
        return function (ctx: FormatterContext<TItem>) {
            var text: FormatterResult = (getText == null ? ctx.value : getText(ctx)) ?? '';
            if ((ctx.item as any)?.__nonDataRow) {
                return text instanceof Node ? text : encode ? htmlEncode(text) : text;
            }

            return itemLinkText(itemType, (ctx.item as any)[idField], text,
                (cssClass == null ? '' : cssClass(ctx)), encode);
        };
    }
}
