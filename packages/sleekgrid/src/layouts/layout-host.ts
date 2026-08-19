import type { GridPluginHost } from "../core/grid-plugin";
import type { GridSignals } from "../core/grid-signals";
import type { ISleekGrid } from "../core/isleekgrid";
import { ViewportInfo } from "../core/viewportinfo";
import type { GridLayoutRefs } from "./layout-refs";

/**
 * Minimal host surface exposed to {@link LayoutEngine} implementations.
 * Narrower than {@link ISleekGrid}; only what layouts need is exposed.
 */
export interface LayoutHost extends Pick<ISleekGrid, "getAllColumns" | "getColumns" | "getOptions" |
    "getContainerNode" | "getDataLength" |"onAfterInit">, GridPluginHost {
    /**
     * Returns the shared reactive signals controlling visibility/pinning.
     * @returns Grid signals object.
     */
    getSignals(): GridSignals;
    /**
     * Returns computed viewport metrics (dimensions, scroll flags, heights).
     * @returns Current {@link ViewportInfo}.
     */
    getViewportInfo(): ViewportInfo;
    /**
     * Removes a DOM node via the grid's configured sanitizer/custom remover.
     * @param node - Element to remove.
     */
    removeNode(node: HTMLElement): void;
    /** Mutable refs tracking per-band DOM nodes and pinning/frozen state. */
    readonly refs: GridLayoutRefs;
}
