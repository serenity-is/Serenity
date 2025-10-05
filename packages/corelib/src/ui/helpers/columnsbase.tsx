import { Column } from "@serenity-is/sleekgrid";

export class ColumnsBase<TRow = any> {
    declare private __items: Column<TRow>[];

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

    valueOf(): Column<TRow>[] {
        return this.__items;
    }
}
