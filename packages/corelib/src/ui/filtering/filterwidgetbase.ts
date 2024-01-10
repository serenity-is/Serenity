import { Decorators } from "../../types/decorators";
import { TemplatedWidget } from "../widgets/templatedwidget";
import { WidgetProps } from "../widgets/widget";
import { FilterStore } from "./filterstore";

@Decorators.registerClass('Serenity.FilterWidgetBase')
export class FilterWidgetBase<P = {}> extends TemplatedWidget<P> {
    private store: FilterStore;

    private onFilterStoreChanged: () => void;

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