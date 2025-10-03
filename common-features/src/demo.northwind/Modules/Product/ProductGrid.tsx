import { Dictionary, EditorUtils, EntityGrid, FilterableAttribute, FormValidationTexts, Lookup, WidgetProps, deepClone, formatNumber, notifyError, parseDecimal, parseInteger, parseQueryString, serviceRequest, toId } from "@serenity-is/corelib";
import { ExcelExportHelper, PdfExportHelper } from "@serenity-is/extensions";
import { Column, FormatterContext, FormatterResult, NonDataRow } from "@serenity-is/sleekgrid";
import { CategoryRow, ProductColumns, ProductRow, ProductService, SupplierRow } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { ProductDialog } from "./ProductDialog";
import "./ProductGrid.css";

export class ProductGrid<P = {}> extends EntityGrid<ProductRow, P> {
    static [Symbol.typeInfo] = this.registerClass(nsDemoNorthwind, [new FilterableAttribute()]);
    protected getColumnsKey() { return ProductColumns.columnsKey; }
    protected getDialogType() { return ProductDialog as any; }
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

    private numericInputFormatter(ctx: FormatterContext): FormatterResult {
        if ((ctx.item as NonDataRow).__nonDataRow)
            return ctx.escape(formatNumber(ctx.value, '#0.##'));

        let klass = 'edit numeric';
        const pending = this.pendingChanges[this.itemId(ctx.item)];
        pending && pending[ctx.column.field] !== undefined && (klass += ' dirty');
        const value = this.getEffectiveValue(ctx.item, ctx.column.field) as number;
        return <input type="text" className={klass} data-field={ctx.column.field} value={formatNumber(value, '0.##')} />;
    }

    private stringInputFormatter(ctx: FormatterContext<ProductRow>): FormatterResult {
        if ((ctx.item as NonDataRow).__nonDataRow)
            return ctx.escape();

        let klass = 'edit string';
        const pending = this.pendingChanges[this.itemId(ctx.item)];
        pending && pending[ctx.column.field] !== undefined && (klass += ' dirty');
        const value = this.getEffectiveValue(ctx.item, ctx.column.field) as string;

        return <input type='text' className={klass}
            data-field={ctx.column.field}
            value={value}
            maxLength={ctx.column.sourceItem.maxLength} />;
    }

    private selectFormatter(ctx: FormatterContext, idField: string, lookup: Lookup<any>) {
        if ((ctx.item as NonDataRow).__nonDataRow)
            return ctx.escape();

        let klass = 'edit';
        const pending = this.pendingChanges[this.itemId(ctx.item)];
        pending && pending[idField] !== undefined && (klass += ' dirty');

        const value = this.getEffectiveValue(ctx.item, idField) as string;
        let markup = <select className={klass} data-field={idField} style={{ width: '100%', maxWidth: '100%' }}>
            <option value=''>--</option>;
            {lookup.items.map(c => <option value={c[lookup.idField]} selected={c[lookup.idField] == value}>
                {c[lookup.textField]}
            </option>)}
        </select>;
        return markup;
    }

    private getEffectiveValue(item: ProductRow, field: string): any {
        const pending = this.pendingChanges[item.ProductID];
        if (pending && pending[field] !== undefined) {
            return pending[field];
        }
        return item[field];
    }

    protected getColumns(): Column[] {
        const columns = new ProductColumns(super.getColumns());
        const num = (ctx: FormatterContext<ProductRow>) => this.numericInputFormatter(ctx);
        const str = (ctx: FormatterContext<ProductRow>) => this.stringInputFormatter(ctx);

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
        const cell = this.slickGrid.getCellFromEvent(e);
        const item = this.itemAt(cell.row);
        const input = e.target as (HTMLInputElement | HTMLSelectElement);
        const field = input.getAttribute('data-field');
        const txt = input.value?.trim() || null;
        let pending = this.pendingChanges[item.ProductID];

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
                notifyError(FormValidationTexts.Decimal, '', null);
                input.value = oldText;
                input.focus();
                return;
            }
        }
        else if (input.classList.contains("numeric")) {
            var i = parseInteger(txt ?? '');
            if (isNaN(i) || i > 32767 || i < 0) {
                notifyError(FormValidationTexts.Integer, '', null);
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
        const keys = Object.keys(this.pendingChanges);
        let current = -1;
        let self = this;

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
            }, () => {
                delete self.pendingChanges[key];
                saveNext();
            });
        })();
    }

    protected getQuickFilters() {
        const flt = super.getQuickFilters();

        const q = parseQueryString();
        if (q["cat"]) {
            const category = flt.find(x => x.field == ProductRow.Fields.CategoryID);
            category && (category.init = e => {
                EditorUtils.setValue(e.element.tryGetWidget(), q["cat"]);
            });
        }

        return flt;
    }
}
