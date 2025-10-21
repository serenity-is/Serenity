import { EditorClass, ValidationResult } from "./editing";
import type { AsyncPostCleanup, AsyncPostRender, ColumnFormat, CompatFormatter, FormatterContext, FormatterResult } from "./formatting";
import { IGroupTotals } from "./group";

export interface Column<TItem = any> {
    asyncPostRender?: AsyncPostRender<TItem>;
    asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
    behavior?: any;
    cannotTriggerInsert?: boolean;
    cssClass?: string;
    defaultSortAsc?: boolean;
    editor?: EditorClass;
    editorFixedDecimalPlaces?: number;
    field?: string;
    frozen?: boolean | "start" | "end";
    focusable?: boolean;
    footerCssClass?: string;
    format?: ColumnFormat<TItem>;
    /** @deprecated, use @see format */
    formatter?: CompatFormatter<TItem>;
    groupTotalsFormat?: (ctx: FormatterContext<IGroupTotals<TItem>>) => FormatterResult;
    /** @deprecated, use @see groupTotalsFormat */
    groupTotalsFormatter?: (totals?: IGroupTotals<TItem>, column?: Column<TItem>, grid?: unknown) => string;
    headerCssClass?: string;
    id?: string;
    maxWidth?: any;
    minWidth?: number;
    name?: string;
    nameFormat?: (ctx: FormatterContext<TItem>) => FormatterResult;
    previousWidth?: number;
    referencedFields?: string[];
    rerenderOnResize?: boolean;
    resizable?: boolean;
    selectable?: boolean;
    sortable?: boolean;
    sortOrder?: number;
    toolTip?: string;
    validator?: (value: any, editorArgs?: any) => ValidationResult;
    visible?: boolean;
    width?: number;
}


export const columnDefaults: Partial<Column> = {
    resizable: true,
    sortable: false,
    minWidth: 30,
    rerenderOnResize: false,
    defaultSortAsc: true,
    focusable: true,
    selectable: true
};

export interface ColumnMetadata<TItem = any> {
    colspan: number | '*';
    cssClasses?: string;
    focusable?: boolean;
    editor?: EditorClass;
    format?: ColumnFormat<TItem>;
    /** @deprecated */
    formatter?: CompatFormatter<TItem>;
    selectable?: boolean;
}

export interface ColumnSort {
    columnId: string;
    sortAsc?: boolean;
}

export interface ItemMetadata<TItem = any> {
    cssClasses?: string;
    columns?: { [key: string]: ColumnMetadata<TItem> };
    focusable?: boolean;
    format?: ColumnFormat<TItem>;
    /** @deprecated */
    formatter?: CompatFormatter<TItem>;
    selectable?: boolean;
}

export function initializeColumns(columns: Column[], defaults: Partial<Column<any>>) {
    var usedIds: { [key: string]: boolean } = {};

    for (var i = 0; i < columns.length; i++) {
        var m = columns[i];

        if (defaults != null) {
            for (var k in defaults) {
                if ((m as any)[k] === undefined)
                    (m as any)[k] = (defaults as any)[k];
            }
        }

        if (m.minWidth && m.width < m.minWidth)
            m.width = m.minWidth;

        if (m.maxWidth && m.width > m.maxWidth)
            m.width = m.maxWidth;

        if (m.id == null ||
            usedIds[m.id]) {
            const prefix = m.id != null && m.id.length ? m.id :
                m.field != null ? m.field : ('col');
            var x = 0;
            while (usedIds[(m.id = prefix + (x == 0 ? "" : '_' + x.toString()))]) x++;
        }

        usedIds[m.id] = true;

        if (m.name === void 0) {
            m.name = titleize(m.field ?? m.id);
        }
    }
}

export function titleize(str: string) {
    if (!str)
        return str;

    str = "" + str;

    // Simple character-by-character approach to avoid ReDoS vulnerabilities
    let result = "";
    for (let i = 0; i < str.length; i++) {
        const char = str[i];
        const prevChar = i > 0 ? str[i - 1] : null;
        const nextChar = i < str.length - 1 ? str[i + 1] : null;

        // Insert underscore before uppercase letters in these cases:
        // 1. lowercase/digit followed by uppercase (camelCase -> camel_Case)
        // 2. uppercase followed by uppercase+lowercase, but not at the start of consecutive caps
        if (char >= 'A' && char <= 'Z' && prevChar) {
            if (prevChar >= 'a' && prevChar <= 'z' || prevChar >= '0' && prevChar <= '9') {
                // Case 1: lowercase/digit -> uppercase
                result += '_';
            } else if (prevChar >= 'A' && prevChar <= 'Z' && nextChar && nextChar >= 'a' && nextChar <= 'z') {
                // Case 2: we're in a sequence of uppercase letters followed by lowercase
                // Find the start of this uppercase sequence
                let seqStart = i - 1;
                while (seqStart > 0 && str[seqStart - 1] >= 'A' && str[seqStart - 1] <= 'Z') {
                    seqStart--;
                }
                // Insert underscore before the last uppercase before the lowercase
                if (i > seqStart) {
                    result += '_';
                }
            }
        }

        // Replace hyphens and whitespace with underscores
        if (char === '-' || /\s/.test(char)) {
            result += '_';
        } else {
            result += char;
        }
    }

    // Convert to lowercase and clean up
    result = result.toLowerCase().replace(/_+/g, '_');

    // Split into words, filter out empty strings, and title case each word
    return result.split('_').filter(x => x.length)
        .map(x => x.charAt(0).toUpperCase() + x.substring(1).toLowerCase()).join(' ');
}
