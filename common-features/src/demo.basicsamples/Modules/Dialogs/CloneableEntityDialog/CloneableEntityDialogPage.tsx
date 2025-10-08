import { EntityGrid, gridPageInit } from "@serenity-is/corelib";
import { ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";
import { SampleInfo } from "../../sample-info";

export default () => {
    gridPageInit(CloneableEntityGrid);

    return <SampleInfo>
        <p>Product dialog here has a clone button. This is actually an integrated
            feature but clone button is invisible by default.</p>
        <p>Open an existing product and click clone button to create a new copy of it. </p>
        <p>See CloneableEntityDialog.ts for information about how to
            make this button visible and customizing cloned entity.</p>
    </SampleInfo>
}

/** Subclass of ProductGrid to override dialog type to CloneableEntityDialog */
export class CloneableEntityGrid<P = {}> extends EntityGrid<ProductRow, P> {

    protected getColumnsKey() { return ProductColumns.columnsKey; }
    protected getDialogType() { return CloneableEntityDialog; }
    protected getRowDefinition() { return ProductRow; }
    protected getService() { return ProductService.baseUrl; }
}

export class CloneableEntityDialog extends ProductDialog {
    static [Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected updateInterface() {

        // by default cloneButton is hidden in base UpdateInterface method
        super.updateInterface();

        // here we show it if it is edit mode (not new)
        this.cloneButton.toggle(this.isEditMode());
    }

    /**
        * Overriding this method is optional to customize cloned entity
        */
    protected getCloningEntity() {
        var clone = super.getCloningEntity();

        // add (Clone) suffix if it's not already added
        var suffix = ' (Clone)';
        if (!(clone.ProductName || '').endsWith(suffix)) {
            clone.ProductName = (clone.ProductName || '') + suffix;
        }

        // it's better to clear image for this sample
        // otherwise we would have to create a temporary copy of it
        // and upload
        clone.ProductImage = null;

        // let's clear fields not logical to be cloned
        clone.UnitsInStock = 0;
        clone.UnitsOnOrder = 0;
        return clone;
    }
}