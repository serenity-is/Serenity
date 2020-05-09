/// <reference types="jquery" />
/// <reference types="jqueryui" />
declare namespace Serenity {
    interface FilterOperator {
        key?: string;
        title?: string;
        format?: string;
    }
    namespace FilterOperators {
        const isTrue = "true";
        const isFalse = "false";
        const contains = "contains";
        const startsWith = "startswith";
        const EQ = "eq";
        const NE = "ne";
        const GT = "gt";
        const GE = "ge";
        const LT = "lt";
        const LE = "le";
        const BW = "bw";
        const IN = "in";
        const isNull = "isnull";
        const isNotNull = "isnotnull";
        const toCriteriaOperator: Q.Dictionary<string>;
    }
}
declare namespace Serenity {
    interface FilterLine {
        field?: string;
        operator?: string;
        isOr?: boolean;
        leftParen?: boolean;
        rightParen?: boolean;
        validationError?: string;
        criteria?: any[];
        displayText?: string;
        state?: any;
    }
}
declare namespace Serenity {
    class FilterStore {
        constructor(fields: PropertyItem[]);
        static getCriteriaFor(items: FilterLine[]): any[];
        static getDisplayTextFor(items: FilterLine[]): string;
        private changed;
        private displayText;
        private fields;
        private fieldByName;
        private items;
        get_fields(): PropertyItem[];
        get_fieldByName(): Q.Dictionary<PropertyItem>;
        get_items(): FilterLine[];
        raiseChanged(): void;
        add_changed(value: (e: JQueryEventObject, a: any) => void): void;
        remove_changed(value: (e: JQueryEventObject, a: any) => void): void;
        get_activeCriteria(): any[];
        get_displayText(): string;
    }
}
declare namespace Serenity {
    interface IFiltering {
        createEditor(): void;
        getCriteria(): CriteriaWithText;
        getOperators(): FilterOperator[];
        loadState(state: any): void;
        saveState(): any;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
    }
    class IFiltering {
    }
    interface CriteriaWithText {
        criteria?: any[];
        displayText?: string;
    }
    interface IQuickFiltering {
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class IQuickFiltering {
    }
    abstract class BaseFiltering implements IFiltering, IQuickFiltering {
        private field;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        private container;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        private operator;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
        abstract getOperators(): FilterOperator[];
        protected appendNullableOperators(list: FilterOperator[]): FilterOperator[];
        protected appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
        protected isNullable(): boolean;
        createEditor(): void;
        protected operatorFormat(op: FilterOperator): any;
        protected getTitle(field: PropertyItem): any;
        protected displayText(op: FilterOperator, values?: any[]): string;
        protected getCriteriaField(): string;
        getCriteria(): CriteriaWithText;
        loadState(state: any): void;
        saveState(): any;
        protected argumentNull(): Q.ArgumentNullException;
        validateEditorValue(value: string): string;
        getEditorValue(): string;
        getEditorText(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
        editorType: any;
        constructor(editorType: any);
        protected useEditor(): boolean;
        protected editor: TEditor;
        createEditor(): void;
        protected useIdField(): boolean;
        getCriteriaField(): string;
        getEditorOptions(): any;
        loadState(state: any): void;
        saveState(): any;
        getEditorValue(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class DateFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class BooleanFiltering extends BaseFiltering {
        getOperators(): FilterOperator[];
    }
    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
        getCriteria(): CriteriaWithText;
    }
    class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
        constructor();
        getOperators(): Serenity.FilterOperator[];
    }
    class EditorFiltering extends BaseEditorFiltering<Serenity.Widget<any>> {
        constructor();
        editorType: string;
        useRelative: boolean;
        useLike: boolean;
        getOperators(): Serenity.FilterOperator[];
        protected useEditor(): boolean;
        getEditorOptions(): any;
        createEditor(): void;
        protected useIdField(): boolean;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class StringFiltering extends BaseFiltering {
        getOperators(): Serenity.FilterOperator[];
        validateEditorValue(value: string): string;
    }
    namespace FilteringTypeRegistry {
        function get(key: string): Function;
    }
}
declare namespace Serenity {
    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        private store;
        private onFilterStoreChanged;
        constructor(div: JQuery, opt?: TOptions);
        destroy(): void;
        protected filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }
}
declare namespace Serenity {
    class FilterPanel extends FilterWidgetBase<any> {
        private rowsDiv;
        constructor(div: JQuery);
        private showInitialLine;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        protected filterStoreChanged(): void;
        updateRowsFromStore(): void;
        private showSearchButton;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        private updateStoreOnReset;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        protected getTemplate(): string;
        protected initButtons(): void;
        protected searchButtonClick(e: JQueryEventObject): void;
        get_hasErrors(): boolean;
        search(): void;
        protected addButtonClick(e: JQueryEventObject): void;
        protected resetButtonClick(e: JQueryEventObject): void;
        protected findEmptyRow(): JQuery;
        protected addEmptyRow(popupField: boolean): JQuery;
        protected onRowFieldChange(e: JQueryEventObject): void;
        protected rowFieldChange(row: JQuery): void;
        protected removeFiltering(row: JQuery): void;
        protected populateOperatorList(row: JQuery): void;
        protected getFieldFor(row: JQuery): PropertyItem;
        protected getFilteringFor(row: JQuery): IFiltering;
        protected onRowOperatorChange(e: JQueryEventObject): void;
        protected rowOperatorChange(row: JQuery): void;
        protected deleteRowClick(e: JQueryEventObject): void;
        protected updateButtons(): void;
        protected andOrClick(e: JQueryEventObject): void;
        protected leftRightParenClick(e: JQueryEventObject): void;
        protected updateParens(): void;
    }
}
declare namespace Serenity {
    class FilterDialog extends TemplatedDialog<any> {
        private filterPanel;
        constructor();
        get_filterPanel(): FilterPanel;
        protected getTemplate(): string;
        protected getDialogOptions(): JQueryUI.DialogOptions;
    }
}
declare namespace Serenity {
    class FilterDisplayBar extends FilterWidgetBase<any> {
        constructor(div: JQuery);
        protected filterStoreChanged(): void;
        protected getTemplate(): string;
    }
}
