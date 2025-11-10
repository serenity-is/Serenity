import { Lookup } from "./lookup";
import { notifyError } from "./notify";
import { ensureScriptDataSync, fetchScriptData, getColumnsScript, getFormScript, getLookupAsync, getRemoteDataAsync, getScriptData, getScriptDataHash, handleScriptDataError, peekScriptData, scriptDataHooks, setRegisteredScripts, setScriptData } from "./scriptdata";
import { scriptDataHashSymbol, scriptDataSymbol } from "./symbols";
import { getGlobalObject } from "./system";

vi.mock("./notify", () => ({
    notifyError: vi.fn()
}));


beforeEach(() => {
    vi.clearAllMocks();
    delete getGlobalObject()[scriptDataHashSymbol];
    delete getGlobalObject()[scriptDataSymbol];
    delete scriptDataHooks.fetchScriptData;
});

describe("getScriptDataHash", () => {
    it("returns null if scriptDataHash is null", () => {
        expect(getScriptDataHash("test")).toBe(null);
    });

    it("returns a new random string if reload is true and existing scriptDataHash is null", () => {
        let hash = getScriptDataHash("test", true);
        expect(typeof hash).toBe("string");
        expect(hash.length).toBeGreaterThan(1);
        let store = getGlobalObject()[scriptDataHashSymbol];
        expect(store).toEqual({
            test: hash
        });
    });

    it("returns a new random string if reload is true", () => {
        getGlobalObject()[scriptDataHashSymbol] = {
            test: "1357",
            some: "2468"
        }
        let hash = getScriptDataHash("test", true);
        expect(typeof hash).toBe("string");
        expect(hash.length).toBeGreaterThan(1);
        let store = getGlobalObject()[scriptDataHashSymbol];
        expect(store).toEqual({
            test: hash,
            some: "2468"
        });
    });

    it("returns the existing hash if available", () => {
        let hashes = {
            test: "1357",
            some: "2468"
        };
        getGlobalObject()[scriptDataHashSymbol] = hashes;
        let hash = getScriptDataHash("test");
        expect(hash).toBe("1357");
        let store = getGlobalObject()[scriptDataHashSymbol];
        expect(store).toBe(hashes);
    });

    it("ignores empty script element with RegisteredScripts ID", () => {
        let script = document.createElement("script");
        script.setAttribute("id", "RegisteredScripts");
        script.type = "application/json";
        document.body.append(script);
        try {
            let hash = getScriptDataHash("test");
            expect(hash).toBe(null);
            expect(getGlobalObject()[scriptDataHashSymbol]).toBeUndefined();
        }
        finally {
            script.remove();
        }

    });

    it("ignores malformed script element with RegisteredScripts ID", () => {
        let script = document.createElement("script");
        script.setAttribute("id", "RegisteredScripts");
        script.type = "application/json";
        script.innerHTML = " malformed json {";
        document.body.append(script);
        try {
            let hash = getScriptDataHash("test");
            expect(hash).toBe(null);
            expect(getGlobalObject()[scriptDataHashSymbol]).toBeUndefined();
        }
        finally {
            script.remove();
        }
    });

    it("parses script element with RegisteredScripts ID and valid JSON", () => {
        let script = document.createElement("script");
        script.setAttribute("id", "RegisteredScripts");
        script.type = "application/json";
        script.innerHTML = '   { "test": "555", "some": "333" }';
        document.body.append(script);
        try {
            let hash = getScriptDataHash("test");
            expect(hash).toBe("555");
            expect(getGlobalObject()[scriptDataHashSymbol]).toEqual({
                test: "555",
                some: "333"
            });
        }
        finally {
            script.remove();
        }
    });
});

describe("fetchScriptData", () => {
    it("uses fetch and DynamicData endpoint with current hash", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["RemoteData.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/RemoteData.Test?v=123");
            return Promise.resolve({
                ok: true,
                status: 200,
                statusText: 'OK',
                json: () => Promise.resolve({ test: 1 })
            });
        };
        window["fetch"] = mockFetch as any;
        try {
            let data = await fetchScriptData("RemoteData.Test");
            expect(data).toEqual({
                test: 1
            });
            expect(calls).toBe(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("returns the same promise for successive calls", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["RemoteData.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/RemoteData.Test?v=123");
            return await Promise.resolve({
                ok: true,
                status: 200,
                statusText: 'OK',
                json: () => Promise.resolve({ test: 1 })
            });
        };
        window["fetch"] = mockFetch as any;
        try {
            let promise1 = fetchScriptData("RemoteData.Test");
            let promise2 = fetchScriptData("RemoteData.Test");
            let promise3 = fetchScriptData("RemoteData.Test");
            expect(promise1 === promise2).toBe(true);
            expect(promise1 === promise3).toBe(true);
            let data1 = await promise1;
            let data2 = await promise2;
            let data3 = await promise3;
            expect(data1).toEqual({
                test: 1
            });
            expect(data2).toEqual({
                test: 1
            });
            expect(data3).toEqual({
                test: 1
            });
            expect(calls).toBe(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("converts response to a lookup for name that starts with Lookup.", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["Lookup.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/Lookup.Test?v=123");
            return Promise.resolve({
                ok: true,
                status: 200,
                statusText: 'OK',
                json: () => Promise.resolve({ Params: { idField: "x", textField: "t" }, Items: [{ x: 3, t: "T3" }] })
            });
        };
        window["fetch"] = mockFetch as any;
        try {
            let data = await fetchScriptData("Lookup.Test") as Lookup<any>;
            expect(data instanceof Lookup).toBe(true);
            expect(data.items).toEqual([{
                x: 3,
                t: "T3"
            }]);
            expect(data.itemById).toEqual({
                "3": {
                    x: 3,
                    t: "T3"
                }
            });
            expect(data.idField).toEqual("x");
            expect(data.textField).toEqual("t");
            expect(calls).toBe(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("throws if fetch is not available", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["Lookup.Test"]: "123" };
        let orgFetch = window["fetch"];
        delete window["fetch"];
        try {
            await expect(async () => {
                return await fetchScriptData("Lookup.Test");
            }).rejects.toMatch("The fetch method is not available!");
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("returns a rejected promise if response.ok is false", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["Lookup.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/Lookup.Test?v=123");
            return Promise.resolve({
                ok: false,
                status: 500,
                statusText: 'Server error'
            });
        };
        window["fetch"] = mockFetch as any;
        try {
            const logSpy = vi.spyOn(console, "log").mockImplementation(() => { });
            let notify = await import("./notify");
            await expect(async () => {
                await fetchScriptData("Lookup.Test")
            }).rejects.toMatch('An error occurred while trying to load the lookup: "Test"!. Please check the error message displayed in the console for more info.');

            expect(notify.notifyError).toHaveBeenCalledTimes(1);
            expect(logSpy).toHaveBeenCalledTimes(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("uses scriptDataHooks.fetchScriptData hook if defined (sync, non-lookup)", async () => {
        const testData = { foo: "bar" };
        const originalHook = scriptDataHooks?.fetchScriptData;
        scriptDataHooks.fetchScriptData = (name: string) => {
            expect(name).toBe("RemoteData.Test");
            return testData as any;
        }
        let data = await fetchScriptData("RemoteData.Test");
        expect(data).toBe(testData);
        scriptDataHooks.fetchScriptData = originalHook;
    });

    it("uses scriptDataHooks.fetchScriptData hook if defined (async, non-lookup)", async () => {
        const testData = { foo: "baz" };
        const originalHook = scriptDataHooks.fetchScriptData;
        scriptDataHooks.fetchScriptData = async (name: string) => {
            expect(name).toBe("RemoteData.Test");
            return Promise.resolve(testData) as any;
        };
        let data = await fetchScriptData("RemoteData.Test");
        expect(data).toBe(testData);
        scriptDataHooks.fetchScriptData = originalHook;
    });

    it("uses scriptDataHooks.fetchScriptData hook and converts to Lookup if name starts with Lookup.", async () => {
        const testLookup = { Params: { idField: "id", textField: "txt" }, Items: [{ id: 1, txt: "A" }] };
        const originalHook = (globalThis as any).scriptDataHooks?.fetchScriptData;
        scriptDataHooks.fetchScriptData = (name: string) => {
            expect(name).toBe("Lookup.Test");
            return testLookup as any;
        };

        let data: any = await fetchScriptData("Lookup.Test");
        expect(data).toBeInstanceOf(Lookup);
        expect(data.items).toEqual([{ id: 1, txt: "A" }]);
        expect(data.idField).toBe("id");
        expect(data.textField).toBe("txt");
        scriptDataHooks.fetchScriptData = originalHook;
    });

    it("uses scriptDataHooks.fetchScriptData hook and returns Lookup if Items present but not items/itemById", async () => {
        const testLookup = { Params: { idField: "id", textField: "txt" }, Items: [{ id: 2, txt: "B" }] };
        const originalHook = (globalThis as any).scriptDataHooks?.fetchScriptData;
        scriptDataHooks.fetchScriptData = (name: string) => {
            expect(name).toBe("Lookup.Test2");
            return testLookup as any;
        };
        let data: any = await fetchScriptData("Lookup.Test2");
        expect(data).toBeInstanceOf(Lookup);
        expect(data.items).toEqual([{ id: 2, txt: "B" }]);
        expect(data.idField).toBe("id");
        expect(data.textField).toBe("txt");
        scriptDataHooks.fetchScriptData = originalHook;
    });
});

describe("getScriptData", () => {
    it("returns data from Serenity.scriptData symbol if available and reload is not true", async () => {
        getGlobalObject()[scriptDataSymbol] = { ["RemoteData.Test"]: "357" };
        let orgFetch = window["fetch"];
        let mockFetch = vi.fn();
        window["fetch"] = mockFetch as any;
        try {
            let data = await getScriptData("RemoteData.Test");
            expect(data).toEqual("357");
            expect(mockFetch).not.toHaveBeenCalled();
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("uses fetch and DynamicData endpoint with current hash", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["RemoteData.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/RemoteData.Test?v=123");
            return Promise.resolve({
                ok: true,
                status: 200,
                statusText: 'OK',
                json: () => Promise.resolve({ test: 1 })
            });
        }

        window["fetch"] = mockFetch as any;
        try {
            let data = await getScriptData("RemoteData.Test");
            expect(data).toEqual({ test: 1 });
            expect(calls).toBe(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("reloads data if second argument is true", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["RemoteData.Test"]: "123" };
        getGlobalObject()[scriptDataSymbol] = { ["RemoteData.Test"]: "old" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url.startsWith("/DynamicData/RemoteData.Test?v=")).toBe(true);
            expect(url).not.toBe("/DynamicData/RemoteData.Test?v=123");
            return {
                ok: true,
                status: 200,
                statusText: 'OK',
                json: async () => "new"
            };
        }

        window["fetch"] = mockFetch as any;
        try {
            expect(peekScriptData("RemoteData.Test")).toBe("old");
            let data = await getScriptData("RemoteData.Test", true);
            expect(calls).toBe(1);
            expect(data).toEqual("new");
            expect(peekScriptData("RemoteData.Test")).toBe("new");
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("calls fetch once for successive calls", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["RemoteData.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/RemoteData.Test?v=123");
            return await Promise.resolve({
                ok: true,
                status: 200,
                statusText: 'OK',
                json: () => Promise.resolve({ test: 1 })
            });
        };

        window["fetch"] = mockFetch as any;
        try {

            let data1, data2, data3: any;
            await Promise.all([
                getScriptData("RemoteData.Test").then(x => data1 = x),
                getScriptData("RemoteData.Test").then(x => data2 = x),
                getScriptData("RemoteData.Test").then(x => data3 = x)
            ]);
            expect(data1).toEqual({ test: 1 });
            expect(data2).toEqual({ test: 1 });
            expect(data3).toEqual({ test: 1 });
            expect(data1).toBe(data2);
            expect(data2).toBe(data3);
            expect(calls).toBe(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("converts response to a lookup for name that starts with Lookup.", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["Lookup.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/Lookup.Test?v=123");
            return Promise.resolve({
                ok: true,
                status: 200,
                statusText: 'OK',
                json: () => Promise.resolve({ Params: { idField: "x", textField: "t" }, Items: [{ x: 3, t: "T3" }] })
            });
        };
        window["fetch"] = mockFetch as any;
        try {
            let data = await getScriptData("Lookup.Test") as Lookup<any>;
            expect(data instanceof Lookup).toBe(true);
            expect(data.items).toEqual([{
                x: 3,
                t: "T3"
            }]);
            expect(data.itemById).toEqual({
                "3": {
                    x: 3,
                    t: "T3"
                }
            });
            expect(data.idField).toEqual("x");
            expect(data.textField).toEqual("t");
            expect(calls).toBe(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("throws if fetch is not available", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["Lookup.Test"]: "123" };
        let orgFetch = window["fetch"];
        delete window["fetch"];
        try {
            await expect(async () => {
                return await getScriptData("Lookup.Test");
            }).rejects.toMatch("The fetch method is not available!");
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("returns a rejected promise if response.ok is false", async () => {
        getGlobalObject()[scriptDataHashSymbol] = { ["Lookup.Test"]: "123" };
        let orgFetch = window["fetch"];
        let calls = 0;
        let mockFetch = async (url: string, init: RequestInit) => {
            calls++;
            expect(url).toBe("/DynamicData/Lookup.Test?v=123");
            return Promise.resolve({
                ok: false,
                status: 500,
                statusText: 'Server error'
            });
        };
        window["fetch"] = mockFetch as any;
        try {
            const logSpy = vi.spyOn(console, "log").mockImplementation(() => { });
            let notify = await import("./notify");
            await expect(async () => {
                await getScriptData("Lookup.Test")
            }).rejects.toMatch('An error occurred while trying to load the lookup: "Test"!. Please check the error message displayed in the console for more info.');

            expect(notify.notifyError).toHaveBeenCalledTimes(1);
            expect(logSpy).toHaveBeenCalledTimes(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });
});

describe("peekScriptData", () => {
    it("returns data from scriptDataItem if available", () => {
        getGlobalObject()[scriptDataSymbol] = { ["RemoteData.Test"]: "357" };
        let orgFetch = window["fetch"];
        let mockFetch = vi.fn();
        window["fetch"] = mockFetch as any;
        try {
            let data = peekScriptData("RemoteData.Test");
            expect(data).toEqual("357");
            expect(mockFetch).not.toHaveBeenCalled();
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });

    it("returns undefined if data is not set", () => {
        let orgFetch = window["fetch"];
        let mockFetch = vi.fn();
        window["fetch"] = mockFetch as any;
        try {
            let data = peekScriptData("RemoteData.Test");
            expect(data).toBeUndefined();
            expect(mockFetch).not.toHaveBeenCalled();
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });
});

describe("setRegisteredScripts", () => {
    it("sets hashes for the provided scripts", () => {
        const scripts = {
            "Script1": "hash1",
            "Script2": "hash2"
        };

        setRegisteredScripts(scripts);

        const scriptDataHash = getGlobalObject()[scriptDataHashSymbol];
        expect(scriptDataHash).toBeDefined();
        expect(scriptDataHash["Script1"]).toBe("hash1");
        expect(scriptDataHash["Script2"]).toBe("hash2");
    });

    it("uses the current timestamp as hash if no hash is provided", () => {
        const scripts = {
            "Script1": "",
            "Script2": null
        };

        const timestamp = Math.trunc(new Date().getTime());
        vi.useFakeTimers().setSystemTime(new Date(timestamp));
        try {
            setRegisteredScripts(scripts);

            const scriptDataHash = getGlobalObject()[scriptDataHashSymbol];
            expect(scriptDataHash).toBeDefined();
            expect(scriptDataHash["Script1"]).toBe(timestamp.toString());
            expect(scriptDataHash["Script2"]).toBe(timestamp.toString());
        } finally {
            vi.useRealTimers();
        }
    });

    it("creates the scriptDataHash store if it does not exist", () => {
        delete getGlobalObject()[scriptDataHashSymbol];

        const scripts = {
            "Script1": "hash1"
        };

        setRegisteredScripts(scripts);

        const scriptDataHash = getGlobalObject()[scriptDataHashSymbol];
        expect(scriptDataHash).toBeDefined();
        expect(scriptDataHash["Script1"]).toBe("hash1");
    });
});

describe("setScriptData", () => {

    it("sets data in the global scriptData store", () => {
        const testData = { key: "value" };
        setScriptData("TestKey", testData);

        const scriptDataStore = getGlobalObject()[scriptDataSymbol];
        expect(scriptDataStore).toBeDefined();
        expect(scriptDataStore["TestKey"]).toEqual(testData);
    });

    it("dispatches a scriptdatachange event when data is set", () => {
        const testData = { key: "value" };
        const eventSpy = vi.spyOn(document, "dispatchEvent");

        setScriptData("TestKey", testData);

        expect(eventSpy).toHaveBeenCalledTimes(1);
        expect(eventSpy).toHaveBeenCalledWith(new Event("scriptdatachange.TestKey"));
    });

    it("creates the scriptData store if it does not exist", () => {
        delete getGlobalObject()[scriptDataSymbol];

        const testData = { key: "value" };
        setScriptData("TestKey", testData);

        const scriptDataStore = getGlobalObject()[scriptDataSymbol];
        expect(scriptDataStore).toBeDefined();
        expect(scriptDataStore["TestKey"]).toEqual(testData);
    });
});

describe("ensureScriptDataSync", () => {
    it("returns data from scriptData store if available", () => {
        const testData = { key: "value" };
        getGlobalObject()[scriptDataSymbol] = { ["TestKey"]: testData };

        const result = ensureScriptDataSync("TestKey");
        expect(result).toEqual(testData);
    });

    it("throws an error if fetchScriptData hook returns a promise in sync mode", () => {
        const originalHook = scriptDataHooks.fetchScriptData;
        scriptDataHooks.fetchScriptData = () => Promise.resolve(1 as any);
        try {
            expect(() => ensureScriptDataSync("TestKey")).toThrow("fetchScriptData hook must return data synchronously when sync is true.");
        }
        finally {
            scriptDataHooks.fetchScriptData = originalHook;
        }
    });

    it("when fetchScriptData hook is used response is converted to a lookup for name that starts with Lookup.", async () => {
        const originalHook = scriptDataHooks.fetchScriptData;
        scriptDataHooks.fetchScriptData = () => ({ Params: { idField: "x", textField: "t" }, Items: [{ x: 3, t: "T3" }] } as any);
        try {
            const data = ensureScriptDataSync("Lookup.Test") as Lookup<any>;
            expect(data instanceof Lookup).toBe(true);
            expect(data.items).toEqual([{
                x: 3,
                t: "T3"
            }]);
            expect(data.itemById).toEqual({
                "3": {
                    x: 3,
                    t: "T3"
                }
            });
            expect(data.idField).toEqual("x");
            expect(data.textField).toEqual("t");
        }
        finally {
            scriptDataHooks.fetchScriptData = originalHook;
        }
    });

    it("converts response to a lookup for name that starts with Lookup.", async () => {
        const lookupData = { Params: { idField: "x", textField: "t" }, Items: [{ x: 3, t: "T3" }] };
        const xhrMock = {
            open: vi.fn(),
            send: vi.fn(),
            status: 200,
            responseText: JSON.stringify(lookupData)
        };
        vi.spyOn(window, "XMLHttpRequest").mockImplementation(function() { return xhrMock as any; });
        const data = ensureScriptDataSync("Lookup.Test") as Lookup<any>;
        expect(data instanceof Lookup).toBe(true);
        expect(data.items).toEqual([{
            x: 3,
            t: "T3"
        }]);
        expect(data.itemById).toEqual({
            "3": {
                x: 3,
                t: "T3"
            }
        });
        expect(data.idField).toEqual("x");
        expect(data.textField).toEqual("t");
    });


    it("fetches data synchronously via XMLHttpRequest if not in store", () => {
        const testData = { key: "value" };
        const xhrMock = {
            open: vi.fn(),
            send: vi.fn(),
            status: 200,
            responseText: JSON.stringify(testData)
        };
        vi.spyOn(window, "XMLHttpRequest").mockImplementation(function() { return xhrMock as any; });

        const result = ensureScriptDataSync("TestKey");
        expect(xhrMock.open).toHaveBeenCalledWith("GET", expect.stringContaining("/DynamicData/TestKey"), false);
        expect(xhrMock.send).toHaveBeenCalled();
        expect(result).toEqual(testData);
    });

    it("throws an error if XMLHttpRequest returns null", () => {
        const xhrMock = {
            open: vi.fn(),
            send: vi.fn(),
            status: 200,
            responseText: JSON.stringify(null)
        };
        vi.spyOn(window, "XMLHttpRequest").mockImplementation(function() { return xhrMock as any; });
        const logSpy = vi.spyOn(console, "log").mockImplementation(() => { });

        expect(() => ensureScriptDataSync("TestKey")).toThrow("Cannot load dynamic data: TestKey!")

        expect(logSpy).not.toHaveBeenCalled();
        expect(vi.mocked(notifyError)).toHaveBeenCalledExactlyOnceWith("Cannot load dynamic data: TestKey!");
    });

    it("uses DynJS.axd endpoint if second argument is true", () => {
        const testData = { key: "value" };
        const xhrMock = {
            open: vi.fn(),
            send: vi.fn(),
            status: 200,
            responseText: `(function() {
            })();`
        };
        vi.spyOn(window, "XMLHttpRequest").mockImplementation(function() { return xhrMock as any; });
        const appendChildSpy = vi.spyOn(window.document.head, "appendChild").mockImplementation(() => {
            const s = Symbol.for('Serenity.scriptData');
            globalThis[s] ??= {};
            globalThis[s]["TestKey"] = testData;
            const script = document.createElement("div");
            document.body.append(script);
            vi.spyOn(document.body, "removeChild").mockImplementation(() => {
                return null;
            });
            return script;
        });

        const result = ensureScriptDataSync("TestKey", true);
        expect(xhrMock.open).toHaveBeenCalledWith("GET", expect.stringContaining("/DynJS.axd/TestKey.js"), false);
        expect(xhrMock.send).toHaveBeenCalled();
        expect(appendChildSpy).toHaveBeenCalledOnce();
        expect(result).toEqual(testData);
    });

    it("throws an error if XMLHttpRequest fails and logs the error", () => {
        const xhrMock = {
            open: vi.fn(),
            send: vi.fn(),
            status: 500,
            statusText: "Server Error"
        };
        vi.spyOn(window, "XMLHttpRequest").mockImplementation(function() { return xhrMock as any; });
        const logSpy = vi.spyOn(console, "log").mockImplementation(() => { });

        expect(() => ensureScriptDataSync("TestKey")).toThrow("An error occurred while trying to load dynamic data: \"TestKey\"!.");

        expect(logSpy).toHaveBeenCalledWith("HTTP 500: Connection refused!");
        expect(vi.mocked(notifyError)).toHaveBeenCalledExactlyOnceWith("An error occurred while trying to load dynamic data: \"TestKey\"!. Please check the error message displayed in the console for more info.");
    });
});

describe("handleScriptDataError", () => {
    it("notifies and throws an error for a missing lookup", () => {
        const notifySpy = vi.mocked(notifyError);

        expect(() => handleScriptDataError("Lookup.Test", 404)).toThrow(
            'No lookup with key "Test" is registered. Please make sure you have a [LookupScript("Test")] attribute in server side code on top of a row / custom lookup and its key is exactly the same.'
        );

        expect(notifySpy).toHaveBeenCalledWith(
            'No lookup with key "Test" is registered. Please make sure you have a [LookupScript("Test")] attribute in server side code on top of a row / custom lookup and its key is exactly the same.'
        );
    });

    it("notifies and throws an error for access denied on a lookup", () => {
        const notifySpy = vi.mocked(notifyError);

        expect(() => handleScriptDataError("Lookup.Test", 403)).toThrow(expect.stringContaining("Access denied while trying to load the lookup"));

        expect(notifySpy).toHaveBeenCalledWith(
            expect.stringContaining("Access denied while trying to load the lookup"),
            null,
            {
                timeOut: 10000
            }
        );
    });

    it("logs and throws an error for a server error", () => {
        const notifySpy = vi.mocked(notifyError);
        const logSpy = vi.spyOn(console, "log").mockImplementation(() => { });

        expect(() => handleScriptDataError("DynamicData.Test", 500, "Server Error")).toThrow(
            'An error occurred while trying to load dynamic data: "DynamicData.Test"!. Please check the error message displayed in the console for more info.'
        );

        expect(logSpy).toHaveBeenCalledWith("HTTP 500: Connection refused!");
        expect(notifySpy).toHaveBeenCalledWith(
            'An error occurred while trying to load dynamic data: "DynamicData.Test"!. Please check the error message displayed in the console for more info.'
        );
    });

    it("logs and throws an error for an unknown error", () => {
        const logSpy = vi.spyOn(console, "log").mockImplementation(() => { });
        const notifySpy = vi.mocked(notifyError);

        expect(() => handleScriptDataError("DynamicData.Test", 0, "Unknown Error")).toThrow(
            'An error occurred while trying to load dynamic data: "DynamicData.Test"!. Please check the error message displayed in the console for more info.'
        );

        expect(logSpy).toHaveBeenCalledWith("An unknown connection error occurred!");
        expect(notifySpy).toHaveBeenCalledWith(
            'An error occurred while trying to load dynamic data: "DynamicData.Test"!. Please check the error message displayed in the console for more info.'
        );
    });
});

describe("getColumnsScript", () => {
    it("calls getScriptData with 'Columns.' plus passed key", async () => {
        const hook = vi.fn((name: string) => {
            return {} as any;
        })
        scriptDataHooks.fetchScriptData = hook;
        await getColumnsScript("TestKey");
        expect(hook).toHaveBeenCalledOnce();
        expect(hook).toHaveBeenCalledWith("Columns.TestKey");
    });
});


describe("getFormScript", () => {
    it("calls getScriptData with 'Form.' plus passed key", async () => {
        const hook = vi.fn((name: string) => {
            return {} as any;
        })
        scriptDataHooks.fetchScriptData = hook;
        await getFormScript("TestKey");
        expect(hook).toHaveBeenCalledOnce();
        expect(hook).toHaveBeenCalledWith("Form.TestKey");
    });
});

describe("getLookupAsync", () => {
    it("calls getScriptData with 'Lookup.' plus passed key", async () => {
        const hook = vi.fn((name: string) => {
            return {} as any;
        })
        scriptDataHooks.fetchScriptData = hook;
        await getLookupAsync("TestKey");
        expect(hook).toHaveBeenCalledOnce();
        expect(hook).toHaveBeenCalledWith("Lookup.TestKey");
    });
});

describe("getRemoteDataAsync", () => {
    it("calls getScriptData with 'RemoteData.' plus passed key", async () => {
        const hook = vi.fn((name: string) => {
            return {} as any;
        })
        scriptDataHooks.fetchScriptData = hook;
        await getRemoteDataAsync("TestKey");
        expect(hook).toHaveBeenCalledOnce();
        expect(hook).toHaveBeenCalledWith("RemoteData.TestKey");
    });
});

describe("getRemoteData", () => {
    it("calls getScriptData with 'RemoteData.' plus passed key", async () => {
        const hook = vi.fn((name: string) => {
            return {} as any;
        })
        scriptDataHooks.fetchScriptData = hook;
        await getRemoteDataAsync("TestKey");
        expect(hook).toHaveBeenCalledOnce();
        expect(hook).toHaveBeenCalledWith("RemoteData.TestKey");
    });
});