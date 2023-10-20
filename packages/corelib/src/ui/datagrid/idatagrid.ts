import { Grid } from "@serenity-is/sleekgrid"
import { RemoteView } from "../../slick";
import { FilterStore } from "../filtering/filterstore";

export interface IDataGrid {
    getElement(): JQuery;
    getGrid(): Grid;
    getView(): RemoteView<any>;
    getFilterStore(): FilterStore;
}
