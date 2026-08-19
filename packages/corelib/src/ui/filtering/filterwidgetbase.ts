import { nsSerenity } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";
import { FilterStore } from "./filterstore";

/**
 * Base widget that owns a {@link FilterStore} and reacts to its changes.
 * @typeParam P - Widget props type.
 */
export class FilterWidgetBase<P = {}> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private store: FilterStore;

    declare private onFilterStoreChanged: () => void;

    /**
     * Creates a filter widget base.
     * @param props - Widget props.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        this.store = new FilterStore([]);
        this.onFilterStoreChanged = () => this.filterStoreChanged();
        this.store.add_changed(this.onFilterStoreChanged);
    }

    /**
     * Cleans up the filter store subscription.
     */
    override destroy() {

        if (this.store) {
            this.store.remove_changed(this.onFilterStoreChanged);
            this.onFilterStoreChanged = null;
            this.store = null;
        }

        super.destroy();
    }

    /**
     * Hook invoked when the filter store changes.
     */
    protected filterStoreChanged() {
    }

    /**
     * Returns the filter store.
     * @returns The filter store.
     */
    get_store(): FilterStore {
        return this.store;
    }

    /**
     * Sets the filter store and subscribes to its changes.
     * @param value - The filter store.
     */
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