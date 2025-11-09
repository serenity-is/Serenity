import { registerEnum } from "./system";

export enum SummaryType {
    Disabled = -1,
    None = 0,
    Sum = 1,
    Avg = 2,
    Min = 3,
    Max = 4
}

registerEnum(SummaryType, 'Serenity.SummaryType');

export type EditorAddon = (props: { propertyItem?: PropertyItem, editorElement: HTMLElement, documentFragment?: DocumentFragment }) => void;

export interface PropertyItem {
    name: string;
    title?: string;
    hint?: string;
    placeholder?: string;
    editorType?: string | { new(props?: any): any } | PromiseLike<{ new(props?: any): any }>;
    editorParams?: any;
    editorAddons?: { type: string | EditorAddon, params?: any }[];
    editorCssClass?: string;
    category?: string;
    collapsible?: boolean;
    collapsed?: boolean;
    tab?: string;
    cssClass?: string;
    headerCssClass?: string;
    formCssClass?: string;
    maxLength?: number;
    required?: boolean;
    insertable?: boolean;
    insertPermission?: string;
    hideOnInsert?: boolean;
    updatable?: boolean;
    updatePermission?: string;
    hideOnUpdate?: boolean;
    readOnly?: boolean;
    readPermission?: string;
    skipOnLoad?: boolean;
    skipOnSave?: boolean;
    unbound?: boolean;
    /** @deprecated use skipOnSave instead */
    oneWay?: boolean;
    defaultValue?: any;
    localizable?: boolean;
    visible?: boolean;
    allowHide?: boolean;
    formatterType?: string | { new(props?: any): { format(ctx: any): string } } | PromiseLike<{ new(props?: any): { format(ctx: any): string } }>;
    formatterParams?: any;
    displayFormat?: string;
    alignment?: string;
    pin?: "start" | "end" | boolean;
    width?: number;
    widthSet?: boolean;
    minWidth?: number;
    maxWidth?: number;
    labelWidth?: string;
    resizable?: boolean;
    sortable?: boolean;
    sortOrder?: number;
    groupOrder?: number;
    summaryType?: SummaryType;
    editLink?: boolean;
    editLinkItemType?: string;
    editLinkIdField?: string;
    editLinkCssClass?: string;
    filteringType?: string;
    filteringParams?: any;
    filteringIdField?: string;
    notFilterable?: boolean;
    filterOnly?: boolean;
    quickFilter?: boolean;
    quickFilterParams?: any;
    quickFilterSeparator?: boolean;
    quickFilterCssClass?: string;
}

/** Alias for PropertyItem. It may replace PropertyItem in the future */
export type UIFieldItem = PropertyItem;

export interface PropertyItemsData {
    items: PropertyItem[];
    additionalItems: PropertyItem[];
}

/** Alias for PropertyItemsData. It may replace PropertyItemsData in the future */
export type UIFieldSet = PropertyItemsData;