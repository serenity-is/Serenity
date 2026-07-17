import { initGlobalMappings, initFlatpickrLocale, initNProgress } from "./init-global-mappings";
import { Fluent } from "../base";

describe("initGlobalMappings", () => {
    let globals: Record<string, any>;

    beforeEach(() => {
        globals = Object.create(null);
    });

    describe("corelib", () => {
        it("should set globals.Serenity to corelib when Serenity does not exist", () => {
            const corelib = { Widget: "widget" };
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity).toBe(corelib);
        });

        it("should not override globals.Serenity when it already equals corelib", () => {
            const corelib = { Widget: "widget" };
            globals.Serenity = corelib;
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity).toBe(corelib);
        });

        it("should copy props to existing Serenity namespace when it differs from corelib", () => {
            const corelib = { Widget: "widget", Form: "form" };
            globals.Serenity = { Existing: "prop" };
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity.Widget).toBe("widget");
            expect(globals.Serenity.Existing).toBe("prop");
            // The copyProps only copies if target[key] == null
            expect(globals.Serenity.Form).toBe("form");
        });

        it("should not copy props when target already has the property", () => {
            const corelib = { Widget: "widget" };
            globals.Serenity = { Widget: "existing" };
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity.Widget).toBe("existing");
        });
    });

    describe("sleekgrid", () => {
        it("should set globals.Slick when it does not exist", () => {
            const sleekgrid = { Grid: "grid" };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Slick).toBe(sleekgrid);
        });

        it("should copy props to existing Slick namespace when it differs from sleekgrid", () => {
            const sleekgrid = { Grid: "grid", Data: "data" };
            globals.Slick = { Existing: "prop" };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Slick.Grid).toBe("grid");
            expect(globals.Slick.Existing).toBe("prop");
        });

        it("should copy sleekgrid props to Serenity namespace", () => {
            const sleekgrid = { Grid: "grid" };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Serenity.Grid).toBe("grid");
        });

        it("should set Q namespace to Serenity when Q does not exist", () => {
            const sleekgrid = { Grid: "grid" };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Q).toBe(globals.Serenity);
        });

        it("should copy Aggregators and AggregateFormatting from Serenity to Slick", () => {
            const sleekgrid = { Grid: "grid" };
            globals.Serenity = {
                Aggregators: { Avg: "avg" },
                AggregateFormatting: { Format: "fmt" }
            };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Slick.Aggregators.Avg).toBe("avg");
            expect(globals.Slick.AggregateFormatting.Format).toBe("fmt");
        });

        it("should set RemoteView from Serenity on Slick", () => {
            const sleekgrid = { Grid: "grid" };
            globals.Serenity = { RemoteView: "remoteView" };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Slick.RemoteView).toBe("remoteView");
        });

        it("should not override existing Aggregators and AggregateFormatting on Slick", () => {
            const sleekgrid = { Grid: "grid" };
            globals.Slick = {
                Aggregators: { ExistingAgg: "agg" }
            };
            globals.Serenity = {
                Aggregators: { Avg: "avg" }
            };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Slick.Aggregators.ExistingAgg).toBe("agg");
            // copyProps only copies when target[key] == null, so Avg should be copied
            expect(globals.Slick.Aggregators.Avg).toBe("avg");
        });
    });

    describe("domwise", () => {
        it("should copy domwise properties to Serenity", () => {
            const domwise = { DomUtils: "utils" };
            initGlobalMappings({ domwise, globals });
            expect(globals.Serenity.DomUtils).toBe("utils");
        });

        it("should not copy if domwise === Serenity", () => {
            globals.Serenity = Object.create(null);
            const domwise = globals.Serenity;
            initGlobalMappings({ domwise, globals });
            // No properties to copy, but importantly no error
            expect(globals.Serenity).toBe(domwise);
        });
    });

    describe("extensions", () => {
        it("should copy properties to Serenity", () => {
            const extensions = { MyExt: "ext" };
            initGlobalMappings({ extensions, globals });
            expect(globals.Serenity.MyExt).toBe("ext");
        });

        it("should set Serenity.Extensions when it does not exist", () => {
            const extensions = { MyExt: "ext" };
            initGlobalMappings({ extensions, globals });
            expect(globals.Serenity.Extensions).toBe(extensions);
        });

        it("should copy to existing Serenity.Extensions when it differs", () => {
            const extensions = { MyExt: "ext" };
            globals.Serenity = { Extensions: { Existing: "prop" } };
            initGlobalMappings({ extensions, globals });
            expect(globals.Serenity.Extensions.MyExt).toBe("ext");
            expect(globals.Serenity.Extensions.Existing).toBe("prop");
        });

        it("should not copy extensions to Serenity if extensions === Serenity", () => {
            globals.Serenity = Object.create(null);
            const extensions = globals.Serenity;
            initGlobalMappings({ extensions, globals });
            expect(globals.Serenity.Extensions).toBe(extensions);
        });
    });

    describe("proextensions", () => {
        it("should copy properties to Serenity when different", () => {
            const proextensions = { ProExt: "pro" };
            globals.Serenity = Object.create(null);
            initGlobalMappings({ proextensions, globals });
            expect(globals.Serenity.ProExt).toBe("pro");
        });
    });

    describe("bootstrap", () => {
        it("should set globals.bootstrap directly when bootstrap has no default export", () => {
            const bootstrap = { Modal: "modal" };
            initGlobalMappings({ bootstrap, globals });
            expect(globals.bootstrap).toBe(bootstrap);
        });

        it("should unwrap default export when bootstrap.default is an object", () => {
            const bootstrapInner = { Modal: "modal" };
            const bootstrap = { default: bootstrapInner };
            initGlobalMappings({ bootstrap, globals });
            expect(globals.bootstrap).toBe(bootstrapInner);
        });
    });

    describe("mousetrap", () => {
        it("should set globals.Mousetrap when mousetrap has no default export", () => {
            const mousetrap = { bind: "bind" };
            initGlobalMappings({ mousetrap, globals });
            expect(globals.Mousetrap).toBe(mousetrap);
        });

        it("should unwrap default export when mousetrap.default is a function", () => {
            const mousetrapInner = vi.fn();
            const mousetrap = { default: mousetrapInner };
            initGlobalMappings({ mousetrap, globals });
            expect(globals.Mousetrap).toBe(mousetrapInner);
        });
    });

    describe("sortable", () => {
        it("should set globals.Sortable when sortable has no default export", () => {
            const sortable = { create: "create" };
            initGlobalMappings({ sortable, globals });
            expect(globals.Sortable).toBe(sortable);
        });

        it("should unwrap default export when sortable.default is a function", () => {
            const sortableInner = vi.fn();
            const sortable = { default: sortableInner };
            initGlobalMappings({ sortable, globals });
            expect(globals.Sortable).toBe(sortableInner);
        });
    });

    describe("nprogress", () => {
        it("should set globals.NProgress when nprogress has no default export", () => {
            const nprogress = { start: vi.fn(), done: vi.fn() };
            initGlobalMappings({ nprogress, globals });
            expect(globals.NProgress).toBe(nprogress);
        });

        it("should unwrap default export when nprogress.default is an object", () => {
            const nprogressInner = { start: vi.fn(), done: vi.fn() };
            const nprogress = { default: nprogressInner };
            initGlobalMappings({ nprogress, globals });
            expect(globals.NProgress).toBe(nprogressInner);
        });
    });

    describe("glightbox", () => {
        it("should set globals.GLightbox when glightbox has no default export", () => {
            const glightbox = { open: "open" };
            initGlobalMappings({ glightbox, globals });
            expect(globals.GLightbox).toBe(glightbox);
        });

        it("should unwrap default export when glightbox.default is a function", () => {
            const glightboxInner = vi.fn();
            const glightbox = { default: glightboxInner };
            initGlobalMappings({ glightbox, globals });
            expect(globals.GLightbox).toBe(glightboxInner);
        });
    });

    describe("flatpickr", () => {
        it("should set globals.flatpickr when flatpickr has no default export", () => {
            const flatpickr = { l10ns: {}, localize: vi.fn() };
            initGlobalMappings({ flatpickr, globals });
            expect(globals.flatpickr).toBe(flatpickr);
        });

        it("should unwrap default export when flatpickr.default is a function", () => {
            const flatpickrInner = Object.assign(vi.fn(), { l10ns: {}, localize: vi.fn() });
            const flatpickr = { default: flatpickrInner };
            initGlobalMappings({ flatpickr, globals });
            expect(globals.flatpickr).toBe(flatpickrInner);
        });
    });

    describe("with no corelib/sleekgrid/domwise/extensions/proextensions", () => {
        it("should not create Serenity namespace", () => {
            initGlobalMappings({ bootstrap: { Modal: "modal" }, globals });
            expect(globals.Serenity).toBeUndefined();
        });

        it("should still set bootstrap, mousetrap, etc.", () => {
            const bootstrap = { Modal: "modal" };
            const mousetrap = { bind: "bind" };
            initGlobalMappings({ bootstrap, mousetrap, globals });
            expect(globals.bootstrap).toBe(bootstrap);
            expect(globals.Mousetrap).toBe(mousetrap);
        });
    });

    describe("when globals is not provided", () => {
        it("should use getGlobalObject() as default", () => {
            const corelib = { Widget: "widget" };
            initGlobalMappings({ corelib });
            const globalObj = (typeof globalThis !== "undefined" ? globalThis : window) as any;
            // The global object may already have Serenity from other tests,
            // but corelib's properties should be accessible on it
            expect(globalObj.Serenity.Widget).toBe("widget");
        });
    });

    describe("with sleekgrid but no Serenity yet", () => {
        it("should create empty Serenity namespace before copying", () => {
            const sleekgrid = { Grid: "grid" };
            initGlobalMappings({ sleekgrid, globals });
            expect(globals.Serenity).toBeTruthy();
        });
    });

    describe("copyProps edge cases", () => {
        it("should trigger the setter when setting a copied property on target", () => {
            const source = { value: 1 };
            const target = {};
            globals.Serenity = target;
            const corelib = source;
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity.value).toBe(1);
            // Setting the property on target triggers the setter defined by copyProps
            globals.Serenity.value = 42;
            expect(source.value).toBe(42);
        });

        it("should not copy property named '_'", () => {
            const corelib = { _: "hidden" as any, visible: "shown" };
            globals.Serenity = { existing: "prop" };
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity.visible).toBe("shown");
            expect((globals.Serenity as any)._).toBeUndefined();
        });
    });

    describe("existing Slick without sleekgrid", () => {
        it("should copy Slick props to Serenity when Slick already exists", () => {
            const corelib = { Widget: "widget" };
            globals.Slick = { ExistingSlickProp: "slick" };
            globals.Serenity = { ExistingSerenityProp: "serenity" };
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity.ExistingSlickProp).toBe("slick");
            expect(globals.Serenity.ExistingSerenityProp).toBe("serenity");
            expect(globals.Serenity.Widget).toBe("widget");
        });

        it("should not copy Slick to Serenity if they are the same object", () => {
            const corelib = { Widget: "widget" };
            const s = { prop: "value" };
            globals.Slick = s;
            globals.Serenity = s;
            initGlobalMappings({ corelib, globals });
            expect(globals.Serenity).toBe(globals.Slick);
        });
    });

    describe("existing Q namespace", () => {
        it("should copy Serenity props to Q when Q exists and differs", () => {
            const corelib = { Widget: "widget" };
            globals.Q = { ExistingQ: "qprop" };
            initGlobalMappings({ corelib, globals });
            expect(globals.Q.Widget).toBe("widget");
            expect(globals.Q.ExistingQ).toBe("qprop");
        });
    });
});

describe("initNProgress", () => {
    beforeEach(() => {
        vi.useFakeTimers();
    });

    afterEach(() => {
        vi.useRealTimers();
    });

    it("should register ajaxStart and ajaxStop handlers on document", () => {
        const nprogress = { start: vi.fn(), done: vi.fn(), serenityInit: undefined as boolean };
        const result = initNProgress(nprogress);

        expect(result).toBe(true);
        expect(nprogress.serenityInit).toBe(true);

        // Dispatch ajaxStart event
        document.dispatchEvent(new Event("ajaxStart"));
        // Should set a timeout for 200ms
        vi.advanceTimersByTime(200);
        expect(nprogress.start).toHaveBeenCalledTimes(1);

        // Dispatch ajaxStop event
        document.dispatchEvent(new Event("ajaxStop"));
        expect(nprogress.done).toHaveBeenCalledTimes(1);
    });

    it("should return early if nprogress has no start/done", () => {
        const nprogress = { serenityInit: false } as any;
        const result = initNProgress(nprogress);
        expect(result).toBeUndefined();
    });

    it("should return early if already initialized", () => {
        const nprogress = { start: vi.fn(), done: vi.fn(), serenityInit: true };
        const result = initNProgress(nprogress);
        expect(result).toBeUndefined();
    });

    it("should clear previous timeout on ajaxStart", () => {
        const nprogress = { start: vi.fn(), done: vi.fn() };
        initNProgress(nprogress);

        // First ajaxStart
        document.dispatchEvent(new Event("ajaxStart"));
        // Second ajaxStart before first timeout completes - should clear first timeout
        document.dispatchEvent(new Event("ajaxStart"));
        vi.advanceTimersByTime(200);
        expect(nprogress.start).toHaveBeenCalledTimes(1);
    });

    it("should clear timeout on ajaxStop so start is not called", () => {
        const nprogress = { start: vi.fn(), done: vi.fn() };
        initNProgress(nprogress);

        document.dispatchEvent(new Event("ajaxStart"));
        document.dispatchEvent(new Event("ajaxStop"));
        vi.advanceTimersByTime(200);
        // Timeout was cleared, so start should not be called
        expect(nprogress.start).not.toHaveBeenCalled();
        expect(nprogress.done).toHaveBeenCalledTimes(1);
    });

    it("should look up NProgress from global object if not provided", () => {
        const nprogress = { start: vi.fn(), done: vi.fn(), serenityInit: undefined as boolean };
        (globalThis as any).NProgress = nprogress;
        const result = initNProgress();
        expect(result).toBe(true);
        expect(nprogress.serenityInit).toBe(true);
        delete (globalThis as any).NProgress;
    });

    it("should return early if NProgress not found on global object", () => {
        const result = initNProgress();
        expect(result).toBeUndefined();
    });
});

describe("initFlatpickrLocale", () => {
    afterEach(() => {
        // Reset document lang
        Object.defineProperty(document.documentElement, "lang", {
            value: "",
            configurable: true
        });
    });

    it("should return early if flatpickr is null", () => {
        expect(() => initFlatpickrLocale(null)).not.toThrow();
    });

    it("should return early if flatpickr has no l10ns", () => {
        expect(() => initFlatpickrLocale({})).not.toThrow();
    });

    it("should localize flatpickr with matching culture from document lang", () => {
        Object.defineProperty(document.documentElement, "lang", {
            value: "tr",
            configurable: true
        });
        const flatpickr = {
            l10ns: {
                tr: { weekdays: { shorthand: ["Paz", "Pzt"] } }
            },
            localize: vi.fn()
        };
        initFlatpickrLocale(flatpickr);
        expect(flatpickr.localize).toHaveBeenCalledWith(flatpickr.l10ns.tr);
    });

    it("should fallback to primary language subtag when full culture not found", () => {
        Object.defineProperty(document.documentElement, "lang", {
            value: "tr-TR",
            configurable: true
        });
        const flatpickr = {
            l10ns: {
                tr: { weekdays: { shorthand: ["Paz", "Pzt"] } }
            },
            localize: vi.fn()
        };
        initFlatpickrLocale(flatpickr);
        expect(flatpickr.localize).toHaveBeenCalledWith(flatpickr.l10ns.tr);
    });

    it("should not localize if language not found in l10ns", () => {
        Object.defineProperty(document.documentElement, "lang", {
            value: "fr",
            configurable: true
        });
        const flatpickr = {
            l10ns: {
                tr: { weekdays: { shorthand: ["Paz", "Pzt"] } }
            },
            localize: vi.fn()
        };
        initFlatpickrLocale(flatpickr);
        expect(flatpickr.localize).not.toHaveBeenCalled();
    });
});
