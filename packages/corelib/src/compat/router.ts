import { bindThis } from "@serenity-is/domwise";
import { Dialog, Fluent, isArrayLike } from "../base";

/**
 * Event payload for the `handleroute` event dispatched by {@link ClassicRouter}.
 * Extends the native {@link Event} with parsed hash-route information.
 */
export interface HandleRouteEvent extends Event {
    /** The route segment being handled for the current index (e.g., `"new"` or `"edit/5"`). */
    route: string,
    /** All route parts split by `"/+/"` from the full hash. */
    parts: string[],
    /** Zero-based index of {@link route} within {@link parts}. */
    index: number,
    /** `true` during the initial few resolves after page load; may affect handler behavior. */
    isInitial: boolean
}

/**
 * A handler of the routes of a page that are not of dialogs, e.g. "view/12" for the record a page shows, see
 * {@link registerPageRoute}. Such a route is the first part of the hash, before the ones of the dialogs
 * that are opened on the page, e.g. "view/12/+/!1".
 */
export interface PageRouteHandler {
    /** Whether a route is one of the page, e.g. "view/12". */
    match(route: string): boolean;
    /**
     * Shows what a route of the page says, or the page without one for an empty route, e.g. when the Back button of the
     * browser takes it out of the hash. It is not called for a change the page made itself, e.g. with a silent navigate.
     */
    handle(route: string): void;
}

/**
 * Contract for the legacy hash-based router.
 * Compat shim for the old `Router` / `Serenity.Router` singleton. The router synchronizes dialog open/close state
 * with the URL hash using `"/+/"` delimited segments and fires `handleroute` events on designated handlers.
 * [DEPRECATED] Hash-based dialog routing is legacy. Prefer explicit client-side routing or modern dialog state management. Kept for backward compatibility.
 */
export interface IClassicRouter {
    /** When `false`, all routing operations become no-ops. */
    enabled: boolean;
    /**
     * Navigates to a new hash, optionally attempting `history.back()` when the URL matches the previous one.
     * @param newHash - Hash string with or without leading `#`. Empty string clears the hash.
     * @param tryBack - When `true`, uses `history.back()` if the target matches {@link ClassicRouter.oldURL}.
     * @param silent - When `true`, suppresses the subsequent `hashchange` handling via {@link ignoreHashChange}.
     */
    navigate(newHash: string, tryBack?: boolean, silent?: boolean): void;
    /**
     * Replaces the current hash without adding a history entry (silent navigation).
     * @param newHash - Target hash (with or without `#`).
     * @param tryBack - When `true`, prefers `history.back()` if applicable.
     */
    replace(newHash: string, tryBack?: boolean): void;
    /**
     * Replaces only the last `"/+/"` segment of the current hash.
     * @param newHash - Replacement for the last segment; when empty/falsy the last segment is removed.
     * @param tryBack - When `true`, prefers `history.back()` if applicable.
     */
    replaceLast(newHash: string, tryBack?: boolean): void;
    /**
     * Registers a dialog open for hash tracking. The actual hash mutation is deferred until the dialog is confirmed open.
     * @param owner - Owner element that triggered the dialog (array-like collections use the first element).
     * @param element - Dialog content element whose `data-qroute` / `data-qprhash` attributes will be managed.
     * @param dialogHash - Factory returning the hash segment for this dialog (e.g., `"!a1"`).
     */
    dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string): void;
    /** Regex used to heuristically detect whether a single hash segment might represent a dialog route (e.g., `new`, `edit/…`, `!…`). */
    mightBeRouteRegex: RegExp;
    /**
     * Resolves the current (or provided) hash by closing/opening dialogs and dispatching `handleroute`.
     * @param newHash - Hash to resolve; defaults to `window.location.hash` when omitted.
     * @returns A status string: `"disabled"` if the router is disabled, `"skipped"` if a recent anchor click looks like a non-route hash, `"shebang"` for `!` prefixed routes, `"missinghandler"` when a handler element cannot be found, or `"calledhandler"` when a `handleroute` event was dispatched.
     */
    resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler";
    /**
     * Temporarily ignores the next `hashchange` event(s).
     * @param expiration - Duration in milliseconds to ignore hash changes. Defaults to `1000`.
     */
    ignoreHashChange(expiration?: number): void;
    /** Removes all event listeners registered by the router and releases resources. */
    destroy(): void;
    /**
     * Registers a handler of the routes of a page that are not of dialogs, e.g. "view/12". They are kept before the
     * routes of the dialogs opened on the page, e.g. "view/12/+/!1", and work besides the `handleroute` event of the
     * `.route-handler` element, e.g. of an entity grid.
     * @param handler - The handler.
     * @returns A function that unregisters the handler.
     */
    registerPageRoute?(handler: PageRouteHandler): () => void;
}

/** The key of the state of an entry of the history that the router added for a dialog, whose value is the hash before it. */
const prevHashKey = "qrouterPrevHash";

/** A hash without its leading `#`. */
const normalizeHash = (hash: string) => (hash ?? "").replace(/^#/, "");

const ignoredSelector = '.s-MessageDialog, .s-MessageModal, .s-PromptDialog, .route-ignore';

/**
 * Legacy hash-based router that maps dialog stack to `"/+/"` delimited hash segments.
 * Compat shim for the old `Router` implementation. Listens to `hashchange`, dialog open/close, and anchor clicks to keep the URL in sync with visible dialogs and to dispatch `handleroute` events.
 * [DEPRECATED] Use explicit routing or state-driven dialog management. Kept solely for backward compatibility with legacy Serenity pages.
 */
export class ClassicRouter implements IClassicRouter {
    private oldURL: string;
    private resolving: number = 0;
    private autoinc: number = 0;
    private ignoreHashLock: number = 0;
    private ignoreHashUntil: number = 0;
    private hashAnchorClickValue: string;
    private hashAnchorClickTime: number;
    /** The hash of a change the router made itself, whose hashchange event is ignored, see {@link expectHash}. */
    private expectedHash: string | null = null;
    private pageRoutes: PageRouteHandler[] = [];
    /** The route of the page in the hash, e.g. "view/12", or an empty string. */
    private pageRoute = "";
    enabled: boolean = true;

    private isEquivalentUrl(url1: string, url2: string) {
        return url1 == url2 || url1 == url2 + '#' || url2 == url1 + '#';
    }

    /** @inheritdoc */
    navigate(newHash: string, tryBack?: boolean, silent?: boolean) {
        if (!this.enabled || this.resolving > 0)
            return;

        newHash = newHash || '';
        newHash = newHash.replace(/^#/, '');
        newHash = (!newHash ? "" : '#' + newHash);
        const newURL = window.location.href.replace(/#$/, '')
            .replace(/#.*$/, '') + newHash;
        if (newURL != window.location.href) {
            if (silent) {
                this.expectHash(newHash);
                // the page made the change, so it knows the route of the page already
                this.pageRoute = this.pageRouteOf(newHash);
            }

            if (tryBack && this.oldURL != null && this.isEquivalentUrl(this.oldURL, newURL)) {
                this.oldURL = null;
                window.history.back();
                return;
            }

            this.oldURL = window.location.href;
            window.location.hash = newHash;
        }
    }

    /**
     * Replaces the hash without adding an entry to the history, and without a hashchange event. When the entry was
     * added for a dialog that is closing, and the new hash is the one before it, it goes back to that entry instead,
     * so the dialog leaves no entry behind for the Back button.
     * @inheritdoc
     */
    replace(newHash: string, tryBack?: boolean) {
        if (!this.enabled || this.resolving > 0)
            return;

        newHash = normalizeHash(newHash);
        if (newHash === normalizeHash(window.location.hash))
            return;

        const state = window.history.state;
        const prevHash = state && typeof state === "object" ? state[prevHashKey] : null;
        if (prevHash === newHash ||
            (tryBack && this.oldURL != null && this.isEquivalentUrl(this.oldURL, this.urlOf(newHash)))) {
            this.oldURL = null;
            this.pageRoute = this.pageRouteOf(newHash);
            this.expectHash(newHash);
            window.history.back();
            return;
        }

        this.pageRoute = this.pageRouteOf(newHash);
        window.history.replaceState(state, "", this.urlOf(newHash));
    }

    /**
     * Adds an entry with the hash to the history, e.g. for a dialog that is opened, without a hashchange event. The entry
     * keeps the hash before it, so {@link replace} goes back to it when the dialog closes.
     */
    private push(newHash: string) {
        if (!this.enabled || this.resolving > 0)
            return;

        newHash = normalizeHash(newHash);
        const current = normalizeHash(window.location.hash);
        if (newHash === current)
            return;

        this.pageRoute = this.pageRouteOf(newHash);
        window.history.pushState({ [prevHashKey]: current }, "", this.urlOf(newHash));
    }

    /** The URL of the page with a hash. */
    private urlOf(hash: string) {
        return window.location.href.replace(/#.*$/, "") + (hash ? "#" + hash : "");
    }

    /** Ignores the hashchange event of a change to the hash that the router makes itself. */
    private expectHash(hash: string) {
        this.expectedHash = normalizeHash(hash);
    }

    /** @inheritdoc */
    registerPageRoute(handler: PageRouteHandler): () => void {
        this.pageRoutes.push(handler);
        // e.g. a link opened the page with a route of it
        this.pageRoute ||= this.pageRouteOf(window.location.hash);
        return () => {
            const index = this.pageRoutes.indexOf(handler);
            index >= 0 && this.pageRoutes.splice(index, 1);
        };
    }

    private findPageRoute(route: string) {
        return route ? this.pageRoutes.find(x => x.match(route)) : undefined;
    }

    /** The route of a page in the first part of a hash, or an empty string. */
    private pageRouteOf(hash: string) {
        const first = normalizeHash(hash).split("/+/")[0];
        return this.findPageRoute(first) ? first : "";
    }

    /** @inheritdoc */
    replaceLast(newHash: string, tryBack?: boolean) {
        if (!this.enabled)
            return;

        let current = window.location.hash || '';
        if (current.charAt(0) == '#')
            current = current.substring(1);

        const parts = current.split('/+/');

        if (parts.length > 1) {
            if (newHash && newHash.length) {
                parts[parts.length - 1] = newHash;
                newHash = parts.join("/+/");
            }
            else {
                parts.splice(parts.length - 1, 1);
                newHash = parts.join("/+/");
            }
        }
        this.replace(newHash, tryBack);
    }


    private isIgnoredDialog(el: HTMLElement) {
        return !!(el?.closest(ignoredSelector) || Dialog.getInstance(el)?.getContentNode()?.closest(ignoredSelector));
    }

    private isVisibleOrHiddenBy(el: HTMLElement): boolean {
        return (el.offsetWidth > 0 && el.offsetHeight > 0) ||  // if visible
            !!(!el.closest(".hidden, [hidden]") && el.closest("[data-hiddenby]")) // or temporarily hidden by another panel
    }

    private getVisibleOrHiddenByDialogs(): HTMLElement[] {
        const visibleDialogs = Array.from(document.querySelectorAll<HTMLElement>(".modal, .panel-body, .ui-dialog-content"))
            .filter(this.isVisibleOrHiddenBy)
            .filter(x => !this.isIgnoredDialog(x));
        visibleDialogs.sort((a: any, b: any) => {
            return parseInt(a.dataset.qrouterorder || "0", 10) - parseInt(b.dataset.qrouterorder || "0", 10);
        });
        return visibleDialogs;
    }

    private pendingDialogHash: () => string;
    private pendingDialogElement: HTMLElement;
    private pendingDialogOwner: HTMLElement;
    private pendingDialogPreHash: string;

    private onDialogOpen(ownerEl: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string,
        byRoute?: boolean) {
        // the route of the page, e.g. "view/12", stays before the ones of the dialogs
        const pageRoute = this.pageRouteOf(window.location.hash);
        const route = pageRoute ? [pageRoute] : [];
        element = isArrayLike(element) ? element[0] : element;
        if (element &&
            this.pendingDialogElement &&
            ((element === this.pendingDialogElement) || (element.contains(this.pendingDialogElement)))) {
            dialogHash = this.pendingDialogHash ?? dialogHash;
            ownerEl = this.pendingDialogOwner;
        }

        this.pendingDialogHash = null;
        this.pendingDialogElement = null;
        this.pendingDialogOwner = null;
        this.pendingDialogPreHash = null;

        ownerEl = isArrayLike(ownerEl) ? ownerEl[0] : ownerEl;
        const ownerIsDialog = ownerEl?.matches(".ui-dialog-content, .panel-body, .modal-content");
        const ownerDlgInst = Dialog.getInstance(ownerEl);
        let value = dialogHash();

        let idPrefix: string;
        if (ownerDlgInst) {
            const dialogs = this.getVisibleOrHiddenByDialogs();
            const index = dialogs.indexOf(ownerDlgInst.getEventsNode());

            for (let i = 0; i <= index; i++) {
                const q = dialogs[i].dataset.qroute;
                if (q && q.length)
                    route.push(q);
            }

            if (!ownerIsDialog) {
                idPrefix = ownerDlgInst?.getContentNode()?.getAttribute("id");
                if (idPrefix) {
                    idPrefix += "_";
                    const id = ownerEl?.getAttribute("id");
                    if (id && id.startsWith(idPrefix))
                        value = id.substring(idPrefix.length) + '@' + value;
                }
            }
        }
        else {
            const id = ownerEl?.getAttribute("id");
            if (id && (!ownerEl.classList.contains("route-handler") ||
                document.querySelector('.route-handler')?.getAttribute("id") != id))
                value = id + "@" + value;
        }

        route.push(value);
        element.dataset.qroute = value;
        // a dialog that a route opened is in the hash already, while one that the user opened is a step of the history,
        // which the Back button closes
        if (byRoute)
            this.replace(route.join("/+/"));
        else
            this.push(route.join("/+/"));
    }

    /** @inheritdoc */
    dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string) {
        if (!this.enabled)
            return;

        const el = isArrayLike(element) ? element[0] : element;
        this.pendingDialogElement = el;
        this.pendingDialogHash = dialogHash;
        this.pendingDialogOwner = isArrayLike(owner) ? owner[0] : owner;
        this.pendingDialogPreHash = this.resolvingPreRoute;
    }

    private resolvingPreRoute: string
    private resolveIndex = 0;

    /** @inheritdoc */
    mightBeRouteRegex: RegExp = /^(new$|edit\/|![0-9]+$)/

    /** @inheritdoc */
    resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler" {
        this.resolveIndex++;

        if (!this.enabled) {
            return "disabled";
        }

        const resolvingCurrent = newHash == null;
        newHash = newHash ?? window.location.hash ?? '';
        if (newHash.charAt(0) == '#')
            newHash = newHash.substring(1);
        const newParts = newHash.split("/+/");

        if (resolvingCurrent &&
            (this.hashAnchorClickTime && new Date().getTime() - this.hashAnchorClickTime < 100) &&
            this.hashAnchorClickValue === newHash &&
            (newHash != '' || window.location.href.indexOf('#') >= 0) &&
            newParts.length == 1 &&
            !newParts.some(x => this.mightBeRouteRegex.test(x))) {
            return "skipped";
        }

        this.resolving++;
        try {
            // the route of the page comes first, before the ones of the dialogs
            const pageHandler = this.findPageRoute(newParts[0]);
            const pageRoute = pageHandler ? newParts.shift() : "";
            if (pageRoute !== this.pageRoute) {
                const previous = this.findPageRoute(this.pageRoute);
                this.pageRoute = pageRoute;
                (pageHandler ?? previous)?.handle(pageRoute);
            }

            const dialogs = this.getVisibleOrHiddenByDialogs();
            const oldParts = dialogs.map((el: any) => el.dataset.qroute);

            let same = 0;
            while (same < dialogs.length &&
                same < newParts.length &&
                oldParts[same] == newParts[same]) {
                same++;
            }

            let closedMessages = false;
            function closeMessages() {
                if (closedMessages) {
                    return;
                }
                closedMessages = true;
                // user pressed back possibly? close any visible confirm dialogs etc.
                Array.from(document.querySelectorAll<HTMLElement>(".s-MessageDialog")).reverse().forEach(x => {
                    if (Fluent.isVisibleLike(x)) {
                        Dialog.getInstance(x)?.close();
                    }
                });
            }

            for (let i = same; i < dialogs.length; i++) {
                const d = dialogs[i];
                Dialog.getInstance(d)?.close("router");
                closeMessages();
            }

            for (let i = same; i < Math.min(newParts.length, 5); i++) {
                let route = newParts[i];
                const routeParts = route.split('@');
                let handler: HTMLElement;
                if (routeParts.length == 2) {
                    const dialog = i > 0 ? dialogs[i - 1] : null;
                    if (dialog) {
                        const idPrefix = Dialog.getInstance(dialog)?.getContentNode().getAttribute("id") ?? dialog.getAttribute("id");
                        if (idPrefix) {
                            handler = document.querySelector('#' + idPrefix + "_" + routeParts[0]);
                            if (handler) {
                                route = routeParts[1];
                            }
                        }
                    }

                    if (!handler) {
                        handler = document.querySelector('#' + routeParts[0]);
                        if (handler) {
                            route = routeParts[1];
                        }
                    }

                    if (!handler)
                        return "missinghandler";
                }

                if (!handler) {
                    handler = i > 0 ? dialogs[i - 1] : document.querySelector('.route-handler');
                }

                if (route.startsWith("!"))
                    return "shebang";

                this.resolvingPreRoute = newParts.slice(0, i).join("/+/");
                try {
                    closeMessages();
                    Fluent.trigger(handler, "handleroute", <HandleRouteEvent>{
                        route: route,
                        parts: newParts,
                        index: i,
                        isInitial: this.resolveIndex <= 3
                    });
                }
                finally {
                    this.resolvingPreRoute = null;
                    return "calledhandler";
                }
            }
        }
        finally {
            this.resolving--;
        }
    }

    private hashChange(_: Event) {
        if (this.expectedHash != null) {
            const expected = this.expectedHash;
            this.expectedHash = null;
            if (normalizeHash(window.location.hash) === expected)
                return;
        }

        if (this.ignoreHashLock > 0) {
            if (new Date().getTime() > this.ignoreHashUntil) {
                this.ignoreHashLock = 0;
            }
            else {
                this.ignoreHashLock--;
                return;
            }
        }
        this.resolve();
    }

    /** @inheritdoc */
    ignoreHashChange(expiration?: number) {
        this.ignoreHashLock++;
        this.ignoreHashUntil = Math.max(this.ignoreHashUntil, new Date().getTime() + (expiration ?? 1000));
    }

    private routerOrder: number = 1;
    private boundThis: any;

    private onDocumentDialogOpen(event: any) {
        if (!this.enabled)
            return;

        const dlg = event.target as HTMLElement;
        if (!dlg || this.isIgnoredDialog(dlg))
            return;

        dlg.dataset.qrouterorder = (this.routerOrder++).toString();

        if (dlg.dataset.qroute)
            return;

        const byRoute = this.resolvingPreRoute != null || this.pendingDialogPreHash != null;
        dlg.dataset.qprhash = this.resolvingPreRoute ?? this.pendingDialogPreHash ?? window.location.hash;
        let owner = this.getVisibleOrHiddenByDialogs().filter(x => x !== dlg).pop();
        if (!owner)
            owner = document.documentElement;

        this.onDialogOpen(owner, dlg, () => "!" + (++this.autoinc).toString(36), byRoute);
    }

    private onDocumentClick(e: Event) {
        if (Fluent.isDefaultPrevented(e))
            return;

        const a = (e.target as HTMLElement).closest?.('a[href^="#"]') as HTMLAnchorElement;
        if (a) {
            this.hashAnchorClickTime = new Date().getTime();
            this.hashAnchorClickValue = a.hash.substring(1);
        }
    }

    private shouldTryBack(e: Event) {
        if (this.isIgnoredDialog(e.target as HTMLElement))
            return false;

        if ((e.target as HTMLElement)?.closest?.(".s-MessageDialog, .s-MessageModal") ||
            (e as any).key === "Escape")
            return true;

        let orgEvent = ((e as any).originalEvent ?? e) as KeyboardEvent;
        if (!orgEvent)
            return false;

        if (orgEvent.key === "Escape" ||
            (orgEvent.target as HTMLElement)?.matches?.(".close, .panel-titlebar-close, .ui-dialog-titlebar-close"))
            return true;

        return false;
    }

    private closeHandler(e: Event) {
        const dlg = e.target as HTMLElement;
        if (!dlg || this.isIgnoredDialog(dlg))
            return;
        delete dlg.dataset.qroute;
        const prhash = dlg.dataset.qprhash;

        let tryBack = this.shouldTryBack(e);

        if (prhash != null)
            this.replace(prhash, tryBack);
        else
            this.replaceLast('', tryBack);
    }

    constructor() {
        const boundThis = this.boundThis = bindThis(this);
        window.addEventListener("hashchange", boundThis.hashChange, false);

        if (typeof document !== "undefined") {

            Fluent.on(document, "dialogopen", ".ui-dialog-content", boundThis.onDocumentDialogOpen);
            Fluent.on(document, "shown.bs.modal", ".modal", boundThis.onDocumentDialogOpen);
            Fluent.on(document, "panelopen", ".panel-body", boundThis.onDocumentDialogOpen);
        }

        Fluent.on(document, "click", boundThis.onDocumentClick);

        Fluent.on(document, "dialogclose.qrouter", boundThis.closeHandler);
        Fluent.on(document, "hidden.bs.modal", boundThis.closeHandler);
        Fluent.on(document, "panelclose.qrouter", boundThis.closeHandler);
    }

    /** @inheritdoc */
    destroy() {
        const boundThis = this.boundThis;
        if (!boundThis)
            return;
        this.boundThis = undefined;
        window.removeEventListener("hashchange", boundThis.hashChange, false);

        if (typeof document !== "undefined") {
            Fluent.off(document, "dialogopen", ".ui-dialog-content", boundThis.onDocumentDialogOpen);
            Fluent.off(document, "shown.bs.modal", ".modal", boundThis.onDocumentDialogOpen);
            Fluent.off(document, "panelopen", ".panel-body", boundThis.onDocumentDialogOpen);

            Fluent.off(document, "click", boundThis.onDocumentClick);

            Fluent.off(document, "dialogclose.qrouter", boundThis.closeHandler);
            Fluent.off(document, "hidden.bs.modal", boundThis.closeHandler);
            Fluent.off(document, "panelclose.qrouter", boundThis.closeHandler);
        }
    }
}

/**
 * Singleton instance of the legacy hash router.
 * Compat shim for the global `Router` / `Serenity.Router`. Initialized at module load and wired to `hashchange` and dialog events.
 * [DEPRECATED] Prefer not to use hash-based dialog routing in new code. Kept for legacy pages that rely on `Router.resolve()` / `Router.navigate()`.
 */
export const Router = new ClassicRouter() as IClassicRouter;