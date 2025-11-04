import { ISleekGrid } from "@serenity-is/sleekgrid";
import { IRemoteView } from "../../slick";
import { FilterStore } from "../filtering/filterstore";

export interface IDataGrid {
    getElement(): HTMLElement;
    getGrid(): ISleekGrid;
    getView(): IRemoteView<any>;
    getFilterStore(): FilterStore;
}
