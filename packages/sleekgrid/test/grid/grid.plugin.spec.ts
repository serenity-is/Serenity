import type { ISleekGrid } from "../../src/core";
import type { GridPlugin } from "../../src/core/grid-plugin";
import { SleekGrid } from "../../src/grid/sleekgrid";

it('should call plugin init with grid instance', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let pluginInitGrid: ISleekGrid | null = null;
    const plugin: GridPlugin = {
        init: (grid: ISleekGrid) => {
            pluginInitGrid = grid;
        }
    }

    grid.registerPlugin(plugin);

    expect(pluginInitGrid).toBe(grid);
});

it('should be able to get plugin by name if it exists', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    const plugin: GridPlugin = {
        init: (_grid: ISleekGrid) => {},
        pluginName: 'test'
    }

    grid.registerPlugin(plugin);

    expect(grid.getPluginByName('test')).toBe(plugin);
});

it('should be able to unregister a plugin', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let pluginDestroyCalled = false;
    const plugin: GridPlugin = {
        init: (_grid: ISleekGrid) => {},
        destroy: () => {
            pluginDestroyCalled = true;
        },
        pluginName: 'test'
    }

    grid.registerPlugin(plugin);
    expect(grid.getPluginByName('test')).toBe(plugin);

    grid.unregisterPlugin(plugin);

    expect(grid.getPluginByName('test')).toBeUndefined();
    expect(pluginDestroyCalled).toBe(true);
});

it('should call plugin.destroy if it exists', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let pluginDestroyCalled = false;
    const plugin: GridPlugin = {
        init: (_grid: ISleekGrid) => {},
        destroy: () => {
            pluginDestroyCalled = true;
        }
    }

    grid.registerPlugin(plugin);
    grid.destroy();

    expect(pluginDestroyCalled).toBe(true);
});

it('should be able to get plugins without names', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    const newPlugin = (): GridPlugin => ({
        init: (_grid: ISleekGrid) => {
        }
    });

    const firstPlugin = newPlugin();
    const secondPlugin = newPlugin();

    grid.registerPlugin(firstPlugin);
    grid.registerPlugin(secondPlugin);

    expect(grid.getPluginByName(undefined)).toEqual(firstPlugin);

    grid.unregisterPlugin(firstPlugin);

    expect(grid.getPluginByName(undefined)).toEqual(secondPlugin);
});
