import * as corelib from "@serenity-is/corelib";
import { vi } from "vitest";

/**
 * Where the page would have gone, instead of navigating it.
 *
 * jsdom does not implement navigation, so an address the code sets would
 * otherwise report "Not implemented: navigation to another Document" and leave
 * the document untouched, which makes a test that reaches such a line check a
 * page that never changed.
 */
export interface MockLocation extends Location {
    /**
     * The addresses the page would have gone to, in order, whatever was used to
     * go there: assigning href, or assign / replace.
     */
    readonly visited: string[];
    /**
     * Forgets what the page did during the previous test: the addresses it would
     * have gone to, and the addresses it would have opened in a new tab.
     */
    reset(): void;
}

const visited: string[] = [];

const visit = (url: string) => {
    visited.push(url);
    // a write also replaces the address through the history, so that a later
    // read of href/search/pathname sees it, as it would after a real navigation
    try {
        const resolved = new URL(url, window.location.href).href;
        if (resolved !== window.location.href)
            window.history.replaceState(null, "", resolved);
    }
    catch {
        // an address that cannot be parsed is only recorded
    }
};

const location: MockLocation = {
    get href() { return window.location.href; },
    set href(url: string) { visit(url); },
    get search() { return window.location.search; },
    get pathname() { return window.location.pathname; },
    get hash() { return window.location.hash; },
    get origin() { return window.location.origin; },
    get host() { return window.location.host; },
    get hostname() { return window.location.hostname; },
    get port() { return window.location.port; },
    get protocol() { return window.location.protocol; },
    get ancestorOrigins() { return window.location.ancestorOrigins; },
    assign: (url: string) => visit(url),
    replace: (url: string) => visit(url),
    reload: () => { },
    toString: () => window.location.toString(),
    get visited() { return visited; },
    reset() {
        visited.length = 0;
        if (vi.isMockFunction(corelib.windowOpen))
            vi.mocked(corelib.windowOpen).mockClear();
    }
};

/**
 * Makes `getWindowLocation()` return a location that records instead of
 * navigating, so a test can check where the page would go.
 *
 * Call it from a `beforeEach`, as a spec that restores its mocks would otherwise
 * drop it after the first test. Reads still come from the real location, so a
 * test that prepares the address with `history.replaceState` keeps working.
 *
 * A package's setup file installs it once, so its specs import the result, and
 * calls it again before every test, as a spec that restores its mocks would
 * otherwise drop it after the first test:
 *
 * ```ts
 * export const testLocation = mockLocation();
 * beforeEach(() => mockLocation().reset());
 * ```
 *
 * Installing keeps what was recorded, so calling it again does not lose the
 * addresses a test has already checked; `reset()` is what forgets them.
 * @returns The location, which is the same object on every call.
 */
export function mockLocation(): MockLocation {
    // like mockFetch, installing twice must not stack another mock
    if (!vi.isMockFunction(corelib.getWindowLocation))
        vi.spyOn(corelib, "getWindowLocation").mockImplementation(() => location);

    // a new tab is the other way out of a page; jsdom does not implement it
    // either. a call is a call, so a test checks the spy instead of a recorder
    if (!vi.isMockFunction(corelib.windowOpen))
        vi.spyOn(corelib, "windowOpen").mockImplementation(() => null);

    return location;
}
