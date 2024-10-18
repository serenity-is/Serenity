import { ChangingLookupTextForm } from "../../ServerTypes/Demo";
import { ComboboxEditor, Decorators, Lookup, LookupEditorBase, LookupEditorOptions, WidgetProps, formatNumber, toId, tryGetWidget } from "@serenity-is/corelib";
import { OrderDetailRow, ProductRow } from "@serenity-is/demo.northwind";
import { GridEditorDialog } from "@serenity-is/extensions";

export default function pageInit() {
    var dlg = new ChangingLookupTextDialog({});
    dlg.loadNewAndOpenDialog();
    tryGetWidget(dlg.domNode.querySelector(".field.ProductID .editor"), ComboboxEditor)?.openDropdown();
}

/**
 * Our custom product editor type
 */
@Decorators.registerEditor('Serenity.Demo.BasicSamples.ChangingLookupTextEditor')
export class ChangingLookupTextEditor extends LookupEditorBase<LookupEditorOptions, ProductRow> {

    protected getLookupKey() {
        return ProductRow.lookupKey;
    }

    protected getItemText(item: ProductRow, lookup: Lookup<ProductRow>) {
        return super.getItemText(item, lookup) +
            ' (' +
            '$' + formatNumber(item.UnitPrice, '#,##0.00') +
            ', ' + (item.UnitsInStock > 0 ? (item.UnitsInStock + ' in stock') : 'out of stock') +
            ', ' + (item.SupplierCompanyName || 'Unknown') +
            ')';
    }
}

@Decorators.registerClass('Serenity.Demo.BasicSamples.ChangingLookupTextDialog')
export class ChangingLookupTextDialog<P={}> extends GridEditorDialog<OrderDetailRow, P> {
    protected getFormKey() { return ChangingLookupTextForm.formKey; }
    protected getLocalTextPrefix() { return OrderDetailRow.localTextPrefix; }

    declare protected form: ChangingLookupTextForm;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.form = new ChangingLookupTextForm(this.idPrefix);

        this.form.ProductID.changeSelect2(async e => {
            var productID = toId(this.form.ProductID.value);
            if (productID != null) {
                this.form.UnitPrice.value = (await ProductRow.getLookupAsync()).itemById[productID].UnitPrice;
            }
        });

        this.form.Discount.addValidationRule(this.uniqueName, e => {
            var price = this.form.UnitPrice.value;
            var quantity = this.form.Quantity.value;
            var discount = this.form.Discount.value;
            if (price != null && quantity != null && discount != null &&
                discount > 0 && discount >= price * quantity) {
                return "Discount can't be higher than total price!";
            }
        });
    }

    protected getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.modal = false;
        return opt;
    }

    protected updateInterface() {
        super.updateInterface();
        this.toolbar.findButton('apply-changes-button').hide();
        this.toolbar.findButton('save-and-close-button').hide();
    }
}