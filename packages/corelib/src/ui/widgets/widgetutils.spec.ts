import { Fluent } from "../../base";
import { Widget } from "./widget";
import { associateWidget, deassociateWidget, getWidgetFrom, getWidgetName, tryGetWidget, useIdPrefix } from "./widgetutils";

describe("getWidgetName", () => {
    it("returns type full name with dots replaced by underscores", () => {
        class TestWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("MyNamespace.MyWidget");
        }
        const name = getWidgetName(TestWidget);
        expect(name).toBe("MyNamespace_MyWidget");
    });

    it("handles types without type info", () => {
        class SimpleWidget {}
        const name = getWidgetName(SimpleWidget);
        expect(name).toBe("SimpleWidget");
    });
});

describe("associateWidget / deassociateWidget", () => {
    it("associates widget with its domNode", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };

        associateWidget(widget);
        const found = tryGetWidget(div);
        expect(found).toBe(widget);
    });

    it("deassociates widget from its domNode", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };

        associateWidget(widget);
        deassociateWidget(widget);
        const found = tryGetWidget(div);
        expect(found).toBeNull();
    });

    it("throws when associating duplicate widget name to same element", () => {
        const div = document.createElement("div");
        const widget1 = { domNode: div };
        const widget2 = { domNode: div };

        associateWidget(widget1);
        expect(() => associateWidget(widget2)).toThrow();
    });

    it("handles deassociate with null/undefined widget", () => {
        expect(() => deassociateWidget(null)).not.toThrow();
        expect(() => deassociateWidget(undefined as any)).not.toThrow();
    });

    it("handles deassociate with widget without domNode", () => {
        expect(() => deassociateWidget({} as any)).not.toThrow();
    });

    it("handles associate with null/undefined widget", () => {
        expect(() => associateWidget(null)).not.toThrow();
        expect(() => associateWidget(undefined as any)).not.toThrow();
    });
});

describe("tryGetWidget", () => {
    it("returns null for non-existent element", () => {
        const div = document.createElement("div");
        const result = tryGetWidget(div);
        expect(result).toBeNull();
    });

    it("returns null for null/undefined element", () => {
        expect(tryGetWidget(null)).toBeNull();
        expect(tryGetWidget(undefined as any)).toBeNull();
    });

    it("handles string selector", () => {
        const div = document.createElement("div");
        div.id = "testWidgetElement";
        document.body.appendChild(div);

        const result = tryGetWidget("#testWidgetElement");
        expect(result).toBeNull();

        document.body.removeChild(div);
    });

    it("handles ArrayLike element", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };
        associateWidget(widget);

        const result = tryGetWidget([div] as any);
        expect(result).toBe(widget);
    });

    it("finds widget by specific type", () => {
        const div = document.createElement("div");
        class MyWidget extends Widget { }
        class OtherWidget extends Widget { }
        const widget = new MyWidget({ element: div });

        const found = tryGetWidget(div, MyWidget);
        expect(found).toBe(widget);

        const notFound = tryGetWidget(div, OtherWidget);
        expect(notFound).toBeNull();

        widget.destroy();
    });

    it("returns first widget when no type specified", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };
        associateWidget(widget);

        const result = tryGetWidget(div);
        expect(result).toBe(widget);
    });
});

describe("getWidgetFrom", () => {
    it("throws for non-existent element", () => {
        expect(() => getWidgetFrom("#nonexistent")).toThrow();
    });

    it("throws when string selector element not found", () => {
        expect(() => getWidgetFrom("#i-dont-exist-in-dom")).toThrow();
    });

    it("throws when element has no widget", () => {
        const div = document.createElement("div");
        expect(() => getWidgetFrom(div)).toThrow();
    });

    it("returns widget when found", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };
        associateWidget(widget);

        const result = getWidgetFrom(div);
        expect(result).toBe(widget);
    });

    it("finds widget by type using isAssignableFrom", () => {
        const div = document.createElement("div");
        class BaseWidget extends Widget { }
        class DerivedWidget extends BaseWidget { }
        const widget = new DerivedWidget({ element: div });

        const found = getWidgetFrom(div, BaseWidget);
        expect(found).toBe(widget);
    });
});

describe("useIdPrefix", () => {
    it("returns proxy with prefix applied", () => {
        const id = useIdPrefix("my_");
        expect(id._).toBe("my__");
        expect(id.Form).toBe("my_Form");
        expect(id.Toolbar).toBe("my_Toolbar");
        expect(id.PropertyGrid).toBe("my_PropertyGrid");
        expect(id.something).toBe("my_something");
    });

    it("handles empty prefix", () => {
        const id = useIdPrefix("");
        expect(id._).toBe("_");
        expect(id.Form).toBe("Form");
    });

    it("handles hash keys for in-page hrefs", () => {
        const id = useIdPrefix("my_");
        expect(id["#_"]).toBe("#my__");
        expect(id["#Form"]).toBe("#my_Form");
        expect(id["#PropertyGrid"]).toBe("#my_PropertyGrid");
        expect(id["#something"]).toBe("#my_something");
    });
});

describe("Fluent extensions", () => {
    it("Fluent.prototype.getWidget returns widget from element", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };
        associateWidget(widget);

        const fluent = Fluent(div);
        const found = fluent.getWidget();
        expect(found).toBe(widget);
    });

    it("Fluent.prototype.tryGetWidget returns widget from element", () => {
        const div = document.createElement("div");
        const widget = { domNode: div };
        associateWidget(widget);

        const fluent = Fluent(div);
        const found = fluent.tryGetWidget();
        expect(found).toBe(widget);
    });

    it("Fluent.prototype.tryGetWidget returns null for element without widget", () => {
        const div = document.createElement("div");
        const fluent = Fluent(div);
        const found = fluent.tryGetWidget();
        expect(found).toBeNull();
    });
});

describe("associateWidget edge cases", () => {
    it("associates second widget to same element when name differs", () => {
        const div = document.createElement("div");

        class FirstWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.FirstWidget");
        }
        class SecondWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.SecondWidget");
        }

        const w1 = new FirstWidget({ element: div });
        // Now try to associate a second widget with the same element
        const w2 = new SecondWidget({ element: div });
        // This should work since names differ

        const found1 = tryGetWidget(div, FirstWidget);
        expect(found1).toBe(w1);

        const found2 = tryGetWidget(div, SecondWidget);
        expect(found2).toBe(w2);

        w1.destroy();
        w2.destroy();
    });
});
