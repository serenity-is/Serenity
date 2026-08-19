import { interfaceTypeInfo, nsSerenity, registerType, type PropertyItem } from "../../base";
import { CriteriaWithText } from "./criteriawithtext";
import { FilterOperator } from "./filteroperator";

/**
 * Interface for filtering handlers that build criteria and editors for a field.
 */
export abstract class IFiltering {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Interface for filtering handlers that build criteria and editors for a field.
 */
export interface IFiltering {
    /** Creates the editor for the current operator. */
    createEditor(): void;
    /** Returns the criteria and display text for the current operator. */
    getCriteria(): CriteriaWithText;
    /** Returns the operators supported by this filtering handler. */
    getOperators(): FilterOperator[];
    /** Loads persisted state into the editor. */
    loadState(state: any): void;
    /** Saves the editor state for persistence. */
    saveState(): any;
    /** Returns the field being filtered. */
    get_field(): PropertyItem;
    /** Sets the field being filtered. */
    set_field(value: PropertyItem): void;
    /** Returns the container element for the editor. */
    get_container(): HTMLElement;
    /** Sets the container element for the editor. */
    set_container(value: HTMLElement): void;
    /** Returns the current operator. */
    get_operator(): FilterOperator;
    /** Sets the current operator. */
    set_operator(value: FilterOperator): void;
}