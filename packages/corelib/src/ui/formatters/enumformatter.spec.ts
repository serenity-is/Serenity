import { addCustomAttribute, registerEnum } from "../../base";
import { EnumKeyAttribute } from "../../types/attributes";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { EnumFormatter } from "./enumformatter";

vi.mock(import("../../base"), async (importOriginal) => {
    const actual = await importOriginal();
    return {
        ...actual,
        tryGetText: vi.fn().mockImplementation((key: string) => key)
    }
});

beforeEach(() => {
    vi.clearAllMocks();
    EnumTypeRegistry.reset();
});

describe("EnumFormatter", () => {

    it("shows empty string if value is null", () => {
        enum TestEnum {
            Value1 = 1
        };
        registerEnum(TestEnum, "TestEnum", "TestEnum");
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format({ value: null, escape: (s) => s })).toBe("");
    });

    it("shows localized text of enum value", () => {
        enum TestEnum {
            Value1 = 1
        };
        registerEnum(TestEnum, "TestEnum", "TestEnum");
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format({ value: 1, escape: (s) => s })).toBe("Enums.TestEnum.Value1");
    });

    it("uses attribute key instead of enum name", () => {
        enum TestEnum {
            Value1 = 1
        };
        addCustomAttribute(TestEnum, new EnumKeyAttribute("TestEnum2"));
        registerEnum(TestEnum, "TestEnum");
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format({ value: 1, escape: (s) => s })).toBe("Enums.TestEnum2.Value1");
    });

    it("returns name for give enumkey and value", () => {
        enum TestEnum {
            Value1 = 1
        };
        registerEnum(TestEnum, "TestEnum", "TestEnum");
        var value = EnumFormatter.getName(EnumTypeRegistry.get("TestEnum"), 1);
        expect(value).toBe("Value1");
    })
});