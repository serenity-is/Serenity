export interface LookupOptions<TItem> {
    idField?: string;
    parentIdField?: string;
    textField?: string;
    textFormatter?(item: TItem): string;
    /**
     * Lookup cache expiration time in minutes (default: 60)
     * Set to 0 to disable expiration (cache indefinitely)
     * Clients can use isExpired() to check if lookup needs refresh
     */
    expireMinutes?: number;
}

declare global {
    namespace Q {
        export interface Lookup<TItem> {
            items: TItem[];
            itemById: { [key: string]: TItem };
            idField: string;
            parentIdField: string;
            textField: string;
            textFormatter: (item: TItem) => string;
            expireMinutes: number;
            isExpired(): boolean;
            getExpirationMinutes(): number;
        }
    }
}

export class Lookup<TItem> {
    public items: TItem[] = [];
    public itemById: { [key: string]: TItem } = {};
    public idField: string;
    public parentIdField: string;
    public textField: string;
    public textFormatter: (item: TItem) => string;
    public expireMinutes: number;
    private expireTime: number;

    constructor(options: LookupOptions<TItem>, items?: TItem[]) {
        options = options || {};
        this.textFormatter = options.textFormatter;
        this.idField = options.idField;
        this.parentIdField = options.parentIdField;
        this.textField = options.textField;
        this.textFormatter = options.textFormatter;
        this.expireMinutes = options.expireMinutes ?? 60;

        if (items != null)
            this.update(items);
    }

    update(value: TItem[]) {
        this.items = [];
        this.itemById = {};
        if (value) {
            for (var k of value)
                this.items.push(k);
        }
        var idField = this.idField;
        if (idField) {
            for (var r of this.items) {
                var v = r[idField];
                if (v != null) {
                    this.itemById[v] = r;
                }
            }
        }
        this.expireTime = Date.now() + (this.expireMinutes * 60000);
    }

    isExpired(): boolean {
        if (!this.expireTime)
            return false;
        return Date.now() >= this.expireTime;
    }

    getExpirationMinutes(): number {
        if (!this.expireTime)
            return -1;
        return Math.max(0, Math.ceil((this.expireTime - Date.now()) / 60000));
    }

    protected get_idField() {
        return this.idField;
    }

    protected get_parentIdField() {
        return this.parentIdField;
    }

    protected get_textField() {
        return this.textField;
    }

    protected get_textFormatter() {
        return this.textFormatter;
    }

    protected get_itemById() {
        return this.itemById;
    }

    protected get_items() {
        return this.items;
    }
}