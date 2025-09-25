import type { MockInstance } from "vitest";
import { Fluent } from "../base";
import { Router } from "./router";

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

describe("Router.enabled", () => {
    it("is true by default", () => {
        expect(Router.enabled).toBe(true);
    });
});

describe("Router.mightBeRouteRegex", () => {
    it("matches 'new' route pattern", () => {
        expect(Router.mightBeRouteRegex.test("new")).toBe(true);
    });

    it("matches 'edit/{id}' route pattern", () => {
        expect(Router.mightBeRouteRegex.test("edit/123")).toBe(true);
    });

    it("doesn't match 'something' route pattern", () => {
        expect(Router.mightBeRouteRegex.test("something")).toBe(false);
    });

    it("doesn't match 'something/else' route pattern", () => {
        expect(Router.mightBeRouteRegex.test("something/else")).toBe(false);
    });

    it("doesn't match routes that start with /", () => {
        expect(Router.mightBeRouteRegex.test("/edit/123")).toBe(false);
        expect(Router.mightBeRouteRegex.test("/new")).toBe(false);
    });
});

describe("Router.navigate", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
        // Mock window.history
        window.history = {
            back: vi.fn(),
            ...window.history
        };
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should not navigate when disabled", () => {
        Router.enabled = false;
        Router.navigate("test");
        expect(window.location.hash).toBe("");
        Router.enabled = true;
    });

    it("should set hash for new route", () => {
        changeJSDOMURL("http://example.com/page");
        Router.navigate("new");
        expect(window.location.hash).toBe("#new");
    });

    it("should handle hash prefix", () => {
        changeJSDOMURL("http://example.com/page");
        Router.navigate("#edit/123");
        expect(window.location.hash).toBe("#edit/123");
    });

    it("should handle empty hash", () => {
        changeJSDOMURL("http://example.com/page");
        Router.navigate("");
        expect(window.location.hash).toBe("");
    });

    it("should handle null/undefined hash", () => {
        changeJSDOMURL("http://example.com/page");
        Router.navigate(null as any);
        expect(window.location.hash).toBe("");
    });

    it("should try back when tryBack is true and URL matches old URL", () => {
        // Set up initial state
        changeJSDOMURL("http://example.com/page#old");
        expect(window.location.hash).toBe("#old");

        // Navigate away
        Router.navigate("new");
        expect(window.location.hash).toBe("#new");

        // Try to navigate back to old
        Router.navigate("old", true);
        expect(window.history.back).toHaveBeenCalled();
    });
});

describe("Router.replace", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should set hash silently", () => {
        changeJSDOMURL("http://example.com/page");
        Router.replace("test");
        expect(window.location.hash).toBe("#test");
    });
});

describe("Router.replaceLast", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should not replace when disabled", () => {
        Router.enabled = false;
        Router.replaceLast("test");
        expect(window.location.hash).toBe("");
        Router.enabled = true;
    });

    it("should replace last segment", () => {
        changeJSDOMURL("http://example.com/page#edit/123/+/details");
        Router.replaceLast("summary");
        expect(window.location.hash).toBe("#edit/123/+/summary");
    });

    it("should remove last segment when newHash is empty", () => {
        changeJSDOMURL("http://example.com/page#edit/123/+/details");
        Router.replaceLast("");
        expect(window.location.hash).toBe("#edit/123");
    });

    it("should handle single segment", () => {
        changeJSDOMURL("http://example.com/page#new");
        Router.replaceLast("edit/456");
        expect(window.location.hash).toBe("#edit/456");
    });
});

describe("Router.dialog", () => {
    it("should not set up dialog when disabled", () => {
        Router.enabled = false;
        const owner = document.createElement("div");
        const element = document.createElement("div");
        const dialogHash = () => "test";

        Router.dialog(owner, element, dialogHash);
        // Since internal state is not accessible, we just verify no errors
        Router.enabled = true;
    });

    it("should set up dialog state when enabled", () => {
        const owner = document.createElement("div");
        const element = document.createElement("div");
        const dialogHash = () => "test";

        expect(() => Router.dialog(owner, element, dialogHash)).not.toThrow();
    });

    it("should handle array-like elements", () => {
        const owner = document.createElement("div");
        const element = document.createElement("div");
        const elements = [element];
        const dialogHash = () => "test";

        expect(() => Router.dialog(owner, elements, dialogHash)).not.toThrow();
    });

    it("should handle array-like owners", () => {
        const owner = document.createElement("div");
        const owners = [owner];
        const element = document.createElement("div");
        const dialogHash = () => "test";

        expect(() => Router.dialog(owners, element, dialogHash)).not.toThrow();
    });
});

describe("Router.resolve", () => {
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
        Router.enabled = false;
        expect(() => Router.resolve()).not.toThrow();
        Router.enabled = true;
    });

    it("should handle empty hash", () => {
        mockLocation.hash = "";
        expect(() => Router.resolve()).not.toThrow();
    });

    it("should handle hash with # prefix", () => {
        mockLocation.hash = "#test";
        expect(() => Router.resolve()).not.toThrow();
    });

    it("should handle complex routes", () => {
        mockLocation.hash = "#edit/123/+/details";
        expect(() => Router.resolve()).not.toThrow();
    });

    it("should accept custom hash parameter", () => {
        expect(() => Router.resolve("custom/route")).not.toThrow();
    });

    it("should handle nested element routing with @ syntax", () => {
        // Set up a customer dialog
        const customerDialog = document.createElement("div");
        customerDialog.dataset.qroute = "edit/ALFKI";
        customerDialog.classList.add("modal");
        customerDialog.setAttribute("id", "Dialog357");
        // Make it "visible" for the test by setting offset dimensions
        Object.defineProperties(customerDialog, {
            offsetWidth: { get: () => 100 },
            offsetHeight: { get: () => 100 }
        });
        document.body.appendChild(customerDialog);

        // Set up an orders grid element within the customer dialog
        // Don't give it route-handler class so it doesn't catch the first route
        const ordersGrid = document.createElement("div");
        ordersGrid.setAttribute("id", "Dialog357_OrdersGrid");
        customerDialog.appendChild(ordersGrid);

        let receivedRoute: string | null = null;
        let receivedIndex: number | null = null;

        const routeHandler = (e: any) => {
            receivedRoute = e.route;
            receivedIndex = e.index;
        };
        ordersGrid.addEventListener("handleroute", routeHandler);

        try {
            // Check if elements exist
            expect(document.getElementById("Dialog357")).toBe(customerDialog);
            expect(document.getElementById("Dialog357_OrdersGrid")).toBe(ordersGrid);
            
            const result = Router.resolve("#edit/ALFKI/+/OrdersGrid@edit/11011");

            // Should return "calledhandler" and trigger the event on the orders grid
            expect(result).toBe("calledhandler");
            expect(receivedRoute).toBe("edit/11011");
            expect(receivedIndex).toBe(1);
        } finally {
            // Clean up
            document.body.removeChild(customerDialog);
        }
    });

    it("should handle nested element routing when dialog ID is not found", () => {
        // Set up a customer dialog with wrong ID
        const customerDialog = document.createElement("div");
        customerDialog.classList.add("modal");
        customerDialog.setAttribute("id", "Dialog999"); // Wrong ID
        customerDialog.dataset.qroute = "edit/ALFKI";
        customerDialog.classList.add("modal");

        Object.defineProperties(customerDialog, {
            offsetWidth: { get: () => 100 },
            offsetHeight: { get: () => 100 }
        });
        document.body.appendChild(customerDialog);

        // Set up an orders grid element that won't be found
        const ordersGrid = document.createElement("div");
        ordersGrid.setAttribute("id", "Dialog357_OrdersGrid"); // Looking for Dialog357 prefix
        customerDialog.appendChild(ordersGrid);

        let eventTriggered = false;
        const routeHandler = () => {
            eventTriggered = true;
        };
        ordersGrid.addEventListener("handleroute", routeHandler);

        try {
            // Resolve hash - should not find the prefixed element
            const result = Router.resolve("#edit/ALFKI/+/OrdersGrid@edit/11011");

            // Should return "missinghandler" because the prefixed ID element is not found
            expect(result).toBe("missinghandler");
            expect(eventTriggered).toBe(false);
        } finally {
            // Clean up
            document.body.removeChild(customerDialog);
        }
    });

    it("should handle @ syntax routing to direct element ID", () => {
        // Set up an element with direct ID (not prefixed)
        const directElement = document.createElement("div");
        directElement.classList.add("route-handler");
        directElement.setAttribute("id", "OrdersGrid");
        document.body.appendChild(directElement);

        let receivedRoute: string | null = null;
        const routeHandler = (e: any) => {
            receivedRoute = e.route;
        };
        directElement.addEventListener("handleroute", routeHandler);

        // Resolve hash with @ syntax but no prefix needed
        const result = Router.resolve("#OrdersGrid@edit/11011");

        // Should find the direct element and trigger the event
        expect(result).toBe("calledhandler");
        expect(receivedRoute).toBe("edit/11011");

        // Clean up
        document.body.removeChild(directElement);
    });
});

describe("Router.ignoreHashChange", () => {
    it("should set ignore flags", () => {
        expect(() => Router.ignoreHashChange()).not.toThrow();
        expect(() => Router.ignoreHashChange(1000)).not.toThrow();
    });
});

describe("Router.enabled functionality", () => {
    it("should allow enabling and disabling", () => {
        Router.enabled = false;
        expect(Router.enabled).toBe(false);
        Router.enabled = true;
        expect(Router.enabled).toBe(true);
    });

    it("should prevent navigation when disabled", () => {
        const originalLocation = window.location;
        const mockLocation = {
            href: "http://example.com/page",
            hash: "",
            replace: vi.fn(),
            assign: vi.fn()
        };
        Object.defineProperty(mockLocation, 'hash', {
            get() { return this._hash || ""; },
            set(value) {
                this._hash = value;
                this.href = "http://example.com/page" + value;
            }
        });
        Object.defineProperty(window, 'location', {
            value: mockLocation,
            writable: true
        });

        Router.enabled = false;
        Router.navigate("test");
        expect(mockLocation.hash).toBe("");

        Router.enabled = true;
        Router.navigate("test");
        expect(mockLocation.hash).toBe("#test");

        Object.defineProperty(window, 'location', {
            value: originalLocation,
            writable: true
        });
    });
});

describe("onDocumentDialogOpen (panel dialogs)", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should set router order on panel dialog", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);

        Fluent.trigger(panel, "panelopen");

        expect(panel.dataset.qrouterorder).toBeDefined();
        expect(parseInt(panel.dataset.qrouterorder)).toBeGreaterThanOrEqual(0);
    });

    it("should set pre-hash on panel dialog", () => {
        changeJSDOMURL("http://example.com/page#initial");
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);

        Fluent.trigger(panel, "panelopen");

        expect(panel.dataset.qprhash).toBe("#initial");
    });

    it("should set route on panel dialog", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);

        Fluent.trigger(panel, "panelopen");

        expect(panel.dataset.qroute).toBeDefined();
        expect(panel.dataset.qroute.startsWith("!")).toBe(true);
    });

    it("should not process ignored dialogs", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body", "s-MessageDialog");
        document.body.appendChild(panel);

        Fluent.trigger(panel, "panelopen");

        expect(panel.dataset.qrouterorder).toBeUndefined();
        expect(panel.dataset.qroute).toBeUndefined();
    });

    it("should not process when router is disabled", () => {
        Router.enabled = false;
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        document.body.appendChild(panel);

        Fluent.trigger(panel, "panelopen");

        expect(panel.dataset.qrouterorder).toBeUndefined();
        expect(panel.dataset.qroute).toBeUndefined();
        Router.enabled = true;
    });

    it("should not set route if already has one", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "existing";
        document.body.appendChild(panel);

        Fluent.trigger(panel, "panelopen");

        expect(panel.dataset.qroute).toBe("existing");
    });

    it("should increment router order for multiple dialogs", () => {
        const panel1 = document.createElement("div");
        panel1.classList.add("panel-body");
        document.body.appendChild(panel1);

        const panel2 = document.createElement("div");
        panel2.classList.add("panel-body");
        document.body.appendChild(panel2);

        Fluent.trigger(panel1, "panelopen");
        Fluent.trigger(panel2, "panelopen");

        const order1 = parseInt(panel1.dataset.qrouterorder);
        const order2 = parseInt(panel2.dataset.qrouterorder);

        expect(order2).toBe(order1 + 1);
    });
});

describe("onDocumentDialogOpen (Bootstrap modals)", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should set router order on Bootstrap modal", () => {
        const modal = document.createElement("div");
        modal.classList.add("modal");
        document.body.appendChild(modal);

        Fluent.trigger(modal, "shown.bs.modal");

        expect(modal.dataset.qrouterorder).toBeDefined();
        expect(parseInt(modal.dataset.qrouterorder)).toBeGreaterThanOrEqual(0);
    });

    it("should set pre-hash on Bootstrap modal", () => {
        changeJSDOMURL("http://example.com/page#initial");
        const modal = document.createElement("div");
        modal.classList.add("modal");
        document.body.appendChild(modal);

        Fluent.trigger(modal, "shown.bs.modal");

        expect(modal.dataset.qprhash).toBe("#initial");
    });

    it("should set route on Bootstrap modal", () => {
        const modal = document.createElement("div");
        modal.classList.add("modal");
        document.body.appendChild(modal);

        Fluent.trigger(modal, "shown.bs.modal");

        expect(modal.dataset.qroute).toBeDefined();
        expect(modal.dataset.qroute.startsWith("!")).toBe(true);
    });

    it("should not process ignored modals", () => {
        const modal = document.createElement("div");
        modal.classList.add("modal", "s-MessageModal");
        document.body.appendChild(modal);

        Fluent.trigger(modal, "shown.bs.modal");

        expect(modal.dataset.qrouterorder).toBeUndefined();
        expect(modal.dataset.qroute).toBeUndefined();
    });

    it("should not process when router is disabled", () => {
        Router.enabled = false;
        const modal = document.createElement("div");
        modal.classList.add("modal");
        document.body.appendChild(modal);

        Fluent.trigger(modal, "shown.bs.modal");

        expect(modal.dataset.qrouterorder).toBeUndefined();
        expect(modal.dataset.qroute).toBeUndefined();
        Router.enabled = true;
    });

    it("should not set route if already has one", () => {
        const modal = document.createElement("div");
        modal.classList.add("modal");
        modal.dataset.qroute = "existing";
        document.body.appendChild(modal);

        Fluent.trigger(modal, "shown.bs.modal");

        expect(modal.dataset.qroute).toBe("existing");
    });

    it("should increment router order for multiple modals", () => {
        const modal1 = document.createElement("div");
        modal1.classList.add("modal");
        document.body.appendChild(modal1);

        const modal2 = document.createElement("div");
        modal2.classList.add("modal");
        document.body.appendChild(modal2);

        Fluent.trigger(modal1, "shown.bs.modal");
        Fluent.trigger(modal2, "shown.bs.modal");

        const order1 = parseInt(modal1.dataset.qrouterorder);
        const order2 = parseInt(modal2.dataset.qrouterorder);

        expect(order2).toBe(order1 + 1);
    });
});

describe("onDocumentDialogOpen (jQuery UI dialogs)", () => {
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
    });

    it("should set router order on jQuery UI dialog", () => {
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content");
        document.body.appendChild(dialog);

        Fluent.trigger(dialog, "dialogopen");

        expect(dialog.dataset.qrouterorder).toBeDefined();
        expect(parseInt(dialog.dataset.qrouterorder)).toBeGreaterThanOrEqual(0);
    });

    it("should set pre-hash on jQuery UI dialog", () => {
        changeJSDOMURL("http://example.com/page#initial");
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content");
        document.body.appendChild(dialog);

        Fluent.trigger(dialog, "dialogopen");

        expect(dialog.dataset.qprhash).toBe("#initial");
    });

    it("should set route on jQuery UI dialog", () => {
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content");
        document.body.appendChild(dialog);

        Fluent.trigger(dialog, "dialogopen");

        expect(dialog.dataset.qroute).toBeDefined();
        expect(dialog.dataset.qroute.startsWith("!")).toBe(true);
    });

    it("should not process ignored dialogs", () => {
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content", "s-MessageDialog");
        document.body.appendChild(dialog);

        Fluent.trigger(dialog, "dialogopen");

        expect(dialog.dataset.qrouterorder).toBeUndefined();
        expect(dialog.dataset.qroute).toBeUndefined();
    });

    it("should not process when router is disabled", () => {
        Router.enabled = false;
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content");
        document.body.appendChild(dialog);

        Fluent.trigger(dialog, "dialogopen");

        expect(dialog.dataset.qrouterorder).toBeUndefined();
        expect(dialog.dataset.qroute).toBeUndefined();
        Router.enabled = true;
    });

    it("should not set route if already has one", () => {
        const dialog = document.createElement("div");
        dialog.classList.add("ui-dialog-content");
        dialog.dataset.qroute = "existing";
        document.body.appendChild(dialog);

        Fluent.trigger(dialog, "dialogopen");

        expect(dialog.dataset.qroute).toBe("existing");
    });

    it("should increment router order for multiple dialogs", () => {
        const dialog1 = document.createElement("div");
        dialog1.classList.add("ui-dialog-content");
        document.body.appendChild(dialog1);

        const dialog2 = document.createElement("div");
        dialog2.classList.add("ui-dialog-content");
        document.body.appendChild(dialog2);

        Fluent.trigger(dialog1, "dialogopen");
        Fluent.trigger(dialog2, "dialogopen");

        const order1 = parseInt(dialog1.dataset.qrouterorder);
        const order2 = parseInt(dialog2.dataset.qrouterorder);

        expect(order2).toBe(order1 + 1);
    });
});

describe("closeHandler (panel dialogs)", () => {
    let replaceSpy: MockInstance<any>;
    let replaceLastSpy: MockInstance<any>;
    let oldLocation: string;

    beforeEach(() => {
        oldLocation = window.location.href;
        // Spy on Router.replace and Router.replaceLast
        replaceSpy = vi.spyOn(Router, 'replace').mockImplementation(() => { });
        replaceLastSpy = vi.spyOn(Router, 'replaceLast').mockImplementation(() => { });
    });

    afterEach(() => {
        changeJSDOMURL(oldLocation);
        replaceSpy.mockRestore();
        replaceLastSpy.mockRestore();
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
        expect(replaceSpy).not.toHaveBeenCalled();
        expect(replaceLastSpy).toHaveBeenCalledWith('', false);
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
        expect(replaceLastSpy).not.toHaveBeenCalled();
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
        expect(replaceLastSpy).not.toHaveBeenCalled();
    });

    it("should try back when closing message dialog", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body", "s-MessageDialog");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        document.body.appendChild(panel);

        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);

        // Message dialogs are ignored, so closeHandler returns early
        expect(replaceSpy).not.toHaveBeenCalled();
        expect(replaceLastSpy).not.toHaveBeenCalled();
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
        // Set originalEvent to simulate the click on the close button
        Object.defineProperty(event, 'originalEvent', {
            value: { target: closeButton },
            writable: true
        });
        document.dispatchEvent(event);

        expect(replaceSpy).toHaveBeenCalledWith("#previous", true);
        expect(replaceLastSpy).not.toHaveBeenCalled();
    });

    it("should not process ignored dialogs", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body", "route-ignore");
        panel.dataset.qroute = "!1";
        panel.dataset.qprhash = "#previous";
        document.body.appendChild(panel);

        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: panel });
        document.dispatchEvent(event);

        expect(panel.dataset.qroute).toBe("!1"); // Should not be deleted
        expect(replaceSpy).not.toHaveBeenCalled();
        expect(replaceLastSpy).not.toHaveBeenCalled();
    });

    it("should not process when target is null", () => {
        const panel = document.createElement("div");
        panel.classList.add("panel-body");
        panel.dataset.qroute = "!1";
        document.body.appendChild(panel);

        const event = new Event("panelclose");
        Object.defineProperty(event, 'target', { value: null });
        document.dispatchEvent(event);

        expect(panel.dataset.qroute).toBe("!1"); // Should not be deleted
        expect(replaceSpy).not.toHaveBeenCalled();
        expect(replaceLastSpy).not.toHaveBeenCalled();
    })
});

describe("Router document click handling", () => {
    it("should return 'skipped' when resolving current hash after anchor click within 100ms", () => {
        // Use fake timers
        vi.useFakeTimers();

        // Click an anchor with hash href
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");

        const clickEvent = new MouseEvent("click", {
            bubbles: true,
            cancelable: true
        });
        anchor.dispatchEvent(clickEvent);

        // Set the current hash to match what was clicked
        changeJSDOMURL(window.location.href.split('#')[0] + "#testHash");

        // Resolve current hash (resolvingCurrent = true)
        const result = Router.resolve(null);

        // Should return "skipped" because hash anchor was clicked recently
        expect(result).toBe("skipped");

        // Restore real timers
        vi.useRealTimers();

        // Clean up
        document.body.removeChild(anchor);
    });

    it("should not return 'skipped' when resolving current hash after anchor click after 100ms", () => {
        // Use fake timers from the start
        vi.useFakeTimers();

        // Click an anchor with hash href
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");

        const clickEvent = new MouseEvent("click", {
            bubbles: true,
            cancelable: true
        });
        anchor.dispatchEvent(clickEvent);

        // Advance time by more than 100ms
        vi.advanceTimersByTime(150);

        // Set the current hash to match what was clicked
        changeJSDOMURL(window.location.href.split('#')[0] + "#testHash");

        // Resolve current hash
        const result = Router.resolve(null);

        // Should not return "skipped" because 100ms window passed
        expect(result).not.toBe("skipped");

        // Restore real timers
        vi.useRealTimers();

        // Clean up
        document.body.removeChild(anchor);
    });

    it("should not return 'skipped' when resolving current hash for route patterns after anchor click", () => {
        // Click an anchor with hash href that matches mightBeRouteRegex
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#new");

        const clickEvent = new MouseEvent("click", {
            bubbles: true,
            cancelable: true
        });
        anchor.dispatchEvent(clickEvent);

        // Set the current hash to match what was clicked
        changeJSDOMURL(window.location.href.split('#')[0] + "#new");

        // Resolve current hash
        const result = Router.resolve(null);

        // Should not return "skipped" because "new" matches mightBeRouteRegex
        expect(result).not.toBe("skipped");

        // Clean up
        document.body.removeChild(anchor);
    });

    it("should handle clicking anchor with hash href without error", () => {
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");

        // Click the anchor
        const clickEvent = new MouseEvent("click", {
            bubbles: true,
            cancelable: true
        });
        expect(() => anchor.dispatchEvent(clickEvent)).not.toThrow();

        // Clean up
        document.body.removeChild(anchor);
    });

    it("should handle clicking anchor without hash href without error", () => {
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "/some/path");

        // Click the anchor
        const clickEvent = new MouseEvent("click", {
            bubbles: true,
            cancelable: true
        });
        expect(() => anchor.dispatchEvent(clickEvent)).not.toThrow();

        // Clean up
        document.body.removeChild(anchor);
    });

    it("should handle clicking anchor when event is default prevented", () => {
        const anchor = document.createElement("a");
        document.body.appendChild(anchor);
        anchor.setAttribute("href", "#testHash");

        // Click the anchor but prevent default
        const clickEvent = new MouseEvent("click", {
            bubbles: true,
            cancelable: true
        });
        anchor.addEventListener("click", (e) => e.preventDefault(), { once: true });
        expect(() => anchor.dispatchEvent(clickEvent)).not.toThrow();

        // Clean up
        document.body.removeChild(anchor);
    });
});