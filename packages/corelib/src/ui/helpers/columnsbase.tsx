import { Column } from "@serenity-is/sleekgrid";

/**
 * Base class for column definitions. Exposes each column as a property of the
 * instance, keyed by its id, source item name, or field name.
 * @typeParam TRow - The type of the row data.
 */
export class ColumnsBase<TRow = any> {
    declare private __items: Column<TRow>[];

    /**
     * Creates a new ColumnsBase instance from the given column definitions.
     * @param items - The column definitions.
     */
    constructor(items: Column<TRow>[]) {
        let key: string;
        this.__items = items ?? [];
        for (const col of this.__items) {
            key = col.id;
            if (key && !(this as any)[key])
                (this as any)[key] = col;
        }
        for (const col of this.__items) {
            key = col.sourceItem?.name;
            if (key && !(this as any)[key])
                (this as any)[key] = col;
        }
        for (const col of this.__items) {
            key = col.field;
            if (key && !(this as any)[key])
                (this as any)[key] = col;
        }
    }

    /**
     * Returns the underlying column definitions array.
     * @returns The column definitions.
     */
    valueOf(): Column<TRow>[] {
        return this.__items;
    }
}
