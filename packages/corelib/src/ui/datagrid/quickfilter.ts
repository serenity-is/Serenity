import { ListRequest } from "@serenity-is/corelib/q";
import { Widget } from "../widgets/widget";

export interface QuickFilterArgs<TWidget> {
    field?: string;
    widget?: TWidget;
    request?: ListRequest;
    equalityFilter?: any;
    value?: any;
    active?: boolean;
    handled?: boolean;
}

export interface QuickFilter<TWidget extends Widget<TOptions>, TOptions> {
    field?: string;
    type?: new (element: JQuery, options: TOptions) => TWidget;
    handler?: (h: QuickFilterArgs<TWidget>) => void;
    title?: string;
    options?: TOptions;
    element?: (e: JQuery) => void;
    init?: (w: TWidget) => void;
    separator?: boolean;
    cssClass?: string;
    loadState?: (w: TWidget, state: any) => void;
    saveState?: (w: TWidget) => any;
    displayText?: (w: TWidget, label: string) => string;
}