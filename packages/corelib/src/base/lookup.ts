/**
 * Options used to construct a {@link Lookup}.
 * @typeParam TItem - Type of the lookup items.
 */
export interface LookupOptions<TItem> {
    /** Name of the field that holds the unique identifier for an item. Used as the key in {@link Lookup.itemById}. */
    idField?: string;
    /** Name of the field that holds the parent identifier (for hierarchical lookups). */
    parentIdField?: string;
    /** Name of the field that holds the human-readable display text for an item. */
    textField?: string;
}

/**
 * Client-side lookup data structure that holds a flat list of items and a fast key-based index.
 * Implementations are typically returned from server-side `LookupScript` endpoints and consumed by editors such as `LookupEditor`.
 * @typeParam TItem - Type of the lookup items.
 */
export interface Lookup<TItem> {
    /** Flat array of all lookup items. */
    items: TItem[];
    /** Dictionary mapping stringified {@link LookupOptions.idField} values to their corresponding items. */
    itemById: { [key: string]: TItem };
    /** Name of the ID field (copied from {@link LookupOptions.idField}). */
    idField: string;
    /** Name of the parent-ID field for hierarchical lookups (copied from {@link LookupOptions.parentIdField}). */
    parentIdField: string;
    /** Name of the display-text field (copied from {@link LookupOptions.textField}). */
    textField: string;
}

/**
 * Concrete implementation of the {@link Lookup} interface for client-side use.
 * Maintains `items` and a `itemById` index synchronized via {@link Lookup.update}.
 * @typeParam TItem - Type of the lookup items.
 * @example
 * ```ts
 * const lookup = new Lookup<{ id: number; name: string }>({ idField: "id", textField: "name" }, items);
 * lookup.itemById["5"] // item with id 5
 * ```
 */
export class Lookup<TItem> {
    /** Flat array of all lookup items. */
    declare public items: TItem[];
    /** Dictionary mapping stringified ID values to items, rebuilt on every {@link update}. */
    declare public itemById: { [key: string]: TItem };
    /** Name of the ID field (from constructor options). */
    declare public idField: string;
    /** Name of the parent-ID field for hierarchical lookups (from constructor options). */
    declare public parentIdField: string;
    /** Name of the display-text field (from constructor options). */
    declare public textField: string;

    /**
     * Creates a new lookup instance.
     * @param options - Field mapping for id/parent/text. Pass `null`/`undefined` for an empty configuration (fields remain `undefined`).
     * @param items - Optional initial item array. If provided, {@link update} is called immediately to populate `items` and `itemById`.
     */
    constructor(options: LookupOptions<TItem>, items?: TItem[]) {
        this.items = []
        this.itemById = {};
        options = options || {};
        this.idField = options.idField;
        this.parentIdField = options.parentIdField;
        this.textField = options.textField;

        if (items != null)
            this.update(items);
    }

    /**
     * Replaces the lookup contents and rebuilds the `itemById` index.
     * @param value - New item array. `null`/`undefined` clears the lookup. Primitive values (e.g. `string` numbers from a distinct query) are auto-wrapped as `{ [idField]: value, [textField]: value }`.
     * @remarks Re-initializes both {@link Lookup.items} and {@link Lookup.itemById}. The method is declared optional (`update?`) on the interface for compatibility with plain-object lookups, but is always present on this class.
     */
    update?(value: TItem[]) {
        this.items = [];
        this.itemById = {};
        if (value) {
            for (const k of value) {
                if (k == null || typeof k !== "object") // special case for distinct lookup
                    this.items.push({ [this.idField]: k, [this.textField]: k } as any);
                else
                    this.items.push(k);
            }
        }
        const idField = this.idField;
        if (idField) {
            for (const r of this.items) {
                const v = (r as any)[idField];
                if (v != null) {
                    this.itemById[v] = r;
                }
            }
        }
    }
}