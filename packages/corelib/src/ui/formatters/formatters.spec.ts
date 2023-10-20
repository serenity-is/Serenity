
jest.mock("../../q", () => ({
    ...jest.requireActual("../../q"),
    tryGetText: jest.fn().mockImplementation((key: string) => key)
}));

import { EnumKeyAttribute, EnumTypeRegistry } from "../..";
import { BooleanFormatter, CheckboxFormatter, DateFormatter, DateTimeFormatter, EnumFormatter, FileDownloadFormatter, MinuteFormatter, NumberFormatter, UrlFormatter } from "./formatters";
import { addAttribute, registerEnum, tryGetText } from "../../q"

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

describe("FileDownloadFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
    });


    it("replaces all backward slashes to foward", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format({value: "file\\with\\backward\\slashes", escape: (s) => s})).toContain("file/with/backward/slashes");
    });

    it("shows empty string if fileOriginalName is not specified", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format({value: "file", escape: (s) => s}).replace(/\s/g, "")).toContain("</i></a>");
    });

    it("shows empty string if fileOriginalName is specified but not found", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        expect(formatter.format({value: "file", item: {}, escape: (s) => s}).replace(/\s/g, "")).toContain("</i></a>");
    });

    it("shows fileOriginalName if fileOriginalName is specified and found", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        expect(formatter.format({value: "file", item: {fileOriginalName: "test"}, escape: (s) => s})).toContain("</i> test</a>");
    });

    it("uses displayFormat if displayFormat is specified", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        formatter.displayFormat = "originalName: {0} dbFile: {1} downloadUrl: {2}"
        expect(formatter.format({value: "file", item: {fileOriginalName: "test"}, escape: (s) => s}))
            .toContain("</i> originalName: test dbFile: file downloadUrl: /upload/file</a>");
    });

    it("uses icon if specified", () => {
        var formatter = new FileDownloadFormatter();
        formatter.iconClass = "testicon"
        expect(formatter.format({value: "file", item: {fileOriginalName: "test"}, escape: (s) => s}))
            .toContain("'testicon'");
    });

    it("adds fa if icon starts with fa", () => {
        var formatter = new FileDownloadFormatter();
        formatter.iconClass = "fa-testicon"
        expect(formatter.format({value: "file", item: {fileOriginalName: "test"}, escape: (s) => s}))
            .toContain("'fa fa-testicon'");
    });

    it("uses default icon if not specified", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format({value: "file", item: {fileOriginalName: "test"}, escape: (s) => s}))
            .toContain("'fa fa-download'");
    });

    it("adds originalNameProperty to referencedFields if specified", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        var column = {};
        formatter.initializeColumn(column);
        expect(column).toEqual({referencedFields: ["fileOriginalName"]});
    })
});

describe("MinuteFormatter", () => {

    it("shows empty string if value is null", () => {
        var formatter = new MinuteFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
        expect(formatter.format({value: NaN, escape: (s) => s})).toBe("");
    })

    it("shows correctly formatted minute", () => {
        var formatter = new MinuteFormatter();

        expect(formatter.format({value: 0, escape: (s) => s})).toBe("00:00");
        expect(formatter.format({value: 12, escape: (s) => s})).toBe("00:12");
        expect(formatter.format({value: 72, escape: (s) => s})).toBe("01:12");
        expect(formatter.format({value: 680, escape: (s) => s})).toBe("11:20");
        expect(formatter.format({value: 1360, escape: (s) => s})).toBe("22:40");
    })
})

describe("NumberFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
        expect(formatter.format({value: NaN, escape: (s) => s})).toBe("");
    });

    it("shows formatted number if value type is number", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format({value: 123456.789, escape: (s) => s})).toBe("123456.79");
    });

    it("parses shows formatted number if value type is string", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format({value: "123456.789", escape: (s) => s})).toBe("123456.79");
    });

    it("shows given string value if value type is string and it is not a number", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format({value: "this is not a number", escape: (s) => s})).toBe("this is not a number");
    });

    it("uses given numberformat", () => {
        var formatter = new NumberFormatter();
        formatter.displayFormat = "0.###"
        expect(formatter.format({value: "123456.789", escape: (s) => s})).toBe("123456.789");
    });
})

describe("UrlFormatter", () => {
    it("shows empty string if value is null or empty", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
        expect(formatter.format({value: "", escape: (s) => s})).toBe("");
    })

    it("shows empty string if urlProperty value is null or empty", () => {
        var formatter = new UrlFormatter();
        formatter.urlProperty = "url";
        expect(formatter.format({value: null, item: {url: null}, escape: (s) => s})).toBe("");
        expect(formatter.format({value: null, item: {url: ""}, escape: (s) => s})).toBe("");
    })

    it("shows link if url is specified", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format({value: "test", escape: (s) => s})).toContain("'test'");
    })

    it("formats url if format is specified", () => {
        var formatter = new UrlFormatter();
        formatter.urlFormat = "test/{0}";
        expect(formatter.format({value: "test", escape: (s) => s})).toContain("'test/test'");
    })

    it("resolves url if it starts with tilda", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format({value: "~/test", escape: (s) => s})).toContain("'/test'");
    })

    it("uses display format property for showing if specified", () => {
        var formatter = new UrlFormatter();
        formatter.displayFormat = "displayFormat {0}"
        expect(formatter.format({value: "~/test", escape: (s) => s})).toContain("displayFormat ~/test");
    })

    it("adds target if specified", () => {
        var formatter = new UrlFormatter();
        formatter.target = "_blank"
        expect(formatter.format({value: "~/test", escape: (s) => s})).toContain("target='_blank'");
    })

    it("adds diplayProperty and UrlProperty to referencedFields if specified", () => {
        var formatter = new UrlFormatter();
        formatter.displayProperty = "display";
        formatter.urlProperty = "url";
        var column = {};
        formatter.initializeColumn(column);
        expect(column).toEqual({referencedFields: ["display", "url"]});
    });
})