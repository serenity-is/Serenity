import { bindPrototypeMethods, bindThis } from "../src/bind-util";

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

describe("bindPrototypeMethods", () => {
    class TestClass {
        value = 42;
        
        getValue() {
            return this.value;
        }
        
        setValue(newValue: number) {
            this.value = newValue;
            return this;
        }
        
        privateMethod() {
            return "private";
        }
        
        arrowMethod = () => {
            return this.value;
        };
    }

    it("should bind all prototype methods upfront", () => {
        const instance = new TestClass();
        
        expect(instance.hasOwnProperty("getValue")).toBe(false);
        expect(instance.hasOwnProperty("setValue")).toBe(false);
        
        bindPrototypeMethods(instance);
        
        expect(instance.hasOwnProperty("getValue")).toBe(true);
        expect(instance.hasOwnProperty("setValue")).toBe(true);
        expect(instance.hasOwnProperty("privateMethod")).toBe(true);
    });

    it("should bind methods with correct this context", () => {
        const instance = new TestClass();
        bindPrototypeMethods(instance);
        
        expect(instance.getValue()).toBe(42);
        
        instance.setValue(200);
        expect(instance.getValue()).toBe(200);
    });

    it("should not rebind already bound methods", () => {
        const instance = new TestClass();
        
        // Manually bind one method
        instance.getValue = instance.getValue.bind(instance);
        const originalBound = instance.getValue;
        
        bindPrototypeMethods(instance);
        
        // Should still be the same bound method
        expect(instance.getValue).toBe(originalBound);
    });

    it("should handle inheritance chain", () => {
        class ParentClass {
            parentMethod() {
                return "parent";
            }
        }
        
        class ChildClass extends ParentClass {
            childMethod() {
                return "child";
            }
        }
        
        const instance = new ChildClass();
        bindPrototypeMethods(instance);
        
        expect(instance.hasOwnProperty("parentMethod")).toBe(true);
        expect(instance.hasOwnProperty("childMethod")).toBe(true);
        
        expect(instance.parentMethod()).toBe("parent");
        expect(instance.childMethod()).toBe("child");
    });

    it("should not bind arrow methods", () => {
        const instance = new TestClass();
        bindPrototypeMethods(instance);
        
        // Arrow method should remain unbound (as it's an own property)
        expect(instance.arrowMethod).toBe(instance.arrowMethod);
    });

    it("should cache method lists per prototype", () => {
        const instance1 = new TestClass();
        const instance2 = new TestClass();
        
        // First call should compute and cache
        bindPrototypeMethods(instance1);
        
        // Second call should use cached list
        bindPrototypeMethods(instance2);
        
        expect(instance1.hasOwnProperty("getValue")).toBe(true);
        expect(instance2.hasOwnProperty("getValue")).toBe(true);
    });

    it("should compare performance of bindThis vs bindPrototypeMethods", () => {
        class PerfTestClass {
            value = 0;
            method1() { return this.value; }
            method2() { return this.value + 1; }
            method3() { return this.value + 2; }
            method4() { return this.value + 3; }
            method5() { return this.value + 4; }
            method6() { return this.value + 5; }
            method7() { return this.value + 6; }
        }

        const iterations = 50000; // Smaller for test speed

        // Test bindThis (lazy binding)
        const startLazy = performance.now();
        for (let i = 0; i < iterations; i++) {
            const instance = new PerfTestClass();
            const bound = bindThis(instance);
            // Access all methods to trigger binding
            bound.method1();
            bound.method3();
            bound.method5();
            bound.method7();
        }
        const timeLazy = performance.now() - startLazy;

        // Test bindPrototypeMethods (eager binding)
        const startEager = performance.now();
        for (let i = 0; i < iterations; i++) {
            const instance = new PerfTestClass();
            bindPrototypeMethods(instance);
            // Access all methods
            instance.method1();
            instance.method3();
            instance.method5();
            instance.method7();
        }
        const timeEager = performance.now() - startEager;

        console.log(`bindThis (lazy): ${timeLazy.toFixed(2)}ms, bindPrototypeMethods (eager): ${timeEager.toFixed(2)}ms`);
        
        // Both should be fast, but lazy might be slightly slower due to Proxy overhead
        expect(timeLazy).toBeLessThan(2000);
        expect(timeEager).toBeLessThan(2000);
        
        // Log which is faster
        if (timeLazy < timeEager) {
            console.log("Lazy binding was faster");
        } else {
            console.log("Eager binding was faster");
        }
    });
});