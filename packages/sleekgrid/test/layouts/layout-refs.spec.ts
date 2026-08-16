import {
    createGridSignalsAndRefs,
    disposeBandRefs,
    forEachBand,
    getAllCanvasNodes,
    getAllHScrollContainers,
    getAllViewportNodes,
    getAllVScrollContainers,
    mapBands,
    type GridBandRefs,
    type GridLayoutRefs
} from "../../src/layouts/layout-refs";

describe("layout refs helpers", () => {
    it("iterates available bands and handles null refs", () => {
        const { refs } = createGridSignalsAndRefs();
        const bands: string[] = [];
        const mutableRefs = refs as unknown as { end: GridBandRefs | null };
        mutableRefs.end = null;

        forEachBand(refs, band => bands.push(band.key));
        forEachBand(null as unknown as GridLayoutRefs, () => bands.push("invalid"));

        expect(bands).toEqual(["start", "main"]);
        expect(mapBands(refs, band => band.key)).toEqual(["start", "main"]);
        expect(mapBands(refs, band => band.key === "main" ? band.key : null)).toEqual(["main"]);
        expect(mapBands(refs, band => band.key === "main" ? band.key : null, false)).toEqual([null, "main"]);
    });

    it("disposes optional header, footer, canvas, and viewport nodes", () => {
        const { refs } = createGridSignalsAndRefs();
        const band = refs.start;
        const headerParent = document.createElement("div");
        const header = document.createElement("div");
        headerParent.append(header);
        const rowParent = document.createElement("div");
        const row = document.createElement("div");
        rowParent.append(row);
        const footerParent = document.createElement("div");
        const footer = document.createElement("div");
        footerParent.append(footer);
        const viewport = document.createElement("div");
        const canvas = document.createElement("div");
        viewport.append(canvas);
        band.headerCols = header;
        band.headerRowCols = row;
        band.footerRowCols = footer;
        band.canvas.top = canvas;
        const removed: HTMLElement[] = [];

        disposeBandRefs(band, node => node && removed.push(node));

        expect(removed).toEqual([headerParent, rowParent, footerParent, canvas, viewport]);
        expect(band.headerCols).toBeNull();
        expect(band.headerRowCols).toBeNull();
        expect(band.footerRowCols).toBeNull();
        expect(band.canvas.top).toBeNull();
    });

    it("collects available canvas, viewport, and scroll containers", () => {
        const { refs } = createGridSignalsAndRefs();
        const headerParent = document.createElement("div");
        const header = document.createElement("div");
        headerParent.append(header);
        refs.main.headerCols = header;
        const headerRowParent = document.createElement("div");
        refs.main.headerRowCols = document.createElement("div");
        headerRowParent.append(refs.main.headerRowCols);
        const footerParent = document.createElement("div");
        refs.main.footerRowCols = document.createElement("div");
        footerParent.append(refs.main.footerRowCols);
        const bodyViewport = document.createElement("div");
        const bodyCanvas = document.createElement("div");
        bodyViewport.append(bodyCanvas);
        refs.main.canvas.body = bodyCanvas;
        const topViewport = document.createElement("div");
        const topCanvas = document.createElement("div");
        topViewport.append(topCanvas);
        refs.main.canvas.top = topCanvas;

        expect(getAllCanvasNodes(refs)).toEqual([topCanvas, bodyCanvas]);
        expect(getAllViewportNodes(refs)).toEqual([topViewport, bodyViewport]);
        expect(getAllHScrollContainers(refs)).toEqual([
            headerParent,
            headerRowParent,
            topViewport,
            bodyViewport,
            footerParent
        ]);
        expect(getAllVScrollContainers(refs)).toEqual([bodyViewport]);
    });

    it("initializes computed values and clamps pinned and frozen configuration", () => {
        const { signals, refs } = createGridSignalsAndRefs();

        expect(signals.hideColumnHeader.value).toBe(true);
        expect(signals.hideHeaderRow.value).toBe(true);
        expect(refs.pinnedStartLast).toBe(-Infinity);
        expect(refs.pinnedEndFirst).toBe(Infinity);
        expect(refs.frozenTopLast).toBe(-Infinity);
        expect(refs.frozenBottomFirst).toBe(Infinity);

        refs.config.colCount = 5;
        refs.config.dataLength = 10;
        refs.config.pinnedStartCols = 4;
        refs.config.pinnedEndCols = 4;
        refs.config.pinnedLimit = 5;
        refs.config.frozenTopRows = 7;
        refs.config.frozenBottomRows = 7;
        refs.config.frozenLimit = 10;

        expect(refs.pinnedStartCols).toBe(4);
        expect(refs.pinnedEndCols).toBe(1);
        expect(refs.pinnedStartLast).toBe(3);
        expect(refs.pinnedEndFirst).toBe(4);
        expect(refs.main.cellOffset).toBe(4);
        expect(refs.end.cellOffset).toBe(4);
        expect(refs.frozenTopRows).toBe(7);
        expect(refs.frozenBottomRows).toBe(3);
        expect(refs.frozenTopLast).toBe(6);
        expect(refs.frozenBottomFirst).toBe(7);
        expect(signals.pinnedStartCols.value).toBe(4);
        expect(signals.pinnedEndCols.value).toBe(1);
        expect(signals.frozenTopRows.value).toBe(7);
        expect(signals.frozenBottomRows.value).toBe(3);

        refs.config.colCount = -1;
        refs.config.dataLength = -1;
        refs.config.pinnedStartCols = -1;
        refs.config.pinnedEndCols = -1;
        refs.config.frozenTopRows = -1;
        refs.config.frozenBottomRows = -1;
        expect(refs.pinnedStartCols).toBe(0);
        expect(refs.pinnedEndCols).toBe(0);
        expect(refs.frozenTopRows).toBe(0);
        expect(refs.frozenBottomRows).toBe(0);
    });
});
