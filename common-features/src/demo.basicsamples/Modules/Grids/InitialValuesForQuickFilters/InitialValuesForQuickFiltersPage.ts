import { EnumEditor, LookupEditor, QuickFilter, Widget, formatDate, gridPageInit } from "@serenity-is/corelib";
import { OrderGrid, OrderRow, OrderShippingState } from "@serenity-is/demo.northwind";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(InitialValuesForQuickFilters);

// get a reference to order row field names
const fld = OrderRow.Fields;

export class InitialValuesForQuickFilters extends OrderGrid {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    /**
     * This method is called to get list of quick filters to be created for this grid.
     * By default, it returns quick filter objects corresponding to properties that
     * have a [QuickFilter] attribute at server side OrderColumns.cs
     */
    protected override getQuickFilters(): QuickFilter<Widget<any>, any>[] {

        // get quick filter list from base class
        let filters = super.getQuickFilters();

        // quick filter init method is a good place to set initial
        // value for a quick filter editor, just after it is created

        let orderDate = filters.find(x => x.field === fld.OrderDate);
        orderDate && (orderDate.init = (w) => {
            // w is a reference to the editor for this quick filter widget. the quick filter of a date field edits a
            // range by default, in two date editors, but an application can give its date fields a filter of its own,
            // e.g. one that edits the range in a single input, see QuickFilterBarDefaults.dateRangeFilter, so this
            // does not tell the editors apart. the state of the filter is the start and the end of the range for
            // both, so it is set through loadState, the way a saved state of the filter is restored.
            // note that in Javascript, months are 0 based, so the end date below is actually November 1st
            const now = new Date();
            const end = new Date(now.getFullYear(), 10, 1);

            orderDate.loadState(w, ["2010-05-01", formatDate(end, "yyyy-MM-dd")]);
        });

        let shippingState = filters.find(x => x.field === fld.ShippingState);
        shippingState && (shippingState.init = (w: EnumEditor) => {
            // enum editor has a string value, so need to call toString()
            w.value = OrderShippingState.NotShipped.toString()
        });

        return filters;
    }

    /**
     * This method is another possible place to modify quick filter widgets.
     * It is where the quick filter widgets are actually created.
     * 
     * By default, it calls getQuickFilters() then renders UI for these
     * quick filters.
     *
     * We could use getQuickFilters() method for ShipVia too,
     * but this is for demonstration purposes
     */
    protected override createQuickFilters(): void {

        // let base class to create quick filters first
        super.createQuickFilters();

        // find a quick filter widget by its field name
        this.findQuickFilter(LookupEditor, fld.ShipVia).values = ["1", "2"];
    }
}