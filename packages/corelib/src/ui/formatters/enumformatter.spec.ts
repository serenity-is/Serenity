import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { addCustomAttribute, EnumKeyAttribute, registerEnum } from "../../base";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { EnumFormatter } from "./enumformatter";

vi.mock(import("../../base"), async (importOriginal) => {
    const actual = await importOriginal();
    return {
        ...actual,
        tryGetText: vi.fn().mockImplementation((key: string) => key),
        localText: vi.fn().mockImplementation((key: string) => key)
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
        expect(formatter.format(ctx({ value: null }))).toBe("");
    });

    it("shows localized text of enum value", () => {
        enum TestEnum {
            Value1 = 1
        };
        registerEnum(TestEnum, "TestEnum", "TestEnum");
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format(ctx({ value: 1 }))).toBe("Enums.TestEnum.Value1");
    });

    it("uses attribute key instead of enum name", () => {
        enum TestEnum {
            Value1 = 1
        };
        addCustomAttribute(TestEnum, new EnumKeyAttribute("TestEnum2"));
        registerEnum(TestEnum, "TestEnum");
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format(ctx({ value: 1 }))).toBe("Enums.TestEnum2.Value1");
    });

    it("returns name for give enumkey and value", () => {
        enum TestEnum {
            Value1 = 1
        };
        registerEnum(TestEnum, "TestEnum", "TestEnum");
        var value = EnumFormatter.getName(EnumTypeRegistry.get("TestEnum"), 1);
        expect(value).toBe("Value1");
    })

    it("handles async enum type loading", async () => {
        enum TestEnum {
            Value1 = 1
        };
        registerEnum(TestEnum, "AsyncEnum", "AsyncEnum");
        const getOrLoad = vi.spyOn(EnumTypeRegistry, "getOrLoad").mockReturnValue(Promise.resolve(TestEnum));
        const formatter = new EnumFormatter();
        formatter.enumKey = "AsyncEnum";
        const container = document.createElement("div");
        const node = formatter.format(ctx({ value: 1 })) as HTMLElement;
        container.appendChild(node);
        expect(node.tagName).toBe("SPAN");
        await Promise.resolve();
        expect(node.parentElement).toBeNull();
        expect(container.firstChild?.nodeType).toBe(Node.TEXT_NODE);
        expect(container.textContent.length).toBeGreaterThan(0);
        getOrLoad.mockRestore();
    });

    it("returns empty text for an empty enum name", () => {
        expect(EnumFormatter.getText("Some.Key", "")).toBe("");
    });

    it("returns empty name for a null value", () => {
        enum TestEnum {
            Value1 = 1
        };
        expect(EnumFormatter.getName(TestEnum, null)).toBe("");
    });
});