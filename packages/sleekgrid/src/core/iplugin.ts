import type { IGrid } from "./igrid";

export interface IPlugin {
    init(grid: IGrid): void;
    pluginName?: string;
    destroy?: () => void;
}
