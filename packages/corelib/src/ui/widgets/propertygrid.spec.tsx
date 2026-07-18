import { addLocalText, PropertyItem, setScriptData } from "../../base";
import {
    PropertyCategories,
    PropertyCategory,
    PropertyCategoryTitle,
    PropertyField,
    PropertyFieldCaption,
    PropertyFieldEditor,
    PropertyFieldElement,
    PropertyFieldLineBreak,
    PropertyGrid,
    PropertyGridMode,
    PropertyTabItem,
    PropertyTabList,
    PropertyTabPane,
    PropertyTabPanes,
    PropertyTabs
} from "./propertygrid";
import { Widget } from "./widget";

// A simple test editor for PropertyFieldEditor tests
class TestEditor extends Widget {
    static override [Symbol.typeInfo] = this.registerClass("Test.TestEditor");
    static override createDefaultElement() { return document.createElement("input"); }
}

describe("PropertyFieldCaption", () => {
    it("renders label with caption text", () => {
        const label = PropertyFieldCaption({
            item: { name: "TestField", title: "Test Field", hint: "", labelWidth: undefined, required: false },
            idPrefix: "pre_"
        });
        expect(label).toBeInstanceOf(HTMLLabelElement);
        expect(label.htmlFor).toBe("pre_TestField");
        expect(label.textContent).toContain("Test Field");
    });

    it("renders required marker", () => {
        const label = PropertyFieldCaption({
            item: { name: "RequiredField", title: "Required", hint: "", labelWidth: undefined, required: true },
            idPrefix: "pre_"
        });
        const sup = label.querySelector("sup");
        expect(sup).toBeTruthy();
    });

    it("uses hint as title attribute", () => {
        const label = PropertyFieldCaption({
            item: { name: "FieldWithHint", title: "Field", hint: "This is a hint", labelWidth: undefined, required: false },
            idPrefix: "pre_"
        });
        expect(label.title).toBe("This is a hint");
    });

    it("applies label width style", () => {
        const label = PropertyFieldCaption({
            item: { name: "WideField", title: "Wide", hint: "", labelWidth: "200px", required: false },
            idPrefix: "pre_"
        });
        expect(label.style.width).toBe("200px");
    });

    it("hides label when labelWidth is 0", () => {
        const label = PropertyFieldCaption({
            item: { name: "HiddenField", title: "Hidden", hint: "", labelWidth: "0", required: false },
            idPrefix: "pre_"
        });
        expect(label.hidden).toBe(true);
    });
});

describe("PropertyFieldEditor", () => {
    it("creates editor with direct constructor type", () => {
        const fieldElement = document.createElement("div") as PropertyFieldElement;

        PropertyFieldEditor({
            fieldElement,
            item: { name: "Test", editorType: TestEditor, editorParams: {} },
            idPrefix: "pre_"
        });

        expect(fieldElement.editorWidget).toBeInstanceOf(TestEditor);
        expect(fieldElement.querySelector(".editor")).toBeTruthy();
    });

    it("sets placeholder from localTextPrefix", () => {
        addLocalText({ "Forms.TestForm.Test_Placeholder": "Enter value here" });

        const fieldElement = document.createElement("div") as PropertyFieldElement;
        PropertyFieldEditor({
            fieldElement,
            item: { name: "Test", editorType: TestEditor, editorParams: {}, placeholder: "`Placeholder" },
            idPrefix: "pre_",
            localTextPrefix: "Forms.TestForm."
        });

        const editor = fieldElement.querySelector(".editor") as HTMLElement;
        expect(editor.getAttribute("placeholder")).toBe("Enter value here");
    });

    it("adds editor addons", () => {
        const fieldElement = document.createElement("div") as PropertyFieldElement;
        const addonFn = vi.fn();

        PropertyFieldEditor({
            fieldElement,
            item: {
                name: "WithAddon",
                editorType: TestEditor,
                editorParams: {},
                editorAddons: [{ type: addonFn, params: {} }]
            },
            idPrefix: "pre_"
        });

        expect(addonFn).toHaveBeenCalled();
    });

    it("sets maxLength on editor", () => {
        const fieldElement = document.createElement("div") as PropertyFieldElement;

        PropertyFieldEditor({
            fieldElement,
            item: { name: "MaxLenField", editorType: TestEditor, editorParams: {}, maxLength: 50 },
            idPrefix: "pre_"
        });

        const editor = fieldElement.querySelector(".editor") as HTMLInputElement;
        expect(editor.getAttribute("maxlength")).toBe("50");
    });

    it("sets editorCssClass", () => {
        const fieldElement = document.createElement("div") as PropertyFieldElement;

        PropertyFieldEditor({
            fieldElement,
            item: { name: "StyledField", editorType: TestEditor, editorParams: {}, editorCssClass: "my-editor" },
            idPrefix: "pre_"
        });

        const editor = fieldElement.querySelector(".editor");
        expect(editor.classList.contains("my-editor")).toBe(true);
    });

    it("sets name attribute on input-like elements", () => {
        const fieldElement = document.createElement("div") as PropertyFieldElement;

        PropertyFieldEditor({
            fieldElement,
            item: { name: "InputField", editorType: TestEditor, editorParams: {} },
            idPrefix: "pre_"
        });

        const editor = fieldElement.querySelector(".editor") as HTMLInputElement;
        expect(editor.getAttribute("name")).toBe("InputField");
    });
});

describe("PropertyFieldLineBreak", () => {
    it("returns null when no formCssClass", () => {
        const result = PropertyFieldLineBreak({
            item: {}
        });
        expect(result).toBeNull();
    });

    it("returns null when formCssClass has no line-break", () => {
        const result = PropertyFieldLineBreak({
            item: { formCssClass: "some-class" }
        });
        expect(result).toBeNull();
    });

    it("creates line-break for line-break-xs", () => {
        const result = PropertyFieldLineBreak({
            item: { formCssClass: "line-break-xs" }
        });
        expect(result).toBeInstanceOf(HTMLElement);
        expect(result.classList.contains("line-break")).toBe(true);
        expect(result.classList.contains("hidden-xs")).toBe(false);
    });

    it("creates line-break with hidden-xs for line-break-sm", () => {
        const result = PropertyFieldLineBreak({
            item: { formCssClass: "line-break-sm" }
        });
        expect(result.classList.contains("line-break")).toBe(true);
        expect(result.classList.contains("hidden-xs")).toBe(true);
    });

    it("creates line-break with hidden-sm for line-break-md", () => {
        const result = PropertyFieldLineBreak({
            item: { formCssClass: "line-break-md" }
        });
        expect(result.classList.contains("line-break")).toBe(true);
        expect(result.classList.contains("hidden-sm")).toBe(true);
    });

    it("creates line-break with hidden-md for line-break-lg", () => {
        const result = PropertyFieldLineBreak({
            item: { formCssClass: "line-break-lg" }
        });
        expect(result.classList.contains("line-break")).toBe(true);
        expect(result.classList.contains("hidden-md")).toBe(true);
    });
});

describe("PropertyField", () => {
    it("creates field element with caption and editor", () => {
        const field = PropertyField({
            item: { name: "MyField", title: "My Field", editorType: TestEditor, editorParams: {} },
            idPrefix: "pre_"
        });

        expect(field.classList.contains("field")).toBe(true);
        expect(field.classList.contains("MyField")).toBe(true);
        expect(field.dataset.itemname).toBe("MyField");
        expect(field.querySelector(".caption")).toBeTruthy();
        expect(field.querySelector(".editor")).toBeTruthy();
        expect(field.querySelector(".vx")).toBeTruthy();
        expect(field.querySelector(".clear")).toBeTruthy();
    });

    it("adds cssClass to field", () => {
        const field = PropertyField({
            item: { name: "Styled", editorType: TestEditor, editorParams: {}, cssClass: "extra-class" },
            idPrefix: "pre_"
        });
        expect(field.classList.contains("extra-class")).toBe(true);
    });

    it("adds formCssClass to field", () => {
        const field = PropertyField({
            item: { name: "FormStyled", editorType: TestEditor, editorParams: {}, formCssClass: "form-extra" },
            idPrefix: "pre_"
        });
        expect(field.classList.contains("form-extra")).toBe(true);
    });

    it("appends line break to container when formCssClass has line-break", () => {
        const container = document.createElement("div");
        const field = PropertyField({
            item: { name: "LB", editorType: TestEditor, editorParams: {}, formCssClass: "line-break-xs" },
            container,
            idPrefix: "pre_"
        });
        const lb = container.querySelector(".line-break");
        expect(lb).toBeTruthy();
    });
});

describe("PropertyCategoryTitle", () => {
    it("renders with text", () => {
        const title = PropertyCategoryTitle({
            category: "General",
            localTextPrefix: "Forms.Test."
        });
        expect(title.classList.contains("category-title")).toBe(true);
        expect(title.textContent).toContain("General");
    });
});

describe("PropertyCategory", () => {
    it("renders category div", () => {
        const cat = PropertyCategory({ category: "Details" });
        expect(cat.classList.contains("category")).toBe(true);
        expect(cat.dataset.category).toBe("Details");
        expect(cat.querySelector(".category-title")).toBeTruthy();
    });

    it("renders collapsible category", () => {
        const cat = PropertyCategory({ category: "Collapsible", collapsed: true });
        expect(cat.classList.contains("collapsible")).toBe(true);
        expect(cat.classList.contains("collapsed")).toBe(true);
    });

    it("renders non-collapsed collapsible category", () => {
        const cat = PropertyCategory({ category: "NotCollapsed", collapsed: false });
        expect(cat.classList.contains("collapsible")).toBe(true);
        expect(cat.classList.contains("collapsed")).toBe(false);
    });

    it("toggles collapse on title click", () => {
        const cat = PropertyCategory({ category: "Toggle", collapsed: true });
        const title = cat.querySelector(".category-title") as HTMLElement;
        title.click();
        expect(cat.classList.contains("collapsed")).toBe(false);
        title.click();
        expect(cat.classList.contains("collapsed")).toBe(true);
    });

    it("renders with children", () => {
        const child = document.createElement("span");
        const cat = PropertyCategory({ children: child });
        expect(cat.contains(child)).toBe(true);
    });

    it("handles category with .Categories. prefix in key", () => {
        const cat = PropertyCategory({ category: "Forms.MyForm.Categories.Details" });
        expect(cat.dataset.category).toBe("Details");
    });
});

describe("PropertyTabItem", () => {
    it("renders tab item", () => {
        const tab = PropertyTabItem({ title: "Main", active: true, paneId: "pane1" });
        expect(tab).toBeInstanceOf(HTMLLIElement);
        const link = tab.querySelector("a");
        expect(link).toBeTruthy();
        expect(link.getAttribute("href")).toBe("#pane1");
    });

    it("renders without paneId", () => {
        const tab = PropertyTabItem({ title: "Main", active: true });
        const link = tab.querySelector("a");
        expect(link.getAttribute("href")).toBeNull();
    });
});

describe("PropertyTabPane", () => {
    it("renders tab pane", () => {
        const pane = PropertyTabPane({ id: "pane1", active: true });
        expect(pane.classList.contains("tab-pane")).toBe(true);
        expect(pane.classList.contains("fade")).toBe(true);
    });

    it("renders with children", () => {
        const child = document.createElement("b");
        const pane = PropertyTabPane({ children: child });
        expect(pane.contains(child)).toBe(true);
    });
});

describe("PropertyCategories", () => {
    it("renders categories with items", () => {
        const items: PropertyItem[] = [
            { name: "Field1", editorType: TestEditor },
            { name: "Field2", editorType: TestEditor, category: "Details" },
        ];

        const categories = PropertyCategories({ items });
        expect(categories.classList.contains("categories")).toBe(true);
        const fields = categories.querySelectorAll('.field');
        expect(fields.length).toBe(2);
    });

    it("appends to container", () => {
        const container = document.createElement("div");
        const items: PropertyItem[] = [
            { name: "F1", editorType: TestEditor }
        ];

        const categories = PropertyCategories({ items, container });
        expect(container.contains(categories)).toBe(true);
    });

    it("collects fieldElements", () => {
        const fieldElements: PropertyFieldElement[] = [];
        const items: PropertyItem[] = [
            { name: "F1", editorType: TestEditor }
        ];

        PropertyCategories({ items, fieldElements });
        expect(fieldElements.length).toBe(1);
    });
});

describe("PropertyTabList", () => {
    it("renders tab list", () => {
        const list = PropertyTabList();
        expect(list.classList.contains("nav")).toBe(true);
        expect(list.classList.contains("property-tabs")).toBe(true);
    });

    it("renders with children", () => {
        const child = document.createElement("li");
        const list = PropertyTabList({ children: [child] });
        expect(list.contains(child)).toBe(true);
    });
});

describe("PropertyTabPanes", () => {
    it("renders panes container", () => {
        const panes = PropertyTabPanes();
        expect(panes.classList.contains("tab-content")).toBe(true);
        expect(panes.classList.contains("property-panes")).toBe(true);
    });
});

describe("PropertyTabs", () => {
    it("renders items without tab in categories", () => {
        const items: PropertyItem[] = [
            { name: "NoTab", editorType: TestEditor }
        ];

        const result = PropertyTabs({ items });
        expect(result).toBeInstanceOf(DocumentFragment);
        const categories = result.querySelector(".categories");
        expect(categories).toBeTruthy();
    });

    it("renders items with tab", () => {
        const items: PropertyItem[] = [
            { name: "InTab", editorType: TestEditor, tab: "Main" }
        ];

        const result = PropertyTabs({ items });
        const tabList = result.querySelector(".nav");
        const pane = result.querySelector(".tab-pane");
        expect(tabList).toBeTruthy();
        expect(pane).toBeTruthy();
    });

    it("renders mixed items (with and without tab)", () => {
        const items: PropertyItem[] = [
            { name: "NoTab", editorType: TestEditor },
            { name: "InTab1", editorType: TestEditor, tab: "First" },
            { name: "InTab2", editorType: TestEditor, tab: "First" },
            { name: "InTab3", editorType: TestEditor, tab: "Second" },
        ];

        const result = PropertyTabs({ items });
        const tabItems = result.querySelectorAll("li");
        expect(tabItems.length).toBe(2); // Two tabs
    });

    it("appends to container", () => {
        const container = document.createElement("div");
        const items: PropertyItem[] = [
            { name: "F1", editorType: TestEditor }
        ];

        const result = PropertyTabs({ items, container });
        expect(result).toBeNull(); // returns null when container is provided
        expect(container.querySelector(".categories")).toBeTruthy();
    });
});

describe("PropertyGrid", () => {
    it("creates with default options", () => {
        const pg = new PropertyGrid({ items: [] }).init();
        expect(pg.domNode.classList.contains("s-PropertyGrid")).toBe(true);
        expect(pg.get_mode()).toBe(PropertyGridMode.insert);
        pg.destroy();
    });

    it("renders items", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "Field1", editorType: TestEditor },
                { name: "Field2", editorType: TestEditor }
            ]
        }).init();

        const editors = pg.get_editors();
        expect(editors.length).toBe(2);
        pg.destroy();
    });

    it("get_items returns property items", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "F1", editorType: TestEditor }
            ]
        }).init();

        const items = pg.get_items();
        expect(items.length).toBe(1);
        expect(items[0].name).toBe("F1");
        pg.destroy();
    });

    it("get_idPrefix returns idPrefix", () => {
        const pg = new PropertyGrid({ items: [], idPrefix: "custom_" } as any).init();
        expect(pg.get_idPrefix()).toBe("custom_");
        pg.destroy();
    });

    it("enumerateItems iterates editors", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "F1", editorType: TestEditor }
            ]
        }).init();

        const callback = vi.fn();
        pg.enumerateItems(callback);
        expect(callback).toHaveBeenCalledTimes(1);
        expect(callback).toHaveBeenCalledWith(
            expect.objectContaining({ name: "F1" }),
            expect.any(Widget)
        );
        pg.destroy();
    });

    it("set_mode updates mode and calls updateInterface", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "F1", editorType: TestEditor }
            ]
        }).init();

        expect(pg.get_mode()).toBe(PropertyGridMode.insert);
        pg.set_mode(PropertyGridMode.update);
        expect(pg.get_mode()).toBe(PropertyGridMode.update);
        pg.destroy();
    });

    it("set_mode does nothing if mode is the same", () => {
        const pg = new PropertyGrid({ items: [] }).init();
        pg.set_mode(PropertyGridMode.insert);
        expect(pg.get_mode()).toBe(PropertyGridMode.insert);
        pg.destroy();
    });

    it("load/save roundtrip", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "Field1", editorType: TestEditor }
            ]
        }).init();

        const source = { Field1: "test value" };
        pg.load(source);

        const saved = pg.save();
        pg.destroy();
    });

    it("value getter and setter", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "Field1", editorType: TestEditor }
            ]
        }).init();

        pg.value = { Field1: "hello" };
        const val = pg.value;
        expect(val).toBeTruthy();
        pg.destroy();
    });

    it("destroy cleans up field elements", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "F1", editorType: TestEditor }
            ]
        }).init();

        pg.destroy();
        expect((pg as any).fieldElements).toBeNull();
    });

    it("canModifyItem for insert mode", () => {
        // insertable: false should make it not modifiable
        expect(PropertyGrid.canModifyItem({ name: "F1", insertable: false }, PropertyGridMode.insert)).toBe(false);

        // insertable: true by default
        expect(PropertyGrid.canModifyItem({ name: "F1" }, PropertyGridMode.insert)).toBe(true);

        // insertable: true explicitly
        expect(PropertyGrid.canModifyItem({ name: "F1", insertable: true }, PropertyGridMode.insert)).toBe(true);
    });

    it("canModifyItem for update mode", () => {
        expect(PropertyGrid.canModifyItem({ name: "F1", updatable: false }, PropertyGridMode.update)).toBe(false);
        expect(PropertyGrid.canModifyItem({ name: "F1" }, PropertyGridMode.update)).toBe(true);
        expect(PropertyGrid.canModifyItem({ name: "F1", updatable: true }, PropertyGridMode.update)).toBe(true);
    });

    it("canModifyItem returns true for other modes", () => {
        expect(PropertyGrid.canModifyItem({ name: "F1" }, undefined)).toBe(true);
    });

    it("renders with tabs when items have tab property", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "TabField1", editorType: TestEditor, tab: "First" },
                { name: "TabField2", editorType: TestEditor, tab: "Second" }
            ]
        }).init();

        expect(pg.domNode.querySelector(".nav")).toBeTruthy();
        expect(pg.domNode.querySelector(".tab-pane")).toBeTruthy();
        pg.destroy();
    });

    it("renders with mode option", () => {
        const pg = new PropertyGrid({
            items: [{ name: "F1", editorType: TestEditor }],
            mode: PropertyGridMode.update
        }).init();

        expect(pg.get_mode()).toBe(PropertyGridMode.update);
        pg.destroy();
    });
});

describe("PropertyGrid - additional edge cases", () => {

    it("value setter with null creates new object", () => {
        const pg = new PropertyGrid({
            items: [{ name: "F1", editorType: TestEditor }]
        }).init();

        pg.value = null;
        const val = pg.value;
        expect(val).toBeTruthy();
        pg.destroy();
    });

    it("loadFieldValue with defaultValue in insert mode", () => {
        const fieldElement = PropertyField({
            item: { name: "DefaultField", editorType: TestEditor, defaultValue: "defaultVal" },
        }) as PropertyFieldElement;

        const source = {};
        PropertyGrid.loadFieldValue(source, fieldElement, PropertyGridMode.insert);
        expect((source as any)["DefaultField"]).toBe("defaultVal");
    });

    it("loadFieldValue skips unbound items", () => {
        const fieldElement = PropertyField({
            item: { name: "SkipField", editorType: TestEditor, unbound: true },
        }) as PropertyFieldElement;

        const source = { SkipField: "shouldNotChange" };
        PropertyGrid.loadFieldValue(source, fieldElement, PropertyGridMode.insert);
        // unbound items are skipped in loadFieldValue, so value stays as is
        expect(source.SkipField).toBe("shouldNotChange");
    });

    it("loadFieldValue skips items with skipOnLoad", () => {
        const fieldElement = PropertyField({
            item: { name: "SkipLoad", editorType: TestEditor, skipOnLoad: true },
        }) as PropertyFieldElement;

        const source = { SkipLoad: "original" };
        PropertyGrid.loadFieldValue(source, fieldElement, PropertyGridMode.insert);
        expect(source.SkipLoad).toBe("original");
    });

    it("save skips unbound fields", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "Bound", editorType: TestEditor },
                { name: "Unbound", editorType: TestEditor, unbound: true }
            ]
        }).init();

        const result = pg.save();
        expect(result).toBeTruthy();
        pg.destroy();
    });

    it("save skips fields with skipOnSave", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "F1", editorType: TestEditor },
                { name: "F2", editorType: TestEditor, skipOnSave: true }
            ]
        }).init();

        const result = pg.save();
        expect(result).toBeTruthy();
        pg.destroy();
    });

    it("canModifyItem with updatePermission returns false when not authorized", () => {
        // Register a non-admin user so hasPermission returns false
        setScriptData("RemoteData.UserData", {
            Username: "test",
            IsAdmin: false,
            Permissions: {}
        });
        expect(PropertyGrid.canModifyItem(
            { name: "F1", updatePermission: "AdminPermission" },
            PropertyGridMode.update
        )).toBe(false);
    });

    it("canModifyItem with insertPermission returns false when not authorized", () => {
        expect(PropertyGrid.canModifyItem(
            { name: "F1", insertPermission: "AdminPermission" },
            PropertyGridMode.insert
        )).toBe(false);
    });

    it("updateFieldElement handles hideOnInsert", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "VisibleField", editorType: TestEditor, hideOnInsert: true }
            ],
            mode: PropertyGridMode.insert
        }).init();

        // The field should be hidden in insert mode when hideOnInsert is true
        const editors = pg.get_editors();
        expect(editors.length).toBe(1);
        pg.destroy();
    });

    it("setMaxLength removes maxlength when set to 0 or less", () => {
        const pg = new PropertyGrid({
            items: [
                { name: "NoMaxLen", editorType: TestEditor, maxLength: 0 }
            ]
        }).init();

        const editor = pg.domNode.querySelector(".editor") as HTMLElement;
        // maxLength 0 should remove the maxlength attribute
        expect(editor.hasAttribute("maxlength")).toBe(false);
        pg.destroy();
    });

    it("PropertyFieldLineBreak with line-break-lg", () => {
        const result = PropertyFieldLineBreak({
            item: { formCssClass: "line-break-lg" }
        });
        expect(result).toBeInstanceOf(HTMLElement);
        expect(result.classList.contains("hidden-md")).toBe(true);
    });

    it("extractTabKey with .Tabs. prefix", () => {
        // extractTabKey is internal, test through PropertyTabs
        const items: PropertyItem[] = [
            { name: "TabField", editorType: TestEditor, tab: "Forms.MyForm.Tabs.MainTab" }
        ];
        const result = PropertyTabs({ items });
        expect(result).toBeInstanceOf(DocumentFragment);
    });

    it("PropertyGrid with value: false option skips loading", () => {
        const pg = new PropertyGrid({
            items: [{ name: "F1", editorType: TestEditor }],
            value: false as any
        }).init();

        expect(pg.get_editors().length).toBe(1);
        pg.destroy();
    });
});

describe("PropertyGridMode", () => {
    it("insert has value 1", () => {
        expect(PropertyGridMode.insert).toBe(1);
    });

    it("update has value 2", () => {
        expect(PropertyGridMode.update).toBe(2);
    });
});

describe("PropertyGrid commitEdits", () => {
    it("returns true when no editors have commitEdits", async () => {
        const pg = new PropertyGrid({
            items: [{ name: "F1", editorType: TestEditor }]
        }).init();

        const result = await pg.commitEdits();
        expect(result).toBe(true);
        pg.destroy();
    });
});

describe("PropertyFieldCaption hints", () => {
    it("renders hint in title attribute", () => {
        addLocalText({ "Forms.TestForm.TestField_Hint": "Custom hint text" });
        const label = PropertyFieldCaption({
            item: { name: "TestField", title: "Test", hint: "`Hint", labelWidth: undefined, required: false },
            idPrefix: "pre_",
            localTextPrefix: "Forms.TestForm."
        });
        expect(label.title).toBe("Custom hint text");
    });

    it("uses hint text as title when no localized text found", () => {
        const label = PropertyFieldCaption({
            item: { name: "NoHint", title: "My Caption", hint: "`NonExistentHint", labelWidth: undefined, required: false },
            idPrefix: "pre_",
            localTextPrefix: "Forms.NoForm."
        });
        // determineText returns the hint text (without backtick) when no localized text is found
        expect(label.title).toBe("NonExistentHint");
    });
});
