import { EditorClass, ValidationResult } from "./editing";
import type { AsyncPostCleanup, AsyncPostRender, ColumnFormat, CompatFormatter, FormatterContext, FormatterResult } from "./formatting";
import { IGroupTotals } from "./group";
import type { ISleekGrid } from "./isleekgrid";

/**
 * Definition of a single grid column.
 * @template TItem - Row item type the column belongs to.
 */
export interface Column<TItem = any> {
    /** Async post-render hook invoked after the cell node is attached to the DOM. */
    asyncPostRender?: AsyncPostRender<TItem>;
    /** Cleanup counterpart to `asyncPostRender`; called before the node is removed or re-rendered. */
    asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
    /** Arbitrary behavior token consumed by plugins (e.g. `"selectAndMove"`). */
    behavior?: any;
    /** When `true`, editing this column cannot trigger insertion of a new row. */
    cannotTriggerInsert?: boolean;
    /** CSS class(es) applied to every body cell in this column. */
    cssClass?: string;
    /** Default sort direction for this column; `true` means ascending. */
    defaultSortAsc?: boolean;
    /** Editor class used when the cell enters edit mode. */
    editor?: EditorClass;
    /** Fixed number of decimal places the editor should preserve (if applicable). */
    editorFixedDecimalPlaces?: number;
    /** Property name on `TItem` that this column is bound to. */
    field?: string;
    /** Freezing / pinning of the column. `true`/`"start"` pins to the start side, `"end"` to the end side. */
    frozen?: boolean | "start" | "end";
    /** Whether cells in this column can receive focus. Defaults to `true`. */
    focusable?: boolean;
    /** CSS class(es) applied to footer row cells in this column. */
    footerCssClass?: string;
    /** Modern formatter for body cells. Prefer this over the deprecated `formatter`. */
    format?: ColumnFormat<TItem>;
    /**
     * Legacy formatter for body cells.
     * @deprecated Use {@link Column.format} instead.
     */
    formatter?: CompatFormatter<TItem>;
    /** Formatter used to render group-totals rows for this column. */
    groupTotalsFormat?: (ctx: FormatterContext<IGroupTotals<TItem>>) => FormatterResult;
    /**
     * Legacy group-totals formatter.
     * @deprecated Use {@link Column.groupTotalsFormat} instead.
     */
    groupTotalsFormatter?: (totals?: IGroupTotals<TItem>, column?: Column<TItem>, grid?: unknown) => string;
    /** CSS class(es) applied to the header cell. */
    headerCssClass?: string;
    /** Unique column identifier. Auto-generated from `field` or a fallback if omitted. */
    id?: string;
    /** Maximum pixel width the column may be resized to. */
    maxWidth?: any;
    /** Minimum pixel width the column may be resized to. */
    minWidth?: number;
    /** Display name shown in the header. Defaults to a titleized form of `field`/`id`. */
    name?: string;
    /** Formatter used to render the header `name` content. */
    nameFormat?: (ctx: FormatterContext<TItem>) => FormatterResult;
    /** Previous width before the last resize; managed internally for `forceFitColumns`. */
    previousWidth?: number;
    /** Extra field names the column depends on (besides `field`), used for dirty tracking. */
    referencedFields?: string[];
    /** When `true`, cells are re-rendered on column resize. */
    rerenderOnResize?: boolean;
    /** Whether the column can be resized by dragging its header border. */
    resizable?: boolean;
    /** Whether cells in this column can be selected. */
    selectable?: boolean;
    /** Whether cells in this column participate in tab navigation. */
    tabbable?: boolean;
    /** Whether clicking the header sorts by this column. */
    sortable?: boolean;
    /** Sort priority when multiple columns are sorted; lower numbers sort first. */
    sortOrder?: number;
    /** Tooltip text for the header cell. */
    toolTip?: string;
    /**
     * Optional validator invoked by the editor.
     * @param value - The value to validate.
     * @param editorArgs - Additional editor context, if any.
     * @returns Validation result indicating validity and an optional message.
     */
    validator?: (value: any, editorArgs?: any) => ValidationResult;
    /** Whether the column is currently visible. Columns with `visible: false` are hidden but retained. */
    visible?: boolean;
    /** Current pixel width of the column. */
    width?: number;
}


/**
 * Default property values applied to each column when none is specified.
 * Used as a fallback by {@link initColumnProps}.
 */
export const columnDefaults: Partial<Column> = {
    resizable: true,
    sortable: false,
    minWidth: 30,
    rerenderOnResize: false,
    defaultSortAsc: true,
    focusable: true,
    selectable: true,
    tabbable: true
};

/**
 * Per-cell metadata that can override column-level settings for a specific row.
 * @template TItem - Row item type.
 */
export interface ColumnMetadata<TItem = any> {
    /** Column span for this cell. Use `"*"` to span to the end of the row. */
    colspan?: number | '*';
    /** Extra CSS classes applied to the cell node. */
    cssClasses?: string;
    /** Whether the cell can receive focus. */
    focusable?: boolean;
    /** Editor class override for this cell. */
    editor?: EditorClass;
    /** Formatter override for this cell. */
    format?: ColumnFormat<TItem>;
    /**
     * Legacy formatter override.
     * @deprecated Use {@link ColumnMetadata.format} instead.
     */
    formatter?: CompatFormatter<TItem>;
    /** Whether the cell can be selected. */
    selectable?: boolean;
    /** Whether the cell participates in tab navigation. */
    tabbable?: boolean;
}

/**
 * Describes a single active sort criterion.
 */
export interface ColumnSort {
    /** Column `id` to sort by. */
    columnId: string;
    /** Sort direction; `true` for ascending, `false` for descending. */
    sortAsc?: boolean;
}

/**
 * Row-level metadata that can influence rendering and interaction.
 * Returned by `DataView.getItemMetadata(row)`.
 * @template TItem - Row item type.
 */
export interface ItemMetadata<TItem = any> {
    /** Extra CSS classes applied to the row node. */
    cssClasses?: string;
    /** Per-column metadata overrides for this row. */
    columns?: { [key: string]: ColumnMetadata<TItem> };
    /** Whether any cell in the row can receive focus. */
    focusable?: boolean;
    /** Default formatter for all cells in the row. */
    format?: ColumnFormat<TItem>;
    /**
     * Legacy default formatter for the row.
     * @deprecated Use {@link ItemMetadata.format} instead.
     */
    formatter?: CompatFormatter<TItem>;
    /** Whether any cell in the row can be selected. */
    selectable?: boolean;
    /** Whether any cell in the row participates in tab navigation. */
    tabbable?: boolean;
}

/**
 * Normalizes column definitions: applies defaults, clamps widths and ensures unique ids/names.
 * Mutates the `columns` array in place.
 * @param columns - Column definitions to initialize.
 * @param defaults - Default values to fall back to for missing properties.
 */
export function initColumnProps(columns: Column[], defaults: Partial<Column<any>>): void {
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

/**
 * Converts a field/column identifier to a human-readable Title Case string.
 * Handles camelCase, PascalCase, snake_case, kebab-case and whitespace separated names.
 * @param str - Raw identifier to titleize.
 * @returns Title-cased, space-separated string (e.g. `"firstName"` → `"First Name"`).
 */
export function titleize(str: string): string {
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
