import type { IGrid } from "./igrid";

export interface GridPlugin {
    init(grid: IGrid): void;
    pluginName?: string;
    destroy?: () => void;
}

/** @deprecated Use GridPlugin instead */
export interface IPlugin extends GridPlugin {

}

export interface GridPluginHost {
    getPluginByName(name: string): GridPlugin;
    registerPlugin(plugin: GridPlugin): void;
    unregisterPlugin(plugin: GridPlugin): void;
}
