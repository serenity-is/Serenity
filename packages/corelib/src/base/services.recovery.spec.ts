import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { mockFetch, unmockFetch } from "../test/mocks";
import { ErrorHandling } from "./errorhandling";
import { addServiceErrorRecovery, serviceCall } from "./services";

describe("addServiceErrorRecovery", () => {
    let removers: (() => void)[];

    beforeEach(() => {
        removers = [];
        vi.spyOn(ErrorHandling, "showServiceError").mockImplementation(() => { });
    });

    afterEach(() => {
        removers.forEach(x => x());
        unmockFetch();
        vi.restoreAllMocks();
    });

    /** A service that fails with the code until it is allowed, as a service that needs the password does. */
    function mockService(code = "RequiresElevation") {
        const state = { allowed: false, calls: 0 };
        mockFetch({
            "/test": () => {
                state.calls++;
                return state.allowed ? { Value: 5 } : { Error: { Code: code, Message: "needs it" } };
            }
        });
        return state;
    }

    it("calls the service again after a recovery that fixes the error, once", async () => {
        const state = mockService();
        const recovery = vi.fn((error: any) => error.Code === "RequiresElevation" ?
            Promise.resolve().then(() => { state.allowed = true; return true; }) : undefined);
        removers.push(addServiceErrorRecovery(recovery));
        const onSuccess = vi.fn();
        const onCleanup = vi.fn();

        const response = await serviceCall<any>({ url: "/test", onSuccess, onCleanup });

        expect(response.Value).toBe(5);
        expect(state.calls).toBe(2);
        expect(recovery).toHaveBeenCalledTimes(1);
        expect(onSuccess).toHaveBeenCalledTimes(1);
        expect(onCleanup).toHaveBeenCalledTimes(1);
        expect(ErrorHandling.showServiceError).not.toHaveBeenCalled();
    });

    it("recovers an error that comes with an HTTP error status, as a validation error does", async () => {
        let allowed = false;
        let calls = 0;
        mockFetch({
            "/test": info => {
                calls++;
                if (allowed)
                    return { Value: 7 };
                info.status = 400;
                info.statusText = "Bad Request";
                return { Error: { Code: "RequiresElevation", Message: "needs it" } };
            }
        });
        removers.push(addServiceErrorRecovery(error => error.Code === "RequiresElevation" ?
            Promise.resolve().then(() => { allowed = true; return true; }) : undefined));

        const response = await serviceCall<any>({ url: "/test" });

        expect(response.Value).toBe(7);
        expect(calls).toBe(2);
        expect(ErrorHandling.showServiceError).not.toHaveBeenCalled();
    });

    it("does not recover the call that is made again, which shows its error", async () => {
        const state = mockService();
        removers.push(addServiceErrorRecovery(() => Promise.resolve(true)));

        await expect(serviceCall({ url: "/test" })).rejects.toThrow("resulted in error");

        expect(state.calls).toBe(2);
        expect(ErrorHandling.showServiceError).toHaveBeenCalledTimes(1);
    });

    it("rejects without showing the error when the user gives up", async () => {
        const state = mockService();
        removers.push(addServiceErrorRecovery(() => Promise.resolve(false)));
        const onCleanup = vi.fn();

        await expect(serviceCall({ url: "/test", onCleanup })).rejects.toMatchObject({ kind: "service-error" });

        expect(state.calls).toBe(1);
        expect(onCleanup).toHaveBeenCalledTimes(1);
        expect(ErrorHandling.showServiceError).not.toHaveBeenCalled();
    });

    it("shows the errors that no recovery handles, and stops asking a removed one", async () => {
        mockService("Other");
        const recovery = vi.fn(() => undefined);
        const remove = addServiceErrorRecovery(recovery);

        await expect(serviceCall({ url: "/test" })).rejects.toThrow("resulted in error");
        expect(recovery).toHaveBeenCalledTimes(1);
        expect(ErrorHandling.showServiceError).toHaveBeenCalledTimes(1);

        remove();
        await expect(serviceCall({ url: "/test" })).rejects.toThrow("resulted in error");
        expect(recovery).toHaveBeenCalledTimes(1);
    });
});
