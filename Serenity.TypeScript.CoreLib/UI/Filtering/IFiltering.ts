namespace Serenity {
    export interface IFiltering {
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

    @Serenity.Decorators.registerInterface('Serenity.IQuickFiltering')
    export class IFiltering {
    }

    export interface CriteriaWithText {
        criteria?: any[];
        displayText?: string;
    }

    export interface IQuickFiltering {
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }

    @Serenity.Decorators.registerInterface('Serenity.IQuickFiltering')
    export class IQuickFiltering {
    }
}