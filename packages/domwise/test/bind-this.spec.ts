import { bindThis } from "#src/bind-this";

describe("bindThis", () => {
    class TestClass {
        value = 42;

        getValue() {
            return this.value;
        }

        setValue(newValue: number) {
            this.value = newValue;
            return this;
        }

        arrowMethod = () => {
            return this.value;
        };
    }

    it("should return a proxy object", () => {
        const instance = new TestClass();
        const bound = bindThis(instance);

        expect(bound).not.toBe(instance);
        expect(typeof bound).toBe("object");
    });

    it("should return the same proxy for multiple calls", () => {
        const instance = new TestClass();
        const bound1 = bindThis(instance);
        const bound2 = bindThis(instance);

        expect(bound1).toBe(bound2);
    });

    it("should lazily bind methods and cache them on instance", () => {
        const instance = new TestClass();
        const bound = bindThis(instance);

        // Method not yet bound
        expect(instance.hasOwnProperty("getValue")).toBe(false);

        // Access through proxy binds and caches
        const boundMethod = bound.getValue;
        expect(typeof boundMethod).toBe("function");
        expect(instance.hasOwnProperty("getValue")).toBe(true);

        // Subsequent access returns cached
        expect(bound.getValue).toBe(boundMethod);
    });

    it("should bind methods with correct this context", () => {
        const instance = new TestClass();
        const bound = bindThis(instance);

        const { getValue, setValue } = bound;

        expect(getValue()).toBe(42);

        setValue(100);
        expect(getValue()).toBe(100);
        expect(instance.value).toBe(100);
    });

    it("should not bind arrow methods or own properties", () => {
        const instance = new TestClass();
        const bound = bindThis(instance);

        // Arrow method should remain as is
        expect(bound.arrowMethod).toBe(instance.arrowMethod);

        // Own property should be returned directly
        (instance as any).customProp = "test";
        expect((bound as any).customProp).toBe("test");
    });

    it("should handle inheritance", () => {
        class ChildClass extends TestClass {
            getDoubleValue() {
                return this.value * 2;
            }
        }

        const instance = new ChildClass();
        const bound = bindThis(instance);

        expect(bound.getDoubleValue()).toBe(84); // 42 * 2
        expect(instance.hasOwnProperty("getDoubleValue")).toBe(true);
    });
});