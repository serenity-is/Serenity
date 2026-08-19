import { interfaceTypeInfo, nsSerenity, registerType } from "../../base";
import { QuickFilter } from "../datagrid/quickfilter";
import { Widget } from "../widgets/widget";

/**
 * Interface for filtering handlers that can initialize a quick filter.
 */
export abstract class IQuickFiltering {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Interface for filtering handlers that can initialize a quick filter.
 */
export interface IQuickFiltering {
    /** Initializes a quick filter for this field. */
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}

