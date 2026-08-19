import type { ISleekGrid } from "./isleekgrid";

/**
 * Contract for grid plugins (e.g. selection models, overlays).
 */
export interface GridPlugin {
    /**
     * Called by the grid when the plugin is registered.
     * @param grid - Host grid instance the plugin attaches to.
     */
    init(grid: ISleekGrid): void;
    /** Optional unique name used by {@link GridPluginHost.getPluginByName} for lookup. */
    pluginName?: string;
    /** Optional teardown hook; called when the grid or plugin is unregistered. */
    destroy?: () => void;
}

/**
 * Legacy alias for {@link GridPlugin}.
 * @deprecated Use {@link GridPlugin} instead.
 */
export interface IPlugin extends GridPlugin {

}

/**
 * Host surface implemented by the grid for managing {@link GridPlugin} lifetimes.
 */
export interface GridPluginHost {
    /**
     * Retrieves a plugin by its {@link GridPlugin.pluginName}.
     * @param name - Plugin name to look up.
     * @returns The plugin instance, or `null`/`undefined` when not found.
     */
    getPluginByName(name: string): GridPlugin;
    /**
     * Registers a plugin and calls its {@link GridPlugin.init}.
     * @param plugin - Plugin to register.
     */
    registerPlugin(plugin: GridPlugin): void;
    /**
     * Unregisters a plugin, calling {@link GridPlugin.destroy} if defined.
     * @param plugin - Plugin to remove.
     */
    unregisterPlugin(plugin: GridPlugin): void;
}
