import * as base from "../base";
import { setEquality, postToService, postToUrl } from "./services-compat";

vi.mock(import("../base"), async () => {
    return {
        getCookie: vi.fn(),
        isSameOrigin: vi.fn().mockReturnValue(true),
        resolveUrl: vi.fn((url: string) => url?.startsWith("~/") ? "/app/" + url.substring(2) : url),
        resolveServiceUrl: vi.fn((service: string) => "/app/Services/" + service),
    };
});

// Mock DOM methods
const mockAppendChild = vi.fn();
const mockSubmit = vi.fn();
const mockRemove = vi.fn();
const mockSetTimeout = vi.fn();
const mockFormAction = vi.fn();

Object.defineProperty(document.body, 'appendChild', {
    writable: true,
    value: mockAppendChild
});

Object.defineProperty(window, 'setTimeout', {
    writable: true,
    value: mockSetTimeout
});

// Mock HTMLFormElement prototype
Object.defineProperty(HTMLFormElement.prototype, 'submit', {
    writable: true,
    value: mockSubmit
});

Object.defineProperty(HTMLFormElement.prototype, 'remove', {
    writable: true,
    value: mockRemove
});

afterEach(() => {
    vi.clearAllMocks();
    mockAppendChild.mockClear();
    mockSubmit.mockClear();
    mockRemove.mockClear();
    mockSetTimeout.mockClear();
    mockFormAction.mockClear();
    (base.isSameOrigin as any).mockReturnValue(true);
    (base.getCookie as any).mockReturnValue(undefined);
});

describe("setEquality", () => {
    it("sets equality filter on request", () => {
        const request: base.ListRequest = {};
        setEquality(request, "Field1", 123);
        expect(request.EqualityFilter).toEqual({ Field1: 123 });
        setEquality(request, "Field2", "Test");
        expect(request.EqualityFilter).toEqual({ Field1: 123, Field2: "Test" });
    });
});

describe("postToService", () => {
    beforeEach(() => {
        document.body.innerHTML = "";
    });

    it("posts to service with service name", () => {
        const options = {
            service: "MyService",
            request: { id: 123 }
        };

        postToService(options);

        expect(mockAppendChild).toHaveBeenCalledTimes(1);
        const form = mockAppendChild.mock.calls[0][0] as HTMLFormElement;
        expect(form.method?.toLowerCase()).toBe("post"); // HTML normalizes to lowercase
        expect(form.action).toContain("/app/Services/MyService");
        expect(form.target).toBe(""); // HTML default when not set
        
        // Check form contains the request input
        const inputs = form.querySelectorAll('input');
        expect(inputs.length).toBe(2); // hidden request input + submit button
        expect(inputs[0].name).toBe("request");
        expect(inputs[0].value).toBe(JSON.stringify({ id: 123 }));
        expect(inputs[1].type).toBe("submit");
        
        expect(mockSubmit).toHaveBeenCalledTimes(1);
        expect(mockSetTimeout).toHaveBeenCalledTimes(1);
        expect(mockRemove).toHaveBeenCalledTimes(0); // remove is called in setTimeout
    });

    it("posts to service with custom URL", () => {
        const options = {
            url: "~/MyCustomUrl",
            request: { data: "test" }
        };

        postToService(options);

        expect(mockAppendChild).toHaveBeenCalledTimes(1);
        const form = mockAppendChild.mock.calls[0][0] as HTMLFormElement;
        expect(form.action).toContain("/app/MyCustomUrl");
    });

    it("posts to service with target", () => {
        const options = {
            service: "MyService",
            request: {},
            target: "_blank"
        };

        postToService(options);

        expect(mockAppendChild).toHaveBeenCalledTimes(1);
        const form = mockAppendChild.mock.calls[0][0] as HTMLFormElement;
        expect(form.target).toBe("_blank");
    });

    it("adds CSRF token when same origin", () => {
        (base.getCookie as any).mockReturnValue("csrf-token-123");

        const options = {
            service: "MyService",
            request: {}
        };

        postToService(options);

        expect(base.getCookie).toHaveBeenCalledWith('CSRF-TOKEN');
        expect(base.isSameOrigin).toHaveBeenCalledWith('/app/Services/MyService');
    });

    it("does not add CSRF token when cross origin", () => {
        // Mock isSameOrigin to return false
        (base.isSameOrigin as any).mockReturnValue(false);
        (base.getCookie as any).mockReturnValue("csrf-token-123");

        const options = {
            service: "MyService",
            request: {}
        };

        postToService(options);

        expect(base.getCookie).not.toHaveBeenCalled();
        expect(base.isSameOrigin).toHaveBeenCalledWith('/app/Services/MyService');
    });
});

describe("postToUrl", () => {
    beforeEach(() => {
        document.body.innerHTML = "";
    });

    it("posts to URL with params", () => {
        const options = {
            url: "~/MyUrl",
            params: { id: 123, name: "test" }
        };

        (base.getCookie as any).mockReturnValue("csrf-token-456");

        postToUrl(options);

        expect(mockAppendChild).toHaveBeenCalledTimes(1);
        const form = mockAppendChild.mock.calls[0][0] as HTMLFormElement;
        expect(form.method?.toLowerCase()).toBe("post"); // HTML normalizes to lowercase
        expect(form.action).toContain("/app/MyUrl");
        expect(form.target).toBe(""); // HTML default when not set
        
        // Check form contains the param inputs
        const inputs = form.querySelectorAll('input');
        expect(inputs.length).toBe(4); // two params + csrf + submit button
        expect(inputs[0].name).toBe("id");
        expect(inputs[0].value).toBe("123");
        expect(inputs[1].name).toBe("name");
        expect(inputs[1].value).toBe("test");
        expect(inputs[2].name).toBe("__RequestVerificationToken");
        expect(inputs[2].value).toBe("csrf-token-456");
        expect(inputs[3].type).toBe("submit");
        
        expect(mockSubmit).toHaveBeenCalledTimes(1);
        expect(mockSetTimeout).toHaveBeenCalledTimes(1);
    });

    it("posts to URL with empty params", () => {
        const options = {
            url: "~/MyUrl",
            params: {}
        };

        postToUrl(options);

        const form = mockAppendChild.mock.calls[0][0] as HTMLFormElement;
        const inputs = form.querySelectorAll('input');
        expect(inputs.length).toBe(1); // only submit button
        expect(inputs[0].type).toBe("submit");
    });

    it("posts to URL with target", () => {
        const options = {
            url: "~/MyUrl",
            params: { id: 1 },
            target: "_blank"
        };

        postToUrl(options);

        const form = mockAppendChild.mock.calls[0][0] as HTMLFormElement;
        expect(form.target).toBe("_blank");
    });

    it("adds CSRF token when same origin", () => {
        (base.getCookie as any).mockReturnValue("csrf-token-456");

        const options = {
            url: "~/MyUrl",
            params: { id: 1 }
        };

        postToUrl(options);

        expect(base.getCookie).toHaveBeenCalledWith('CSRF-TOKEN');
        expect(base.isSameOrigin).toHaveBeenCalledWith('/app/MyUrl');
    });

    it("does not add CSRF token when cross origin", () => {
        (base.isSameOrigin as any).mockReturnValue(false);
        (base.getCookie as any).mockReturnValue("csrf-token-456");

        const options = {
            url: "~/MyUrl",
            params: { id: 1 }
        };

        postToUrl(options);

        expect(base.getCookie).not.toHaveBeenCalled();
        expect(base.isSameOrigin).toHaveBeenCalledWith('/app/MyUrl');
    });
});
