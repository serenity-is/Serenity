import { FilteredLookupInDetailForm } from "../../ServerTypes/Demo";
import { Decorators, EntityDialog, Widget, WidgetProps, gridPageInit, toId } from "@serenity-is/corelib";
import { OrderDetailDialog, OrderDetailForm, OrderDetailsEditor, OrderGrid, OrderRow, OrderService, ProductRow } from "@serenity-is/demo.northwind";
import "./FilteredLookupInDetailPage.css";

export default () => gridPageInit(FilteredLookupInDetailGrid);

/**
 * Subclass of OrderGrid to override dialog type to FilteredLookupInDetailDialog
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.FilteredLookupInDetailGrid')
export class FilteredLookupInDetailGrid extends OrderGrid {

    protected getDialogType() { return FilteredLookupInDetailDialog; }
}

/**
 * Our subclass of order detail dialog with a CategoryID property
 * that will be used to set CascadeValue of product editor
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.FilteredLookupOrderDetailDialog')
export class FilteredLookupOrderDetailDialog extends OrderDetailDialog {

    constructor(props: any) {
        super(props);

        this.form = new OrderDetailForm(this.idPrefix);

        // we can set cascade field in constructor
        // we could also use FilterField but in this case, when CategoryID is null
        // lookup editor would show all products in any category
        this.form.ProductID.cascadeField = ProductRow.Fields.CategoryID;

        // but CategoryID value is not yet available here as detail editor will set it 
        // after calling constructor (creating a detail dialog) so we'll use BeforeLoadEntity
    }

    /**
     * This method is called just before an entity is loaded to dialog
     * This is also called for new record mode with an empty entity
     */
    protected beforeLoadEntity(entity) {
        super.beforeLoadEntity(entity);

        // setting cascade value here
        // make sure you have [LookupInclude] on CategoryID property of ProductRow
        // otherwise this field won't be available in lookup script (will always be null),
        // so can't be filtered and you'll end up with an empty product list.
        this.form.ProductID.cascadeValue = this.categoryID;
    }

    declare public categoryID: number;
}

/**
 * Our subclass of Order Details editor with a CategoryID property
 */
@Decorators.registerEditor('Serenity.Demo.BasicSamples.FilteredLookupDetailEditor')
export class FilteredLookupDetailEditor<P = {}> extends OrderDetailsEditor<P> {

    protected getDialogType() { return FilteredLookupOrderDetailDialog; }

    declare public categoryID: number;

    /**
     * This method is called to initialize an edit dialog created by
     * grid editor when Add button or an edit link is clicked
     * We have an opportunity here to pass CategoryID to edit dialog
     */
    protected initEntityDialog(itemType: string, dialog: Widget<any>) {
        super.initEntityDialog(itemType, dialog);

        // passing category ID from grid editor to detail dialog
        (dialog as FilteredLookupOrderDetailDialog).categoryID = this.categoryID;
    }
}

/**
 * Basic order dialog with a category selection
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.FilteredLookupInDetailDialog')
export class FilteredLookupInDetailDialog<P = {}> extends EntityDialog<OrderRow, P> {

    protected getFormKey() { return FilteredLookupInDetailForm.formKey; }
    protected getRowDefinition() { return OrderRow; }
    protected getService() { return OrderService.baseUrl; }

    declare private form: FilteredLookupInDetailForm;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.form = new FilteredLookupInDetailForm(this.idPrefix);
        this.form.CategoryID.change(e => {
            this.form.DetailList.categoryID = toId(this.form.CategoryID.value);
        });
    }
}