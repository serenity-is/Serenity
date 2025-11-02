import { bindThis } from "@serenity-is/wisdom";
import { FilterPanelTexts, Fluent, PropertyItem, localText, nsSerenity } from "../../base";
import { Combobox } from "../editors/combobox";
import { ComboboxEditor } from "../editors/comboboxeditor";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";
import { WidgetProps, getWidgetFrom } from "../widgets/widget";
import { FilteringTypeRegistry } from "./filteringtyperegistry";
import { FilterLine } from "./filterline";
import { FilterOperator } from "./filteroperator";
import { FilterWidgetBase } from "./filterwidgetbase";
import { IFiltering } from "./ifiltering";

export interface FilterFieldSelectOptions {
    fields: PropertyItem[];
}

class FilterFieldSelect<P extends FilterFieldSelectOptions = FilterFieldSelectOptions> extends ComboboxEditor<P, PropertyItem> {
    static [Symbol.typeInfo] = this.registerClass(nsSerenity);

    constructor(props: WidgetProps<P>) {
        super(props);

        for (var field of this.options.fields) {
            this.addOption(field.name, localText(field.title, field.title ?? field.name), field);
        }
    }

    emptyItemText() {
        if (!this.value) {
            return FilterPanelTexts.SelectField;
        }

        return null;
    }

    getComboboxOptions() {
        var opt = super.getComboboxOptions();
        opt.allowClear = false;
        return opt;
    }
}

class FilterOperatorSelect extends ComboboxEditor<any, FilterOperator> {
    static [Symbol.typeInfo] = this.registerClass(nsSerenity);

    constructor(props: WidgetProps<{ source: FilterOperator[] }>) {
        super(props);

        for (var op of this.options.source) {
            var title = op.title ?? (FilterPanelTexts.OperatorNames.asTry() as any)[op.key] ?? op.key;
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

export class FilterPanel<P = {}> extends FilterWidgetBase<P> {
    static [Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private rowsDiv: HTMLElement;
    declare private resetButton: HTMLButtonElement;
    declare private searchButton: HTMLButtonElement;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add("s-FilterPanel'")
        this.rowsDiv = this.findById('Rows');
        this.updateButtons();
    }

    declare private _showInitialLine: boolean;

    get showInitialLine() {
        return this._showInitialLine;
    }

    set showInitialLine(value: boolean) {
        if (this._showInitialLine !== value) {
            this._showInitialLine = value;
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

        if (this.showInitialLine && !this.rowsDiv.childElementCount) {
            this.addEmptyRow(false);
        }

        this.updateParens();
    }

    declare private _showSearchButton: boolean;

    get showSearchButton(): boolean {
        return this._showSearchButton;
    }

    set showSearchButton(value: boolean) {
        if (this._showSearchButton !== value) {
            this._showSearchButton = value;
            this.updateButtons();
        }
    }

    declare updateStoreOnReset: boolean;

    protected override renderContents(): any {
        const id = this.useIdPrefix();
        return (<>
            <div id={id.Rows} class="filter-lines" />
            <div id={id.Buttons} class="buttons">
                <button id={id.AddButton} type="button" class="btn btn-primary add" onClick={e => this.addButtonClick(e)}>
                    {FilterPanelTexts.AddFilter}
                </button>
                <button id={id.SearchButton} type="button" class="btn btn-success search" onClick={e => this.searchButtonClick(e)} ref={el => this.searchButton = el}>
                    {FilterPanelTexts.SearchButton}</button>
                <button id={id.ResetButton} type="button" class="btn btn-danger reset" onClick={e => this.resetButtonClick(e)} ref={el => this.resetButton = el}>
                    {FilterPanelTexts.ResetButton}
                </button>
            </div>
            <div style="clear: both" />
        </>);
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
                errorText = FilterPanelTexts.InvalidOperator;
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
            row.querySelector<HTMLElement>('div.v').appendChild(<span class="error" title={errorText} />)
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

        if (this.updateStoreOnReset) {
            if (this.get_store().get_items().length > 0) {
                this.get_store().get_items().length = 0;
                this.get_store().raiseChanged();
            }
        }

        Fluent(this.rowsDiv).empty();
        this.updateButtons();
        if (this.showInitialLine) {
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
        const emptyRow = this.findEmptyRow();

        if (emptyRow != null) {
            const fieldSelect = emptyRow.querySelector<HTMLInputElement>('input.field-select');
            fieldSelect?.focus();
            popupField && Combobox.getInstance(fieldSelect)?.openDropdown();
            return emptyRow;
        }

        const boundThis = bindThis(this);
        const isLastRowOr = !!this.rowsDiv.lastElementChild?.querySelector('a.andor.or');
        const fieldSelect = <input class="field-select" type="hidden" /> as HTMLInputElement;
        const row = this.rowsDiv.appendChild(
            <div class="filter-line">
                <a class="delete" title={FilterPanelTexts.RemoveField}
                    onClick={boundThis.deleteRowClick}><span></span></a>
                <div class="l" style="display: none">
                    <a class="rightparen" href="#" onClick={boundThis.leftRightParenClick}>)</a>
                    <a class={["andor", isLastRowOr && "or"]} href="#"
                        title={FilterPanelTexts.ChangeAndOr} onClick={boundThis.andOrClick}>
                        {FilterPanelTexts[(isLastRowOr ? 'Or' : 'And')]}
                    </a>
                    <a class="leftparen" href="#" onClick={boundThis.leftRightParenClick}>(</a>
                </div>
                <div class="f">
                    {fieldSelect}
                </div>
                <div class="o"></div>
                <div class="v"></div>
            </div>) as HTMLElement;
        new FilterFieldSelect({ element: fieldSelect, fields: this.get_store().get_fields() }).changeSelect2(bindThis(this).onRowFieldChange);
        this.updateButtons();
        fieldSelect.focus();
        popupField && Combobox.getInstance(fieldSelect)?.openDropdown();
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

        const hidden = opDiv.appendChild(<input type="hidden" class="op-select" /> as HTMLInputElement);

        var operators = filtering.getOperators();
        var opSelect = new FilterOperatorSelect({ element: hidden, source: operators });
        opSelect.changeSelect2(bindThis(this).onRowOperatorChange);
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
        Fluent.toggle(this.searchButton, this.rowsDiv.childElementCount >= 1 && this.showSearchButton);
        Fluent.toggle(this.resetButton, this.rowsDiv.childElementCount >= 1);
    }

    protected andOrClick(e: Event): void {
        e.preventDefault();
        var andor = e.target as HTMLElement;
        andor.classList.toggle('or');
        andor.textContent = FilterPanelTexts[(andor.classList.contains('or') ? 'Or' : 'And')];
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
            divL.hidden = ((rows.length === 1) ? true : false);
            if (index === 0)
                divL.querySelectorAll<HTMLElement>('a.rightparen, a.andor').forEach(el => el.hidden = true);
            else {
                divL.hidden = false;
                divL.querySelectorAll<HTMLElement>('a.lefparen, a.andor').forEach(el => el.hidden = false);
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