import { Fluent, PropertyItem, localText, tryGetText } from "../../base";
import { Decorators } from "../../types/decorators";
import { Combobox } from "../editors/combobox";
import { ComboboxEditor } from "../editors/comboboxeditor";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";
import { WidgetProps, getWidgetFrom } from "../widgets/widget";
import { FilteringTypeRegistry, IFiltering } from "./filtering";
import { FilterLine } from "./filterline";
import { FilterOperator } from "./filteroperator";
import { FilterWidgetBase } from "./filterwidgetbase";

export interface FilterFieldSelectOptions {
    fields: PropertyItem[];
}

@Decorators.registerClass('Serenity.FilterFieldSelect')
class FilterFieldSelect<P extends FilterFieldSelectOptions = FilterFieldSelectOptions> extends ComboboxEditor<P, PropertyItem> {
    constructor(props: WidgetProps<P>) {
        super(props);

        for (var field of this.options.fields) {
            this.addOption(field.name, (tryGetText(field.title) ??
                field.title ?? field.name), field);
        }
    }

    emptyItemText() {
        if (!this.value) {
            return localText('Controls.FilterPanel.SelectField');
        }

        return null;
    }

    getComboboxOptions() {
        var opt = super.getComboboxOptions();
        opt.allowClear = false;
        return opt;
    }
}

@Decorators.registerClass('Serenity.FilterOperatorSelect')
class FilterOperatorSelect extends ComboboxEditor<any, FilterOperator> {
    constructor(props: WidgetProps<{ source: FilterOperator[] }>) {
        super(props);

        for (var op of this.options.source) {
            var title = (op.title ?? (
                tryGetText("Controls.FilterPanel.OperatorNames." + op.key) ?? op.key));
            this.addOption(op.key, title, op);
        }

        if (this.options.source.length && this.options.source[0])
            this.value = this.options.source[0].key;
    }

    emptyItemText(): string {
        return null;
    }

    getComboboxOptions() {
        var opt = super.getComboboxOptions();
        opt.allowClear = false;
        return opt;
    }
}

@Decorators.registerClass("Serenity.FilterPanel")
export class FilterPanel<P = {}> extends FilterWidgetBase<P> {

    private rowsDiv: HTMLElement;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add("s-FilterPanel'")
        this.rowsDiv = this.findById('Rows');
        this.initButtons();
        this.updateButtons();
    }

    private showInitialLine: boolean;

    get_showInitialLine() {
        return this.showInitialLine;
    }

    set_showInitialLine(value: boolean) {
        if (this.showInitialLine !== value) {
            this.showInitialLine = value;
            if (this.showInitialLine && !this.rowsDiv.lastElementChild) {
                this.addEmptyRow(false);
            }
        }
    }

    protected filterStoreChanged() {
        super.filterStoreChanged();
        this.updateRowsFromStore();
    }

    updateRowsFromStore() {
        Fluent(this.rowsDiv).empty();

        var items = this.get_store().get_items();
        for (var item of items) {
            this.addEmptyRow(false);

            var rowDiv = this.rowsDiv.lastElementChild as HTMLElement;

            var divl = rowDiv.querySelector("div.l");
            divl.querySelector('.leftparen').classList.toggle('active', !!item.leftParen);
            divl.querySelector('.rightparen').classList.toggle('active', !!item.rightParen);
            var andor = divl.querySelector('.andor');
            andor.classList.toggle('or', !!item.isOr);
            andor.textContent = localText((!!item.isOr ? 'Controls.FilterPanel.Or' :
                'Controls.FilterPanel.And'));

            var fieldSelect = getWidgetFrom(rowDiv.querySelector('div.f input.field-select'), FilterFieldSelect);

            fieldSelect.value = item.field;
            this.rowFieldChange(rowDiv);
            var operatorSelect = getWidgetFrom(rowDiv.querySelector(':scope div.o > input.op-select'), FilterOperatorSelect);

            operatorSelect.set_value(item.operator);
            this.rowOperatorChange(rowDiv);

            var filtering = this.getFilteringFor(rowDiv);
            if (filtering != null) {
                filtering.set_operator({ key: item.operator });
                filtering.loadState(item.state);
            }
        }

        if (this.get_showInitialLine() && !this.rowsDiv.childElementCount) {
            this.addEmptyRow(false);
        }

        this.updateParens();
    }

    private showSearchButton: boolean;

    get_showSearchButton(): boolean {
        return this.showSearchButton;
    }

    set_showSearchButton(value: boolean): void {
        if (this.showSearchButton !== value) {
            this.showSearchButton = value;
            this.updateButtons();
        }
    }

    private updateStoreOnReset: boolean;

    get_updateStoreOnReset() {
        return this.updateStoreOnReset;
    }

    set_updateStoreOnReset(value: boolean): void {
        if (this.updateStoreOnReset !== value) {
            this.updateStoreOnReset = value;
        }
    }

    protected getTemplate(): string {
        return "<div id='~_Rows' class='filter-lines'>" +
            "</div>" +
            "<div id='~_Buttons' class='buttons'>" +
            "<button id='~_AddButton' class='btn btn-primary add'></button>" +
            "<button id='~_SearchButton' class='btn btn-success search'></button>" +
            "<button id='~_ResetButton' class='btn btn-danger reset'></button>" +
            "</div>" +
            "<div style='clear: both'>" +
            "</div>"
    }

    protected initButtons(): void {
        this.byId('AddButton').text(localText('Controls.FilterPanel.AddFilter'))
            .on("click", (e) => this.addButtonClick(e as any));

        this.byId('SearchButton').text(localText('Controls.FilterPanel.SearchButton'))
            .on("click", (e) => this.searchButtonClick(e as any));

        this.byId('ResetButton').text(localText('Controls.FilterPanel.ResetButton'))
            .on("click", (e) => this.resetButtonClick(e as any));
    }

    protected searchButtonClick(e: Event) {
        e.preventDefault();
        this.search();
    }

    get_hasErrors(): boolean {
        return !!this.rowsDiv.querySelector(":scope > div.v > span.error");
    }

    search() {
        this.rowsDiv.querySelectorAll(":scope > div.v > span.error").forEach(x => x.remove());

        var filterLines = [];
        var errorText = null;
        for (var i = 0; i < this.rowsDiv.children.length; i++) {
            var row = this.rowsDiv.children[i] as HTMLElement;
            var filtering = this.getFilteringFor(row);
            if (filtering == null) {
                continue;
            }

            var field = this.getFieldFor(row);
            var op = getWidgetFrom(row.querySelector('div.o input.op-select'), FilterOperatorSelect).value;

            if (op == null || op.length === 0) {
                errorText = localText('Controls.FilterPanel.InvalidOperator');
                break;
            }

            var line: FilterLine = {};
            line.field = field.name;
            line.operator = op;
            var divL = row.querySelector("div.l");
            line.isOr = !!divL.querySelector('a.andor.or');
            line.leftParen = !!row.querySelector('div.l a.leftparen.active');
            line.rightParen = !!row.querySelector('div.l a.rightparen.active');
            filtering.set_operator({ key: op });
            var criteria;
            try {
                criteria = filtering.getCriteria();
            }
            catch (ex) {
                errorText = ex.message ?? ex.toString();
                break;
            }
            line.criteria = criteria.criteria;
            line.state = filtering.saveState();
            line.displayText = criteria.displayText;
            filterLines.push(line);
        }

        // if an error occurred, display it, otherwise set current filters
        if (errorText != null) {
            Fluent("span").class('error')
                .attr('title', errorText).appendTo(row.querySelector<HTMLElement>('div.v'));
            row.querySelector<HTMLElement>('div.v input')?.focus();
            return;
        }

        var items = this.get_store().get_items();
        items.length = 0;
        items.push.apply(items, filterLines);
        this.get_store().raiseChanged();
    }

    protected addButtonClick(e: Event) {
        this.addEmptyRow(true);
        e.preventDefault();
    }

    protected resetButtonClick(e: Event) {
        e.preventDefault();

        if (this.get_updateStoreOnReset()) {
            if (this.get_store().get_items().length > 0) {
                this.get_store().get_items().length = 0;
                this.get_store().raiseChanged();
            }
        }

        Fluent(this.rowsDiv).empty();
        this.updateButtons();
        if (this.get_showInitialLine()) {
            this.addEmptyRow(false);
        }
    }

    protected findEmptyRow(): HTMLElement {
        var result: HTMLElement = null;

        Array.from(this.rowsDiv.children).forEach(function (row: HTMLElement) {
            var fieldInput = row.querySelector<HTMLInputElement>('div.f input.field-select');
            if (!fieldInput)
                return;
            var val = fieldInput.value;
            if (!val) {
                result = row;
                return false;
            }
        });

        return result;
    }

    protected addEmptyRow(popupField: boolean): HTMLElement {
        var emptyRow = this.findEmptyRow();

        if (emptyRow != null) {
            emptyRow.querySelector<HTMLInputElement>('input.field-select')?.focus();
            if (popupField) {
                Combobox.getInstance(emptyRow?.querySelector("input.field-select"))?.openDropdown();
            }
            return emptyRow;
        }

        var isLastRowOr = !!this.rowsDiv.lastElementChild?.querySelector('a.andor.or');

        var row = this.rowsDiv.appendChild(document.createElement("div"));
        row.classList.add("filter-line");
        row.innerHTML = "<a class='delete'><span></span></a>" +
            "<div class='l'>" +
            "<a class='rightparen' href='#'>)</a>" +
            "<a class='andor' href='#'></a>" +
            "<a class='leftparen' href='#'>(</a>" +
            "</div>" +
            "<div class='f'>" +
            "<input type='hidden' class='field-select'>" +
            "</div>" +
            "<div class='o'></div>" +
            "<div class='v'></div>" +
            "<div style='clear: both'></div>";

        var parenDiv = row.querySelector<HTMLElement>('div.l');
        parenDiv.style.display = "none";

        parenDiv.querySelectorAll('a.leftparen, a.rightparen').forEach(el =>
            Fluent.on(el, "click", this.leftRightParenClick.bind(this)));

        var andor = parenDiv.querySelector<HTMLElement>('a.andor');
        andor.setAttribute('title', localText('Controls.FilterPanel.ChangeAndOr'));
        if (isLastRowOr) {
            andor.classList.add('or');
            andor.textContent = localText('Controls.FilterPanel.Or');
        }
        else {
            andor.textContent = localText('Controls.FilterPanel.And');
        }

        Fluent.on(andor, "click", this.andOrClick.bind(this));
        var del = row.querySelector('a.delete');
        del.setAttribute('title', localText('Controls.FilterPanel.RemoveField'));
        Fluent.on(del, "click", this.deleteRowClick.bind(this));

        new FilterFieldSelect({
            fields: this.get_store().get_fields(),
            element: row.querySelector<HTMLInputElement>('div.f input')
        }).changeSelect2(e => this.onRowFieldChange(e));

        this.updateParens();
        this.updateButtons();

        row.querySelector<HTMLInputElement>('input.field-select')?.focus();

        if (popupField) {
            Combobox.getInstance(row.querySelector("input.field-select"))?.openDropdown();
        }

        return row;
    }

    protected onRowFieldChange(e: Event) {
        var row = (e.target as HTMLElement).closest('div.filter-line');
        this.rowFieldChange(row as any);
        row.querySelector<HTMLInputElement>('div.o input.op-select')?.focus();
    }

    protected rowFieldChange(row: HTMLElement) {
        delete (row as any).__Filtering;
        this.removeFiltering(row);
        this.populateOperatorList(row);
        this.rowOperatorChange(row);
        this.updateParens();
        this.updateButtons();
    }

    protected removeFiltering(row: HTMLElement): void {
        delete (row as any).__Filtering;
        delete (row as any).__FilteringField;
    }

    protected populateOperatorList(row: HTMLElement): void {
        var opDiv = row.querySelector<HTMLElement>('div.o');
        Fluent(opDiv).empty();

        var filtering = this.getFilteringFor(row);
        if (filtering == null)
            return;

        var hidden = Fluent("input").attr("type", 'hidden').class('op-select').appendTo(opDiv);

        var operators = filtering.getOperators();
        var opSelect = new FilterOperatorSelect({ element: hidden, source: operators });
        opSelect.changeSelect2(this.onRowOperatorChange.bind(this));
    }

    protected getFieldFor(row: HTMLElement) {
        if (!row) {
            return null;
        }
        var select = getWidgetFrom(row.querySelector('div.f input.field-select'), FilterFieldSelect);

        if (!select || !select.value) {
            return null;
        }

        return this.get_store().get_fieldByName()[select.get_value()];
    }

    protected getFilteringFor(row: HTMLElement): IFiltering {
        var field = this.getFieldFor(row);

        if (field == null)
            return null;

        var filtering = (row as any).__Filtering as IFiltering;

        if (filtering != null)
            return filtering;

        var filteringType = FilteringTypeRegistry.get(
            (field.filteringType ?? 'String'));

        var editorDiv = row.querySelector<HTMLElement>('div.v');
        filtering = new (filteringType as any)(field.filteringParams ?? {}) as IFiltering;
        ReflectionOptionsSetter.set(filtering, field.filteringParams);
        filtering.set_container(editorDiv);
        filtering.set_field(field);
        (row as any).__Filtering = filtering;
        return filtering;
    }

    protected onRowOperatorChange(e: Event) {
        var row = (e.target as HTMLElement).closest('div.filter-line');
        this.rowOperatorChange(row as any);
        row.querySelectorAll<HTMLElement>('div.v input, div.v textarea, div.v select').forEach(el => {
            if (Fluent.isVisibleLike(el)) {
                try { el.focus(); } catch { }
            }
            return false;
        });
    }

    protected rowOperatorChange(row: HTMLElement): void {

        if (!row) {
            return;
        }

        var editorDiv = row.querySelector<HTMLElement>('div.v');
        Fluent(editorDiv).empty();
        var filtering = this.getFilteringFor(row);
        if (filtering == null)
            return;

        var operatorSelect = getWidgetFrom(row.querySelector('div.o input.op-select'), FilterOperatorSelect);

        if (!operatorSelect.get_value())
            return;

        var ops = filtering.getOperators().filter(function (x) {
            return x.key === operatorSelect.value;
        });

        var op = ((ops.length > 0) ? ops[0] : null);
        if (op == null)
            return;

        filtering.set_operator(op);
        filtering.createEditor();
    }

    protected deleteRowClick(e: Event): void {
        e.preventDefault();
        var row = (e.target as HTMLElement).closest('div.filter-line');
        row.remove();

        if (!this.rowsDiv.childElementCount) {
            this.search();
        }

        this.updateParens();
        this.updateButtons();
    }

    protected updateButtons(): void {
        this.byId('SearchButton').toggle(
            this.rowsDiv.childElementCount >= 1 && this.showSearchButton);
        this.byId('ResetButton').toggle(
            this.rowsDiv.childElementCount >= 1);
    }

    protected andOrClick(e: Event): void {
        e.preventDefault();
        var andor = e.target as HTMLElement;
        andor.classList.toggle('or');
        andor.textContent = localText('Controls.FilterPanel.' +
            andor.classList.contains('or') ? 'Or' : 'And');
    }

    protected leftRightParenClick(e: Event): void {
        e.preventDefault();
        (e.target as HTMLElement).classList.toggle('active');
        this.updateParens();
    }

    protected updateParens() {
        var rows = Array.from(this.rowsDiv.children)
        var inParen = false;
        rows.forEach((row: HTMLElement, index) => {

            row.classList.remove('paren-start');
            row.classList.remove('paren-end');
            var divL = row.querySelector<HTMLElement>('div.l');
            if (!divL)
                return;
            divL.style.display = ((rows.length === 1) ? 'none' : 'block');
            if (index === 0)
                divL.querySelectorAll<HTMLElement>('a.rightparen, a.andor').forEach(el => el.style.visibility = "hidden");
            else {
                divL.style.display = "block";
                divL.querySelectorAll<HTMLElement>('a.lefparen, a.andor').forEach(el => el.style.visibility = 'visible');
            }

            var lp = divL.querySelector('a.leftparen');
            var rp = divL.querySelector('a.rightparen');
            if (rp.classList.contains('active') && inParen) {
                inParen = false;
                if (index > 0) {
                    rows[index - 1].classList.add('paren-end');
                }
            }
            if (lp.classList.contains('active')) {
                inParen = true;
                if (index > 0) {
                    rows[index - 1].classList.add('paren-start');
                }
            }
        });
    }
}