import { Lookup } from "./lookup";
import { fetchScriptData, getScriptData, getScriptDataHash, peekScriptData } from "./scriptdata";
import { getStateStore } from "./system";

jest.mock("./notify", () => ({
    notifyError: jest.fn()
}));

const __scriptData = "__scriptData";
const __scriptHash = "__scriptHash";

beforeEach(() => {
    jest.clearAllMocks();
    let store = getStateStore();
    delete store[__scriptHash];
    delete store[__scriptData];
});

describe("getScriptDataHash", () => {
    it("returns null if __scriptHash is null", () => {
        expect(getScriptDataHash("test")).toBe(null);
    });

    it("returns a new random string if reload is true and existing __scriptHash is null", () => {
        let hash = getScriptDataHash("test", true);
        expect(typeof hash).toBe("string");
        expect(hash.length).toBeGreaterThan(1);
        let store = getStateStore(__scriptHash);
        expect(store).toEqual({
            test: hash
        });
    });

    it("returns a new random string if reload is true", () => {
        getStateStore()[__scriptHash] = {
            test: "1357",
            some: "2468"
        }
        let hash = getScriptDataHash("test", true);
        expect(typeof hash).toBe("string");
        expect(hash.length).toBeGreaterThan(1);
        let store = getStateStore(__scriptHash);
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
        getStateStore()[__scriptHash] = hashes;
        let hash = getScriptDataHash("test");
        expect(hash).toBe("1357");
        let store = getStateStore(__scriptHash);
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
            expect(getStateStore()[__scriptHash]).toBeUndefined();
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
            expect(getStateStore()[__scriptHash]).toBeUndefined();
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
            expect(getStateStore(__scriptHash)).toEqual({
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
        getStateStore(__scriptHash)["RemoteData.Test"] = "123";
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
        getStateStore(__scriptHash)["RemoteData.Test"] = "123";
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
        getStateStore(__scriptHash)["Lookup.Test"] = "123";
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
        getStateStore(__scriptHash)["Lookup.Test"] = "123";
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
        getStateStore(__scriptHash)["Lookup.Test"] = "123";
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
            const logSpy = jest.spyOn(console, "log").mockImplementation(() => { });
            let notify = await import("./notify");
            await expect(async () => {
                await fetchScriptData("Lookup.Test")
            }).rejects.toMatch("Server error");

            expect(notify.notifyError).toHaveBeenCalledTimes(1);
            expect(logSpy).toHaveBeenCalledTimes(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });
});

describe("getScriptData", () => {
    it("returns data from __scriptData if available and reload is not true", async () => {
        getStateStore(__scriptData)["RemoteData.Test"] = "357";
        let orgFetch = window["fetch"];
        let mockFetch = jest.fn();
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
        getStateStore(__scriptHash)["RemoteData.Test"] = "123";
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
        getStateStore(__scriptHash)["RemoteData.Test"] = "123";
        getStateStore(__scriptData)["RemoteData.Test"] = "old";
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
        getStateStore(__scriptHash)["RemoteData.Test"] = "123";
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
        getStateStore(__scriptHash)["Lookup.Test"] = "123";
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
        getStateStore(__scriptHash)["Lookup.Test"] = "123";
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
        getStateStore(__scriptHash)["Lookup.Test"] = "123";
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
            const logSpy = jest.spyOn(console, "log").mockImplementation(() => { });
            let notify = await import("./notify");
            await expect(async () => {
                await getScriptData("Lookup.Test")
            }).rejects.toMatch("Server error");

            expect(notify.notifyError).toHaveBeenCalledTimes(1);
            expect(logSpy).toHaveBeenCalledTimes(1);
        }
        finally {
            window["fetch"] = orgFetch;
        }
    });
});

describe("peekScriptData", () => {
    it("returns data from __stateStore if available", () => {
        getStateStore(__scriptData)["RemoteData.Test"] = "357";
        let orgFetch = window["fetch"];
        let mockFetch = jest.fn();
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
        let mockFetch = jest.fn();
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