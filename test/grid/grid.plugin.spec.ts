import { IPlugin } from "../../src/grid/";
import { Grid } from "../../src/grid/grid";

it('should call plugin init with grid instance', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    let pluginInitGrid: Grid | null = null;
    const plugin: IPlugin = {
        init: (grid: Grid) => {
            pluginInitGrid = grid;
        }
    }

    grid.registerPlugin(plugin);

    expect(pluginInitGrid).toBe(grid);
});

it('should be able to get plugin by name if it exists', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    const plugin: IPlugin = {
        init: (_grid: Grid) => {},
        pluginName: 'test'
    }

    grid.registerPlugin(plugin);

    expect(grid.getPluginByName('test')).toBe(plugin);
});

it('should be able to unregister a plugin', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    let pluginDestroyCalled = false;
    const plugin: IPlugin = {
        init: (_grid: Grid) => {},
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
    const grid = new Grid(document.createElement('div'), [], [], {});

    let pluginDestroyCalled = false;
    const plugin: IPlugin = {
        init: (_grid: Grid) => {},
        destroy: () => {
            pluginDestroyCalled = true;
        }
    }

    grid.registerPlugin(plugin);
    grid.destroy();

    expect(pluginDestroyCalled).toBe(true);
});

it('should be able to get plugins without names', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    const newPlugin = (): IPlugin => ({
        init: (_grid: Grid) => {
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
