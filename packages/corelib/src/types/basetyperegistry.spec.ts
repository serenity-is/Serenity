import { Config, getType, getGlobalTypeRegistry, isPromiseLike } from "../base";
import { BaseTypeRegistry } from "./basetyperegistry";

vi.mock("../base", async () => {
    return {
        Config: {
            rootNamespaces: [],
            lazyTypeLoader: null as any
        },
        getType: vi.fn(),
        getGlobalTypeRegistry: vi.fn(),
        getTypeNameProp: vi.fn(),
        isPromiseLike: vi.fn()
    };
});

// Mock types for testing
class TestType1 {
    static [Symbol.typeInfo] = { typeName: "TestType1" };
}

class TestType2 {
    static [Symbol.typeInfo] = { typeName: "TestType2" };
}

class TestEditor {
    static [Symbol.typeInfo] = { typeName: "TestEditor" };
}

class TestDialog {
    static [Symbol.typeInfo] = { typeName: "TestDialog" };
}

describe("BaseTypeRegistry", () => {

    class MyRegistry extends BaseTypeRegistry<any> {
        constructor() {
            super({
                loadKind: "test",
                defaultSuffix: "Type"
            });
        }

        protected override getSecondaryTypeKey(type: any) {
            return (type as any).secondaryTypeKey;
        }
        
        protected override isMatchingType(type: any) {
            return type[Symbol.typeInfo]?.typeName?.startsWith("Test");
        }

        protected override loadError(key: string) {
            throw new Error(`Type not found: ${key}`);
        }
    }

    let registry: MyRegistry;

    let originalLazyTypeLoader: any;

    beforeEach(() => {
        // Clear all mocks
        vi.clearAllMocks();

        // Save original config
        originalLazyTypeLoader = Config.lazyTypeLoader;

        // Reset config for tests
        Config.rootNamespaces = ["Test"];
        Config.lazyTypeLoader = null;

        // Mock isPromiseLike to work with promises
        vi.mocked(isPromiseLike).mockImplementation((obj: any) => obj && typeof obj.then === 'function');

        // Mock getTypes to return our test types
        vi.mocked(getGlobalTypeRegistry).mockReturnValue({
            TestType1: TestType1, 
            TestType2: TestType2,
            TestEditor: TestEditor,
            TestDialog: TestDialog
        });

        // Mock getType function
        vi.mocked(getType).mockImplementation((key: string) => {
            switch (key) {
                case "TestType1": return TestType1;
                case "Test.TestType1": return TestType1;
                case "TestType2": return TestType2;
                case "TestEditor": return TestEditor;
                case "TestDialog": return TestDialog;
                default: return null;
            }
        });

        // Create registry for testing
        registry = new MyRegistry();
    });

    describe("tryGet", () => {
        it("returns null for empty key", () => {
            expect(registry.tryGet("")).toBeNull();
            expect(registry.tryGet(null as any)).toBeNull();
            expect(registry.tryGet(undefined as any)).toBeNull();
        });

        it("finds type by exact name from known types", () => {
            const result = registry.tryGet("TestType1");
            expect(result).toBe(TestType1);
        });

        it("finds type by secondaryTypeKey if available", () => {
            (TestType1 as any).secondaryTypeKey = "customKey";
            const result = registry.tryGet("customKey");
            expect(result).toBe(TestType1);
            delete (TestType1 as any).secondaryTypeKey;
        });

        it("searches in root namespaces", () => {
            // Create a registry that matches TestType1
            class TestRegistry extends BaseTypeRegistry<any> {
                constructor() {
                    super({
                        loadKind: "test",
                        defaultSuffix: ""
                    });
                }

                protected override isMatchingType(type: any) {
                    return type === TestType1;
                }

                protected override loadError(key: string) {
                }
            }

            const testRegistry = new TestRegistry();

            // Mock getType to only return for namespaced key
            vi.mocked(getType).mockImplementation((key: string) => {
                if (key === "Test.TestType1") return TestType1;
                return null;
            });

            const result = testRegistry.tryGet("Test.TestType1");
            expect(result).toBe(TestType1);
            expect(vi.mocked(getType)).toHaveBeenCalledWith("Test.TestType1");
        });

        it("returns undefined for non-matching types", () => {
            const result = registry.tryGet("NonExistentType");
            expect(result).toBeUndefined();
        });

        it("caches found types", () => {
            registry.tryGet("TestType1");
            // Second call should use cache
            vi.mocked(getGlobalTypeRegistry).mockClear(); // Clear the mock to ensure it's not called again
            const result = registry.tryGet("TestType1");
            expect(result).toBe(TestType1);
            // getTypes should not be called again since it's cached
            expect(vi.mocked(getGlobalTypeRegistry)).not.toHaveBeenCalled();
        });
    });

    describe("get", () => {
        it("returns type when found", () => {
            const result = registry.get("TestType1");
            expect(result).toBe(TestType1);
        });

        it("throws error when type not found", () => {
            expect(() => registry.get("NonExistentType")).toThrow("Type not found: NonExistentType");
        });
    });

    describe("tryGetOrLoad", () => {
        it("returns type synchronously when found", () => {
            const result = registry.tryGetOrLoad("TestType1");
            expect(result).toBe(TestType1);
        });

        it("returns promise when lazy loading is configured", () => {
            Config.lazyTypeLoader = vi.fn().mockReturnValue(Promise.resolve(TestType1));
            const result = registry.tryGetOrLoad("LazyType");
            expect(typeof result).toBe("object"); // Promise
            expect(vi.mocked(Config.lazyTypeLoader!)).toHaveBeenCalledWith("LazyType", "test");
        });

        it("returns null when lazy loader returns null", () => {
            Config.lazyTypeLoader = vi.fn().mockReturnValue(null);
            const result = registry.tryGetOrLoad("LazyType");
            expect(result).toBeNull();
        });

        it("handles lazy loading with suffix", () => {
            Config.lazyTypeLoader = vi.fn().mockImplementation((key: string) => {
                return key === "LazyTypeType" ? TestType1 : null;
            });
            const result = registry.tryGetOrLoad("LazyType");
            expect(result).toBe(TestType1);
        });
    });

    describe("getOrLoad", () => {
        it("returns type when found synchronously", () => {
            const result = registry.getOrLoad("TestType1");
            expect(result).toBe(TestType1);
        });

        it("throws error when type not found and no lazy loader", () => {
            expect(() => registry.getOrLoad("NonExistentType")).toThrow("Type not found: NonExistentType");
        });

        it("returns promise from lazy loader", () => {
            Config.lazyTypeLoader = vi.fn().mockReturnValue(Promise.resolve(TestType1));
            const result = registry.getOrLoad("LazyType");
            expect(typeof result).toBe("object"); // Promise
        });
    });

    describe("reset", () => {
        it("clears the known types cache", () => {
            registry.tryGet("TestType1"); // Load into cache
            registry.reset();
            // After reset, should reinitialize
            const result = registry.tryGet("TestType1");
            expect(result).toBe(TestType1);
        });
    });

    describe("edge cases", () => {
        it("handles types without typeName", () => {
            vi.mocked(getGlobalTypeRegistry).mockReturnValue({});
            registry.reset();
            const result = registry.tryGet("anything");
            expect(result).toBeUndefined();
        });

        it("handles secondaryTypeKey function returning undefined", () => {
            class TestRegistry2 extends BaseTypeRegistry<any> {
                constructor() {
                    super({
                        loadKind: "test",
                        defaultSuffix: ""
                    });
                }

                protected override isMatchingType(type: any) {
                    return type === TestType1;
                }

                protected override loadError(key: string) {
                }
            }
            const registry2 = new TestRegistry2();
            const result = registry2.tryGet("TestType1");
            expect(result).toBe(TestType1);
        });

        it("handles multiple root namespaces", () => {
            Config.rootNamespaces = ["Test", "Another"];
            vi.mocked(getType).mockImplementation((key: string) => {
                if (key === "Another.TestType1") return TestType1;
                return null;
            });
            const result = registry.tryGet("TestType1");
            expect(result).toBe(TestType1);
        });
    });
});
