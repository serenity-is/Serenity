import { Config, getGlobalTypeRegistry, hasCustomAttribute, isAssignableFrom, notifyError, registerClass } from "../base";
import { Widget } from "../ui/widgets/widget";
import { EditorTypeRegistry } from "./editortyperegistry";

vi.mock("../base", async (importActual) => {
    const actual = await importActual<typeof import("../base")>();
    return {
        ...actual,
        hasCustomAttribute: vi.fn(),
        isAssignableFrom: vi.fn(),
        notifyError: vi.fn(),
        Config: {
            rootNamespaces: [],
            lazyTypeLoader: null as any
        }
    };
});

// Mock editor classes
class TestEditor1 extends Widget {
    static override [Symbol.typeInfo] = { typeName: "TestEditor1", typeKind: "class" as const };
}

class TestEditor2 extends Widget {
    static override [Symbol.typeInfo] = { typeName: "TestEditor2", typeKind: "class" as const };
}

class TestEditorWithSuffix extends Widget {
    static override [Symbol.typeInfo] = { typeName: "TestEditorWithSuffix", typeKind: "class" as const };
}

class NonEditorClass {
    static [Symbol.typeInfo] = { typeName: "NonEditorClass", typeKind: "class" as const };
}

beforeEach(() => {
    Config.rootNamespaces.splice(0);
    const typeRegistry = getGlobalTypeRegistry();
    Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
    EditorTypeRegistry.reset();

    // Clear all mocks
    vi.clearAllMocks();

    // Mock isAssignableFrom to return true for editor classes
    Object.assign(typeRegistry, {
        TestEditor1: TestEditor1,
        TestEditor2: TestEditor2,
        TestEditorWithSuffix: TestEditorWithSuffix,
        NonEditorClass: NonEditorClass
    });

    // Mock hasCustomAttribute and isAssignableFrom to identify editor types
    const mockHasCustomAttribute = vi.mocked(hasCustomAttribute);
    const mockIsAssignableFrom = vi.mocked(isAssignableFrom);
    mockHasCustomAttribute.mockReturnValue(false); // Default to false
    mockIsAssignableFrom.mockImplementation((baseType: any, derivedType: any) => {
        return baseType === Widget &&
            (derivedType === TestEditor1 ||
                derivedType === TestEditor2 ||
                derivedType === TestEditorWithSuffix);
    });
});

describe("EditorTypeRegistry", () => {
    it('can find editor type by its key', function () {
        registerClass(TestEditor1, 'MyProject.TestEditor1');

        const type = EditorTypeRegistry.tryGet("MyProject.TestEditor1");
        expect(type).toBe(TestEditor1);
    });

    it('can find editor type by its full name', function () {
        registerClass(TestEditor2, 'MyProject.MyModule.TestEditor2');

        const type = EditorTypeRegistry.tryGet("MyProject.MyModule.TestEditor2");
        expect(type).toBe(TestEditor2);
    });

    it('returns undefined for non-existent editor', function () {
        const type = EditorTypeRegistry.tryGet("NonExistent.Editor");
        expect(type).toBeUndefined();
    });

    it('get throws error for non-existent editor', function () {
        expect(() => EditorTypeRegistry.get("NonExistent.Editor")).toThrow();
    });

    it('reset clears the registry cache', function () {
        registerClass(TestEditor1, 'Test.Editor1');

        // First call should find it
        const type1 = EditorTypeRegistry.tryGet("Test.Editor1");
        expect(type1).toBe(TestEditor1);

        // Reset should clear cache
        EditorTypeRegistry.reset();

        // After reset, should still find it (since it's in the global registry)
        const type2 = EditorTypeRegistry.tryGet("Test.Editor1");
        expect(type2).toBe(TestEditor1);
    });

    it('tryGetOrLoad returns editor synchronously when found', function () {
        registerClass(TestEditor1, 'Test.Editor1');

        const type = EditorTypeRegistry.tryGetOrLoad("Test.Editor1");
        expect(type).toBe(TestEditor1);
    });

    it('getOrLoad returns editor when found', function () {
        registerClass(TestEditor2, 'Test.Editor2');

        const type = EditorTypeRegistry.getOrLoad("Test.Editor2");
        expect(type).toBe(TestEditor2);
    });

    it('getOrLoad throws error for non-existent editor', function () {
        expect(() => EditorTypeRegistry.getOrLoad("Test.Editor3")).toThrow();
    });

    it('works with multiple registered editors', function () {
        registerClass(TestEditor1, 'Test.Editor1');
        registerClass(TestEditor2, 'Test.Editor2');

        const type1 = EditorTypeRegistry.tryGet("Test.Editor1");
        const type2 = EditorTypeRegistry.tryGet("Test.Editor2");

        expect(type1).toBe(TestEditor1);
        expect(type2).toBe(TestEditor2);
        expect(type1).not.toBe(type2);
    });

    it('loadError provides helpful error message for editors', function () {
        const mockNotifyError = vi.mocked(notifyError);
        mockNotifyError.mockImplementation(() => { });

        expect(() => EditorTypeRegistry.getOrLoad("Test.Editor3")).toThrow(
            'The editor class "Test.Editor3" was not found'
        );

        expect(mockNotifyError).toHaveBeenCalledWith(
            expect.stringContaining('The editor class "Test.Editor3" was not found'),
            '',
            { preWrap: true, timeOut: 5000 }
        );
    });

    it('handles editor with no namespace', function () {
        registerClass(TestEditor1, 'SimpleEditor');

        const type = EditorTypeRegistry.tryGet("SimpleEditor");
        expect(type).toBe(TestEditor1);
    });

    it('handles editors with complex namespace paths', function () {
        registerClass(TestEditor2, 'MyCompany.MyProject.MyModule.MyEditor');

        const type = EditorTypeRegistry.tryGet("MyCompany.MyProject.MyModule.MyEditor");
        expect(type).toBe(TestEditor2);
    });

    it('returns null for empty or null keys', function () {
        expect(EditorTypeRegistry.tryGet("")).toBeNull();
        expect(EditorTypeRegistry.tryGet(null as any)).toBeNull();
        expect(EditorTypeRegistry.tryGet(undefined as any)).toBeNull();
    });

    it('getOrLoad throws with proper error message for empty key', function () {
        expect(() => EditorTypeRegistry.getOrLoad("")).toThrow();
    });

    it('tryGetOrLoad returns null for empty key', function () {
        expect(EditorTypeRegistry.tryGetOrLoad("")).toBeNull();
    });

    it('handles editor names with Editor suffix', function () {
        registerClass(TestEditorWithSuffix, 'MyProject.TestEditorWithSuffix');

        const type = EditorTypeRegistry.tryGet("MyProject.TestEditorWithSuffix");
        expect(type).toBe(TestEditorWithSuffix);
    });

    it('works with editors registered with different patterns', function () {
        registerClass(TestEditor1, 'Editor1');
        registerClass(TestEditor2, 'TestEditor2');

        const type1 = EditorTypeRegistry.tryGet("Editor1");
        const type2 = EditorTypeRegistry.tryGet("TestEditor2");

        expect(type1).toBe(TestEditor1);
        expect(type2).toBe(TestEditor2);
    });

    it('searches root namespaces for editors', function () {
        // Store original root namespaces
        const originalNamespaces = [...Config.rootNamespaces];

        // Set up root namespaces
        Config.rootNamespaces.length = 0;
        Config.rootNamespaces.push("Test", "Serenity");

        // Register editors with full names in root namespaces
        registerClass(TestEditor1, 'Test.MyEditor');
        registerClass(TestEditor2, 'Serenity.MyEditor');

        // Reset to pick up new config
        EditorTypeRegistry.reset();

        // Should find editors by short name when they exist in root namespaces
        const type1 = EditorTypeRegistry.tryGet("MyEditor");
        expect(type1).toBe(TestEditor1); // Should find Test.MyEditor first

        // Test with a different key
        registerClass(TestEditorWithSuffix, 'Serenity.AnotherEditor');
        EditorTypeRegistry.reset();

        const type2 = EditorTypeRegistry.tryGet("AnotherEditor");
        expect(type2).toBe(TestEditorWithSuffix);

        // Restore original config
        Config.rootNamespaces.length = 0;
        Config.rootNamespaces.push(...originalNamespaces);
    });

    it('can find type registered after initialization', function () {
        const typeRegistry = getGlobalTypeRegistry();
        Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
        registerClass(TestEditor1, 'Test.MyEditor1');
        const type1 = EditorTypeRegistry.tryGet("Test.MyEditor1");
        expect(type1).toBe(TestEditor1);
        let type2 = EditorTypeRegistry.tryGet("Test.MyEditor2");
        expect(type2).toBeUndefined();
        registerClass(TestEditor2, 'Test.MyEditor2');
        type2 = EditorTypeRegistry.tryGet("Test.MyEditor2");
        expect(type2).toBe(TestEditor2);
    });
});