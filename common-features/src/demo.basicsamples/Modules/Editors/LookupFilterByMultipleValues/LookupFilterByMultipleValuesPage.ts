import { Criteria, EntityGrid, ListRequest, Lookup, LookupEditorBase, LookupEditorOptions, gridPageInit } from "@serenity-is/corelib";
import { CategoryRow, ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { LookupFilterByMultipleForm } from "../../ServerTypes/Demo";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";
import "./LookupFilterByMultipleValuesPage.css";

export default () => gridPageInit(LookupFilterByMultipleGrid);

/**
 * Subclass of ProductGrid to override dialog type to CloneableEntityDialog
 */
export class LookupFilterByMultipleGrid<P = {}> extends EntityGrid<ProductRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getColumnsKey() { return ProductColumns.columnsKey; }
    protected override getDialogType() { return LookupFilterByMultipleDialog; }
    protected override getRowDefinition() { return ProductRow; }
    protected override getService() { return ProductService.baseUrl; }

    /**
     * This method is called just before List request is sent to service.
     * You have an opportunity here to cancel request or modify it.
     * Here we'll add a custom criteria to list request.
     */
    protected override setViewParams() {
        super.setViewParams();
        
        // this has no relation to our lookup editor but as we'll allow picking only 
        // categories of Produce and Seafood in product dialog, it's better to show
        // only products from these categories in grid too
        let request = this.view.params as ListRequest;
        request.Criteria = Criteria.and(request.Criteria,
            Criteria(ProductRow.Fields.CategoryName).in(['Produce', 'Seafood']));
    }
}

/**
 * This is our custom product dialog that uses a different product form
 * (LookupFilterByMultipleForm) with our special category editor.
 */
export class LookupFilterByMultipleDialog extends ProductDialog {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getFormKey() { return LookupFilterByMultipleForm.formKey; }
}

/**
 * This is our category editor that will show only categories of Produce and
 * Seafood. We are subclassing LookupEditorBase which also LookupEditor
 * derives from.
 *
 * After compiling and transforming templates, this editor type will be
 * available in server side to use in our LookupFilterByMultipleForm,
 * which is a version of ProductForm that uses our custom editor.
 */
export class ProduceSeafoodCategoryEditor extends
    LookupEditorBase<LookupEditorOptions, CategoryRow> {
    static override[Symbol.typeInfo] = this.registerEditor(nsDemoBasicSamples);

    /**
     * Normally LookupEditor requires a lookup key to determine which set of
     * lookup data to show in editor. As our editor will only show category
     * data, we lock it to category lookup key.
     */
    protected override getLookupKey() {
        return CategoryRow.lookupKey;
    }

    /**
     * Here we are filtering by category name but you could filter by any field.
     * Just make sure the fields you filter on has [LookupInclude] attribute on them,
     * otherwise their value will be null in client side as they are not sent back
     * from server in lookup script.
     */
    protected override getItems(lookup: Lookup<CategoryRow>) {
        return super.getItems(lookup).filter(x =>
            x.CategoryName === 'Produce' || x.CategoryName === 'Seafood');
    }
}