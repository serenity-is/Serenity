import { PropertyItem } from "../../Services/PropertyItem";
import { RemoteView } from "../../Slick/RemoteView";
import { FilterStore } from "../Filtering/FilterStore";

export interface IDataGrid {
    getElement(): JQuery;
    getGrid(): Slick.Grid;
    getView(): RemoteView<any>;
    getFilterStore(): FilterStore;
}

declare global {
    namespace Slick {
        export interface Column {
            sourceItem?: PropertyItem;
        }
    }
}