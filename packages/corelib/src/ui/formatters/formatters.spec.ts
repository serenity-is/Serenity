
jest.mock("@serenity-is/corelib/q", () => ({
    ...jest.requireActual("@serenity-is/corelib/q"),
    tryGetText: jest.fn().mockImplementation((key: string) => key)
}));

import { EnumKeyAttribute, EnumTypeRegistry } from "../..";
import { BooleanFormatter, CheckboxFormatter, DateFormatter, DateTimeFormatter, EnumFormatter } from "./formatters";
import { addAttribute, registerEnum, tryGetText } from "@serenity-is/corelib/q"

describe("BooleanFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
    });

    it("shows true text from localizer if value is true and true text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.trueText = "trueText";
        expect(formatter.format({value: true, escape: (s) => s})).toBe("trueText");
    });

    it("shows false text from localizer if value is false and false text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.falseText = "falseText";
        expect(formatter.format({value: false, escape: (s) => s})).toBe("falseText");
    });

    it("shows Dialogs.YesButton text from localizer if value is true and true text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({value: true, escape: (s) => s})).toBe("Dialogs.YesButton");
    });

    it("shows Dialogs.NoButton text from localizer if value is false and false text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({value: false, escape: (s) => s})).toBe("Dialogs.NoButton");
    });

    it("shows Yes text from localizer if value is true and Dialogs.YesButton is null", () => {
        var formatter = new BooleanFormatter();
        (tryGetText as any).mockReturnValueOnce(null);
        (tryGetText as any).mockReturnValueOnce(null);
        expect(formatter.format({value: true, escape: (s) => s})).toBe("Yes");
    });

    it("shows No text from localizer if value is false and Dialogs.NoButton is null", () => {
        var formatter = new BooleanFormatter();
        (tryGetText as any).mockReturnValueOnce(null);
        (tryGetText as any).mockReturnValueOnce(null);
        expect(formatter.format({value: false, escape: (s) => s})).toBe("No");
    });
})

describe("CheckboxFormatter", () => {
    it("shows checked class if value is true", () => {
        var formatter = new CheckboxFormatter();
        expect(formatter.format({value: true, escape: (s) => s})).toContain("checked");
    })

    it("removes checked class if value is false", () => {
        var formatter = new CheckboxFormatter();
        expect(formatter.format({value: false, escape: (s) => s})).not.toContain("checked");
    })
})

describe("DateFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new DateFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
    });

    it("shows formatted date if value type is Date", () => {
        var formatter = new DateFormatter();
        var date = new Date(2023, 0, 1); // 01-01-2023
        expect(formatter.format({value: date, escape: (s) => s})).toBe("01/01/2023");
    });

    it("shows formatted date if value type string and it is a date", () => {
        var formatter = new DateFormatter();
        expect(formatter.format({value: "2023-01-01T00:00:00.000Z", escape: (s) => s})).toBe("01/01/2023");
    });

    it("shows given value if value type is string and it is not a date", () => {
        var formatter = new DateFormatter();
        expect(formatter.format({value: "this is not a date", escape: (s) => s})).toBe("this is not a date");
    });

    it("shows given value if value type is string and it is empty", () => {
        var formatter = new DateFormatter();
        expect(formatter.format({value: "", escape: (s) => s})).toBe("");
    });

    it("uses given display format", () => {
        var formatter = new DateFormatter();
        formatter.displayFormat = "dd-MM-yyyy";
        expect(formatter.format({value: "2023-01-01T00:00:00.000Z", escape: (s) => s})).toBe("01-01-2023");
    });
})

describe("DateTimeFormatter", () => {
    it("shows correctly formatted date time", () => {
        var formatter = new DateTimeFormatter();
        var date = new Date(2023, 0, 1, 12, 15, 18); // 01-01-2023 12:15:18
        expect(formatter.format({value: date, escape: (s) => s})).toBe("01/01/2023 12:15:18");
    });
})

enum TestEnum {
    Value1 = 1,
};

describe("EnumFormatter", () => {
    registerEnum(TestEnum, "TestEnum", "TestEnum");

    it("shows empty string if value is null", () => {
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
    });

    it("shows localized text of enum value", () => {
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        expect(formatter.format({value: 1, escape: (s) => s})).toBe("Enums.TestEnum.Value1");
    });

    it("uses attribute key instead of enum name", () => {
        var formatter = new EnumFormatter();
        formatter.enumKey = "TestEnum";
        addAttribute(TestEnum, new EnumKeyAttribute("TestEnum2"));
        expect(formatter.format({value: 1, escape: (s) => s})).toBe("Enums.TestEnum2.Value1");
    });

    it("returns name for give enumkey and value", () => {
        var value = EnumFormatter.getName(EnumTypeRegistry.get("TestEnum"), 1);
        expect(value).toBe("Value1");
    })
});