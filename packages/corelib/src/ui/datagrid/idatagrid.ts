import { ISleekGrid } from "@serenity-is/sleekgrid";
import { IRemoteView } from "../../slick";
import { FilterStore } from "../filtering/filterstore";

/**
 * Abstraction for data grids that expose the root element, underlying SlickGrid
 * instance, remote view, and filter store.
 */
export interface IDataGrid {
    /**
     * Returns the root DOM element of the grid widget.
     * @returns The grid container element.
     */
    getElement(): HTMLElement;
    /**
     * Returns the underlying SlickGrid / SleekGrid instance.
     * @returns The grid instance used for rendering and interaction.
     */
    getGrid(): ISleekGrid;
    /**
     * Returns the remote view that manages paging, sorting and server communication.
     * @returns The remote view instance.
     */
    getView(): IRemoteView<any>;
    /**
     * Returns the filter store owned by the grid.
     * @returns The current {@link FilterStore} instance.
     */
    getFilterStore(): FilterStore;
}
