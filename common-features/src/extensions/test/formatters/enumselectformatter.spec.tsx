import { registerEnum } from "@serenity-is/corelib";
import { EnumTypeRegistry } from "@serenity-is/corelib";
import { EnumSelectFormatter } from "../../Modules/Formatters/EnumSelectFormatter";

enum TestEnum {
    Value1 = 1,
    Value2 = 2
}

describe("EnumSelectFormatter", () => {
    beforeEach(() => {
        registerEnum(TestEnum, "My.TestEnum", "TestEnum");
    });

    it("has allowClear true by default", () => {
        const formatter = new EnumSelectFormatter();
        expect(formatter.allowClear).toBe(true);
        expect(formatter.props.allowClear).toBe(true);
    });

    it("renders an option for each enum value and marks selected", () => {
        const select = new EnumSelectFormatter({ enumKey: "TestEnum" }).format({ value: 2 } as any) as HTMLElement;
        expect(select.tagName).toBe("SELECT");
        const options = Array.from(select.querySelectorAll("option"));
        expect(options.length).toBe(3);
        expect(options[0].value).toBe("");
        expect(options[1].value).toBe("1");
        expect(options[2].value).toBe("2");
        expect(options[2].selected).toBe(true);
    });

    it("omits clear option when allowClear is false", () => {
        const select = new EnumSelectFormatter({ enumKey: "TestEnum", allowClear: false }).format({ value: 1 } as any) as HTMLElement;
        const options = Array.from(select.querySelectorAll("option"));
        expect(options.length).toBe(2);
        expect(options[0].value).toBe("1");
    });

    it("uses custom empty item text", () => {
        const select = new EnumSelectFormatter({ enumKey: "TestEnum", emptyItemText: "Pick" }).format({ value: 1 } as any) as HTMLElement;
        expect(select.querySelector("option").textContent).toBe("Pick");
    });

    it("supports property accessors", () => {
        const formatter = new EnumSelectFormatter();
        formatter.enumKey = "TestEnum";
        formatter.allowClear = false;
        formatter.emptyItemText = "Choose";
        expect(formatter.enumKey).toBe("TestEnum");
        expect(formatter.allowClear).toBe(false);
        expect(formatter.emptyItemText).toBe("Choose");
    });

    it("throws when enum key is not specified", () => {
        expect(() => new EnumSelectFormatter().format({ value: 1 } as any)).toThrow();
    });
});
