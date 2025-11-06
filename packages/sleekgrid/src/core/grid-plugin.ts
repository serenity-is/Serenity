import type { ISleekGrid } from "./isleekgrid";

export interface GridPlugin {
    init(grid: ISleekGrid): void;
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
