import { interfaceTypeInfo, nsSerenity, registerType } from "../../base";
import { QuickFilter } from "../datagrid/quickfilter";
import { Widget } from "../widgets/widget";

export interface IQuickFiltering {
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}

export class IQuickFiltering {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}
