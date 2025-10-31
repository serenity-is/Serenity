import { Dialog, Fluent, isArrayLike } from "../base";

export interface HandleRouteEvent extends Event {
    route: string,
    parts: string[],
    index: number,
    isInitial: boolean
}

export namespace Router {
    let oldURL: string;
    let resolving: number = 0;
    let autoinc: number = 0;
    let ignoreHashLock: number = 0;
    let ignoreHashUntil: number = 0;
    let hashAnchorClickValue: string;
    let hashAnchorClickTime: number;

    export let enabled: boolean = true;

    function isEquivalentUrl(url1: string, url2: string) {
        return url1 == url2 || url1 == url2 + '#' || url2 == url1 + '#';
    }

    export function navigate(newHash: string, tryBack?: boolean, silent?: boolean) {
        if (!enabled || resolving > 0)
            return;

        newHash = newHash || '';
        newHash = newHash.replace(/^#/, '');
        newHash = (!newHash ? "" : '#' + newHash);
        const newURL = window.location.href.replace(/#$/, '')
            .replace(/#.*$/, '') + newHash;
        if (newURL != window.location.href) {
            if (tryBack && oldURL != null && isEquivalentUrl(oldURL, newURL)) {
                if (silent)
                    ignoreHashChange();

                oldURL = null;
                window.history.back();
                return;
            }

            if (silent)
                ignoreHashChange();

            oldURL = window.location.href;
            window.location.hash = newHash;
        }
    }

    export function replace(newHash: string, tryBack?: boolean) {
        navigate(newHash, tryBack, true);
    }

    export function replaceLast(newHash: string, tryBack?: boolean) {
        if (!enabled)
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
        Router.replace(newHash, tryBack);
    }

    const ignoredSelector = '.s-MessageDialog, .s-MessageModal, .s-PromptDialog, .route-ignore';

    function isIgnoredDialog(el: HTMLElement) {
        return !!(el?.closest(ignoredSelector) || Dialog.getInstance(el)?.getContentNode()?.closest(ignoredSelector));
    }

    function isVisibleOrHiddenBy(el: HTMLElement): boolean {
        return (el.offsetWidth > 0 && el.offsetHeight > 0) ||  // if visible
            !!(!el.closest(".hidden, [hidden]") && el.closest("[data-hiddenby]")) // or temporarily hidden by another panel
    }

    function getVisibleOrHiddenByDialogs(): HTMLElement[] {
        const visibleDialogs = Array.from(document.querySelectorAll<HTMLElement>(".modal, .panel-body, .ui-dialog-content"))
            .filter(isVisibleOrHiddenBy)
            .filter(x => !isIgnoredDialog(x));
        visibleDialogs.sort((a: any, b: any) => {
            return parseInt(a.dataset.qrouterorder || "0", 10) - parseInt(b.dataset.qrouterorder || "0", 10);
        });
        return visibleDialogs;
    }

    let pendingDialogHash: () => string;
    let pendingDialogElement: HTMLElement;
    let pendingDialogOwner: HTMLElement;
    let pendingDialogPreHash: string;

    function onDialogOpen(ownerEl: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string) {
        const route = [];
        element = isArrayLike(element) ? element[0] : element;
        if (element &&
            pendingDialogElement &&
            (element === pendingDialogElement) || (element.contains(pendingDialogElement))) {
            dialogHash = pendingDialogHash ?? dialogHash;
            ownerEl = pendingDialogOwner;
        }

        pendingDialogHash = null;
        pendingDialogElement = null;
        pendingDialogOwner = null;
        pendingDialogPreHash = null;

        ownerEl = isArrayLike(ownerEl) ? ownerEl[0] : ownerEl;
        const ownerIsDialog = ownerEl?.matches(".ui-dialog-content, .panel-body, .modal-content");
        const ownerDlgInst = Dialog.getInstance(ownerEl);
        let value = dialogHash();

        let idPrefix: string;
        if (ownerDlgInst) {
            const dialogs = getVisibleOrHiddenByDialogs();
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
        Router.replace(route.join("/+/"));
    }

    export function dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string) {
        if (!enabled)
            return;

        const el = isArrayLike(element) ? element[0] : element;
        pendingDialogElement = el;
        pendingDialogHash = dialogHash;
        pendingDialogOwner = isArrayLike(owner) ? owner[0] : owner;
        pendingDialogPreHash = resolvingPreRoute;
    }

    let resolvingPreRoute: string;
    let resolveIndex = 0;

    export let mightBeRouteRegex: RegExp = /^(new$|edit\/|![0-9]+$)/

    export function resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler" {
        resolveIndex++;

        if (!enabled) {
            return "disabled";
        }

        const resolvingCurrent = newHash == null;
        newHash = newHash ?? window.location.hash ?? '';
        if (newHash.charAt(0) == '#')
            newHash = newHash.substring(1);
        const newParts = newHash.split("/+/");

        if (resolvingCurrent &&
            (hashAnchorClickTime && new Date().getTime() - hashAnchorClickTime < 100) &&
            hashAnchorClickValue === newHash &&
            (newHash != '' || window.location.href.indexOf('#') >= 0) &&
            newParts.length == 1 &&
            !newParts.some(x => mightBeRouteRegex.test(x))) {
            return "skipped";
        }

        resolving++;
        try {
            const dialogs = getVisibleOrHiddenByDialogs();
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

                resolvingPreRoute = newParts.slice(0, i).join("/+/");
                try {
                    closeMessages();
                    Fluent.trigger(handler, "handleroute", <HandleRouteEvent>{
                        route: route,
                        parts: newParts,
                        index: i,
                        isInitial: resolveIndex <= 3
                    });
                }
                finally {
                    resolvingPreRoute = null;
                    return "calledhandler";
                }
            }
        }
        finally {
            resolving--;
        }
    }

    function hashChange(_: Event) {
        if (ignoreHashLock > 0) {
            if (new Date().getTime() > ignoreHashUntil) {
                ignoreHashLock = 0;
            }
            else {
                ignoreHashLock--;
                return;
            }
        }
        resolve();
    }

    export function ignoreHashChange(expiration?: number) {
        ignoreHashLock++;
        ignoreHashUntil = Math.max(ignoreHashUntil, new Date().getTime() + (expiration ?? 1000));
    }

    window.addEventListener("hashchange", hashChange, false);

    let routerOrder = 1;

    if (typeof document !== "undefined") {
        function onDocumentDialogOpen(event: any) {
            if (!enabled)
                return;

            const dlg = event.target as HTMLElement;
            if (!dlg || isIgnoredDialog(dlg))
                return;

            dlg.dataset.qrouterorder = (routerOrder++).toString();

            if (dlg.dataset.qroute)
                return;

            dlg.dataset.qprhash = resolvingPreRoute ?? pendingDialogPreHash ?? window.location.hash;
            let owner = getVisibleOrHiddenByDialogs().filter(x => x !== dlg).pop();
            if (!owner)
                owner = document.documentElement;

            onDialogOpen(owner, dlg, () => {
                return "!" + (++autoinc).toString(36);
            });
        }

        Fluent.on(document, "dialogopen", ".ui-dialog-content", onDocumentDialogOpen);
        Fluent.on(document, "shown.bs.modal", ".modal", onDocumentDialogOpen);
        Fluent.on(document, "panelopen", ".panel-body", onDocumentDialogOpen);

        Fluent.on(document, "click", e => {
            if (!Fluent.isDefaultPrevented(e)) {
                const a = (e.target as HTMLElement).closest?.('a[href^="#"]') as HTMLAnchorElement;
                if (a) {
                    hashAnchorClickTime = new Date().getTime();
                    hashAnchorClickValue = a.hash.substring(1);
                }
            }
        });

        function shouldTryBack(e: Event) {
            if (isIgnoredDialog(e.target as HTMLElement))
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

        function closeHandler(e: any) {
            const dlg = e.target as HTMLElement;
            if (!dlg || isIgnoredDialog(e.target))
                return;
            delete dlg.dataset.qroute;
            const prhash = dlg.dataset.qprhash;

            let tryBack = shouldTryBack(e);

            if (prhash != null)
                Router.replace(prhash, tryBack);
            else
                Router.replaceLast('', tryBack);
        }

        Fluent.on(document, "dialogclose.qrouter", closeHandler);
        Fluent.on(document, "hidden.bs.modal", closeHandler);
        Fluent.on(document, "panelclose.qrouter", closeHandler);
    }
}