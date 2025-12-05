import { getGlobalObject } from "../base"

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
 * Setups global namespace mappings like Serenity, Slick etc. for compatibility with feature 
 * packages that use global references via tsbuild's importAsGlobals option.
 * @param param0 
 */
export function initGlobalMappings({ corelib, globals, domwise, sleekgrid,
    extensions, proextensions, bootstrap, flatpickr, glightbox, mousetrap, nprogress, sortable }: {
        globals?: any,
        corelib?: any,
        domwise?: any,
        sleekgrid?: any,
        extensions?: any,
        proextensions?: any,
        bootstrap?: any,
        flatpickr?: any,
        glightbox?: any,
        mousetrap?: any,
        nprogress?: any,
        sortable?: any
    }): void {
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

export function initFlatpickrLocale(flatpickr: any) {
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