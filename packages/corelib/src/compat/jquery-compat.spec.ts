﻿import { jQueryPatch } from "./jquery-compat";

afterEach(() => {
    delete (window as any).jQuery;
});

describe("jQuery CSRF-TOKEN integration", () => {
    it("should set the CSRF token in jQuery ajax headers", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ajaxSetup = (jQuery as any).ajaxSetup = vi.fn();

        jQueryPatch();

        expect(ajaxSetup).toHaveBeenCalledTimes(1);
        const beforeSend = ajaxSetup.mock.calls[0][0]?.beforeSend;
        expect(beforeSend).toBeDefined();
        const cookieSpy = (jQuery as any).cookie = vi.fn().mockReturnValue("TEST-TOKEN");
        const setRequestHeader = vi.fn();
        beforeSend({ setRequestHeader }, {});
        expect(cookieSpy).toHaveBeenCalledTimes(1);
        expect(cookieSpy).toHaveBeenCalledWith("CSRF-TOKEN");
        expect(setRequestHeader).toHaveBeenCalledTimes(1);
        expect(setRequestHeader).toHaveBeenCalledWith("X-CSRF-TOKEN", "TEST-TOKEN");
    });

    it("should not set the CSRF token for cross origin request", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ajaxSetup = (jQuery as any).ajaxSetup = vi.fn();

        jQueryPatch();

        expect(ajaxSetup).toHaveBeenCalledTimes(1);
        const beforeSend = ajaxSetup.mock.calls[0][0]?.beforeSend;
        expect(beforeSend).toBeDefined();
        const cookieSpy = (jQuery as any).cookie = vi.fn().mockReturnValue("TEST-TOKEN");
        const setRequestHeader = vi.fn();
        beforeSend({ setRequestHeader }, { crossDomain: true });
        expect(cookieSpy).not.toHaveBeenCalled();
        expect(setRequestHeader).not.toHaveBeenCalled();
    });
});
