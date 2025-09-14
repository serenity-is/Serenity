import { nsSerenity } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";
import { FilterStore } from "./filterstore";

export class FilterWidgetBase<P = {}> extends Widget<P> {
    static [Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private store: FilterStore;

    declare private onFilterStoreChanged: () => void;

    constructor(props: WidgetProps<P>) {
        super(props);

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