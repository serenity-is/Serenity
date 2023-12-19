import { ListRequest } from "@serenity-is/base";
import { Widget, WidgetNode } from "../widgets/widget";

export interface QuickFilterArgs<TWidget> {
    field?: string;
    widget?: TWidget;
    request?: ListRequest;
    equalityFilter?: any;
    value?: any;
    active?: boolean;
    handled?: boolean;
}

export interface QuickFilter<TWidget extends Widget<P>, P> {
    field?: string;
    type?: (new (node: WidgetNode, options?: P) => TWidget) | (new (props?: P) => TWidget);
    handler?: (h: QuickFilterArgs<TWidget>) => void;
    title?: string;
    options?: P;
    element?: (e: JQuery) => void;
    init?: (w: TWidget) => void;
    separator?: boolean;
    cssClass?: string;
    loadState?: (w: TWidget, state: any) => void;
    saveState?: (w: TWidget) => any;
    displayText?: (w: TWidget, label: string) => string;
}