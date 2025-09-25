import * as base from "../base";
import { ScriptData, canLoadScriptData, getColumns, getColumnsAsync, getColumnsData, getForm, getFormAsync, getFormData, getLookup, reloadLookup } from "./scriptdata-compat";

vi.mock(import("../base"), async () => {
    return {
        ensureScriptDataSync: vi.fn(),
        getColumnsScript: vi.fn(),
        getFormScript: vi.fn(),
        getGlobalObject: vi.fn().mockImplementation(() => globalThis),
        getLookupAsync: vi.fn(),
        getRemoteData: vi.fn(),
        getRemoteDataAsync: vi.fn(),
        getScriptData: vi.fn(),
        getScriptDataHash: vi.fn(),
        peekScriptData: vi.fn(),
        reloadLookupAsync: vi.fn(),
        setScriptData: vi.fn(),
    };
});

afterEach(() => {
    vi.clearAllMocks();
});

describe("ScriptData.reload", () => {
    it("calls getScriptDataHash, setScriptData, ensureScriptDataSync with expected arguments", () => {
        ScriptData.reload("Test.ScriptKey", true);
        expect(base.getScriptDataHash).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey", true);
        expect(base.setScriptData).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey", null);
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey", true);
    });
});

describe("ScriptData.reloadAsync", () => {
    it("calls getScriptData with expected arguments", async () => {
        const mockData = { test: "data" };
        vi.mocked(base.getScriptData).mockResolvedValue(mockData);
        
        const result = await ScriptData.reloadAsync("Test.ScriptKey");
        
        expect(base.getScriptData).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey", true);
        expect(result).toBe(mockData);
    });
});

describe("ScriptData.bindToChange", () => {
    it("adds event listener and returns unbind function when document is available", () => {
        const mockOnChange = vi.fn();
        const unbind = ScriptData.bindToChange("Test.ScriptKey", mockOnChange);
        
        expect(typeof unbind).toBe("function");
        
        if (typeof unbind === "function") {
            unbind();
        }
    });

    it("returns undefined when document is not available", () => {
        const originalDocument = globalThis.document;
        delete (globalThis as any).document;
        
        try {
            const result = ScriptData.bindToChange("Test.ScriptKey", vi.fn());
            expect(result).toBeUndefined();
        } finally {
            globalThis.document = originalDocument;
        }
    });
});

describe("canLoadScriptData", () => {
    it("returns true when peekScriptData returns data", () => {
        vi.mocked(base.peekScriptData).mockReturnValue({ test: "data" });
        vi.mocked(base.getScriptDataHash).mockReturnValue(null);
        
        const result = canLoadScriptData("Test.ScriptKey");
        
        expect(base.peekScriptData).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey");
        expect(result).toBe(true);
    });

    it("returns true when getScriptDataHash returns data", () => {
        vi.mocked(base.peekScriptData).mockReturnValue(null);
        vi.mocked(base.getScriptDataHash).mockReturnValue("hash");
        
        const result = canLoadScriptData("Test.ScriptKey");
        
        expect(base.peekScriptData).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey");
        expect(base.getScriptDataHash).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey");
        expect(result).toBe(true);
    });

    it("returns false when both peekScriptData and getScriptDataHash return null", () => {
        vi.mocked(base.peekScriptData).mockReturnValue(null);
        vi.mocked(base.getScriptDataHash).mockReturnValue(null);
        
        const result = canLoadScriptData("Test.ScriptKey");
        
        expect(base.peekScriptData).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey");
        expect(base.getScriptDataHash).toHaveBeenCalledExactlyOnceWith("Test.ScriptKey");
        expect(result).toBe(false);
    });
});

describe("getLookup", () => {
    it("calls ScriptData.ensure with Lookup prefix", () => {
        const mockLookup = { items: [] };
        vi.mocked(base.ensureScriptDataSync).mockReturnValue(mockLookup);
        
        const result = getLookup("TestKey");
        
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Lookup.TestKey");
        expect(result).toBe(mockLookup);
    });
});

describe("reloadLookup", () => {
    it("calls ScriptData.reload with Lookup prefix", () => {
        const mockLookup = { items: [] };
        vi.mocked(base.ensureScriptDataSync).mockReturnValue(mockLookup);
        
        const result = reloadLookup("TestKey");
        
        expect(base.getScriptDataHash).toHaveBeenCalledExactlyOnceWith("Lookup.TestKey", true);
        expect(base.setScriptData).toHaveBeenCalledExactlyOnceWith("Lookup.TestKey", null);
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Lookup.TestKey", undefined);
        expect(result).toBe(mockLookup);
    });
});

describe("getColumns", () => {
    it("calls getColumnsData and returns items", () => {
        const mockData = { items: [{ name: "Test" }] };
        vi.mocked(base.ensureScriptDataSync).mockReturnValue(mockData);
        
        const result = getColumns("TestKey");
        
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Columns.TestKey");
        expect(result).toBe(mockData.items);
    });
});

describe("getColumnsAsync", () => {
    it("calls getColumnsScript and returns items", async () => {
        const mockData = { items: [{ name: "Test" }], additionalItems: [] };
        vi.mocked(base.getColumnsScript).mockResolvedValue(mockData);
        
        const result = await getColumnsAsync("TestKey");
        
        expect(base.getColumnsScript).toHaveBeenCalledExactlyOnceWith("TestKey");
        expect(result).toBe(mockData.items);
    });
});

describe("getColumnsData", () => {
    it("calls ScriptData.ensure with Columns prefix", () => {
        const mockData = { items: [{ name: "Test" }] };
        vi.mocked(base.ensureScriptDataSync).mockReturnValue(mockData);
        
        const result = getColumnsData("TestKey");
        
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Columns.TestKey");
        expect(result).toBe(mockData);
    });
});

describe("getForm", () => {
    it("calls getFormData and returns items", () => {
        const mockData = { items: [{ name: "Test" }] };
        vi.mocked(base.ensureScriptDataSync).mockReturnValue(mockData);
        
        const result = getForm("TestKey");
        
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Form.TestKey");
        expect(result).toBe(mockData.items);
    });
});

describe("getFormAsync", () => {
    it("calls getFormScript and returns items", async () => {
        const mockData = { items: [{ name: "Test" }], additionalItems: [] };
        vi.mocked(base.getFormScript).mockResolvedValue(mockData);
        
        const result = await getFormAsync("TestKey");
        
        expect(base.getFormScript).toHaveBeenCalledExactlyOnceWith("TestKey");
        expect(result).toBe(mockData.items);
    });
});

describe("getFormData", () => {
    it("calls ScriptData.ensure with Form prefix", () => {
        const mockData = { items: [{ name: "Test" }] };
        vi.mocked(base.ensureScriptDataSync).mockReturnValue(mockData);
        
        const result = getFormData("TestKey");
        
        expect(base.ensureScriptDataSync).toHaveBeenCalledExactlyOnceWith("Form.TestKey");
        expect(result).toBe(mockData);
    });
});
