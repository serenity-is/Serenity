import { describe, expect, it, vi } from "vitest";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { ListRequestCapabilities, registerEnum } from "../../base";
import { EnumEditor } from "./enumeditor";

enum TestEnum {
    Option1 = 1,
    Option2 = 2,
    Option3 = 3
}

enum IconTestEnum {
    Option1 = 1,
    Option2 = 2
}

registerEnum(IconTestEnum, "Serenity.Tests.IconTestEnum", { enumIcons: { [IconTestEnum.Option2]: "sv-ph-chart-bar-duotone" } });

const WidgetCategory = {
    Charts: 'charts',
    Tables: 'tables'
} as const;

type WidgetCategory = typeof WidgetCategory[keyof typeof WidgetCategory];

registerEnum(WidgetCategory, "Serenity.Tests.WidgetCategory", {
    enumKey: 'My.WidgetCategory',
    enumIcons: { 'charts': 'sv-ph-chart-bar-duotone' }
});

enum TestFlags {
    None = 0,
    Read = 1,
    Write = 2,
    ReadWrite = 3,
    Delete = 4
}

registerEnum(TestFlags, "Serenity.Tests.TestFlags", { enumFlags: true });

describe("EnumEditor with a set of flags", () => {
    const property = { name: "Access" } as any;

    function create(props: any) {
        const element = document.createElement("input");
        document.body.append(element);
        return new EnumEditor({ element, ...props });
    }

    it("offers the members combining others too with the multiple option, but not none", () => {
        const editor = create({ enumType: TestFlags, flags: true, multiple: true });

        expect(editor["_items"].map((x: any) => x.id)).toEqual(["1", "2", "3", "4"]);
        editor.destroy();
    });

    it("shows a value with the fewest members that make it, the ones combining others first", () => {
        const editor = create({ enumType: TestFlags, flags: true, multiple: true });
        const target: any = {};

        editor.setEditValue({ Access: TestFlags.ReadWrite | TestFlags.Delete }, property);
        expect(editor.values).toEqual(["3", "4"]);

        // selecting a combined member along with one of its own bits doesn't change the value
        editor.values = ["1", "3"];
        editor.getEditValue(property, target);
        expect(target.Access).toBe(3);
        editor.destroy();
    });

    it("gives the members selected OR-ed together as the value, and null for none", () => {
        const editor = create({ enumType: TestFlags, flags: true, multiple: true });
        const target: any = {};

        editor.setEditValue({ Access: TestFlags.Read | TestFlags.Delete }, property);
        expect(editor.values).toEqual(["1", "4"]);
        editor.getEditValue(property, target);
        expect(target.Access).toBe(5);

        // a text is read as a number
        editor.setEditValue({ Access: "3" }, property);
        expect(editor.values).toEqual(["3"]);
        expect(editor.flagsValue).toBe(TestFlags.ReadWrite);

        editor.setEditValue({ Access: null }, property);
        editor.getEditValue(property, target);
        expect(target.Access).toBeNull();
        editor.destroy();
    });

    it("selects a single member of a set of flags without the multiple option, including the combined ones", () => {
        const editor = create({ enumType: TestFlags, flags: true });
        const target: any = {};

        editor.setEditValue({ Access: TestFlags.ReadWrite }, property);
        editor.getEditValue(property, target);

        expect(editor["isMultiple"]()).toBe(false);
        expect(editor["_items"]).toHaveLength(5);
        expect(target.Access).toBe("3");
        editor.destroy();
    });

    it("is a set of flags by the registration of the enum, unless told otherwise", () => {
        const byKey = create({ enumKey: "Serenity.Tests.TestFlags", multiple: true });
        const notFlags = create({ enumKey: "Serenity.Tests.TestFlags", multiple: true, flags: false });
        const target: any = {};

        byKey.setEditValue({ Access: 6 }, property);
        byKey.getEditValue(property, target);
        expect(target.Access).toBe(6);
        expect(byKey["_items"]).toHaveLength(4);

        // multiple values of an enum that is not a set of flags are a list, as before
        expect(notFlags["_items"]).toHaveLength(5);
        notFlags.setEditValue({ Access: ["1", "4"] }, property);
        notFlags.getEditValue(property, target);
        expect(target.Access).toEqual(["1", "4"]);
        byKey.destroy();
        notFlags.destroy();
    });

    it("knows the capabilities of a list request as a set of flags, e.g. for the options of a service lookup editor", () => {
        const editor = create({ enumKey: "Serenity.Services.ListRequestCapabilities", multiple: true });
        const target: any = {};

        editor.setEditValue({ Capabilities: 1 | 16 }, { name: "Capabilities" } as any);
        editor.getEditValue({ name: "Capabilities" } as any, target);

        expect(target.Capabilities).toBe(17);
        expect(editor["_items"].map((x: any) => x.id)).not.toContain("0");
        // a member combining others is offered by its own name, not by the ones it is made of
        expect(editor["_items"].find((x: any) => x.id === String(ListRequestCapabilities.SourceAdapter)).text).toBe("SourceAdapter");

        // e.g. what the in-memory source adapter handles, along with the column selection
        editor.setEditValue({ Capabilities: ListRequestCapabilities.SourceAdapter | ListRequestCapabilities.ColumnSelection },
            { name: "Capabilities" } as any);
        expect(editor.values).toEqual([String(ListRequestCapabilities.ColumnSelection), String(ListRequestCapabilities.SourceAdapter)]);
        editor.destroy();
    });
});

describe("EnumEditor", () => {
    it("builds options from enumType", () => {
        const editor = new EnumEditor({ enumType: TestEnum } as any);
        const items = editor["_items"];
        expect(items.length).toBe(3);
        expect(items.map((x: any) => x.id)).toEqual(["1", "2", "3"]);
        editor.destroy();
    });

    it("uses the registered icon classes of the enum values", () => {
        const editor = new EnumEditor({ enumType: IconTestEnum } as any);
        const items = editor["_items"];
        expect(items.map((x: any) => x.icon)).toEqual([undefined, "sv-ph-chart-bar-duotone"]);
        editor.destroy();
    });

    it("allowClear defaults to true", () => {
        const editor = new EnumEditor({ enumType: TestEnum } as any);
        expect((editor as any).allowClear()).toBe(true);
        editor.destroy();
    });

    it("allowClear reflects option", () => {
        const editor = new EnumEditor({ enumType: TestEnum, allowClear: false } as any);
        expect((editor as any).allowClear()).toBe(false);
        editor.destroy();
    });

    it("does not throw when the enum type cannot be resolved", () => {
        const getOrLoad = vi.spyOn(EnumTypeRegistry, "getOrLoad").mockReturnValue(undefined as any);
        try {
            const editor = new EnumEditor({} as any);
            expect(editor["_items"]).toEqual([]);
            editor.destroy();
        }
        finally {
            getOrLoad.mockRestore();
        }
    });

    it("builds options from a string enum", () => {
        const editor = new EnumEditor({ enumType: WidgetCategory } as any);
        const items = editor["_items"];
        expect(items.map((x: any) => x.id)).toEqual(["charts", "tables"]);
        editor.destroy();
    });

    it("uses the icon classes of a string enum", () => {
        const editor = new EnumEditor({ enumType: WidgetCategory } as any);
        const items = editor["_items"];
        expect(items.map((x: any) => x.icon)).toEqual(["sv-ph-chart-bar-duotone", undefined]);
        editor.destroy();
    });

    it("loads a string enum by its key", () => {
        const editor = new EnumEditor({ enumKey: 'My.WidgetCategory' } as any);
        const items = editor["_items"];
        expect(items.map((x: any) => x.id)).toEqual(["charts", "tables"]);
        editor.destroy();
    });
});
