import { Config, Criteria, Fluent, formatDate, getInstanceType, getTypeFullName, isAssignableFrom, localText, parseISODateTime, stringFormat, tryGetText, type PropertyItem } from "../../base";
import { ArgumentNullException, Exception, deepClone, extend, getTypes } from "../../q";
import { Decorators } from "../../types/decorators";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { QuickFilter } from "../datagrid/quickfilter";
import { Combobox } from "../editors/combobox";
import { DateEditor } from "../editors/dateeditor";
import { DateTimeEditor } from "../editors/datetimeeditor";
import { DecimalEditor } from "../editors/decimaleditor";
import { EditorUtils } from "../editors/editorutils";
import { EnumEditor } from "../editors/enumeditor";
import { IntegerEditor } from "../editors/integereditor";
import { LookupEditor } from "../editors/lookupeditor";
import { ServiceLookupEditor } from "../editors/servicelookupeditor";
import { StringEditor } from "../editors/stringeditor";
import { Widget } from "../widgets/widget";
import { FilterOperator, FilterOperators } from "./filteroperator";

export interface IFiltering {
    createEditor(): void;
    getCriteria(): CriteriaWithText;
    getOperators(): FilterOperator[];
    loadState(state: any): void;
    saveState(): any;
    get_field(): PropertyItem;
    set_field(value: PropertyItem): void;
    get_container(): HTMLElement;
    set_container(value: HTMLElement): void;
    get_operator(): FilterOperator;
    set_operator(value: FilterOperator): void;
}

@Decorators.registerInterface('Serenity.IFiltering')
export class IFiltering {
}

export interface CriteriaWithText {
    criteria?: any[];
    displayText?: string;
}

export interface IQuickFiltering {
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}

@Decorators.registerInterface('Serenity.IQuickFiltering')
export class IQuickFiltering {
}

@Decorators.registerClass('Serenity.BaseFiltering', [IFiltering, IQuickFiltering])
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

        throw new Exception(stringFormat("Filtering '{0}' has no editor for '{1}' operator",
            getTypeFullName(getInstanceType(this)), this.get_operator().key));
    }

    protected operatorFormat(op: FilterOperator) {
        return op.format ?? tryGetText('Controls.FilterPanel.OperatorFormats.' + op.key) ?? op.key;
    }

    protected getTitle(field: PropertyItem) {
        return tryGetText(field.title) ?? (field.title ?? field.name);
    }

    protected displayText(op: FilterOperator, values?: any[]) {
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

    protected getCriteriaField() {
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

        throw new Exception(stringFormat("Filtering '{0}' has no handler for '{1}' operator",
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
        return new Error(localText('Controls.FilterPanel.ValueRequired'));
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
            throw new Exception(stringFormat("Couldn't find input in filter container for {0}",
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
}

function Filtering(name: string) {
    return Decorators.registerClass('Serenity.' + name + 'Filtering')
}

@Filtering('BaseEditor')
export abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
    constructor(public editorTypeRef: any) {
        super();
    }

    protected useEditor() {
        switch (this.get_operator().key) {
            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge':
                return true;
        }
        return false;
    }

    declare protected editor: TEditor;

    createEditor() {
        if (this.useEditor()) {
            this.editor = new (this.editorTypeRef as typeof Widget<{}>)({
                element: el => {
                    this.get_container().append(el);
                },
                ...this.getEditorOptions()
            }).init() as TEditor;
            return;
        }
        this.editor = null;
        super.createEditor();
    }

    protected useIdField() {
        return false;
    }

    getCriteriaField() {
        if (this.useEditor() &&
            this.useIdField() &&
            this.get_field().filteringIdField) {
            return this.get_field().filteringIdField;
        }

        return super.getCriteriaField();
    }

    getEditorOptions() {
        var opt = deepClone(this.get_field().editorParams || {});
        delete opt['cascadeFrom'];
        // currently can't support cascadeFrom in filtering
        return extend(opt, this.get_field().filteringParams);
    }

    loadState(state: any) {
        if (this.useEditor()) {
            if (state == null) {
                return;
            }

            EditorUtils.setValue(this.editor, state);
            return;
        }

        super.loadState(state);
    }

    saveState() {
        if (this.useEditor()) {
            return EditorUtils.getValue(this.editor);
        }

        return super.saveState();
    }

    getEditorValue() {
        if (this.useEditor()) {
            var value = EditorUtils.getValue(this.editor);

            if (value == null || (typeof value == "string" && value.trim().length === 0))
                throw this.argumentNull();

            return value;
        }

        return super.getEditorValue();
    }

    initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        super.initQuickFilter(filter);

        filter.type = this.editorTypeRef;
        filter.options = extend(extend({}, deepClone(this.getEditorOptions())), deepClone(this.get_field().quickFilterParams));
    }
}

@Filtering('Date')
export class DateFiltering extends BaseEditorFiltering<DateEditor> {

    constructor() {
        super(DateEditor)
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}

@Filtering('Boolean')
export class BooleanFiltering extends BaseFiltering {
    getOperators() {
        return this.appendNullableOperators([
            { key: FilterOperators.isTrue },
            { key: FilterOperators.isFalse }
        ]);
    }
}

@Filtering('DateTime')
export class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {

    constructor() {
        super(DateTimeEditor)
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(
            this.appendComparisonOperators([]));
    }

    getCriteria() {
        var result: CriteriaWithText = {};

        switch (this.get_operator().key) {
            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge': {
                {
                    var text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    var date = parseISODateTime(this.getEditorValue());
                    date = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                    var next = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);
                    var criteria = Criteria(this.getCriteriaField());
                    var dateValue = formatDate(date, 'yyyy-MM-dd');
                    var nextValue = formatDate(next, 'yyyy-MM-dd');
                    switch (this.get_operator().key) {
                        case 'eq': {
                            result.criteria = Criteria.and(criteria.ge(dateValue), criteria.lt(nextValue));
                            return result;
                        }
                        case 'ne': {
                            result.criteria = Criteria.paren(Criteria.or(criteria.lt(dateValue), criteria.gt(nextValue)));
                            return result;
                        }
                        case 'lt': {
                            result.criteria = criteria.lt(dateValue);
                            return result;
                        }
                        case 'le': {
                            result.criteria = criteria.lt(nextValue);
                        }
                        case 'gt': {
                            result.criteria = criteria.ge(nextValue);
                            return result;
                        }
                        case 'ge': {
                            result.criteria = criteria.ge(dateValue);
                            return result;
                        }
                    }
                }
                break;
            }
        }

        return super.getCriteria();
    }
}

@Filtering('Decimal')
export class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    constructor() {
        super(DecimalEditor);
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(
            this.appendComparisonOperators([]));
    }
}

@Filtering('Editor')
export class EditorFiltering extends BaseEditorFiltering<Widget<any>> {

    constructor(public readonly props: { editorType?: string, useRelative?: boolean, useLike?: boolean } = {}) {
        super(Widget);
        this.props ??= {};
    }

    get editorType() { return this.props.editorType }
    set editorType(value) { this.props.editorType = value }

    get useRelative() { return this.props.useRelative }
    set useRelative(value) { this.props.useRelative = value }

    get useLike() { return this.props.useLike }
    set useLike(value) { this.props.useLike = value }

    getOperators(): FilterOperator[] {
        var list = [];

        list.push({ key: FilterOperators.EQ });
        list.push({ key: FilterOperators.NE });

        if (this.useRelative) {
            list.push({ key: FilterOperators.LT });
            list.push({ key: FilterOperators.LE });
            list.push({ key: FilterOperators.GT });
            list.push({ key: FilterOperators.GE });
        }

        if (this.useLike) {
            list.push({ key: FilterOperators.contains });
            list.push({ key: FilterOperators.startsWith });
        }

        this.appendNullableOperators(list);

        return list;
    }

    protected useEditor() {
        var op = this.get_operator().key;

        return op === FilterOperators.EQ ||
            op === FilterOperators.NE ||
            (this.useRelative && (
                op === FilterOperators.LT ||
                op === FilterOperators.LE ||
                op === FilterOperators.GT ||
                op === FilterOperators.GE));
    }

    getEditorOptions() {
        var opt = super.getEditorOptions();
        if (this.useEditor() && this.editorType === (this.get_field().editorType ?? 'String')) {
            opt = extend(opt, this.get_field().editorParams);
        }

        return opt;
    }

    createEditor() {
        if (this.useEditor()) {
            var editorType = EditorTypeRegistry.get(this.editorType) as typeof Widget<{}>;

            this.editor = new editorType({
                element: el => this.get_container().append(el),
                ...this.getEditorOptions()
            });

            return;
        }

        super.createEditor();
    }

    protected useIdField(): boolean {
        return this.useEditor();
    }

    initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        super.initQuickFilter(filter);

        filter.type = EditorTypeRegistry.get(this.editorType);
    }
}

@Filtering('Enum')
export class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
    constructor() {
        super(EnumEditor);
    }

    getOperators() {
        var op = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }];
        return this.appendNullableOperators(op);
    }

    getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}

@Filtering('Integer')
export class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    constructor() {
        super(IntegerEditor);
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}

@Filtering('Lookup')
export class LookupFiltering extends BaseEditorFiltering<LookupEditor> {

    constructor() {
        super(LookupEditor);
    }

    getOperators(): FilterOperator[] {
        var ops = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }, { key: FilterOperators.contains }, { key: FilterOperators.startsWith }]
        return this.appendNullableOperators(ops);
    }

    protected useEditor(): boolean {
        var op = this.get_operator().key;
        return op == FilterOperators.EQ || op == FilterOperators.NE;
    }

    protected useIdField(): boolean {
        return this.useEditor();
    }

    getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}

@Filtering('ServiceLookup')
export class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {

    constructor() {
        super(ServiceLookupEditor);
    }

    getOperators(): FilterOperator[] {
        var ops = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }, { key: FilterOperators.contains }, { key: FilterOperators.startsWith }]
        return this.appendNullableOperators(ops);
    }

    protected useEditor(): boolean {
        var op = this.get_operator().key;
        return op == FilterOperators.EQ || op == FilterOperators.NE;
    }

    protected useIdField(): boolean {
        return this.useEditor();
    }

    getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}

@Filtering('String')
export class StringFiltering extends BaseFiltering {

    getOperators(): FilterOperator[] {
        var ops = [
            { key: FilterOperators.contains },
            { key: FilterOperators.startsWith },
            { key: FilterOperators.EQ },
            { key: FilterOperators.NE }
        ];
        return this.appendNullableOperators(ops);
    }

    validateEditorValue(value: string) {
        if (value.length === 0) {
            return value;
        }

        return super.validateEditorValue(value);
    }
}

export namespace FilteringTypeRegistry {

    let knownTypes: { [key: string]: Function };

    function initialize(): void {

        if (knownTypes != null)
            return;

        knownTypes = {};

        for (var type of getTypes()) {
            if (!isAssignableFrom(IFiltering, type))
                continue;

            var fullName = getTypeFullName(type).toLowerCase();

            knownTypes[fullName] = type;

            for (var k of Config.rootNamespaces) {
                if (fullName.startsWith(k.toLowerCase() + '.')) {
                    var kx = fullName.substring(k.length + 1).toLowerCase();

                    if (knownTypes[kx] == null) {
                        knownTypes[kx] = type;
                    }
                }
            }
        }

        setTypeKeysWithoutFilterHandlerSuffix();
    }

    function setTypeKeysWithoutFilterHandlerSuffix() {
        var suffix = 'filtering';

        for (var k of Object.keys(knownTypes)) {
            if (!k.endsWith(suffix))
                continue;

            var p = k.substring(0, k.length - suffix.length);
            if (!p)
                continue;

            if (knownTypes[p] != null)
                continue;

            knownTypes[p] = knownTypes[k];
        }
    }

    export function reset(): void {
        knownTypes = null;
    }

    export function get(key: string): Function {

        if (!key)
            throw new ArgumentNullException('key');

        initialize();
        var formatterType = knownTypes[key.toLowerCase()];
        if (formatterType == null)
            throw new Exception(stringFormat(
                "Can't find {0} filter handler type!", key));

        return formatterType;
    }
}