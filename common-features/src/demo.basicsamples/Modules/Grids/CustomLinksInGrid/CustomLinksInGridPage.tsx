import { Decorators, Fluent, LookupEditor, confirmDialog, count, first, formatDate, gridPageInit, htmlEncode, notifyInfo, notifySuccess, stringFormat, toId } from "@serenity-is/corelib";
import { CustomerDialog, CustomerRow, OrderColumns, OrderDialog, OrderGrid, OrderRow } from "@serenity-is/demo.northwind";
import { Column } from "@serenity-is/sleekgrid";

export default () => gridPageInit(CustomLinksInGrid);

@Decorators.registerClass('Serenity.Demo.BasicSamples.CustomLinksInGrid')
export class CustomLinksInGrid extends OrderGrid {

    /**
     * We override getColumns() to change format functions for some columns.
     * You could also write them as formatter classes, and use them at server side
     */
    protected getColumns(): Column[] {
        var columns = new OrderColumns(super.getColumns());

        columns.CustomerCompanyName && (columns.CustomerCompanyName.format =
            ctx => <a href="#" class="customer-link">{ctx.value}</a>);

        columns.OrderDate && (columns.OrderDate.format =
            ctx => <a href="#" class="date-link">{formatDate(ctx.value)}</a>);

        columns.EmployeeFullName && (columns.EmployeeFullName.format =
            ctx => <a href="#" class="employee-link">{ctx.value}</a>);

        columns.ShipCountry && (columns.ShipCountry.format =
            ctx => <a href="#" class="ship-country-link">{ctx.value}</a>);

        return columns.valueOf();
    }

    protected onClick(e: Event, row: number, cell: number): void {

        // let base grid handle clicks for its edit links
        super.onClick(e, row, cell);

        // if base grid already handled, we shouldn"t handle it again
        if (Fluent.isDefaultPrevented(e)) {
            return;
        }

        // get reference to current item
        var item = this.itemAt(row);

        // get reference to clicked element
        var target = e.target as HTMLElement;

        if (target.classList.contains("customer-link")) {
            e.preventDefault();

            let message = stringFormat(
                "<p>You have clicked an order from customer: {0}.</p>" +
                "<p>If you click Yes, i'll open Customer dialog.</p>" +
                "<p>If you click No, i'll open Order dialog.</p>",
                htmlEncode(item.CustomerCompanyName));

            confirmDialog(message, async () => {
                // CustomerDialog doesn't use CustomerID but ID (identity)
                // so need to find customer to get its actual ID
                var customer = first((await CustomerRow.getLookupAsync()).items,
                    x => x.CustomerID == item.CustomerID);

                new CustomerDialog({}).loadByIdAndOpenDialog(customer.ID);
            },
                {
                    htmlEncode: false,
                    onNo: () => {
                        new OrderDialog().loadByIdAndOpenDialog(item.OrderID);
                    }
                });
        }
        else if (target.classList.contains("date-link")) {
            e.preventDefault();

            var ordersInSameDate = count(this.view.getItems(), x => x.OrderDate == item.OrderDate);

            notifyInfo("You clicked an order from date " +
                formatDate(item.OrderDate) + ". There are " +
                ordersInSameDate + " orders from the same date that is loaded in grid at the moment");
        }
        else if (target.classList.contains("employee-link")) {
            e.preventDefault();

            notifySuccess("You clicked an employee name, " +
                "so i've opened a new Order Dialog from same customer " +
                "with that employee prepopulated!");

            new OrderDialog().loadEntityAndOpenDialog({
                CustomerID: item.CustomerID,
                EmployeeID: item.EmployeeID
            });
        }
        else if (target.classList.contains("ship-country-link")) {
            e.preventDefault();

            notifySuccess("Let's filter the grid to orders from " + item.ShipCountry);
            var countryFilter = this.findQuickFilter(LookupEditor,
                OrderRow.Fields.ShipCountry);
            countryFilter.value = item.ShipCountry;
            this.refresh();
        }
    }

    /**
     * This method is called for columns with [EditLink] attribute,
     * but only for edit links of this grid's own item type.
     * It is also called by Add Product button with a NULL entityOrId
     * parameter so we should check that entityOrId is a string
     * to be sure it is originating from a link.
     *
     * As we changed format for other columns, this will only be called
     * for links in remaining OrderID column
     */
    protected editItem(entityOrId) {
        // check that this is an edit link click, not add button, ID is always a string
        if (typeof entityOrId == "string") {
            // convert ID to an integer, and find order with that ID
            var item = this.view.getItemById(toId(entityOrId));
            // date is a ISO string, so need to parse it first
            var date = formatDate(item.OrderDate);

            // ask for confirmation
            confirmDialog(stringFormat("You clicked edit link for order with ID: {0} and Date: {1}. Should i open that order?",
                item.OrderID, date), () => {
                    new OrderDialog().loadByIdAndOpenDialog(item.OrderID);
                });
        }
        else {
            super.editItem(entityOrId);
        }
    }
}