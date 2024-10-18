import { Decorators, WidgetProps } from "@serenity-is/corelib";
import { toId } from "@serenity-is/corelib";
import { GridEditorDialog } from "@serenity-is/extensions";
import { OrderDetailForm, OrderDetailRow, ProductRow } from "../ServerTypes/Demo";

import "./OrderDetailDialog.css";

@Decorators.registerClass("Serenity.Demo.Northwind.OrderDetailDialog")
export class OrderDetailDialog extends GridEditorDialog<OrderDetailRow> {
    protected getFormKey() { return OrderDetailForm.formKey; }
    protected getLocalTextPrefix() { return OrderDetailRow.localTextPrefix; }

    declare protected form: OrderDetailForm;

    constructor(props: WidgetProps<any>) {
        super(props);

        this.form = new OrderDetailForm(this.idPrefix);

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
}
