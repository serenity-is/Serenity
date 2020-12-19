import { Grid } from "../../SlickGrid/Grid";
import { RemoteView } from "../../SlickGrid/RemoteView";
import { FilterStore } from "../Filtering/FilterStore";

export interface IDataGrid {
    getElement(): JQuery;
    getGrid(): Grid;
    getView(): RemoteView<any>;
    getFilterStore(): FilterStore;
}