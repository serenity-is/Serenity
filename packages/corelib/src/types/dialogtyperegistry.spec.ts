import { Config, getTypeRegistry, isAssignableFrom, notifyError, registerClass } from "../base";
import { IDialog } from "../interfaces";
import { DialogTypeRegistry } from "./dialogtyperegistry";
import { EditorTypeRegistry } from "./editortyperegistry";

vi.mock("../base", async (importActual) => {
    const actual = await importActual<typeof import("../base")>();
    return {
        ...actual,
        isAssignableFrom: vi.fn(),
        notifyError: vi.fn(),
        Config: {
            rootNamespaces: [],
            lazyTypeLoader: null as any
        }
    };
});

// Mock dialog classes
class TestDialog1 implements IDialog {
    static [Symbol.typeInfo] = { typeName: "TestDialog1" };
    dialogOpen(asPanel?: boolean): void {
        // Mock implementation
    }
}

class TestDialog2 implements IDialog {
    static [Symbol.typeInfo] = { typeName: "TestDialog2" };
    dialogOpen(asPanel?: boolean): void {
        // Mock implementation
    }
}

class TestDialogWithSuffix implements IDialog {
    static [Symbol.typeInfo] = { typeName: "TestDialogWithSuffix" };
    dialogOpen(asPanel?: boolean): void {
        // Mock implementation
    }
}

class NonDialogClass {
    static [Symbol.typeInfo] = { typeName: "NonDialogClass" };
}

beforeEach(() => {
    Config.rootNamespaces.splice(0);
    const typeRegistry = getTypeRegistry();
    Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
    DialogTypeRegistry.reset();

    // Clear all mocks
    vi.clearAllMocks();

    // Mock isAssignableFrom to return true for dialog classes
    Object.assign(typeRegistry, {
        TestDialog1: TestDialog1,
        TestDialog2: TestDialog2,
        TestDialogWithSuffix: TestDialogWithSuffix,
        NonDialogClass: NonDialogClass
    });

    // Mock isAssignableFrom to identify dialog types
    const mockIsAssignableFrom = vi.mocked(isAssignableFrom);
    mockIsAssignableFrom.mockImplementation((baseType: any, derivedType: any) => {
        return baseType === IDialog &&
               (derivedType === TestDialog1 ||
                derivedType === TestDialog2 ||
                derivedType === TestDialogWithSuffix);
    });
});

describe("DialogTypeRegistry", () => {
    it('can find dialog type by its key', function () {
        registerClass(TestDialog1, 'MyProject.TestDialog1');

        const type = DialogTypeRegistry.tryGet("MyProject.TestDialog1");
        expect(type).toBe(TestDialog1);
    });

    it('can find dialog type by its full name', function () {
        registerClass(TestDialog2, 'MyProject.MyModule.TestDialog2');

        const type = DialogTypeRegistry.tryGet("MyProject.MyModule.TestDialog2");
        expect(type).toBe(TestDialog2);
    });

    it('returns undefined for non-existent dialog', function () {
        const type = DialogTypeRegistry.tryGet("NonExistent.Dialog");
        expect(type).toBeUndefined();
    });

    it('get throws error for non-existent dialog', function () {
        expect(() => DialogTypeRegistry.get("NonExistent.Dialog")).toThrow();
    });

    it('reset clears the registry cache', function () {
        registerClass(TestDialog1, 'Test.Dialog1');

        // First call should find it
        const type1 = DialogTypeRegistry.tryGet("Test.Dialog1");
        expect(type1).toBe(TestDialog1);

        // Reset should clear cache
        DialogTypeRegistry.reset();

        // After reset, should still find it (since it's in the global registry)
        const type2 = DialogTypeRegistry.tryGet("Test.Dialog1");
        expect(type2).toBe(TestDialog1);
    });

    it('tryGetOrLoad returns dialog synchronously when found', function () {
        registerClass(TestDialog1, 'Test.Dialog1');

        const type = DialogTypeRegistry.tryGetOrLoad("Test.Dialog1");
        expect(type).toBe(TestDialog1);
    });

    it('getOrLoad returns dialog when found', function () {
        registerClass(TestDialog2, 'Test.Dialog2');

        const type = DialogTypeRegistry.getOrLoad("Test.Dialog2");
        expect(type).toBe(TestDialog2);
    });

    it('getOrLoad throws error for non-existent dialog', function () {
        expect(() => DialogTypeRegistry.getOrLoad("NonExistent.Dialog")).toThrow();
    });

    it('works with multiple registered dialogs', function () {
        registerClass(TestDialog1, 'Test.Dialog1');
        registerClass(TestDialog2, 'Test.Dialog2');

        const type1 = DialogTypeRegistry.tryGet("Test.Dialog1");
        const type2 = DialogTypeRegistry.tryGet("Test.Dialog2");

        expect(type1).toBe(TestDialog1);
        expect(type2).toBe(TestDialog2);
        expect(type1).not.toBe(type2);
    });

    it('loadError provides helpful error message for dialogs', function () {
        const notifyErrorSpy = vi.fn();
        const mockNotifyError = vi.mocked(notifyError);
        mockNotifyError.mockImplementation(notifyErrorSpy);

        try {
            DialogTypeRegistry.get("Definitely.Not.Found.Dialog");
            expect(true).toBe(false); // Should not reach here
        } catch (e: any) {
            expect(e.message).toContain("dialog class not found");
            expect(e.message).toContain("Definitely.Not.Found.Dialog");
            expect(e.message).toContain("static [Symbol.typeInfo] = this.registerClass");
            expect(e.message).toContain("side-effect-import");
            expect(e.message).toContain("LookupEditor attribute");
            expect(e.message).toContain("node ./tsbuild.js");
        }

        // Should also call notifyError
        expect(notifyErrorSpy).toHaveBeenCalled();
    });

    it('handles dialog with no namespace', function () {
        registerClass(TestDialog1, 'SimpleDialog');

        const type = DialogTypeRegistry.tryGet("SimpleDialog");
        expect(type).toBe(TestDialog1);
    });

    it('handles dialogs with complex namespace paths', function () {
        registerClass(TestDialog2, 'MyCompany.MyProject.MyModule.MyDialog');

        const type = DialogTypeRegistry.tryGet("MyCompany.MyProject.MyModule.MyDialog");
        expect(type).toBe(TestDialog2);
    });

    it('returns null for empty or null keys', function () {
        expect(DialogTypeRegistry.tryGet("")).toBeNull();
        expect(DialogTypeRegistry.tryGet(null as any)).toBeNull();
        expect(DialogTypeRegistry.tryGet(undefined as any)).toBeNull();
    });

    it('getOrLoad throws with proper error message for empty key', function () {
        expect(() => DialogTypeRegistry.getOrLoad("")).toThrow();
    });

    it('tryGetOrLoad returns undefined for empty key', function () {
        const result = DialogTypeRegistry.tryGetOrLoad("");
        expect(result).toBeNull();
    });

    it('handles dialog names with Dialog suffix', function () {
        registerClass(TestDialogWithSuffix, 'MyProject.TestDialogWithSuffix');

        const type = DialogTypeRegistry.tryGet("MyProject.TestDialogWithSuffix");
        expect(type).toBe(TestDialogWithSuffix);
    });

    it('works with dialogs registered with different patterns', function () {
        // Register with full namespace
        registerClass(TestDialog1, 'Company.Project.Module.TestDialog1');
        // Register with shorter name
        registerClass(TestDialog2, 'TestDialog2');

        const type1 = DialogTypeRegistry.tryGet("Company.Project.Module.TestDialog1");
        const type2 = DialogTypeRegistry.tryGet("TestDialog2");

        expect(type1).toBe(TestDialog1);
        expect(type2).toBe(TestDialog2);
    });

    it('searches root namespaces for dialogs', function () {
        Config.rootNamespaces.push("Test", "Serenity");

        // Register formatters with full names in root namespaces
        registerClass(TestDialog1, 'Test.MyDialog');
        registerClass(TestDialog2, 'Serenity.MyDialog');

        // Reset to pick up new config
        DialogTypeRegistry.reset();

        // Should find dialogs by short name when they exist in root namespaces
        const type1 = DialogTypeRegistry.tryGet("MyDialog");
        expect(type1).toBe(TestDialog1); // Should find Test.MyDialog first

        // Test with a different key
        registerClass(TestDialogWithSuffix, 'Serenity.AnotherDialog');
        DialogTypeRegistry.reset();

        const type2 = DialogTypeRegistry.tryGet("AnotherDialog");
        expect(type2).toBe(TestDialogWithSuffix);
    });    

    it('can find type registered after initialization', function () {
        const typeRegistry = getTypeRegistry();
        Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
        registerClass(TestDialog1, 'Test.MyDialog1');
        const type1 = DialogTypeRegistry.tryGet("Test.MyDialog1");
        expect(type1).toBe(TestDialog1);
        let type2 = DialogTypeRegistry.tryGet("Test.MyDialog2");
        expect(type2).toBeUndefined();
        registerClass(TestDialog2, 'Test.MyDialog2');
        type2 = DialogTypeRegistry.tryGet("Test.MyDialog2");
        expect(type2).toBe(TestDialog2);
    });

});