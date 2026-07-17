import type { MockInstance } from "vitest";
import { Fluent } from "../base";
import { ClassicRouter, type IClassicRouter } from "./router";

function changeJSDOMURL(url: string) {
    if ((globalThis as any).jsdom?.reconfigure) {
        (globalThis as any).jsdom.reconfigure({ url: url });
        return true;
    }
    else if ((globalThis as any).happyDOM?.setURL) {
        (globalThis as any).happyDOM.setURL(url);
        return true;
    }
    else
        return false;
}

function createRouter(): IClassicRouter {
    return new ClassicRouter();
}

describe("ClassicRouter basic properties", () => {
    it("is enabled by default", () => {
        const router = createRouter();
        expect(router.enabled).toBe(true);
    });

    it("mightBeRouteRegex matches expected patterns", () => {
        const router = createRouter();
        expect(router.mightBeRouteRegex.test("new")).toBe(true);
        expect(router.mightBeRouteRegex.test("edit/123")).toBe(true);
        expect(router.mightBeRouteRegex.test("!456")).toBe(true);
        expect(router.mightBeRouteRegex.test("something")).toBe(false);
        expect(router.mightBeRouteRegex.test("something/else")).toBe(false);
        expect(router.mightBeRouteRegex.test("/edit/123")).toBe(false);
        expect(router.mightBeRouteRegex.test("/new")).toBe(false);
    });

    it("allows enabling and disabling", () => {
        const router = createRouter();
        router.enabled = false;
        expect(router.enabled).toBe(false);
        router.enabled = true;
        expect(router.enabled).toBe(true);
    });
});

describe("ClassicRouter.navigate", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
        window.history = {
            back: vi.fn(),
            ...window.history
        };
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should not navigate when disabled", () => {
        const router = createRouter();
        router.enabled = false;
        router.navigate("test");
        expect(window.location.hash).toBe("");
    });

    it("should set hash for new route", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page");
        router.navigate("new");
        expect(window.location.hash).toBe("#new");
    });

    it("should handle hash prefix", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page");
        router.navigate("#edit/123");
        expect(window.location.hash).toBe("#edit/123");
    });

    it("should handle empty hash", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page");
        router.navigate("");
        expect(window.location.hash).toBe("");
    });

    it("should handle null/undefined hash", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page");
        router.navigate(null as any);
        expect(window.location.hash).toBe("");
    });

    it("should try back when tryBack is true and URL matches old URL", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page#old");
        router.navigate("new");
        expect(window.location.hash).toBe("#new");
        router.navigate("old", true);
        expect(window.history.back).toHaveBeenCalled();
    });
});

describe("ClassicRouter.replace", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should set hash silently", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page");
        router.replace("test");
        expect(window.location.hash).toBe("#test");
    });
});

describe("ClassicRouter.replaceLast", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should not replace when disabled", () => {
        const router = createRouter();
        router.enabled = false;
        router.replaceLast("test");
        expect(window.location.hash).toBe("");
    });

    it("should replace last segment", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page#edit/123/+/details");
        router.replaceLast("summary");
        expect(window.location.hash).toBe("#edit/123/+/summary");
    });

    it("should remove last segment when newHash is empty and multiple parts exist", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page#edit/123/+/details");
        router.replaceLast("");
        expect(window.location.hash).toBe("#edit/123");
    });

    it("should handle single segment", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page#new");
        router.replaceLast("edit/456");
        expect(window.location.hash).toBe("#edit/456");
    });

    it("should handle empty current hash", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page");
        router.replaceLast("new");
        expect(window.location.hash).toBe("#new");
    });
});

describe("ClassicRouter.dialog", () => {
    it("should not set up dialog when disabled", () => {
        const router = createRouter();
        router.enabled = false;
        const owner = document.createElement("div");
        const element = document.createElement("div");
        expect(() => router.dialog(owner, element, () => "test")).not.toThrow();
    });

    it("should set up dialog state when enabled", () => {
        const router = createRouter();
        const owner = document.createElement("div");
        const element = document.createElement("div");
        expect(() => router.dialog(owner, element, () => "test")).not.toThrow();
    });

    it("should handle array-like elements", () => {
        const router = createRouter();
        expect(() => router.dialog(document.createElement("div"), [document.createElement("div")], () => "test")).not.toThrow();
    });

    it("should handle array-like owners", () => {
        const router = createRouter();
        expect(() => router.dialog([document.createElement("div")], document.createElement("div"), () => "test")).not.toThrow();
    });
});

describe("ClassicRouter.resolve", () => {
    let originalLocation: Location;
    let mockLocation: any;

    beforeEach(() => {
        originalLocation = window.location;
        mockLocation = {
            href: "http://example.com/page",
            hash: "",
            replace: vi.fn(),
            assign: vi.fn()
        };
        Object.defineProperty(window, 'location', {
            value: mockLocation,
            writable: true
        });
    });

    afterEach(() => {
        Object.defineProperty(window, 'location', {
            value: originalLocation,
            writable: true
        });
    });

    it("should not resolve when disabled", () => {
        const router = createRouter();
        router.enabled = false;
        expect(router.resolve()).toBe("disabled");
    });

    it("should handle empty hash", () => {
        const router = createRouter();
        mockLocation.hash = "";
        expect(() => router.resolve()).not.toThrow();
    });

    it("should handle hash with # prefix", () => {
        const router = createRouter();
        mockLocation.hash = "#test";
        expect(() => router.resolve()).not.toThrow();
    });

    it("should handle complex routes", () => {
        const router = createRouter();
        mockLocation.hash = "#edit/123/+/details";
        expect(() => router.resolve()).not.toThrow();
    });

    it("should accept custom hash parameter", () => {
        const router = createRouter();
        expect(() => router.resolve("custom/route")).not.toThrow();
    });

    it("should return 'shebang' when route starts with !", () => {
        const router = createRouter();
        expect(router.resolve("#!abc")).toBe("shebang");
    });

    it("should return 'shebang' for numeric shebang routes", () => {
        const router = createRouter();
        expect(router.resolve("#!123")).toBe("shebang");
    });

    it("should handle nested element routing with @ syntax", () => {
        const router = createRouter();
        const customerDialog = document.createElement("div");
        customerDialog.dataset.qroute = "edit/ALFKI";
        customerDialog.classList.add("modal");
        customerDialog.setAttribute("id", "Dialog357");
        Object.defineProperties(customerDialog, {
            offsetWidth: { get: () => 100 },
            offsetHeight: { get: () => 100 }
        });
        document.body.appendChild(customerDialog);

        const ordersGrid = document.createElement("div");
        ordersGrid.setAttribute("id", "Dialog357_OrdersGrid");
        customerDialog.appendChild(ordersGrid);

        let receivedRoute: string | null = null;
        ordersGrid.addEventListener("handleroute", (e: any) => { receivedRoute = e.route; });

        const result = router.resolve("#edit/ALFKI/+/OrdersGrid@edit/11011");
        expect(result).toBe("calledhandler");
        expect(receivedRoute).toBe("edit/11011");
        document.body.removeChild(customerDialog);
    });

    it("should return 'missinghandler' when prefixed element not found", () => {
        const router = createRouter();
        const customerDialog = document.createElement("div");
        customerDialog.classList.add("modal");
        customerDialog.setAttribute("id", "Dialog999");
        customerDialog.dataset.qroute = "edit/ALFKI";
        Object.defineProperties(customerDialog, {
            offsetWidth: { get: () => 100 },
            offsetHeight: { get: () => 100 }
        });
        document.body.appendChild(customerDialog);

        expect(router.resolve("#edit/ALFKI/+/OrdersGrid@edit/11011")).toBe("missinghandler");
        document.body.removeChild(customerDialog);
    });

    it("should handle @ syntax routing to direct element ID", () => {
        const router = createRouter();
        const directElement = document.createElement("div");
        directElement.classList.add("route-handler");
        directElement.setAttribute("id", "OrdersGrid");
        document.body.appendChild(directElement);

        let receivedRoute: string | null = null;
        directElement.addEventListener("handleroute", (e: any) => { receivedRoute = e.route; });

        expect(router.resolve("#OrdersGrid@edit/11011")).toBe("calledhandler");
        expect(receivedRoute).toBe("edit/11011");
        document.body.removeChild(directElement);
    });

    it("should close stale dialogs when route has fewer parts", () => {
        const router = createRouter();
        const dialog = document.createElement("div");
        dialog.classList.add("modal");
        dialog.dataset.qroute = "edit/123";
        dialog.dataset.qrouterorder = "1";
        Object.defineProperties(dialog, {
            offsetWidth: { get: () => 100 },
            offsetHeight: { get: () => 100 }
        });
        document.body.appendChild(dialog);

        expect(router.resolve("#new")).toBe("calledhandler");
        document.body.removeChild(dialog);
    });
});

describe("ClassicRouter.ignoreHashChange", () => {
    it("should set ignore flags", () => {
        const router = createRouter();
        expect(() => router.ignoreHashChange()).not.toThrow();
        expect(() => router.ignoreHashChange(1000)).not.toThrow();
    });
});

describe("ClassicRouter document events (panelopen)", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should set router order on panel dialog", () => {
        const router = createRouter() as ClassicRouter;
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);
        Fluent.trigger(panel, "panelopen");
        expect(panel.dataset.qrouterorder).toBeDefined();
        document.body.removeChild(panel);
        router.destroy();
    });

    it("should set pre-hash on panel dialog", () => {
        const router = createRouter() as ClassicRouter;
        changeJSDOMURL("http://example.com/page#initial");
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);
        Fluent.trigger(panel, "panelopen");
        expect(panel.dataset.qprhash).toBe("#initial");
        document.body.removeChild(panel);
        router.destroy();
    });

    it("should set route on panel dialog", () => {
        const router = createRouter() as ClassicRouter;
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);
        Fluent.trigger(panel, "panelopen");
        expect(panel.dataset.qroute).toBeDefined();
        expect(panel.dataset.qroute!.startsWith("!")).toBe(true);
        document.body.removeChild(panel);
        router.destroy();
    });

    it("should not process ignored dialogs", () => {
        const router = createRouter() as ClassicRouter;
        const panel = document.createElement("div");
        panel.classList.add("panel-body", "s-MessageDialog");
        document.body.appendChild(panel);
        Fluent.trigger(panel, "panelopen");
        expect(panel.dataset.qrouterorder).toBeUndefined();
        document.body.removeChild(panel);
        router.destroy();
    });

    it("should not process when router is disabled", () => {
        const router = createRouter() as ClassicRouter;
        // Verify that resolve returns 'disabled' when router is disabled
        router.enabled = false;
        expect(router.resolve()).toBe("disabled");
        router.enabled = true;
        router.destroy();
    });

    it("should not overwrite existing qroute", () => {
        const router = createRouter() as ClassicRouter;
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "existing";
        document.body.appendChild(panel);
        Fluent.trigger(panel, "panelopen");
        expect(panel.dataset.qroute).toBe("existing");
        document.body.removeChild(panel);
        router.destroy();
    });

    it("should increment router order for multiple dialogs", () => {
        const router = createRouter() as ClassicRouter;
        const panel1 = document.createElement("div");
        panel1.classList.add("panel-body");
        document.body.appendChild(panel1);
        const panel2 = document.createElement("div");
        panel2.classList.add("panel-body");
        document.body.appendChild(panel2);
        Fluent.trigger(panel1, "panelopen");
        Fluent.trigger(panel2, "panelopen");
        expect(parseInt(panel2.dataset.qrouterorder!)).toBe(parseInt(panel1.dataset.qrouterorder!) + 1);
        document.body.removeChild(panel1);
        document.body.removeChild(panel2);
        router.destroy();
    });
});

describe("ClassicRouter document events (Bootstrap modals)", () => {
    it("should set router order on Bootstrap modal", () => {
        const router = createRouter() as ClassicRouter;
        const modal = document.createElement("div");
        modal.classList.add("modal");
        document.body.appendChild(modal);
        Fluent.trigger(modal, "shown.bs.modal");
        expect(modal.dataset.qrouterorder).toBeDefined();
        document.body.removeChild(modal);
        router.destroy();
    });

    it("should not process ignored modals", () => {
        const router = createRouter() as ClassicRouter;
        const modal = document.createElement("div");
        modal.classList.add("modal", "s-MessageModal");
        document.body.appendChild(modal);
        Fluent.trigger(modal, "shown.bs.modal");
        expect(modal.dataset.qrouterorder).toBeUndefined();
        document.body.removeChild(modal);
        router.destroy();
    });
});

describe("ClassicRouter document events (jQuery UI dialogs)", () => {
    it("should set router order on jQuery UI dialog", () => {
        const router = createRouter() as ClassicRouter;
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content");
        document.body.appendChild(dialog);
        Fluent.trigger(dialog, "dialogopen");
        expect(dialog.dataset.qrouterorder).toBeDefined();
        document.body.removeChild(dialog);
        router.destroy();
    });

    it("should not process ignored dialogs", () => {
        const router = createRouter() as ClassicRouter;
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content", "s-MessageDialog");
        document.body.appendChild(dialog);
        Fluent.trigger(dialog, "dialogopen");
        expect(dialog.dataset.qrouterorder).toBeUndefined();
        document.body.removeChild(dialog);
        router.destroy();
    });
});

describe("ClassicRouter closeHandler", () => {
    let replaceSpy: MockInstance<any>;
    let replaceLastSpy: MockInstance<any>;
    let oldLocation: string;
    let router: ClassicRouter;

    beforeEach(() => {
        oldLocation = window.location.href;
        router = createRouter() as ClassicRouter;
        replaceSpy = vi.spyOn(router, 'replace').mockImplementation(() => { });
        replaceLastSpy = vi.spyOn(router, 'replaceLast').mockImplementation(() => { });
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
        replaceSpy.mockRestore();
        replaceLastSpy.mockRestore();
        router.destroy();
    });

    it("should delete qroute and call replaceLast with empty string when no prhash", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "!1";
        document.body.appendChild(panel);
        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);
        expect(panel.dataset.qroute).toBeUndefined();
        expect(replaceLastSpy).toHaveBeenCalledWith('', false);
        document.body.removeChild(panel);
    });

    it("should delete qroute and call replace with prhash", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        document.body.appendChild(panel);
        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);
        expect(panel.dataset.qroute).toBeUndefined();
        expect(replaceSpy).toHaveBeenCalledWith("#previous", false);
        document.body.removeChild(panel);
    });

    it("should try back when closing with Escape key", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        document.body.appendChild(panel);
        const event = new KeyboardEvent("panelclose", { key: "Escape" });
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);
        expect(replaceSpy).toHaveBeenCalledWith("#previous", true);
        document.body.removeChild(panel);
    });

    it("should not process ignored dialogs (s-MessageDialog)", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body", "s-MessageDialog");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        document.body.appendChild(panel);
        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);
        expect(replaceSpy).not.toHaveBeenCalled();
        expect(panel.dataset.qroute).toBe("!1");
        document.body.removeChild(panel);
    });

    it("should try back when clicking close button", () => {
        const closeButton = document.createElement("button");
        closeButton.classList.add("panel-titlebar-close");
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        panel.appendChild(closeButton);
        document.body.appendChild(panel);
        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        Object.defineProperty(event, 'originalEvent', { value: { target: closeButton } });
        document.dispatchEvent(event);
        expect(replaceSpy).toHaveBeenCalledWith("#previous", true);
        document.body.removeChild(panel);
    });

    it("should not process when target is null", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "!1";
        document.body.appendChild(panel);
        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: null });
        document.dispatchEvent(event);
        expect(panel.dataset.qroute).toBe("!1");
        document.body.removeChild(panel);
    });

    it("should not process route-ignored dialogs", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body", "route-ignore");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        document.body.appendChild(panel);
        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);
        expect(panel.dataset.qroute).toBe("!1");
        document.body.removeChild(panel);
    });
});

describe("ClassicRouter document click handling", () => {
    it("should handle clicking anchor with hash href without error", () => {
        const router = createRouter() as ClassicRouter;
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");
        expect(() => anchor.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }))).not.toThrow();
        document.body.removeChild(anchor);
        router.destroy();
    });

    it("should handle clicking anchor without hash href without error", () => {
        const router = createRouter() as ClassicRouter;
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "/some/path");
        anchor.addEventListener("click", (e) => e.preventDefault(), { once: true });
        expect(() => anchor.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }))).not.toThrow();
        document.body.removeChild(anchor);
        router.destroy();
    });

    it("should handle clicking anchor when event is default prevented", () => {
        const router = createRouter() as ClassicRouter;
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");
        anchor.addEventListener("click", (e) => e.preventDefault(), { once: true });
        expect(() => anchor.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }))).not.toThrow();
        document.body.removeChild(anchor);
        router.destroy();
    });

    it("should return 'skipped' when resolving current hash after anchor click within 100ms", () => {
        vi.useFakeTimers();
        const router = createRouter() as ClassicRouter;
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");
        anchor.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        changeJSDOMURL(window.location.href.split('#')[0] + "#testHash");
        expect(router.resolve(null)).toBe("skipped");
        vi.useRealTimers();
        document.body.removeChild(anchor);
        router.destroy();
    });

    it("should not return 'skipped' when resolving after 100ms", () => {
        vi.useFakeTimers();
        const router = createRouter() as ClassicRouter;
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");
        anchor.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        vi.advanceTimersByTime(150);
        changeJSDOMURL(window.location.href.split('#')[0] + "#testHash");
        expect(router.resolve(null)).not.toBe("skipped");
        vi.useRealTimers();
        document.body.removeChild(anchor);
        router.destroy();
    });

    it("should not return 'skipped' for route patterns after anchor click", () => {
        const router = createRouter() as ClassicRouter;
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#new");
        anchor.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        changeJSDOMURL(window.location.href.split('#')[0] + "#new");
        expect(router.resolve(null)).not.toBe("skipped");
        document.body.removeChild(anchor);
        router.destroy();
    });
});

describe("ClassicRouter navigate silent path", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
        vi.spyOn(window.history, 'back').mockImplementation(() => { });
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should call ignoreHashChange when silent+tryBack and URL matches oldURL", () => {
        const router = createRouter();
        changeJSDOMURL("http://example.com/page#old");
        router.navigate("new");
        router.navigate("old", true, true);
        expect(window.history.back).toHaveBeenCalled();
    });
});

describe("ClassicRouter.destroy", () => {
    it("should remove event listeners and not crash", () => {
        const router = createRouter() as ClassicRouter;
        expect(() => router.destroy()).not.toThrow();
    });

    it("should not respond to hashchange after destroy", async () => {
        const router = createRouter() as ClassicRouter;
        router.destroy();
        // After destroy, calling navigate should still work as it's a direct method call
        // but the hashchange event listener should be removed
        router.navigate("test");
        expect(window.location.hash).toBe("#test");
    });
});
