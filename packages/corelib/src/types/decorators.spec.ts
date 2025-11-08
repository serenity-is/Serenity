import { Decorators } from "./decorators";
import { AdvancedFilteringAttribute, CloseButtonAttribute, ElementAttribute, FilterableAttribute, MaximizableAttribute, OptionAttribute, PanelAttribute, ResizableAttribute, StaticPanelAttribute } from "./attributes";
import { EditorAttribute, EnumKeyAttribute, addCustomAttribute, registerClass, registerEditor, registerEnum, registerFormatter, registerInterface, registerType } from "../base";
import { addTypeMember } from "../compat";

vi.mock("../base", async (importOriginal) => {
    const actual = await importOriginal() as any;
    return {
        ...actual,
        registerType: vi.fn(),
        registerClass: vi.fn(),
        registerInterface: vi.fn(),
        registerEditor: vi.fn(),
        registerEnum: vi.fn(),
        registerFormatter: vi.fn(),
        addCustomAttribute: vi.fn()
    };
});

vi.mock("../compat", async (importOriginal) => {
    const actual = await importOriginal() as any;
    return {
        ...actual,
        addTypeMember: vi.fn()
    };
});

describe("Decorators", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    describe("registerType", () => {
        it("should call registerType with the target", () => {
            class TestClass {
                static [Symbol.typeInfo] = {};
            }

            const decorator = Decorators.registerType();
            decorator(TestClass);

            expect(registerType).toHaveBeenCalledWith(TestClass);
        });
    });

    describe("registerClass", () => {
        it("should call registerClass with string name and interfaces", () => {
            class TestClass {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerClass("TestName", [Interface1, Interface2]);
            decorator(TestClass);

            expect(registerClass).toHaveBeenCalledWith(TestClass, "TestName", [Interface1, Interface2]);
        });

        it("should call registerClass with interfaces array when first param is array", () => {
            class TestClass {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerClass([Interface1, Interface2]);
            decorator(TestClass);

            expect(registerClass).toHaveBeenCalledWith(TestClass, null, [Interface1, Interface2]);
        });
    });

    describe("registerInterface", () => {
        it("should call registerInterface with string name and interfaces", () => {
            class TestInterface {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerInterface("TestName", [Interface1, Interface2]);
            decorator(TestInterface);

            expect(registerInterface).toHaveBeenCalledWith(TestInterface, "TestName", [Interface1, Interface2]);
        });

        it("should call registerInterface with interfaces array when first param is array", () => {
            class TestInterface {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerInterface([Interface1, Interface2]);
            decorator(TestInterface);

            expect(registerInterface).toHaveBeenCalledWith(TestInterface, null, [Interface1, Interface2]);
        });
    });

    describe("registerEditor", () => {
        it("should call registerEditor with string name and interfaces", () => {
            class TestEditor {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerEditor("TestName", [Interface1, Interface2]);
            decorator(TestEditor);

            expect(registerEditor).toHaveBeenCalledWith(TestEditor, "TestName", [Interface1, Interface2]);
        });

        it("should call registerEditor with interfaces array when first param is array", () => {
            class TestEditor {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerEditor([Interface1, Interface2]);
            decorator(TestEditor);

            expect(registerEditor).toHaveBeenCalledWith(TestEditor, null, [Interface1, Interface2]);
        });
    });

    describe("registerEnum", () => {
        it("should call registerEnum and add EnumKeyAttribute", () => {
            const TestEnum = {
                Value1: 1,
                Value2: 2
            };

            Decorators.registerEnum(TestEnum, "TestEnumKey", "TestEnumName");

            expect(registerEnum).toHaveBeenCalledWith(TestEnum, "TestEnumName", "TestEnumKey");
            expect(addCustomAttribute).toHaveBeenCalledWith(TestEnum, expect.any(EnumKeyAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe("TestEnumKey");
        });
    });

    describe("registerEnumType", () => {
        it("should call registerEnum with correct parameters", () => {
            const TestEnum = {
                Value1: 1,
                Value2: 2
            };

            Decorators.registerEnumType(TestEnum, "TestName", "TestKey");

            expect(registerEnum).toHaveBeenCalledWith(TestEnum, "TestName", "TestKey");
        });
    });

    describe("registerFormatter", () => {
        it("should call registerFormatter with string name and interfaces", () => {
            class TestFormatter {}
            class Interface1 {}
            class Interface2 {}

            const decorator = Decorators.registerFormatter("TestName", [Interface1, Interface2]);
            decorator(TestFormatter);

            expect(registerFormatter).toHaveBeenCalledWith(TestFormatter, "TestName", [Interface1, Interface2]);
        });

        it("should call registerFormatter with default interfaces when no params", () => {
            class TestFormatter {}

            const decorator = Decorators.registerFormatter();
            decorator(TestFormatter);

            expect(registerFormatter).toHaveBeenCalledWith(TestFormatter, null, expect.any(Array));
        });
    });

    describe("enumKey", () => {
        it("should add EnumKeyAttribute with the specified value", () => {
            class TestClass {}

            const decorator = Decorators.enumKey("TestKey");
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(EnumKeyAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe("TestKey");
        });
    });

    describe("option", () => {
        it("should add OptionAttribute for regular property", () => {
            class TestClass {}

            const decorator = Decorators.option();
            decorator(TestClass.prototype, "testProperty");

            expect(addTypeMember).toHaveBeenCalledWith(TestClass, {
                name: "testProperty",
                attr: [expect.any(OptionAttribute)],
                kind: expect.any(Number), // TypeMemberKind.field
                getter: null,
                setter: null
            });
        });

        it("should add OptionAttribute for getter/setter property", () => {
            class TestClass {}

            const decorator = Decorators.option();
            decorator(TestClass.prototype, "get_testProperty");

            expect(addTypeMember).toHaveBeenCalledWith(TestClass, {
                name: "testProperty",
                attr: [expect.any(OptionAttribute)],
                kind: expect.any(Number), // TypeMemberKind.property
                getter: "get_testProperty",
                setter: "set_testProperty"
            });
        });
    });

    describe("closeButton", () => {
        it("should add CloseButtonAttribute with default value", () => {
            class TestClass {}

            const decorator = Decorators.closeButton();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(CloseButtonAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(true);
        });

        it("should add CloseButtonAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.closeButton(false);
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(CloseButtonAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(false);
        });
    });

    describe("editor", () => {
        it("should add EditorAttribute", () => {
            class TestClass {}

            const decorator = Decorators.editor();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(EditorAttribute));
        });
    });

    describe("element", () => {
        it("should add ElementAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.element("div");
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(ElementAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe("div");
        });
    });

    describe("advancedFiltering", () => {
        it("should add AdvancedFilteringAttribute with default value", () => {
            class TestClass {}

            const decorator = Decorators.advancedFiltering();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(AdvancedFilteringAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(true);
        });

        it("should add FilterableAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.advancedFiltering(false);
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(FilterableAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(false);
        });
    });

    describe("maximizable", () => {
        it("should add MaximizableAttribute with default value", () => {
            class TestClass {}

            const decorator = Decorators.maximizable();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(MaximizableAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(true);
        });

        it("should add MaximizableAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.maximizable(false);
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(MaximizableAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(false);
        });
    });

    describe("panel", () => {
        it("should add PanelAttribute with default value", () => {
            class TestClass {}

            const decorator = Decorators.panel();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(PanelAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(true);
        });

        it("should add PanelAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.panel(false);
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(PanelAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(false);
        });
    });

    describe("resizable", () => {
        it("should add ResizableAttribute with default value", () => {
            class TestClass {}

            const decorator = Decorators.resizable();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(ResizableAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(true);
        });

        it("should add ResizableAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.resizable(false);
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(ResizableAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(false);
        });
    });

    describe("staticPanel", () => {
        it("should add StaticPanelAttribute with default value", () => {
            class TestClass {}

            const decorator = Decorators.staticPanel();
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(StaticPanelAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(true);
        });

        it("should add StaticPanelAttribute with specified value", () => {
            class TestClass {}

            const decorator = Decorators.staticPanel(false);
            decorator(TestClass);

            expect(addCustomAttribute).toHaveBeenCalledWith(TestClass, expect.any(StaticPanelAttribute));
            expect((addCustomAttribute as any).mock.calls[0][1].value).toBe(false);
        });
    });

    describe("responsive", () => {
        it("should do nothing (deprecated)", () => {
            class TestClass {}

            const decorator = (Decorators as any).responsive(true);
            decorator(TestClass);

            expect(addCustomAttribute).not.toHaveBeenCalled();
        });
    });
});