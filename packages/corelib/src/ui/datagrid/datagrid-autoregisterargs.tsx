import type { ISleekGrid } from "@serenity-is/sleekgrid";
import type { DataGrid } from "./datagrid";

/**
 * Arguments for auto register callback
 */
export interface AutoRegisterArgs<P = any, T = any> {
    pluginType: { new(props: P): T },
    /** Set to true to cancel the auto register process for this grid / dataGrid */
    cancel: boolean;
    /** The data grid instance if available */
    dataGrid?: DataGrid<any> | null;
    /** The ISleekGrid instance */
    sleekGrid?: ISleekGrid;
    /** 
     * Options that can be modified by the callback, which are passed to the plugin/mixin constructor.
     * Note that the options set here only applies to the auto registered instance and 
     * not to any manually created instance.
     */
    options: Partial<P>;
}

export type AutoRegisterHandler<P = any, T = any> = (args: AutoRegisterArgs<P, T>) => void;
