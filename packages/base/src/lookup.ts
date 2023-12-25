export interface LookupOptions<TItem> {
    idField?: string;
    parentIdField?: string;
    textField?: string;
}

export interface Lookup<TItem> {
    items: TItem[];
    itemById: { [key: string]: TItem };
    idField: string;
    parentIdField: string;
    textField: string;
}

export class Lookup<TItem> {
    public items: TItem[] = [];
    public itemById: { [key: string]: TItem } = {};
    public idField: string;
    public parentIdField: string;
    public textField: string;

    constructor(options: LookupOptions<TItem>, items?: TItem[]) {
        options = options || {};
        this.idField = options.idField;
        this.parentIdField = options.parentIdField;
        this.textField = options.textField;

        if (items != null)
            this.update(items);
    }

    update?(value: TItem[]) {
        this.items = [];
        this.itemById = {};
        if (value) {
            for (var k of value) {
                if (k == null || typeof k !== "object") // special case for distinct lookup
                    this.items.push({ [this.idField]: k, [this.textField]: k } as any);
                else
                    this.items.push(k);
            }
        }
        var idField = this.idField;
        if (idField) {
            for (var r of this.items) {
                var v = (r as any)[idField];
                if (v != null) {
                    this.itemById[v] = r;
                }
            }
        }
    }
}