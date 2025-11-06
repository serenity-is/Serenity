import { EntityDialogTexts, Fluent, confirmDialog, faIcon, gridPageInit } from "@serenity-is/corelib";
import { CustomerGrid, CustomerService, OrderDialog } from "@serenity-is/demo.northwind";
import { Column } from "@serenity-is/sleekgrid";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(InlineActionGrid);

const deleteRowAction = "delete-row";
const viewDetailsAction = "view-details";
const newOrderAction = "new-order";

export class InlineActionGrid extends CustomerGrid {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override createColumns() {
        var columns = super.createColumns();

        let inlineAction = (actionKey: string, hint, iconClass: string): Column => ({
            id: "inline_action_" + actionKey,
            name: '',
            width: 24,
            format: () => <a class="inline-action" data-action={actionKey} title={hint}><i class={iconClass}></i></a>,
            minWidth: 24,
            maxWidth: 24
        })

        columns.unshift(inlineAction(deleteRowAction, "Delete", faIcon("trash", "danger")));
        columns.splice(1, 0, inlineAction(viewDetailsAction, "View Details", faIcon("search")));
        columns.splice(2, 0, inlineAction(newOrderAction, "New Order", faIcon("cart-plus")));

        return columns;
    }

    protected override onClick(e: Event, row: number, cell: number) {
        super.onClick(e, row, cell);

        if (Fluent.isDefaultPrevented(e))
            return;

        var item = this.itemAt(row);
        var action = (e.target as HTMLElement).closest(".inline-action")?.getAttribute("data-action");
        if (action) {
            e.preventDefault();

            switch (action) {
                case deleteRowAction: {
                    confirmDialog(EntityDialogTexts.DeleteConfirmation, () => {
                        CustomerService.Delete({
                            EntityId: item.CustomerID,
                        }, _ => {
                            this.refresh();
                        });
                    });
                    break;
                }

                case viewDetailsAction: {
                    this.editItem(item.CustomerID);
                    break;
                }

                case newOrderAction: {
                    var dlg = new OrderDialog();
                    this.initDialog(dlg);
                    dlg.loadEntityAndOpenDialog({
                        CustomerID: item.CustomerID
                    });
                }
            }
        }
    }
}