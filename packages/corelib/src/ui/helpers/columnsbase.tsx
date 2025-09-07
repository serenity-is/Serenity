import { Column } from "@serenity-is/sleekgrid";

export class ColumnsBase<TRow = any> {
    constructor(items: Column<TRow>[]) {
        (this as any).__items = items;
        for (var col of items) {
            let id = col.id;
            if (id && !(this as any)[id])
                (this as any)[id] = col;
        }
        for (var col of items) {
            let id = col.sourceItem?.name;
            if (id && !(this as any)[id])
                (this as any)[id] = col;
        }
        for (var col of items) {
            let id = col.field;
            if (id && !(this as any)[id])
                (this as any)[id] = col;
        }
    }

    valueOf(): Column<TRow>[] {
        return (this as any).__items;
    }
}
