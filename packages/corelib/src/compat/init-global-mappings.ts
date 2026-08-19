import { Fluent, getGlobalObject } from "../base";

function copyProps(source: any, target: any) {
    if (!source || !target)
        return;
    Object.keys(source).forEach(function (key) {
        if (source[key] != null &&
            target[key] == null &&
            key !== '_') {
            Object.defineProperty(target, key, {
                get: function () { return source[key]; },
                set: function (v) { source[key] = v; },
                enumerable: true,
                configurable: true
            });
        }
    });
}

/**
 * Options bag for {@link initGlobalMappings}.
 */
export interface InitGlobalMappingsOptions {
    /** Global object to install mappings on; defaults to {@link getGlobalObject}. */
    globals?: any;
    /** Core library exports to expose as `Serenity` / `Q` globals. */
    corelib?: any;
    /** `domwise` package exports to merge into `Serenity`. */
    domwise?: any;
    /** `sleekgrid` package exports to expose as `Slick` and merge into `Serenity`. */
    sleekgrid?: any;
    /** `extensions` package exports to merge into `Serenity` / `Serenity.Extensions`. */
    extensions?: any;
    /** `proextensions` package exports to merge into `Serenity`. */
    proextensions?: any;
    /** Bootstrap module (unwraps `.default` if needed) to expose as `bootstrap`. */
    bootstrap?: any;
    /** flatpickr module to expose as `flatpickr` (also calls {@link initFlatpickrLocale}). */
    flatpickr?: any;
    /** GLightbox module to expose as `GLightbox`. */
    glightbox?: any;
    /** Mousetrap module to expose as `Mousetrap`. */
    mousetrap?: any;
    /** NProgress module to expose as `NProgress` (also calls {@link initNProgress}). */
    nprogress?: any;
    /** SortableJS module to expose as `Sortable`. */
    sortable?: any;
}

/**
 * Installs legacy global namespace mappings (`Serenity`, `Slick`, `Q`, and vendor globals) for
 * compatibility with feature packages that consume globals via `tsbuild`'s `importAsGlobals`.
 * @param options - Bag of package exports / vendor modules to expose on the global object.
 * @remarks
 * - When `corelib` is provided it becomes `globals.Serenity` (or is merged via live getters if `Serenity` already exists).
 * - `sleekgrid` populates `globals.Slick` and is merged into `Serenity`; `Aggregators`/`AggregateFormatting` sub-objects and `RemoteView` are synced between `Slick` and `Serenity`.
 * - `extensions`/`proextensions`/`domwise` are merged into `Serenity` (extensions also under `Serenity.Extensions`).
 * - `bootstrap`/`mousetrap`/`sortable`/`nprogress`/`glightbox`/`flatpickr` unwrap `.default` when needed and are assigned to `bootstrap`/`Mousetrap`/`Sortable`/`NProgress`/`GLightbox`/`flatpickr` respectively.
 * - Missing or already-present targets are merged via getter/setter proxies (`copyProps`) so later assignments stay in sync.
 * @example
 * initGlobalMappings({ corelib: SerenityCore, sleekgrid: SlickGrid, globals: window });
 */
export function initGlobalMappings({ corelib, globals, domwise, sleekgrid,
    extensions, proextensions, bootstrap, flatpickr, glightbox, mousetrap, nprogress, sortable }: InitGlobalMappingsOptions): void {
    globals = globals ?? getGlobalObject();

    if (corelib) {
        if (!globals.Serenity) {
            globals.Serenity = corelib;
        }
        else if (globals.Serenity !== corelib) {
            copyProps(corelib, globals.Serenity);
        }
    }

    if (corelib || sleekgrid || domwise || extensions || proextensions) {
        if (!globals.Serenity) {
            globals.Serenity = Object.create(null);
        }

        if (sleekgrid) {
            if (!globals.Slick) {
                globals.Slick = sleekgrid;
            }
            else if (globals.Slick !== sleekgrid) {
                copyProps(sleekgrid, globals.Slick);
            }

            if (globals.Serenity !== sleekgrid) {
                copyProps(sleekgrid, globals.Serenity);
            }

            ['Aggregators', 'AggregateFormatting'].forEach(function (x) {
                globals.Slick[x] = globals.Slick[x] || {};
                copyProps(globals.Serenity[x], globals.Slick[x]);
            });

            ['RemoteView'].forEach(function (x) {
                globals.Slick[x] = globals.Serenity[x];
            });
        }
        else if (globals.Slick && globals.Serenity !== globals.Slick) {
            copyProps(globals.Slick, globals.Serenity);
        }

        if (!globals.Q) {
            globals.Q = globals.Serenity;
        }
        else if (globals.Q !== globals.Serenity) {
            copyProps(globals.Serenity, globals.Q);
        }

        if (domwise && globals.Serenity !== domwise) {
            copyProps(domwise, globals.Serenity);
        }

        if (extensions) {
            if (globals.Serenity !== extensions) {
                copyProps(extensions, globals.Serenity);
            }
            if (!globals.Serenity.Extensions) {
                globals.Serenity.Extensions = extensions;
            }
            else if (globals.Serenity.Extensions !== extensions) {
                copyProps(extensions, globals.Serenity.Extensions);
            }
        }

        if (proextensions) {
            if (globals.Serenity !== proextensions) {
                copyProps(proextensions, globals.Serenity);
            }
        }
    }

    if (bootstrap) {
        if (typeof bootstrap.default === "object")
            bootstrap = bootstrap.default;
        globals.bootstrap = bootstrap;
    }

    if (mousetrap) {
        if (typeof mousetrap.default === "function")
            mousetrap = mousetrap.default;
        globals.Mousetrap = mousetrap;
    }

    if (sortable) {
        if (typeof sortable.default === "function")
            sortable = sortable.default;
        globals.Sortable = sortable;
    }

    if (nprogress) {
        if (typeof nprogress.default === "object")
            nprogress = nprogress.default;
        globals.NProgress = nprogress;
        initNProgress(nprogress);
    }

    if (glightbox) {
        if (typeof glightbox.default === "function")
            glightbox = glightbox.default;
        globals.GLightbox = glightbox;
    }

    if (flatpickr) {
        if (typeof flatpickr.default === "function")
            flatpickr = flatpickr.default;
        globals.flatpickr = flatpickr;
        initFlatpickrLocale(flatpickr);
    }
}

/**
 * Localizes a flatpickr instance to the document language.
 * @param flatpickr - flatpickr module/instance with a `l10ns` dictionary and `localize` method; no-op if missing `l10ns`.
 * @remarks Reads `document.documentElement.lang` (falls back to `"en"`), tries the full locale (e.g. `"tr-tr"`) then the base language (e.g. `"tr"`) if available in `flatpickr.l10ns`. Called automatically by {@link initGlobalMappings} when a flatpickr module is provided.
 * @example
 * initFlatpickrLocale(flatpickr);
 */
export function initFlatpickrLocale(flatpickr: any): void {
    if (!flatpickr || !flatpickr.l10ns)
        return;
    let culture = typeof document === "undefined" ? 'en' : (document.documentElement?.lang || 'en').toLowerCase();
    if (flatpickr.l10ns[culture]) {
        flatpickr.localize(flatpickr.l10ns[culture]);
    } else {
        culture = culture.split('-')[0];
        flatpickr.l10ns[culture] && flatpickr.localize(flatpickr.l10ns[culture]);
    }
}

/**
 * Wires NProgress to global `ajaxStart`/`ajaxStop` events (via {@link Fluent}).
 * @param nprogress - NProgress instance; defaults to `getGlobalObject().NProgress` when omitted.
 * @returns `true` once initialized (`nprogress.serenityInit` is set); `undefined`/falsy if already initialized, missing, or `document` is unavailable.
 * @remarks Starts the progress bar 200 ms after `ajaxStart` (debounced) and completes it on `ajaxStop`. No-ops if `start`/`done` are missing, already initialized, or running outside a browser. Called automatically by {@link initGlobalMappings} when an `nprogress` module is provided.
 * @example
 * initNProgress(NProgress);
 */
export function initNProgress(nprogress?: any): boolean {
    nprogress = nprogress || getGlobalObject()?.NProgress;
    if (!nprogress ||
        !nprogress.start ||
        !nprogress.done ||
        nprogress.serenityInit ||
        typeof document === 'undefined')
        return;

    let npt: number;
    Fluent.on(document, "ajaxStart", function () {
        clearTimeout(npt);
        npt = setTimeout(nprogress.start, 200);
    });

    Fluent.on(document, "ajaxStop", function () {
        clearTimeout(npt);
        nprogress.done();
    });

    return nprogress.serenityInit = true;
}
