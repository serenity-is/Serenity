import { jQueryPatch } from "./jquerypatch";

afterEach(() => {
    delete (window as any).jQuery;
});

describe("jQuery CSRF-TOKEN integration", () => {
    it("should set the CSRF token in jQuery ajax headers", () => {
        const jQuery = (window as any).jQuery = jest.fn();
        const ajaxSetup = (jQuery as any).ajaxSetup = jest.fn();

        jQueryPatch();

        expect(ajaxSetup).toHaveBeenCalledTimes(1);
        const beforeSend = ajaxSetup.mock.calls[0][0]?.beforeSend;
        expect(beforeSend).toBeDefined();
        const cookieSpy = (jQuery as any).cookie = jest.fn().mockReturnValue("TEST-TOKEN");
        const setRequestHeader = jest.fn();
        beforeSend({ setRequestHeader }, {});
        expect(cookieSpy).toHaveBeenCalledTimes(1);
        expect(cookieSpy).toHaveBeenCalledWith("CSRF-TOKEN");
        expect(setRequestHeader).toHaveBeenCalledTimes(1);
        expect(setRequestHeader).toHaveBeenCalledWith("X-CSRF-TOKEN", "TEST-TOKEN");
    });

    it("should not set the CSRF token for cross origin request", () => {
        const jQuery = (window as any).jQuery = jest.fn();
        const ajaxSetup = (jQuery as any).ajaxSetup = jest.fn();

        jQueryPatch();

        expect(ajaxSetup).toHaveBeenCalledTimes(1);
        const beforeSend = ajaxSetup.mock.calls[0][0]?.beforeSend;
        expect(beforeSend).toBeDefined();
        const cookieSpy = (jQuery as any).cookie = jest.fn().mockReturnValue("TEST-TOKEN");
        const setRequestHeader = jest.fn();
        beforeSend({ setRequestHeader }, { crossDomain: true });
        expect(cookieSpy).not.toHaveBeenCalled();
        expect(setRequestHeader).not.toHaveBeenCalled();
    });
});
