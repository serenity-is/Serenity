import { Column } from "../core/column";
import { GridOptions } from "../core/gridoptions";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

/**
 * Pluggable layout strategy responsible for creating DOM panes and responding
 * to grid option changes. The grid instantiates one engine (typically
 * {@link BasicLayout} or {@link FrozenLayout}).
 */
export interface LayoutEngine {
    /** Human-readable layout name (e.g. `"BasicLayout"`). */
    layoutName: string;
    /**
     * Initializes the layout, creating DOM inside `host.getContainerNode()`.
     * @param host - Layout host providing grid state, signals and refs.
     */
    init(host: LayoutHost): void;
    /** Tears down DOM and listeners created by {@link LayoutEngine.init}. */
    destroy(): void;
    /**
     * Adjusts the frozen-row refs from the current grid options without a
     * full re-layout. Called when `frozenRows` / `frozenBottom` change.
     */
    adjustFrozenRowsOption?(): void;
    /**
     * Called after `grid.setOptions(args)` merges new options.
     * @param args - Options delta passed to `setOptions`.
     */
    afterSetOptions(args: GridOptions): void;
    /**
     * Optionally reorders the visible columns before they are laid out.
     * May be called before {@link LayoutEngine.init} during early option setup.
     * @param viewCols - Current visible columns in display order.
     * @param refs - Mutable layout refs whose config may be updated.
     * @returns Reordered columns, or `null` when no reorder is needed.
     */
    reorderViewColumns?(viewCols: Column[], refs: GridLayoutRefs): Column[];
    /** Whether the engine supports pinned (frozen) columns. */
    supportPinnedCols?: boolean;
    /** Whether the engine supports end-pinned columns. */
    supportPinnedEnd?: boolean;
    /** Whether the engine supports top-frozen rows. */
    supportFrozenRows?: boolean;
    /** Whether the engine supports bottom-frozen rows. */
    supportFrozenBottom?: boolean;
}
