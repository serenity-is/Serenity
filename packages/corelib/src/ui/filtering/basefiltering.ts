import { ClassTypeInfo, classTypeInfo, Criteria, FilterPanelTexts, Fluent, getInstanceType, getTypeFullName, localText, nsSerenity, registerType, stringFormat, StringLiteral, type AttributeSpecifier, type InterfaceType, type PropertyItem } from "../../base";
import { deepClone } from "../../compat";
import { QuickFilter } from "../datagrid/quickfilter";
import { Combobox } from "../editors/combobox";
import { StringEditor } from "../editors/stringeditor";
import { Widget } from "../widgets/widget";
import { CriteriaWithText } from "./criteriawithtext";
import { FilterOperator, FilterOperators } from "./filteroperator";
import { IFiltering } from "./ifiltering";
import { IQuickFiltering } from "./iquickfiltering";

export abstract class BaseFiltering implements IFiltering, IQuickFiltering {

    declare private field: PropertyItem;

    public get_field() {
        return this.field;
    }

    set_field(value: PropertyItem) {
        this.field = value;
    }

    declare private container: HTMLElement;

    get_container(): HTMLElement {
        return this.container;
    }

    set_container(value: HTMLElement) {
        this.container = value;
    }

    declare private operator: FilterOperator;

    get_operator() {
        return this.operator;
    }

    set_operator(value: FilterOperator) {
        this.operator = value;
    }

    abstract getOperators(): FilterOperator[];

    protected appendNullableOperators(list: FilterOperator[]) {
        if (!this.isNullable()) {
            return list;
        }
        list.push({ key: FilterOperators.isNotNull });
        list.push({ key: FilterOperators.isNull });
        return list;
    }

    protected appendComparisonOperators(list: FilterOperator[]) {
        list.push({ key: FilterOperators.EQ });
        list.push({ key: FilterOperators.NE });
        list.push({ key: FilterOperators.LT });
        list.push({ key: FilterOperators.LE });
        list.push({ key: FilterOperators.GT });
        list.push({ key: FilterOperators.GE });
        return list;
    }

    protected isNullable() {
        return this.get_field().required !== true;
    }

    public createEditor() {
        switch (this.get_operator().key) {
            case 'true':
            case 'false':
            case 'isnull':
            case 'isnotnull': {
                return;
            }
            case 'contains':
            case 'startswith':
            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge': {
                const input = document.createElement("input");
                input.type = "text";
                Fluent(this.get_container()).empty().append(input);
                return;
            }
        }

        throw new Error(stringFormat("Filtering '{0}' has no editor for '{1}' operator",
            getTypeFullName(getInstanceType(this)), this.get_operator().key));
    }

    protected operatorFormat(op: FilterOperator): string {
        return op.format ?? (FilterPanelTexts.asTry().OperatorFormats as any)[op.key] ?? op.key;
    }

    protected getTitle(field: PropertyItem): string {
        return localText(field.title, field.title ?? field.name);
    }

    protected displayText(op: FilterOperator, values?: any[]): string {
        if (!values || values.length === 0) {
            return stringFormat(this.operatorFormat(op), this.getTitle(this.field));
        }
        else if (values.length === 1) {
            return stringFormat(this.operatorFormat(op), this.getTitle(this.field), values[0]);
        }
        else {
            return stringFormat(this.operatorFormat(op), this.getTitle(this.field), values[0], values[1]);
        }
    }

    protected getCriteriaField(): string {
        return this.field.name;
    }

    public getCriteria(): CriteriaWithText {
        var result: CriteriaWithText = {};
        var text: string;
        var field = Criteria(this.getCriteriaField());
        var op = this.get_operator().key;
        switch (op) {
            case 'true': {
                result.displayText = this.displayText(this.get_operator(), []);
                result.criteria = field.eq(true);
                return result;
            }

            case 'false': {
                result.displayText = this.displayText(this.get_operator(), []);
                result.criteria = field.eq(false);
                return result;
            }

            case 'isnull': {
                result.displayText = this.displayText(this.get_operator(), []);
                result.criteria = field.isNull();
                return result;
            }

            case 'isnotnull': {
                result.displayText = this.displayText(this.get_operator(), []);
                result.criteria = field.isNotNull();
                return result;
            }

            case 'contains': {
                text = this.getEditorText();
                result.displayText = this.displayText(this.get_operator(), [text]);
                result.criteria = field.contains(text);
                return result;
            }

            case 'startswith': {
                text = this.getEditorText();
                result.displayText = this.displayText(this.get_operator(), [text]);
                result.criteria = field.startsWith(text);
                return result;
            }

            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge': {
                text = this.getEditorText();
                result.displayText = this.displayText(this.get_operator(), [text]);
                result.criteria = field[op](this.getEditorValue());
                return result;
            }
        }

        throw new Error(stringFormat("Filtering '{0}' has no handler for '{1}' operator",
            getTypeFullName(getInstanceType(this)), this.get_operator().key));
    }

    loadState(state: any) {
        var input = this.get_container().querySelector<HTMLInputElement>(Fluent.inputLikeSelector);
        input && (input.value = state);
    }

    saveState() {
        switch (this.get_operator().key) {
            case 'contains':
            case 'startswith':
            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge': {
                var input = this.get_container().querySelector<HTMLInputElement>(Fluent.inputLikeSelector);
                return input?.value;
            }
        }
        return null;
    }

    protected argumentNull() {
        return new Error(FilterPanelTexts.ValueRequired);
    }

    validateEditorValue(value: string) {
        if (value.length === 0) {
            throw this.argumentNull();
        }
        return value;
    }

    getEditorValue() {
        var inputs = this.get_container().querySelectorAll<HTMLInputElement>(Fluent.inputLikeSelector + ":not(.select2-focusser)");
        if (inputs.length !== 1) {
            throw new Error(stringFormat("Couldn't find input in filter container for {0}",
                (this.field.title ?? this.field.name)));
        }
        let input = inputs[0];

        var value;
        let combobox = Combobox.getInstance(input);
        if (combobox) {
            value = combobox.isMultiple ? combobox.getValues().join(",") : combobox.getValue();
        }
        else {
            value = input.value;
        }

        value = (value ?? '').trim();

        return this.validateEditorValue(value);
    }

    getEditorText(): string {

        var input = this.get_container().querySelector<HTMLInputElement>(Fluent.inputLikeSelector + ":not(.select2-focusser):not(.select2-input)");
        if (!input) {
            return this.get_container().textContent?.trim();
        }
        var value;
        let combobox = Combobox.getInstance(input);
        if (combobox) {
            value = combobox.getSelectedItems()?.join(", ");
        }
        else {
            value = input.value;
        }
        return value;
    }

    initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        filter.field = this.getCriteriaField();
        filter.type = StringEditor;
        filter.title = this.getTitle(this.field);
        filter.options = deepClone(this.get_field().quickFilterParams);
    }

    protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): ClassTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, Symbol.typeInfo) && this[Symbol.typeInfo])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);

        const typeInfo = this[Symbol.typeInfo] = classTypeInfo(typeName, intfAndAttr);
        registerType(this);
        return typeInfo;
    }

    static [Symbol.typeInfo] = this.registerClass(nsSerenity, [IFiltering, IQuickFiltering]);
}

