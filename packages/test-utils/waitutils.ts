import { getActiveRequests } from "@serenity-is/corelib";
import { vi } from "vitest";

export function waitForAjaxRequests(timeout: number = 10000): Promise<void> {
    return waitUntil(() => typeof globalThis.jQuery !== 'undefined' ? globalThis.jQuery.active == 0 : (getActiveRequests() <= 0), timeout);
}

export function waitUntil(predicate: () => boolean, timeout: number = 10000, checkInterval: number = 10): Promise<void> {
    var start = Date.now();
    return new Promise((resolve, reject) => {
        let interval = setInterval(() => {
            if (!predicate()) {
                if (Date.now() - start > timeout) {
                    clearInterval(interval);
                    reject("Timed out while waiting for condition to be true!");
                }
                if (vi.isFakeTimers()) {
                    vi.runOnlyPendingTimers();
                }
                return;
            }
            clearInterval(interval);
            resolve(void 0);
        }, checkInterval);
        if (vi.isFakeTimers()) {
            vi.runOnlyPendingTimers();
        }
    })
}