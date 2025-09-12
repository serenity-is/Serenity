import { interfaceTypeInfo, registerType, type PropertyItem } from "../../base";
import { CriteriaWithText } from "./criteriawithtext";
import { FilterOperator } from "./filteroperator";

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

export class IFiltering {
    static typeInfo = interfaceTypeInfo("Serenity.IFiltering"); static { registerType(this); }
}