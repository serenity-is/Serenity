import { Decorators } from "../../Decorators";
import { TemplatedWidget } from "../Widgets/TemplatedWidget";
import { FilterStore } from "./FilterStore";

@Decorators.registerClass('Serenity.FilterWidgetBase')
export class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
    private store: FilterStore;

    private onFilterStoreChanged: () => void;

    constructor(div: JQuery, opt?: TOptions) {
        super(div, opt);

        this.store = new FilterStore([]);
        this.onFilterStoreChanged = () => this.filterStoreChanged();
        this.store.add_changed(this.onFilterStoreChanged);
    }

    destroy() {

        if (this.store) {
            this.store.remove_changed(this.onFilterStoreChanged);
            this.onFilterStoreChanged = null;
            this.store = null;
        }

        super.destroy();
    }

    protected filterStoreChanged() {
    }

    get_store(): FilterStore {
        return this.store;
    }

    set_store(value: FilterStore): void {
        if (this.store !== value) {
            if (this.store != null) 
                this.store.remove_changed(this.onFilterStoreChanged);

            this.store = value || new FilterStore([]);
            this.store.add_changed(this.onFilterStoreChanged);
            this.filterStoreChanged();
        }
    }
}