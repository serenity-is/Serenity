import type { GridPluginHost } from "../core/grid-plugin";
import type { GridSignals } from "../core/grid-signals";
import type { ISleekGrid } from "../core/isleekgrid";
import { ViewportInfo } from "../core/viewportinfo";
import type { GridLayoutRefs } from "./layout-refs";

export interface LayoutHost extends Pick<ISleekGrid, "getAllColumns" | "getColumns" | "getOptions" |
    "getContainerNode" | "getDataLength" |"onAfterInit">, GridPluginHost {
    getSignals(): GridSignals;
    getViewportInfo(): ViewportInfo;
    removeNode(node: HTMLElement): void;
    readonly refs: GridLayoutRefs;
}
