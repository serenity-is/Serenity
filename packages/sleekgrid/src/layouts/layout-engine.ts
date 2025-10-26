import { Column } from "../core/column";
import { GridOptions } from "../core/gridoptions";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

export interface LayoutEngine {
    layoutName: string;
    init(host: LayoutHost): void;
    destroy(): void;
    afterHeaderColumnDrag(): void;
    afterSetOptions(args: GridOptions): void;
    calcCanvasWidth(): number;
    getCanvasWidth(): number;
    realScrollHeightChange(): void;
    /** this might be called before init, chicken egg situation */
    reorderViewColumns?(viewCols: Column[], refs: GridLayoutRefs): Column[];
    resizeCanvas(): void;
    setOverflow(): void;
    setPaneVisibility?(): void;
    updateCanvasWidth(): boolean;
    updateHeadersWidth(): void;
}
