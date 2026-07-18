import { addCustomAttribute } from "../../base";
import { ElementAttribute } from "../../types/attributes";
import { Widget } from "./widget";
import { createElementFor, ensureParentOrFragment, handleElementProp, isFragmentWorkaround, setElementProps } from "./widgetinternal";

describe("ensureParentOrFragment", () => {
    it("returns node if it already has a parent", () => {
        const parent = document.createElement("div");
        const child = document.createElement("span");
        parent.appendChild(child);

        const result = ensureParentOrFragment(child);
        expect(result).toBe(child);
        expect(child.parentNode).toBe(parent);
    });

    it("wraps node in fragment if it has no parent", () => {
        const node = document.createElement("div");
        const result = ensureParentOrFragment(node);
        expect(result).toBe(node);
        expect(node.parentNode).toBeInstanceOf(DocumentFragment);
        expect((node.parentNode as any)[isFragmentWorkaround]).toBe(true);
    });
});

describe("createElementFor", () => {
    it("creates default element when type has no ElementAttribute", () => {
        class SimpleWidget extends Widget {}
        const el = createElementFor(SimpleWidget);
        expect(el).toBeInstanceOf(HTMLElement);
        expect(el.tagName).toBe("DIV");
    });

    it("creates element from ElementAttribute HTML", () => {
        class CustomWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.CustomWidget");
        }

        addCustomAttribute(CustomWidget, new ElementAttribute("<input type='text' />"));
        const el = createElementFor(CustomWidget);
        expect(el).toBeInstanceOf(HTMLElement);
        expect(el.tagName).toBe("INPUT");
        expect((el as HTMLInputElement).type).toBe("text");
    });

    it("handles ElementAttribute with empty content", () => {
        class EmptyAttrWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.EmptyAttrWidget");
        }

        addCustomAttribute(EmptyAttrWidget, new ElementAttribute("<div></div>"));
        const el = createElementFor(EmptyAttrWidget);
        expect(el).toBeInstanceOf(HTMLElement);
        expect(el.innerHTML).toBe("");
    });

    it("returns default element when ElementAttribute HTML has no children", () => {
        class InvalidAttrWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.InvalidAttrWidget");
        }
        addCustomAttribute(InvalidAttrWidget, new ElementAttribute(""));
        const el = createElementFor(InvalidAttrWidget);
        expect(el.tagName).toBe("DIV");
    });
});

describe("handleElementProp", () => {
    it("creates default element when element prop is not provided", () => {
        class TestWidget extends Widget {}
        const el = handleElementProp(TestWidget, {});
        expect(el).toBeInstanceOf(HTMLElement);
        expect(el.tagName).toBe("DIV");
    });

    it("uses HTMLElement from element prop", () => {
        const existing = document.createElement("span");
        const el = handleElementProp(Widget, { element: existing });
        expect(el).toBe(existing);
    });

    it("uses first element from ArrayLike element prop", () => {
        const existing = document.createElement("span");
        const el = handleElementProp(Widget, { element: [existing] as any });
        expect(el).toBe(existing);
    });

    it("calls element function with created domNode", () => {
        const fn = vi.fn();
        const el = handleElementProp(Widget, { element: fn });
        expect(fn).toHaveBeenCalledTimes(1);
        expect(fn).toHaveBeenCalledWith(expect.any(HTMLElement));
        expect(el).toBeInstanceOf(HTMLElement);
    });

    it("queries DOM by string selector", () => {
        const target = document.createElement("div");
        target.id = "handleElementPropTarget";
        document.body.appendChild(target);

        const el = handleElementProp(Widget, { element: "#handleElementPropTarget" });
        expect(el).toBe(target);

        document.body.removeChild(target);
    });

    it("throws when string selector does not match any element", () => {
        expect(() => {
            handleElementProp(Widget, { element: "#nonexistent-element-xyz" });
        }).toThrow();
    });
});

describe("setElementProps", () => {
    it("sets id on domNode", () => {
        const div = document.createElement("div");
        const widget = { domNode: div } as any;
        setElementProps(widget, { id: "myId" });
        expect(div.id).toBe("myId");
    });

    it("sets name attribute", () => {
        const div = document.createElement("div");
        const widget = { domNode: div } as any;
        setElementProps(widget, { name: "myName" });
        expect(div.getAttribute("name")).toBe("myName");
    });

    it("sets placeholder attribute", () => {
        const div = document.createElement("div");
        const widget = { domNode: div } as any;
        setElementProps(widget, { placeholder: "enter value" });
        expect(div.getAttribute("placeholder")).toBe("enter value");
    });

    it("sets class via addClass", () => {
        const div = document.createElement("div");
        const widget = { domNode: div } as any;
        setElementProps(widget, { class: "my-class" });
        expect(div.classList.contains("my-class")).toBe(true);
    });

    it("sets className via addClass", () => {
        const div = document.createElement("div");
        const widget = { domNode: div } as any;
        // Note: 'className' in EditorProps is separate from 'class'
        setElementProps(widget, { className: "my-other-class" } as any);
        expect(div.classList.contains("my-other-class")).toBe(true);
    });

    it("sets maxLength attribute", () => {
        const input = document.createElement("input");
        const widget = { domNode: input } as any;
        setElementProps(widget, { maxLength: 50 });
        expect(input.getAttribute("maxLength")).toBe("50");
    });

    it("sets maxLength from lowercase maxlength", () => {
        const input = document.createElement("input");
        const widget = { domNode: input } as any;
        setElementProps(widget, { maxlength: 100 } as any);
        expect(input.getAttribute("maxLength")).toBe("100");
    });

    it("handles null/undefined domNode gracefully", () => {
        const widget = { domNode: null } as any;
        expect(() => setElementProps(widget, { id: "test" })).not.toThrow();
    });

    it("handles null/undefined props gracefully", () => {
        const widget = { domNode: document.createElement("div") } as any;
        expect(() => setElementProps(widget, null)).not.toThrow();
        expect(() => setElementProps(widget, undefined)).not.toThrow();
    });
});
