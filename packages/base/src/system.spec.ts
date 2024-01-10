import { implementedInterfacesSymbol, isAssignableFromSymbol, isInstanceOfTypeSymbol, isInterfaceTypeSymbol } from "./symbols";
import { Enum, addCustomAttribute, classTypeInfo, fieldsProxy, getBaseType, getCustomAttribute, getCustomAttributes, getInstanceType, getType, getTypeFullName, getTypeNameProp, getTypeShortName, initFormType, isAssignableFrom, isEnum, isInstanceOfType, registerClass, registerEnum, registerInterface, typeInfoProperty } from "./system";
import { ensureTypeInfo, peekTypeInfo } from "./system-internal";

describe("Enum.getValues", () => {
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

        registerEnum(Test, 'EnumGetValuesRegisteredEnums.Test')

        expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
    });
});

describe("Enum.toString", () => {
    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }

    it('returns empty string for null', () => {
        expect(Enum.toString(Test, null)).toBe("");
        expect(Enum.toString(Test, undefined)).toBe("");
    });

    it('returns the value as string if value is not a number', () => {
        expect(Enum.toString(Test, "test" as any)).toBe("test");
        expect(Enum.toString(Test, "0" as any)).toBe("0");
    });

    it('returns names', function () {
        expect(Enum.toString(Test, 1)).toBe("Some");
        expect(Enum.toString(Test, 5)).toBe("Other");
        expect(Enum.toString(Test, -1)).toBe("Another");
    });

    it('returns number if non-existent', function () {
        expect(Enum.toString(Test, 0)).toBe("0");
        expect(Enum.toString(Test, -5)).toBe("-5");
    });

    it('works with flags', () => {
        enum Test2 {
            Some = 1,
            Other = 2,
            Another = 4
        }

        ensureTypeInfo(Test2 as any).enumFlags = true;
        expect(Enum.toString(Test2, Test2.Some + Test2.Another)).toBe("Some | Another");
        expect(Enum.toString(Test2, Test2.Some + Test2.Another + 16)).toBe("Some | Another | 16");
    });
});

namespace Module1 {
    export class ISome { }
    registerInterface(ISome, "ISome");
}

namespace CopyModule1 {
    export class ISome { }
    registerInterface(ISome, "ISome");
}

class Module1Class { }
registerClass(Module1Class, "SomeClassUsingCopy1", [Module1.ISome]);

class CopyModule1Class { }
registerClass(CopyModule1Class, "SomeClassUsingCopy2", [CopyModule1.ISome]);

describe("isAssignableFrom", () => {

    it("interfaces can also be matched by their registration names", function () {
        expect(isAssignableFrom(Module1.ISome, Module1Class)).toBe(true);
        expect(isAssignableFrom(Module1.ISome, CopyModule1Class)).toBe(true);
        expect(isAssignableFrom(CopyModule1.ISome, Module1Class)).toBe(true);
        expect(isAssignableFrom(CopyModule1.ISome, CopyModule1Class)).toBe(true);
    });

    it("interfaces are matched by their registration names even when class name is different", function () {
        class ISomeMeSome { }
        registerInterface(ISomeMeSome, "ISome")
        expect(isAssignableFrom(ISomeMeSome, Module1Class)).toBe(true);
        expect(isAssignableFrom(ISomeMeSome, CopyModule1Class)).toBe(true);
    });

    it("interfaces with different class names and registration names won't match", function () {
        class IOther { }
        registerInterface(IOther, "IOther")
        expect(isAssignableFrom(IOther, Module1Class)).toBe(false);
        expect(isAssignableFrom(IOther, CopyModule1Class)).toBe(false);
    });

    it("interfaces with same class names but different registration names won't match", function () {
        class ISome { }
        registerInterface(ISome, "ISomeDiff")
        class X { }
        registerClass(X, "X", [ISome])
        expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
    });

    it("classes that are not registered as interfaces won't match", function () {
        class ISome { }
        registerClass(ISome, "ISome")
        class X { }
        registerClass(X, "X", [ISome])
        expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
    });

    it("returns true for same class", function () {
        expect(isAssignableFrom(String, String)).toBe(true);
    });

    it("returns true for sub class", function () {
        class A { }
        class B extends A { }
        expect(isAssignableFrom(A, B)).toBe(true);
        expect(isAssignableFrom(B, A)).toBe(false);
    });

});



describe("registerClass", () => {
    function expectClassDetails(klass: any, name: string, intf?: any[]) {
        expect(isEnum(klass)).toBe(false);

        if (intf != null) {
            expect(klass[implementedInterfacesSymbol]).toStrictEqual(intf);
        }
        else
            expect(klass[implementedInterfacesSymbol]).toBeUndefined();

        expect(peekTypeInfo(klass)?.customAttributes).toBeUndefined();
        expect(peekTypeInfo(klass)?.enumFlags).toBeUndefined();

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

        expect(klass[isAssignableFromSymbol]).toBeUndefined();
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
        expect((Derived as any)[implementedInterfacesSymbol] === (Test as any)[implementedInterfacesSymbol]).toBe(true);
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
        expect((Derived as any)[implementedInterfacesSymbol] === (Test as any)[implementedInterfacesSymbol]).toBe(true);
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
        expect((Derived as any)[implementedInterfacesSymbol] !== (Test as any)[implementedInterfacesSymbol]).toBe(true);

        // check base class again to make sure its interfaces are not modified
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);
    });

    it("works with types that already have a static typeInfo property with typeName", () => {
        class MyClass {
            static [typeInfoProperty] = classTypeInfo("MyClassName");
        }

        registerClass(MyClass, null);
        expect(getType("MyClassName")).toBe(MyClass);
    });
});

describe("registerEnum", () => {
    function expectTypeDetails(enumObj: any, name: string) {
        expect(isEnum(enumObj)).toBe(true);
        expect(enumObj[isInterfaceTypeSymbol]).toBeNull();

        expect(enumObj[implementedInterfacesSymbol]).toBeUndefined();
        expect(peekTypeInfo(enumObj)?.enumFlags).toBeUndefined();
        expect(enumObj[implementedInterfacesSymbol]).toBeUndefined();

        if (name != null) {
            expect(getTypeFullName(enumObj)).toBe(name);
            expect(enumObj[fullName]).toBe(name);
            expect(getType(name)).toStrictEqual(enumObj);
        }
        else {
            var fullName = getTypeFullName(enumObj);
            expect(fullName).toBe('Object');
            expect(enumObj[fullName]).toBeUndefined();
            expect(getType(fullName) == null).toBe(true);
        }

        expect(enumObj[isAssignableFromSymbol]).toBeUndefined();
    }

    it('works with no name', function () {

        enum Test {
            A = 1,
            B = 2
        }

        registerEnum(Test, null);
        expectTypeDetails(Test, null);
    });

    it('works with name', function () {

        enum Test {
            A = 1,
            B = 2
        }

        const name = 'Test_Enum_With_Name';
        registerEnum(Test, name);
    });

    it('works with enum key', function () {

        enum Test {
            A = 1,
            B = 2
        }

        const name = 'Test_Enum_Name';
        const key = 'Test_Enum_Key';
        registerEnum(Test, name, key);
        expect(getType(name)).toBe(Test);
        expect(getType(key)).toBe(Test);
    });
});

describe("registerInterface", () => {
    function expectTypeDetails(klass: any, name: string, intf?: any[]) {
        expect(isEnum(klass)).toBe(false);

        if (intf != null) {
            expect(klass[implementedInterfacesSymbol]).toStrictEqual(intf);
        }
        else
            expect(klass[implementedInterfacesSymbol]).toBeUndefined();

        expect(peekTypeInfo(klass)?.enumFlags).toBeUndefined();
        expect(klass[isInterfaceTypeSymbol]).toBe(true);

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

        expect(typeof klass[isAssignableFromSymbol]).toBe("function");
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
        expect((IDerived as any)[implementedInterfacesSymbol] === (ITest as any)[implementedInterfacesSymbol]).toBe(true);
    });

    it('works with derived class with empty interface list derives interfaces as a copy', function () {

        // NOTE: interfaces are not supposed to be derived classes
        // just testing copy of [implementedInterfacesSymbol] here

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
        expect((IDerived as any)[implementedInterfacesSymbol] === (ITest as any)[implementedInterfacesSymbol]).toBe(true);
    });

    it('works with derived class with extra interface list derives interfaces as a copy', function () {

        // NOTE: interfaces are not supposed to be derived classes
        // just testing copy of [implementedInterfacesSymbol] here

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
        expect((IDerived as any)[implementedInterfacesSymbol] !== (ITest as any)[implementedInterfacesSymbol]).toBe(true);

        // check base class again to make sure its interfaces are not modified
        expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);
    });
});

describe("isInstanceOfType", () => {
    it("returns false if instance is null or undefined", () => {
        expect(isInstanceOfType(null, String)).toBe(false);
        expect(isInstanceOfType(undefined, Object)).toBe(false);
    });

    it("uses isInstanceOfTypeSymbol function if available", () => {
        class Test1 {
            static [isInstanceOfTypeSymbol](type: any) { return typeof type === "string" && type.startsWith("t"); }
        }

        class Test2 {
            static [isInstanceOfTypeSymbol] = true;
        }

        expect(isInstanceOfType("test", Test1)).toBe(true);
        expect(isInstanceOfType("vest", Test1)).toBe(false);
        expect(isInstanceOfType("test", Test2)).toBe(false);
        expect(isInstanceOfType("vest", Test2)).toBe(false);
    });
});

describe("getInstanceType", () => {
    it("throws for null or undefined", () => {
        expect(() => getInstanceType(undefined)).toThrow();
        expect(() => getInstanceType(null)).toThrow();
    });

    it("returns Object if can't read constructor", () => {
        let a = {
            get constructor() { throw "test"; }
        }
        expect(getInstanceType(a)).toBe(Object);
    });
});

describe("getBaseType", () => {
    it("returns null for null, Object, NaN, and interfaces", () => {
        expect(getBaseType(null)).toBeNull();
        expect(getBaseType(undefined)).toBeNull();
        expect(getBaseType(Object)).toBeNull();
        expect(getBaseType(NaN)).toBeNull();

        class ITest {
        }
        registerInterface(ITest, "getBaseType.ITest");
        expect(getBaseType(ITest)).toBeNull();
    });

    it("uses getPrototypeOf for others", () => {

        class Test {
        }

        class Sub extends Test {
        }

        expect(getBaseType(Test)).toBe(Object);
        expect(getBaseType(Sub)).toBe(Test);
    });
});

describe("initFormType", () => {
    it("uses w function", () => {
        class TestForm {
            w(name: string, widget: string) {
                return name + "_" + widget;
            }
        }

        initFormType(TestForm, ["test1", "1", "test2", "2", "test3", "3"]);
        var form = new TestForm();
        expect((form as any).test1).toBe("test1_1");
        expect((form as any).test2).toBe("test2_2");
        expect((form as any).test3).toBe("test3_3");
    });
});

describe("fieldsProxy", () => {
    it("returns same instance every time", () => {
        class Row1Fields {
        }

        class Row2Fields {
        }

        expect(fieldsProxy<Row1Fields>()).toBe(fieldsProxy<Row2Fields>());
    });

    it("returns property name as is", () => {
        class Fields {
            declare A: string;
            declare B: string;
        }

        var proxy = fieldsProxy<Fields>();
        expect(proxy.A).toBe("A");
        expect(proxy.B).toBe("B");

    });
});

describe("getType", () => {
    it("can return type from a root object", () => {
        let root = {
            A: {
                B: class {
                }
            }
        }

        expect(getType("A.B", root)).toBe(root.A.B);
    });
});

describe("getTypeShortName", () => {
    it("returns part after last dot", () => {
        class Test {
        }
        registerClass(Test, "NamespaceOf.C.IsD")

        expect(getTypeShortName(Test)).toBe("IsD");
    });

    it("returns as is if no dot", () => {
        class Test {
        }
        registerClass(Test, "Me")

        expect(getTypeShortName(Test)).toBe("Me");
    });
});

describe("addCustomAttribute", () => {
    it("can add custom attribute", () => {
        class MyAttribute {
        }
        registerClass(MyAttribute, "MyAttribute");
        class MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr);
    });

    it("can add multiple attribute", () => {
        class MyAttribute1 {
        }
        class MyAttribute2 {
        }
        class MyClass {
        }
        const attr1 = new MyAttribute1();
        addCustomAttribute(MyClass, attr1);
        const attr2 = new MyAttribute2();
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttribute(MyClass, MyAttribute1)).toBe(attr1);
        expect(getCustomAttribute(MyClass, MyAttribute2)).toBe(attr2);
    });

    it("can add multiple attribute of same type", () => {
        class MyAttribute {
            constructor(public value: boolean) {
            }
        }
        class MyClass {
        }
        const attr1 = new MyAttribute(false);
        addCustomAttribute(MyClass, attr1);
        const attr2 = new MyAttribute(true);
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr2); // returns last added one
        let attrs = getCustomAttributes(MyClass, MyAttribute);
        expect(attrs?.length).toBe(2);
        expect(attrs[0]).toBe(attr2); // returns last added one first
        expect(attrs[1]).toBe(attr1);
    });
});