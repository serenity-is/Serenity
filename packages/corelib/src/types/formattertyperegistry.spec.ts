import { Config, ISlickFormatter, getTypeRegistry, isAssignableFrom, notifyError, registerClass } from "../base";
import { DialogTypeRegistry } from "./dialogtyperegistry";
import { FormatterTypeRegistry } from "./formattertyperegistry";

vi.mock("../base", async (importActual) => {
    const actual = await importActual<typeof import("../base")>();
    return {
        ...actual,
        isAssignableFrom: vi.fn(),
        notifyError: vi.fn(),
        Config: {
            rootNamespaces: [] as string[],
            lazyTypeLoader: null as any
        }
    };
});

// Mock formatter classes
class TestFormatter1 {
    static [Symbol.typeInfo] = { typeName: "TestFormatter1" };
    format(ctx: any): string {
        return "formatted1";
    }
}

class TestFormatter2 {
    static [Symbol.typeInfo] = { typeName: "TestFormatter2" };
    format(ctx: any): string {
        return "formatted2";
    }
}

class TestFormatterWithSuffix {
    static [Symbol.typeInfo] = { typeName: "TestFormatterWithSuffix" };
    format(ctx: any): string {
        return "formatted-suffix";
    }
}

class NonFormatterClass {
    static [Symbol.typeInfo] = { typeName: "NonFormatterClass" };
}

beforeEach(() => {
    Config.rootNamespaces.splice(0);
    const typeRegistry = getTypeRegistry();
    Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
    FormatterTypeRegistry.reset();

    // Clear all mocks
    vi.clearAllMocks();

    // Mock isAssignableFrom to return true for formatter classes
    Object.assign(typeRegistry, {
        TestFormatter1: TestFormatter1,
        TestFormatter2: TestFormatter2,
        TestFormatterWithSuffix: TestFormatterWithSuffix,
        NonFormatterClass: NonFormatterClass
    });

    // Mock isAssignableFrom to identify formatter types
    const mockIsAssignableFrom = vi.mocked(isAssignableFrom);
    mockIsAssignableFrom.mockImplementation((baseType: any, derivedType: any) => {
        return baseType === ISlickFormatter &&
            (derivedType === TestFormatter1 ||
                derivedType === TestFormatter2 ||
                derivedType === TestFormatterWithSuffix);
    });
});

describe("FormatterTypeRegistry", () => {
    it('can find formatter type by its key', function () {
        registerClass(TestFormatter1, 'MyProject.TestFormatter1');

        const type = FormatterTypeRegistry.tryGet("MyProject.TestFormatter1");
        expect(type).toBe(TestFormatter1);
    });

    it('can find formatter type by its full name', function () {
        registerClass(TestFormatter2, 'MyProject.MyModule.TestFormatter2');

        const type = FormatterTypeRegistry.tryGet("MyProject.MyModule.TestFormatter2");
        expect(type).toBe(TestFormatter2);
    });

    it('returns undefined for non-existent formatter', function () {
        const type = FormatterTypeRegistry.tryGet("NonExistent.Formatter");
        expect(type).toBeUndefined();
    });

    it('get throws error for non-existent formatter', function () {
        expect(() => FormatterTypeRegistry.get("NonExistent.Formatter")).toThrow();
    });

    it('reset clears the registry cache', function () {
        registerClass(TestFormatter1, 'Test.Formatter1');

        // First call should find it
        const type1 = FormatterTypeRegistry.tryGet("Test.Formatter1");
        expect(type1).toBe(TestFormatter1);

        // Reset should clear cache
        FormatterTypeRegistry.reset();

        // After reset, should still find it (since it's in the global registry)
        const type2 = FormatterTypeRegistry.tryGet("Test.Formatter1");
        expect(type2).toBe(TestFormatter1);
    });

    it('tryGetOrLoad returns formatter synchronously when found', function () {
        registerClass(TestFormatter1, 'Test.Formatter1');

        const type = FormatterTypeRegistry.tryGetOrLoad("Test.Formatter1");
        expect(type).toBe(TestFormatter1);
    });

    it('getOrLoad returns formatter when found', function () {
        registerClass(TestFormatter2, 'Test.Formatter2');

        const type = FormatterTypeRegistry.getOrLoad("Test.Formatter2");
        expect(type).toBe(TestFormatter2);
    });

    it('getOrLoad throws error for non-existent formatter', function () {
        expect(() => FormatterTypeRegistry.getOrLoad("Test.Formatter3")).toThrow();
    });

    it('works with multiple registered formatters', function () {
        registerClass(TestFormatter1, 'Test.Formatter1');
        registerClass(TestFormatter2, 'Test.Formatter2');

        const type1 = FormatterTypeRegistry.tryGet("Test.Formatter1");
        const type2 = FormatterTypeRegistry.tryGet("Test.Formatter2");

        expect(type1).toBe(TestFormatter1);
        expect(type2).toBe(TestFormatter2);
        expect(type1).not.toBe(type2);
    });

    it('loadError provides helpful error message for formatters', function () {
        const mockNotifyError = vi.mocked(notifyError);
        mockNotifyError.mockImplementation(() => { });

        expect(() => FormatterTypeRegistry.getOrLoad("Test.Formatter3")).toThrow(
            '"Test.Formatter3" formatter class not found!'
        );

        expect(mockNotifyError).toHaveBeenCalledWith(
            expect.stringContaining('formatter class not found'),
            '',
            { escapeHtml: false, timeOut: 5000 }
        );
    });

    it('handles formatter with no namespace', function () {
        registerClass(TestFormatter1, 'SimpleFormatter');

        const type = FormatterTypeRegistry.tryGet("SimpleFormatter");
        expect(type).toBe(TestFormatter1);
    });

    it('handles formatters with complex namespace paths', function () {
        registerClass(TestFormatter2, 'MyCompany.MyProject.MyModule.MyFormatter');

        const type = FormatterTypeRegistry.tryGet("MyCompany.MyProject.MyModule.MyFormatter");
        expect(type).toBe(TestFormatter2);
    });

    it('returns null for empty or null keys', function () {
        expect(FormatterTypeRegistry.tryGet("")).toBeNull();
        expect(FormatterTypeRegistry.tryGet(null as any)).toBeNull();
        expect(FormatterTypeRegistry.tryGet(undefined as any)).toBeNull();
    });

    it('getOrLoad throws with proper error message for empty key', function () {
        expect(() => FormatterTypeRegistry.getOrLoad("")).toThrow();
    });

    it('tryGetOrLoad returns undefined for empty key', function () {
        expect(FormatterTypeRegistry.tryGetOrLoad("")).toBeNull();
    });

    it('handles formatter names with Formatter suffix', function () {
        registerClass(TestFormatterWithSuffix, 'MyProject.TestFormatterWithSuffix');

        const type = FormatterTypeRegistry.tryGet("MyProject.TestFormatterWithSuffix");
        expect(type).toBe(TestFormatterWithSuffix);
    });

    it('works with formatters registered with different patterns', function () {
        registerClass(TestFormatter1, 'Formatter1');
        registerClass(TestFormatter2, 'TestFormatter2');

        const type1 = FormatterTypeRegistry.tryGet("Formatter1");
        const type2 = FormatterTypeRegistry.tryGet("TestFormatter2");

        expect(type1).toBe(TestFormatter1);
        expect(type2).toBe(TestFormatter2);
    });

    it('searches root namespaces for formatters', function () {
        Config.rootNamespaces.push("Test", "Serenity");

        // Register formatters with full names in root namespaces
        registerClass(TestFormatter1, 'Test.MyFormatter');
        registerClass(TestFormatter2, 'Serenity.MyFormatter');

        // Reset to pick up new config
        FormatterTypeRegistry.reset();

        // Should find formatters by short name when they exist in root namespaces
        const type1 = FormatterTypeRegistry.tryGet("MyFormatter");
        expect(type1).toBe(TestFormatter1); // Should find Test.MyFormatter first

        // Test with a different key
        registerClass(TestFormatterWithSuffix, 'Serenity.AnotherFormatter');
        FormatterTypeRegistry.reset();

        const type2 = FormatterTypeRegistry.tryGet("AnotherFormatter");
        expect(type2).toBe(TestFormatterWithSuffix);
    });

    it('can find type registered after initialization', function () {
        const typeRegistry = getTypeRegistry();
        Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
        registerClass(TestFormatter1, 'Test.MyFormatter1');
        const type1 = FormatterTypeRegistry.tryGet("Test.MyFormatter1");
        expect(type1).toBe(TestFormatter1);
        let type2 = FormatterTypeRegistry.tryGet("Test.MyFormatter2");
        expect(type2).toBeUndefined();
        registerClass(TestFormatter2, 'Test.MyFormatter2');
        type2 = FormatterTypeRegistry.tryGet("Test.MyFormatter2");
        expect(type2).toBe(TestFormatter2);
    });
});