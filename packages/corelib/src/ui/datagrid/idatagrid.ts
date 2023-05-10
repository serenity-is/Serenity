import { Grid } from "@serenity-is/sleekgrid"
import { RemoteView } from "@serenity-is/corelib/slick";
import { FilterStore } from "../filtering/filterstore";

export interface IDataGrid {
    getElement(): JQuery;
    getGrid(): Grid;
    getView(): RemoteView<any>;
    getFilterStore(): FilterStore;
}
