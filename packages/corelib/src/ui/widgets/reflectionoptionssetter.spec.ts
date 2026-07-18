import { addTypeMember, TypeMemberKind } from "../../compat/system-compat";
import { OptionAttribute } from "../../types/attributes";
import { ReflectionOptionsSetter } from "./reflectionoptionssetter";

describe("makeCamelCase", () => {
    // makeCamelCase is not exported, but we test it through getPropertyValue and setPropertyValue

    it("converts 'ID' to 'id' via setPropertyValue", () => {
        const target = { id: null };
        ReflectionOptionsSetter.setPropertyValue(target, "ID", "test");
        expect(target["id"]).toBe("test");
    });

    it("converts single char to lowercase via setPropertyValue", () => {
        const target = { a: null };
        ReflectionOptionsSetter.setPropertyValue(target, "A", "val");
        expect(target["a"]).toBe("val");
    });

    it("handles already camelCase property via setPropertyValue", () => {
        const target = { myProp: null };
        ReflectionOptionsSetter.setPropertyValue(target, "myProp", "val");
        expect(target["myProp"]).toBe("val");
    });

    it("handles all uppercase via setPropertyValue", () => {
        const target = { ABC: null };
        ReflectionOptionsSetter.setPropertyValue(target, "ABC", "val");
        expect(target["ABC"]).toBe("val");
    });

    it("handles empty string", () => {
        const target = { "": null };
        ReflectionOptionsSetter.setPropertyValue(target, "", "val");
        expect(target[""]).toBe("val");
    });
});

describe("getPropertyValue", () => {
    it("gets value from direct property", () => {
        const obj = { myProp: "hello" };
        const result = ReflectionOptionsSetter.getPropertyValue(obj, "myProp");
        expect(result).toBe("hello");
    });

    it("gets value from getter method", () => {
        const obj = {
            _value: 42,
            get_value() { return this._value; }
        };
        const result = ReflectionOptionsSetter.getPropertyValue(obj, "value");
        expect(result).toBe(42);
    });

    it("gets value from getter method (camelCase)", () => {
        const obj = {
            _myProp: "test",
            get_myProp() { return this._myProp; }
        };
        const result = ReflectionOptionsSetter.getPropertyValue(obj, "MyProp");
        expect(result).toBe("test");
    });

    it("returns undefined for non-existent property", () => {
        const obj = {};
        const result = ReflectionOptionsSetter.getPropertyValue(obj, "nonexistent");
        expect(result).toBeUndefined();
    });

    it("gets value from direct property with null/undefined getter", () => {
        const obj = {
            get_myProp: undefined,
            myProp: "direct"
        };
        const result = ReflectionOptionsSetter.getPropertyValue(obj, "myProp");
        expect(result).toBe("direct");
    });
});

describe("setPropertyValue", () => {
    it("sets value via setter method", () => {
        const obj = {
            _value: null,
            set_value(v: any) { this._value = v; }
        };
        ReflectionOptionsSetter.setPropertyValue(obj, "value", 99);
        expect(obj._value).toBe(99);
    });

    it("sets value via setter method (camelCase)", () => {
        const obj = {
            _myProp: null,
            set_myProp(v: any) { this._myProp = v; }
        };
        ReflectionOptionsSetter.setPropertyValue(obj, "MyProp", "set");
        expect(obj._myProp).toBe("set");
    });

    it("sets value directly on property when no setter exists", () => {
        const obj = { myProp: null };
        ReflectionOptionsSetter.setPropertyValue(obj, "myProp", "direct");
        expect(obj.myProp).toBe("direct");
    });

    it("sets value directly on camelCase property", () => {
        const obj = { myProp: null };
        ReflectionOptionsSetter.setPropertyValue(obj, "MyProp", "value");
        expect(obj.myProp).toBe("value");
    });
});

describe("set", () => {
    it("does nothing when options is null", () => {
        const target = { myProp: "original" };
        ReflectionOptionsSetter.set(target, null);
        expect(target.myProp).toBe("original");
    });

    it("does nothing when target type is Object", () => {
        const target = Object.create(null);
        target.myProp = "original";
        ReflectionOptionsSetter.set(target, { myProp: "new" });
        expect(target.myProp).toBe("original");
    });

    it("sets properties marked with OptionAttribute (property kind)", () => {
        class WithOptionProps {
            myProp: string;
        }

        addTypeMember(WithOptionProps, {
            name: "myProp",
            kind: TypeMemberKind.property,
            attr: [new OptionAttribute()],
            setter: "myProp"
        });

        const target = new WithOptionProps();
        target.myProp = "original";

        // Since setter "myProp" is not a method, setting goes to field fallback
        // The set function looks for the setter method name, so let's create
        // a proper property with setter method
        const target2 = {
            _myProp: "original",
            set_myProp(v: string) { this._myProp = v; }
        };

        // We need to set up type members on the object's constructor
        // Actually, the set function uses getInstanceType(target) and then getTypeMembers
        // Let's use a class-based approach
        class MyOptionsClass {
            _myProp: string = "original";
            set_myProp(v: string) { this._myProp = v; }
        }

        addTypeMember(MyOptionsClass, {
            name: "myProp",
            kind: TypeMemberKind.property,
            attr: [new OptionAttribute()],
            setter: "set_myProp"
        });

        const instance = new MyOptionsClass();
        ReflectionOptionsSetter.set(instance, { myProp: "updated" });
        expect(instance._myProp).toBe("updated");
    });

    it("sets fields marked with OptionAttribute (field kind)", () => {
        class WithOptionField {
            myField: string = "original";
        }

        addTypeMember(WithOptionField, {
            name: "myField",
            kind: TypeMemberKind.field,
            attr: [new OptionAttribute()]
        });

        const target = new WithOptionField();
        ReflectionOptionsSetter.set(target, { myField: "updated" });
        expect(target.myField).toBe("updated");
    });

    it("ignores properties without OptionAttribute", () => {
        class WithoutOption {
            myProp: string = "original";
        }

        addTypeMember(WithoutOption, {
            name: "myProp",
            kind: TypeMemberKind.property,
            attr: []  // no OptionAttribute
        });

        const target = new WithoutOption();
        ReflectionOptionsSetter.set(target, { myProp: "updated" });
        expect(target.myProp).toBe("original"); // should not be set
    });

    it("handles camelCase key mapping for options", () => {
        class CamelCaseClass {
            _myProp: string = "original";
            set_myProp(v: string) { this._myProp = v; }
        }

        addTypeMember(CamelCaseClass, {
            name: "myProp",
            kind: TypeMemberKind.property,
            attr: [new OptionAttribute()],
            setter: "set_myProp"
        });

        const target = new CamelCaseClass();
        // Pass with PascalCase key
        ReflectionOptionsSetter.set(target, { MyProp: "updated" });
        expect(target._myProp).toBe("updated");
    });

    it("uses direct key match if camelCase lookup fails", () => {
        class DirectClass {
            _myProp: string = "original";
            set_myProp(v: string) { this._myProp = v; }
        }

        addTypeMember(DirectClass, {
            name: "myProp",
            kind: TypeMemberKind.property,
            attr: [new OptionAttribute()],
            setter: "set_myProp"
        });

        const target = new DirectClass();
        ReflectionOptionsSetter.set(target, { myProp: "updated" });
        expect(target._myProp).toBe("updated");
    });

    it("calls setter method if it exists", () => {
        class SetterClass {
            _value: number = 0;
            set_value(v: number) { this._value = v; }
        }

        addTypeMember(SetterClass, {
            name: "value",
            kind: TypeMemberKind.property,
            attr: [new OptionAttribute()],
            setter: "set_value"
        });

        const target = new SetterClass();
        ReflectionOptionsSetter.set(target, { value: 42 });
        expect(target._value).toBe(42);
    });
});
