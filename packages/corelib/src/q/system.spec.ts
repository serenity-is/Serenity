import { Decorators } from "../decorators";
import { Enum, coalesce, getStateStore, getType, getTypeFullName, getTypeNameProp, isAssignableFrom, isEnum, isValue, registerClass, registerEnum, registerInterface, today } from "./system";

describe("Q.coalesce", () => {
    it('returns first value if not null', function () {
        expect(coalesce(1, 2)).toBe(1);
    });

    it('returns second value if null', function () {
        expect(coalesce(null, 2)).toBe(2);
        expect(coalesce(undefined, 2)).toBe(2);
    });

    it('returns second if both null', function () {
        expect(coalesce(null, null)).toBe(null);
        expect(coalesce(undefined, null)).toBe(null);
        expect(coalesce(undefined, undefined)).toBe(undefined);
        expect(coalesce(null, undefined)).toBe(undefined);
    });
});

describe("Q.Enum.getValues", () => {
    it('returns correct values', function () {
        enum Test {
            Some = 1,
            Other = 5,
            Another = -1
        }

        expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
    });

    it('returns correct values for registered enums', function () {

        enum Test {
            Some = 1,
            Other = 5,
            Another = -1
        }

        Decorators.registerEnum(Test, 'EnumGetValuesRegisteredEnums.Test')

        expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
    });
});

describe("Q.Enum.toString", () => {
    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }
    
    it('returns names', function() {
        expect(Enum.toString(Test, 1)).toBe("Some");
        expect(Enum.toString(Test, 5)).toBe("Other");
        expect(Enum.toString(Test, -1)).toBe("Another");
    });
    
    it('returns number if non-existent', function() {
        expect(Enum.toString(Test, 0)).toBe("0");
        expect(Enum.toString(Test, -5)).toBe("-5");
    });
});

describe("Q.getStateStore", () => {
    it('if globalThis.Q is null, it can assign it', function () {
        var q = (globalThis as any).Q;
        try {
            delete (globalThis as any).Q;
            expect((globalThis as any).Q).toBeUndefined();
            var stateStore = getStateStore();
            expect((globalThis as any).Q).toBeTruthy();
            expect(stateStore).toBe((globalThis as any).Q.__stateStore);
        }
        finally {
            (globalThis as any).Q = q;
        }
    });

    it('if globalThis.Q is not null, it can create __stateStore', function () {
        var q = (globalThis as any).Q;
        try {
            delete (globalThis as any).Q;
            expect((globalThis as any).Q).toBeUndefined();
            var newQ = (globalThis as any).Q = Object.create(null);
            expect(newQ.__stateStore).toBeUndefined();
            var stateStore = getStateStore();
            expect((globalThis as any).Q).toBeTruthy();
            expect((globalThis as any).Q === newQ).toBe(true);
            expect(typeof (globalThis as any).Q.__stateStore == "object").toBe(true);
            expect(Object.keys((globalThis as any).Q.__stateStore).length).toBe(0);
            expect(stateStore === (globalThis as any).Q.__stateStore).toBe(true);
        }
        finally {
            (globalThis as any).Q = q;
        }
    });

    it('if globalThis.Q.__stateStore is not null, it returns that', function () {
        var q = (globalThis as any).Q;
        try {
            delete (globalThis as any).Q;
            expect((globalThis as any).Q).toBeUndefined();
            var newQ = (globalThis as any).Q = Object.create(null);
            var newStore = newQ.__stateStore = Object.create(null);
            newStore.__myKey = "theKey";
            var stateStore = getStateStore();
            expect((globalThis as any).Q).toBeTruthy();
            expect((globalThis as any).Q === newQ).toBe(true);
            expect(stateStore === newStore).toBe(true);
            expect((globalThis as any).Q.__stateStore === newStore).toBe(true);
            expect(Object.keys((globalThis as any).Q.__stateStore).length).toBe(1);
            expect((globalThis as any).Q.__stateStore.__myKey).toBe("theKey");
        }
        finally {
            (globalThis as any).Q = q;
        }
    });

    it('if a new store key is provided, it auto initializes it to empty object and returns', function () {
        var q = (globalThis as any).Q;
        try {
            delete (globalThis as any).Q;
            var newStore = {};
            (globalThis as any).Q = { __stateStore: newStore };
            var sub = getStateStore("sub");
            expect(typeof sub).toBe("object");
            expect(Object.keys(sub).length).toBe(0);
            expect((globalThis as any).Q.__stateStore === newStore).toBe(true);
            expect(Object.keys(newStore).length).toBe(1);
        }
        finally {
            (globalThis as any).Q = q;
        }
    });

    it('if returns same sub store instance every time', function () {
        var q = (globalThis as any).Q;
        try {
            delete (globalThis as any).Q;
            (globalThis as any).Q = { __stateStore: {} };
            var sub1 = getStateStore("sub");
            sub1.test = "A";
            var sub2 = getStateStore("sub");
            expect(sub1 === sub2).toBe(true);
            expect(sub2.test).toBe("A");
        }
        finally {
            (globalThis as any).Q = q;
        }
    });

    it('if does not return same sub store for different keys', function () {
        var q = (globalThis as any).Q;
        try {
            delete (globalThis as any).Q;
            (globalThis as any).Q = { __stateStore: {} };
            var sub1 = getStateStore("sub1");
            sub1.test = "A";
            var sub2 = getStateStore("sub2");
            expect(sub1 !== sub2).toBe(true);
            expect(sub1.test).toBe("A");
            expect(sub2.test).toBeUndefined();
        }
        finally {
            (globalThis as any).Q = q;
        }
    });
});

namespace Module1 {
    @Decorators.registerInterface("ISome")
    export class ISome {
    }
}

namespace CopyModule1 {
    @Decorators.registerInterface("ISome")
    export class ISome {
    }
}

@Decorators.registerClass("SomeClassUsingCopy1", [Module1.ISome])
class Module1Class {
}

@Decorators.registerClass("SomeClassUsingCopy2", [CopyModule1.ISome])
class CopyModule1Class {
}

describe("Q.isAssignableFrom", () => {
    it("interfaces can also be matched by their registration names", function () {
        expect(isAssignableFrom(Module1.ISome, Module1Class)).toBe(true);
        expect(isAssignableFrom(Module1.ISome, CopyModule1Class)).toBe(true);
        expect(isAssignableFrom(CopyModule1.ISome, Module1Class)).toBe(true);
        expect(isAssignableFrom(CopyModule1.ISome, CopyModule1Class)).toBe(true);
    });

    it("interfaces are matched by their registration names even when class name is different", function () {
        @Decorators.registerInterface("ISome")
        class ISomeMeSome {
        }
        expect(isAssignableFrom(ISomeMeSome, Module1Class)).toBe(true);
        expect(isAssignableFrom(ISomeMeSome, CopyModule1Class)).toBe(true);
    });

    it("interfaces with different class names and registration names won't match", function () {

        @Decorators.registerInterface("IOther")
        class IOther {
        }

        expect(isAssignableFrom(IOther, Module1Class)).toBe(false);
        expect(isAssignableFrom(IOther, CopyModule1Class)).toBe(false);
    });

    it("interfaces with same class names but different registration names won't match", function () {

        @Decorators.registerInterface("ISomeDiff")
        class ISome {
        }

        @Decorators.registerClass("X", [ISome])
        class X {
        }

        expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
    });

    it("classes that are not registered as interfaces won't match", function () {

        @Decorators.registerClass("ISome")
        class ISome {
        }

        @Decorators.registerClass("X", [ISome])
        class X {
        }

        expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
    });
});

describe("Q.isValue", () => {
    it('isValue returns false for null or undefined', function () {
        expect(isValue(null)).toBe(false);
        expect(isValue(undefined)).toBe(false);
    });

    it('isValue returns true for non null or undefined', function () {
        expect(isValue(true)).toBe(true);
        expect(isValue(false)).toBe(true);
        expect(isValue("")).toBe(true);
    });
});

describe("Q.registerClass", () => {
    function expectClassDetails(klass: any, name: string, intf?: any[]) {
        expect(isEnum(klass)).toBe(false);

        if (intf != null) {
            expect(klass.__interfaces).toStrictEqual(intf);
        }
        else
            expect(klass.__interfaces).toBeUndefined();

        expect(klass.__metadata).toBeUndefined();
        expect(klass.__interface).toBe(false);

        if (name != null) {
            expect(getTypeFullName(klass)).toBe(name);
            expect(getTypeNameProp(klass)).toBe(name);

            expect(getType(name)).toStrictEqual(klass);
        }
        else {
            var fullName = getTypeFullName(klass);
            expect(fullName).toBe(klass.name);
            expect(getTypeNameProp(klass)).toBeUndefined();

            expect(getType(fullName) == null).toBe(true);
        }

        expect(klass.__isAssignableFrom).toBeUndefined();
    }

    it('works with no name', function () {

        class Test {
        }

        const name = 'Test_Class_NoName';
        registerClass(Test, null);
        expectClassDetails(Test, null);
    });

    it('works with with name', function () {

        class Test {
        }

        const name = 'Test_Class_NoKind';
        registerClass(Test, name);
        expectClassDetails(Test, name);
    });

    it('works with no name with interfaces', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class Test {
        }

        registerClass(Test, null, [Intf1, Intf2]);
        expectClassDetails(Test, null, [Intf1, Intf2]);
    });

    it('works with name and interfaces', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class Test {
        }

        const name = 'Test_Class_With_Name_And_Interfaces';
        registerClass(Test, name, [Intf1, Intf2]);
        expectClassDetails(Test, name, [Intf1, Intf2]);
    });

    it('works with derived class with null interface list null derives interfaces as is', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class Test {
        }

        class Derived extends Test {
        }

        const nameTest = 'Test_DerivedClassNullIntfDerivesInterfacesAsIs';
        const nameDerived = 'Derived_DerivedClassNullIntfDerivesInterfacesAsIs';
        registerClass(Test, nameTest, [Intf1, Intf2]);
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);

        registerClass(Derived, nameDerived, null);
        expectClassDetails(Derived, nameDerived, [Intf1, Intf2]);
        // should be the same reference
        expect((Derived as any).__interfaces === (Test as any).__interfaces).toBe(true);
    });

    it('works with derived class with empty interface list derives interfaces as a copy', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class Test {
        }

        class Derived extends Test {
        }

        const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsACopy';
        const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsACopy';
        registerClass(Test, nameTest, [Intf1, Intf2]);
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);

        registerClass(Derived, nameDerived, []);
        expectClassDetails(Derived, nameDerived, [Intf1, Intf2]);
        // should be the same reference
        expect((Derived as any).__interfaces === (Test as any).__interfaces).toBe(true);
    });

    it('works with derived class with extra interface list derives interfaces as a copy', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class Test {
        }

        class Intf3 {
        }

        class Derived extends Test {
        }

        const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsCopy';
        const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsCopy';
        registerClass(Test, nameTest, [Intf1, Intf2]);
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);

        registerClass(Derived, nameDerived, [Intf3]);
        expectClassDetails(Derived, nameDerived, [Intf1, Intf2, Intf3]);

        // should not be the same reference
        expect((Derived as any).__interfaces !== (Test as any).__interfaces).toBe(true);

        // check base class again to make sure its interfaces are not modified
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);
    });
});

describe("Q.registerEnum", () => {
    function expectTypeDetails(enumObj: any, name: string) {
        expect(isEnum(enumObj)).toBe(true);
        expect(enumObj.__interface).toBeNull();

        expect(enumObj.__interfaces).toBeUndefined();
        expect(enumObj.__metadata).toBeUndefined();

        if (name != null) {
            expect(getTypeFullName(enumObj)).toBe(name);
            expect(enumObj.__typeName).toBe(name);
            expect(enumObj.__typeName$).toBe(name);
            expect(getType(name)).toStrictEqual(enumObj);
        }
        else {
            var fullName = getTypeFullName(enumObj);
            expect(fullName).toBe('Object');
            expect(enumObj.__typeName).toBeUndefined();
            expect(enumObj.__typeName$).toBeUndefined();
            expect(getType(fullName) == null).toBe(true);
        }

        expect(enumObj.__isAssignableFrom).toBeUndefined();
    }

    it('works with no name', function () {

        enum Test {
            A = 1,
            B = 2
        }

        const name = 'Test_Enum_NoName';
        registerEnum(Test, null);
        expectTypeDetails(Test, null);
    });

    it('works with with name', function () {

        enum Test {
            A = 1,
            B = 2
        }

        const name = 'Test_Enum_With_Name';
        registerEnum(Test, name);
    });
});

describe("Q.registerInterface", () => {
    function expectTypeDetails(klass: any, name: string, intf?: any[]) {
        expect(isEnum(klass)).toBe(false);

        if (intf != null) {
            expect(klass.__interfaces).toStrictEqual(intf);
        }
        else
            expect(klass.__interfaces).toBeUndefined();

        expect(klass.__metadata).toBeUndefined();
        expect(klass.__interface).toBe(true);

        if (name != null) {
            expect(getTypeFullName(klass)).toBe(name);
            expect(getTypeNameProp(klass)).toBe(name);

            expect(getType(name)).toStrictEqual(klass);
        }
        else {
            var fullName = getTypeFullName(klass);
            expect(fullName).toBe(klass.name);
            expect(getTypeNameProp(klass));

            expect(getType(fullName) == null).toBe(true);
        }

        expect(typeof klass.__isAssignableFrom).toBe("function");
    }

    it('works with no name', function () {

        class ITest {
        }

        registerInterface(ITest, null);
        expectTypeDetails(ITest, null);
    });

    it('works with with name', function () {

        class ITest {
        }

        const name = 'ITest_Interface_NoKind';
        registerInterface(ITest, name);
        expectTypeDetails(ITest, name);
    });

    it('works with no name with interfaces', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class ITest {
        }

        registerInterface(ITest, null, [Intf1, Intf2]);
        expectTypeDetails(ITest, null, [Intf1, Intf2]);
    });

    it('works with name and interfaces', function () {

        class Intf1 {
        }

        class Intf2 {
        }

        class ITest {
        }

        const name = 'Test_Interface_With_Name_And_Interfaces';
        registerInterface(ITest, name, [Intf1, Intf2]);
        expectTypeDetails(ITest, name, [Intf1, Intf2]);
    });

    it('works with derived class with null interface list null derives interfaces as is', function () {

        // NOTE: interfaces are not supposed to be derived

        class Intf1 {
        }

        class Intf2 {
        }

        class ITest {
        }

        class IDerived extends ITest {
        }

        const nameTest = 'Test_DerivedClassNullIntfDerivesInterfacesAsIs';
        const nameDerived = 'Derived_DerivedClassNullIntfDerivesInterfacesAsIs';
        registerInterface(ITest, nameTest, [Intf1, Intf2]);
        expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);

        registerInterface(IDerived, nameDerived, null);
        expectTypeDetails(IDerived, nameDerived, [Intf1, Intf2]);
        // should be the same reference
        expect((IDerived as any).__interfaces === (ITest as any).__interfaces).toBe(true);
    });

    it('works with derived class with empty interface list derives interfaces as a copy', function () {

        // NOTE: interfaces are not supposed to be derived classes
        // just testing copy of __interfaces here

        class Intf1 {
        }

        class Intf2 {
        }

        class ITest {
        }

        class IDerived extends ITest {
        }

        const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsACopy';
        const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsACopy';
        registerInterface(ITest, nameTest, [Intf1, Intf2]);
        expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);

        registerInterface(IDerived, nameDerived, []);
        expectTypeDetails(IDerived, nameDerived, [Intf1, Intf2]);
        // should be the same reference
        expect((IDerived as any).__interfaces === (ITest as any).__interfaces).toBe(true);
    });

    it('works with derived class with extra interface list derives interfaces as a copy', function () {

        // NOTE: interfaces are not supposed to be derived classes
        // just testing copy of __interfaces here

        class Intf1 {
        }

        class Intf2 {
        }

        class ITest {
        }

        class Intf3 {
        }

        class IDerived extends ITest {
        }

        const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsCopy';
        const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsCopy';
        registerInterface(ITest, nameTest, [Intf1, Intf2]);
        expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);

        registerInterface(IDerived, nameDerived, [Intf3]);
        expectTypeDetails(IDerived, nameDerived, [Intf1, Intf2, Intf3]);

        // should not be the same reference
        expect((IDerived as any).__interfaces !== (ITest as any).__interfaces).toBe(true);

        // check base class again to make sure its interfaces are not modified
        expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);
    });
});

describe("Q.today", () => {
    it('returns a date without time part', function () {
        var d = today();
        expect(d.getHours()).toBe(0);
        expect(d.getMinutes()).toBe(0);
        expect(d.getSeconds()).toBe(0);
        expect(d.getMilliseconds()).toBe(0);
    });
});