import { registerEnum } from "./system";

/**
 * Aggregation type used for grid / grouping summaries.
 * Controls how a column's values are aggregated in group headers or footers.
 */
export enum SummaryType {
    /** Summaries are disabled for the column. */
    Disabled = -1,
    /** No summary. */
    None = 0,
    /** Sum of values. */
    Sum = 1,
    /** Average of values. */
    Avg = 2,
    /** Minimum value. */
    Min = 3,
    /** Maximum value. */
    Max = 4
}

registerEnum(SummaryType, 'Serenity.SummaryType');

/**
 * Callback that augments an editor with additional UI alongside the editor element.
 * Used via {@link PropertyItem.editorAddons} to inject buttons or custom fragments.
 * @param props - Context supplied to the addon.
 * @param props.propertyItem - Metadata for the field the editor belongs to, if available.
 * @param props.editorElement - Root DOM element of the editor.
 * @param props.documentFragment - Optional document fragment the addon can attach to when the editor is rendered inside a fragment.
 * @returns Void; the addon is expected to manipulate the DOM directly.
 */
export type EditorAddon = (props: { propertyItem?: PropertyItem, editorElement: HTMLElement, documentFragment?: DocumentFragment }) => void;

/**
 * Describes a single field / column / form property as returned by server-side metadata.
 * Drives form generation, grid columns, editors, formatters and filtering. Each
 * property corresponds to a row field or an unbound UI field.
 */
export interface PropertyItem {
    /** Field / property key (usually the row field name). */
    name: string;
    /** Display title / column header / form label. Falls back to `name` when omitted. */
    title?: string;
    /** Tooltip / hint shown on hover or beside the label. */
    hint?: string;
    /** Placeholder text for the editor input. */
    placeholder?: string;
    /** Editor type key (e.g. `"String"`, `"Date"`) or a constructor / lazy import for a custom editor. */
    editorType?: string | { new(props?: any): any } | PromiseLike<{ new(props?: any): any }>;
    /** Options passed to the editor constructor. */
    editorParams?: any;
    /** Addons rendered alongside the editor (e.g. buttons). Each entry specifies a type key or {@link EditorAddon} callback and optional params. */
    editorAddons?: { type: string | EditorAddon, params?: any }[];
    /** Extra CSS class(es) applied to the editor element. */
    editorCssClass?: string;
    /** Category group shown as a collapsible section in forms. */
    category?: string;
    /** Whether the {@link category} section can be collapsed. */
    collapsible?: boolean;
    /** Whether the category starts collapsed. Only meaningful when {@link collapsible} is true. */
    collapsed?: boolean;
    /** Tab name the field belongs to when the form uses tabs. */
    tab?: string;
    /** CSS class applied to the grid cell / column. */
    cssClass?: string;
    /** CSS class applied to the column header. */
    headerCssClass?: string;
    /** CSS class applied to the form field container (`<div class="field">`). */
    formCssClass?: string;
    /** Maximum string length for validation. */
    maxLength?: number;
    /** Whether a value is required. */
    required?: boolean;
    /** Whether the field can be set on insert. When false the field is read-only during creation. */
    insertable?: boolean;
    /** Permission key required to set the field on insert. */
    insertPermission?: string;
    /** Hides the field in insert (create) mode. */
    hideOnInsert?: boolean;
    /** Whether the field can be updated after creation. */
    updatable?: boolean;
    /** Permission key required to update the field. */
    updatePermission?: string;
    /** Hides the field in update (edit) mode. */
    hideOnUpdate?: boolean;
    /** Whether the field is read-only in the UI (still submitted unless {@link skipOnSave} is set). */
    readOnly?: boolean;
    /** Permission key required to read / view the field. Clients may hide the field when the user lacks it. */
    readPermission?: string;
    /** When true the field is not populated on load (e.g. sensitive data). */
    skipOnLoad?: boolean;
    /** When true the field value is not sent back on save. */
    skipOnSave?: boolean;
    /** True for unbound fields that do not map to a row column (e.g. calculated UI-only fields). */
    unbound?: boolean;
    /** @deprecated use {@link skipOnSave} instead — kept for backward compatibility. */
    oneWay?: boolean;
    /** Default value applied to new records / empty editors. */
    defaultValue?: any;
    /** Whether the field supports per-language values (requires Localizations). */
    localizable?: boolean;
    /** Whether the column / field is visible by default. Hidden columns can still be shown via column picker. */
    visible?: boolean;
    /** Whether the user is allowed to hide the column via the column picker. */
    allowHide?: boolean;
    /** Whether the column can receive focus / be navigated via keyboard. */
    focusable?: boolean;
    /** Formatter type key or constructor / lazy import used to render the cell value. */
    formatterType?: string | { new(props?: any): { format(ctx: any): string } } | PromiseLike<{ new(props?: any): { format(ctx: any): string } }>;
    /** Options passed to the formatter. */
    formatterParams?: any;
    /** Display format string (e.g. date / number format) consumed by the formatter. */
    displayFormat?: string;
    /** Horizontal alignment for the column (`"left" | "center" | "right"` or similar). */
    alignment?: string;
    /** Pin / freeze the column to the start (left) or end (right) of the grid, or `true` for start. */
    pin?: "start" | "end" | boolean;
    /** Preferred column width in pixels. */
    width?: number;
    /** True when {@link width} was explicitly set (vs. auto-calculated). */
    widthSet?: boolean;
    /** Minimum column width in pixels. */
    minWidth?: number;
    /** Maximum column width in pixels. */
    maxWidth?: number;
    /** Width of the form label for this field (e.g. `"150px"`). */
    labelWidth?: string;
    /** Whether the column is user-resizable. */
    resizable?: boolean;
    /** Whether to show a selection checkbox column behavior for this column. */
    showSelection?: boolean;
    /** Whether the column can be sorted. */
    sortable?: boolean;
    /** Default sort order index (lower values are sorted first). Negative or undefined means no default sort. */
    sortOrder?: number;
    /** Whether the column participates in tab-stop navigation. */
    tabbable?: boolean;
    /** Order index for grouping; controls group-by precedence when multiple columns are grouped. */
    groupOrder?: number;
    /** Aggregation used for group / footer summaries. See {@link SummaryType}. */
    summaryType?: SummaryType;
    /** When true the cell value is rendered as a link that opens the record's edit dialog. */
    editLink?: boolean;
    /** Row type key used for the edit link dialog (defaults to the current row type). */
    editLinkItemType?: string;
    /** Field name that provides the ID for the edit link (defaults to the identity field). */
    editLinkIdField?: string;
    /** Extra CSS class for the edit link anchor. */
    editLinkCssClass?: string;
    /** Filter editor type key that determines the filtering UI for this field (e.g. `"String"`, `"Date"`). */
    filteringType?: string;
    /** Options passed to the filtering editor. */
    filteringParams?: any;
    /** Field that provides the ID value for filtering (useful for lookup display fields). */
    filteringIdField?: string;
    /** When true the column cannot be used as a filter criterion. */
    notFilterable?: boolean;
    /** When true the field appears only in filter dialogs / panels and not in the grid itself. */
    filterOnly?: boolean;
    /** Whether the field appears in the quick-filter bar above the grid. */
    quickFilter?: boolean;
    /** Options for the quick-filter editor. */
    quickFilterParams?: any;
    /** When true a separator is rendered before this quick filter in the bar. */
    quickFilterSeparator?: boolean;
    /** Extra CSS class for the quick-filter item. */
    quickFilterCssClass?: string;
}

/**
 * Alias for {@link PropertyItem}.
 * Prefer {@link UIFieldItem} for new code; `PropertyItem` is kept for compatibility and may be phased out.
 */
export type UIFieldItem = PropertyItem;

/**
 * Metadata bundle for a form or columns set.
 * @property items - Primary fields / columns in display order.
 * @property additionalItems - Extra fields not shown by default but available via column picker or customization (e.g. audit fields).
 */
export interface PropertyItemsData {
    /** Primary fields / columns in display order. */
    items: PropertyItem[];
    /** Extra fields available on demand (e.g. via column picker); not rendered initially. */
    additionalItems: PropertyItem[];
}

/**
 * Alias for {@link PropertyItemsData}.
 * Prefer {@link UIFieldSet} for new code; `PropertyItemsData` is kept for compatibility.
 */
export type UIFieldSet = PropertyItemsData;