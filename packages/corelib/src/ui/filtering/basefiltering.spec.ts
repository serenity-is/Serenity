import { BaseFiltering } from "./basefiltering";
import { FilterOperator } from "./filteroperator";

describe("BaseFiltering", () => {
    describe("getters/setters", () => {
        it("sets and gets field", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const field = { name: "TestField", title: "Test Title" };
            filtering.set_field(field);
            expect(filtering.get_field()).toBe(field);
        });

        it("sets and gets container", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const container = document.createElement("div");
            filtering.set_container(container);
            expect(filtering.get_container()).toBe(container);
        });

        it("sets and gets operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const op = { key: "eq" };
            filtering.set_operator(op);
            expect(filtering.get_operator()).toBe(op);
        });
    });

    describe("isNullable", () => {
        it("returns true when required is not true", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            expect(filtering["isNullable"]()).toBe(true);
        });

        it("returns false when required is true", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test", required: true });
            expect(filtering["isNullable"]()).toBe(false);
        });
    });

    describe("appendNullableOperators", () => {
        it("appends isNotNull and isNull when field is nullable", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const result = filtering["appendNullableOperators"]([]);
            expect(result).toHaveLength(2);
            expect(result[0].key).toBe("isnotnull");
            expect(result[1].key).toBe("isnull");
        });

        it("does not append when field is required", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test", required: true });
            const result = filtering["appendNullableOperators"]([{ key: "eq" }]);
            expect(result).toHaveLength(1);
        });
    });

    describe("appendComparisonOperators", () => {
        it("appends EQ, NE, LT, LE, GT, GE", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const result = filtering["appendComparisonOperators"]([]);
            expect(result).toHaveLength(6);
            expect(result[0].key).toBe("eq");
            expect(result[1].key).toBe("ne");
            expect(result[2].key).toBe("lt");
            expect(result[3].key).toBe("le");
            expect(result[4].key).toBe("gt");
            expect(result[5].key).toBe("ge");
        });
    });

    describe("createEditor", () => {
        it("does nothing for true/false/isnull/isnotnull operators", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);

            filtering.set_operator({ key: "true" });
            filtering.createEditor();
            expect(container.children.length).toBe(0);

            filtering.set_operator({ key: "false" });
            filtering.createEditor();
            expect(container.children.length).toBe(0);

            filtering.set_operator({ key: "isnull" });
            filtering.createEditor();
            expect(container.children.length).toBe(0);

            filtering.set_operator({ key: "isnotnull" });
            filtering.createEditor();
            expect(container.children.length).toBe(0);
        });

        it("creates input for comparison and like operators", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);

            const ops = ["contains", "startswith", "eq", "ne", "lt", "le", "gt", "ge"];
            ops.forEach(opKey => {
                const c = document.createElement("div");
                filtering.set_container(c);
                filtering.set_operator({ key: opKey });
                filtering.createEditor();
                expect(c.querySelector("input")).toBeTruthy();
            });
        });

        it("throws for unknown operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            filtering.set_container(document.createElement("div"));
            filtering.set_operator({ key: "custom_op" });
            expect(() => filtering.createEditor()).toThrow();
        });
    });

    describe("operatorFormat", () => {
        it("returns op.format if available", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const op: FilterOperator = { key: "eq", format: "{0} = {1}" };
            expect(filtering["operatorFormat"](op)).toBe("{0} = {1}");
        });

        it("returns from FilterPanelTexts.OperatorFormats if format not set", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const op: FilterOperator = { key: "eq" };
            const result = filtering["operatorFormat"](op);
            expect(typeof result).toBe("string");
        });

        it("returns the key as fallback", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const op: FilterOperator = { key: "nonexistent_key_12345" };
            expect(filtering["operatorFormat"](op)).toBe("nonexistent_key_12345");
        });
    });

    describe("getTitle", () => {
        it("returns localText of title", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const field = { name: "TestField", title: "Test Title" };
            const title = filtering["getTitle"](field);
            expect(title).toBeTruthy();
        });

        it("returns title if no localText and title exists", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            const field = { name: "TestField", title: "Test Title" };
            const title = filtering["getTitle"](field);
            expect(typeof title).toBe("string");
        });
    });

    describe("displayText", () => {
        it("formats with no values", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const op: FilterOperator = { key: "isnull" };
            const text = filtering["displayText"](op);
            expect(typeof text).toBe("string");
        });

        it("formats with one value", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const op: FilterOperator = { key: "eq", format: "{0} = {1}" };
            const text = filtering["displayText"](op, ["value1"]);
            expect(text).toContain("value1");
        });

        it("formats with two values", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const op: FilterOperator = { key: "bw", format: "{0} between {1} and {2}" };
            const text = filtering["displayText"](op, ["start", "end"]);
            expect(text).toContain("start");
            expect(text).toContain("end");
        });
    });

    describe("getCriteriaField", () => {
        it("returns field name", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "MyField" });
            expect(filtering["getCriteriaField"]()).toBe("MyField");
        });
    });

    describe("getCriteria", () => {
        it("handles isTrue operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Active" });
            filtering.set_operator({ key: "true" });
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual([["Active"], "=", true]);
        });

        it("handles isFalse operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Active" });
            filtering.set_operator({ key: "false" });
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual([["Active"], "=", false]);
        });

        it("handles isNull operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });
            filtering.set_operator({ key: "isnull" });
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual(["is null", ["Field"]]);
        });

        it("handles isNotNull operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });
            filtering.set_operator({ key: "isnotnull" });
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual(["is not null", ["Field"]]);
        });

        it("handles contains operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "contains" });
            filtering.createEditor();
            container.querySelector("input").value = "search";
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual([["Field"], "like", "%search%"]);
        });

        it("handles startswith operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "startswith" });
            filtering.createEditor();
            container.querySelector("input").value = "prefix";
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual([["Field"], "like", "prefix%"]);
        });

        it("handles eq operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();
            container.querySelector("input").value = "value";
            const result = filtering.getCriteria();
            expect(result.criteria).toEqual([["Field"], "=", "value"]);
        });

        it("handles ne, lt, le, gt, ge operators", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });

            const ops = ["ne", "lt", "le", "gt", "ge"];
            ops.forEach(opKey => {
                const container = document.createElement("div");
                filtering.set_container(container);
                filtering.set_operator({ key: opKey });
                filtering.createEditor();
                container.querySelector("input").value = "5";
                const result = filtering.getCriteria();
                expect(result.criteria).toBeTruthy();
                expect(result.criteria[1]).toBeTruthy();
                expect(result.criteria[2]).toBe("5");
            });
        });

        it("throws for unknown operator", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Field" });
            filtering.set_operator({ key: "unknown_op" });
            filtering.set_container(document.createElement("div"));
            expect(() => filtering.getCriteria()).toThrow();
        });
    });

    describe("loadState", () => {
        it("sets input value", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            filtering.loadState("testState");
            expect(container.querySelector("input").value).toBe("testState");
        });

        it("does nothing if no input found", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "isnull" });

            expect(() => filtering.loadState("test")).not.toThrow();
        });
    });

    describe("saveState", () => {
        it("returns input value for comparison operators", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();
            container.querySelector("input").value = "saved";
            expect(filtering.saveState()).toBe("saved");
        });

        it("returns null for boolean/null operators", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            ["true", "false", "isnull", "isnotnull"].forEach(opKey => {
                filtering.set_operator({ key: opKey });
                expect(filtering.saveState()).toBeNull();
            });
        });
    });

    describe("validateEditorValue", () => {
        it("throws on empty string", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            expect(() => filtering.validateEditorValue("")).toThrow();
        });

        it("returns non-empty value", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            expect(filtering.validateEditorValue("valid")).toBe("valid");
        });
    });

    describe("getEditorValue", () => {
        it("gets value from input", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();
            container.querySelector("input").value = "  testValue  ";

            expect(filtering.getEditorValue()).toBe("testValue");
        });

        it("throws if no input found", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            filtering.set_container(document.createElement("div"));
            filtering.set_operator({ key: "eq" });

            expect(() => filtering.getEditorValue()).toThrow();
        });

        it("throws if input value is empty", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();
            container.querySelector("input").value = "   ";

            expect(() => filtering.getEditorValue()).toThrow();
        });
    });

    describe("getEditorText", () => {
        it("gets text from input", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();
            container.querySelector("input").value = "textValue";

            expect(filtering.getEditorText()).toBe("textValue");
        });

        it("returns textContent if no input found", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            container.textContent = "directText";
            filtering.set_container(container);
            filtering.set_operator({ key: "isnull" });

            expect(filtering.getEditorText()).toBe("directText");
        });
    });

    describe("initQuickFilter", () => {
        it("initializes quick filter with field and type", () => {
            const filtering = new (class extends BaseFiltering {
                getOperators(): FilterOperator[] { return []; }
            })();
            filtering.set_field({ name: "QuickField", title: "Quick Title", quickFilterParams: { some: "param" } });
            const filter: any = {};
            filtering.initQuickFilter(filter);
            expect(filter.field).toBe("QuickField");
            expect(filter.title).toBeTruthy();
            expect(filter.type).toBeTruthy();
            expect(filter.options).toEqual({ some: "param" });
        });
    });

    describe("registerClass", () => {
        it("registers class with type info", () => {
            const TestClass = class extends BaseFiltering {
                static [Symbol.typeInfo] = this.registerClass("Serenity.TestClass");
                getOperators(): FilterOperator[] { return []; }
            };
            expect(TestClass[Symbol.for("Serenity.typeInfo")]).toBeDefined();
        });

        it("throws if typeInfo already exists on the class", () => {
            // Use a unique class name to avoid conflicts with other tests
            const uniqueName = "Serenity.TestClass_" + Date.now();
            const TestClass = class extends BaseFiltering {
                static [Symbol.typeInfo] = (this.registerClass as any)(uniqueName);
                getOperators(): FilterOperator[] { return []; }
            };
            expect(() => {
                TestClass["registerClass"]("Serenity.AnotherName");
            }).toThrow();
        });
    });
});
