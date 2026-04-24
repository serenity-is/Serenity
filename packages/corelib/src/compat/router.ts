import { bindThis } from "@serenity-is/domwise";
import { Dialog, Fluent, isArrayLike } from "../base";

export interface HandleRouteEvent extends Event {
    route: string,
    parts: string[],
    index: number,
    isInitial: boolean
}

export interface IClassicRouter {
	enabled: boolean;
	navigate(newHash: string, tryBack?: boolean, silent?: boolean): void;
	replace(newHash: string, tryBack?: boolean): void;
	replaceLast(newHash: string, tryBack?: boolean): void;
	dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string): void;
	mightBeRouteRegex: RegExp;
	resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler";
	ignoreHashChange(expiration?: number): void;
}

const ignoredSelector = '.s-MessageDialog, .s-MessageModal, .s-PromptDialog, .route-ignore';

class RouterImplementation implements IClassicRouter {
    private oldURL: string;
    private resolving: number = 0;
    private autoinc: number = 0;
    private ignoreHashLock: number = 0;
    private ignoreHashUntil: number = 0;
    private hashAnchorClickValue: string;
    private hashAnchorClickTime: number;
    enabled: boolean = true;

    private isEquivalentUrl(url1: string, url2: string) {
        return url1 == url2 || url1 == url2 + '#' || url2 == url1 + '#';
    }

    navigate(newHash: string, tryBack?: boolean, silent?: boolean) {
        if (!this.enabled || this.resolving > 0)
            return;

        newHash = newHash || '';
        newHash = newHash.replace(/^#/, '');
        newHash = (!newHash ? "" : '#' + newHash);
        const newURL = window.location.href.replace(/#$/, '')
            .replace(/#.*$/, '') + newHash;
        if (newURL != window.location.href) {
            if (tryBack && this.oldURL != null && this.isEquivalentUrl(this.oldURL, newURL)) {
                if (silent)
                    this.ignoreHashChange();

                this.oldURL = null;
                window.history.back();
                return;
            }

            if (silent)
                this.ignoreHashChange();

            this.oldURL = window.location.href;
            window.location.hash = newHash;
        }
    }

    replace(newHash: string, tryBack?: boolean) {
        this.navigate(newHash, tryBack, true);
    }

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

    private onDialogOpen(ownerEl: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string) {
        const route = [];
        element = isArrayLike(element) ? element[0] : element;
        if (element &&
            this.pendingDialogElement &&
            (element === this.pendingDialogElement) || (element.contains(this.pendingDialogElement))) {
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
        Router.replace(route.join("/+/"));
    }

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

    mightBeRouteRegex: RegExp = /^(new$|edit\/|![0-9]+$)/

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

    ignoreHashChange(expiration?: number) {
        this.ignoreHashLock++;
        this.ignoreHashUntil = Math.max(this.ignoreHashUntil, new Date().getTime() + (expiration ?? 1000));
    }

    private routerOrder: number = 1;

    private onDocumentDialogOpen(event: any) {
        if (!this.enabled)
            return;

        const dlg = event.target as HTMLElement;
        if (!dlg || this.isIgnoredDialog(dlg))
            return;

        dlg.dataset.qrouterorder = (this.routerOrder++).toString();

        if (dlg.dataset.qroute)
            return;

        dlg.dataset.qprhash = this.resolvingPreRoute ?? this.pendingDialogPreHash ?? window.location.hash;
        let owner = this.getVisibleOrHiddenByDialogs().filter(x => x !== dlg).pop();
        if (!owner)
            owner = document.documentElement;

        this.onDialogOpen(owner, dlg, () => "!" + (++this.autoinc).toString(36));
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
            Router.replace(prhash, tryBack);
        else
            Router.replaceLast('', tryBack);
    }

    constructor() {
        const boundThis = bindThis(this);
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

    destroy() {
        const boundThis = bindThis(this);
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

export const Router = new RouterImplementation() as IClassicRouter;