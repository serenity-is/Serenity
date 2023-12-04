import { Lookup } from "./lookup";
import { fetchScriptData, getScriptDataHash } from "./scriptdata";
import { getStateStore } from "./system";

const __scriptData = "__scriptData";
const __scriptHash = "__scriptHash";

beforeEach(() => {
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
        var hashes = {
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
        var script = document.createElement("script");
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
        var script = document.createElement("script");
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
        var script = document.createElement("script");
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
            var data = await fetchScriptData("RemoteData.Test");
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
            var data = await fetchScriptData("Lookup.Test") as Lookup<any>;
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
});