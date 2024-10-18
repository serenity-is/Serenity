import { Decorators, Dictionary, EntityGrid, Lookup, LookupEditor, WidgetProps, deepClone, formatNumber, htmlEncode, localText, notifyError, parseDecimal, parseInteger, parseQueryString, serviceRequest, toId } from "@serenity-is/corelib";
import { ExcelExportHelper, PdfExportHelper } from "@serenity-is/extensions";
import { Column, FormatterContext, NonDataRow } from "@serenity-is/sleekgrid";
import { CategoryRow, ProductColumns, ProductRow, ProductService, SupplierRow } from "../ServerTypes/Demo";
import { ProductDialog } from "./ProductDialog";
import "./ProductGrid.css";

@Decorators.registerClass()
@Decorators.filterable()
export class ProductGrid<P = {}> extends EntityGrid<ProductRow, P> {
    protected getColumnsKey() { return ProductColumns.columnsKey; }
    protected getDialogType() { return <any>ProductDialog; }
    protected getRowDefinition() { return ProductRow; }
    protected getService() { return ProductService.baseUrl; }

    declare private pendingChanges: Dictionary<any>;

    constructor(props: WidgetProps<P>) {
        super(props);
        this.pendingChanges = {};
        this.slickContainer.on('change', 'input.edit, textarea.edit, select.edit', (e) => this.inputsChange(e as any));
    }

    protected getButtons() {
        var buttons = super.getButtons();

        buttons.push(ExcelExportHelper.createToolButton({
            grid: this,
            service: ProductService.baseUrl + '/ListExcel',
            onViewSubmit: () => this.onViewSubmit(),
            separator: true
        }));

        buttons.push(PdfExportHelper.createToolButton({
            grid: this,
            onViewSubmit: () => this.onViewSubmit(),
            reportTitle: 'Product List',
            columnTitles: {
                'Discontinued': 'Dis.',
            },
            tableOptions: {
                columnStyles: {
                    ProductID: {
                        cellWidth: 25,
                        halign: 'right'
                    },
                    Discountinued: {
                        cellWidth: 25
                    }
                }
            }
        }));

        buttons.push({
            title: 'Save Changes',
            cssClass: 'apply-changes-button disabled',
            onClick: e => this.saveClick(),
            separator: true
        });

        return buttons;
    }

    protected onViewProcessData(response) {
        this.pendingChanges = {};
        this.setSaveButtonState();
        return super.onViewProcessData(response);
    }

    // PLEASE NOTE! Inline editing in grids is not something Serenity supports nor recommends.
    // SlickGrid has some set of limitations, UI is very hard to use on some devices like mobile, 
    // custom widgets and validations are not possible, and as a bonus the code can become a mess.
    // 
    // This was just a sample how-to after much requests, and is not supported. 
    // This is all we can offer, please don't ask us to Guide you...

    /**
     * It would be nice if we could use autonumeric, Serenity editors etc. here, to control input validation,
     * but it's not supported by SlickGrid as we are only allowed to return a string, and should attach
     * no event handlers to rendered cell contents
     */
    private numericInputFormatter(ctx) {
        if ((ctx.item as NonDataRow).__nonDataRow)
            return ctx.escape(formatNumber(ctx.value, '#0.##'));

        var klass = 'edit numeric';
        var item = ctx.item as ProductRow;
        var pending = this.pendingChanges[item.ProductID];

        if (pending && pending[ctx.column.field] !== undefined) {
            klass += ' dirty';
        }

        var value = this.getEffectiveValue(item, ctx.column.field) as number;

        return "<input type='text' class='" + klass +
            "' data-field='" + ctx.column.field +
            "' value='" + formatNumber(value, '0.##') + "'/>";
    }

    private stringInputFormatter(ctx) {
        if ((ctx.item as NonDataRow).__nonDataRow)
            return ctx.escape();

        var klass = 'edit string';
        var item = ctx.item as ProductRow;
        var pending = this.pendingChanges[item.ProductID];
        var column = ctx.column as Column;

        if (pending && pending[column.field] !== undefined) {
            klass += ' dirty';
        }

        var value = this.getEffectiveValue(item, column.field) as string;

        return "<input type='text' class='" + klass +
            "' data-field='" + column.field +
            "' value='" + htmlEncode(value) +
            "' maxlength='" + column.sourceItem.maxLength + "'/>";
    }

    /**
     * Sorry but you cannot use LookupEditor, e.g. Select2 here, only possible is a SELECT element
     */
    private selectFormatter(ctx: FormatterContext, idField: string, lookup: Lookup<any>) {
        if ((ctx.item as NonDataRow).__nonDataRow)
            return ctx.escape();

        var klass = 'edit';
        var item = ctx.item as ProductRow;
        var pending = this.pendingChanges[item.ProductID];
        var column = ctx.column as Column;

        if (pending && pending[idField] !== undefined) {
            klass += ' dirty';
        }

        var value = this.getEffectiveValue(item, idField);
        var markup = "<select class='" + klass +
            "' data-field='" + idField +
            "' style='width: 100%; max-width: 100%'>" +
            "<option value=''>--</option>";
        for (var c of lookup.items) {
            let id = c[lookup.idField];
            markup += "<option value='" + htmlEncode(id) + "'"
            if (id == value) {
                markup += " selected";
            }
            markup += ">" + htmlEncode(c[lookup.textField]) + "</option>";
        }
        return markup + "</select>";
    }

    private getEffectiveValue(item, field): any {
        var pending = this.pendingChanges[item.ProductID];
        if (pending && pending[field] !== undefined) {
            return pending[field];
        }

        return item[field];
    }

    protected getColumns() {
        var columns = new ProductColumns(super.getColumns());
        var num = (ctx: FormatterContext<ProductRow>) => this.numericInputFormatter(ctx);
        var str = (ctx: FormatterContext<ProductRow>) => this.stringInputFormatter(ctx);

        const fld = ProductRow.Fields;
        columns.QuantityPerUnit && (columns.QuantityPerUnit.format = str);
        columns.CategoryName && (columns.CategoryName.referencedFields = [fld.CategoryID]) &&
            (columns.CategoryName.format = ctx => this.selectFormatter(ctx, fld.CategoryID, (CategoryRow as any).getLookup()));

        columns.SupplierCompanyName && (columns.SupplierCompanyName.referencedFields = [fld.SupplierID]) &&
            (columns.SupplierCompanyName.format = ctx => this.selectFormatter(ctx, fld.SupplierID, (SupplierRow as any).getLookup()));

        columns.UnitPrice && (columns.UnitPrice.format = num);
        columns.UnitsInStock && (columns.UnitsInStock.format = num);
        columns.UnitsOnOrder && (columns.UnitsOnOrder.format = num);
        columns.ReorderLevel && (columns.ReorderLevel.format = num);

        return columns.valueOf();
    }

    private inputsChange(e: Event) {
        var cell = this.slickGrid.getCellFromEvent(e);
        var item = this.itemAt(cell.row);
        var input = e.target as (HTMLInputElement | HTMLSelectElement);
        var field = input.getAttribute('data-field');
        var txt = input.value?.trim() || null;
        var pending = this.pendingChanges[item.ProductID];

        var effective = this.getEffectiveValue(item, field);
        var oldText: string;
        if (input.classList.contains("numeric"))
            oldText = formatNumber(effective, '0.##');
        else
            oldText = effective as string;

        var value: any;
        if (field === 'UnitPrice') {
            value = parseDecimal(txt ?? '');
            if (value == null || isNaN(value)) {
                notifyError(localText('Validation.Decimal'), '', null);
                input.value = oldText;
                input.focus();
                return;
            }
        }
        else if (input.classList.contains("numeric")) {
            var i = parseInteger(txt ?? '');
            if (isNaN(i) || i > 32767 || i < 0) {
                notifyError(localText('Validation.Integer'), '', null);
                input.value = oldText;
                input.focus();
                return;
            }
            value = i;
        }
        else if (input.tagName === 'SELECT')
            value = toId(input.value);
        else
            value = txt;

        if (!pending) {
            this.pendingChanges[item.ProductID] = pending = {};
        }

        pending[field] = value;
        item[field] = value;
        this.view.refresh();

        if (input.classList.contains("numeric"))
            value = formatNumber(value, '0.##');

        input.value = value;
        input.classList.add('dirty');

        this.setSaveButtonState();
    }

    private setSaveButtonState() {
        this.toolbar.findButton('apply-changes-button').toggleClass('disabled',
            Object.keys(this.pendingChanges).length === 0);
    }

    private saveClick() {
        if (Object.keys(this.pendingChanges).length === 0) {
            return;
        }

        // this calls save service for all modified rows, one by one
        // you could write a batch update service
        var keys = Object.keys(this.pendingChanges);
        var current = -1;
        var self = this;

        (function saveNext() {
            if (++current >= keys.length) {
                self.refresh();
                return;
            }

            var key = keys[current];
            var entity = deepClone(self.pendingChanges[key]);
            entity.ProductID = key;
            serviceRequest(ProductService.Methods.Update, {
                EntityId: key,
                Entity: entity
            }, (response) => {
                delete self.pendingChanges[key];
                saveNext();
            });
        })();
    }

    protected getQuickFilters() {
        var flt = super.getQuickFilters();

        var q = parseQueryString();
        if (q["cat"]) {
            var category = flt.find(x => x.field == ProductRow.Fields.CategoryID);
            category && (category.init = e => {
                e.element.getWidget(LookupEditor).value = q["cat"];
            });
        }

        return flt;
    }

}
