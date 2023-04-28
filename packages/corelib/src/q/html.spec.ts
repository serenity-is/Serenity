import { addEmptyOption, addOption, clearOptions, findElementWithRelativeId, htmlEncode, attrEncode, outerHtml, newBodyDiv } from "./html";
import $ from "@optionaldeps/jquery";

describe("addEmptyOption", () => {
    it("adds an option to the select", () => {
        const select = document.createElement("select");
        addEmptyOption($(select));
        expect(select.children.length).toBe(1);
        expect((select.children[0] as HTMLOptionElement).value).toBe("");
        expect(select.children[0].textContent).toBe("Controls.SelectEditor.EmptyItemText");
    }); 
});

describe("addOption", () => {
    it("adds an option to the select", () => {
        const select = document.createElement("select");
        addOption($(select), "test", "text");
        expect(select.children.length).toBe(1);
        expect((select.children[0] as HTMLOptionElement).value).toBe("test");
        expect(select.children[0].textContent).toBe("text");
    });

    it("can handle null values", () => {
        const select = document.createElement("select");
        addOption($(select), null, null);
        expect(select.children.length).toBe(1);
        expect((select.children[0] as HTMLOptionElement).value).toBe("");
        expect(select.children[0].textContent).toBe("");
    });
});

describe("attrEncode", () => {
    it("is same with htmlEncode", () => {
        expect(attrEncode("<div>test</div>")).toBe("&lt;div&gt;test&lt;/div&gt;");
        expect(attrEncode("'&><\"")).toBe("&#39;&amp;&gt;&lt;&quot;");
    });
});

describe("clearOptions", () => {
    it("clears all options from the select", () => {
        const select = document.createElement("select");
        addOption($(select), "test", "text");
        addEmptyOption($(select));
        clearOptions($(select));
        expect(select.children.length).toBe(0);
    });
});

describe("findElementWithRelativeId", () => {
    it("returns null if the from element is null", () => {
        var target = document.createElement("div");
        target.id = "test";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(null, "test")).toBeNull();
        }
        finally {
            target.remove();
        }
    });

    it("returns null if the from element has no id, and no element with the given id in document", () => {
        const from = document.createElement("div");
        var target = document.createElement("div");
        target.id = "test2";
        document.body.append(target);
        document.body.append(from);
        try {
            expect(findElementWithRelativeId(from, "test1")).toBeNull();
        }
        finally {
            from.remove();
            target.remove();
        }
    });

    it("returns null if the detached from element has no id, and no element with the given id in document", () => {
        const from = document.createElement("div");
        var target = document.createElement("div");
        target.id = "test2";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test1")).toBeNull();
        }
        finally {
            target.remove();
            from.remove();
        }
    });

    it("if the from element has no id finds the element with the given id", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        target.id = "test";
        document.body.append(target);
        document.body.append(from);
        try {
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
        }
        finally {
            target.remove();
            from.remove();
        }
    });

    it("may still find the element with the given id in document if the from element is detached", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        target.id = "test";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
        }
        finally {
            target.remove();
        }
    });

    it("won't find the element with the given id in document if the from element is detached and context is passed as null", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        target.id = "test";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test", null)).toBeNull();
        }
        finally {
            target.remove();
        }
    });

    it("may use jQuery if a jQuery object is passed", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        target.id = "test";
        document.body.append(target);
        document.body.append(from);
        try {
            expect(findElementWithRelativeId($(from), "test")?.get(0) === target).toBe(true);
        }
        finally {
            target.remove();
            from.remove();
        }
    });
    
    it("can find element with ID = fromID + searched ID", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        from.id = "from";
        target.id = "fromtest";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
        }
        finally {
            target.remove();
        }
    });

    it("can find element with ID = fromID + _ + searched ID", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        from.id = "from";
        target.id = "from_test";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
        }
        finally {
            target.remove();
        }
    });

    it("can find element with ID = fromID splitted by underscore + _ + searched ID", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        from.id = "from_my_element";
        target.id = "from_mytest";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
            target.id = "from_my_test";
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
            target.id = "from_test";
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
            target.id = "fromtest";
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
            target.id = "test";
            expect(findElementWithRelativeId(from, "test") === target).toBe(true);
        }
        finally {
            target.remove();
        }
    });

    it("gives more priority to find ID + relative ID than findID + _ + relative ID", () => {
        const from = document.createElement("div");
        from.id = "from";
        var target1 = document.createElement("div");
        target1.id = "from_test";
        var target2 = document.createElement("div");
        target2.id = "fromtest";
        document.body.append(target1);
        document.body.append(target2);
        try {
            expect(findElementWithRelativeId(from, "test") === target2).toBe(true);
        }
        finally {
            target1.remove();
            target2.remove();
        }
    });

    it("gives more priority to longer ID matches", () => {
        const from = document.createElement("div");
        from.id = "from_my_element";
        var target1 = document.createElement("div");
        target1.id = "fromtest";
        var target2 = document.createElement("div");
        target2.id = "from_mytest";
        document.body.append(target1);
        document.body.append(target2);
        try {
            expect(findElementWithRelativeId(from, "test") === target2).toBe(true);
        }
        finally {
            target1.remove();
            target2.remove();
        }
    });

    it("can find quick filter cascade editor ID's", () => {
        const city = document.createElement("div");
        city.id = "Serenity_Demo_Norhtwind_CustomerGrid0_QuickFilter_City";
        var country = document.createElement("div");
        country.id = "Serenity_Demo_Norhtwind_CustomerGrid0_QuickFilter_Country";
        document.body.append(country);
        document.body.append(city);
        try {
            expect(findElementWithRelativeId(city, "Country") === country).toBe(true);
        }
        finally {
            city.remove();
            country.remove();
        }
    });

    it("can find quick filter cascade editor ID's with jQuery", () => {
        const city = document.createElement("div");
        city.id = "Serenity_Demo_Norhtwind_CustomerGrid0_QuickFilter_City";
        var country = document.createElement("div");
        country.id = "Serenity_Demo_Norhtwind_CustomerGrid0_QuickFilter_Country";
        document.body.append(country);
        document.body.append(city);
        try {
            expect(findElementWithRelativeId($(city), "Country")?.get?.(0) === country).toBe(true);
        }
        finally {
            city.remove();
            country.remove();
        }
    });    

    it("won't search document if the context is passed", () => {
        var target = document.createElement("div");
        const from = document.createElement("div");
        target.id = "test";
        document.body.append(target);
        try {
            expect(findElementWithRelativeId(from, "test", from.getRootNode() as HTMLElement)).toBeNull();
        }
        finally {
            target.remove();
        }
    });
});

describe("htmlEncode", () => {
    it("encodes html", () => {
        expect(htmlEncode("<div>test</div>")).toBe("&lt;div&gt;test&lt;/div&gt;");
        expect(htmlEncode("'&><\"")).toBe("&#39;&amp;&gt;&lt;&quot;");
    });

    it("handles null and empty string values", () => {
        expect(htmlEncode(null)).toBe("");
        expect(htmlEncode(undefined)).toBe("");
        expect(htmlEncode("")).toBe("");
    });

    it("converts other types to string", () => {
        expect(htmlEncode(1)).toBe("1");
        expect(htmlEncode(true)).toBe("true");
        expect(htmlEncode(false)).toBe("false");
    });
});

describe("outerHtml", () => {
    it("returns the outer html of the element", () => {
        const div = document.createElement("div");
        div.innerHTML = "<span>test</span>";
        expect(outerHtml($(div))).toBe("<div><span>test</span></div>");
    });
});

describe("newBodyDiv", () => {
    it("creates a new div and appends it to the document body", () => {
        const div = newBodyDiv();
        expect(div.parent().get(0) === document.body).toBe(true);
        expect(div.get(0).tagName).toBe("DIV");
    });
});